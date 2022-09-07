using Mailing;
using Mailing.Models;
using Microsoft.AspNetCore.Mvc;

namespace MailingWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SendingsController : ControllerBase
{
    private readonly ILogger<SendingsController> Logger;
    private readonly EmailSendingScheduler EmailSendingScheduler;

    public SendingsController(
        ILogger<SendingsController> logger,
        MailingBackgroundService mailingBackgroundService)
    {
        Logger = logger;
        EmailSendingScheduler = mailingBackgroundService.EmailSendingScheduler;
    }

    // GET: api/<SendingsController>
    [HttpGet]
    public async Task<IReadOnlyList<EmailSending>> GetAsync()
    {
        IReadOnlyList<EmailSending> sendings = await EmailSendingScheduler.GetAllEmailSendingsAsync();
        return sendings;
    }

    // GET api/<SendingsController>/5
    [HttpGet("{id}")]
    public async Task<EmailSending> Get(int id)
    {
        EmailSending emailSending = await EmailSendingScheduler.GetEmailSendingByIdAsync(id);
        return emailSending;
    }

    // POST api/<SendingsController>
    [HttpPost]
    public async Task Post([FromBody] EmailSending emailSending)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    // PUT api/<SendingsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<SendingsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
