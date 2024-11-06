using Dima.Api.Data;
using Dima.Core.Enums;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class OrderHandler(AppDbContext context) : IOrderHandler
{
    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order == null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Falha ao obter pedido");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(null, 400, "Este pedido já foi cancelado");
            case EOrderStatus.WaitingPayment:
                break;
            case EOrderStatus.Paid:
                return new Response<Order?>(null, 400, "Este pedido já foi pago e não pode ser cancelado");
            case EOrderStatus.Refund:
                return new Response<Order?>(null, 400, "Este pedido já foi reembolsado");
            default:
                return new Response<Order?>(null, 400, "Este pedido não tem um status válido");
        }

        order.Status = EOrderStatus.Canceled;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Internal Server Error");
        }

        return new Response<Order?>(order, message: "Pedido cancelado com sucesso");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        Product? product;

        try
        {
            product = await context
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.IsActive == true);

            if (product == null)
                return new Response<Order?>(null, 404, "Produto não encontrado");

            context.Attach(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Não foi possível obter o produto");
        }

        Voucher? voucher = null;
        try
        {
            if (request.VoucherId is not null)
            {
                voucher = await context
                    .Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.VoucherId && x.IsActive == true);

                if (voucher == null)
                    return new Response<Order?>(null, 404, "Voucher não encontrado");

                voucher.IsActive = false;
                context.Update(voucher);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Ocorreu um erro ao obter o voucher");
        }

        try
        {
            var order = new Order
            {
                UserId = request.UserId,
                VoucherId = request.VoucherId,
                Voucher = voucher,
                ProductId = request.ProductId,
                Product = product
            };

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            return new Response<Order?>(order, 201, $"Pedido {order.Number} criado com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Internal Server Error");
        }
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
    {
        try
        {
            var query = context
                .Orders
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .Where(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.CreatedAt);

            var orders = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync() ?? [];

            var count = await query.CountAsync();

            return new PagedResponse<List<Order>?>(orders, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new PagedResponse<List<Order>?>(null, 500, "Internal Server Error");
        }
    }

    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
    {
        try
        {
            var order = await context
                .Orders
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Number == request.Number);

            return order is null
                ? new Response<Order?>(null, 404, "Pedido não encontrado")
                : new Response<Order?>(order);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Internal Server Error");
        }
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context
                .Orders
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Id == request.Id);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");

            switch (order.Status)
            {
                case EOrderStatus.Canceled:
                    return new Response<Order?>(null, 400, "Este pedido já foi cancelado e não poder ser pago");
                case EOrderStatus.WaitingPayment:
                    break;
                case EOrderStatus.Paid:
                    return new Response<Order?>(null, 400, "Este pedido já foi pago");
                case EOrderStatus.Refund:
                    return new Response<Order?>(null, 400, "Este pedido já foi reembolsado");
                default:
                    return new Response<Order?>(null, 400, "Este pedido não tem um status válido");
            }

            order.Status = EOrderStatus.Paid;
            order.ExternalReference = request.ExternalReference;
            order.UpdatedAt = DateTime.Now;

            context.Orders.Update(order);
            await context.SaveChangesAsync();

            return new Response<Order?>(order, 200, $"Pedido {order.Number} pago com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Ocorreu um erro ao efetuar o pagamento");
        }
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context
                .Orders
                .Include(x => x.Voucher)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Id == request.Id);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");

            switch (order.Status)
            {
                case EOrderStatus.Canceled:
                    return new Response<Order?>(null, 400, "Este pedido já foi cancelado e não poder ser estornado");
                case EOrderStatus.WaitingPayment:
                    return new Response<Order?>(null, 400, "Este pedido ainda não foi pago");
                case EOrderStatus.Paid:
                    break;
                case EOrderStatus.Refund:
                    return new Response<Order?>(null, 400, "Este pedido já foi estornado");
                default:
                    return new Response<Order?>(null, 400, "Este pedido não tem um status válido");
            }

            order.Status = EOrderStatus.Refund;
            order.UpdatedAt = DateTime.Now;
            context.Update(order);
            await context.SaveChangesAsync();

            return new Response<Order?>(order, 200, $"Pedido {order.Number} estornado com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Order?>(null, 500, "Internal Server Error");
        }
    }
}
