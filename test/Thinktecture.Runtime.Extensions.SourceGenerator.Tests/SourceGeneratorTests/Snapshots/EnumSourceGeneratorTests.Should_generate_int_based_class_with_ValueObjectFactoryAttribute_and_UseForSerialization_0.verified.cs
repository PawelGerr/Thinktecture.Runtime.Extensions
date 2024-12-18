﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, int, global::Thinktecture.ValidationError>))]
   sealed partial class TestEnum : global::Thinktecture.IEnum<int, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
      global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>,
      global::Thinktecture.IValueObjectConvertable<string>,
      global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         var convertFromKey = new global::System.Func<int, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
         global::System.Linq.Expressions.Expression<global::System.Func<int, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static @key => global::Thinktecture.Tests.TestEnum.Get(@key);

         var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, int>(static item => item.Key);
         global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, int>> convertToKeyExpression = static item => item.Key;

         var enumType = typeof(global::Thinktecture.Tests.TestEnum);
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(int), true, false, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
      }

      private static readonly global::System.Lazy<Lookups> _lookups = new global::System.Lazy<Lookups>(GetLookups, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _lookups.Value.List;

      /// <summary>
      /// The identifier of this item.
      /// </summary>
      public int Key { get; }

      private readonly int _hashCode;
      private readonly global::System.Lazy<int> _itemIndex;

      private TestEnum(
         int @key)
      {
         ValidateConstructorArguments(
            ref @key);

         this.Key = @key;
         this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), @key.GetHashCode());
         this._itemIndex = new global::System.Lazy<int>(() =>
                                                        {
                                                           for (var i = 0; i < Items.Count; i++)
                                                           {
                                                              if (this == Items[i])
                                                                 return i;
                                                           }

                                                           throw new global::System.Exception($"Current item '{@key}' not found in 'Items'.");
                                                        }, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);
      }

      static partial void ValidateConstructorArguments(
         ref int @key);

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      int global::Thinktecture.IValueObjectConvertable<int>.ToValue()
      {
         return this.Key;
      }

      /// <summary>
      /// Gets an enumeration item for provided <paramref name="key"/>.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      /// <exception cref="Thinktecture.UnknownEnumIdentifierException">If there is no item with the provided <paramref name="key"/>.</exception>
      public static global::Thinktecture.Tests.TestEnum Get(int @key)
      {
         if (!_lookups.Value.Lookup.TryGetValue(@key, out var item))
         {
            throw new global::Thinktecture.UnknownEnumIdentifierException(typeof(global::Thinktecture.Tests.TestEnum), @key);
         }

         return item;
      }

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] int @key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
      {
         return _lookups.Value.Lookup.TryGetValue(@key, out item);
      }

      /// <summary>
      /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <param name="provider">An object that provides culture-specific formatting information.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
      public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] int @key, global::System.IFormatProvider? @provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
      {
         if(global::Thinktecture.Tests.TestEnum.TryGet(@key, out item))
         {
            return null;
         }
         else
         {
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{@key}'.");
         }
      }

      /// <summary>
      /// Implicit conversion to the type <see cref="int"/>.
      /// </summary>
      /// <param name="item">Item to covert.</param>
      /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
      public static implicit operator int(global::Thinktecture.Tests.TestEnum? item)
      {
         return item is null ? default : item.Key;
      }

      /// <summary>
      /// Explicit conversion from the type <see cref="int"/>.
      /// </summary>
      /// <param name="key">Value to covert.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
      public static explicit operator global::Thinktecture.Tests.TestEnum?(int @key)
      {
         return global::Thinktecture.Tests.TestEnum.Get(@key);
      }

      /// <inheritdoc />
      public bool Equals(global::Thinktecture.Tests.TestEnum? other)
      {
         return global::System.Object.ReferenceEquals(this, other);
      }

      /// <inheritdoc />
      public override bool Equals(object? other)
      {
         return other is global::Thinktecture.Tests.TestEnum item && Equals(item);
      }

      /// <inheritdoc />
      public override int GetHashCode()
      {
         return _hashCode;
      }

      /// <inheritdoc />
      public override string ToString()
      {
         return this.Key.ToString();
      }

      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void Switch(
         global::System.Action @item1,
         global::System.Action @item2)
      {
         switch (_itemIndex.Value)
         {
            case 0:
               @item1();
               return;
            case 1:
               @item2();
               return;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }
      }

      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>
      /// <param name="default">The action to execute if no item-specific action is provided.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void SwitchPartially(
         global::System.Action<global::Thinktecture.Tests.TestEnum>? @default = null,
         global::System.Action? @item1 = null,
         global::System.Action? @item2 = null)
      {
         switch (_itemIndex.Value)
         {
            case 0:
               if (@item1 is null)
                  break;

               @item1();
               return;
            case 1:
               if (@item2 is null)
                  break;

               @item2();
               return;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }

         @default?.Invoke(this);
      }

      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>
      /// <param name="state">State to be passed to the callbacks.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void Switch<TState>(
         TState state,
         global::System.Action<TState> @item1,
         global::System.Action<TState> @item2)
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               @item1(state);
               return;
            case 1:
               @item2(state);
               return;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }
      }

      /// <summary>
      /// Executes an action depending on the current item.
      /// </summary>
      /// <param name="state">State to be passed to the callbacks.</param>
      /// <param name="default">The action to execute if no item-specific action is provided.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void SwitchPartially<TState>(
         TState state,
         global::System.Action<TState, global::Thinktecture.Tests.TestEnum>? @default = null,
         global::System.Action<TState>? @item1 = null,
         global::System.Action<TState>? @item2 = null)
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               if (@item1 is null)
                  break;

               @item1(state);
               return;
            case 1:
               if (@item2 is null)
                  break;

               @item2(state);
               return;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }

         @default?.Invoke(state, this);
      }

      /// <summary>
      /// Executes a function depending on the current item.
      /// </summary>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult Switch<TResult>(
         global::System.Func<TResult> @item1,
         global::System.Func<TResult> @item2)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               return @item1();
            case 1:
               return @item2();
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }
      }

      /// <summary>
      /// Executes a function depending on the current item.
      /// </summary>
      /// <param name="default">The function to execute if no item-specific action is provided.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult SwitchPartially<TResult>(
         global::System.Func<global::Thinktecture.Tests.TestEnum, TResult> @default,
         global::System.Func<TResult>? @item1 = null,
         global::System.Func<TResult>? @item2 = null)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               if (@item1 is null)
                  break;

               return @item1();
            case 1:
               if (@item2 is null)
                  break;

               return @item2();
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }

         return @default(this);
      }

      /// <summary>
      /// Executes a function depending on the current item.
      /// </summary>
      /// <param name="state">State to be passed to the callbacks.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult Switch<TState, TResult>(
         TState state,
         global::System.Func<TState, TResult> @item1,
         global::System.Func<TState, TResult> @item2)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
		   where TState : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               return @item1(state);
            case 1:
               return @item2(state);
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }
      }

      /// <summary>
      /// Executes a function depending on the current item.
      /// </summary>
      /// <param name="state">State to be passed to the callbacks.</param>
      /// <param name="default">The function to execute if no item-specific action is provided.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult SwitchPartially<TState, TResult>(
         TState state,
         global::System.Func<TState, global::Thinktecture.Tests.TestEnum, TResult> @default,
         global::System.Func<TState, TResult>? @item1 = null,
         global::System.Func<TState, TResult>? @item2 = null)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
		   where TState : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               if (@item1 is null)
                  break;

               return @item1(state);
            case 1:
               if (@item2 is null)
                  break;

               return @item2(state);
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }

         return @default(state, this);
      }

      /// <summary>
      /// Maps an item to an instance of type <typeparamref name="TResult"/>.
      /// </summary>
      /// <param name="item1">The instance to return if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The instance to return if the current item is equal to <see cref="Item2"/>.</param>
      public TResult Map<TResult>(
         TResult @item1,
         TResult @item2)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               return @item1;
            case 1:
               return @item2;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }
      }

      /// <summary>
      /// Maps an item to an instance of type <typeparamref name="TResult"/>.
      /// </summary>
      /// <param name="default">The instance to return if no value is provided for current item.</param>
      /// <param name="item1">The instance to return if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The instance to return if the current item is equal to <see cref="Item2"/>.</param>
      public TResult MapPartially<TResult>(
         TResult @default,
         global::Thinktecture.Argument<TResult> @item1 = default,
         global::Thinktecture.Argument<TResult> @item2 = default)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         switch (_itemIndex.Value)
         {
            case 0:
               if (!@item1.IsSet)
                  break;

               return @item1.Value;
            case 1:
               if (!@item2.IsSet)
                  break;

               return @item2.Value;
            default:
               throw new global::System.ArgumentOutOfRangeException($"Unknown item '{this}'.");
         }

         return @default;
      }

      private static Lookups GetLookups()
      {
         var lookup = new global::System.Collections.Generic.Dictionary<int, global::Thinktecture.Tests.TestEnum>(2);
         var list = new global::System.Collections.Generic.List<global::Thinktecture.Tests.TestEnum>(2);

         void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
         {
            if (item is null)
               throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

            if (lookup.ContainsKey(item.Key))
               throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

            lookup.Add(item.Key, item);
            list.Add(item);
         }

         AddItem(@Item1, nameof(@Item1));
         AddItem(@Item2, nameof(@Item2));

#if NET8_0_OR_GREATER
         var frozenDictionary = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup);
         return new Lookups(frozenDictionary, list.AsReadOnly());
#else
         return new Lookups(lookup, list.AsReadOnly());
#endif
      }

      private record struct Lookups(
         global::System.Collections.Generic.IReadOnlyDictionary<int, global::Thinktecture.Tests.TestEnum> Lookup,
         global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> List);
   }
}
