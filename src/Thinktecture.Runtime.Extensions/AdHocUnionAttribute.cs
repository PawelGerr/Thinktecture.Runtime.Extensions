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
   public bool T1IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T2"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T2Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T2"/> a nullable reference type.
   /// This setting has no effect if <see name="T2"/> is a struct.
   /// </summary>
   public bool T2IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T3"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T3Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T3"/> a nullable reference type.
   /// This setting has no effect if <see name="T3"/> is a struct.
   /// </summary>
   public bool T3IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T4"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T4Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T4"/> a nullable reference type.
   /// This setting has no effect if <see name="T4"/> is a struct.
   /// </summary>
   public bool T4IsNullableReferenceType { get; set; }

   /// <summary>
   /// Changes the name of all members regarding <see name="T5"/>.
   /// By default, the type name is used.
   /// </summary>
   public string? T5Name { get; set; }

   /// <summary>
   /// Makes the type argument <see name="T5"/> a nullable reference type.
   /// This setting has no effect if <see name="T5"/> is a struct.
   /// </summary>
   public bool T5IsNullableReferenceType { get; set; }

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
