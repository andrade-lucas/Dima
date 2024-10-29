using Dima.Core.Handlers;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Components.Reports;

public partial class IncomesByCategoryChartComponent : ComponentBase
{
    #region Properties

    public List<double> Data { get; set; } = [];
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
            var result = await Handler.GetIncomesByCategoryReportAsync(new GetIncomesByCategoryRequest());

            if (!result.IsSuccess || result.Data == null)
            {
                Snackbar.Add("Erro ao carregar dados", Severity.Error);
                return;
            }

            foreach (var item in result.Data)
            {
                Labels.Add(item.Category);
                Data.Add((double)item.Incomes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    #endregion
}
