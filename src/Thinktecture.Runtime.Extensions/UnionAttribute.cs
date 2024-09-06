namespace Thinktecture;

/// <summary>
/// Marks a type as a discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public class UnionAttribute<T1, T2> : UnionAttributeBase
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
   public bool T1IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   public bool T2IsNullableReferenceType { get; set; }
}

/// <summary>
/// Marks a type as a discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T3">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class UnionAttribute<T1, T2, T3> : UnionAttributeBase
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
   public bool T1IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   public bool T2IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T3"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T3"/> is a struct.
   /// </summary>
   public bool T3IsNullableReferenceType { get; set; }
}

/// <summary>
/// Marks a type as a discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T3">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T4">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class UnionAttribute<T1, T2, T3, T4> : UnionAttributeBase
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
   public bool T1IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   public bool T2IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T3"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T3"/> is a struct.
   /// </summary>
   public bool T3IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T4"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T4Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T4"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T4"/> is a struct.
   /// </summary>
   public bool T4IsNullableReferenceType { get; set; }
}

/// <summary>
/// Marks a type as a discriminated union.
/// </summary>
/// <typeparam name="T1">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T2">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T3">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T4">One of the types of the discriminated union.</typeparam>
/// <typeparam name="T5">One of the types of the discriminated union.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class UnionAttribute<T1, T2, T3, T4, T5> : UnionAttributeBase
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
   public bool T1IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T2"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T2"/> is a struct.
   /// </summary>
   public bool T2IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T3"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T3"/> is a struct.
   /// </summary>
   public bool T3IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T4"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T4Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T4"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T4"/> is a struct.
   /// </summary>
   public bool T4IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <typeparamref name="T5"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T5Name { get; set; }

   /// <summary>
   /// Makes the type argument <typeparamref name="T5"/> a nullable reference type.
   /// This setting has no effect if <typeparamref name="T5"/> is a struct.
   /// </summary>
   public bool T5IsNullableReferenceType { get; set; }
}
