using Dima.Api.Data;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class ProductHandler(AppDbContext context) : IProductHandler
{
    public async Task<PagedResponse<List<Product>?>> GetAllAsync(GetAllProductsRequest request)
    {
        try
        {
            var query = context
                .Products
                .AsNoTracking()
                .Where(x => x.IsActive);

            var products = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync() ?? [];

            var count = products.Count();

            return new PagedResponse<List<Product>?>(products, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new PagedResponse<List<Product>?>(null, 500, "Internal Server Error");
        }
    }

    public async Task<Response<Product?>> GetBySlugAsync(GetProductBySlugRequest request)
    {
        try
        {
            var product = await context
                .Products
                .AsNoTracking()
                .Where(x => x.IsActive && x.Slug == request.Slug)
                .FirstOrDefaultAsync();

            return product == null
                ? new Response<Product?>(null, 404, "Produto não encontrado")
                : new Response<Product?>(product);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Product?>(null, 500, "Internal Server Error");
        }
    }
}
