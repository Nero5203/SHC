namespace Domain.Entities.User.Settings
{
    public class StorageSettings
    {
        //upload behavior
        public string UploadStrategy { get; set; } = "Balanced";
        public bool UploadInChunks { get; set; } = true;
        public int ChunkSizeKB { get; set; } = 1024;
        public bool ResumeInterruptedUploads { get; set; } = true;

        //performance
        public bool PreferFastUploads { get; set; } = true;
        public int MaxConcurrentUploads { get; set; } = 3;
        public int MaxConcurrentDownloads { get; set; } = 3;

        //network awareness
        public bool SyncOnlyOnWiFi { get; set; } = false;
        public bool AutoSyncEnabled { get; set; } = true;
        public int SyncIntervalSeconds { get; set; } = 30;

        //file handling
        public string ConflictResolutionStrategy { get; set; } = "KeepBoth";
        public bool AutoRenameDuplicates { get; set; } = true;
        public bool EnableCompression { get; set; } = false;
        public bool EnableVersioning { get; set; } = false;
    }
}