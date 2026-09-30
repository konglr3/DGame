namespace Fantasy;

public static class ChatChannelCenterHelper
{
    public static ChatChannelComponent Apply(Scene scene, long channelId)
    {
        return scene.GetComponent<ChatChannelCenterComponent>().Apply(channelId);
    }

    public static bool TryGet(Scene scene, long channelId, out ChatChannelComponent channel)
    {
        return scene.GetComponent<ChatChannelCenterComponent>().TryGet(channelId, out channel);
    }

    public static void Disband(Scene scene, long channelId)
    {
        scene.GetComponent<ChatChannelCenterComponent>().Disband(channelId);
    }
}
