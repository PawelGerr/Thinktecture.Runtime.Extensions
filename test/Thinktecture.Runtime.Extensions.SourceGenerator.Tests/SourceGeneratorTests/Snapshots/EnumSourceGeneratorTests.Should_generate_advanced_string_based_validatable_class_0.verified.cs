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
         global::System.Linq.Expressions.Expression<global::System.Func<string?, global::Thinktecture.Tests.TestEnum?>> convertFromKeyExpression = static @name => global::Thinktecture.Tests.TestEnum.Get(@name);

         var convertToKey = new global::System.Func<global::Thinktecture.Tests.TestEnum, string>(static item => item.Name);
         global::System.Linq.Expressions.Expression<global::System.Func<global::Thinktecture.Tests.TestEnum, string>> convertToKeyExpression = static item => item.Name;

         var enumType = typeof(global::Thinktecture.Tests.TestEnum);
         var metadata = new global::Thinktecture.Internal.KeyedValueObjectMetadata(enumType, typeof(string), true, true, convertFromKey, convertFromKeyExpression, null, convertToKey, convertToKeyExpression);

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
      public string Name { get; }

      /// <inheritdoc />
      public bool IsValid { get; }

      /// <summary>
      /// Checks whether current enumeration item is valid.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">The enumeration item is not valid.</exception>
      public void EnsureValid()
      {
         if (!IsValid)
            throw new global::System.InvalidOperationException($"The current enumeration item of type \"TestEnum\" with identifier \"{this.Name}\" is not valid.");
      }

      private readonly int _hashCode;
      private readonly global::System.Lazy<int> _itemIndex;

      private TestEnum(
         string @name,
         int @structProperty,
         int? @nullableStructProperty,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structField,
         string @referenceField)
         : this(@name, true, @structProperty, @nullableStructProperty, @referenceProperty, @nullableReferenceProperty, @structField, @referenceField)
      {
      }

      private TestEnum(
         string @name,
         bool isValid,
         int @structProperty,
         int? @nullableStructProperty,
         string @referenceProperty,
         string? @nullableReferenceProperty,
         int @structField,
         string @referenceField)
      {
         ValidateConstructorArguments(
            ref @name,
            isValid,
            ref @structProperty,
            ref @nullableStructProperty,
            ref @referenceProperty,
            ref @nullableReferenceProperty,
            ref @structField,
            ref @referenceField);

         if (@name is null)
            throw new global::System.ArgumentNullException(nameof(@name));

         this.Name = @name;
         this.IsValid = isValid;
         this.StructProperty = @structProperty;
         this.NullableStructProperty = @nullableStructProperty;
         this.ReferenceProperty = @referenceProperty;
         this.NullableReferenceProperty = @nullableReferenceProperty;
         this.StructField = @structField;
         this.ReferenceField = @referenceField;
         this._hashCode = global::System.HashCode.Combine(typeof(global::Thinktecture.Tests.TestEnum), global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.GetHashCode(@name));
         this._itemIndex = new global::System.Lazy<int>(() =>
                                                        {
                                                           for (var i = 0; i < Items.Count; i++)
                                                           {
                                                              if (this == Items[i])
                                                                 return i;
                                                           }

                                                           throw new global::System.Exception($"Current item '{@name}' not found in 'Items'.");
                                                        }, global::System.Threading.LazyThreadSafetyMode.PublicationOnly);
      }

      static partial void ValidateConstructorArguments(
         ref string @name,
         bool isValid,
         ref int @structProperty,
         ref int? @nullableStructProperty,
         ref string @referenceProperty,
         ref string? @nullableReferenceProperty,
         ref int @structField,
         ref string @referenceField);

      /// <summary>
      /// Gets the identifier of the item.
      /// </summary>
      [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
      string global::Thinktecture.IValueObjectConvertable<string>.ToValue()
      {
         return this.Name;
      }

      /// <summary>
      /// Gets an enumeration item for provided <paramref name="name"/>.
      /// </summary>
      /// <param name="name">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="name"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("name")]
      public static global::Thinktecture.Tests.TestEnum? Get(string? @name)
      {
         if (@name is null)
            return default;

         if (!_lookups.Value.Lookup.TryGetValue(@name, out var item))
         {
            item = CreateAndCheckInvalidItem(@name);
         }

         return item;
      }

#if NET9_0_OR_GREATER
      /// <summary>
      /// Gets an enumeration item for provided <paramref name="name"/>.
      /// </summary>
      /// <param name="name">The identifier to return an enumeration item for.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if <paramref name="name"/> is not <c>null</c>; otherwise <c>null</c>.</returns>
      public static global::Thinktecture.Tests.TestEnum Get(global::System.ReadOnlySpan<char> @name)
      {
         if (!_lookups.Value.AlternateLookup.TryGetValue(@name, out var item))
         {
            item = CreateAndCheckInvalidItem(@name.ToString());
         }

         return item;
      }
#endif

      private static global::Thinktecture.Tests.TestEnum CreateAndCheckInvalidItem(string @name)
      {
         var item = CreateInvalidItem(@name);

         if (item is null)
            throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must not return 'null'.");

         if (item.IsValid)
            throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must return an instance with property 'IsValid' equals to 'false'.");

         if (_lookups.Value.Lookup.ContainsKey(item.Name))
            throw new global::System.Exception("The implementation of method 'CreateInvalidItem' must not return an instance with property 'Name' equals to one of a valid item.");

         return item;
      }

      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name="name"/> if a valid item exists.
      /// </summary>
      /// <param name="name">The identifier to return an enumeration item for.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name="name"/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet([global::System.Diagnostics.CodeAnalysis.AllowNull] string @name, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
      {
         if (@name is null)
         {
            item = default;
            return false;
         }

         if(_lookups.Value.Lookup.TryGetValue(@name, out item))
            return true;

         item = CreateAndCheckInvalidItem(@name);
         return false;
      }

#if NET9_0_OR_GREATER
      /// <summary>
      /// Gets a valid enumeration item for provided <paramref name="name"/> if a valid item exists.
      /// </summary>
      /// <param name="name">The identifier to return an enumeration item for.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>true</c> if a valid item with provided <paramref name="name"/> exists; <c>false</c> otherwise.</returns>
      public static bool TryGet(global::System.ReadOnlySpan<char> @name, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out global::Thinktecture.Tests.TestEnum item)
      {
         if(_lookups.Value.AlternateLookup.TryGetValue(@name, out item))
            return true;

         item = CreateAndCheckInvalidItem(@name.ToString());
         return false;
      }
#endif

      /// <summary>
      /// Validates the provided <paramref name="name"/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name="name">The identifier to return an enumeration item for.</param>
      /// <param name="provider">An object that provides culture-specific formatting information.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name="name"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
      public static global::Thinktecture.ValidationError? Validate([global::System.Diagnostics.CodeAnalysis.AllowNull] string @name, global::System.IFormatProvider? @provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
      {
         if(global::Thinktecture.Tests.TestEnum.TryGet(@name, out item))
         {
            return null;
         }
         else
         {
            if(@name is not null)
               item = CreateAndCheckInvalidItem(@name);
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{@name}'.");
         }
      }

#if NET9_0_OR_GREATER
      /// <summary>
      /// Validates the provided <paramref name="name"/> and returns a valid enumeration item if found.
      /// </summary>
      /// <param name="name">The identifier to return an enumeration item for.</param>
      /// <param name="provider">An object that provides culture-specific formatting information.</param>
      /// <param name="item">An instance of <see cref="TestEnum"/>.</param>
      /// <returns><c>null</c> if a valid item with provided <paramref name="name"/> exists; <see cref="global::Thinktecture.ValidationError"/> with an error message otherwise.</returns>
      public static global::Thinktecture.ValidationError? Validate(global::System.ReadOnlySpan<char> @name, global::System.IFormatProvider? @provider, [global::System.Diagnostics.CodeAnalysis.MaybeNull] out global::Thinktecture.Tests.TestEnum item)
      {
         if(global::Thinktecture.Tests.TestEnum.TryGet(@name, out item))
         {
            return null;
         }
         else
         {
            item = CreateAndCheckInvalidItem(@name.ToString());
            return global::Thinktecture.Internal.ValidationErrorCreator.CreateValidationError<global::Thinktecture.ValidationError>($"There is no item of type 'TestEnum' with the identifier '{@name}'.");
         }
      }
#endif

      /// <summary>
      /// Implicit conversion to the type <see cref="string"/>.
      /// </summary>
      /// <param name="item">Item to covert.</param>
      /// <returns>The <see cref="TestEnum.Name"/> of provided <paramref name="item"/> or <c>default</c> if <paramref name="item"/> is <c>null</c>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
      public static implicit operator string?(global::Thinktecture.Tests.TestEnum? item)
      {
         return item is null ? default : item.Name;
      }

      /// <summary>
      /// Explicit conversion from the type <see cref="string"/>.
      /// </summary>
      /// <param name="name">Value to covert.</param>
      /// <returns>An instance of <see cref="TestEnum"/> if the <paramref name="name"/> is a known item or implements <see cref="Thinktecture.IValidatableEnum"/>.</returns>
      [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("name")]
      public static explicit operator global::Thinktecture.Tests.TestEnum?(string? @name)
      {
         return global::Thinktecture.Tests.TestEnum.Get(@name);
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

         return global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer.Equals(this.Name, other.Name);
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
         return this.Name.ToString();
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

      private static Lookups GetLookups()
      {
         var lookup = new global::System.Collections.Generic.Dictionary<string, global::Thinktecture.Tests.TestEnum>(2, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer);
         var list = new global::System.Collections.Generic.List<global::Thinktecture.Tests.TestEnum>(2);

         void AddItem(global::Thinktecture.Tests.TestEnum item, string itemName)
         {
            if (item is null)
               throw new global::System.ArgumentNullException($"The item \"{itemName}\" of type \"TestEnum\" must not be null.");

            if (item.Name is null)
               throw new global::System.ArgumentException($"The \"Name\" of the item \"{itemName}\" of type \"TestEnum\" must not be null.");

            if (!item.IsValid)
               throw new global::System.ArgumentException($"All \"public static readonly\" fields of type \"TestEnum\" must be valid but the item \"{itemName}\" with the identifier \"{item.Name}\" is not.");

            if (lookup.ContainsKey(item.Name))
               throw new global::System.ArgumentException($"The type \"TestEnum\" has multiple items with the identifier \"{item.Name}\".");

            lookup.Add(item.Name, item);
            list.Add(item);
         }

         AddItem(@Item1, nameof(@Item1));
         AddItem(@Item2, nameof(@Item2));

#if NET8_0_OR_GREATER
         var frozenDictionary = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(lookup, global::Thinktecture.ComparerAccessors.StringOrdinal.EqualityComparer);
#if NET9_0_OR_GREATER
         return new Lookups(frozenDictionary, frozenDictionary.GetAlternateLookup<global::System.ReadOnlySpan<char>>(), list.AsReadOnly());
#else
         return new Lookups(frozenDictionary, list.AsReadOnly());
#endif
#else
         return new Lookups(lookup, list.AsReadOnly());
#endif
      }

      private record struct Lookups(
         global::System.Collections.Generic.IReadOnlyDictionary<string, global::Thinktecture.Tests.TestEnum> Lookup,
#if NET9_0_OR_GREATER
         global::System.Collections.Frozen.FrozenDictionary<string, global::Thinktecture.Tests.TestEnum>.AlternateLookup<global::System.ReadOnlySpan<char>> AlternateLookup,
#endif
         global::System.Collections.Generic.IReadOnlyList<global::Thinktecture.Tests.TestEnum> List);
   }
}
