using System.Collections.Generic;
using System.Reflection;
using Fantasy.Async;
using GOS;

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
            UnityEngine.Debug.LogWarning(errorCode);
            if(errorCode != 0)
                return;
            await TestAutoReconnect(client);
        }


        private static async FTask TestAutoReconnect(GOSClient client)
        {
            await client.Scene.TimerComponent.Unity.WaitAsync(1500);
            client.Scene.GetComponent<HostComponent>().Disconnect(true);
        }
    }
}