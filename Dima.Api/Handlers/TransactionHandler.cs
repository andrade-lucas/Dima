using Dima.Api.Data;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using Dima.Core.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class TransactionHandler(AppDbContext context) : ITransactionHandler
{
    public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        try
        {
            var transaction = new Transaction
            {
                UserId = request.UserId,
                Title = request.Title,
                Type = request.Type,
                Amount = request.Amount,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.Now,
                PaidOrReceivedAt = request.PaidOrReceivedAt
            };

            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, 201, "Transação criada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Transaction?>(null, 500, "Erro ao criar Transação");
        }
    }

    public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (transaction is null)
            {
                return new Response<Transaction?>(null, 404, "Transação não encontrada");
            }

            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, message: "Transação deletada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Transaction?>(null, 500, "Erro ao deletar Transação");
        }
    }

    public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
    {
        try
        {
            var transaction = await context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            return transaction is null
                ? new Response<Transaction?>(null, 404, "Transação não encontrada")
                : new Response<Transaction?>(transaction, message: "Transação deletada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao obter Transação pelo id {0}: {1}", request.Id, ex.Message);
            return new Response<Transaction?>(null, 404, "Transação não encontrada");
        }
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriod(GetTransactionsByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.StartOfMonth();
            request.EndDate ??= DateTime.Now.EndOfMonth();
            var query = context
                .Transactions
                .AsNoTracking()
                .Where(x =>
                    x.UserId == request.UserId
                    && x.CreatedAt >= request.StartDate
                    && x.CreatedAt <= request.EndDate)
                .OrderBy(x => x.Title);

            var transactions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Transaction>?>(transactions, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new PagedResponse<List<Transaction>?>(null, 500, "Ocorreu um erro ao obter Transações");
        }
    }

    public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        try
        {
            var transaction = await context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (transaction is null)
            {
                return new Response<Transaction?>(null, 404, "Transação não encontrada");
            }

            transaction.Title = request.Title;
            transaction.Type = request.Type;
            transaction.Amount = request.Amount;
            transaction.CategoryId = request.CategoryId;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, message: "Transação atualizada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new Response<Transaction?>(null, 500, "Erro ao atualizar Transação");
        }
    }
}
