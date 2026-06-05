using application.Ports.Driven.FileStorage;
using Domain.Entities.StorageNodes;

namespace adapters.Driven.Persistence.Repositories.FileStorage
{
    public class FileStorageRepository : IFileStorageRepository
    {
        public async Task<string> SaveAsync(
            StorageNode storageNode,
            Guid fileItemId,
            string fileName,
            Stream content,
            CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName);
            var storedFileName = $"{fileItemId:N}{extension}";
            var relativePath = Path.Combine("files", storedFileName);
            var fullPath = BuildFullPath(storageNode, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            await using var destination = new FileStream(
                fullPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

            await content.CopyToAsync(destination, cancellationToken);

            return relativePath.Replace("\\", "/");
        }

        public Task<Stream?> OpenReadAsync(
            StorageNode storageNode,
            string storedPath,
            CancellationToken cancellationToken = default)
        {
            var fullPath = BuildFullPath(storageNode, storedPath);

            if (!File.Exists(fullPath))
            {
                return Task.FromResult<Stream?>(null);
            }

            Stream stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return Task.FromResult<Stream?>(stream);
        }

        public Task DeleteAsync(
            StorageNode storageNode,
            string storedPath,
            CancellationToken cancellationToken = default)
        {
            var fullPath = BuildFullPath(storageNode, storedPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        private static string BuildFullPath(StorageNode storageNode, string storedPath)
        {
            var basePath = Path.GetFullPath(storageNode.BasePath);

            if (!basePath.EndsWith(Path.DirectorySeparatorChar))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            var normalizedStoredPath = storedPath.Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(basePath, normalizedStoredPath));

            if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Stored file path is outside the storage node base path.");
            }

            return fullPath;
        }
    }
}
