using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;

namespace Dima.Web.Components.Reports;

public partial class IncomesAndExpensesComponent : ComponentBase
{
    #region Properties

    public ChartOptions Options { get; set; } = new();
    public List<ChartSeries>? Series { get; set; }
    public List<string> Labels { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IReportHandler Handler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Handler.GetIncomesAndExpensesReportAsync(new GetIncomesAndExpensesRequest());

            if (!result.IsSuccess || result.Data == null)
            {
                Snackbar.Add("Erro ao obter dados", Severity.Error);
                return;
            }

            var incomes = new List<double>();
            var expenses = new List<double>();

            foreach (var item in result.Data)
            {
                incomes.Add((double)item.Incomes);
                expenses.Add((double)item.Expenses * -1);
                Labels.Add(GetMonthName(item.Month));
            }

            Options.YAxisTicks = 1000;
            Options.LineStrokeWidth = 5;
            Options.ChartPalette = ["#76FF01", Colors.Red.Default];
            Series =
            [
                new ChartSeries { Name = "Entradas", Data = incomes.ToArray() },
                new ChartSeries { Name = "Saídas", Data = expenses.ToArray() },
            ];

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    #endregion

    #region Private methods

    private static string GetMonthName(int month)
        => new DateTime(DateTime.Now.Year, month, 1).ToString("MMM", CultureInfo.CurrentCulture);

    #endregion
}
