using System.Collections.Generic;
using System.Reflection;
using Fantasy;
using Fantasy.Async;
using Fantasy.Helper;
using GOS.Common;

namespace GOS
{
    public static class GOSGame
    {
        private static bool isInitialize;
        
        private static IEngineModule EngineModule;
        private static string ApiUrl;
        
        /// <summary>
        /// 异步初始化 Fantasy 网络框架
        /// </summary>
        /// <param name="assemblies">热更程序集</param>
        public static async FTask Initialize(List<Assembly> assemblies, IEngineModule engineModule, string apiUrl)
        {
            if(isInitialize)
                return;
            isInitialize = true;
            // ⚠️ 重要: 手动加载程序集必须手动触发 Fantasy 注册
            // RuntimeInitializeOnLoadMethod 只在 Unity 启动时自动执行一次
            // 手动加载的 DLL 不会触发 RuntimeInitializeOnLoadMethod
            // 调用 Assembly.EnsureLoaded() 来触发该程序集中的 Fantasy 框架注册
            if (assemblies != null && assemblies.Count > 0)
            {
                foreach (var assembly in assemblies)
                {
                    assembly.EnsureLoaded();
                }
            }
            // 1. 初始化 Fantasy 框架
            await Fantasy.Platform.Unity.Entry.Initialize();

            EngineModule = engineModule;
            ApiUrl = apiUrl;
            
            Fantasy.Log.Info("Fantasy 初始化完成!");
        }


        public static async FTask<GOSClient> CreateClient()
        {
            if(!isInitialize)
                return null;
            var client = new GOSClient();
            await client.Initialize(EngineModule, ApiUrl);
            return client;
        }
        
    }
}