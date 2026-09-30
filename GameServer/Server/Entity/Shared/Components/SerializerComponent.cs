using Fantasy.Entitas;
using Fantasy.Network;
using Fantasy.Serialize;
using LightProto;

namespace Fantasy;

public sealed class SerializerComponent : Entity
{
    public readonly MemoryStreamBufferPool BufferPool = new();
}
