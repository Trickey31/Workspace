using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class UserCommandHandler : ICommandHandler<CreateUserCommand>,
                                      ICommandHandler<AuthenCommand.Login, AuthResponse.Authenticated>,
                                      ICommandHandler<UpdateUserCommand>,
                                      ICommandHandler<ChangePasswordCommand>,
                                      ICommandHandler<DeleteUserCommand>,
                                      ICommandHandler<DeleteUserInProjectCommand>,
                                      ICommandHandler<ResetPasswordCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        private readonly IRepositoryBase<Users_Projects, Guid> _userProjectRepository;
        private readonly IEmailService _emailService;

        public UserCommandHandler(UserManager<User> userManager,
                                  IMapper mapper, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService,
                                  ApplicationDbContext context, IUserService userService, ILogService logService, IRepositoryBase<Users_Projects, Guid> userProjectRepository, IEmailService emailService) 
        {
            _userManager = userManager;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _context = context;
            _userService = userService;
            _logService = logService;
            _userProjectRepository = userProjectRepository;
            _emailService = emailService;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var email = await _userManager.FindByEmailAsync(request.Email);

            if(email != null)
            {
                return Result.Failure(new Error("400", "Email is existed"));
            }

            var user = _mapper.Map<User>(request);

            user.Id = Guid.NewGuid();
            user.UserName = CommonExtensions.ConvertEmailToUserName(request.Email);
            user.ImgLink = "wwwroot/ObjKey_3/useravatar(7).png";

            var result = await _userManager.CreateAsync(user, request.Password);

            if(result.Succeeded)
            {
                await _context.SaveChangesAsync();

                var roleId = await _context.Roles
                                   .Where(r => r.Name == "User")
                                   .Select(r => r.Id)
                                   .FirstOrDefaultAsync();

                if(roleId == null)
                {
                    return Result.Failure(new Error("500", "Unsuccessful"));
                }

                var userRole = new IdentityUserRole<Guid>
                {
                    UserId = user.Id,
                    RoleId = roleId
                };

                _context.UserRoles.Add(userRole);

                await _context.SaveChangesAsync();

                await _logService.CreateLog("POST", "Create new user", "USER", null,JsonConvert.SerializeObject(user), user.Id);

                return Result.Success();
            } else
            {
                return Result.Failure(new Error("500", "Unsuccessful"));
            }
        }

        public async Task<TResult<AuthResponse.Authenticated>> Handle(AuthenCommand.Login request, CancellationToken cancellationToken)
        {

            // Check User
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null)
            {
                return Result.Failure<AuthResponse.Authenticated>(new Error("400", "Email not found"));
            }

            var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                return Result.Failure<AuthResponse.Authenticated>(new Error("400", "Password incorrect"));
            }

            var role = await _userManager.GetRolesAsync(user);

            // Generate JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, role[0])
            };

            var accessToken = _jwtTokenService.GenerateAccessToken(claims);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            //await _userManager.SetAuthenticationTokenAsync(user,)

            var response = new AuthResponse.Authenticated()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
            };

            return Result.Success(response);
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                return Result.Failure(new Error("404", "User not found"));
            }

            var oldEntity = JsonConvert.SerializeObject(user);

            user.Name = request.Name;
            user.UserName = request.UserName;
            user.PhoneNumber = request.PhoneNumber;
            user.ImgLink = request.ImgLink;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _logService.CreateLog("PUT", "Update user", "USER", oldEntity, JsonConvert.SerializeObject(user), user.Id);

                return Result.Success();
            }
            else
            {
                return Result.Failure(new Error("500", "Update user failed"));
            }
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            if (user.Value == null)
            {
                return Result.Failure(new Error("401", "Unauthorized"));
            }

            var oldEntity = JsonConvert.SerializeObject(user.Value);

            if(request.OldPassword == request.NewPassword)
            {
                return Result.Failure(new Error("400", "New Password and Current Password can not equal"));
            }

            if(request.NewPassword != request.ConfirmPassword)
            {
                return Result.Failure(new Error("400", "Confirm Password invalid"));
            }

            var result = await _userManager.ChangePasswordAsync(user.Value, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                return Result.Failure(new Error("400", "Failed to change password"));
            }

            await _logService.CreateLog("PUT", "Change password", "USER", oldEntity, JsonConvert.SerializeObject(user.Value), user.Value.Id);

            return Result.Success();
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if(user == null)
            {
                return Result.Failure(new Error("400", "User not found"));
            }

            await _userManager.DeleteAsync(user);

            return Result.Success();
        }

        public async Task<Result> Handle(DeleteUserInProjectCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetCurrentUserAsync();

            var userProject = await _userProjectRepository.FindSingleAsync(x => x.UserId == user.Value.Id && x.ProjectId == request.ProjectId);

            if (userProject == null || userProject.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", "User not found"));
            }

            if (userProject.RoleId != 1)
            {
                return Result.Failure(new Error("400", "Only administrator can remove other user"));
            }

            var entity = await _userProjectRepository.FindSingleAsync(x => x.UserId == request.Id && x.ProjectId == request.ProjectId);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", "User not found"));
            }

            if(entity.RoleId == 1)
            {
                return Result.Failure(new Error("400", "Invalid"));
            }

            entity.IsDelete = Constants.DELETED;
            entity.UpdatedDate = DateTime.Now.ToUniversalTime();

            _userProjectRepository.Update(entity);

            return Result.Success();
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null)
            {
                return Result.Failure(new Error("400", "Email not found"));
            }

            var password = GeneratePassword(8);

            var passwordHasher = new PasswordHasher<User>();

            var passwordHash = passwordHasher.HashPassword(user, password);

            user.PasswordHash = passwordHash;

            await _userManager.UpdateAsync(user);

            await _logService.CreateLog("PUT", "Reset password", "USER", null, JsonConvert.SerializeObject(user), user.Id);

            await _emailService.SendEmailResetPasswordAsync(user.Email, password);

            return Result.Success();
        }

        private static string GeneratePassword(int length)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";
            const string specialChars = "!@#$%^&*()";

            Random random = new Random();

            char[] password = new char[length];

            // Ensure at least one of each required character
            password[0] = upperChars[random.Next(upperChars.Length)];
            password[1] = digitChars[random.Next(digitChars.Length)];
            password[2] = specialChars[random.Next(specialChars.Length)];

            // Fill the rest of the password with random characters from all sets
            string allChars = upperChars + lowerChars + digitChars + specialChars;
            for (int i = 3; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the characters to avoid predictable patterns
            password = password.OrderBy(x => random.Next()).ToArray();

            return new string(password);
        }
    }
}
