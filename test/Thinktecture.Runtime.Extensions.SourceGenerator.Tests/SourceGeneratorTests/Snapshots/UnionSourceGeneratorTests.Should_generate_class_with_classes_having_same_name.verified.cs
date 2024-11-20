﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

abstract partial class TestUnion
{
   private TestUnion()
   {
   }

   /// <summary>
   /// Executes an action depending on the current type.
   /// </summary>
   /// <param name="child1">The action to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child1"/>.</param>
   /// <param name="child2">The action to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2"/>.</param>
   /// <param name="child2Child1">The action to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2.Child1"/>.</param>
   public void Switch(
      global::System.Action<global::Thinktecture.Tests.TestUnion.Child1> @child1,
      global::System.Action<global::Thinktecture.Tests.TestUnion.Child2> @child2,
      global::System.Action<global::Thinktecture.Tests.TestUnion.Child2.Child1> @child2Child1)
   {
      switch (this)
      {
         case global::Thinktecture.Tests.TestUnion.Child1 value:
            @child1(value);
            return;
         case global::Thinktecture.Tests.TestUnion.Child2 value:
            @child2(value);
            return;
         case global::Thinktecture.Tests.TestUnion.Child2.Child1 value:
            @child2Child1(value);
            return;
         default:
            throw new global::System.ArgumentOutOfRangeException($"Unexpected type '{this.GetType().FullName}'.");
      }
   }

   /// <summary>
   /// Executes an action depending on the current type.
   /// </summary>
   /// <param name="state">State to be passed to the callbacks.</param>
   /// <param name="child1">The action to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child1"/>.</param>
   /// <param name="child2">The action to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2"/>.</param>
   /// <param name="child2Child1">The action to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2.Child1"/>.</param>
   public void Switch<TState>(
      TState state,
      global::System.Action<TState, global::Thinktecture.Tests.TestUnion.Child1> @child1,
      global::System.Action<TState, global::Thinktecture.Tests.TestUnion.Child2> @child2,
      global::System.Action<TState, global::Thinktecture.Tests.TestUnion.Child2.Child1> @child2Child1)
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif
   {
      switch (this)
      {
         case global::Thinktecture.Tests.TestUnion.Child1 value:
            @child1(state, value);
            return;
         case global::Thinktecture.Tests.TestUnion.Child2 value:
            @child2(state, value);
            return;
         case global::Thinktecture.Tests.TestUnion.Child2.Child1 value:
            @child2Child1(state, value);
            return;
         default:
            throw new global::System.ArgumentOutOfRangeException($"Unexpected type '{this.GetType().FullName}'.");
      }
   }

   /// <summary>
   /// Executes a function depending on the current type.
   /// </summary>
   /// <param name="child1">The function to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child1"/>.</param>
   /// <param name="child2">The function to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2"/>.</param>
   /// <param name="child2Child1">The function to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2.Child1"/>.</param>
   public TResult Switch<TResult>(
      global::System.Func<global::Thinktecture.Tests.TestUnion.Child1, TResult> @child1,
      global::System.Func<global::Thinktecture.Tests.TestUnion.Child2, TResult> @child2,
      global::System.Func<global::Thinktecture.Tests.TestUnion.Child2.Child1, TResult> @child2Child1)
#if NET9_0_OR_GREATER
		where TResult : allows ref struct
#endif
   {
      switch (this)
      {
         case global::Thinktecture.Tests.TestUnion.Child1 value:
            return @child1(value);
         case global::Thinktecture.Tests.TestUnion.Child2 value:
            return @child2(value);
         case global::Thinktecture.Tests.TestUnion.Child2.Child1 value:
            return @child2Child1(value);
         default:
            throw new global::System.ArgumentOutOfRangeException($"Unexpected type '{this.GetType().FullName}'.");
      }
   }

   /// <summary>
   /// Executes a function depending on the current type.
   /// </summary>
   /// <param name="state">State to be passed to the callbacks.</param>
   /// <param name="child1">The function to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child1"/>.</param>
   /// <param name="child2">The function to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2"/>.</param>
   /// <param name="child2Child1">The function to execute if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2.Child1"/>.</param>
   public TResult Switch<TState, TResult>(
      TState state,
      global::System.Func<TState, global::Thinktecture.Tests.TestUnion.Child1, TResult> @child1,
      global::System.Func<TState, global::Thinktecture.Tests.TestUnion.Child2, TResult> @child2,
      global::System.Func<TState, global::Thinktecture.Tests.TestUnion.Child2.Child1, TResult> @child2Child1)
#if NET9_0_OR_GREATER
		where TResult : allows ref struct
		where TState : allows ref struct
#endif
   {
      switch (this)
      {
         case global::Thinktecture.Tests.TestUnion.Child1 value:
            return @child1(state, value);
         case global::Thinktecture.Tests.TestUnion.Child2 value:
            return @child2(state, value);
         case global::Thinktecture.Tests.TestUnion.Child2.Child1 value:
            return @child2Child1(state, value);
         default:
            throw new global::System.ArgumentOutOfRangeException($"Unexpected type '{this.GetType().FullName}'.");
      }
   }

   /// <summary>
   /// Maps current instance to an instance of type <typeparamref name="TResult"/>.
   /// </summary>
   /// <param name="child1">The instance to return if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child1"/>.</param>
   /// <param name="child2">The instance to return if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2"/>.</param>
   /// <param name="child2Child1">The instance to return if the current type is <see cref="global::Thinktecture.Tests.TestUnion.Child2.Child1"/>.</param>
   public TResult Map<TResult>(
      TResult @child1,
      TResult @child2,
      TResult @child2Child1)
#if NET9_0_OR_GREATER
		where TResult : allows ref struct
#endif
   {
      switch (this)
      {
         case global::Thinktecture.Tests.TestUnion.Child1 value:
               return @child1;
         case global::Thinktecture.Tests.TestUnion.Child2 value:
               return @child2;
         case global::Thinktecture.Tests.TestUnion.Child2.Child1 value:
               return @child2Child1;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unexpected type '{this.GetType().FullName}'.");
         }
   }
}
