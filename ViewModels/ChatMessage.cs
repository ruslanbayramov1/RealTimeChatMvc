namespace ChatSignalR.ViewModels;

public class ChatMessage
{
    public string User { get; set; }
    public string Message { get; set; }
    public string ProfileImageUrl { get; set; }
    public DateTime SendedAt { get; set; }
}
