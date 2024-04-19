namespace ProperNutritionDiary.User.Api.User.Refresh;

public class UserRefresh(Guid userId, string deviceHash, DateTime dateAdded, string ip, string? rT)
{
    public Guid UserId { get; set; } = userId;
    public string Ip { get; set; } = ip;
    public string DeviceHash { get; set; } = deviceHash;
    public string? RT { get; set; } = rT;
    public DateTime DateAdded { get; set; } = dateAdded;
}
