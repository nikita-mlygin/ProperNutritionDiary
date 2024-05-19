using System.Security.Claims;

namespace ProperNutritionDiary.BuildingBlocks.PresentationPackages;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal u)
    {
        var idStr = u.FindFirstValue(ClaimTypes.NameIdentifier);

        return string.IsNullOrEmpty(idStr) ? null : TryGetGuid(idStr);
    }

    public static Guid? TryGetGuid(string idStr)
    {
        return Guid.TryParse(idStr, out var res) ? res : null;
    }
}
