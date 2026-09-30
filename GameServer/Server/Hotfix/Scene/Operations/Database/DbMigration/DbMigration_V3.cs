using Fantasy;
using Fantasy.Async;
using Fantasy.Database;
using Fantasy.Entitas.Interface;
using MongoDB.Driver;

namespace Hotfix;

public sealed class DbMigration_V3 : IDbMigration
{
    public int Version => 3;

    public async FTask Upgrade(Scene scene, IDatabase database)
    {
        switch (scene.SceneType)
        {
            case SceneType.Address:
                await database.CreateIndex<GroupData>(
                    [Builders<GroupData>.IndexKeys.Ascending(d => d.Name)],
                    [new CreateIndexOptions { Name = "idx_group_data_name" }]);

                await database.CreateIndex<GroupMember>(
                    [
                        Builders<GroupMember>.IndexKeys.Ascending(d => d.GroupId).Ascending(d => d.RoleId)
                    ],
                    [new CreateIndexOptions { Unique = true, Name = "idx_group_member_group_role" }]);

                await database.CreateIndex<GroupMember>(
                    [
                        Builders<GroupMember>.IndexKeys.Ascending(d => d.GroupId).Ascending(d => d.State)
                    ],
                    [new CreateIndexOptions { Name = "idx_group_member_group_state" }]);

                await database.CreateIndex<GroupMember>(
                    [Builders<GroupMember>.IndexKeys.Ascending(d => d.RoleId)],
                    [new CreateIndexOptions { Name = "idx_group_member_role" }]);

                await database.CreateIndex<GroupBan>(
                    [
                        Builders<GroupBan>.IndexKeys.Ascending(d => d.GroupId).Ascending(d => d.RoleId)
                    ],
                    [new CreateIndexOptions { Unique = true, Name = "idx_group_ban_group_role" }]);
                break;
        }
    }
}
