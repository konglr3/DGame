using System.Collections.Generic;
using System.Text;
using DGame;
using Fantasy;
using Fantasy.Async;
using GameProto;
using GOS.Presence;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 好友窗口：运行时搭建简易三 Tab UI（列表 / 申请 / 推荐）。
    /// </summary>
    public sealed class FriendUI : UIWindow
    {
        private enum FriendTab
        {
            List = 0,
            Request = 1,
            Recommend = 2
        }

        private InputField m_inputTarget;
        private InputField m_inputStatus;
        private Text m_textContent;
        private FriendTab m_curTab = FriendTab.List;
        private PresenceComponent m_presence;

        protected override ModelType GetModelType() => ModelType.NormalHaveClose;

        public override bool FullScreen => false;

        protected override void ScriptGenerator()
        {
            BuildRuntimeUi();
        }

        protected override void RegisterEvent()
        {
            AddUIEvent(IFriendLogicEvent_Event.OnFriendListChange, RefreshContent);
        }

        protected override void OnCreate()
        {
            m_presence = GameClient.Instance.Scene?.EnsurePresence();
            if (m_presence != null)
            {
                m_presence.OnPresenceChanged += RefreshContent;
            }

            FriendNetMgr.Instance.RefreshAll().Coroutine();
            RefreshContent();
        }

        protected override void OnDestroy()
        {
            if (m_presence != null)
            {
                m_presence.OnPresenceChanged -= RefreshContent;
                m_presence = null;
            }
        }

        private void BuildRuntimeUi()
        {
            var root = gameObject.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            var panel = CreatePanel(root, "Panel", new Color(0f, 0f, 0f, 0.85f));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.15f, 0.1f);
            panelRect.anchorMax = new Vector2(0.85f, 0.9f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CreateButton(panel.transform, "m_btnClose", "关闭", new Vector2(0.85f, 0.92f), new Vector2(0.98f, 0.98f), () => Close());
            CreateButton(panel.transform, "m_btnTabList", "好友", new Vector2(0.02f, 0.92f), new Vector2(0.22f, 0.98f), () => SwitchTab(FriendTab.List));
            CreateButton(panel.transform, "m_btnTabRequest", "申请", new Vector2(0.24f, 0.92f), new Vector2(0.44f, 0.98f), () => SwitchTab(FriendTab.Request));
            CreateButton(panel.transform, "m_btnTabRecommend", "推荐", new Vector2(0.46f, 0.92f), new Vector2(0.66f, 0.98f), () => SwitchTab(FriendTab.Recommend));

            m_inputTarget = CreateInput(panel.transform, "m_inputTarget", "角色名或RoleId", new Vector2(0.02f, 0.84f), new Vector2(0.55f, 0.90f));
            CreateButton(panel.transform, "m_btnAdd", "添加", new Vector2(0.57f, 0.84f), new Vector2(0.68f, 0.90f), OnClickAdd);
            CreateButton(panel.transform, "m_btnDelete", "删除", new Vector2(0.70f, 0.84f), new Vector2(0.81f, 0.90f), OnClickDelete);
            CreateButton(panel.transform, "m_btnBlock", "屏蔽", new Vector2(0.83f, 0.84f), new Vector2(0.94f, 0.90f), OnClickBlock);
            CreateButton(panel.transform, "m_btnRefresh", "刷新", new Vector2(0.02f, 0.68f), new Vector2(0.16f, 0.74f), () => FriendNetMgr.Instance.RefreshAll().Coroutine());

            m_inputStatus = CreateInput(panel.transform, "m_inputStatus", "我的状态", new Vector2(0.18f, 0.68f), new Vector2(0.72f, 0.74f));
            CreateButton(panel.transform, "m_btnStatus", "更新状态", new Vector2(0.74f, 0.68f), new Vector2(0.98f, 0.74f), OnClickUpdateStatus);

            m_textContent = CreateText(panel.transform, "m_textContent", string.Empty, new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.66f));
            m_textContent.alignment = TextAnchor.UpperLeft;
            m_textContent.horizontalOverflow = HorizontalWrapMode.Wrap;
            m_textContent.verticalOverflow = VerticalWrapMode.Overflow;
        }

        private void SwitchTab(FriendTab tab)
        {
            m_curTab = tab;
            RefreshContent();
        }

        private void RefreshContent()
        {
            if (m_textContent == null)
            {
                return;
            }

            var sb = new StringBuilder();
            switch (m_curTab)
            {
                case FriendTab.List:
                    AppendFriendLines(sb, FriendDataMgr.Instance.MutualFriends, true);
                    break;
                case FriendTab.Request:
                    sb.AppendLine("[收到的申请] 点击条目前数字对应角色，可用按钮同意/拒绝");
                    AppendFriendLines(sb, FriendDataMgr.Instance.IncomingRequests, true);
                    sb.AppendLine();
                    sb.AppendLine("[发出的申请]");
                    AppendFriendLines(sb, FriendDataMgr.Instance.OutgoingRequests, false);
                    break;
                case FriendTab.Recommend:
                    AppendFriendLines(sb, FriendDataMgr.Instance.RecommendFriends, false);
                    break;
            }

            m_textContent.text = sb.ToString();
        }

        private void AppendFriendLines(StringBuilder sb, List<CSFriendInfo> friends, bool showOnline)
        {
            if (friends == null || friends.Count == 0)
            {
                sb.AppendLine("（空）");
                return;
            }

            var presence = GameClient.Instance.Scene?.GetComponent<PresenceComponent>();
            for (var i = 0; i < friends.Count; i++)
            {
                var friend = friends[i];
                if (friend == null)
                {
                    continue;
                }

                var onlineText = string.Empty;
                if (showOnline && presence != null && presence.TryGet(friend.RoleId, out var cache))
                {
                    onlineText = cache.Online ? $" [在线] {cache.StatusText}" : " [离线]";
                }

                sb.AppendLine($"{i + 1}. {friend.RoleName} (Lv.{friend.Level}) Id:{friend.RoleId}{onlineText}");
            }

            if (m_curTab == FriendTab.Request && FriendDataMgr.Instance.IncomingRequests.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("操作：在输入框填 RoleId，再点添加=同意；删除=拒绝");
            }
            else if (m_curTab == FriendTab.List)
            {
                sb.AppendLine();
                sb.AppendLine("操作：输入 RoleId 后，添加=加好友；刷新下方还可在推荐页添加");
            }
        }

        private void OnClickAdd()
        {
            HandleAdd().Coroutine();
        }

        private void OnClickDelete()
        {
            HandleDelete().Coroutine();
        }

        private void OnClickBlock()
        {
            HandleBlock().Coroutine();
        }

        private async FTask HandleAdd()
        {
            var text = m_inputTarget != null ? m_inputTarget.text.Trim() : string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                GameModule.UIModule.ShowTipsUI(G.R("请输入角色名或RoleId"));
                return;
            }

            if (ulong.TryParse(text, out var roleId))
            {
                await FriendNetMgr.Instance.AddFriend(roleId);
            }
            else
            {
                await FriendNetMgr.Instance.AddFriend(0, text);
            }

            await FriendNetMgr.Instance.RefreshAll();
        }

        private async FTask HandleDelete()
        {
            if (!TryParseRoleId(out var roleId))
            {
                return;
            }

            await FriendNetMgr.Instance.DeleteFriend(roleId);
            await FriendNetMgr.Instance.RefreshAll();
        }

        private async FTask HandleBlock()
        {
            if (!TryParseRoleId(out var roleId))
            {
                return;
            }

            await FriendNetMgr.Instance.BlockFriend(roleId);
            await FriendNetMgr.Instance.RefreshAll();
        }

        private bool TryParseRoleId(out ulong roleId)
        {
            roleId = 0;
            var text = m_inputTarget != null ? m_inputTarget.text.Trim() : string.Empty;
            if (!ulong.TryParse(text, out roleId) || roleId == 0)
            {
                GameModule.UIModule.ShowTipsUI(G.R("请输入有效 RoleId"));
                return false;
            }

            return true;
        }

        private void OnClickUpdateStatus()
        {
            var presence = GameClient.Instance.Scene?.EnsurePresence();
            presence?.UpdateStatus(m_inputStatus != null ? m_inputStatus.text : string.Empty).Coroutine();
            GameModule.UIModule.ShowTipsUI(G.R("状态已更新"));
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            return go;
        }

        private static Text CreateText(Transform parent, string name, string content, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = go.GetComponent<Text>();
            text.text = content;
            text.color = Color.white;
            text.fontSize = 22;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return text;
        }

        private static InputField CreateInput(Transform parent, string name, string placeholder, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8, 4);
            textRect.offsetMax = new Vector2(-8, -4);
            var text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.supportRichText = false;

            var placeholderGo = new GameObject("Placeholder", typeof(RectTransform), typeof(Text));
            placeholderGo.transform.SetParent(go.transform, false);
            var placeholderRect = placeholderGo.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(8, 4);
            placeholderRect.offsetMax = new Vector2(-8, -4);
            var placeholderText = placeholderGo.GetComponent<Text>();
            placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            placeholderText.fontSize = 20;
            placeholderText.color = new Color(1f, 1f, 1f, 0.4f);
            placeholderText.text = placeholder;

            var input = go.GetComponent<InputField>();
            input.textComponent = text;
            input.placeholder = placeholderText;
            return input;
        }

        private static void CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = new Color(0.25f, 0.45f, 0.75f, 1f);
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(onClick);

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var text = textGo.GetComponent<Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 20;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
