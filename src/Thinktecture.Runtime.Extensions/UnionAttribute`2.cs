namespace Thinktecture;

/// <summary>
/// Marks a type as an ad hoc discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class UnionAttribute<T1, T2> : UnionAttributeBase
{
   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T1"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T1Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T1"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T1"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T1IsStateless"/> is <c>true</c> and T1 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T1IsNullableReferenceType
   {
      get => field || (typeof(T1).IsClass && T1IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T1"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T1)</c> from accessors like <c>AsT1</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T1IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T2IsStateless"/> is <c>true</c> and T2 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T2IsNullableReferenceType
   {
      get => field || (typeof(T2).IsClass && T2IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T2"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T2)</c> from accessors like <c>AsT2</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T2IsStateless { get; set; }
}

/// <summary>
/// Marks a type as an ad hoc discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T3">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class UnionAttribute<T1, T2, T3> : UnionAttributeBase
{
   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T1"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T1Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T1"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T1"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T1IsStateless"/> is <c>true</c> and T1 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T1IsNullableReferenceType
   {
      get => field || (typeof(T1).IsClass && T1IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T1"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T1)</c> from accessors like <c>AsT1</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T1IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T2IsStateless"/> is <c>true</c> and T2 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T2IsNullableReferenceType
   {
      get => field || (typeof(T2).IsClass && T2IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T2"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T2)</c> from accessors like <c>AsT2</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T2IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T3"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T3"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T3IsStateless"/> is <c>true</c> and T3 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T3IsNullableReferenceType
   {
      get => field || (typeof(T3).IsClass && T3IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T3"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T3)</c> from accessors like <c>AsT3</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T3IsStateless { get; set; }
}

/// <summary>
/// Marks a type as an ad hoc discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T3">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T4">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class UnionAttribute<T1, T2, T3, T4> : UnionAttributeBase
{
   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T1"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T1Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T1"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T1"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T1IsStateless"/> is <c>true</c> and T1 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T1IsNullableReferenceType
   {
      get => field || (typeof(T1).IsClass && T1IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T1"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T1)</c> from accessors like <c>AsT1</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T1IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T2IsStateless"/> is <c>true</c> and T2 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T2IsNullableReferenceType
   {
      get => field || (typeof(T2).IsClass && T2IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T2"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T2)</c> from accessors like <c>AsT2</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T2IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T3"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T3"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T3IsStateless"/> is <c>true</c> and T3 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T3IsNullableReferenceType
   {
      get => field || (typeof(T3).IsClass && T3IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T3"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T3)</c> from accessors like <c>AsT3</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T3IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T4"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T4Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T4"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T4"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T4IsStateless"/> is <c>true</c> and T4 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T4IsNullableReferenceType
   {
      get => field || (typeof(T4).IsClass && T4IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T4"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T4)</c> from accessors like <c>AsT4</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T4IsStateless { get; set; }
}

/// <summary>
/// Marks a type as an ad hoc discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T3">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T4">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T5">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class UnionAttribute<T1, T2, T3, T4, T5> : UnionAttributeBase
{
   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T1"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T1Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T1"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T1"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T1IsStateless"/> is <c>true</c> and T1 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T1IsNullableReferenceType
   {
      get => field || (typeof(T1).IsClass && T1IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T1"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T1)</c> from accessors like <c>AsT1</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T1IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T2IsStateless"/> is <c>true</c> and T2 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T2IsNullableReferenceType
   {
      get => field || (typeof(T2).IsClass && T2IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T2"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T2)</c> from accessors like <c>AsT2</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T2IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T3"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T3"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T3IsStateless"/> is <c>true</c> and T3 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T3IsNullableReferenceType
   {
      get => field || (typeof(T3).IsClass && T3IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T3"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T3)</c> from accessors like <c>AsT3</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T3IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T4"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T4Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T4"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T4"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T4IsStateless"/> is <c>true</c> and T4 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T4IsNullableReferenceType
   {
      get => field || (typeof(T4).IsClass && T4IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T4"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T4)</c> from accessors like <c>AsT4</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T4IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T5"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T5Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T5"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T5"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T5IsStateless"/> is <c>true</c> and T5 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T5IsNullableReferenceType
   {
      get => field || (typeof(T5).IsClass && T5IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <typeparamref name="T5"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T5)</c> from accessors like <c>AsT5</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T5IsStateless { get; set; }
}
