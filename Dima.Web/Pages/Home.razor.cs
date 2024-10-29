using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages;

public partial class HomePage : ComponentBase
{
    #region Properties

    public bool ShowValues { get; set; } = true;
    public FinancialSummary? FinancialSummary { get; set; }

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
            var result = await Handler.GetFinancialSummaryReportAsync(new GetFinancialSummaryRequest());

            if (!result.IsSuccess || result.Data == null)
            {
                Snackbar.Add("Não foi possível carregar os dados", Severity.Error);
                return;
            }

            FinancialSummary = result.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add("Não foi possível carregar os dados", Severity.Error);
        }
    }

    #endregion

    #region Methods

    public void ToggleShowValues() => ShowValues = !ShowValues;

    #endregion
}
