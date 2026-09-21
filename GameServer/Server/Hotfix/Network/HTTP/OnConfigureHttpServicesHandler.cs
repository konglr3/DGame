using Fantasy.Async;
using Fantasy.Event;
using Fantasy.Network.HTTP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Hotfix;

/// <summary>
/// HTTP 服务配置：关闭非可空引用类型的隐式 Required，
/// 避免 Fantasy 协议里的 ResponseType 等字段导致 [ApiController] 自动 400。
/// </summary>
public sealed class OnConfigureHttpServicesHandler : AsyncEventSystem<OnConfigureHttpServices>
{
    protected override async FTask Handler(OnConfigureHttpServices self)
    {
        self.Builder.Services.Configure<MvcOptions>(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });

        await FTask.CompletedTask;
    }
}
