using Fantasy.Entitas;

namespace GOS
{
    public sealed class HostConnectWatcherComponent : Entity
    {
        public const int RECONNECT_MAX_TIMES = 3;
        /// <summary> 重连间隔（秒） </summary>
        public const float RECONNECT_TIME_INTERVAL = 2f;

        /// <summary> 已重连次数  </summary>
        public int ReconnectedTimes;
        /// <summary> 重连间隔计时器  </summary>
        public float ReconnectIntervalTimer;
    }
}
