namespace Fantasy;

/// <summary>
/// 服务器错误码常量表
/// </summary>
public static class ErrorCode
{
    public const uint SUCCESS = 0; // 响应处理成功
    public const uint REGISTER_ACCOUNT_EXISTS = 1001; // 账号已存在
    public const uint REGISTER_UNKNOW_EORROR = 1002; // 未知错误
    public const uint REGISTER_INVALID_PARAMETER = 1003; // 参数无效，账号或密码为空
    public const uint LOGIN_INCORRECT_PASSWORD = 1004; // 密码错误，请重试
    public const uint LOGIN_ACCOUNT_NOT_EXIST = 1005; // 账号不存在
    public const uint LOGIN_ACCOUNT_EXISTS_PASSWORD_ERROR = 1006; // 账号已存在但密码错误
    public const uint LOGIN_UNKNOW_EORROR = 1007; // 未知错误
    public const uint LOGIN_INVALID_PARAMETER = 1008; // 参数无效，账号或密码为空
    public const uint LOGIN_ACCOUNT_ALREADY_ONLINE = 1009; // 账号已在线
    public const uint LOGIN_REGISTER_ACCOUNT_OR_PASSWORD_NOT_EMPTY = 1010; // 账号或密码不能为空
    public const uint LOGIN_REGISTER_INCORRECT_FORMAT = 1011; // 账号格式不正确，需6-24位字母和数字
    public const uint LOGIN_REGISTER_PASSWORD_LESS_LIMIT = 1012; // 密码长度至少需8位
    public const uint REGISTER_TOW_PASSWORD_INCONSISTENT = 1013; // 两次输入的密码不一致
    public const uint REGISTER_SUCCESS = 1014; // 注册成功
    public const uint RLOGIN_TOKEN_ERROR = 1015; // 登录Token验证失败

    public const uint ROLE_CREATE_SUCCESS = 2001; // 创建角色成功
    public const uint ROLE_CONFIG_NOT_FOUND = 2002; // 创角色配置不存在
    public const uint ROLE_NAME_DUPLICATE = 2003; // 角色名已存在
    public const uint ROLE_NAME_INVALID = 2004; // 角色名无效（为空或包含非法字符）
    public const uint ROLE_SEX_INVALID = 2005; // 性别参数无效
    public const uint ROLE_NOT_FOUND = 2006; // 角色不存在
    public const uint ROLE_NOT_BELONG_TO_ACCOUNT = 2007; // 角色不属于该账号
    public const uint ROLE_NAME_LENGTH_OUT_RANGE = 2008; // 角色名长度不符，请使用中文2-6字，英文4-12字符
    public const uint ROLE_NAME_CHAR_INVALID = 2009; // 角色名字含有非法字符
    
    public const uint FUNC_QUERY_ERROR = 3001; // 系统开放查询错误

    public const uint ROOM_INVALID_PARAMETER = 4001; // 房间参数无效
    public const uint ROOM_NOT_FOUND = 4002; // 房间不存在
    public const uint ROOM_ALREADY_JOINED = 4003; // 玩家已在房间中
    public const uint ROOM_PLAYER_COUNT_INVALID = 4004; // 房间玩家数量配置无效
    public const uint ROOM_CREATE_FAILED = 4005; // 房间创建失败

    public const uint FRIEND_INVALID_PARAMETER = 5001; // 好友参数无效
    public const uint FRIEND_TARGET_NOT_FOUND = 5002; // 目标角色不存在
    public const uint FRIEND_CANNOT_SELF = 5003; // 不能添加自己为好友
    public const uint FRIEND_ALREADY_FRIEND = 5004; // 已是好友
    public const uint FRIEND_ALREADY_PENDING = 5005; // 已发送或已收到申请
    public const uint FRIEND_BLOCKED = 5006; // 已被屏蔽或已屏蔽对方
    public const uint FRIEND_LIMIT = 5007; // 好友或申请数量已达上限
    public const uint FRIEND_NOT_FOUND = 5008; // 好友关系不存在
    public const uint FRIEND_INTERNAL_ERROR = 5009; // 好友系统内部错误

    public const uint PRESENCE_INVALID_PARAMETER = 5101; // 状态显示参数无效
    public const uint PRESENCE_INTERNAL_ERROR = 5102; // 状态显示内部错误

    public const uint GROUP_INVALID_PARAMETER = 5201; // 群组参数无效
    public const uint GROUP_NOT_FOUND = 5202; // 群组不存在
    public const uint GROUP_NO_PERMISSION = 5203; // 无权限
    public const uint GROUP_FULL = 5204; // 群组已满
    public const uint GROUP_ALREADY_MEMBER = 5205; // 已是成员
    public const uint GROUP_ALREADY_PENDING = 5206; // 已提交申请
    public const uint GROUP_BANNED = 5207; // 已被封禁
    public const uint GROUP_LIMIT = 5208; // 加入群数量或申请数已达上限
    public const uint GROUP_SOLE_SUPERADMIN = 5209; // 唯一超管不可退出
    public const uint GROUP_NOT_MEMBER = 5210; // 不是群成员
    public const uint GROUP_INTERNAL_ERROR = 5211; // 群组系统内部错误
}
