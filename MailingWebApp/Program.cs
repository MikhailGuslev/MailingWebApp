using DataLayer.Entities;
using LinqToDB.AspNet;
using LinqToDB.Configuration;
using Mailing;
using Mailing.Settings;
using MailingWebApp.Infrastructure;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<MailingServiceSettings>()
    .BindConfiguration(nameof(MailingServiceSettings));

string connectStr = builder.Configuration
    .GetConnectionString(nameof(StorageDb));

builder.Services
    .AddLinqToDBContext<StorageDb>((config, options) =>
        options.UseSQLite(connectStr)
    );

builder.Services
    .AddScopedRepositories()
    .AddHostedService<StorageInitializerHostedService>()
    .AddHostedService<MailingBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
