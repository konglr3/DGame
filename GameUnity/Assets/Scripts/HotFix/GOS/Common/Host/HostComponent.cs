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
    }
}