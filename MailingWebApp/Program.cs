using Mailing;
using Mailing.Settings;
using MailingWebApp.Infrastructure;
using NLog.Extensions.Logging;
using PluginManager;
using PluginManager.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<MailingServiceSettings>()
    .BindConfiguration(nameof(MailingServiceSettings));

builder.Services
    .AddStorageContext()
    .AddScopedRepositories()
    .AddSingleton<IPluginService, PluginService>()
    .AddHostedService<StorageInitializerHostedService>()
    .AddHostedService<MailingBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(config => config.AllowAnyOrigin());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
