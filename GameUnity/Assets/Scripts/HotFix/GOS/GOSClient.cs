using System.Collections;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;
using GOS.Common;
using UnityEngine;

namespace GOS
{
    public class GOSClient
    {
        public Scene Scene { get; private set; }

        public AuthComponent Auth { get; private set; }

        public GOSClient()
        {
        }

        public async FTask Initialize(IEngineModule engineModule, string apiUrl)
        {
            Scene = await Fantasy.Scene.Create();
            var hostComponent = Scene.AddComponent<HostComponent>();
            hostComponent.ApiUrl = apiUrl;
            hostComponent.AutoReconnectTimes = 5;
            Scene.AddComponent<EngineComponent>().Module = engineModule;

            Auth = Scene.AddComponent<AuthComponent>();
            Auth.ReadSaverHistoryAccounts();
        }
    }
}