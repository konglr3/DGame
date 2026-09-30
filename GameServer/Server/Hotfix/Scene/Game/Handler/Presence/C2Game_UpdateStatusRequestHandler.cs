using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_UpdateStatusRequestHandler
    : RoamingRPC<PlayerData, C2Game_UpdateStatusRequest, Game2C_UpdateStatusResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_UpdateStatusRequest request, Game2C_UpdateStatusResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.PRESENCE_INTERNAL_ERROR;
            return;
        }

        var presence = playerData.Scene.GetComponent<PresenceComponent>();
        if (presence == null)
        {
            response.ErrorCode = ErrorCode.PRESENCE_INTERNAL_ERROR;
            return;
        }

        presence.UpdateStatus(playerData, request.StatusText);
        response.ErrorCode = ErrorCode.SUCCESS;
        await FTask.CompletedTask;
    }
}
