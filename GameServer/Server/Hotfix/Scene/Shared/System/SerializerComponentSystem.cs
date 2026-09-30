using Fantasy.Serialize;
using LightProto;

namespace Fantasy;

public static class SerializerComponentSystem
{
    public static void Initialize(this SerializerComponent self)
    {
        // ProtoBufHelper 在框架 Initialize 时已就绪，这里仅作为 Chat 场景挂载标记。
    }

    public static byte[] Serialize<T>(this SerializerComponent self, T @object) where T : IProtoParser<T>
    {
        return SerializerManager.ProtoBufHelper.Serialize(@object);
    }

    public static T Deserialize<T>(this SerializerComponent self, byte[] bytes) where T : IProtoParser<T>
    {
        return SerializerManager.ProtoBufHelper.Deserialize<T>(bytes);
    }
}
