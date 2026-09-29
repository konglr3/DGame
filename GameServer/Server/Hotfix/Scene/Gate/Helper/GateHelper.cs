using Fantasy;
using Fantasy.Async;
using GameProto;

namespace Hotfix;

public static class GateHelper
{
    public static async FTask<CSServerInfo> DistributionGateAddress()
    {
        await FTask.CompletedTask;
        var serverInfo = TbServerConfig.ServerInfoList[0];
        return serverInfo;
    }
}