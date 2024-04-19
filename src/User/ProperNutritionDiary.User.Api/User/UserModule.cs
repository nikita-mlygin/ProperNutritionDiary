using Carter;
using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProperNutritionDiary.User.Api.User.Login;
using ProperNutritionDiary.User.Api.User.Reg;
using ProperNutritionDiary.User.Api.User.Tokens;

namespace ProperNutritionDiary.User.Api.User;

public class UserModule() : CarterModule("api/")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("guest", GetGuest);
        app.MapPost("r", Refresh);
        app.MapPost("login", Login);
        app.MapPost("reg", Reg);
    }

    private static async Task<Results<Ok<TokenDto>, ForbidHttpResult>> GetGuest(
        UserService userService,
        HttpContext httpCtx
    )
    {
        var tokens = await userService.GenerateGuestToken(httpCtx);

        if (tokens is null)
            return TypedResults.Forbid();

        (var jwt, var rt) = tokens.Value;

        httpCtx.Response.Cookies.Append("rt", rt);

        return TypedResults.Ok(new TokenDto(jwt, rt));
    }

    private static async Task<Results<Ok<TokenDto>, ForbidHttpResult>> Login(
        [FromBody] LoginRequest rq,
        HttpContext httpContext,
        UserService uS
    )
    {
        return (await uS.FirstGeneration(rq.Login, rq.Password, httpContext)).Match(
            res =>
                (Results<Ok<TokenDto>, ForbidHttpResult>)
                    TypedResults.Ok(OnSuccess(res, httpContext)),
            (Error err) => TypedResults.Forbid()
        );
    }

    private static async Task<Results<Ok, ForbidHttpResult>> Reg(
        [FromBody] RegRequest rq,
        UserService uS
    )
    {
        return (await uS.Registration(rq.Login, rq.Password)).Match(
            () => (Results<Ok, ForbidHttpResult>)TypedResults.Ok(),
            err => (Results<Ok, ForbidHttpResult>)TypedResults.Forbid()
        );
    }

    private static async Task<Results<Ok<TokenDto>, ForbidHttpResult>> Refresh(
        [FromBody] string eJwt,
        HttpContext httpContext,
        UserService uS
    )
    {
        return (await uS.RefreshToken(eJwt, httpContext.Request.Cookies["rt"]!, httpContext)).Match(
            res =>
                (Results<Ok<TokenDto>, ForbidHttpResult>)
                    TypedResults.Ok(OnSuccess(res!.Value, httpContext)),
            () => TypedResults.Forbid()
        );
    }

    private static TokenDto OnSuccess((string jwt, string rt) res, HttpContext ctx)
    {
        ctx.Response.Cookies.Append("rt", res.rt);

        return new TokenDto(res.jwt, res.rt);
    }
}
