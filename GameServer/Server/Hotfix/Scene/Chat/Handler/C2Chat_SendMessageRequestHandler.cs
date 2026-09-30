using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Chat Roaming：客户端发送聊天消息。
/// </summary>
public sealed class C2Chat_SendMessageRequestHandler
    : RoamingRPC<ChatUnit, C2Chat_SendMessageRequest, Chat2C_SendMessageResponse>
{
    protected override async FTask Run(
        ChatUnit chatUnit,
        C2Chat_SendMessageRequest request,
        Chat2C_SendMessageResponse response,
        Action reply)
    {
        if (chatUnit == null || chatUnit.IsDisposed)
        {
            response.ErrorCode = 1;
            return;
        }

        if (request.ChatInfoTree != null)
        {
            request.ChatInfoTree.UnitId = chatUnit.Id;
            if (string.IsNullOrEmpty(request.ChatInfoTree.UserName))
            {
                request.ChatInfoTree.UserName = chatUnit.UserName;
            }
        }

        response.ErrorCode = ChatSceneHelper.Distribution(chatUnit, request.ChatInfoTree);
        await FTask.CompletedTask;
    }
}
