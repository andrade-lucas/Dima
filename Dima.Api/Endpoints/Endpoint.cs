using Dima.Api.Common.Api;
using Dima.Api.Endpoints.Categories;
using Dima.Api.Endpoints.Identity;
using Dima.Api.Endpoints.Reports;
using Dima.Api.Endpoints.Transactions;
using Dima.Api.Models;

namespace Dima.Api.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("");

        endpoints.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { message = "OK" }).WithTags("Health Check");

        endpoints.MapGroup("v1/categories")
            .WithTags("Categories")
            .RequireAuthorization()
            .MapEndpoint<GetAllCategoriesEndpoint>()
            .MapEndpoint<GetCategoryByIdEndpoint>()
            .MapEndpoint<CreateCategoryEndpoint>()
            .MapEndpoint<UpdateCategoryEndpoint>()
            .MapEndpoint<DeleteCategoryEndpoint>();

        endpoints.MapGroup("v1/transactions")
            .WithTags("Transactions")
            .RequireAuthorization()
            .MapEndpoint<GetTransactionsByPeriodEndpoint>()
            .MapEndpoint<GetTransactionByIdEndpoint>()
            .MapEndpoint<CreateTransactionEndpoint>()
            .MapEndpoint<UpdateTransactionEndpoint>()
            .MapEndpoint<DeleteTransactionEndpoint>();

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapIdentityApi<User>();

        endpoints.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoint<GetRolesEndpoint>()
            .MapEndpoint<LogoutEndpoint>();

        endpoints.MapGroup("v1/reports")
            .WithTags("Reports")
            .RequireAuthorization()
            .MapEndpoint<GetExpensesByCategoryEndpoint>()
            .MapEndpoint<GetFinancialSummaryEndpoint>()
            .MapEndpoint<GetIncomesAndExpensesEndpoint>()
            .MapEndpoint<GetIncomesByCategoryEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);

        return app;
    }
}
