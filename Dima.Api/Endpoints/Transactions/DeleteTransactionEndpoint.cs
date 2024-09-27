using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;

namespace Dima.Api.Endpoints.Transactions;

public class DeleteTransactionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id}", HandleAsync)
            .WithName("Transactions: Delete")
            .WithSummary("Deleta uma nova transação")
            .WithOrder(4)
            .Produces<Response<Transaction?>>();
    }

    private static async Task<IResult> HandleAsync(long id, ITransactionHandler handler)
    {
        var request = new DeleteTransactionRequest
        {
            Id = id,
            UserId = "test@balta.io"
        };
        var result = await handler.DeleteAsync(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
