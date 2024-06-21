using MediatR;
using Newtonsoft.Json;
using System.Data;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class FileCommandHandler : ICommandHandler<CreateFileCommand, FileCommandResponse>,
                                      ICommandHandler<DeleteFileCommand>
    {
        private readonly IRepositoryBase<Files, Guid> _fileRepository;
        private readonly ILogService _logService;

        public FileCommandHandler(IRepositoryBase<Files, Guid> fileRepository, ILogService logService)
        {
            _fileRepository = fileRepository;
            _logService = logService;
        }

        public async Task<TResult<FileCommandResponse>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
        {
            var entity = new Files();

            string fileName = "";
            string filePath = "";
            string folderName = request.ObjKey.ToString();

            var extractPath = $"wwwroot/ObjKey_{request.ObjKey}/{request.ObjId}";

            // Kiểm tra sự tồn tại của thư mục
            if (!Directory.Exists(extractPath))
            {
                // Tạo thư mục nếu không tồn tại
                Directory.CreateDirectory(extractPath);
            }

            if (request.File != null && request.File.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetFileName(request.File.FileName);
                filePath = extractPath + "/" + fileName;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    request.File.CopyTo(stream);
                }
            }
            else
            {
                return Result.Failure<FileCommandResponse>(new Error("400", "File invalid"));
            }

            entity.Name = Path.GetFileName(request.File.FileName);
            entity.Link = filePath;
            entity.ObjId = request.ObjId;
            entity.ObjKey = request.ObjKey;
            entity.CreatedDate = DateTime.Now.ToUniversalTime();
            entity.IsDelete = Constants.IS_DELETE;

            _fileRepository.Add(entity);

            await _logService.CreateLog("POST", "Add new file", "FILE", null, JsonConvert.SerializeObject(entity), entity.Id);

            return Result.Success(new FileCommandResponse(entity.Link));
        }

        public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var entity = await _fileRepository.FindByIdAsync(request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                return Result.Failure(new Error("400", "Record not found"));
            }

            var oldEntity = JsonConvert.SerializeObject(entity);

            entity.UpdatedDate = DateTime.Now.ToUniversalTime();
            entity.IsDelete = Constants.DELETED;

            var filePath = entity.Link;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _fileRepository.Update(entity);

            await _logService.CreateLog("DELETE", "Delete file", "FILE", oldEntity, JsonConvert.SerializeObject(entity), entity.Id);


            return Result.Success();
        }
    }
}
