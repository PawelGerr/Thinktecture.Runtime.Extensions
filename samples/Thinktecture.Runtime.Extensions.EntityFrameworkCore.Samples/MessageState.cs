using System;

namespace Thinktecture;

[Union]
public abstract partial record MessageState
{
   public int Order { get; }

   public record Initial : MessageState;

   public record Parsed(DateTime CreatedAt) : MessageState;

   public record Processed(DateTime CreatedAt) : MessageState;

   public record Error(string Message) : MessageState;
}
