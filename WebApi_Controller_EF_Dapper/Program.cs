using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text.Json;
using WebApi_Controller_EF_Dapper.Business;
using WebApi_Controller_EF_Dapper.Domain.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//==================================================================================================
//Serviços
//==================================================================================================

//Adicionando o serviço do Swagger
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // Resolve conflito de nomes de endpoits no swagger
});

//--------------------------------------------------------------------------------------------------
//Log de aplicaçõa com SeriLog
//--------------------------------------------------------------------------------------------------

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["Database:SQlServer"],
            sinkOptions: new MSSqlServerSinkOptions()
            {
                AutoCreateSqlTable = true,
                TableName = "ApplicationLog"
            });
});

//--------------------------------------------------------------------------------------------------
//DBContext
//--------------------------------------------------------------------------------------------------
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SQlServer"]);
builder.Services.AddMvc();

builder.Services.AddEndpointsApiExplorer();

//--------------------------------------------------------------------------------------------------
//Meus servicos personalizados da aplicacao
//--------------------------------------------------------------------------------------------------
builder.Services.AddScoped<ServiceAllProductsSold>();

//==================================================================================================
//Application
//==================================================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//-----------------------------------------------------------------
//filtro de erros
//-----------------------------------------------------------------

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "DataBase Out!!!", statusCode: 500);
        else if (error is FormatException)
            return Results.Problem(title: "Error to convert data to other type format", statusCode: 500);
        else if (error is JsonException)
            return Results.Problem(title: "Format error on current Json", statusCode: 500);
    }

    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run(); //<============ RUN