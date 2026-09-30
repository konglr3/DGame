using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_ListRecommendFriendsRequestHandler
    : RoamingRPC<PlayerData, C2Game_ListRecommendFriendsRequest, Game2C_ListRecommendFriendsResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_ListRecommendFriendsRequest request, Game2C_ListRecommendFriendsResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        var friendComponent = playerData.Scene.GetComponent<FriendComponent>();
        if (friendComponent == null)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        response.Friends = await friendComponent.ListRecommend(playerData, request.Limit);
        response.ErrorCode = ErrorCode.SUCCESS;
    }
}
