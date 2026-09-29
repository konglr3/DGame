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
            self.SuppressAutoReconnect = false;
            self.ApiUrl = null;
            self.Client?.Dispose();
            self.Client = null;
            self.Disconnect();
        }
    }

    public sealed class HostComponentAwakeSystem : AwakeSystem<HostComponent>
    {
        protected override void Awake(HostComponent self)
        {
            
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
                var eventComponent = self.GetOrAddComponent<EventAwaiterComponent>();
                var e = await eventComponent.Wait<GateConnectResultEvent>();
                if (e.ResultType != EventAwaiterResultType.Success || e.Value.ErrorCode != 0)
                {
                    self.IsConnecting = false;
                    return e.Value.ErrorCode;
                }

                self.gateAddress = gateAddress;
                self.IsConnecting = false;

                // 自动重连：Socket 通后必须补发 Gate 登录，才能 CancelOfflineTimeout 并恢复 Roaming
                if (isReconnect)
                {
                    var auth = self.Scene.GetComponent<AuthComponent>();
                    var loginError = auth != null ? await auth.ReconnectGate() : 1u;
                    if (loginError != 0)
                    {
                        if (self.Scene.Session != null && !self.Scene.Session.IsDisposed)
                        {
                            self.Scene.Session.Dispose();
                        }

                        self.GetComponent<HostConnectWatcherComponent>()?.OnSessionConnectFail();
                        return loginError;
                    }
                }

                self.RemoveComponent<HostConnectWatcherComponent>();
                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
                self.IsConnecting = false;
            }
            return 1;
        }

        /// <summary>
        /// 断开当前网络连接
        /// </summary>
        /// <param name="keepWatcher">重连场景下保留 Watcher，避免打断自动重连</param>
        public static void Disconnect(this HostComponent self, bool keepWatcher = false)
        {
            if (self.IsConnecting)
                return;

            var scene = self.Scene;
            self.IsConnecting = false;

            if (!keepWatcher)
            {
                self.SuppressAutoReconnect = true;
                self.RemoveComponent<HostConnectWatcherComponent>();
            }

            self.GetComponent<EventAwaiterComponent>()?.Notify(new GateConnectResultEvent()
            {
                ErrorCode = 1
            });

            if (scene != null && !scene.IsDisposed && scene.Session != null && !scene.Session.IsDisposed)
            {
                scene.Session.Dispose();
            }

            if (!keepWatcher)
            {
                self.SuppressAutoReconnect = false;
            }
        }

        /// <summary>
        /// 网络重连
        /// </summary>
        public static bool Reconnect(this HostComponent self)
        {
            if (string.IsNullOrEmpty(self.gateAddress))
            {
                Fantasy.Log.Error("Invalid reconnect param");
                return false;
            }

            // 保留 Watcher，按间隔继续重试
            self.Disconnect(keepWatcher: true);
            self.ConnectAsync(self.gateAddress, true).Coroutine();
            return true;
        }

        private static void OnSessionConnected(this HostComponent self)
        {
            Log.Debug("OnSessionConnected");
            self.IsConnecting = false;
            // Watcher 在 ConnectAsync 完成 Gate 登录（重连场景）后再移除，避免登录失败丢失重试
            self.StartHeartbeat();
            self.GetComponent<EventAwaiterComponent>()?.Notify(new GateConnectResultEvent());
        }

        private static void OnSessionConnectFail(this HostComponent self)
        {
            Log.Debug("OnSessionConnectFail");
            self.IsConnecting = false;
            self.GetComponent<HostConnectWatcherComponent>()?.OnSessionConnectFail();
            self.GetComponent<EventAwaiterComponent>()?.Notify(new GateConnectResultEvent()
            {
                ErrorCode = 1
            });
        }

        private static void OnSessionDisconnect(this HostComponent self)
        {
            Log.Debug("OnSessionDisconnect");
            self.IsConnecting = false;

            // 主动断开：不自动重连
            if (self.SuppressAutoReconnect)
            {
                return;
            }

            // 已在监控/重连中（例如重连时 Dispose Session 再次回调），忽略
            if (self.GetComponent<HostConnectWatcherComponent>() != null)
            {
                return;
            }

            self.AddComponent<HostConnectWatcherComponent>().OnSessionDisconnect();
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
