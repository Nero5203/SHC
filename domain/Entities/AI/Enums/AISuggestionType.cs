namespace Domain.Entities.AI.Enums
{
    public enum AISuggestionType
    {
        // Activity
        ContinueWorking,
        FrequentlyUsed,
        RecommendedFile,

        // Cleanup
        DuplicateFile,
        DeleteFile,
        StorageCleanup,

        // Organization
        RenameFile,
        MoveFile,
        AddTags,

        // Security
        ReviewSharedLink
    }
}
