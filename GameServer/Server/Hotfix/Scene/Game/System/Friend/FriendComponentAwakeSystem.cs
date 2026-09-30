using Fantasy.Async;
using Fantasy.Entitas.Interface;
using MongoDB.Driver;

namespace Fantasy;

public sealed class FriendComponentAwakeSystem : AwakeSystem<FriendComponent>
{
    protected override void Awake(FriendComponent self)
    {
        EnsureIndexes(self).Coroutine();
    }

    private static async FTask EnsureIndexes(FriendComponent self)
    {
        var database = self.Scene.World.Database;
        try
        {
            await database.CreateIndex<FriendEdge>(
                [
                    Builders<FriendEdge>.IndexKeys.Ascending(d => d.OwnerRoleId).Ascending(d => d.TargetRoleId)
                ],
                [new CreateIndexOptions { Unique = true, Name = "idx_friend_edge_owner_target" }]);

            await database.CreateIndex<FriendEdge>(
                [
                    Builders<FriendEdge>.IndexKeys.Ascending(d => d.OwnerRoleId).Ascending(d => d.State)
                ],
                [new CreateIndexOptions { Name = "idx_friend_edge_owner_state" }]);
        }
        catch (System.Exception e)
        {
            Log.Warning($"FriendComponent EnsureIndexes: {e.Message}");
        }
    }
}
