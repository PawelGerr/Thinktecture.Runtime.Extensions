using System.Threading.Tasks;

namespace Thinktecture.Helpers;

public interface INotificationSender
{
   Task SendAsync(string message);
}
