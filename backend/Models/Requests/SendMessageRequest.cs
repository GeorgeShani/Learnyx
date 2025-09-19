using learnyx.Models.DTOs;

namespace learnyx.Models.Requests;

public class SendMessageRequest
{
    public string? TextContent { get; set; }
    public List<MessageContentDTO>? Contents { get; set; }
    public int? ReplyToMessageId { get; set; }
}