using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class OrderDetailsPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public string Number { get; set; } = string.Empty;

    #endregion

    #region Properties

    public bool IsBusy { get; set; } = false;
    public Order? Order { get; set; }

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    [Inject]
    public IOrderHandler Handler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = new GetOrderByNumberRequest
            {
                Number = Number,
            };
            var result = await Handler.GetByNumberAsync(request);

            if (result is { IsSuccess: true, Data: not null })
                Order = result.Data;
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add("Erro ao carregar pedido", Severity.Error);
        }
    }

    #endregion

    #region Private Methods

    private async Task PayOrderAsync()
    {
        IsBusy = true;

        try
        {
            var request = new PayOrderRequest
            {
                Id = Order!.Id,
            };
            var result = await Handler.PayAsync(request);
            if (result.IsSuccess)
            {
                Snackbar.Add("Pedido pago com sucesso", Severity.Success);
                Navigation.NavigateTo("/");
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
}
