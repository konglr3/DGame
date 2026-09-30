using System.Runtime.Serialization;
using LightProto;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Fantasy;

public partial class ChatInfoTree
{
    [BsonIgnore]
    [JsonIgnore]
    [ProtoIgnore]
    [IgnoreDataMember]
    public Scene Scene { get; set; }
}
