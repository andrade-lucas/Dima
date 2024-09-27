using Dima.Api.Common.Api;
using Dima.Core;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dima.Api.Endpoints.Transactions;

public class GetTransactionsByPeriodEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", HandleAsync)
            .WithName("Transactions: Get All")
            .WithSummary("Obter todas as transações de um usuário dentro de um período")
            .WithOrder(0)
            .Produces<PagedResponse<List<Transaction>?>>();
    }

    private static async Task<IResult> HandleAsync(
        ITransactionHandler handler, 
        [FromQuery]DateTime? startDate = null,
        [FromQuery]DateTime? endDate = null,
        [FromQuery]int pageNumber = Configuration.DefaultPageNumber, 
        [FromQuery]int pageSize = Configuration.DefaultPageSize
    )
    {
        var request = new GetTransactionsByPeriodRequest
        {
            UserId = "test@balta.io",
            PageNumber = pageNumber,
            PageSize = pageSize,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await handler.GetByPeriod(request);

        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}
