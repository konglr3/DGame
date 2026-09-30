using Fantasy.Entitas;
using Fantasy.Entitas.Interface;

namespace Fantasy;

public sealed class ChatUnitManageComponentDestroySystem : DestroySystem<ChatUnitManageComponent>
{
    protected override void Destroy(ChatUnitManageComponent self)
    {
        foreach (var chatUnit in self.Units.Values.ToArray())
        {
            chatUnit.Dispose();
        }

        self.Units.Clear();
    }
}

public static class ChatUnitManageComponentSystem
{
    public static ChatUnit Add(this ChatUnitManageComponent self, ChatUnit chatUnit)
    {
        self.Units[chatUnit.Id] = chatUnit;
        Log.Debug($"Add ChatUnit Count: {self.Units.Count} UnitId: {chatUnit.Id} UserName: {chatUnit.UserName}");
        return chatUnit;
    }

    public static ChatUnit? Get(this ChatUnitManageComponent self, long unitId)
    {
        return self.Units.GetValueOrDefault(unitId);
    }

    public static bool TryGet(this ChatUnitManageComponent self, long unitId, out ChatUnit chatUnit)
    {
        return self.Units.TryGetValue(unitId, out chatUnit!);
    }

    public static void Remove(this ChatUnitManageComponent self, long unitId, bool isDispose = true)
    {
        if (!self.Units.TryGetValue(unitId, out var chatUnit))
        {
            return;
        }

        self.Units.Remove(unitId);

        if (isDispose && !chatUnit.IsDisposed)
        {
            chatUnit.Dispose();
        }

        Log.Debug($"Remove ChatUnit: {chatUnit.UserName}({unitId}) Count: {self.Units.Count}");
    }
}
