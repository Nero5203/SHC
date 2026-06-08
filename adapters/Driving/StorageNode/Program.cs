var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/node/health", (IConfiguration configuration) =>
{
    var basePath = GetBasePath(configuration);
    Directory.CreateDirectory(basePath);

    var drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(basePath))!);

    return Results.Ok(new
    {
        Status = "Online",
        BasePath = basePath,
        TotalCapacityBytes = drive.TotalSize,
        FreeCapacityBytes = drive.AvailableFreeSpace,
        UsedCapacityBytes = drive.TotalSize - drive.AvailableFreeSpace,
        CheckedAt = DateTime.UtcNow
    });
});

app.MapPost("/api/node/files", async (HttpRequest request, IConfiguration configuration) =>
{
    if (!IsAuthorized(request, configuration))
    {
        return Results.Unauthorized();
    }

    if (!request.HasFormContentType)
    {
        return Results.BadRequest("Expected multipart form data.");
    }

    var form = await request.ReadFormAsync();
    var file = form.Files["file"];
    var fileItemIdValue = form["fileItemId"].FirstOrDefault();
    var fileName = form["fileName"].FirstOrDefault() ?? file?.FileName;

    if (file == null || file.Length == 0)
    {
        return Results.BadRequest("File is required.");
    }

    if (!Guid.TryParse(fileItemIdValue, out var fileItemId))
    {
        return Results.BadRequest("File item id is required.");
    }

    if (string.IsNullOrWhiteSpace(fileName))
    {
        return Results.BadRequest("File name is required.");
    }

    var extension = Path.GetExtension(fileName);
    var storedFileName = $"{fileItemId:N}{extension}";
    var relativePath = Path.Combine("files", storedFileName);
    var fullPath = BuildFullPath(configuration, relativePath);

    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

    await using var destination = new FileStream(
        fullPath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None);

    await file.CopyToAsync(destination);

    return Results.Ok(new
    {
        StoredPath = relativePath.Replace("\\", "/")
    });
});

app.MapGet("/api/node/files", (HttpRequest request, IConfiguration configuration) =>
{
    if (!IsAuthorized(request, configuration))
    {
        return Results.Unauthorized();
    }

    var storedPath = request.Query["storedPath"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(storedPath))
    {
        return Results.BadRequest("Stored path is required.");
    }

    var fullPath = BuildFullPath(configuration, storedPath);

    if (!File.Exists(fullPath))
    {
        return Results.NotFound();
    }

    return Results.File(
        File.OpenRead(fullPath),
        "application/octet-stream",
        Path.GetFileName(fullPath));
});

app.MapDelete("/api/node/files", (HttpRequest request, IConfiguration configuration) =>
{
    if (!IsAuthorized(request, configuration))
    {
        return Results.Unauthorized();
    }

    var storedPath = request.Query["storedPath"].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(storedPath))
    {
        return Results.BadRequest("Stored path is required.");
    }

    var fullPath = BuildFullPath(configuration, storedPath);

    if (File.Exists(fullPath))
    {
        File.Delete(fullPath);
    }

    return Results.NoContent();
});

app.Run();

static bool IsAuthorized(HttpRequest request, IConfiguration configuration)
{
    var expectedKey = configuration["StorageNode:ApiKey"];

    if (string.IsNullOrWhiteSpace(expectedKey))
    {
        return true;
    }

    return request.Headers.TryGetValue("X-Storage-Node-Key", out var suppliedKey) &&
           string.Equals(suppliedKey.ToString(), expectedKey, StringComparison.Ordinal);
}

static string GetBasePath(IConfiguration configuration)
{
    return configuration["StorageNode:BasePath"] ?? "C:\\storage\\node1";
}

static string BuildFullPath(IConfiguration configuration, string storedPath)
{
    var basePath = Path.GetFullPath(GetBasePath(configuration));

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
