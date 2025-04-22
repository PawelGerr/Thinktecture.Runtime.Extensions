using System;
using System.Collections.Generic;

namespace Thinktecture;

public class Message
{
   public required Guid Id { get; init; }
   public required List<MessageState> States { get; set; }
}
