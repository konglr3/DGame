using Fantasy;

namespace Hotfix;

/// <summary>
/// PlayerData 与协议层 CSPlayerData 的转换。
/// </summary>
public static class PlayerDataHelper
{
    /// <summary>
    /// PlayerData 转换成 CSPlayerData。
    /// </summary>
    public static CSPlayerData ToCSPlayerData(this PlayerData self)
        => new()
        {
            RoleName = self.RoleName,
            HeadID = self.HeadID,
            RoleID = (ulong)self.Id,
            Sex = self.Sex,
            Level = self.Level,
            Exp = self.Exp,
            FightValue = self.FightValue,
            Diamond = self.Diamond,
            Gold = self.Gold,
            Stam = self.Stam,
            IsFinGuide = self.IsFinGuide,
            Sign = self.Sign,
            WorldID = self.WorldID,
            TotalRmb = self.TotalRmb,
            LastAddStamTime = self.LastAddStamTime,
            DailyBuyStamCount = self.DailyBuyStamCount,
            CreateTime = self.CreateTime,
            LastLoginTime = self.LastLoginTime,
        };
}
