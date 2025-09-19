using learnyx.Models.Enums;

namespace learnyx.Models.Requests;

public class CreateConversationRequest
{
    public ConversationType Type { get; set; }
    public int? User2Id { get; set; }
}