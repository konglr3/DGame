using Fantasy.Entitas.Interface;

namespace Fantasy;

public sealed class GamePlayerManageComponentDestroySystem : DestroySystem<GamePlayerManageComponent>
{
    protected override void Destroy(GamePlayerManageComponent self)
    {
        self.Players.Clear();
    }
}

public static class GamePlayerManageComponentSystem
{
    public static void Add(this GamePlayerManageComponent self, PlayerData playerData)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            return;
        }

        self.Players[playerData.Id] = playerData;
    }

    public static bool TryGet(this GamePlayerManageComponent self, long roleId, out PlayerData playerData)
        => self.Players.TryGetValue(roleId, out playerData!);

    public static PlayerData? Get(this GamePlayerManageComponent self, long roleId)
        => self.Players.GetValueOrDefault(roleId);

    public static void Remove(this GamePlayerManageComponent self, long roleId)
        => self.Players.Remove(roleId);
}
