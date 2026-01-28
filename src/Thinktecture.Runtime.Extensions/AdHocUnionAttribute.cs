namespace Thinktecture;

/// <summary>
/// Marks a type as an ad hoc discriminated union.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class AdHocUnionAttribute : UnionAttributeBase
{
   /// <summary>
   /// One of the types of the discriminated union.
   /// </summary>
   public Type T1 { get; }

   /// <summary>
   /// One of the types of the discriminated union.
   /// </summary>
   public Type T2 { get; }

   /// <summary>
   /// One of the types of the discriminated union.
   /// </summary>
   public Type? T3 { get; }

   /// <summary>
   /// One of the types of the discriminated union.
   /// </summary>
   public Type? T4 { get; }

   /// <summary>
   /// One of the types of the discriminated union.
   /// </summary>
   public Type? T5 { get; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T1"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T1Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T1"/> a nullable reference type.
   /// This setting has no effect if <see name="T1"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T1IsStateless"/> is <c>true</c> and T1 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T1IsNullableReferenceType
   {
      get => field || (T1.IsClass && T1IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <see name="T1"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T1)</c> from accessors like <c>AsT1</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T1IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T2"/> a nullable reference type.
   /// This setting has no effect if <see name="T2"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T2IsStateless"/> is <c>true</c> and T2 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T2IsNullableReferenceType
   {
      get => field || (T2.IsClass && T2IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <see name="T2"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T2)</c> from accessors like <c>AsT2</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T2IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T3"/> a nullable reference type.
   /// This setting has no effect if <see name="T3"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T3IsStateless"/> is <c>true</c> and T3 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T3IsNullableReferenceType
   {
      get => field || ((T3?.IsClass ?? false) && T3IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <see name="T3"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T3)</c> from accessors like <c>AsT3</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T3IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T4"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T4Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T4"/> a nullable reference type.
   /// This setting has no effect if <see name="T4"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T4IsStateless"/> is <c>true</c> and T4 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T4IsNullableReferenceType
   {
      get => field || ((T4?.IsClass ?? false) && T4IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <see name="T4"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T4)</c> from accessors like <c>AsT4</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T4IsStateless { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T5"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T5Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T5"/> a nullable reference type.
   /// This setting has no effect if <see name="T5"/> is a struct.
   /// </summary>
   /// <remarks>
   /// Returns <c>true</c> when explicitly set to <c>true</c> or when <see cref="T5IsStateless"/> is <c>true</c> and T5 is a reference type (since <c>default(T)</c> for reference types equals <c>null</c>).
   /// </remarks>
   public bool T5IsNullableReferenceType
   {
      get => field || ((T5?.IsClass ?? false) && T5IsStateless);
      set;
   }

   /// <summary>
   /// Indicates that <see name="T5"/> is a stateless type that carries no meaningful instance data.
   /// Stateless types reduce memory footprint by storing only the discriminator index rather than type data.
   /// The generated code will return <c>default(T5)</c> from accessors like <c>AsT5</c> and <c>Value</c>.
   /// It is recommended to use struct types for stateless members to avoid null-handling complexity.
   /// </summary>
   public bool T5IsStateless { get; set; }

   /// <summary>
   /// Initializes a new instance of <see cref="AdHocUnionAttribute"/>.
   /// </summary>
   /// <param name="t1">One of the types of the discriminated union.</param>
   /// <param name="t2">One of the types of the discriminated union.</param>
   /// <param name="t3">One of the types of the discriminated union.</param>
   /// <param name="t4">One of the types of the discriminated union.</param>
   /// <param name="t5">One of the types of the discriminated union.</param>
   public AdHocUnionAttribute(
      Type t1,
      Type t2,
      Type? t3 = null,
      Type? t4 = null,
      Type? t5 = null)
   {
      T1 = t1;
      T2 = t2;
      T3 = t3;
      T4 = t4;
      T5 = t5;
   }
}
