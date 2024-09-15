using Dima.Api.Data;
using Dima.Api.Handlers;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Categories;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cnnStr = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

builder.Services.AddDbContext<AppDbContext>(x => 
{
    x.UseSqlServer(cnnStr);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => 
{
    x.CustomSchemaIds(n => n.FullName);
});
builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost(
        "/v1/categories", 
        (CreateCategoryRequest request, ICategoryHandler handler) => handler.CreateAsync(request)
    )
    .WithName("Categories: Create")
    .WithSummary("Cria uma nova categoria")
    .Produces<Response<Category?>>();

app.MapPut(
        "/v1/categories/{id}", 
        (long id, UpdateCategoryRequest request, ICategoryHandler handler) => {
            request.Id = id;
            handler.UpdateAsync(request);
        })
    .WithName("Categories: Update")
    .WithSummary("Atualiza uma nova categoria")
    .Produces<Response<Category?>>();

app.MapDelete(
        "/v1/categories/{id}", 
        (long id, [FromBody]DeleteCategoryRequest request, ICategoryHandler handler) => {
            request.Id = id;
            handler.DeleteAsync(request);
        })
    .WithName("Categories: Delete")
    .WithSummary("Deleta uma nova categoria")
    .Produces<Response<Category?>>();

app.Run();
