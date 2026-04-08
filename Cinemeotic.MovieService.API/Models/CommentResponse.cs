namespace Cinemeotic.MovieService.API.Models;

public sealed record class CommentResponse
{
    public Guid Id { get; init; }
    public Guid MovieId { get; init; }
    public Guid UserId { get; init; }

    public Guid? ParentCommentId { get; init; }
    public Guid? RootCommentId { get; init; }
    public short Depth { get; init; }
    public int SortOrder { get; init; }

    public string Content { get; init; } = null!;

    public int ReplyCount { get; init; }
    public int TotalRepliesCount { get; init; }
    public bool IsEdited { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
