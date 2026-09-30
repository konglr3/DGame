using Fantasy.Async;
using Fantasy.Entitas.Interface;
using MongoDB.Driver;

namespace Fantasy;

public sealed class GroupComponentAwakeSystem : AwakeSystem<GroupComponent>
{
    protected override void Awake(GroupComponent self)
    {
        EnsureIndexes(self).Coroutine();
    }

    private static async FTask EnsureIndexes(GroupComponent self)
    {
        var database = self.Scene.World.Database;
        try
        {
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
        }
        catch (System.Exception e)
        {
            Log.Warning($"GroupComponent EnsureIndexes: {e.Message}");
        }
    }
}
