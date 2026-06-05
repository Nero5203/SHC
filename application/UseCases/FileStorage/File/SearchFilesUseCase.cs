using application.Ports.Driven.FileStorage;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.FileStorage;

namespace application.UseCases.FileStorage.File
{
    public class SearchFilesUseCase : ISearchFilesUseCase
    {
        private readonly IFileRepository _fileRepository;

        public SearchFilesUseCase(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<IReadOnlyList<FileItem>> ExecuteAsync(Guid userId, string? query, Guid? folderId)
        {
            return await _fileRepository.SearchAsync(userId, query, folderId);
        }
    }
}
