namespace Fantasy;

public static class ChatHelper
{
    /// <summary>
    /// 发送一个聊天消息给 ChatUnit（不能在 ChatScene 中调用）
    /// </summary>
    public static void SendChatMessage(Scene scene, long chatUnitAddress, ChatInfoTree tree)
    {
        if (scene.SceneType == SceneType.Chat)
        {
            Log.Warning("ChatHelper.SendChatMessage: scene is a chat scene.");
            return;
        }

        scene.Send(chatUnitAddress, new Other2Chat_ChatMessage
        {
            ChatInfoTree = tree
        });
    }
}
