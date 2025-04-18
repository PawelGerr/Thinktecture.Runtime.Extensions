using System;
using System.Threading;
using MessagePack;
using MessagePack.Resolvers;
using Thinktecture;
using Thinktecture.SmartEnums;
using Thinktecture.Unions;
using Thinktecture.ValueObjects;

var groceries = DoRoundTripSerializationOfTypesWithoutFormatter(ProductType.Groceries);
Console.WriteLine(groceries);

var groceriesWithFormatter = DoRoundTripSerialization(ProductTypeWithMessagePackFormatter.Groceries);
Console.WriteLine(groceriesWithFormatter);

var productName = DoRoundTripSerializationOfTypesWithoutFormatter(ProductName.Create("My product"));
Console.WriteLine(productName);

var productNameWithFormatter = DoRoundTripSerialization(ProductNameWithMessagePackFormatter.Create("My product"));
Console.WriteLine(productNameWithFormatter);

// The "Boundary" is complex Value Object and requires the nuget package "Thinktecture.Runtime.Extensions.MessagePack".

var boundaryWithFormatter = DoRoundTripSerialization(BoundaryWithMessagePackFormatter.Create(1, 2));
Console.WriteLine(boundaryWithFormatter);

// Ad hoc union
var textOrNumber = DoRoundTripSerializationOfTypesWithoutFormatter((TextOrNumberSerializable)42);
Console.WriteLine(textOrNumber);

var textOrNumberWithFormatter = DoRoundTripSerialization((TextOrNumberSerializableWithFormatter)42);
Console.WriteLine(textOrNumberWithFormatter);

// Smart Enums and Value Objects without [MessagePackFormatterAttribute] need "ThinktectureMessageFormatterResolver".
static T DoRoundTripSerializationOfTypesWithoutFormatter<T>(T obj)
{
   var resolver = CompositeResolver.Create(ThinktectureMessageFormatterResolver.Instance, StandardResolver.Instance);
   var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

   var bytes = MessagePackSerializer.Serialize(obj, options, CancellationToken.None);
   var deserializedObj = MessagePackSerializer.Deserialize<T>(bytes, options, CancellationToken.None);

   return deserializedObj;
}

// Smart Enums and Value Objects with "MessagePackFormatterAttribute" (implemented by MessagePackValueObjectSourceGenerator) don't need custom options.
static T DoRoundTripSerialization<T>(T obj)
{
   var options = MessagePackSerializerOptions.Standard;

   var bytes = MessagePackSerializer.Serialize(obj, MessagePackSerializerOptions.Standard, CancellationToken.None);
   var deserializedObj = MessagePackSerializer.Deserialize<T>(bytes, options, CancellationToken.None);

   return deserializedObj;
}
