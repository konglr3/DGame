namespace Fantasy;

public static class ChatChannelComponentSystem
{
    public static void Send(this ChatChannelComponent self, ChatInfoTree tree)
    {
        var chatUnitManageComponent = self.Scene.GetComponent<ChatUnitManageComponent>();
        foreach (var unitId in self.Units)
        {
            if (!chatUnitManageComponent.Units.TryGetValue(unitId, out var chatUnit))
            {
                continue;
            }

            ChatSceneHelper.PushToClient(chatUnit, tree);
        }
    }

    public static bool JoinChannel(this ChatChannelComponent self, long chatUnitId)
    {
        var chatUnitManageComponent = self.Scene.GetComponent<ChatUnitManageComponent>();

        if (!chatUnitManageComponent.TryGet(chatUnitId, out var chatUnit))
        {
            return false;
        }

        self.Units.Add(chatUnitId);
        if (!chatUnit.Channels.ContainsKey(self.Id))
        {
            chatUnit.Channels.Add(self.Id, self);
        }

        return true;
    }

    public static bool IsJoinedChannel(this ChatChannelComponent self, long chatUnitId)
    {
        return self.Units.Contains(chatUnitId);
    }

    public static void ExitChannel(this ChatChannelComponent self, long chatUnitId, bool isRemoveUnitChannel = true)
    {
        if (!self.Units.Contains(chatUnitId))
        {
            return;
        }

        var chatUnitManageComponent = self.Scene.GetComponent<ChatUnitManageComponent>();

        if (chatUnitManageComponent.TryGet(chatUnitId, out var chatUnit) && isRemoveUnitChannel)
        {
            chatUnit.Channels.Remove(self.Id);
        }

        self.Units.Remove(chatUnitId);
        if (self.Units.Count == 0)
        {
            self.Dispose();
        }
    }
}
