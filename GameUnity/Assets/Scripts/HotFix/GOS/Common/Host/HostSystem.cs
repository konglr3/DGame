using System;
using System.Net.Http;
using System.Text;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas.Interface;
using Fantasy.EventAwaiter;
using Fantasy.Helper;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace GOS
{
    
    // 定义事件数据类型 (必须是 struct)
    public struct GateConnectResultEvent
    {
        public uint ErrorCode;
    }

    public sealed class GateConnectorComponentDestroySystem : DestroySystem<HostComponent>
    {
        protected override void Destroy(HostComponent self)
        {
            self.IsConnecting = false;
            self.ApiUrl = null;
            self.Client?.Dispose();
            self.Client = null;
            self.Disconnect();
        }
    }
    
    public static class HostSystem
    {
        public static async FTask<TResponse> CallByPost<TResponse>(this HostComponent self, string apiName, IMessage request) where TResponse : IMessage
        {
            var json = request.ToJson();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{self.ApiUrl}{apiName}";
            var response = await self.Client.PostAsync(url, content);
            var jsonBody = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(jsonBody))
                return default;
            Log.Info($"Post Response: {url} {jsonBody}");
            return jsonBody.Deserialize<TResponse>();
        }   

        
        public static async FTask<uint> ConnectAsync(this HostComponent self, string gateAddress, bool isReconnect = false)
        {
            try
            {
                if (self.IsConnecting)
                    return 1;
                self.IsConnecting = true;
                self.Scene.Connect(gateAddress, NetworkProtocolType.KCP, self.OnSessionConnected,
                    self.OnSessionConnectFail, self.OnSessionDisconnect, false);
                var eventComponent = self.AddComponent<EventAwaiterComponent>();
                var e = await eventComponent.Wait<GateConnectResultEvent>();
                if (e.ResultType != EventAwaiterResultType.Success || e.Value.ErrorCode != 0)
                {
                    return e.Value.ErrorCode;
                }
                
                self.gateAddress = gateAddress;
                
                self.IsConnecting = false;
                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return 1;
        }
        
        /// <summary>
        /// 断开当前网络连接
        /// </summary>
        public static void Disconnect(this HostComponent self)
        {
            var scene = self.Scene;
            self.IsConnecting = false;
            self.GetComponent<EventAwaiterComponent>().Notify( new GateConnectResultEvent()
            {
                ErrorCode = 1
            });
            self.RemoveComponent<HostConnectWatcherComponent>();
            if (scene != null && !scene.IsDisposed && scene.Session != null && !scene.Session.IsDisposed)
            {
                scene.Session.Dispose();
            }
        }
        
        /// <summary>
        /// 网络重连
        /// </summary>
        public static void Reconnect(this HostComponent self)
        {
            if (string.IsNullOrEmpty(self.gateAddress))
            {
                Fantasy.Log.Error("Invalid reconnect param");
                return;
            }
            if (self.IsConnecting)
                return;

            self.Disconnect();
            self.ConnectAsync(self.gateAddress, true).Coroutine();
        }
        
        

        private static void OnSessionConnected(this HostComponent self)
        {
            Log.Info("OnSessionConnected");
            self.StartHeartbeat();
            self.GetComponent<EventAwaiterComponent>().Notify( new GateConnectResultEvent());
        }
        private static void OnSessionConnectFail(this HostComponent self)
        {
            Log.Info("OnSessionConnectFail");
            self.GetComponent<EventAwaiterComponent>().Notify( new GateConnectResultEvent()
            {
                ErrorCode = 1
            });
        }
        private static void OnSessionDisconnect(this HostComponent self)
        {
            Log.Info("OnSessionDisconnect");
            //
        }
        
        /// <summary>
        /// 开启心跳检测
        /// </summary>
        private static void StartHeartbeat(this HostComponent self)
        {
            var scene = self.Scene;
            if (scene == null || scene.IsDisposed || scene.Session == null || scene.Session.IsDisposed)
            {
                return;
            }

            var session = scene.Session;
            var heartbeatComponent = session.GetComponent<SessionHeartbeatComponent>();
            if (heartbeatComponent != null)
            {
                // 组件已存在，先停止再启动（重连场景）
                heartbeatComponent.Stop();
            }
            else
            {
                // 组件不存在，添加新组件（首次登录场景）
                heartbeatComponent = session.AddComponent<SessionHeartbeatComponent>();
            }
            heartbeatComponent.Start(30 * 1000, 3 * 1000, 4 * 1000);
        }
    }
}