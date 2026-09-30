using LightProto;
using System;
using MemoryPack;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Pool;
using Fantasy.Network.Interface;
using Fantasy.Serialize;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PreferConcreteValueOverDefault
// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable RedundantUsingDirective
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Fantasy
{
    /// <summary>
    /// 活动开启配置
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class CSActivityOpenEntry : AMessage, IDisposable
    {
        public static CSActivityOpenEntry Create(bool autoReturn = true)
        {
            var cSActivityOpenEntry = MessageObjectPool<CSActivityOpenEntry>.Rent();
            cSActivityOpenEntry.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSActivityOpenEntry.SetIsPool(false);
            }
            
            return cSActivityOpenEntry;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ActivityId = default;
            ActivityType = default;
            OpenTime = default;
            EndTime = default;
            DelayTime = default;
            MessageObjectPool<CSActivityOpenEntry>.Return(this);
        }
        /// <summary>
        /// 活动ID
        /// </summary>
        [ProtoMember(1)]
        public int ActivityId { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        [ProtoMember(2)]
        public int ActivityType { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [ProtoMember(3)]
        public uint OpenTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [ProtoMember(4)]
        public uint EndTime { get; set; }
        /// <summary>
        /// 延迟消失时间
        /// </summary>
        [ProtoMember(5)]
        public uint DelayTime { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class C2G_CreateRoomRequest : AMessage, IRequest
    {
        public static C2G_CreateRoomRequest Create(bool autoReturn = true)
        {
            var c2G_CreateRoomRequest = MessageObjectPool<C2G_CreateRoomRequest>.Rent();
            c2G_CreateRoomRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_CreateRoomRequest.SetIsPool(false);
            }
            
            return c2G_CreateRoomRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            PlayerCount = default;
            MessageObjectPool<C2G_CreateRoomRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_CreateRoomRequest; } 
        [ProtoIgnore]
        public G2C_CreateRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public int PlayerCount { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_CreateRoomResponse : AMessage, IResponse
    {
        public static G2C_CreateRoomResponse Create(bool autoReturn = true)
        {
            var g2C_CreateRoomResponse = MessageObjectPool<G2C_CreateRoomResponse>.Rent();
            g2C_CreateRoomResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_CreateRoomResponse.SetIsPool(false);
            }
            
            return g2C_CreateRoomResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            if (RoomInfo != null)
            {
                RoomInfo.Dispose();
                RoomInfo = null;
            }
            PlayerCount = default;
            PlayerInfos.Clear();
            MessageObjectPool<G2C_CreateRoomResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_CreateRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public CSRoomInfo RoomInfo { get; set; }
        [ProtoMember(3)]
        public int PlayerCount { get; set; }
        [ProtoMember(4)]
        public List<CSRoomPlayerInfo> PlayerInfos { get; set; } = new List<CSRoomPlayerInfo>();
    }
    [Serializable]
    [ProtoContract]
    public partial class C2G_JoinRoomRequest : AMessage, IRequest
    {
        public static C2G_JoinRoomRequest Create(bool autoReturn = true)
        {
            var c2G_JoinRoomRequest = MessageObjectPool<C2G_JoinRoomRequest>.Rent();
            c2G_JoinRoomRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_JoinRoomRequest.SetIsPool(false);
            }
            
            return c2G_JoinRoomRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoomId = default;
            MessageObjectPool<C2G_JoinRoomRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_JoinRoomRequest; } 
        [ProtoIgnore]
        public G2C_JoinRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public int RoomId { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_JoinRoomResponse : AMessage, IResponse
    {
        public static G2C_JoinRoomResponse Create(bool autoReturn = true)
        {
            var g2C_JoinRoomResponse = MessageObjectPool<G2C_JoinRoomResponse>.Rent();
            g2C_JoinRoomResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_JoinRoomResponse.SetIsPool(false);
            }
            
            return g2C_JoinRoomResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            if (RoomInfo != null)
            {
                RoomInfo.Dispose();
                RoomInfo = null;
            }
            PlayerCount = default;
            PlayerInfos.Clear();
            MessageObjectPool<G2C_JoinRoomResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_JoinRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public CSRoomInfo RoomInfo { get; set; }
        [ProtoMember(3)]
        public int PlayerCount { get; set; }
        [ProtoMember(4)]
        public List<CSRoomPlayerInfo> PlayerInfos { get; set; } = new List<CSRoomPlayerInfo>();
    }
    [Serializable]
    [ProtoContract]
    public partial class C2G_LeaveRoomRequest : AMessage, IRequest
    {
        public static C2G_LeaveRoomRequest Create(bool autoReturn = true)
        {
            var c2G_LeaveRoomRequest = MessageObjectPool<C2G_LeaveRoomRequest>.Rent();
            c2G_LeaveRoomRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_LeaveRoomRequest.SetIsPool(false);
            }
            
            return c2G_LeaveRoomRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<C2G_LeaveRoomRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_LeaveRoomRequest; } 
        [ProtoIgnore]
        public G2C_LeaveRoomResponse ResponseType { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_LeaveRoomResponse : AMessage, IResponse
    {
        public static G2C_LeaveRoomResponse Create(bool autoReturn = true)
        {
            var g2C_LeaveRoomResponse = MessageObjectPool<G2C_LeaveRoomResponse>.Rent();
            g2C_LeaveRoomResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_LeaveRoomResponse.SetIsPool(false);
            }
            
            return g2C_LeaveRoomResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<G2C_LeaveRoomResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_LeaveRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class G2C_RoomPlayerInfoChangedNotify : AMessage, IMessage
    {
        public static G2C_RoomPlayerInfoChangedNotify Create(bool autoReturn = true)
        {
            var g2C_RoomPlayerInfoChangedNotify = MessageObjectPool<G2C_RoomPlayerInfoChangedNotify>.Rent();
            g2C_RoomPlayerInfoChangedNotify.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_RoomPlayerInfoChangedNotify.SetIsPool(false);
            }
            
            return g2C_RoomPlayerInfoChangedNotify;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (RoomInfo != null)
            {
                RoomInfo.Dispose();
                RoomInfo = null;
            }
            PlayerCount = default;
            PlayerInfos.Clear();
            MessageObjectPool<G2C_RoomPlayerInfoChangedNotify>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_RoomPlayerInfoChangedNotify; } 
        [ProtoMember(1)]
        public CSRoomInfo RoomInfo { get; set; }
        [ProtoMember(2)]
        public int PlayerCount { get; set; }
        [ProtoMember(3)]
        public List<CSRoomPlayerInfo> PlayerInfos { get; set; } = new List<CSRoomPlayerInfo>();
    }
    [Serializable]
    [ProtoContract]
    public partial class C2S_SyncFrameDataReq : AMessage, IMessage
    {
        public static C2S_SyncFrameDataReq Create(bool autoReturn = true)
        {
            var c2S_SyncFrameDataReq = MessageObjectPool<C2S_SyncFrameDataReq>.Rent();
            c2S_SyncFrameDataReq.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2S_SyncFrameDataReq.SetIsPool(false);
            }
            
            return c2S_SyncFrameDataReq;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ForecastFrameId = default;
            RevClientFrameId = default;
            if (RoomInfo != null)
            {
                RoomInfo.Dispose();
                RoomInfo = null;
            }
            RoomPlayerId = default;
            if (FrameData != null)
            {
                FrameData.Dispose();
                FrameData = null;
            }
            MessageObjectPool<C2S_SyncFrameDataReq>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2S_SyncFrameDataReq; } 
        [ProtoMember(1)]
        public int ForecastFrameId { get; set; }
        [ProtoMember(2)]
        public int RevClientFrameId { get; set; }
        [ProtoMember(3)]
        public CSRoomInfo RoomInfo { get; set; }
        [ProtoMember(4)]
        public int RoomPlayerId { get; set; }
        [ProtoMember(5)]
        public CSOnePlayerFrameCmd FrameData { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class S2C_BattleFinClientDataReq : AMessage, IMessage
    {
        public static S2C_BattleFinClientDataReq Create(bool autoReturn = true)
        {
            var s2C_BattleFinClientDataReq = MessageObjectPool<S2C_BattleFinClientDataReq>.Rent();
            s2C_BattleFinClientDataReq.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                s2C_BattleFinClientDataReq.SetIsPool(false);
            }
            
            return s2C_BattleFinClientDataReq;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (StartParam != null)
            {
                StartParam.Dispose();
                StartParam = null;
            }
            DurationTime = default;
            MessageObjectPool<S2C_BattleFinClientDataReq>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.S2C_BattleFinClientDataReq; } 
        [ProtoMember(1)]
        public CSBattleStartParam StartParam { get; set; }
        [ProtoMember(2)]
        public uint DurationTime { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSBattleStartParam : AMessage, IDisposable
    {
        public static CSBattleStartParam Create(bool autoReturn = true)
        {
            var cSBattleStartParam = MessageObjectPool<CSBattleStartParam>.Rent();
            cSBattleStartParam.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSBattleStartParam.SetIsPool(false);
            }
            
            return cSBattleStartParam;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RandSeed = default;
            Fps = default;
            PlayerCount = default;
            BattleStatus = default;
            IsGuide = default;
            StartTime = default;
            BattleGID = default;
            MultiPlayerBattle = default;
            CaptainPlayerId = default;
            LastOnlyBattleTime = default;
            MessageObjectPool<CSBattleStartParam>.Return(this);
        }
        [ProtoMember(1)]
        public int RandSeed { get; set; }
        [ProtoMember(2)]
        public int Fps { get; set; }
        [ProtoMember(3)]
        public int PlayerCount { get; set; }
        [ProtoMember(4)]
        public int BattleStatus { get; set; }
        [ProtoMember(5)]
        public byte IsGuide { get; set; }
        [ProtoMember(6)]
        public uint StartTime { get; set; }
        [ProtoMember(7)]
        public ulong BattleGID { get; set; }
        [ProtoMember(8)]
        public byte MultiPlayerBattle { get; set; }
        [ProtoMember(9)]
        public ulong CaptainPlayerId { get; set; }
        [ProtoMember(10)]
        public uint LastOnlyBattleTime { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class C2S_StartBattleRequest : AMessage, IRequest
    {
        public static C2S_StartBattleRequest Create(bool autoReturn = true)
        {
            var c2S_StartBattleRequest = MessageObjectPool<C2S_StartBattleRequest>.Rent();
            c2S_StartBattleRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2S_StartBattleRequest.SetIsPool(false);
            }
            
            return c2S_StartBattleRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<C2S_StartBattleRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2S_StartBattleRequest; } 
        [ProtoIgnore]
        public S2C_StartBattleResponse ResponseType { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class S2C_StartBattleResponse : AMessage, IResponse
    {
        public static S2C_StartBattleResponse Create(bool autoReturn = true)
        {
            var s2C_StartBattleResponse = MessageObjectPool<S2C_StartBattleResponse>.Rent();
            s2C_StartBattleResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                s2C_StartBattleResponse.SetIsPool(false);
            }
            
            return s2C_StartBattleResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<S2C_StartBattleResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.S2C_StartBattleResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class S2C_NotifyBattleLoading : AMessage, IMessage
    {
        public static S2C_NotifyBattleLoading Create(bool autoReturn = true)
        {
            var s2C_NotifyBattleLoading = MessageObjectPool<S2C_NotifyBattleLoading>.Rent();
            s2C_NotifyBattleLoading.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                s2C_NotifyBattleLoading.SetIsPool(false);
            }
            
            return s2C_NotifyBattleLoading;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<S2C_NotifyBattleLoading>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.S2C_NotifyBattleLoading; } 
    }
    [Serializable]
    [ProtoContract]
    public partial class C2S_BattleLoadDoneRequest : AMessage, IRequest
    {
        public static C2S_BattleLoadDoneRequest Create(bool autoReturn = true)
        {
            var c2S_BattleLoadDoneRequest = MessageObjectPool<C2S_BattleLoadDoneRequest>.Rent();
            c2S_BattleLoadDoneRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2S_BattleLoadDoneRequest.SetIsPool(false);
            }
            
            return c2S_BattleLoadDoneRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<C2S_BattleLoadDoneRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2S_BattleLoadDoneRequest; } 
        [ProtoIgnore]
        public S2C_BattleLoadDoneResponse ResponseType { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class S2C_BattleLoadDoneResponse : AMessage, IResponse
    {
        public static S2C_BattleLoadDoneResponse Create(bool autoReturn = true)
        {
            var s2C_BattleLoadDoneResponse = MessageObjectPool<S2C_BattleLoadDoneResponse>.Rent();
            s2C_BattleLoadDoneResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                s2C_BattleLoadDoneResponse.SetIsPool(false);
            }
            
            return s2C_BattleLoadDoneResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<S2C_BattleLoadDoneResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.S2C_BattleLoadDoneResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class S2C_NotifyEnterBattle : AMessage, IMessage
    {
        public static S2C_NotifyEnterBattle Create(bool autoReturn = true)
        {
            var s2C_NotifyEnterBattle = MessageObjectPool<S2C_NotifyEnterBattle>.Rent();
            s2C_NotifyEnterBattle.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                s2C_NotifyEnterBattle.SetIsPool(false);
            }
            
            return s2C_NotifyEnterBattle;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RandSeed = default;
            PlayerCount = default;
            IsHaveRoomInfo = default;
            if (RoomInfoList != null)
            {
                RoomInfoList.Dispose();
                RoomInfoList = null;
            }
            BattleStatus = default;
            IsGuide = default;
            StartTime = default;
            BattleGID = default;
            MultiPlayerBattle = default;
            CaptainPlayerId = default;
            PlayerDataList.Clear();
            if (Chapter != null)
            {
                Chapter.Dispose();
                Chapter = null;
            }
            Stage = default;
            MapID = default;
            MessageObjectPool<S2C_NotifyEnterBattle>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.S2C_NotifyEnterBattle; } 
        [ProtoMember(1)]
        public int RandSeed { get; set; }
        [ProtoMember(2)]
        public int PlayerCount { get; set; }
        [ProtoMember(3)]
        public byte IsHaveRoomInfo { get; set; }
        [ProtoMember(4)]
        public CSRoomInfo RoomInfoList { get; set; }
        [ProtoMember(5)]
        public int BattleStatus { get; set; }
        [ProtoMember(6)]
        public byte IsGuide { get; set; }
        [ProtoMember(7)]
        public uint StartTime { get; set; }
        [ProtoMember(8)]
        public ulong BattleGID { get; set; }
        [ProtoMember(9)]
        public byte MultiPlayerBattle { get; set; }
        [ProtoMember(10)]
        public ulong CaptainPlayerId { get; set; }
        [ProtoMember(11)]
        public List<CSLevelPlayerData> PlayerDataList { get; set; } = new List<CSLevelPlayerData>();
        [ProtoMember(12)]
        public CSChapterInfo Chapter { get; set; }
        [ProtoMember(13)]
        public int Stage { get; set; }
        [ProtoMember(14)]
        public int MapID { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSChapterInfo : AMessage, IMessage
    {
        public static CSChapterInfo Create(bool autoReturn = true)
        {
            var cSChapterInfo = MessageObjectPool<CSChapterInfo>.Rent();
            cSChapterInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSChapterInfo.SetIsPool(false);
            }
            
            return cSChapterInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ChapterID = default;
            Difficult = default;
            MessageObjectPool<CSChapterInfo>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.CSChapterInfo; } 
        [ProtoMember(1)]
        public int ChapterID { get; set; }
        [ProtoMember(2)]
        public int Difficult { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class S2C_BroadcastFrameData : AMessage, IMessage
    {
        public static S2C_BroadcastFrameData Create(bool autoReturn = true)
        {
            var s2C_BroadcastFrameData = MessageObjectPool<S2C_BroadcastFrameData>.Rent();
            s2C_BroadcastFrameData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                s2C_BroadcastFrameData.SetIsPool(false);
            }
            
            return s2C_BroadcastFrameData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (RoomInfo != null)
            {
                RoomInfo.Dispose();
                RoomInfo = null;
            }
            SveFrameId = default;
            FrameCount = default;
            FrameDataList.Clear();
            MessageObjectPool<S2C_BroadcastFrameData>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.S2C_BroadcastFrameData; } 
        [ProtoMember(1)]
        public CSRoomInfo RoomInfo { get; set; }
        [ProtoMember(2)]
        public int SveFrameId { get; set; }
        [ProtoMember(3)]
        public int FrameCount { get; set; }
        [ProtoMember(4)]
        public List<CSSyncOneFrameData> FrameDataList { get; set; } = new List<CSSyncOneFrameData>();
    }
    [Serializable]
    [ProtoContract]
    public partial class CSLevelPlayerData : AMessage, IDisposable
    {
        public static CSLevelPlayerData Create(bool autoReturn = true)
        {
            var cSLevelPlayerData = MessageObjectPool<CSLevelPlayerData>.Rent();
            cSLevelPlayerData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSLevelPlayerData.SetIsPool(false);
            }
            
            return cSLevelPlayerData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (PlayerShowData != null)
            {
                PlayerShowData.Dispose();
                PlayerShowData = null;
            }
            if (PlayerBattleData != null)
            {
                PlayerBattleData.Dispose();
                PlayerBattleData = null;
            }
            MessageObjectPool<CSLevelPlayerData>.Return(this);
        }
        [ProtoMember(1)]
        public CSMiniRoleBaseShowData PlayerShowData { get; set; }
        [ProtoMember(2)]
        public CSBattlePlayerData PlayerBattleData { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSMiniRoleBaseShowData : AMessage, IDisposable
    {
        public static CSMiniRoleBaseShowData Create(bool autoReturn = true)
        {
            var cSMiniRoleBaseShowData = MessageObjectPool<CSMiniRoleBaseShowData>.Rent();
            cSMiniRoleBaseShowData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSMiniRoleBaseShowData.SetIsPool(false);
            }
            
            return cSMiniRoleBaseShowData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Uin = default;
            RoleID = default;
            WorldID = default;
            Online = default;
            FightVal = default;
            VIPLevel = default;
            RoleName = default;
            Sex = default;
            Head = default;
            HeadSex = default;
            HeadURL = default;
            HeadFrame = default;
            MessageObjectPool<CSMiniRoleBaseShowData>.Return(this);
        }
        [ProtoMember(1)]
        public uint Uin { get; set; }
        [ProtoMember(2)]
        public ulong RoleID { get; set; }
        [ProtoMember(3)]
        public uint WorldID { get; set; }
        [ProtoMember(4)]
        public byte Online { get; set; }
        [ProtoMember(5)]
        public ulong FightVal { get; set; }
        [ProtoMember(6)]
        public uint VIPLevel { get; set; }
        [ProtoMember(7)]
        public string RoleName { get; set; }
        [ProtoMember(8)]
        public byte Sex { get; set; }
        [ProtoMember(9)]
        public uint Head { get; set; }
        [ProtoMember(10)]
        public byte HeadSex { get; set; }
        [ProtoMember(11)]
        public string HeadURL { get; set; }
        [ProtoMember(12)]
        public uint HeadFrame { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSBattlePlayerData : AMessage, IDisposable
    {
        public static CSBattlePlayerData Create(bool autoReturn = true)
        {
            var cSBattlePlayerData = MessageObjectPool<CSBattlePlayerData>.Rent();
            cSBattlePlayerData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSBattlePlayerData.SetIsPool(false);
            }
            
            return cSBattlePlayerData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleID = default;
            if (PlayerBaseData != null)
            {
                PlayerBaseData.Dispose();
                PlayerBaseData = null;
            }
            if (PlayerRunData != null)
            {
                PlayerRunData.Dispose();
                PlayerRunData = null;
            }
            MessageObjectPool<CSBattlePlayerData>.Return(this);
        }
        [ProtoMember(1)]
        public ulong RoleID { get; set; }
        [ProtoMember(2)]
        public CSBattlePlayerBaseData PlayerBaseData { get; set; }
        [ProtoMember(3)]
        public CSLevelUnitRunData PlayerRunData { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSLevelUnitRunData : AMessage, IDisposable
    {
        public static CSLevelUnitRunData Create(bool autoReturn = true)
        {
            var cSLevelUnitRunData = MessageObjectPool<CSLevelUnitRunData>.Rent();
            cSLevelUnitRunData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSLevelUnitRunData.SetIsPool(false);
            }
            
            return cSLevelUnitRunData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Hp = default;
            Level = default;
            Exp = default;
            Gold = default;
            MessageObjectPool<CSLevelUnitRunData>.Return(this);
        }
        [ProtoMember(1)]
        public int Hp { get; set; }
        [ProtoMember(2)]
        public int Level { get; set; }
        [ProtoMember(3)]
        public int Exp { get; set; }
        [ProtoMember(4)]
        public int Gold { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSBattlePlayerBaseData : AMessage, IDisposable
    {
        public static CSBattlePlayerBaseData Create(bool autoReturn = true)
        {
            var cSBattlePlayerBaseData = MessageObjectPool<CSBattlePlayerBaseData>.Rent();
            cSBattlePlayerBaseData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSBattlePlayerBaseData.SetIsPool(false);
            }
            
            return cSBattlePlayerBaseData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            PlayerLevel = default;
            BodyType = default;
            FashionID = default;
            WeaponFashionID = default;
            CreateRoleDays = default;
            FailureCount = default;
            if (AttrData != null)
            {
                AttrData.Dispose();
                AttrData = null;
            }
            MessageObjectPool<CSBattlePlayerBaseData>.Return(this);
        }
        [ProtoMember(1)]
        public int PlayerLevel { get; set; }
        [ProtoMember(2)]
        public byte BodyType { get; set; }
        [ProtoMember(3)]
        public uint FashionID { get; set; }
        [ProtoMember(4)]
        public uint WeaponFashionID { get; set; }
        [ProtoMember(5)]
        public int CreateRoleDays { get; set; }
        [ProtoMember(6)]
        public int FailureCount { get; set; }
        [ProtoMember(7)]
        public CSUnitBattleAttrData AttrData { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSUnitBattleAttrData : AMessage, IDisposable
    {
        public static CSUnitBattleAttrData Create(bool autoReturn = true)
        {
            var cSUnitBattleAttrData = MessageObjectPool<CSUnitBattleAttrData>.Rent();
            cSUnitBattleAttrData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSUnitBattleAttrData.SetIsPool(false);
            }
            
            return cSUnitBattleAttrData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Atk = default;
            Hp = default;
            MaxHp = default;
            MoveSpeed = default;
            MessageObjectPool<CSUnitBattleAttrData>.Return(this);
        }
        [ProtoMember(1)]
        public int Atk { get; set; }
        [ProtoMember(2)]
        public int Hp { get; set; }
        [ProtoMember(3)]
        public int MaxHp { get; set; }
        [ProtoMember(4)]
        public int MoveSpeed { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSRoomInfo : AMessage, IDisposable
    {
        public static CSRoomInfo Create(bool autoReturn = true)
        {
            var cSRoomInfo = MessageObjectPool<CSRoomInfo>.Rent();
            cSRoomInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSRoomInfo.SetIsPool(false);
            }
            
            return cSRoomInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoomId = default;
            RoomSeq = default;
            CaptainRoleId = default;
            MessageObjectPool<CSRoomInfo>.Return(this);
        }
        [ProtoMember(1)]
        public int RoomId { get; set; }
        [ProtoMember(2)]
        public int RoomSeq { get; set; }
        [ProtoMember(3)]
        public ulong CaptainRoleId { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSRoomPlayerInfo : AMessage, IDisposable
    {
        public static CSRoomPlayerInfo Create(bool autoReturn = true)
        {
            var cSRoomPlayerInfo = MessageObjectPool<CSRoomPlayerInfo>.Rent();
            cSRoomPlayerInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSRoomPlayerInfo.SetIsPool(false);
            }
            
            return cSRoomPlayerInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleId = default;
            RoleName = default;
            Level = default;
            FightValue = default;
            MessageObjectPool<CSRoomPlayerInfo>.Return(this);
        }
        [ProtoMember(1)]
        public ulong RoleId { get; set; }
        [ProtoMember(2)]
        public string RoleName { get; set; }
        [ProtoMember(3)]
        public uint Level { get; set; }
        [ProtoMember(4)]
        public uint FightValue { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSSyncOneFrameData : AMessage, IDisposable
    {
        public static CSSyncOneFrameData Create(bool autoReturn = true)
        {
            var cSSyncOneFrameData = MessageObjectPool<CSSyncOneFrameData>.Rent();
            cSSyncOneFrameData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSSyncOneFrameData.SetIsPool(false);
            }
            
            return cSSyncOneFrameData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            FrameId = default;
            SpeedUp = default;
            PlayerCount = default;
            PlayerFrameData.Clear();
            MessageObjectPool<CSSyncOneFrameData>.Return(this);
        }
        [ProtoMember(1)]
        public int FrameId { get; set; }
        [ProtoMember(2)]
        public int SpeedUp { get; set; }
        [ProtoMember(3)]
        public int PlayerCount { get; set; }
        [ProtoMember(4)]
        public List<CSOnePlayerFrameCmd> PlayerFrameData { get; set; } = new List<CSOnePlayerFrameCmd>();
    }
    [Serializable]
    [ProtoContract]
    public partial class CSOnePlayerFrameCmd : AMessage, IDisposable
    {
        public static CSOnePlayerFrameCmd Create(bool autoReturn = true)
        {
            var cSOnePlayerFrameCmd = MessageObjectPool<CSOnePlayerFrameCmd>.Rent();
            cSOnePlayerFrameCmd.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSOnePlayerFrameCmd.SetIsPool(false);
            }
            
            return cSOnePlayerFrameCmd;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            PlayerId = default;
            FrameCmdCount = default;
            FrameDataList.Clear();
            MessageObjectPool<CSOnePlayerFrameCmd>.Return(this);
        }
        [ProtoMember(1)]
        public int PlayerId { get; set; }
        [ProtoMember(2)]
        public int FrameCmdCount { get; set; }
        [ProtoMember(3)]
        public List<CSFrameCmd> FrameDataList { get; set; } = new List<CSFrameCmd>();
    }
    [Serializable]
    [ProtoContract]
    public partial class CSFrameCmd : AMessage, IDisposable
    {
        public static CSFrameCmd Create(bool autoReturn = true)
        {
            var cSFrameCmd = MessageObjectPool<CSFrameCmd>.Rent();
            cSFrameCmd.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSFrameCmd.SetIsPool(false);
            }
            
            return cSFrameCmd;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Type = default;
            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
            MessageObjectPool<CSFrameCmd>.Return(this);
        }
        [ProtoMember(1)]
        public byte Type { get; set; }
        [ProtoMember(2)]
        public CSFrameData Data { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSFrameData : AMessage, IDisposable
    {
        public static CSFrameData Create(bool autoReturn = true)
        {
            var cSFrameData = MessageObjectPool<CSFrameData>.Rent();
            cSFrameData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSFrameData.SetIsPool(false);
            }
            
            return cSFrameData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (Gm != null)
            {
                Gm.Dispose();
                Gm = null;
            }
            MessageObjectPool<CSFrameData>.Return(this);
        }
        [ProtoMember(1)]
        public CSFrameCmdGmInfo Gm { get; set; }
    }
    [Serializable]
    [ProtoContract]
    public partial class CSFrameCmdGmInfo : AMessage, IDisposable
    {
        public static CSFrameCmdGmInfo Create(bool autoReturn = true)
        {
            var cSFrameCmdGmInfo = MessageObjectPool<CSFrameCmdGmInfo>.Rent();
            cSFrameCmdGmInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSFrameCmdGmInfo.SetIsPool(false);
            }
            
            return cSFrameCmdGmInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            GmString = default;
            MessageObjectPool<CSFrameCmdGmInfo>.Return(this);
        }
        [ProtoMember(1)]
        public string GmString { get; set; }
    }
    /// <summary>
    /// 发送聊天消息（经 Gate Roaming 转发到 Chat）
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Chat_SendMessageRequest : AMessage, IRoamingRequest
    {
        public static C2Chat_SendMessageRequest Create(bool autoReturn = true)
        {
            var c2Chat_SendMessageRequest = MessageObjectPool<C2Chat_SendMessageRequest>.Rent();
            c2Chat_SendMessageRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Chat_SendMessageRequest.SetIsPool(false);
            }
            
            return c2Chat_SendMessageRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (ChatInfoTree != null)
            {
                ChatInfoTree.Dispose();
                ChatInfoTree = null;
            }
            MessageObjectPool<C2Chat_SendMessageRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Chat_SendMessageRequest; } 
        [ProtoIgnore]
        public Chat2C_SendMessageResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.ChatRoamingType;
        [ProtoMember(1)]
        public ChatInfoTree ChatInfoTree { get; set; }
    }
    /// <summary>
    /// 发送聊天消息返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Chat2C_SendMessageResponse : AMessage, IRoamingResponse
    {
        public static Chat2C_SendMessageResponse Create(bool autoReturn = true)
        {
            var chat2C_SendMessageResponse = MessageObjectPool<Chat2C_SendMessageResponse>.Rent();
            chat2C_SendMessageResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chat2C_SendMessageResponse.SetIsPool(false);
            }
            
            return chat2C_SendMessageResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<Chat2C_SendMessageResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Chat2C_SendMessageResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// Chat 推送给客户端的聊天消息
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Chat2C_Message : AMessage, IMessage
    {
        public static Chat2C_Message Create(bool autoReturn = true)
        {
            var chat2C_Message = MessageObjectPool<Chat2C_Message>.Rent();
            chat2C_Message.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chat2C_Message.SetIsPool(false);
            }
            
            return chat2C_Message;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            if (ChatInfoTree != null)
            {
                ChatInfoTree.Dispose();
                ChatInfoTree = null;
            }
            MessageObjectPool<Chat2C_Message>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Chat2C_Message; } 
        [ProtoMember(1)]
        public ChatInfoTree ChatInfoTree { get; set; }
    }
    /// <summary>
    /// 聊天消息树
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class ChatInfoTree : AMessage, IDisposable
    {
        public static ChatInfoTree Create(bool autoReturn = true)
        {
            var chatInfoTree = MessageObjectPool<ChatInfoTree>.Rent();
            chatInfoTree.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chatInfoTree.SetIsPool(false);
            }
            
            return chatInfoTree;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ChatChannelType = default;
            ChatChannelId = default;
            UnitId = default;
            UserName = default;
            Target.Clear();
            Node.Clear();
            MessageObjectPool<ChatInfoTree>.Return(this);
        }
        [ProtoMember(1)]
        public int ChatChannelType { get; set; }
        [ProtoMember(2)]
        public long ChatChannelId { get; set; }
        [ProtoMember(3)]
        public long UnitId { get; set; }
        [ProtoMember(4)]
        public string UserName { get; set; }
        [ProtoMember(5)]
        public List<long> Target { get; set; } = new List<long>();
        [ProtoMember(6)]
        public List<ChatInfoNode> Node { get; set; } = new List<ChatInfoNode>();
    }
    /// <summary>
    /// 聊天信息节点
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class ChatInfoNode : AMessage, IDisposable
    {
        public static ChatInfoNode Create(bool autoReturn = true)
        {
            var chatInfoNode = MessageObjectPool<ChatInfoNode>.Rent();
            chatInfoNode.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chatInfoNode.SetIsPool(false);
            }
            
            return chatInfoNode;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ChatNodeType = default;
            ChatNodeEvent = default;
            Content = default;
            Color = default;
            Data = null;
            MessageObjectPool<ChatInfoNode>.Return(this);
        }
        [ProtoMember(1)]
        public int ChatNodeType { get; set; }
        [ProtoMember(2)]
        public int ChatNodeEvent { get; set; }
        [ProtoMember(3)]
        public string Content { get; set; }
        [ProtoMember(4)]
        public string Color { get; set; }
        [ProtoMember(5)]
        public byte[] Data { get; set; }
    }
    /// <summary>
    /// 聊天位置信息节点
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class ChatPositionNode : AMessage, IDisposable
    {
        public static ChatPositionNode Create(bool autoReturn = true)
        {
            var chatPositionNode = MessageObjectPool<ChatPositionNode>.Rent();
            chatPositionNode.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chatPositionNode.SetIsPool(false);
            }
            
            return chatPositionNode;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MapName = default;
            PosX = default;
            PosY = default;
            PosZ = default;
            MessageObjectPool<ChatPositionNode>.Return(this);
        }
        [ProtoMember(1)]
        public string MapName { get; set; }
        [ProtoMember(2)]
        public float PosX { get; set; }
        [ProtoMember(3)]
        public float PosY { get; set; }
        [ProtoMember(4)]
        public float PosZ { get; set; }
    }
    /// <summary>
    /// 聊天打开 UI 节点
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class ChatOpenUINode : AMessage, IDisposable
    {
        public static ChatOpenUINode Create(bool autoReturn = true)
        {
            var chatOpenUINode = MessageObjectPool<ChatOpenUINode>.Rent();
            chatOpenUINode.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chatOpenUINode.SetIsPool(false);
            }
            
            return chatOpenUINode;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            UIName = default;
            MessageObjectPool<ChatOpenUINode>.Return(this);
        }
        [ProtoMember(1)]
        public string UIName { get; set; }
    }
    /// <summary>
    /// 聊天链接信息节点
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class ChatLinkNode : AMessage, IDisposable
    {
        public static ChatLinkNode Create(bool autoReturn = true)
        {
            var chatLinkNode = MessageObjectPool<ChatLinkNode>.Rent();
            chatLinkNode.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chatLinkNode.SetIsPool(false);
            }
            
            return chatLinkNode;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Link = default;
            MessageObjectPool<ChatLinkNode>.Return(this);
        }
        [ProtoMember(1)]
        public string Link { get; set; }
    }
    /// <summary>
    /// 装备/道具信息实体（演示用）
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class ChatItem : AMessage, IDisposable
    {
        public static ChatItem Create(bool autoReturn = true)
        {
            var chatItem = MessageObjectPool<ChatItem>.Rent();
            chatItem.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                chatItem.SetIsPool(false);
            }
            
            return chatItem;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Level = default;
            Name = default;
            HP = default;
            MP = default;
            MessageObjectPool<ChatItem>.Return(this);
        }
        [ProtoMember(1)]
        public string Level { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string HP { get; set; }
        [ProtoMember(4)]
        public string MP { get; set; }
    }
    /// <summary>
    /// 好友条目
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class CSFriendInfo : AMessage, IDisposable
    {
        public static CSFriendInfo Create(bool autoReturn = true)
        {
            var cSFriendInfo = MessageObjectPool<CSFriendInfo>.Rent();
            cSFriendInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSFriendInfo.SetIsPool(false);
            }
            
            return cSFriendInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleId = default;
            RoleName = default;
            HeadID = default;
            Level = default;
            State = default;
            MessageObjectPool<CSFriendInfo>.Return(this);
        }
        [ProtoMember(1)]
        public ulong RoleId { get; set; }
        [ProtoMember(2)]
        public string RoleName { get; set; }
        [ProtoMember(3)]
        public int HeadID { get; set; }
        [ProtoMember(4)]
        public uint Level { get; set; }
        [ProtoMember(5)]
        public int State { get; set; }
    }
    /// <summary>
    /// 添加好友（发申请或接受 Incoming）
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_AddFriendRequest : AMessage, IRoamingRequest
    {
        public static C2Game_AddFriendRequest Create(bool autoReturn = true)
        {
            var c2Game_AddFriendRequest = MessageObjectPool<C2Game_AddFriendRequest>.Rent();
            c2Game_AddFriendRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_AddFriendRequest.SetIsPool(false);
            }
            
            return c2Game_AddFriendRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            TargetRoleId = default;
            TargetRoleName = default;
            MessageObjectPool<C2Game_AddFriendRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_AddFriendRequest; } 
        [ProtoIgnore]
        public Game2C_AddFriendResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public ulong TargetRoleId { get; set; }
        [ProtoMember(2)]
        public string TargetRoleName { get; set; }
    }
    /// <summary>
    /// 添加好友返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_AddFriendResponse : AMessage, IRoamingResponse
    {
        public static Game2C_AddFriendResponse Create(bool autoReturn = true)
        {
            var game2C_AddFriendResponse = MessageObjectPool<Game2C_AddFriendResponse>.Rent();
            game2C_AddFriendResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_AddFriendResponse.SetIsPool(false);
            }
            
            return game2C_AddFriendResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            if (Friend != null)
            {
                Friend.Dispose();
                Friend = null;
            }
            MessageObjectPool<Game2C_AddFriendResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_AddFriendResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public CSFriendInfo Friend { get; set; }
    }
    /// <summary>
    /// 删除好友 / 拒绝申请
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_DeleteFriendRequest : AMessage, IRoamingRequest
    {
        public static C2Game_DeleteFriendRequest Create(bool autoReturn = true)
        {
            var c2Game_DeleteFriendRequest = MessageObjectPool<C2Game_DeleteFriendRequest>.Rent();
            c2Game_DeleteFriendRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_DeleteFriendRequest.SetIsPool(false);
            }
            
            return c2Game_DeleteFriendRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            TargetRoleId = default;
            TargetRoleName = default;
            MessageObjectPool<C2Game_DeleteFriendRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_DeleteFriendRequest; } 
        [ProtoIgnore]
        public Game2C_DeleteFriendResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public ulong TargetRoleId { get; set; }
        [ProtoMember(2)]
        public string TargetRoleName { get; set; }
    }
    /// <summary>
    /// 删除好友返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_DeleteFriendResponse : AMessage, IRoamingResponse
    {
        public static Game2C_DeleteFriendResponse Create(bool autoReturn = true)
        {
            var game2C_DeleteFriendResponse = MessageObjectPool<Game2C_DeleteFriendResponse>.Rent();
            game2C_DeleteFriendResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_DeleteFriendResponse.SetIsPool(false);
            }
            
            return game2C_DeleteFriendResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<Game2C_DeleteFriendResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_DeleteFriendResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// 屏蔽用户
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_BlockFriendRequest : AMessage, IRoamingRequest
    {
        public static C2Game_BlockFriendRequest Create(bool autoReturn = true)
        {
            var c2Game_BlockFriendRequest = MessageObjectPool<C2Game_BlockFriendRequest>.Rent();
            c2Game_BlockFriendRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_BlockFriendRequest.SetIsPool(false);
            }
            
            return c2Game_BlockFriendRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            TargetRoleId = default;
            TargetRoleName = default;
            MessageObjectPool<C2Game_BlockFriendRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_BlockFriendRequest; } 
        [ProtoIgnore]
        public Game2C_BlockFriendResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public ulong TargetRoleId { get; set; }
        [ProtoMember(2)]
        public string TargetRoleName { get; set; }
    }
    /// <summary>
    /// 屏蔽用户返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_BlockFriendResponse : AMessage, IRoamingResponse
    {
        public static Game2C_BlockFriendResponse Create(bool autoReturn = true)
        {
            var game2C_BlockFriendResponse = MessageObjectPool<Game2C_BlockFriendResponse>.Rent();
            game2C_BlockFriendResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_BlockFriendResponse.SetIsPool(false);
            }
            
            return game2C_BlockFriendResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            if (Friend != null)
            {
                Friend.Dispose();
                Friend = null;
            }
            MessageObjectPool<Game2C_BlockFriendResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_BlockFriendResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public CSFriendInfo Friend { get; set; }
    }
    /// <summary>
    /// 按状态列出好友
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_ListFriendsRequest : AMessage, IRoamingRequest
    {
        public static C2Game_ListFriendsRequest Create(bool autoReturn = true)
        {
            var c2Game_ListFriendsRequest = MessageObjectPool<C2Game_ListFriendsRequest>.Rent();
            c2Game_ListFriendsRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_ListFriendsRequest.SetIsPool(false);
            }
            
            return c2Game_ListFriendsRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            State = default;
            Limit = default;
            Cursor = default;
            MessageObjectPool<C2Game_ListFriendsRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_ListFriendsRequest; } 
        [ProtoIgnore]
        public Game2C_ListFriendsResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public int State { get; set; }
        [ProtoMember(2)]
        public int Limit { get; set; }
        [ProtoMember(3)]
        public string Cursor { get; set; }
    }
    /// <summary>
    /// 列出好友返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_ListFriendsResponse : AMessage, IRoamingResponse
    {
        public static Game2C_ListFriendsResponse Create(bool autoReturn = true)
        {
            var game2C_ListFriendsResponse = MessageObjectPool<Game2C_ListFriendsResponse>.Rent();
            game2C_ListFriendsResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_ListFriendsResponse.SetIsPool(false);
            }
            
            return game2C_ListFriendsResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            Friends = null;
            Cursor = default;
            MessageObjectPool<Game2C_ListFriendsResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_ListFriendsResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public List<CSFriendInfo> Friends { get; set; }
        [ProtoMember(3)]
        public string Cursor { get; set; }
    }
    /// <summary>
    /// 推荐好友
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_ListRecommendFriendsRequest : AMessage, IRoamingRequest
    {
        public static C2Game_ListRecommendFriendsRequest Create(bool autoReturn = true)
        {
            var c2Game_ListRecommendFriendsRequest = MessageObjectPool<C2Game_ListRecommendFriendsRequest>.Rent();
            c2Game_ListRecommendFriendsRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_ListRecommendFriendsRequest.SetIsPool(false);
            }
            
            return c2Game_ListRecommendFriendsRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Limit = default;
            MessageObjectPool<C2Game_ListRecommendFriendsRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_ListRecommendFriendsRequest; } 
        [ProtoIgnore]
        public Game2C_ListRecommendFriendsResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public int Limit { get; set; }
    }
    /// <summary>
    /// 推荐好友返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_ListRecommendFriendsResponse : AMessage, IRoamingResponse
    {
        public static Game2C_ListRecommendFriendsResponse Create(bool autoReturn = true)
        {
            var game2C_ListRecommendFriendsResponse = MessageObjectPool<Game2C_ListRecommendFriendsResponse>.Rent();
            game2C_ListRecommendFriendsResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_ListRecommendFriendsResponse.SetIsPool(false);
            }
            
            return game2C_ListRecommendFriendsResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            Friends = null;
            MessageObjectPool<Game2C_ListRecommendFriendsResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_ListRecommendFriendsResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public List<CSFriendInfo> Friends { get; set; }
    }
    /// <summary>
    /// 好友关系变更推送 Op: 1=Upsert 2=Remove
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_FriendChangedNotify : AMessage, IMessage
    {
        public static G2C_FriendChangedNotify Create(bool autoReturn = true)
        {
            var g2C_FriendChangedNotify = MessageObjectPool<G2C_FriendChangedNotify>.Rent();
            g2C_FriendChangedNotify.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_FriendChangedNotify.SetIsPool(false);
            }
            
            return g2C_FriendChangedNotify;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Op = default;
            if (Friend != null)
            {
                Friend.Dispose();
                Friend = null;
            }
            MessageObjectPool<G2C_FriendChangedNotify>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_FriendChangedNotify; } 
        [ProtoMember(1)]
        public int Op { get; set; }
        [ProtoMember(2)]
        public CSFriendInfo Friend { get; set; }
    }
    /// <summary>
    /// 查询功能开放列表
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2G_QueryFuncOpenListRequest : AMessage, IRequest
    {
        public static C2G_QueryFuncOpenListRequest Create(bool autoReturn = true)
        {
            var c2G_QueryFuncOpenListRequest = MessageObjectPool<C2G_QueryFuncOpenListRequest>.Rent();
            c2G_QueryFuncOpenListRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_QueryFuncOpenListRequest.SetIsPool(false);
            }
            
            return c2G_QueryFuncOpenListRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<C2G_QueryFuncOpenListRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_QueryFuncOpenListRequest; } 
        [ProtoIgnore]
        public G2C_QueryFuncOpenListResponse ResponseType { get; set; }
    }
    /// <summary>
    /// 功能开放列表返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_QueryFuncOpenListResponse : AMessage, IResponse
    {
        public static G2C_QueryFuncOpenListResponse Create(bool autoReturn = true)
        {
            var g2C_QueryFuncOpenListResponse = MessageObjectPool<G2C_QueryFuncOpenListResponse>.Rent();
            g2C_QueryFuncOpenListResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_QueryFuncOpenListResponse.SetIsPool(false);
            }
            
            return g2C_QueryFuncOpenListResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            OpenFuncList = null;
            MessageObjectPool<G2C_QueryFuncOpenListResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_QueryFuncOpenListResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        /// <summary>
        /// 已开放功能ID列表
        /// </summary>
        [ProtoMember(2)]
        public List<int> OpenFuncList { get; set; }
    }
    /// <summary>
    /// 新增功能开放通知
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_FuncOpenNotify : AMessage, IMessage
    {
        public static G2C_FuncOpenNotify Create(bool autoReturn = true)
        {
            var g2C_FuncOpenNotify = MessageObjectPool<G2C_FuncOpenNotify>.Rent();
            g2C_FuncOpenNotify.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_FuncOpenNotify.SetIsPool(false);
            }
            
            return g2C_FuncOpenNotify;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            NewOpenFuncList = null;
            MessageObjectPool<G2C_FuncOpenNotify>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_FuncOpenNotify; } 
        /// <summary>
        /// 新增开放的功能ID列表
        /// </summary>
        [ProtoMember(1)]
        public List<int> NewOpenFuncList { get; set; }
    }
    /// <summary>
    /// 注册账号协议
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2A_RegisterRequest : AMessage, IRequest
    {
        public static C2A_RegisterRequest Create(bool autoReturn = true)
        {
            var c2A_RegisterRequest = MessageObjectPool<C2A_RegisterRequest>.Rent();
            c2A_RegisterRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2A_RegisterRequest.SetIsPool(false);
            }
            
            return c2A_RegisterRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            UserName = default;
            Password = default;
            MessageObjectPool<C2A_RegisterRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2A_RegisterRequest; } 
        [ProtoIgnore]
        public A2C_RegisterResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string UserName { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
    }
    /// <summary>
    /// 注册账号协议返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class A2C_RegisterResponse : AMessage, IResponse
    {
        public static A2C_RegisterResponse Create(bool autoReturn = true)
        {
            var a2C_RegisterResponse = MessageObjectPool<A2C_RegisterResponse>.Rent();
            a2C_RegisterResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                a2C_RegisterResponse.SetIsPool(false);
            }
            
            return a2C_RegisterResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<A2C_RegisterResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.A2C_RegisterResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// 登录账号协议
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2A_LoginRequest : AMessage, IRequest
    {
        public static C2A_LoginRequest Create(bool autoReturn = true)
        {
            var c2A_LoginRequest = MessageObjectPool<C2A_LoginRequest>.Rent();
            c2A_LoginRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2A_LoginRequest.SetIsPool(false);
            }
            
            return c2A_LoginRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            UserName = default;
            Password = default;
            LoginType = default;
            MessageObjectPool<C2A_LoginRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2A_LoginRequest; } 
        [ProtoIgnore]
        public A2C_LoginResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string UserName { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
        [ProtoMember(3)]
        public uint LoginType { get; set; }
    }
    /// <summary>
    /// 登录账号协议返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class A2C_LoginResponse : AMessage, IResponse
    {
        public static A2C_LoginResponse Create(bool autoReturn = true)
        {
            var a2C_LoginResponse = MessageObjectPool<A2C_LoginResponse>.Rent();
            a2C_LoginResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                a2C_LoginResponse.SetIsPool(false);
            }
            
            return a2C_LoginResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            RoleID = default;
            Token = default;
            ServerInfoList = null;
            RecentServerList = null;
            RecentServerRoleInfoList = null;
            GateAddress = default;
            MessageObjectPool<A2C_LoginResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.A2C_LoginResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public long RoleID { get; set; }
        [ProtoMember(3)]
        public string Token { get; set; }
        [ProtoMember(4)]
        public List<CSServerInfo> ServerInfoList { get; set; }
        [ProtoMember(5)]
        public List<int> RecentServerList { get; set; }
        [ProtoMember(6)]
        public List<CSRecentServerRoleInfo> RecentServerRoleInfoList { get; set; }
        [ProtoMember(7)]
        public string GateAddress { get; set; }
    }
    /// <summary>
    /// 服务器信息
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class CSServerInfo : AMessage, IDisposable
    {
        public static CSServerInfo Create(bool autoReturn = true)
        {
            var cSServerInfo = MessageObjectPool<CSServerInfo>.Rent();
            cSServerInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSServerInfo.SetIsPool(false);
            }
            
            return cSServerInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ServerID = default;
            Name = default;
            Group = default;
            Address = default;
            Port = default;
            Recommend = default;
            State = default;
            MessageObjectPool<CSServerInfo>.Return(this);
        }
        /// <summary>
        /// 服务器ID
        /// </summary>
        [ProtoMember(1)]
        public int ServerID { get; set; }
        /// <summary>
        /// 服务器名字
        /// </summary>
        [ProtoMember(2)]
        public string Name { get; set; }
        /// <summary>
        /// 服务器分组
        /// </summary>
        [ProtoMember(3)]
        public int Group { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        [ProtoMember(4)]
        public string Address { get; set; }
        /// <summary>
        /// 服务器端口号
        /// </summary>
        [ProtoMember(5)]
        public int Port { get; set; }
        /// <summary>
        /// 是否是推荐服务器
        /// </summary>
        [ProtoMember(6)]
        public bool Recommend { get; set; }
        /// <summary>
        /// 服务器状态
        /// </summary>
        [ProtoMember(7)]
        public byte State { get; set; }
    }
    /// <summary>
    /// 最近登录服务器的角色摘要
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class CSRecentServerRoleInfo : AMessage, IDisposable
    {
        public static CSRecentServerRoleInfo Create(bool autoReturn = true)
        {
            var cSRecentServerRoleInfo = MessageObjectPool<CSRecentServerRoleInfo>.Rent();
            cSRecentServerRoleInfo.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSRecentServerRoleInfo.SetIsPool(false);
            }
            
            return cSRecentServerRoleInfo;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ServerID = default;
            Level = default;
            MessageObjectPool<CSRecentServerRoleInfo>.Return(this);
        }
        /// <summary>
        /// 服务器ID
        /// </summary>
        [ProtoMember(1)]
        public int ServerID { get; set; }
        /// <summary>
        /// 角色等级
        /// </summary>
        [ProtoMember(2)]
        public uint Level { get; set; }
    }
    /// <summary>
    /// 同步当前选择的服务器
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2A_RecordRecentServer : AMessage, IMessage
    {
        public static C2A_RecordRecentServer Create(bool autoReturn = true)
        {
            var c2A_RecordRecentServer = MessageObjectPool<C2A_RecordRecentServer>.Rent();
            c2A_RecordRecentServer.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2A_RecordRecentServer.SetIsPool(false);
            }
            
            return c2A_RecordRecentServer;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleID = default;
            ServerID = default;
            MessageObjectPool<C2A_RecordRecentServer>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2A_RecordRecentServer; } 
        [ProtoMember(1)]
        public long RoleID { get; set; }
        /// <summary>
        /// 服务器ID
        /// </summary>
        [ProtoMember(2)]
        public int ServerID { get; set; }
    }
    /// <summary>
    /// 客户端登录到Gate服务器
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2G_LoginRequest : AMessage, IRequest
    {
        public static C2G_LoginRequest Create(bool autoReturn = true)
        {
            var c2G_LoginRequest = MessageObjectPool<C2G_LoginRequest>.Rent();
            c2G_LoginRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2G_LoginRequest.SetIsPool(false);
            }
            
            return c2G_LoginRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Token = default;
            ServerID = default;
            MessageObjectPool<C2G_LoginRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2G_LoginRequest; } 
        [ProtoIgnore]
        public G2C_LoginResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int ServerID { get; set; }
    }
    /// <summary>
    /// 登录协议返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_LoginResponse : AMessage, IResponse
    {
        public static G2C_LoginResponse Create(bool autoReturn = true)
        {
            var g2C_LoginResponse = MessageObjectPool<G2C_LoginResponse>.Rent();
            g2C_LoginResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_LoginResponse.SetIsPool(false);
            }
            
            return g2C_LoginResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            if (PlayerData != null)
            {
                PlayerData.Dispose();
                PlayerData = null;
            }
            MessageObjectPool<G2C_LoginResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_LoginResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public CSPlayerData PlayerData { get; set; }
    }
    /// <summary>
    /// 通知客户端重复登录
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_RepeatLogin : AMessage, IMessage
    {
        public static G2C_RepeatLogin Create(bool autoReturn = true)
        {
            var g2C_RepeatLogin = MessageObjectPool<G2C_RepeatLogin>.Rent();
            g2C_RepeatLogin.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_RepeatLogin.SetIsPool(false);
            }
            
            return g2C_RepeatLogin;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<G2C_RepeatLogin>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_RepeatLogin; } 
    }
    /// <summary>
    /// 玩家角色基础数据
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class CSPlayerData : AMessage, IDisposable
    {
        public static CSPlayerData Create(bool autoReturn = true)
        {
            var cSPlayerData = MessageObjectPool<CSPlayerData>.Rent();
            cSPlayerData.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSPlayerData.SetIsPool(false);
            }
            
            return cSPlayerData;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleID = default;
            RoleNo = default;
            RoleName = default;
            HeadID = default;
            Sex = default;
            Level = default;
            Exp = default;
            FightValue = default;
            Diamond = default;
            Gold = default;
            Stam = default;
            LastLoginTime = default;
            CreateTime = default;
            IsFinGuide = default;
            Sign = default;
            WorldID = default;
            TotalRmb = default;
            LastAddStamTime = default;
            DailyBuyStamCount = default;
            MessageObjectPool<CSPlayerData>.Return(this);
        }
        /// <summary>
        /// RoleID
        /// </summary>
        [ProtoMember(1)]
        public ulong RoleID { get; set; }
        /// <summary>
        /// RoleNO
        /// </summary>
        [ProtoMember(2)]
        public ulong RoleNo { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [ProtoMember(3)]
        public string RoleName { get; set; }
        /// <summary>
        /// 头像ID
        /// </summary>
        [ProtoMember(4)]
        public int HeadID { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [ProtoMember(5)]
        public byte Sex { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        [ProtoMember(6)]
        public uint Level { get; set; }
        /// <summary>
        /// 经验
        /// </summary>
        [ProtoMember(7)]
        public uint Exp { get; set; }
        /// <summary>
        /// 战斗力
        /// </summary>
        [ProtoMember(8)]
        public uint FightValue { get; set; }
        /// <summary>
        /// 钻石
        /// </summary>
        [ProtoMember(9)]
        public uint Diamond { get; set; }
        /// <summary>
        /// 金币
        /// </summary>
        [ProtoMember(10)]
        public uint Gold { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        [ProtoMember(11)]
        public uint Stam { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        [ProtoMember(12)]
        public long LastLoginTime { get; set; }
        /// <summary>
        /// 创角时间
        /// </summary>
        [ProtoMember(13)]
        public long CreateTime { get; set; }
        /// <summary>
        /// 是否完成新手引导
        /// </summary>
        [ProtoMember(14)]
        public byte IsFinGuide { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        [ProtoMember(15)]
        public string Sign { get; set; }
        /// <summary>
        /// 所在主服
        /// </summary>
        [ProtoMember(16)]
        public int WorldID { get; set; }
        /// <summary>
        /// 累计充值金额
        /// </summary>
        [ProtoMember(17)]
        public uint TotalRmb { get; set; }
        /// <summary>
        /// 上次增加体力时间
        /// </summary>
        [ProtoMember(18)]
        public long LastAddStamTime { get; set; }
        /// <summary>
        /// 每日购买体力次数
        /// </summary>
        [ProtoMember(19)]
        public int DailyBuyStamCount { get; set; }
    }
    /// <summary>
    /// 通过 Game Roaming 获取当前玩家角色数据
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_GetPlayerDataRequest : AMessage, IRoamingRequest
    {
        public static C2Game_GetPlayerDataRequest Create(bool autoReturn = true)
        {
            var c2Game_GetPlayerDataRequest = MessageObjectPool<C2Game_GetPlayerDataRequest>.Rent();
            c2Game_GetPlayerDataRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_GetPlayerDataRequest.SetIsPool(false);
            }
            
            return c2Game_GetPlayerDataRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            MessageObjectPool<C2Game_GetPlayerDataRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_GetPlayerDataRequest; } 
        [ProtoIgnore]
        public Game2C_GetPlayerDataResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
    }
    /// <summary>
    /// 获取玩家角色数据返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_GetPlayerDataResponse : AMessage, IRoamingResponse
    {
        public static Game2C_GetPlayerDataResponse Create(bool autoReturn = true)
        {
            var game2C_GetPlayerDataResponse = MessageObjectPool<Game2C_GetPlayerDataResponse>.Rent();
            game2C_GetPlayerDataResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_GetPlayerDataResponse.SetIsPool(false);
            }
            
            return game2C_GetPlayerDataResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            if (PlayerData != null)
            {
                PlayerData.Dispose();
                PlayerData = null;
            }
            MessageObjectPool<Game2C_GetPlayerDataResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_GetPlayerDataResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public CSPlayerData PlayerData { get; set; }
    }
    /// <summary>
    /// 单个状态显示信息
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class CSStatusPresence : AMessage, IDisposable
    {
        public static CSStatusPresence Create(bool autoReturn = true)
        {
            var cSStatusPresence = MessageObjectPool<CSStatusPresence>.Rent();
            cSStatusPresence.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                cSStatusPresence.SetIsPool(false);
            }
            
            return cSStatusPresence;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleId = default;
            RoleName = default;
            StatusText = default;
            MessageObjectPool<CSStatusPresence>.Return(this);
        }
        [ProtoMember(1)]
        public ulong RoleId { get; set; }
        [ProtoMember(2)]
        public string RoleName { get; set; }
        [ProtoMember(3)]
        public string StatusText { get; set; }
    }
    /// <summary>
    /// 关注用户
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_FollowUsersRequest : AMessage, IRoamingRequest
    {
        public static C2Game_FollowUsersRequest Create(bool autoReturn = true)
        {
            var c2Game_FollowUsersRequest = MessageObjectPool<C2Game_FollowUsersRequest>.Rent();
            c2Game_FollowUsersRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_FollowUsersRequest.SetIsPool(false);
            }
            
            return c2Game_FollowUsersRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleIds = null;
            MessageObjectPool<C2Game_FollowUsersRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_FollowUsersRequest; } 
        [ProtoIgnore]
        public Game2C_FollowUsersResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public List<ulong> RoleIds { get; set; }
    }
    /// <summary>
    /// 关注用户返回（当前在线快照）
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_FollowUsersResponse : AMessage, IRoamingResponse
    {
        public static Game2C_FollowUsersResponse Create(bool autoReturn = true)
        {
            var game2C_FollowUsersResponse = MessageObjectPool<Game2C_FollowUsersResponse>.Rent();
            game2C_FollowUsersResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_FollowUsersResponse.SetIsPool(false);
            }
            
            return game2C_FollowUsersResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            Presences = null;
            MessageObjectPool<Game2C_FollowUsersResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_FollowUsersResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public List<CSStatusPresence> Presences { get; set; }
    }
    /// <summary>
    /// 取消关注用户
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_UnfollowUsersRequest : AMessage, IRoamingRequest
    {
        public static C2Game_UnfollowUsersRequest Create(bool autoReturn = true)
        {
            var c2Game_UnfollowUsersRequest = MessageObjectPool<C2Game_UnfollowUsersRequest>.Rent();
            c2Game_UnfollowUsersRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_UnfollowUsersRequest.SetIsPool(false);
            }
            
            return c2Game_UnfollowUsersRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            RoleIds = null;
            MessageObjectPool<C2Game_UnfollowUsersRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_UnfollowUsersRequest; } 
        [ProtoIgnore]
        public Game2C_UnfollowUsersResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public List<ulong> RoleIds { get; set; }
    }
    /// <summary>
    /// 取消关注用户返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_UnfollowUsersResponse : AMessage, IRoamingResponse
    {
        public static Game2C_UnfollowUsersResponse Create(bool autoReturn = true)
        {
            var game2C_UnfollowUsersResponse = MessageObjectPool<Game2C_UnfollowUsersResponse>.Rent();
            game2C_UnfollowUsersResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_UnfollowUsersResponse.SetIsPool(false);
            }
            
            return game2C_UnfollowUsersResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<Game2C_UnfollowUsersResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_UnfollowUsersResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// 更新自身状态文案
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class C2Game_UpdateStatusRequest : AMessage, IRoamingRequest
    {
        public static C2Game_UpdateStatusRequest Create(bool autoReturn = true)
        {
            var c2Game_UpdateStatusRequest = MessageObjectPool<C2Game_UpdateStatusRequest>.Rent();
            c2Game_UpdateStatusRequest.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                c2Game_UpdateStatusRequest.SetIsPool(false);
            }
            
            return c2Game_UpdateStatusRequest;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            StatusText = default;
            MessageObjectPool<C2Game_UpdateStatusRequest>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.C2Game_UpdateStatusRequest; } 
        [ProtoIgnore]
        public Game2C_UpdateStatusResponse ResponseType { get; set; }
        [ProtoIgnore]
        public int RouteType => Fantasy.RoamingType.GameRoamingType;
        [ProtoMember(1)]
        public string StatusText { get; set; }
    }
    /// <summary>
    /// 更新自身状态返回
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class Game2C_UpdateStatusResponse : AMessage, IRoamingResponse
    {
        public static Game2C_UpdateStatusResponse Create(bool autoReturn = true)
        {
            var game2C_UpdateStatusResponse = MessageObjectPool<Game2C_UpdateStatusResponse>.Rent();
            game2C_UpdateStatusResponse.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                game2C_UpdateStatusResponse.SetIsPool(false);
            }
            
            return game2C_UpdateStatusResponse;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            ErrorCode = 0;
            MessageObjectPool<Game2C_UpdateStatusResponse>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.Game2C_UpdateStatusResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
    }
    /// <summary>
    /// 状态显示变更推送
    /// </summary>
    [Serializable]
    [ProtoContract]
    public partial class G2C_StatusPresenceNotify : AMessage, IMessage
    {
        public static G2C_StatusPresenceNotify Create(bool autoReturn = true)
        {
            var g2C_StatusPresenceNotify = MessageObjectPool<G2C_StatusPresenceNotify>.Rent();
            g2C_StatusPresenceNotify.AutoReturn = autoReturn;
            
            if (!autoReturn)
            {
                g2C_StatusPresenceNotify.SetIsPool(false);
            }
            
            return g2C_StatusPresenceNotify;
        }
        
        public void Return()
        {
            if (!AutoReturn)
            {
                SetIsPool(true);
                AutoReturn = true;
            }
            else if (!IsPool())
            {
                return;
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!IsPool()) return; 
            Joins = null;
            Leaves = null;
            MessageObjectPool<G2C_StatusPresenceNotify>.Return(this);
        }
        public uint OpCode() { return OuterOpcode.G2C_StatusPresenceNotify; } 
        [ProtoMember(1)]
        public List<CSStatusPresence> Joins { get; set; }
        [ProtoMember(2)]
        public List<CSStatusPresence> Leaves { get; set; }
    }
}