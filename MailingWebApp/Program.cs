using Mailing;
using Mailing.Abstractions;
using Mailing.Repositories;
using Mailing.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<MailingServiceSettings>()
    .BindConfiguration(nameof(MailingServiceSettings));

builder.Services
    .AddSingleton<IMessageModelProviderRepository, FakeMessageModelProviderRepository>()
    .AddSingleton<IEmailSendingRepository, FakeEmailSendingRepository>()
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
