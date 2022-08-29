using DataLayer;
using DataLayer.Entities;
using LinqToDB.AspNet;
using LinqToDB.Configuration;
using Mailing;
using Mailing.Abstractions;
using Mailing.Settings;
using MailingWebApp.Repositories;
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
    .AddTransient<StorageInitializer>()
    .AddScoped<IMessageModelProviderRepository, FakeMessageModelProviderRepository>()
    .AddScoped<IEmailSendingRepository, FakeEmailSendingRepository>()
    .AddHostedService<MailingBackgroundService>();

var app = builder.Build();

using (IServiceScope initScope = app.Services.CreateScope())
{
    initScope.ServiceProvider.GetRequiredService<StorageInitializer>();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
