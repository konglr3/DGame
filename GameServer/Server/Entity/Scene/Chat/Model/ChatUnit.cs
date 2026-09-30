using Fantasy.Entitas;

namespace Fantasy;

public sealed class ChatUnit : Entity
{
    public string UserName;
    public readonly Dictionary<long, ChatChannelComponent> Channels = new();
    public readonly Dictionary<int, long> SendTime = new();
}
