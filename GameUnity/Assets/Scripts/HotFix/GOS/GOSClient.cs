using System.Collections;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;
using GOS.Common;
using GOS.Login;
using UnityEngine;

namespace GOS
{
    public class GOSClient
    {
        public Scene Scene { get; private set; }

        public LoginComponent Login { get; private set; }

        public GOSClient()
        {
        }

        public async FTask Initialize(IEngineModule engineModule, string apiUrl)
        {
            Scene = await Fantasy.Scene.Create();
            var hostComponent = Scene.AddComponent<HostComponent>();
            hostComponent.ApiUrl = apiUrl;
            Scene.AddComponent<EngineComponent>().Module = engineModule;

            Login = Scene.AddComponent<LoginComponent>();
            Login.ReadSaverHistoryAccounts();
        }
    }
}