using Fantasy.Entitas.Interface;

namespace Fantasy;

public sealed class ChatUnitDestroySystem : DestroySystem<ChatUnit>
{
    protected override void Destroy(ChatUnit self)
    {
        self.UserName = null!;
        foreach (var (_, chatChannelComponent) in self.Channels)
        {
            chatChannelComponent.ExitChannel(self.Id, false);
        }

        self.Channels.Clear();
        self.SendTime.Clear();
    }
}
