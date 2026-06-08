using System.Net.Http.Headers;
using System.Net.Http.Json;
using application.Ports.Driven.FileStorage;
using Domain.Entities.StorageNodes;
using Microsoft.Extensions.Configuration;

namespace adapters.Driven.ExternalServices.FileStorage
{
    public class RemoteFileStorageRepository : IFileStorageRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RemoteFileStorageRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> SaveAsync(
            StorageNode storageNode,
            Guid fileItemId,
            string fileName,
            Stream content,
            CancellationToken cancellationToken = default)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(fileItemId.ToString()), "fileItemId");
            form.Add(new StringContent(fileName), "fileName");

            var fileContent = new StreamContent(content);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            form.Add(fileContent, "file", fileName);

            using var request = CreateRequest(storageNode, HttpMethod.Post, "/api/node/files");
            request.Content = form;

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<SaveFileResponse>(cancellationToken: cancellationToken);

            if (string.IsNullOrWhiteSpace(payload?.StoredPath))
            {
                throw new InvalidOperationException("Storage node did not return a stored file path.");
            }

            return payload.StoredPath;
        }

        public async Task<Stream?> OpenReadAsync(
            StorageNode storageNode,
            string storedPath,
            CancellationToken cancellationToken = default)
        {
            var path = $"/api/node/files?storedPath={Uri.EscapeDataString(storedPath)}";
            var request = CreateRequest(storageNode, HttpMethod.Get, path);
            var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            request.Dispose();

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                response.Dispose();
                return null;
            }

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return new HttpResponseStream(stream, response);
        }

        public async Task DeleteAsync(
            StorageNode storageNode,
            string storedPath,
            CancellationToken cancellationToken = default)
        {
            using var request = CreateRequest(
                storageNode,
                HttpMethod.Delete,
                $"/api/node/files?storedPath={Uri.EscapeDataString(storedPath)}");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private HttpRequestMessage CreateRequest(StorageNode storageNode, HttpMethod method, string path)
        {
            var request = new HttpRequestMessage(method, BuildNodeUri(storageNode, path));
            var apiKey = _configuration["StorageNodes:ApiKey"];

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Add("X-Storage-Node-Key", apiKey);
            }

            return request;
        }

        private static Uri BuildNodeUri(StorageNode storageNode, string path)
        {
            var host = storageNode.IpAddress.Trim();
            var baseHost = host.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                           host.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                ? host
                : $"http://{host}";

            var pathParts = path.Split('?', 2);
            var builder = new UriBuilder(baseHost)
            {
                Port = storageNode.Port,
                Path = pathParts[0].TrimStart('/')
            };

            if (pathParts.Length == 2)
            {
                builder.Query = pathParts[1];
            }

            return builder.Uri;
        }

        private sealed class SaveFileResponse
        {
            public string StoredPath { get; set; } = "";
        }

        private sealed class HttpResponseStream : Stream
        {
            private readonly Stream _inner;
            private readonly HttpResponseMessage _response;

            public HttpResponseStream(Stream inner, HttpResponseMessage response)
            {
                _inner = inner;
                _response = response;
            }

            public override bool CanRead => _inner.CanRead;
            public override bool CanSeek => _inner.CanSeek;
            public override bool CanWrite => _inner.CanWrite;
            public override long Length => _inner.Length;

            public override long Position
            {
                get => _inner.Position;
                set => _inner.Position = value;
            }

            public override void Flush()
            {
                _inner.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _inner.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _inner.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _inner.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _inner.Write(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _inner.Dispose();
                    _response.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
