public class UserValidationMessage : BaseMessage
{
    public int UserId { get; set; }
    public string CorrelationId { get; set; }
    public string ReplyTo { get; set; }
}