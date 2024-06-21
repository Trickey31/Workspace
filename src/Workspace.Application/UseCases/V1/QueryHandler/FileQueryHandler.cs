using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Linq.Expressions;
using Workspace.Contract;
using Workspace.Domain;
using Workspace.Persistence;

namespace Workspace.Application
{
    public class FileQueryHandler : IQueryHandler<GetFilesQuery, List<FileQueryResponse>>,
                                    IQueryHandler<DownloadFileQuery, IActionResult>
    {
        private readonly IRepositoryBase<Files, Guid> _fileRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileQueryHandler(IRepositoryBase<Files, Guid> fileRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _fileRepository = fileRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResult<List<FileQueryResponse>>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Files, bool>> filter = c =>
                     c.IsDelete == Constants.IS_DELETE && c.ObjKey == request.ObjKey;

            if(request.ObjId != null)
            {
                filter = filter.And(x => x.ObjId == request.ObjId);
            }

            var list = _fileRepository.FindAll(filter).OrderBy(x => x.CreatedDate).ToList();

            var response = _mapper.Map<List<FileQueryResponse>>(list);

            return response;
        }

        public async Task<TResult<IActionResult>> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
        {
            var entity = await _fileRepository.FindByIdAsync(request.Id);

            if (entity == null || entity.IsDelete == Constants.DELETED)
            {
                throw new NotFoundException("Record not found!!!");
            }

            if (!System.IO.File.Exists(entity.Link))
            {
                throw new NotFoundException("File not found!!!");
            }

            var contentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
            if (provider.TryGetContentType(entity.Link, out var outContentType))
            {
                contentType = outContentType;
            }

            var fileBytes = await File.ReadAllBytesAsync(entity.Link);

            _httpContextAccessor.HttpContext.Response.Headers.Append("Content-Disposition", "attachment; filename=" + entity.Name);

            var fileResult = new FileContentResult(fileBytes, contentType);
            return Result.Success<IActionResult>(fileResult);
        }
    }
}
