﻿using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using System.Net.Http.Json;

namespace Dima.Web.Handlers;

public class OrderHandler(IHttpClientFactory httpClientFactory) : IOrderHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"/v1/orders/{request.Id}/cancel", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Erro ao cancelar pedido");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync("/v1/orders", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Erro ao criar pedido");
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
    {
        return await _client.GetFromJsonAsync<PagedResponse<List<Order>?>>("/v1/orders")
            ?? new PagedResponse<List<Order>?>(null, 400, "Erro ao obter pedidos");
    }

    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
    {
        return await _client.GetFromJsonAsync<Response<Order?>>($"v1/orders/{request.Number}")
            ?? new Response<Order?>(null, 400, "Erro ao obter pedido");
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"/v1/orders/{request.Id}/pay", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Erro ao pagar pedido");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        var result = await _client.PostAsJsonAsync($"/v1/orders/{request.Id}/refund", request);

        return await result.Content.ReadFromJsonAsync<Response<Order?>>()
            ?? new Response<Order?>(null, 400, "Erro ao estornar pedido");
    }
}
