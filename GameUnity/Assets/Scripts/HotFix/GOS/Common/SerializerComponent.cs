using Fantasy.Entitas;
using Fantasy.Network;
using Fantasy.Serialize;
using LightProto;

namespace GOS.Common
{
    public sealed class SerializerComponent : Entity
    {
        public readonly MemoryStreamBufferPool BufferPool = new();
    }

    public static class SerializerComponentSystem
    {
        public static void Initialize(this SerializerComponent self)
        {
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
}
