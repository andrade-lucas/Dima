using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class CheckoutPage : ComponentBase
{
    #region Parameters

    [Parameter]
    public string ProductSlug { get; set; } = string.Empty;

    [SupplyParameterFromQuery(Name = "voucher")]
    public string? VoucherNumber { get; set; }

    #endregion

    #region Properties

    public PatternMask Mask = new("####-####")
    {
        MaskChars = [new MaskChar('#', @"[0-9a-fA-F]")],
        Placeholder = '_',
        CleanDelimiters = true,
        Transformation = AllUpperCase
    };
    public bool IsBusy { get; set; } = false;
    public bool IsValid { get; set; } = false;
    public CreateOrderRequest InputModel { get; set; } = new();
    public Product? Product { get; set; }
    public Voucher? Voucher { get; set; }
    public decimal Total { get; set; }

    #endregion

    #region Services

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public IProductHandler ProductHandler { get; set; } = null!;

    [Inject]
    public IVoucherHandler VoucherHandler { get; set; } = null!;

    [Inject]
    public IOrderHandler OrderHandler { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        await GetProduct();
        await GetVoucher();

        IsValid = true;
        Total = Product!.Price - (Voucher?.Amount ?? 0);
        
        if (Total < 0)
            Total = 0;
    }

    #endregion

    #region Public methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        try
        {
            InputModel.ProductId = Product!.Id;
            InputModel.VoucherId = Voucher?.Id;

            var result = await OrderHandler.CreateAsync(InputModel);
            if (result.IsSuccess)
            {
                Snackbar.Add("Pedido realizado com sucesso", Severity.Success);
                Navigation.NavigateTo($"/pedidos/{result.Data!.Number}");
            }
            else
                Snackbar.Add("Erro ao gerar pedido", Severity.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Snackbar.Add("Ocorreu um erro ao gerar pedido", Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Private methods

    private async Task GetProduct()
    {
        if (string.IsNullOrEmpty(ProductSlug))
        {
            Navigation.NavigateTo("premium");
            return;
        }

        try
        {
            var request = new GetProductBySlugRequest
            {
                Slug = ProductSlug
            };
            var result = await ProductHandler.GetBySlugAsync(request);
            if (result is { IsSuccess: true, Data: not null })
                Product = result.Data;
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task GetVoucher()
    {
        if (string.IsNullOrEmpty(VoucherNumber))
            return;

        try
        {
            var request = new GetVoucherByNumberRequest
            {
                Number = VoucherNumber.Replace("-", "")
            };
            var result = await VoucherHandler.GetByNumberAsync(request);
            if (result is { IsSuccess: true, Data: not null })
                Voucher = result.Data;
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    #endregion

    private static char AllUpperCase(char c) => c.ToString().ToUpperInvariant()[0];
}
