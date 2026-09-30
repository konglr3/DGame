using System.Runtime.Serialization;
using Fantasy;
using LightProto;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Fantasy
{
    /// <summary>
    /// 与 OuterMessage 中生成的 ChatInfoTree 同程序集 partial，用于挂载运行时 Scene。
    /// </summary>
    public partial class ChatInfoTree
    {
        [BsonIgnore]
        [JsonIgnore]
        [ProtoIgnore]
        [IgnoreDataMember]
        public Scene Scene { get; set; }
    }
}