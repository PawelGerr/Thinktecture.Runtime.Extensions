using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectProtoBufCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly IReadOnlyList<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Complex-ValueObject-ProtoBuf-CodeGenerator";
   public override string FileNameSuffix => ".ProtoBuf";

   public ComplexValueObjectProtoBufCodeGenerator(
      ITypeInformation type,
      IReadOnlyList<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
      StringBuilder stringBuilder)
   {
      _type = type;
      _assignableInstanceFieldsAndProperties = assignableInstanceFieldsAndProperties;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_type.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_type.ContainingTypes);

      _sb.Append(@"
[global::ProtoBuf.ProtoContract(Serializer = typeof(ValueObjectProtoBufSerializer))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
   public sealed class ValueObjectProtoBufSerializer : global::ProtoBuf.Serializers.ISerializer<").AppendTypeFullyQualified(_type).Append(@">
   {
      private const global::ProtoBuf.Serializers.SerializerFeatures _WIRE_TYPE_MASK = (global::ProtoBuf.Serializers.SerializerFeatures) 15;

      public global::ProtoBuf.Serializers.SerializerFeatures Features => global::ProtoBuf.Serializers.SerializerFeatures.WireTypeStartGroup | global::ProtoBuf.Serializers.SerializerFeatures.CategoryMessage;
");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
      private global::ProtoBuf.Serializers.ISerializer<").AppendTypeFullyQualified(memberInfo).Append(">? _").Append(memberInfo.ArgumentName).Append("Serializer;");
      }

      _sb.Append(@"

      /// <inheritdoc />
      public ").AppendTypeFullyQualified(_type).Append(@" Read(ref global::ProtoBuf.ProtoReader.State state, ").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@" value)
      {");

      InitSerializers();

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         ").AppendTypeFullyQualifiedNullAnnotated(memberInfo).Append(" ").AppendEscaped(memberInfo.ArgumentName).Append(" = default;");
      }

      _sb.Append(@"

         int fieldNumber;

         while ((fieldNumber = state.ReadFieldHeader()) > 0)
         {
            switch (fieldNumber)
            {");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         // TODO: Use protomember attribute number
         _sb.Append(@"
               case ").Append(i + 1).Append(@":
               {
                  if ((_").Append(memberInfo.ArgumentName).Append(@"Serializer.Features & global::ProtoBuf.Serializers.SerializerFeatures.CategoryMessage) == global::ProtoBuf.Serializers.SerializerFeatures.CategoryMessage)
                  {
                     ").AppendEscaped(memberInfo.ArgumentName).Append(" = state.ReadMessage(_").Append(memberInfo.ArgumentName).Append("Serializer.Features, default!, _").Append(memberInfo.ArgumentName).Append(@"Serializer);
                  }
                  else
                  {
                     ").AppendEscaped(memberInfo.ArgumentName).Append(" = _").Append(memberInfo.ArgumentName).Append(@"Serializer.Read(ref state, default!);
                  }
                  break;
               }");
      }

      _sb.Append(@"
               default:
               {
                  throw new global::ProtoBuf.ProtoException($""Unexpected field number {fieldNumber} during deserialization of \""").Append(_type.Name).Append(@"\""."");
               }
            }
         }

         var validationError = ").AppendTypeFullyQualified(_type).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                    ").AppendEscaped(memberInfo.ArgumentName).Append("!,");
      }

      _sb.Append(@"
                                    out var obj);

         if (validationError is not null)
            throw new global::ProtoBuf.ProtoException(validationError.ToString() ?? ""Unable to deserialize \""").Append(_type.Name).Append(@"\""."");

         return obj!;
      }

      /// <inheritdoc />
      public void Write(ref global::ProtoBuf.ProtoWriter.State state, ").AppendTypeFullyQualified(_type).Append(@" value)
      {");

      InitSerializers();

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         // TODO: Use protomember attribute number
         _sb.Append(@"
         state.WriteFieldHeader(").Append(i + 1).Append(", (global::ProtoBuf.WireType) (_").Append(memberInfo.ArgumentName).Append(@"Serializer.Features & _WIRE_TYPE_MASK));

         if ((_").Append(memberInfo.ArgumentName).Append(@"Serializer.Features & global::ProtoBuf.Serializers.SerializerFeatures.CategoryMessage) == global::ProtoBuf.Serializers.SerializerFeatures.CategoryMessage)
         {
            state.WriteMessage(_").Append(memberInfo.ArgumentName).Append("Serializer.Features, value.").Append(memberInfo.Name).Append(@");
         }
         else
         {
            _").Append(memberInfo.ArgumentName).Append("Serializer.Write(ref state, value.").Append(memberInfo.Name).Append(@");
         }
");
      }

      _sb.Append(@"
      }
   }
}");

      _sb.RenderContainingTypesEnd(_type.ContainingTypes)
         .Append(@"
");
   }

   private void InitSerializers()
   {
      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         _").Append(memberInfo.ArgumentName).Append("Serializer ??= state.GetSerializer<").AppendTypeFullyQualified(memberInfo).Append(">();");
      }

      _sb.Append(@"
");
   }
}
