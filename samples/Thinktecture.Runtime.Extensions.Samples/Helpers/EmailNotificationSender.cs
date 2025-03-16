using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Thinktecture.Helpers;

public class EmailNotificationSender(ILogger<EmailNotificationSender> logger) : INotificationSender
{
   public Task SendAsync(string message)
   {
      logger.LogInformation("Sending email: {Message}", message);
      return Task.CompletedTask;
   }
}
