using System.Collections.Generic;
using Fantasy.Entitas;

namespace GOS
{
    public class AuthComponent : Entity
    {
        public Dictionary<string, AuthAccount> HistoryAccounts = new();

        public string LoginToken;

        /// <summary>
        /// 当前登录的区服 ID，自动重连补发 Gate 登录时使用。
        /// </summary>
        public int ServerID;
    }
}
