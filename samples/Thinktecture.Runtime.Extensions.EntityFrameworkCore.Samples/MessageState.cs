using System;

namespace Thinktecture;

[Union]
public abstract partial record MessageState
{
   public int Order { get; }

   public sealed record Initial : MessageState;

   public sealed record Parsed(DateTime CreatedAt) : MessageState;

   public sealed record Processed(DateTime CreatedAt) : MessageState;

   public sealed record Error(string Message) : MessageState;
}
