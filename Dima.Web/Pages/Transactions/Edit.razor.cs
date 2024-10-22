using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Requests.Transactions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions;

public partial class EditTransactionPage : ComponentBase
{
    #region Params

    [Parameter]
    public string Id { get; set; } = string.Empty;

    #endregion


    #region Properties

    public bool IsBusy { get; set; } = false;
    public UpdateTransactionRequest InputModel { get; set; } = new();
    public List<Category> Categories { get; set; } = [];

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public ITransactionHandler TransactionHandler { get; set; } = null!;

    [Inject]
    public ICategoryHandler CategoryHandler { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;

        try
        {
            await GetCategoriesAsync();
            await GetTransactionByIdAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Public methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            var result = await TransactionHandler.UpdateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add(result.Message, Severity.Success);
                NavigationManager.NavigateTo("/lancamentos/historico");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
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

    #region Private methods

    public async Task GetCategoriesAsync()
    {
        var request = new GetAllCategoriesRequest();
        var result = await CategoryHandler.GetAllAsync(request);

        if (result is { IsSuccess: true, Data: not null })
        {
            Categories = result.Data;
            InputModel.CategoryId = Categories.FirstOrDefault()?.Id ?? 0;
        }
    }

    private async Task GetTransactionByIdAsync()
    {
        var request = new GetTransactionByIdRequest { Id = long.Parse(Id) };
        var result = await TransactionHandler.GetByIdAsync(request);

        if (result is { IsSuccess: true, Data: not null })
        {
            InputModel.Id = result.Data.Id;
            InputModel.Title = result.Data.Title;
            InputModel.CategoryId = result.Data.CategoryId;
            InputModel.Amount = result.Data.Amount;
            InputModel.Type = result.Data.Type;
            InputModel.PaidOrReceivedAt = result.Data.PaidOrReceivedAt;
        }
        else
            Snackbar.Add(result.Message, Severity.Error);
    }

    #endregion
}
