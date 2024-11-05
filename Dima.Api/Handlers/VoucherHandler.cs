using Dima.Api.Data;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class VoucherHandler(AppDbContext context) : IVoucherHandler
{
    public async Task<Response<Voucher?>> GetByNumberAsync(GetVoucherByNumberRequest request)
    {
        try
        {
            var voucher = await context
                .Vouchers
                .AsNoTracking()
                .Where(x => x.Number == request.Number && x.IsActive == true)
                .FirstOrDefaultAsync();

            return voucher is null
                ? new Response<Voucher?>(null, 404, "Voucher não encontrado")
                : new Response<Voucher?>(voucher);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Voucher?>(null, 500, ex.Message);
        }
    }
}
