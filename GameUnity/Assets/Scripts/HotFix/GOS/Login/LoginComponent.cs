using System.Collections.Generic;
using Fantasy.Entitas;

namespace GOS.Login
{
    public class LoginComponent : Entity
    {
        public Dictionary<string, LoginAccount> HistoryAccounts = new();

        public string LoginToken;
    }
}