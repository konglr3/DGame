using System.Net.Http;
using Fantasy.Entitas;
using Fantasy.Network;

namespace GOS
{
    public class HostComponent : Entity
    {
        public string ApiUrl;
        
        public HttpClient Client = new HttpClient();
        
        public string gateAddress;

        public bool IsConnecting = false;

        /// <summary> 自动重连最大次数，小于等于 0 时使用 Watcher 默认值 </summary>
        public int AutoReconnectTimes;

        /// <summary> 主动断开时抑制断线回调触发的自动重连 </summary>
        public bool SuppressAutoReconnect;
    }
}