using Fantasy;
using GOS.Common;

namespace GOS.Chat
{
    /// <summary>
    /// 聊天信息节点工厂
    /// </summary>
    public static class ChatNodeFactory
    {
        public static ChatInfoTree AddendTextNode(this ChatInfoTree chatInfoTree, string content)
        {
            chatInfoTree.Node.Add(new ChatInfoNode
            {
                ChatNodeType = (int)ChatNodeType.Text,
                Content = content
            });
            return chatInfoTree;
        }

        public static ChatInfoTree AddendLinkNode(this ChatInfoTree chatInfoTree, string content, string link)
        {
            var chatLinkNode = new ChatLinkNode
            {
                Link = link
            };
            var serializerComponent = chatInfoTree.Scene.GetComponent<SerializerComponent>();
            chatInfoTree.Node.Add(new ChatInfoNode
            {
                ChatNodeType = (int)ChatNodeType.Link,
                ChatNodeEvent = (int)ChatNodeEvent.OpenLink,
                Content = content,
                Data = serializerComponent.Serialize(chatLinkNode)
            });
            return chatInfoTree;
        }

        public static ChatInfoTree AddendImageNode(this ChatInfoTree chatInfoTree, string content)
        {
            chatInfoTree.Node.Add(new ChatInfoNode
            {
                ChatNodeType = (int)ChatNodeType.Image,
                Content = content
            });
            return chatInfoTree;
        }

        public static ChatInfoTree AddendOpenUINode(this ChatInfoTree chatInfoTree, string content, string uiName)
        {
            var chatOpenUINode = new ChatOpenUINode
            {
                UIName = uiName
            };
            var serializerComponent = chatInfoTree.Scene.GetComponent<SerializerComponent>();
            chatInfoTree.Node.Add(new ChatInfoNode
            {
                ChatNodeType = (int)ChatNodeType.OpenUI,
                ChatNodeEvent = (int)ChatNodeEvent.OpenUI,
                Content = content,
                Data = serializerComponent.Serialize(chatOpenUINode)
            });
            return chatInfoTree;
        }

        public static ChatInfoTree AddendPositionNode(this ChatInfoTree chatInfoTree, string content, string mapName,
            float mapX, float mapY, float mapZ)
        {
            var chatPositionNode = new ChatPositionNode
            {
                MapName = mapName,
                PosX = mapX,
                PosY = mapY,
                PosZ = mapZ,
            };
            var serializerComponent = chatInfoTree.Scene.GetComponent<SerializerComponent>();
            chatInfoTree.Node.Add(new ChatInfoNode
            {
                ChatNodeType = (int)ChatNodeType.Position,
                ChatNodeEvent = (int)ChatNodeEvent.Position,
                Content = content,
                Data = serializerComponent.Serialize(chatPositionNode)
            });
            return chatInfoTree;
        }
    }
}
