using Fantasy;
using Fantasy.Async;
using Fantasy.Event;
// ReSharper disable InconsistentNaming

namespace Hotfix;

public sealed class OnSceneCreate_Init : AsyncEventSystem<OnCreateScene>
{
    protected override async FTask Handler(OnCreateScene self)
    {
        var scene = self.Scene;
        switch (scene.SceneType)
        {
            case SceneType.Operations:
                // Operations 场景是 AuthWorld 的单例入口 用于驱动账号库迁移
                await scene.AddComponent<DatabaseComponent>().MigrateAsync();
                break;
            // case SceneType.Address:
            //     // Address 场景是 GameWorld 的单例入口 用于驱动玩家库迁移
            //     await scene.AddComponent<DatabaseComponent>().MigrateAsync();
                break;
            case SceneType.Authentication:
                // 账号管理组件 用于注册和登录
                scene.AddComponent<AccountManagerComponent>();
                // 账号 Jwt Token 组件 用于生成和验证Token
                scene.AddComponent<AccountJwtComponent>();
                break;

            case SceneType.Gate:
                // 账号 Jwt Token 组件 用于生成和验证Token
                scene.AddComponent<AccountJwtComponent>();
                // 添加账号数据管理组件
                scene.AddComponent<PlayerManagerComponent>();
                break;
            case SceneType.Game:
                // Game 场景挂房间管理组件。
                scene.AddComponent<RoomManagerComponent>();
                // 在线玩家 / 好友 / 状态显示
                scene.AddComponent<GamePlayerManageComponent>();
                scene.AddComponent<FriendComponent>();
                scene.AddComponent<PresenceComponent>();
                scene.AddComponent<GroupComponent>();
                break;
            case SceneType.Chat:
                // 序列化组件（聊天节点附加 Data）
                scene.AddComponent<SerializerComponent>().Initialize();
                // ChatUnit 管理组件
                scene.AddComponent<ChatUnitManageComponent>();
                // 聊天频道中控
                scene.AddComponent<ChatChannelCenterComponent>();
                break;
        }

        await FTask.CompletedTask;
    }
}
