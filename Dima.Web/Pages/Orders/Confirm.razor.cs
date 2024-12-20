﻿using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class ConfirmOrderPaymentPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public string Number { get; set; } = string.Empty;

    #endregion

    #region Properties

    public Order? Order { get; set; }

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IOrderHandler OrderHandler { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var request = new PayOrderRequest
            {
                Number = Number
            };

            var result = await OrderHandler.PayAsync(request);

            if (result.IsSuccess && result.Data != null)
            {
                Order = result.Data;
                Snackbar.Add(result.Message, Severity.Success);
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch(Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    #endregion
}
