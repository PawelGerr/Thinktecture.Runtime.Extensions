﻿// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::System.ComponentModel.TypeConverter(typeof(global::Thinktecture.ValueObjectTypeConverter<global::Thinktecture.Tests.TestEnum, string, global::Thinktecture.ValidationError>))]
   sealed partial class TestEnum : global::Thinktecture.IEnum<string, global::Thinktecture.Tests.TestEnum, global::Thinktecture.ValidationError>,
#if NET9_0_OR_GREATER
      global::Thinktecture.IValueObjectFactory<global::Thinktecture.Tests.TestEnum, global::System.ReadOnlySpan<char>, global::Thinktecture.ValidationError>,
#endif
      global::Thinktecture.IValidatableEnum,
      global::System.IEquatable<global::Thinktecture.Tests.TestEnum?>
   {
      [global::System.Runtime.CompilerServices.ModuleInitializer]
      internal static void ModuleInit()
      {
         var convertFromKey = new global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>(global::Thinktecture.Tests.TestEnum.Get);
         global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static @key => global::Thinktecture.Tests.TestEnum.Get(@key);

         var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Key);
         global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Key;

         var enumType = typeof(global::Thinktecture.Tests.TestEnum);
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, true, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

         global::Thinktecture.Internal.KeyedValueObjectMetadataLookup.AddMetadata(enumType, metadata);
      }

#if NET9_0_OR_GREATER
      private static readonly global::System.Lazy<(global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>, global::System.Collections.Frozen.FrozenDictionary<string, global::Thinktecture.Tests.TestEnum>.AlternateLookup<global::System.ReadOnlySpan<char>>)> _lookups
                                             = new global::System.Lazy<(global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>, global::System.Collections.Frozen.FrozenDictionary<string, global::Thinktecture.Tests.TestEnum>.AlternateLookup<global::System.ReadOnlySpan<char>>)>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> _itemsLookup => _lookups.Value.Item1;
#else
      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>> _lookups
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>>(GetLookup, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      private static global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> _itemsLookup => _lookups.Value;
#endif
      private static readonly global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>> _items
                                             = new global::System.Lazy<global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum>>(() => global::System.Linq.Enumerable.ToList(_itemsLookup.Values).AsReadOnly(), global::System.Threading.LazyThreadSafetyMode.PublicationOnly);

      /// <summary>
      /// Gets all valid items.
      /// </summary>
      public static global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> Items => _items.Value;

      /// <summary>
      /// The identifier of this item.
      /// </summary>
      public string Key { get; }

      /// <inheritdoc />
      public bool IsValid { get; }

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {
         if (!IsValid)
            throw new global::System.InvalidOperationException($"The current enumeration item of type \"TestEnum\" with identifier \"{this.Key}\" is not valid.");
      }

      private readonly int _hashCode;
      private readonly global::System.Lazy<int> _itemIndex;

      private TestEnum(
         string @key)
         : this(@key, true)
      {
      }

      private TestEnum(
         string @key,
         bool isValid)
      {
         ValidateConstructorArguments(
            ref @key,
            isValid);

         if (@key is null)
            throw new global::System.ArgumentNullException(nameof(@key));

         this.Key = @key;
         this.IsValid = isValid;
         this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::System.StringComparer.OrdinalIgnoreCase.GetHashCode(@key));
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
         ref string @key,
         bool isValid);

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
      {
         return this.Key;
      }

      /// <summary>
      /// Gets an enumeration item for provided <paramref name="key"/>.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
      public static global::Thinktecture.Tests.TestEnum? Get(string? @key)
      {
         if (@key is null)
            return default;

         if (!_itemsLookup.TryGetValue(@key, out var item))
         {
            item = CreateAndCheckInvalidItem(@key);
         }

         return item;
      }

#if NET9_0_OR_GREATER
      /// <summary>
      /// Gets an enumeration item for provided <paramref name="key"/>.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="key"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      public static global::Thinktecture.Tests.TestEnum Get(global::System.ReadOnlySpan<char> @key)
      {
         if (!_lookups.Value.Item2.TryGetValue(@key, out var item))
         {
            item = CreateAndCheckInvalidItem(@key.ToString());
         }

         return item;
      }
#endif

      private static global::Thinktecture.Tests.TestEnum CreateAndCheckInvalidItem(string @key)
      {
         var item = CreateInvalidItem(@key);

         if (item is null)
            throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must not return 'null'.");

         if (item.IsValid)
            throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");

         return item;
      }

      private static global::Thinktecture.Tests.TestEnum CreateInvalidItem(string @key)
      {
         return new global::Thinktecture.Tests.TestEnum(@key, false);
      }

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string @key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
      {
         if (@key is null)
         {
            item = default;
            return false;
         }

         if(_itemsLookup.TryGetValue(@key, out item))
            return true;

         item = CreateAndCheckInvalidItem(@key);
         return false;
      }

#if NET9_0_OR_GREATER
      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name="key"/> if a valid item exists.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name="key"/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet(global::System.ReadOnlySpan<char> @key, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
      {
         if(_lookups.Value.Item2.TryGetValue(@key, out item))
            return true;

         item = CreateAndCheckInvalidItem(@key.ToString());
         return false;
      }
#endif

      /// <summary>
      /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <param name="provider">An object that provides culture-specific formatting information.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
      public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string @key, global::System.IFormatProvider? @provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
      {
         if(global::Thinktecture.Tests.TestEnum.TryGet(@key, out item))
         {
            return null;
         }
         else
         {
            if(@key is not null)
               item = CreateAndCheckInvalidItem(@key);
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{@key}'.");
         }
      }

#if NET9_0_OR_GREATER
      /// <summary>
      /// Validates the provided <paramref name="key"/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name="key">The identifier to return an enumeration item for.</param>
      /// <param name="provider">An object that provides culture-specific formatting information.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name="key"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
      public static global::Thinktecture.ValidationError? Validate(global::System.ReadOnlySpan<char> @key, global::System.IFormatProvider? @provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
      {
         if(global::Thinktecture.Tests.TestEnum.TryGet(@key, out item))
         {
            return null;
         }
         else
         {
            item = CreateAndCheckInvalidItem(@key.ToString());
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{@key}'.");
         }
      }
#endif

      /// <summary>
      /// Implicit conversion to the type <see cref="string"/>.
      /// </summary>
      /// <param name="item">Item to covert.</param>
      /// <returns>The <see cref="TestEnum.Key"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
      public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
      {
         return item is null ? default : item.Key;
      }

      /// <summary>
      /// Explicit conversion from the type <see cref="string"/>.
      /// </summary>
      /// <param name="key">Value to covert.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="key"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("key")]
      public static explicit operator global::Thinktecture.Tests.TestEnum?(string? @key)
      {
         return global::Thinktecture.Tests.TestEnum.Get(@key);
      }

      /// <inheritdoc />
      public bool Equals(global::Thinktecture.Tests.TestEnum? other)
      {
         if (other is null)
            return false;

         if (!global::System.Object.ReferenceEquals(GetType(), other.GetType()))
            return false;

         if (global::System.Object.ReferenceEquals(this, other))
            return true;

         if (this.IsValid != other.IsValid)
            return false;

         return global::System.StringComparer.OrdinalIgnoreCase.Equals(this.Key, other.Key);
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
      /// <param name="invalid">The action to execute if the current item is an invalid item.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void Switch(
         global::System.Action<global::Thinktecture.Tests.TestEnum> invalid,
         global::System.Action @item1,
         global::System.Action @item2)
      {
         if (!this.IsValid)
         {
            invalid(this);
            return;
         }

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
      /// <param name="invalid">The action to execute if the current item is an invalid item.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void SwitchPartially(
         global::System.Action<global::Thinktecture.Tests.TestEnum>? @default = null,
         global::System.Action<global::Thinktecture.Tests.TestEnum>? invalid = null,
         global::System.Action? @item1 = null,
         global::System.Action? @item2 = null)
      {
         if (!this.IsValid)
         {
            if(invalid is null)
            {
               @default?.Invoke(this);
               return;
            }

            invalid(this);
            return;
         }

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
      /// <param name="invalid">The action to execute if the current item is an invalid item.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void Switch<TState>(
         TState state,
         global::System.Action<TState, global::Thinktecture.Tests.TestEnum> invalid,
         global::System.Action<TState> @item1,
         global::System.Action<TState> @item2)
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif
      {
         if (!this.IsValid)
         {
            invalid(state, this);
            return;
         }

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
      /// <param name="invalid">The action to execute if the current item is an invalid item.</param>
      /// <param name="item1">The action to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The action to execute if the current item is equal to <see cref="Item2"/>.</param>
      public void SwitchPartially<TState>(
         TState state,
         global::System.Action<TState, global::Thinktecture.Tests.TestEnum>? @default = null,
         global::System.Action<TState, global::Thinktecture.Tests.TestEnum>? invalid = null,
         global::System.Action<TState>? @item1 = null,
         global::System.Action<TState>? @item2 = null)
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif
      {
         if (!this.IsValid)
         {
            if(invalid is null)
            {
               @default?.Invoke(state, this);
               return;
            }

            invalid(state, this);
            return;
         }

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
      /// <param name="invalid">The function to execute if the current item is an invalid item.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult Switch<TResult>(
         global::System.Func<global::Thinktecture.Tests.TestEnum, TResult> invalid,
         global::System.Func<TResult> @item1,
         global::System.Func<TResult> @item2)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         if (!this.IsValid)
         {
            return invalid(this);
         }

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
      /// <param name="invalid">The function to execute if the current item is an invalid item.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult SwitchPartially<TResult>(
         global::System.Func<global::Thinktecture.Tests.TestEnum, TResult> @default,
         global::System.Func<global::Thinktecture.Tests.TestEnum, TResult>? invalid = null,
         global::System.Func<TResult>? @item1 = null,
         global::System.Func<TResult>? @item2 = null)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         if (!this.IsValid)
         {
            if(invalid is null)
               return @default(this);

            return invalid(this);
         }

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
      /// <param name="invalid">The function to execute if the current item is an invalid item.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult Switch<TState, TResult>(
         TState state,
         global::System.Func<TState, global::Thinktecture.Tests.TestEnum, TResult> invalid,
         global::System.Func<TState, TResult> @item1,
         global::System.Func<TState, TResult> @item2)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
		   where TState : allows ref struct
#endif
      {
         if (!this.IsValid)
         {
            return invalid(state, this);
         }

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
      /// <param name="invalid">The function to execute if the current item is an invalid item.</param>
      /// <param name="item1">The function to execute if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The function to execute if the current item is equal to <see cref="Item2"/>.</param>
      public TResult SwitchPartially<TState, TResult>(
         TState state,
         global::System.Func<TState, global::Thinktecture.Tests.TestEnum, TResult> @default,
         global::System.Func<TState, global::Thinktecture.Tests.TestEnum, TResult>? invalid = null,
         global::System.Func<TState, TResult>? @item1 = null,
         global::System.Func<TState, TResult>? @item2 = null)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
		   where TState : allows ref struct
#endif
      {
         if (!this.IsValid)
         {
            if(invalid is null)
               return @default(state, this);

            return invalid(state, this);
         }

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
      /// <param name="invalid">The instance to return if the current item is an invalid item.</param>
      /// <param name="item1">The instance to return if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The instance to return if the current item is equal to <see cref="Item2"/>.</param>
      public TResult Map<TResult>(
         TResult invalid,
         TResult @item1,
         TResult @item2)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         if (!this.IsValid)
            return invalid;

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
      /// <param name="invalid">The instance to return if the current item is an invalid item.</param>
      /// <param name="item1">The instance to return if the current item is equal to <see cref="Item1"/>.</param>
      /// <param name="item2">The instance to return if the current item is equal to <see cref="Item2"/>.</param>
      public TResult MapPartially<TResult>(
         TResult @default,
         global::Thinktecture.Argument<TResult> invalid = default,
         global::Thinktecture.Argument<TResult> @item1 = default,
         global::Thinktecture.Argument<TResult> @item2 = default)
#if NET9_0_OR_GREATER
		   where TResult : allows ref struct
#endif
      {
         if (!this.IsValid)
            return invalid.IsSet ? invalid.Value : @default;

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

      private static
#if NET9_0_OR_GREATER
         (global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>, global::System.Collections.Frozen.FrozenDictionary<string, global::Thinktecture.Tests.TestEnum>.AlternateLookup<global::System.ReadOnlySpan<char>>)
#else
         global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum>
#endif
          GetLookup()
      {
         var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::System.StringComparer.OrdinalIgnoreCase);

         void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
         {
            if (item is null)
               throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

            if (item.Key is null)
               throw new global::System.ArgumentException($"The \"Key\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

            if (!item.IsValid)
               throw new global::System.ArgumentException($"All \"public static readonly\" fields of type \"TestEnum\" must be valid but the item \"{itemName}\" with the identifier \"{item.Key}\" is not.");

            if (lookup.ContainsKey(item.Key))
               throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Key}\".");

            lookup.Add(item.Key, item);
         }

         AddItem(@Item1, nameof(@Item1));
         AddItem(@Item2, nameof(@Item2));

#if NET8_0_OR_GREATER
         var frozenDictionary = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::System.StringComparer.OrdinalIgnoreCase);
#if NET9_0_OR_GREATER
         return (frozenDictionary, frozenDictionary.GetAlternateLookup<global::System.ReadOnlySpan<char>>());
#else
         return frozenDictionary;
#endif
#else
         return lookup;
#endif
      }
   }
}
