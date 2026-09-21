using Fantasy.Async;
using Fantasy.Helper;
using Fantasy.Network.HTTP;
using GameProto;
using Hotfix;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(SceneContextFilter))]
public class AuthController : ControllerBase
{
    private readonly Scene scene;
    
    /// <summary>
    /// 构造函数依赖注入
    /// </summary>
    /// <param name="scene"></param>
    public AuthController(Scene scene)
    {
        this.scene = scene;
    }

    [HttpPost("register")]
    public async FTask<IActionResult> Register([FromBody] C2A_RegisterRequest req)
    {
        using A2C_RegisterResponse response = A2C_RegisterResponse.Create();
        response.ErrorCode = await scene.Register(req.UserName, req.Password);
        return Ok(response.ToJson());
    }
    
    [HttpPost("login")]
    public async FTask<IActionResult> Login([FromBody] C2A_LoginRequest req)
    {
        using A2C_LoginResponse response = A2C_LoginResponse.Create();
        var result = await scene.Login(req.UserName, req.Password);
        if (result.ErrorCode == ErrorCode.SUCCESS)
        {
            response.Token = result.Token;
            response.RoleID = result.AccountId;
            response.GateAddress =
                $"{TbServerConfig.ServerInfoList[0].Address}:{TbServerConfig.ServerInfoList[0].Port}";
            // response.ServerInfoList = TbServerConfig.ServerInfoList;
            // response.RecentServerList = new List<int>();
            // response.RecentServerList.AddRange(result.RecentServerList); 
            // response.RecentServerRoleInfoList = await scene.GetComponent<AccountManagerComponent>().GetRecentServerRoleInfos(result.AccountId, result.RecentServerList);
        }
        response.ErrorCode = result.ErrorCode;
        return Ok(response.ToJson());
    }
}
