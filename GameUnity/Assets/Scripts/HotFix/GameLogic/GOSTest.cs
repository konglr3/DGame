using System.Collections.Generic;
using System.Reflection;
using Fantasy;
using Fantasy.Async;
using Fantasy.Helper;
using GOS;
using GOS.Chat;

namespace GameLogic
{
    public static class GOSTest
    {

        public static async FTask Initialize(List<Assembly> assemblies)
        {
            var apiUrl = $"http://127.0.0.1:20001/api/";
            await GOS.GOSGame.Initialize(assemblies, new GOSEngineModule(), apiUrl);
            var client = await GOS.GOSGame.CreateClient();
            
            // var response = await client.Login.Register("testUsername", "testPassword");
            // UnityEngine.Debug.LogWarning(response?.ToJson());
            
            var errorCode = await client.Auth.Login("testUsername", "testPassword");
            if(errorCode != 0)
                return;
            // await TestAutoReconnect(client);
            // await TestGameRoamingRpc(client);
            await TestChat(client);
        }


        private static async FTask TestAutoReconnect(GOSClient client)
        {
            await client.Scene.TimerComponent.Unity.WaitAsync(1500);
            client.Scene.GetComponent<HostComponent>().Disconnect(true);
        }

        private static async FTask TestGameRoamingRpc(GOSClient client)
        {
            var session = client.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                UnityEngine.Debug.LogError("TestGameRoamingRpc fail: session disposed");
                return;
            }

            var response = await session.C2Game_GetPlayerDataRequest();
            UnityEngine.Debug.LogWarning(
                $"TestGameRoamingRpc ErrorCode={response.ErrorCode} PlayerData={response.PlayerData?.ToJson()}");

            await TestChat(client);
        }

        private static async FTask TestChat(GOSClient client)
        {
            var chat = client.Scene.EnsureChat();
            chat.OnMessageReceived = (tree, text) =>
            {
                UnityEngine.Debug.LogWarning($"[Chat] from={tree?.UserName} channel={tree?.ChatChannelType} text={text}");
            };

            var broadcastResponse = await chat.SendBroadcast("hello chat");
            UnityEngine.Debug.LogWarning($"TestChat Broadcast ErrorCode={broadcastResponse?.ErrorCode}");

            var teamResponse = await chat.SendTeam("team hello");
            UnityEngine.Debug.LogWarning($"TestChat Team ErrorCode={teamResponse?.ErrorCode}");
        }

    }
}
