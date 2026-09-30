using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Game Roaming：获取当前玩家角色数据。
/// </summary>
public sealed class C2Game_GetPlayerDataRequestHandler
    : RoamingRPC<PlayerData, C2Game_GetPlayerDataRequest, Game2C_GetPlayerDataResponse>
{
    protected override async FTask Run(
        PlayerData playerData,
        C2Game_GetPlayerDataRequest request,
        Game2C_GetPlayerDataResponse response,
        Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.ROLE_NOT_FOUND;
            return;
        }

        response.ErrorCode = ErrorCode.SUCCESS;
        response.PlayerData = PlayerDataHelper.ToCSPlayerData(playerData);
        await FTask.CompletedTask;
    }
}
