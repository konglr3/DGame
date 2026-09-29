using System.Collections.Generic;
using System.Linq;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas.Interface;
using Fantasy.Helper;
using GOS.Common;
using UnityEngine;

namespace GOS
{
    public static class AuthSystem
    {

        #region Account

        public static void ReadSaverHistoryAccounts(this AuthComponent self)
        {
            var saveJson = self.Scene.GetComponent<EngineComponent>().Module?.Saver.GetString("HISTORY_ACCOUNTS");
            if (!string.IsNullOrEmpty(saveJson))
            {
                self.HistoryAccounts = saveJson.Deserialize<Dictionary<string, AuthAccount>>();
            }
        }

        public static AuthAccount GetLastAccountPassword(this AuthComponent self)
        {
            AuthAccount lastAccount = null;
            foreach (var account in self.HistoryAccounts.Values)
            {
                if (lastAccount == null || account.LastLoginTime < lastAccount.LastLoginTime)
                    lastAccount = account;
            }
            return lastAccount;
        }

        public static void SaveAccountPassword(this AuthComponent self, string accountId, string token)
        {
            if (!self.HistoryAccounts.TryGetValue(accountId, out var account))
            {
                account = new AuthAccount();
            }
            account.AccountId = accountId;
            account.Token = token;
            account.LastLoginTime = TimeHelper.Now;

            var saveJson = self.HistoryAccounts.ToJson();
            self.Scene.GetComponent<EngineComponent>().Module?.Saver.SetString("HISTORY_ACCOUNTS", saveJson);
        }
        

        #endregion

        #region  Register Login

        public static async FTask<A2C_RegisterResponse> Register(this AuthComponent self, string userName, string password)
        {
            var request = new C2A_RegisterRequest() { UserName = userName, Password = password };
            var response = await self.Scene.GetComponent<HostComponent>().CallByPost<A2C_RegisterResponse>("auth/register", request);
            return response;
        }

        public static async FTask<uint> AutoLogin(this AuthComponent self)
        {
            var historyAccount = self.GetLastAccountPassword();
            if (historyAccount != null && !string.IsNullOrEmpty(historyAccount.Token))
            {
                var hostComponent = self.Scene.GetComponent<HostComponent>();
                return await hostComponent.ConnectAsync(historyAccount.Token);
            }
            return default;
        }

        public static async FTask<uint> Login(this AuthComponent self, string userName, string password, int loginType = 0)
        {
            var hostComponent = self.Scene.GetComponent<HostComponent>();
            var request = new C2A_LoginRequest() { UserName = userName, Password = password };
            var authResponse = await hostComponent.CallByPost<A2C_LoginResponse>("auth/login", request);
            if (!JwtParseHelper.Parse(authResponse.Token, out var payload))
                return 1;
            if (authResponse.ErrorCode != 0)
                return 2;
            self.SaveAccountPassword(authResponse.RoleID.ToString(), authResponse.Token);
            return await self.LoginGate(authResponse.Token, authResponse.GateAddress, 1051);
        }

        /// <summary>
        /// 断线自动重连成功后，用已保存 Token 补发 Gate 登录，取消延迟下线并恢复 Roaming。
        /// </summary>
        public static async FTask<uint> ReconnectGate(this AuthComponent self)
        {
            if (string.IsNullOrEmpty(self.LoginToken) || self.ServerID == 0)
            {
                Log.Error("ReconnectGate fail: missing LoginToken or ServerID");
                return 1;
            }

            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("ReconnectGate fail: session disposed");
                return 1;
            }

            var loginResponse = (G2C_LoginResponse)await session.Call(new C2G_LoginRequest()
            {
                Token = self.LoginToken,
                ServerID = self.ServerID
            });
            if (loginResponse.ErrorCode != 0)
            {
                Log.Error($"ReconnectGate fail: ErrorCode={loginResponse.ErrorCode}");
            }

            return loginResponse.ErrorCode;
        }

        private static async FTask<uint> LoginGate(this AuthComponent self, string token, string gateAddress, int serverId)
        {
            var hostComponent = self.Scene.GetComponent<HostComponent>();
            var errorCode = await hostComponent.ConnectAsync(gateAddress);
            if (errorCode != 0)
                return errorCode;
            var loginResponse = (G2C_LoginResponse)await self.Scene.Session.Call(new C2G_LoginRequest()
                { Token = token, ServerID = serverId});
            if (loginResponse.ErrorCode != 0)
                return loginResponse.ErrorCode;
            self.LoginToken = token;
            self.ServerID = serverId;
            Debug.LogWarning(loginResponse.PlayerData.ToJson());
            return loginResponse.ErrorCode;
        }

        #endregion
    }

}
