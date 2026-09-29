using Fantasy;
using Fantasy.Entitas.Interface;
using UnityEngine;

namespace GOS
{
    public sealed class HostConnectWatcherComponentDestroySystem : DestroySystem<HostConnectWatcherComponent>
    {
        protected override void Destroy(HostConnectWatcherComponent self)
        {
            self.ReconnectedTimes = 0;
            self.ReconnectIntervalTimer = 0;
        }
    }

    public sealed class HostConnectWatcherComponentUpdateSystem : UpdateSystem<HostConnectWatcherComponent>
    {
        protected override void Update(HostConnectWatcherComponent self)
        {
            if (self.ReconnectIntervalTimer <= 0)
            {
                return;
            }

            self.ReconnectIntervalTimer -= Time.deltaTime;
            if (self.ReconnectIntervalTimer <= 0)
            {
                self.ReconnectIntervalTimer = 0;
                self.Reconnect();
            }
        }
    }

    public static class HostConnectWatcherSystem
    {
        /// <summary>
        /// 断线后开始自动重连：先等待间隔，再发起重连。
        /// </summary>
        public static void OnSessionDisconnect(this HostConnectWatcherComponent self)
        {
            self.ReconnectedTimes = 0;
            self.ScheduleReconnect();
        }

        /// <summary>
        /// 重连失败后，若未达上限则继续按间隔重试。
        /// </summary>
        public static void OnSessionConnectFail(this HostConnectWatcherComponent self)
        {
            var maxTimes = self.GetMaxReconnectTimes();
            if (self.ReconnectedTimes >= maxTimes)
            {
                Log.Debug($"Reconnect Already Reached Max Times:{maxTimes}");
                self.Dispose();
                return;
            }

            self.ScheduleReconnect();
        }

        public static void Reconnect(this HostConnectWatcherComponent self)
        {
            var hostComponent = self.Parent as HostComponent;
            if (hostComponent == null)
            {
                return;
            }

            var maxTimes = self.GetMaxReconnectTimes();
            if (self.ReconnectedTimes >= maxTimes)
            {
                Log.Debug($"Reconnect Already Reached Max Times:{maxTimes}");
                self.Dispose();
                return;
            }

            self.ReconnectedTimes++;
            Log.Debug($"Reconnect-- Times:{self.ReconnectedTimes}/{maxTimes}");
            hostComponent.Reconnect();
        }

        private static void ScheduleReconnect(this HostConnectWatcherComponent self)
        {
            self.ReconnectIntervalTimer = HostConnectWatcherComponent.RECONNECT_TIME_INTERVAL;
            Log.Debug($"Schedule Reconnect after {HostConnectWatcherComponent.RECONNECT_TIME_INTERVAL}s, Times:{self.ReconnectedTimes}");
        }

        private static int GetMaxReconnectTimes(this HostConnectWatcherComponent self)
        {
            var hostComponent = self.Parent as HostComponent;
            if (hostComponent != null && hostComponent.AutoReconnectTimes > 0)
            {
                return hostComponent.AutoReconnectTimes;
            }

            return HostConnectWatcherComponent.RECONNECT_MAX_TIMES;
        }
    }
}
