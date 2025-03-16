using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Thinktecture.Helpers;

public class SmsNotificationSender(ILogger<SmsNotificationSender> logger) : INotificationSender
{
   public Task SendAsync(string message)
   {
      logger.LogInformation("Sending sms: {Message}", message);
      return Task.CompletedTask;
   }
}
