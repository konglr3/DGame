using System.Collections.Generic;
using System.Linq;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas.Interface;
using Fantasy.Helper;
using GOS.Common;
using UnityEngine;

namespace GOS.Login
{
    public static class LoginSystem
    {

        #region Account

        public static void ReadSaverHistoryAccounts(this LoginComponent self)
        {
            var saveJson = self.Scene.GetComponent<EngineComponent>().Module?.Saver.GetString("HISTORY_ACCOUNTS");
            if (!string.IsNullOrEmpty(saveJson))
            {
                self.HistoryAccounts = saveJson.Deserialize<Dictionary<string, LoginAccount>>();
            }
        }

        public static LoginAccount GetLastAccountPassword(this LoginComponent self)
        {
            LoginAccount lastAccount = null;
            foreach (var account in self.HistoryAccounts.Values)
            {
                if (lastAccount == null || account.LastLoginTime < lastAccount.LastLoginTime)
                    lastAccount = account;
            }
            return lastAccount;
        }

        public static void SaveAccountPassword(this LoginComponent self, string accountId, string token)
        {
            if (!self.HistoryAccounts.TryGetValue(accountId, out var account))
            {
                account = new LoginAccount();
            }
            account.AccountId = accountId;
            account.Token = token;
            account.LastLoginTime = TimeHelper.Now;

            var saveJson = self.HistoryAccounts.ToJson();
            self.Scene.GetComponent<EngineComponent>().Module?.Saver.SetString("HISTORY_ACCOUNTS", saveJson);
        }
        

        #endregion

        #region  Register Login

        public static async FTask<A2C_RegisterResponse> Register(this LoginComponent self, string userName, string password)
        {
            var request = new C2A_RegisterRequest() { UserName = userName, Password = password };
            var response = await self.Scene.GetComponent<HostComponent>().CallByPost<A2C_RegisterResponse>("auth/register", request);
            return response;
        }

        public static async FTask<uint> AutoLogin(this LoginComponent self)
        {
            var historyAccount = self.GetLastAccountPassword();
            if (historyAccount != null && !string.IsNullOrEmpty(historyAccount.Token))
            {
                var hostComponent = self.Scene.GetComponent<HostComponent>();
                return await hostComponent.ConnectAsync(historyAccount.Token);
            }
            return default;
        }

        public static async FTask<uint> Login(this LoginComponent self, string userName, string password, int loginType = 0)
        {
            var hostComponent = self.Scene.GetComponent<HostComponent>();
            var request = new C2A_LoginRequest() { UserName = userName, Password = password };
            var authResponse = await hostComponent.CallByPost<A2C_LoginResponse>("auth/login", request);
            if (!JwtParseHelper.Parse(authResponse.Token, out var payload))
                return 1;
            if (authResponse.ErrorCode != 0)
                return 2;
            self.SaveAccountPassword(authResponse.RoleID.ToString(), authResponse.Token);
            return await self.LoginGate(authResponse.Token, authResponse.GateAddress);
        }

        private static async FTask<uint> LoginGate(this LoginComponent self, string token, string gateAddress)
        {
            var hostComponent = self.Scene.GetComponent<HostComponent>();
            var errorCode = await hostComponent.ConnectAsync(gateAddress);
            if (errorCode != 0)
                return errorCode;
            var loginResponse = (G2C_LoginResponse)await self.Scene.Session.Call(new C2G_LoginRequest()
                { Token = token, ServerID = 1051});
            if (loginResponse.ErrorCode != 0)
                return errorCode;
            self.LoginToken = token;
            Debug.LogWarning(loginResponse.PlayerData.ToJson());
            return loginResponse.ErrorCode;
        }

        #endregion
    }

}