namespace Di;
/// <summary>
/// 账号
/// </summary>
public class Account
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// 密码盐
    /// </summary>
    public string PasswordSalt { get; set; } = default!;

    /// <summary>
    /// 创建日期
    /// </summary>
    public DateTime CreateTime { get; set; } = default!;

    /// <summary>
    /// 更新日期
    /// </summary>
    public DateTime UpdateTime { get; set; } = default!;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = default!;
}
