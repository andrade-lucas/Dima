using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Net.Http.Json;

namespace Dima.Web.Handlers;

public class ReportHandler(IHttpClientFactory httpFactory) : IReportHandler
{
    private readonly HttpClient _client = httpFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpensesByCategoryRequest request)
    {
        try
        {
            return await _client.GetFromJsonAsync<Response<List<ExpensesByCategory>?>>("/v1/reports/expenses")
                ?? new Response<List<ExpensesByCategory>?>(new List<ExpensesByCategory>(), 400, "Erro ao obter saídas");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new Response<List<ExpensesByCategory>?>(null, 400, "Erro ao obter saídas");
        }
    }

    public async Task<Response<FinancialSummary?>> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request)
    {
        try
        {
            return await _client.GetFromJsonAsync<Response<FinancialSummary?>>("/v1/reports/summary")
                ?? new Response<FinancialSummary?>(null, 400, "Erro ao obter resultado financeiro");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new Response<FinancialSummary?>(null, 400, "Erro ao obter resultado financeiro");
        }
    }

    public async Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAndExpensesRequest request)
    {
        try
        {
            return await _client.GetFromJsonAsync<Response<List<IncomesAndExpenses>?>>("/v1/reports/incomes-and-expenses")
                ?? new Response<List<IncomesAndExpenses>?>(new List<IncomesAndExpenses>(), 400, "Erro ao obter saídas e entradas");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new Response<List<IncomesAndExpenses>?>(null, 400, "Erro ao obter saídas e entradas");
        }
    }

    public async Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request)
    {
        try
        {
            return await _client.GetFromJsonAsync<Response<List<IncomesByCategory>?>>("/v1/reports/incomes")
                ?? new Response<List<IncomesByCategory>?>(new List<IncomesByCategory>(), 400, "Erro ao obter entradas");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new Response<List<IncomesByCategory>?>(null, 400, "Erro ao obter entradas");
        }
    }
}
