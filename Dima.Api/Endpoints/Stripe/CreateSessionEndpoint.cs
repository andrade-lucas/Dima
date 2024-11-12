using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Requests.Stripe;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.Api.Endpoints.Stripe;

public class CreateSessionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .WithName("Stripe: Create Session")
            .WithSummary("Cria uma nova sessão do Stripe")
            .WithOrder(1)
            .Produces<Response<string?>>();
    }

    private static async Task<IResult> HandleAsync(IStripeHandler handler, ClaimsPrincipal user, CreateSessionRequest request)
    {
        request.UserId = user.Identity!.Name ?? string.Empty;
        var result = await handler.CreateSessionAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
