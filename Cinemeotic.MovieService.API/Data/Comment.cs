using CineMeoTic.Common.Models;

namespace Cinemeotic.MovieService.API.Data;

public sealed class Comment : AuditableTimeStamped
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public Guid UserId { get; set; }

    // Threading
    public Guid? ParentCommentId { get; set; }
    public Guid? RootCommentId { get; set; }
    public short Depth { get; set; } = default;
    public int SortOrder { get; set; } = default;

    public string Content { get; set; } = null!;

    // Aggregate counters
    public int ReplyCount { get; set; } = default;        // direct replies
    public int TotalRepliesCount { get; set; } = default; // all descendants
    public bool IsEdited { get; set; } = false;

    public Comment? ParentComment { get; set; }
    public Comment? RootComment { get; set; }
    public ICollection<Comment>? Replies { get; set; }

    public Movie Movie { get; set; } = null!;
}
