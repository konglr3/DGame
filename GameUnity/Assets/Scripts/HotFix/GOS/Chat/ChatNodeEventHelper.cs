using Fantasy;
using GOS.Common;

namespace GOS.Chat
{
    public static class ChatNodeEventHelper
    {
        public static void Handler(Scene scene, ChatInfoNode node)
        {
            switch ((ChatNodeEvent)node.ChatNodeEvent)
            {
                case ChatNodeEvent.OpenLink:
                    OpenLinkHandler(scene, node);
                    return;
                case ChatNodeEvent.OpenUI:
                    OpenUIHandler(scene, node);
                    return;
                case ChatNodeEvent.Position:
                    PositionHandler(scene, node);
                    return;
                case ChatNodeEvent.UseItem:
                    UseItemHandler(scene, node);
                    return;
            }
        }

        private static void OpenLinkHandler(Scene scene, ChatInfoNode node)
        {
            if (node.Data == null || node.Data.Length == 0)
            {
                return;
            }

            var chatLinkNode = scene.GetComponent<SerializerComponent>().Deserialize<ChatLinkNode>(node.Data);
            Log.Debug($"OpenLinkHandler Link:{chatLinkNode.Link}");
        }

        private static void OpenUIHandler(Scene scene, ChatInfoNode node)
        {
            if (node.Data == null || node.Data.Length == 0)
            {
                return;
            }

            var chatOpenUINode = scene.GetComponent<SerializerComponent>().Deserialize<ChatOpenUINode>(node.Data);
            Log.Debug($"OpenUIHandler UIName:{chatOpenUINode.UIName}");
        }

        private static void PositionHandler(Scene scene, ChatInfoNode node)
        {
            if (node.Data == null || node.Data.Length == 0)
            {
                return;
            }

            var chatPositionNode = scene.GetComponent<SerializerComponent>().Deserialize<ChatPositionNode>(node.Data);
            Log.Debug(
                $"PositionHandler MapName:{chatPositionNode.MapName} X:{chatPositionNode.PosX} Y:{chatPositionNode.PosY} Z:{chatPositionNode.PosZ}");
        }

        private static void UseItemHandler(Scene scene, ChatInfoNode node)
        {
            // TODO: Implement UseItemHandler
        }
    }
}
