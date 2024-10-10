using Dima.Core.Handlers;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;

namespace Dima.Web.Pages.Identity;

public partial class LogoutPage : ComponentBase
{
    #region Services

    [Inject]
    public IAccountHandler Handler { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    [Inject]
    public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        if (await AuthenticationStateProvider.CheckAuthenticatedAsync())
        {
            await Handler.LogoutAsync();
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            AuthenticationStateProvider.NotifyAuthenticationStateChanged();
        }

        await base.OnInitializedAsync();
    }

    #endregion
}
