using CaaS.Api;
using CaaS.Api.Dtos;
using CaaS.Core.Dal.Ado;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.OrderProcessing;
using CaaS.Core.Logic.Statistics;
using CaaS.Core.Security;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddAuthentication("AppKeyAuthentication")
                .AddScheme<AuthenticationSchemeOptions, AppKeyAuthenticationHandler>("AppKeyAuthentication", null);
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IDaoProvider>(new AdoDaoProvider("ProdDbConnection"));
builder.Services.AddSingleton<IDiscountLogic, DiscountLogic>();
builder.Services.AddSingleton<DtoCreator>();
builder.Services.AddSingleton<IAppKeyGenerator, AppKeyGenerator>();
builder.Services.AddSingleton<IOrderProcessingLogic, OrderProcessingLogic>();
builder.Services.AddSingleton<IStatisticsLogic, StatisticsLogic>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(builder =>
    builder.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()));

builder.Services.AddOpenApiDocument(
    settings => settings.PostProcess = doc => doc.Info.Title = "CaaS"
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors();
app.UseOpenApi();
app.UseSwaggerUi3(settings => settings.Path = "/swagger");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
