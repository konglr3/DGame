using Fantasy;
using Fantasy.Async;
using Fantasy.Database;
using MongoDB.Driver;

namespace Hotfix;

public sealed class DbMigration_V2 : IDbMigration
{
    public int Version => 2;

    public async FTask Upgrade(Scene scene, IDatabase database)
    {
        switch (scene.SceneType)
        {
            case SceneType.Address:
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
                break;
        }
    }
}
