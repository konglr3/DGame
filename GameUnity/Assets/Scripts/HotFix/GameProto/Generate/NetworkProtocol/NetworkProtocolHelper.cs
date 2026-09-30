using System.Runtime.CompilerServices;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using System.Collections.Generic;
#pragma warning disable CS8618
namespace Fantasy
{
   public static class NetworkProtocolHelper
   {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_CreateRoomResponse> C2G_CreateRoomRequest(this Session session, C2G_CreateRoomRequest C2G_CreateRoomRequest_request)
		{
			return (G2C_CreateRoomResponse)await session.Call(C2G_CreateRoomRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_CreateRoomResponse> C2G_CreateRoomRequest(this Session session, int playerCount)
		{
			using var C2G_CreateRoomRequest_request = Fantasy.C2G_CreateRoomRequest.Create();
			C2G_CreateRoomRequest_request.PlayerCount = playerCount;
			return (G2C_CreateRoomResponse)await session.Call(C2G_CreateRoomRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_JoinRoomResponse> C2G_JoinRoomRequest(this Session session, C2G_JoinRoomRequest C2G_JoinRoomRequest_request)
		{
			return (G2C_JoinRoomResponse)await session.Call(C2G_JoinRoomRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_JoinRoomResponse> C2G_JoinRoomRequest(this Session session, int roomId)
		{
			using var C2G_JoinRoomRequest_request = Fantasy.C2G_JoinRoomRequest.Create();
			C2G_JoinRoomRequest_request.RoomId = roomId;
			return (G2C_JoinRoomResponse)await session.Call(C2G_JoinRoomRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LeaveRoomResponse> C2G_LeaveRoomRequest(this Session session, C2G_LeaveRoomRequest C2G_LeaveRoomRequest_request)
		{
			return (G2C_LeaveRoomResponse)await session.Call(C2G_LeaveRoomRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LeaveRoomResponse> C2G_LeaveRoomRequest(this Session session)
		{
			using var C2G_LeaveRoomRequest_request = Fantasy.C2G_LeaveRoomRequest.Create();
			return (G2C_LeaveRoomResponse)await session.Call(C2G_LeaveRoomRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_RoomPlayerInfoChangedNotify(this Session session, G2C_RoomPlayerInfoChangedNotify G2C_RoomPlayerInfoChangedNotify_message)
		{
			session.Send(G2C_RoomPlayerInfoChangedNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_RoomPlayerInfoChangedNotify(this Session session, CSRoomInfo roomInfo, int playerCount, List<CSRoomPlayerInfo> playerInfos)
		{
			using var G2C_RoomPlayerInfoChangedNotify_message = Fantasy.G2C_RoomPlayerInfoChangedNotify.Create();
			G2C_RoomPlayerInfoChangedNotify_message.RoomInfo = roomInfo;
			G2C_RoomPlayerInfoChangedNotify_message.PlayerCount = playerCount;
			G2C_RoomPlayerInfoChangedNotify_message.PlayerInfos = playerInfos;
			session.Send(G2C_RoomPlayerInfoChangedNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2S_SyncFrameDataReq(this Session session, C2S_SyncFrameDataReq C2S_SyncFrameDataReq_message)
		{
			session.Send(C2S_SyncFrameDataReq_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2S_SyncFrameDataReq(this Session session, int forecastFrameId, int revClientFrameId, CSRoomInfo roomInfo, int roomPlayerId, CSOnePlayerFrameCmd frameData)
		{
			using var C2S_SyncFrameDataReq_message = Fantasy.C2S_SyncFrameDataReq.Create();
			C2S_SyncFrameDataReq_message.ForecastFrameId = forecastFrameId;
			C2S_SyncFrameDataReq_message.RevClientFrameId = revClientFrameId;
			C2S_SyncFrameDataReq_message.RoomInfo = roomInfo;
			C2S_SyncFrameDataReq_message.RoomPlayerId = roomPlayerId;
			C2S_SyncFrameDataReq_message.FrameData = frameData;
			session.Send(C2S_SyncFrameDataReq_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_BattleFinClientDataReq(this Session session, S2C_BattleFinClientDataReq S2C_BattleFinClientDataReq_message)
		{
			session.Send(S2C_BattleFinClientDataReq_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_BattleFinClientDataReq(this Session session, CSBattleStartParam startParam, uint durationTime)
		{
			using var S2C_BattleFinClientDataReq_message = Fantasy.S2C_BattleFinClientDataReq.Create();
			S2C_BattleFinClientDataReq_message.StartParam = startParam;
			S2C_BattleFinClientDataReq_message.DurationTime = durationTime;
			session.Send(S2C_BattleFinClientDataReq_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<S2C_StartBattleResponse> C2S_StartBattleRequest(this Session session, C2S_StartBattleRequest C2S_StartBattleRequest_request)
		{
			return (S2C_StartBattleResponse)await session.Call(C2S_StartBattleRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<S2C_StartBattleResponse> C2S_StartBattleRequest(this Session session)
		{
			using var C2S_StartBattleRequest_request = Fantasy.C2S_StartBattleRequest.Create();
			return (S2C_StartBattleResponse)await session.Call(C2S_StartBattleRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_NotifyBattleLoading(this Session session, S2C_NotifyBattleLoading S2C_NotifyBattleLoading_message)
		{
			session.Send(S2C_NotifyBattleLoading_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_NotifyBattleLoading(this Session session)
		{
			using var message = Fantasy.S2C_NotifyBattleLoading.Create();
			session.Send(message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<S2C_BattleLoadDoneResponse> C2S_BattleLoadDoneRequest(this Session session, C2S_BattleLoadDoneRequest C2S_BattleLoadDoneRequest_request)
		{
			return (S2C_BattleLoadDoneResponse)await session.Call(C2S_BattleLoadDoneRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<S2C_BattleLoadDoneResponse> C2S_BattleLoadDoneRequest(this Session session)
		{
			using var C2S_BattleLoadDoneRequest_request = Fantasy.C2S_BattleLoadDoneRequest.Create();
			return (S2C_BattleLoadDoneResponse)await session.Call(C2S_BattleLoadDoneRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_NotifyEnterBattle(this Session session, S2C_NotifyEnterBattle S2C_NotifyEnterBattle_message)
		{
			session.Send(S2C_NotifyEnterBattle_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_NotifyEnterBattle(this Session session, int randSeed, int playerCount, byte isHaveRoomInfo, CSRoomInfo roomInfoList, int battleStatus, byte isGuide, uint startTime, ulong battleGID, byte multiPlayerBattle, ulong captainPlayerId, List<CSLevelPlayerData> playerDataList, CSChapterInfo chapter, int stage, int mapID)
		{
			using var S2C_NotifyEnterBattle_message = Fantasy.S2C_NotifyEnterBattle.Create();
			S2C_NotifyEnterBattle_message.RandSeed = randSeed;
			S2C_NotifyEnterBattle_message.PlayerCount = playerCount;
			S2C_NotifyEnterBattle_message.IsHaveRoomInfo = isHaveRoomInfo;
			S2C_NotifyEnterBattle_message.RoomInfoList = roomInfoList;
			S2C_NotifyEnterBattle_message.BattleStatus = battleStatus;
			S2C_NotifyEnterBattle_message.IsGuide = isGuide;
			S2C_NotifyEnterBattle_message.StartTime = startTime;
			S2C_NotifyEnterBattle_message.BattleGID = battleGID;
			S2C_NotifyEnterBattle_message.MultiPlayerBattle = multiPlayerBattle;
			S2C_NotifyEnterBattle_message.CaptainPlayerId = captainPlayerId;
			S2C_NotifyEnterBattle_message.PlayerDataList = playerDataList;
			S2C_NotifyEnterBattle_message.Chapter = chapter;
			S2C_NotifyEnterBattle_message.Stage = stage;
			S2C_NotifyEnterBattle_message.MapID = mapID;
			session.Send(S2C_NotifyEnterBattle_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CSChapterInfo(this Session session, CSChapterInfo CSChapterInfo_message)
		{
			session.Send(CSChapterInfo_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CSChapterInfo(this Session session, int chapterID, int difficult)
		{
			using var CSChapterInfo_message = Fantasy.CSChapterInfo.Create();
			CSChapterInfo_message.ChapterID = chapterID;
			CSChapterInfo_message.Difficult = difficult;
			session.Send(CSChapterInfo_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_BroadcastFrameData(this Session session, S2C_BroadcastFrameData S2C_BroadcastFrameData_message)
		{
			session.Send(S2C_BroadcastFrameData_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void S2C_BroadcastFrameData(this Session session, CSRoomInfo roomInfo, int sveFrameId, int frameCount, List<CSSyncOneFrameData> frameDataList)
		{
			using var S2C_BroadcastFrameData_message = Fantasy.S2C_BroadcastFrameData.Create();
			S2C_BroadcastFrameData_message.RoomInfo = roomInfo;
			S2C_BroadcastFrameData_message.SveFrameId = sveFrameId;
			S2C_BroadcastFrameData_message.FrameCount = frameCount;
			S2C_BroadcastFrameData_message.FrameDataList = frameDataList;
			session.Send(S2C_BroadcastFrameData_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Chat2C_SendMessageResponse> C2Chat_SendMessageRequest(this Session session, C2Chat_SendMessageRequest C2Chat_SendMessageRequest_request)
		{
			return (Chat2C_SendMessageResponse)await session.Call(C2Chat_SendMessageRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Chat2C_SendMessageResponse> C2Chat_SendMessageRequest(this Session session, ChatInfoTree chatInfoTree)
		{
			using var C2Chat_SendMessageRequest_request = Fantasy.C2Chat_SendMessageRequest.Create();
			C2Chat_SendMessageRequest_request.ChatInfoTree = chatInfoTree;
			return (Chat2C_SendMessageResponse)await session.Call(C2Chat_SendMessageRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Chat2C_Message(this Session session, Chat2C_Message Chat2C_Message_message)
		{
			session.Send(Chat2C_Message_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Chat2C_Message(this Session session, ChatInfoTree chatInfoTree)
		{
			using var Chat2C_Message_message = Fantasy.Chat2C_Message.Create();
			Chat2C_Message_message.ChatInfoTree = chatInfoTree;
			session.Send(Chat2C_Message_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_AddFriendResponse> C2Game_AddFriendRequest(this Session session, C2Game_AddFriendRequest C2Game_AddFriendRequest_request)
		{
			return (Game2C_AddFriendResponse)await session.Call(C2Game_AddFriendRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_AddFriendResponse> C2Game_AddFriendRequest(this Session session, ulong targetRoleId, string targetRoleName)
		{
			using var C2Game_AddFriendRequest_request = Fantasy.C2Game_AddFriendRequest.Create();
			C2Game_AddFriendRequest_request.TargetRoleId = targetRoleId;
			C2Game_AddFriendRequest_request.TargetRoleName = targetRoleName;
			return (Game2C_AddFriendResponse)await session.Call(C2Game_AddFriendRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_DeleteFriendResponse> C2Game_DeleteFriendRequest(this Session session, C2Game_DeleteFriendRequest C2Game_DeleteFriendRequest_request)
		{
			return (Game2C_DeleteFriendResponse)await session.Call(C2Game_DeleteFriendRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_DeleteFriendResponse> C2Game_DeleteFriendRequest(this Session session, ulong targetRoleId, string targetRoleName)
		{
			using var C2Game_DeleteFriendRequest_request = Fantasy.C2Game_DeleteFriendRequest.Create();
			C2Game_DeleteFriendRequest_request.TargetRoleId = targetRoleId;
			C2Game_DeleteFriendRequest_request.TargetRoleName = targetRoleName;
			return (Game2C_DeleteFriendResponse)await session.Call(C2Game_DeleteFriendRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_BlockFriendResponse> C2Game_BlockFriendRequest(this Session session, C2Game_BlockFriendRequest C2Game_BlockFriendRequest_request)
		{
			return (Game2C_BlockFriendResponse)await session.Call(C2Game_BlockFriendRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_BlockFriendResponse> C2Game_BlockFriendRequest(this Session session, ulong targetRoleId, string targetRoleName)
		{
			using var C2Game_BlockFriendRequest_request = Fantasy.C2Game_BlockFriendRequest.Create();
			C2Game_BlockFriendRequest_request.TargetRoleId = targetRoleId;
			C2Game_BlockFriendRequest_request.TargetRoleName = targetRoleName;
			return (Game2C_BlockFriendResponse)await session.Call(C2Game_BlockFriendRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListFriendsResponse> C2Game_ListFriendsRequest(this Session session, C2Game_ListFriendsRequest C2Game_ListFriendsRequest_request)
		{
			return (Game2C_ListFriendsResponse)await session.Call(C2Game_ListFriendsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListFriendsResponse> C2Game_ListFriendsRequest(this Session session, int state, int limit, string cursor)
		{
			using var C2Game_ListFriendsRequest_request = Fantasy.C2Game_ListFriendsRequest.Create();
			C2Game_ListFriendsRequest_request.State = state;
			C2Game_ListFriendsRequest_request.Limit = limit;
			C2Game_ListFriendsRequest_request.Cursor = cursor;
			return (Game2C_ListFriendsResponse)await session.Call(C2Game_ListFriendsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListRecommendFriendsResponse> C2Game_ListRecommendFriendsRequest(this Session session, C2Game_ListRecommendFriendsRequest C2Game_ListRecommendFriendsRequest_request)
		{
			return (Game2C_ListRecommendFriendsResponse)await session.Call(C2Game_ListRecommendFriendsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListRecommendFriendsResponse> C2Game_ListRecommendFriendsRequest(this Session session, int limit)
		{
			using var C2Game_ListRecommendFriendsRequest_request = Fantasy.C2Game_ListRecommendFriendsRequest.Create();
			C2Game_ListRecommendFriendsRequest_request.Limit = limit;
			return (Game2C_ListRecommendFriendsResponse)await session.Call(C2Game_ListRecommendFriendsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_FriendChangedNotify(this Session session, G2C_FriendChangedNotify G2C_FriendChangedNotify_message)
		{
			session.Send(G2C_FriendChangedNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_FriendChangedNotify(this Session session, int op, CSFriendInfo friend)
		{
			using var G2C_FriendChangedNotify_message = Fantasy.G2C_FriendChangedNotify.Create();
			G2C_FriendChangedNotify_message.Op = op;
			G2C_FriendChangedNotify_message.Friend = friend;
			session.Send(G2C_FriendChangedNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_QueryFuncOpenListResponse> C2G_QueryFuncOpenListRequest(this Session session, C2G_QueryFuncOpenListRequest C2G_QueryFuncOpenListRequest_request)
		{
			return (G2C_QueryFuncOpenListResponse)await session.Call(C2G_QueryFuncOpenListRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_QueryFuncOpenListResponse> C2G_QueryFuncOpenListRequest(this Session session)
		{
			using var C2G_QueryFuncOpenListRequest_request = Fantasy.C2G_QueryFuncOpenListRequest.Create();
			return (G2C_QueryFuncOpenListResponse)await session.Call(C2G_QueryFuncOpenListRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_FuncOpenNotify(this Session session, G2C_FuncOpenNotify G2C_FuncOpenNotify_message)
		{
			session.Send(G2C_FuncOpenNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_FuncOpenNotify(this Session session, List<int> newOpenFuncList)
		{
			using var G2C_FuncOpenNotify_message = Fantasy.G2C_FuncOpenNotify.Create();
			G2C_FuncOpenNotify_message.NewOpenFuncList = newOpenFuncList;
			session.Send(G2C_FuncOpenNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_CreateGroupResponse> C2Game_CreateGroupRequest(this Session session, C2Game_CreateGroupRequest C2Game_CreateGroupRequest_request)
		{
			return (Game2C_CreateGroupResponse)await session.Call(C2Game_CreateGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_CreateGroupResponse> C2Game_CreateGroupRequest(this Session session, string name, string description, string avatarUrl, string langTag, bool open, int maxCount)
		{
			using var C2Game_CreateGroupRequest_request = Fantasy.C2Game_CreateGroupRequest.Create();
			C2Game_CreateGroupRequest_request.Name = name;
			C2Game_CreateGroupRequest_request.Description = description;
			C2Game_CreateGroupRequest_request.AvatarUrl = avatarUrl;
			C2Game_CreateGroupRequest_request.LangTag = langTag;
			C2Game_CreateGroupRequest_request.Open = open;
			C2Game_CreateGroupRequest_request.MaxCount = maxCount;
			return (Game2C_CreateGroupResponse)await session.Call(C2Game_CreateGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UpdateGroupResponse> C2Game_UpdateGroupRequest(this Session session, C2Game_UpdateGroupRequest C2Game_UpdateGroupRequest_request)
		{
			return (Game2C_UpdateGroupResponse)await session.Call(C2Game_UpdateGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UpdateGroupResponse> C2Game_UpdateGroupRequest(this Session session, ulong groupId, string name, string description, string avatarUrl, string langTag, bool hasOpen, bool open, int maxCount)
		{
			using var C2Game_UpdateGroupRequest_request = Fantasy.C2Game_UpdateGroupRequest.Create();
			C2Game_UpdateGroupRequest_request.GroupId = groupId;
			C2Game_UpdateGroupRequest_request.Name = name;
			C2Game_UpdateGroupRequest_request.Description = description;
			C2Game_UpdateGroupRequest_request.AvatarUrl = avatarUrl;
			C2Game_UpdateGroupRequest_request.LangTag = langTag;
			C2Game_UpdateGroupRequest_request.HasOpen = hasOpen;
			C2Game_UpdateGroupRequest_request.Open = open;
			C2Game_UpdateGroupRequest_request.MaxCount = maxCount;
			return (Game2C_UpdateGroupResponse)await session.Call(C2Game_UpdateGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UpdateGroupMetadataResponse> C2Game_UpdateGroupMetadataRequest(this Session session, C2Game_UpdateGroupMetadataRequest C2Game_UpdateGroupMetadataRequest_request)
		{
			return (Game2C_UpdateGroupMetadataResponse)await session.Call(C2Game_UpdateGroupMetadataRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UpdateGroupMetadataResponse> C2Game_UpdateGroupMetadataRequest(this Session session, ulong groupId, Dictionary<string, string> metadata)
		{
			using var C2Game_UpdateGroupMetadataRequest_request = Fantasy.C2Game_UpdateGroupMetadataRequest.Create();
			C2Game_UpdateGroupMetadataRequest_request.GroupId = groupId;
			C2Game_UpdateGroupMetadataRequest_request.Metadata = metadata;
			return (Game2C_UpdateGroupMetadataResponse)await session.Call(C2Game_UpdateGroupMetadataRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_DeleteGroupResponse> C2Game_DeleteGroupRequest(this Session session, C2Game_DeleteGroupRequest C2Game_DeleteGroupRequest_request)
		{
			return (Game2C_DeleteGroupResponse)await session.Call(C2Game_DeleteGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_DeleteGroupResponse> C2Game_DeleteGroupRequest(this Session session, ulong groupId)
		{
			using var C2Game_DeleteGroupRequest_request = Fantasy.C2Game_DeleteGroupRequest.Create();
			C2Game_DeleteGroupRequest_request.GroupId = groupId;
			return (Game2C_DeleteGroupResponse)await session.Call(C2Game_DeleteGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListGroupsResponse> C2Game_ListGroupsRequest(this Session session, C2Game_ListGroupsRequest C2Game_ListGroupsRequest_request)
		{
			return (Game2C_ListGroupsResponse)await session.Call(C2Game_ListGroupsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListGroupsResponse> C2Game_ListGroupsRequest(this Session session, string nameFilter, int limit, string cursor)
		{
			using var C2Game_ListGroupsRequest_request = Fantasy.C2Game_ListGroupsRequest.Create();
			C2Game_ListGroupsRequest_request.NameFilter = nameFilter;
			C2Game_ListGroupsRequest_request.Limit = limit;
			C2Game_ListGroupsRequest_request.Cursor = cursor;
			return (Game2C_ListGroupsResponse)await session.Call(C2Game_ListGroupsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListUserGroupsResponse> C2Game_ListUserGroupsRequest(this Session session, C2Game_ListUserGroupsRequest C2Game_ListUserGroupsRequest_request)
		{
			return (Game2C_ListUserGroupsResponse)await session.Call(C2Game_ListUserGroupsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListUserGroupsResponse> C2Game_ListUserGroupsRequest(this Session session)
		{
			using var C2Game_ListUserGroupsRequest_request = Fantasy.C2Game_ListUserGroupsRequest.Create();
			return (Game2C_ListUserGroupsResponse)await session.Call(C2Game_ListUserGroupsRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_JoinGroupResponse> C2Game_JoinGroupRequest(this Session session, C2Game_JoinGroupRequest C2Game_JoinGroupRequest_request)
		{
			return (Game2C_JoinGroupResponse)await session.Call(C2Game_JoinGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_JoinGroupResponse> C2Game_JoinGroupRequest(this Session session, ulong groupId)
		{
			using var C2Game_JoinGroupRequest_request = Fantasy.C2Game_JoinGroupRequest.Create();
			C2Game_JoinGroupRequest_request.GroupId = groupId;
			return (Game2C_JoinGroupResponse)await session.Call(C2Game_JoinGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_LeaveGroupResponse> C2Game_LeaveGroupRequest(this Session session, C2Game_LeaveGroupRequest C2Game_LeaveGroupRequest_request)
		{
			return (Game2C_LeaveGroupResponse)await session.Call(C2Game_LeaveGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_LeaveGroupResponse> C2Game_LeaveGroupRequest(this Session session, ulong groupId)
		{
			using var C2Game_LeaveGroupRequest_request = Fantasy.C2Game_LeaveGroupRequest.Create();
			C2Game_LeaveGroupRequest_request.GroupId = groupId;
			return (Game2C_LeaveGroupResponse)await session.Call(C2Game_LeaveGroupRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListGroupUsersResponse> C2Game_ListGroupUsersRequest(this Session session, C2Game_ListGroupUsersRequest C2Game_ListGroupUsersRequest_request)
		{
			return (Game2C_ListGroupUsersResponse)await session.Call(C2Game_ListGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_ListGroupUsersResponse> C2Game_ListGroupUsersRequest(this Session session, ulong groupId, int state, int limit, string cursor)
		{
			using var C2Game_ListGroupUsersRequest_request = Fantasy.C2Game_ListGroupUsersRequest.Create();
			C2Game_ListGroupUsersRequest_request.GroupId = groupId;
			C2Game_ListGroupUsersRequest_request.State = state;
			C2Game_ListGroupUsersRequest_request.Limit = limit;
			C2Game_ListGroupUsersRequest_request.Cursor = cursor;
			return (Game2C_ListGroupUsersResponse)await session.Call(C2Game_ListGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_AddGroupUsersResponse> C2Game_AddGroupUsersRequest(this Session session, C2Game_AddGroupUsersRequest C2Game_AddGroupUsersRequest_request)
		{
			return (Game2C_AddGroupUsersResponse)await session.Call(C2Game_AddGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_AddGroupUsersResponse> C2Game_AddGroupUsersRequest(this Session session, ulong groupId, List<ulong> roleIds)
		{
			using var C2Game_AddGroupUsersRequest_request = Fantasy.C2Game_AddGroupUsersRequest.Create();
			C2Game_AddGroupUsersRequest_request.GroupId = groupId;
			C2Game_AddGroupUsersRequest_request.RoleIds = roleIds;
			return (Game2C_AddGroupUsersResponse)await session.Call(C2Game_AddGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_PromoteGroupUsersResponse> C2Game_PromoteGroupUsersRequest(this Session session, C2Game_PromoteGroupUsersRequest C2Game_PromoteGroupUsersRequest_request)
		{
			return (Game2C_PromoteGroupUsersResponse)await session.Call(C2Game_PromoteGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_PromoteGroupUsersResponse> C2Game_PromoteGroupUsersRequest(this Session session, ulong groupId, List<ulong> roleIds)
		{
			using var C2Game_PromoteGroupUsersRequest_request = Fantasy.C2Game_PromoteGroupUsersRequest.Create();
			C2Game_PromoteGroupUsersRequest_request.GroupId = groupId;
			C2Game_PromoteGroupUsersRequest_request.RoleIds = roleIds;
			return (Game2C_PromoteGroupUsersResponse)await session.Call(C2Game_PromoteGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_DemoteGroupUsersResponse> C2Game_DemoteGroupUsersRequest(this Session session, C2Game_DemoteGroupUsersRequest C2Game_DemoteGroupUsersRequest_request)
		{
			return (Game2C_DemoteGroupUsersResponse)await session.Call(C2Game_DemoteGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_DemoteGroupUsersResponse> C2Game_DemoteGroupUsersRequest(this Session session, ulong groupId, List<ulong> roleIds)
		{
			using var C2Game_DemoteGroupUsersRequest_request = Fantasy.C2Game_DemoteGroupUsersRequest.Create();
			C2Game_DemoteGroupUsersRequest_request.GroupId = groupId;
			C2Game_DemoteGroupUsersRequest_request.RoleIds = roleIds;
			return (Game2C_DemoteGroupUsersResponse)await session.Call(C2Game_DemoteGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_KickGroupUsersResponse> C2Game_KickGroupUsersRequest(this Session session, C2Game_KickGroupUsersRequest C2Game_KickGroupUsersRequest_request)
		{
			return (Game2C_KickGroupUsersResponse)await session.Call(C2Game_KickGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_KickGroupUsersResponse> C2Game_KickGroupUsersRequest(this Session session, ulong groupId, List<ulong> roleIds)
		{
			using var C2Game_KickGroupUsersRequest_request = Fantasy.C2Game_KickGroupUsersRequest.Create();
			C2Game_KickGroupUsersRequest_request.GroupId = groupId;
			C2Game_KickGroupUsersRequest_request.RoleIds = roleIds;
			return (Game2C_KickGroupUsersResponse)await session.Call(C2Game_KickGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_BanGroupUsersResponse> C2Game_BanGroupUsersRequest(this Session session, C2Game_BanGroupUsersRequest C2Game_BanGroupUsersRequest_request)
		{
			return (Game2C_BanGroupUsersResponse)await session.Call(C2Game_BanGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_BanGroupUsersResponse> C2Game_BanGroupUsersRequest(this Session session, ulong groupId, List<ulong> roleIds)
		{
			using var C2Game_BanGroupUsersRequest_request = Fantasy.C2Game_BanGroupUsersRequest.Create();
			C2Game_BanGroupUsersRequest_request.GroupId = groupId;
			C2Game_BanGroupUsersRequest_request.RoleIds = roleIds;
			return (Game2C_BanGroupUsersResponse)await session.Call(C2Game_BanGroupUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_GroupChangedNotify(this Session session, G2C_GroupChangedNotify G2C_GroupChangedNotify_message)
		{
			session.Send(G2C_GroupChangedNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_GroupChangedNotify(this Session session, int op, ulong groupId, CSGroupInfo group, CSGroupUser user)
		{
			using var G2C_GroupChangedNotify_message = Fantasy.G2C_GroupChangedNotify.Create();
			G2C_GroupChangedNotify_message.Op = op;
			G2C_GroupChangedNotify_message.GroupId = groupId;
			G2C_GroupChangedNotify_message.Group = group;
			G2C_GroupChangedNotify_message.User = user;
			session.Send(G2C_GroupChangedNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<A2C_RegisterResponse> C2A_RegisterRequest(this Session session, C2A_RegisterRequest C2A_RegisterRequest_request)
		{
			return (A2C_RegisterResponse)await session.Call(C2A_RegisterRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<A2C_RegisterResponse> C2A_RegisterRequest(this Session session, string userName, string password)
		{
			using var C2A_RegisterRequest_request = Fantasy.C2A_RegisterRequest.Create();
			C2A_RegisterRequest_request.UserName = userName;
			C2A_RegisterRequest_request.Password = password;
			return (A2C_RegisterResponse)await session.Call(C2A_RegisterRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<A2C_LoginResponse> C2A_LoginRequest(this Session session, C2A_LoginRequest C2A_LoginRequest_request)
		{
			return (A2C_LoginResponse)await session.Call(C2A_LoginRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<A2C_LoginResponse> C2A_LoginRequest(this Session session, string userName, string password, uint loginType)
		{
			using var C2A_LoginRequest_request = Fantasy.C2A_LoginRequest.Create();
			C2A_LoginRequest_request.UserName = userName;
			C2A_LoginRequest_request.Password = password;
			C2A_LoginRequest_request.LoginType = loginType;
			return (A2C_LoginResponse)await session.Call(C2A_LoginRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2A_RecordRecentServer(this Session session, C2A_RecordRecentServer C2A_RecordRecentServer_message)
		{
			session.Send(C2A_RecordRecentServer_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void C2A_RecordRecentServer(this Session session, long roleID, int serverID)
		{
			using var C2A_RecordRecentServer_message = Fantasy.C2A_RecordRecentServer.Create();
			C2A_RecordRecentServer_message.RoleID = roleID;
			C2A_RecordRecentServer_message.ServerID = serverID;
			session.Send(C2A_RecordRecentServer_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LoginResponse> C2G_LoginRequest(this Session session, C2G_LoginRequest C2G_LoginRequest_request)
		{
			return (G2C_LoginResponse)await session.Call(C2G_LoginRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LoginResponse> C2G_LoginRequest(this Session session, string token, int serverID)
		{
			using var C2G_LoginRequest_request = Fantasy.C2G_LoginRequest.Create();
			C2G_LoginRequest_request.Token = token;
			C2G_LoginRequest_request.ServerID = serverID;
			return (G2C_LoginResponse)await session.Call(C2G_LoginRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_RepeatLogin(this Session session, G2C_RepeatLogin G2C_RepeatLogin_message)
		{
			session.Send(G2C_RepeatLogin_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_RepeatLogin(this Session session)
		{
			using var message = Fantasy.G2C_RepeatLogin.Create();
			session.Send(message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_GetPlayerDataResponse> C2Game_GetPlayerDataRequest(this Session session, C2Game_GetPlayerDataRequest C2Game_GetPlayerDataRequest_request)
		{
			return (Game2C_GetPlayerDataResponse)await session.Call(C2Game_GetPlayerDataRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_GetPlayerDataResponse> C2Game_GetPlayerDataRequest(this Session session)
		{
			using var C2Game_GetPlayerDataRequest_request = Fantasy.C2Game_GetPlayerDataRequest.Create();
			return (Game2C_GetPlayerDataResponse)await session.Call(C2Game_GetPlayerDataRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_FollowUsersResponse> C2Game_FollowUsersRequest(this Session session, C2Game_FollowUsersRequest C2Game_FollowUsersRequest_request)
		{
			return (Game2C_FollowUsersResponse)await session.Call(C2Game_FollowUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_FollowUsersResponse> C2Game_FollowUsersRequest(this Session session, List<ulong> roleIds)
		{
			using var C2Game_FollowUsersRequest_request = Fantasy.C2Game_FollowUsersRequest.Create();
			C2Game_FollowUsersRequest_request.RoleIds = roleIds;
			return (Game2C_FollowUsersResponse)await session.Call(C2Game_FollowUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UnfollowUsersResponse> C2Game_UnfollowUsersRequest(this Session session, C2Game_UnfollowUsersRequest C2Game_UnfollowUsersRequest_request)
		{
			return (Game2C_UnfollowUsersResponse)await session.Call(C2Game_UnfollowUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UnfollowUsersResponse> C2Game_UnfollowUsersRequest(this Session session, List<ulong> roleIds)
		{
			using var C2Game_UnfollowUsersRequest_request = Fantasy.C2Game_UnfollowUsersRequest.Create();
			C2Game_UnfollowUsersRequest_request.RoleIds = roleIds;
			return (Game2C_UnfollowUsersResponse)await session.Call(C2Game_UnfollowUsersRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UpdateStatusResponse> C2Game_UpdateStatusRequest(this Session session, C2Game_UpdateStatusRequest C2Game_UpdateStatusRequest_request)
		{
			return (Game2C_UpdateStatusResponse)await session.Call(C2Game_UpdateStatusRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<Game2C_UpdateStatusResponse> C2Game_UpdateStatusRequest(this Session session, string statusText)
		{
			using var C2Game_UpdateStatusRequest_request = Fantasy.C2Game_UpdateStatusRequest.Create();
			C2Game_UpdateStatusRequest_request.StatusText = statusText;
			return (Game2C_UpdateStatusResponse)await session.Call(C2Game_UpdateStatusRequest_request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_StatusPresenceNotify(this Session session, G2C_StatusPresenceNotify G2C_StatusPresenceNotify_message)
		{
			session.Send(G2C_StatusPresenceNotify_message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_StatusPresenceNotify(this Session session, List<CSStatusPresence> joins, List<CSStatusPresence> leaves)
		{
			using var G2C_StatusPresenceNotify_message = Fantasy.G2C_StatusPresenceNotify.Create();
			G2C_StatusPresenceNotify_message.Joins = joins;
			G2C_StatusPresenceNotify_message.Leaves = leaves;
			session.Send(G2C_StatusPresenceNotify_message);
		}

   }
}