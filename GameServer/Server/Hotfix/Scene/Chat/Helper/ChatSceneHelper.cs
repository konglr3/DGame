using Fantasy.Helper;
using Fantasy.Network.Roaming;
using Fantasy.Platform.Net;

namespace Fantasy;

public static class ChatSceneHelper
{
    private const int ChatCD = 1000;
    private const int MaxTextLength = 10;
    private const int MaxShowItemCount = 2;

    /// <summary>
    /// 聊天消息分发入口
    /// </summary>
    public static uint Distribution(ChatUnit chatUnit, ChatInfoTree tree, bool isCheckSendTime = true)
    {
        var result = Condition(chatUnit, tree, isCheckSendTime);
        if (result != 0)
        {
            return result;
        }

        switch ((ChatChannelType)tree.ChatChannelType)
        {
            case ChatChannelType.Broadcast:
                Broadcast(chatUnit.Scene, tree);
                return 0;
            case ChatChannelType.Team:
            case ChatChannelType.Group:
                return Channel(chatUnit, tree);
            case ChatChannelType.Private:
                return Private(chatUnit, tree);
            default:
                return 1;
        }
    }

    private static uint Condition(ChatUnit chatUnit, ChatInfoTree tree, bool isCheckSendTime = true)
    {
        var now = TimeHelper.Now;

        if (isCheckSendTime)
        {
            chatUnit.SendTime.TryGetValue(tree.ChatChannelType, out var sendTime);
            if (now - sendTime < ChatCD)
            {
                return 1;
            }
        }

        var itemCount = 0;
        var chatTextSize = 0;

        foreach (var chatInfoNode in tree.Node)
        {
            switch ((ChatNodeType)chatInfoNode.ChatNodeType)
            {
                case ChatNodeType.Text:
                    chatTextSize += chatInfoNode.Content?.Length ?? 0;
                    break;
                case ChatNodeType.Image:
                    chatTextSize += 5;
                    break;
                case ChatNodeType.OpenUI:
                    chatTextSize += 10;
                    break;
                case ChatNodeType.Item:
                    itemCount++;
                    break;
            }
        }

        if (chatTextSize > MaxTextLength)
        {
            return 2;
        }

        if (itemCount > MaxShowItemCount)
        {
            return 3;
        }

        if (isCheckSendTime)
        {
            chatUnit.SendTime[tree.ChatChannelType] = now;
        }

        return 0;
    }

    private static void Broadcast(Scene scene, ChatInfoTree tree)
    {
        if (tree.Target.Count > 0)
        {
            var chatUnitManageComponent = scene.GetComponent<ChatUnitManageComponent>();
            foreach (var chatUnitId in tree.Target)
            {
                if (!chatUnitManageComponent.TryGet(chatUnitId, out var chatUnit))
                {
                    continue;
                }

                PushToClient(chatUnit, tree);
            }

            return;
        }

        var chat2G = new Chat2G_ChatMessage
        {
            ChatInfoTree = tree,
            SessionRuntimeId = 0
        };
        var gateConfigs = SceneConfigData.Instance.GetSceneBySceneType(SceneType.Gate);
        foreach (var gateSceneConfig in gateConfigs)
        {
            scene.Send(gateSceneConfig.Address, chat2G);
        }
    }

    private static uint Channel(ChatUnit chatUnit, ChatInfoTree tree)
    {
        if (!chatUnit.Channels.TryGetValue(tree.ChatChannelId, out var channel))
        {
            return 1;
        }

        channel.Send(tree);
        return 0;
    }

    private static uint Private(ChatUnit chatUnit, ChatInfoTree tree)
    {
        if (tree.Target == null || tree.Target.Count <= 0)
        {
            return 1;
        }

        var targetChatUnitId = tree.Target[0];
        var scene = chatUnit.Scene;
        if (!scene.GetComponent<ChatUnitManageComponent>().TryGet(targetChatUnitId, out var targetChatUnit))
        {
            return 2;
        }

        PushToClient(chatUnit, tree);
        PushToClient(targetChatUnit, tree);
        return 0;
    }

    /// <summary>
    /// 通过对应 Gate 向指定客户端推送聊天消息。
    /// </summary>
    public static void PushToClient(ChatUnit chatUnit, ChatInfoTree tree)
    {
        if (chatUnit == null || chatUnit.IsDisposed)
        {
            return;
        }

        if (!chatUnit.TryGetLinkTerminus(out var terminus) ||
            terminus == null ||
            terminus.ForwardSceneAddress == 0 ||
            terminus.ForwardSessionAddress == 0)
        {
            Log.Warning($"PushToClient fail, terminus not ready UnitId:{chatUnit.Id}");
            return;
        }

        chatUnit.Scene.Send(terminus.ForwardSceneAddress, new Chat2G_ChatMessage
        {
            ChatInfoTree = tree,
            SessionRuntimeId = terminus.ForwardSessionAddress
        });
    }
}
