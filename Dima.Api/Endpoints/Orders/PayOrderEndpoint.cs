using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.Api.Endpoints.Orders;

public class PayOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id}/pay", HandleAsync)
            .WithName("Order: Pay an Order")
            .WithSummary("Paga um pedido")
            .WithDescription("Paga um pedido")
            .WithOrder(5)
            .Produces<Response<Order?>>();
    }

    private static async Task<IResult> HandleAsync(
        IOrderHandler handler, 
        ClaimsPrincipal user,
        [FromRoute] long id,
        [FromBody] PayOrderRequest request
    )
    {
        request.Id = id;
        request.UserId = user.Identity!.Name ?? string.Empty;

        var result = await handler.PayAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
