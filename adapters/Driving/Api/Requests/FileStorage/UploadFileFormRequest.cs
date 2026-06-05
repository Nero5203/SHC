using Microsoft.AspNetCore.Http;

namespace api.Requests.FileStorage
{
    public class UploadFileFormRequest
    {
        public Guid UserId { get; set; }
        public Guid? FolderId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
