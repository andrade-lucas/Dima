using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions;

public partial class ListTransactionsPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; } = false;
    public List<Transaction> Transactions { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    public int CurrentYear { get; set; } = DateTime.Now.Year;
    public int CurrentMonth { get; set; } = DateTime.Now.Month;

    public int[] Years { get; set; } =
    {
        DateTime.Now.AddYears(1).Year,
        DateTime.Now.AddYears(0).Year,
        DateTime.Now.AddYears(-1).Year,
        DateTime.Now.AddYears(-2).Year,
        DateTime.Now.AddYears(-3).Year,
    };

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IDialogService Dialog { get; set; } = null!;

    [Inject]
    public ITransactionHandler Handler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync() => await GetTransactionsAsync();

    #endregion

    #region Public methods

    public async Task OnSearchAsync()
    {
        await GetTransactionsAsync();
        StateHasChanged();
    }

    public Func<Transaction, bool> Filter => transaction =>
    {
        if (string.IsNullOrEmpty(SearchTerm))
            return true;

        var idMatches = transaction.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
        var titleMatches = transaction.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);

        return idMatches || titleMatches;
    };

    public async Task OnDeleteButtonClickedAsync(long id, string title)
    {
        var result = await Dialog.ShowMessageBox(
            "ATENÇÃO",
            $"Ao prosseguir a transação {title} será excluída. Esta é uma ação irreversível! Deseja continuar?",
            yesText: "Excluir",
            cancelText: "Cancelar"
        );

        if (result == true)
        {
            await DeleteAsync(id, title);
        }
    }

    #endregion

    #region Private methods

    private async Task DeleteAsync(long id, string title)
    {
        IsBusy = true;

        try
        {
            var request = new DeleteTransactionRequest { Id = id };
            var response = await Handler.DeleteAsync(request);

            Snackbar.Add($"{title} excluída com sucesso", Severity.Success);
            Transactions.RemoveAll(x => x.Id == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetTransactionsAsync()
    {
        IsBusy = true;

        try
        {
            var request = new GetTransactionsByPeriodRequest
            {
                StartDate = DateTime.Now.StartOfMonth(CurrentYear, CurrentMonth),
                EndDate = DateTime.Now.EndOfMonth(CurrentYear, CurrentMonth),
                PageNumber = 1,
                PageSize = 1000
            };
            var response = await Handler.GetByPeriod(request);

            if (response.IsSuccess)
            {
                Transactions = response.Data ?? [];
            }
            else
                Snackbar.Add(response.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}
