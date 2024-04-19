namespace ProperNutritionDiary.User.Api.User.Tokens;

public class TokenDto(string jwt, string rt)
{
    public string Jwt { get; set; } = jwt;
    public string Rt { get; set; } = rt;
}
