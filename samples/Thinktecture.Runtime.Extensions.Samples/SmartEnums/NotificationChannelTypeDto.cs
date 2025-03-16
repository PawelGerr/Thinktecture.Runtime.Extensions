using System;
using Microsoft.Extensions.DependencyInjection;
using Thinktecture.Helpers;

namespace Thinktecture.SmartEnums;

// Smart enum representing DTO for notification channel types.
[SmartEnum<string>(KeyMemberName = "Name")]
public abstract partial class NotificationChannelTypeDto
{
   public static readonly NotificationChannelTypeDto Email = new NotificationTypeDto<EmailNotificationSender>("email");
   public static readonly NotificationChannelTypeDto Sms = new NotificationTypeDto<SmsNotificationSender>("sms");

   // Add further properties and methods as needed.

   public abstract INotificationSender GetNotificationSender(IServiceProvider serviceProvider);

   // Generic derived class that captures the implementation type.
   // Having concrete (generic) type, we get a compile time type check (in comparison to typeof(EmailNotificationSender))
   // and we could do advanced stuff inside the derived class.
   private sealed class NotificationTypeDto<TImplementation> : NotificationChannelTypeDto
      where TImplementation : class, INotificationSender
   {
      public NotificationTypeDto(string key)
         : base(key)
      {
      }

      public override INotificationSender GetNotificationSender(IServiceProvider serviceProvider)
      {
         return serviceProvider.GetRequiredService<TImplementation>();
      }
   }
}
