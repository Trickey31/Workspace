using AutoMapper;
using Workspace.Contract;
using Workspace.Domain;
using static Workspace.Contract.TaskResponse;

namespace Workspace.Application
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile() 
        {
            #region ============ MAP ENTITY TO RESPONSE ============

            CreateMap<Tasks, TaskResponse>().ReverseMap();
            CreateMap<PagedResult<Tasks>, PagedResult<TaskResponse>>().ReverseMap();

            CreateMap<Project, ProjectResponse>().ReverseMap();
            CreateMap<PagedResult<Project>, PagedResult<ProjectResponse>>().ReverseMap();

            CreateMap<CTerm, CTermResponse>().ReverseMap();

            CreateMap<Files, FileQueryResponse>().ReverseMap();

            CreateMap<Logs, LogResponse>().ReverseMap();
            CreateMap<Notification, NotificationResponse>().ReverseMap();
            CreateMap<Comment, CommentResponse>().ReverseMap();
            CreateMap<User, UserResponse>().ReverseMap();

            #endregion

            #region ============ MAP COMMAND TO ENTITY ============

            CreateMap<CreateTaskCommand, Tasks>().ReverseMap();
            CreateMap<UpdateTaskCommand, Tasks>().ReverseMap();
            CreateMap<CreatePersonalTaskCommand, Tasks>().ReverseMap();
            CreateMap<UpdatePersonalTaskCommand, Tasks>().ReverseMap();



            CreateMap<CreateProjectCommand, Project>().ReverseMap();
            CreateMap<UpdateProjectCommand, Project>().ReverseMap();

            CreateMap<CreateUserCommand, User>().ReverseMap();

            CreateMap<CreateCTermCommand, CTerm>().ReverseMap();
            CreateMap<UpdateCTermCommand, CTerm>().ReverseMap();

            CreateMap<CreateCommentCommand, Comment>().ReverseMap();

            #endregion
        }
    }
}
