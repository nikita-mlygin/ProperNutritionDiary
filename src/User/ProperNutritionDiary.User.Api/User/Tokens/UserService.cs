using System.Security.Cryptography;
using System.Text;
using DomainDesignLib.Abstractions.Result;
using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.User.Api.Database;
using ProperNutritionDiary.User.Api.User.Refresh;
using ProperNutritionDiary.User.Api.User.Role;

namespace ProperNutritionDiary.User.Api.User.Tokens;

public class UserService(AppCtx ctx, TokenGenerator tg, ILogger<UserService> logger)
{
    private readonly TokenGenerator tg = tg;
    private readonly ILogger<UserService> logger = logger;

    private readonly AppCtx ctx = ctx;

    public async Task<(string jwt, string rt)?> GenerateGuestToken(HttpContext httpCtx)
    {
        var login = $"guest_{GetIpAddress(httpCtx)}_{GetDeviceHash(httpCtx)}";

        var user = await ctx.Users.Where(x => x.Login == login).FirstOrDefaultAsync();

        if (user is null)
        {
            user = new User(Guid.NewGuid(), login, "", "guest");

            await ctx.Users.AddAsync(user);
        }

        var res = tg.GenerateTokens(user.Id, user.Login, user.Role);

        await ctx.Refreshes.AddAsync(
            new UserRefresh(
                user.Id,
                GetDeviceHash(httpCtx),
                DateTime.UtcNow,
                GetIpAddress(httpCtx),
                res.rt
            )
        );

        await ctx.SaveChangesAsync();

        return res;
    }

    public async Task<(string jwt, string rt)?> RefreshToken(
        string expiredToken,
        string rt,
        HttpContext httpCtx
    )
    {
        var expiredTokenData = tg.ParseExpiredLogin(expiredToken);

        if (expiredTokenData is null)
            return null;

        var oldRt = await ctx
            .Refreshes.Where(x =>
                x.UserId == expiredTokenData.Value.id
                && x.Ip == GetIpAddress(httpCtx)
                && x.DeviceHash == GetDeviceHash(httpCtx)
            )
            .FirstOrDefaultAsync();

        if (oldRt == null)
            return null;

        logger.LogInformation("Old rt received {OldRt}, now rt is {Rt}", oldRt.RT, rt);

        if (oldRt.RT != rt)
        {
            ctx.Remove(oldRt);
            return null;
        }

        var nTokens = tg.GenerateTokens(
            expiredTokenData.Value.id,
            expiredTokenData.Value.login,
            expiredTokenData.Value.role
        );

        oldRt.RT = nTokens.rt;

        await ctx.SaveChangesAsync();

        httpCtx.Response.Cookies.Append(
            "rt",
            nTokens.rt,
            new CookieOptions()
            {
                HttpOnly = true,
                Secure = true, // Ensure this matches your deployment (use false for local development if necessary)
                SameSite = SameSiteMode.None
            }
        );

        return nTokens;
    }

    public async Task<Result<(string jwt, string rt)>> FirstGeneration(
        string login,
        string password,
        HttpContext httpCtx
    )
    {
        User? user = await ctx.Users.FirstOrDefaultAsync(x =>
            x.Login == login && x.PasswordHash == GenerateHash(password)
        );

        (string jwt, string rt)? tokens = null!;

        async Task updateCtx(User user, string rt)
        {
            var oldRt = await ctx.Refreshes.Where(x => x.UserId == user!.Id).FirstOrDefaultAsync();

            if (oldRt is not null)
            {
                oldRt.RT = rt;
                return;
            }

            await ctx.AddAsync(
                new UserRefresh(
                    user!.Id,
                    GetDeviceHash(httpCtx),
                    DateTime.UtcNow,
                    GetIpAddress(httpCtx),
                    rt
                )
            );
        }

        return await Result
            .Check(user is null, new Error("AuthError", "Login or password is incorrect"))
            .Success(() =>
            {
                tokens = tg.GenerateTokens(user!.Id, user.Login, user.Role);
                return tokens.Value;
            })
            .After(async () =>
            {
                await updateCtx(user!, tokens.Value.rt);

                await ctx.SaveChangesAsync();
            })
            .After(() =>
            {
                httpCtx.Response.Cookies.Append(
                    "rt",
                    tokens.Value.rt,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = true, // Ensure this matches your deployment (use false for local development if necessary)
                        SameSite = SameSiteMode.None
                    }
                );
            })
            .Build();
    }

    public async Task<Result> Registration(string login, string password)
    {
        var user = new User(Guid.NewGuid(), login, GenerateHash(password), Roles.Plain);

        await ctx.AddAsync(user);
        await ctx.SaveChangesAsync();

        return Result.Success();
    }

    private static string GetDeviceHash(HttpContext httpCtx)
    {
        return GenerateHash(
            string.Join("|", httpCtx.Request.Headers.UserAgent.Select(x => x ?? ""))
        );
    }

    private static string GetIpAddress(HttpContext httpCtx)
    {
        return httpCtx.Connection.RemoteIpAddress?.ToString() ?? "nullIp";
    }

    private static string GenerateHash(string str)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(str));

        var builder = new StringBuilder();

        // to HEX
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
