using ChatSignalR.ViewModels;

namespace ChatSignalR.Contexts;

public class SharedDb
{
    public static List<ChatUser> Users { get; set; } = new();
    public static List<ChatMessage> Messages { get; set; } = new();
}
