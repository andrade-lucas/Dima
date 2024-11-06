using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.Api.Endpoints.Orders;

public class RefundOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id}/refund", HandleAsync)
            .WithName("Order: Refund an Order")
            .WithSummary("Estorna um pedido")
            .WithDescription("Estorna um pedido")
            .WithOrder(6)
            .Produces<Response<Order?>>();
    }

    private static async Task<IResult> HandleAsync(IOrderHandler handler, ClaimsPrincipal user, [FromRoute] long id)
    {
        var request = new RefundOrderRequest
        {
            Id = id,
            UserId = user.Identity!.Name ?? string.Empty
        };

        var result = await handler.RefundAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
