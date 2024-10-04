using Dima.Core.Handlers;
using Dima.Core.Requests.Account;
using Dima.Core.Responses;
using System.Net.Http.Json;
using System.Text;

namespace Dima.Web.Handlers;

public class AccountHandler(IHttpClientFactory httpClientFactory) : IAccountHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<string>> LoginAsync(LoginRequest request)
    {
        var result = await _client.PostAsJsonAsync("/v1/identity/login?useCookies=true", request);

        return result.IsSuccessStatusCode
            ? new Response<string>("", 200, "Login realizado com sucesso")
            : new Response<string>(null, 400, "Não foi possível realizar o login");
    }

    public async Task LogoutAsync()
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8, "application/json");
        await _client.PostAsJsonAsync("/v1/identity/logout", emptyContent);
    }

    public async Task<Response<string>> RegisterAsync(RegisterRequest request)
    {
        var result = await _client.PostAsJsonAsync("/v1/identity/register", request);

        return result.IsSuccessStatusCode
            ? new Response<string>("", 201, "Usuário cadastrado com sucesso")
            : new Response<string>(null, 400, "Erro ao cadastrar usuário");
    }
}
