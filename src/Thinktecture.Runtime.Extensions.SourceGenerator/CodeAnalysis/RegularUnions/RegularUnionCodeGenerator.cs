using System.Text;

namespace Thinktecture.CodeAnalysis.RegularUnions;

public class RegularUnionCodeGenerator : CodeGeneratorBase
{
   private readonly record struct TypeMember(
      RegularUnionTypeMemberState State,
      string ArgumentName);

   public override string CodeGeneratorName => "RegularUnion-CodeGenerator";
   public override string FileNameSuffix => ".RegularUnion";

   private readonly RegularUnionSourceGenState _state;
   private readonly StringBuilder _sb;
   private readonly IReadOnlyList<TypeMember> _typeMembers;

   public RegularUnionCodeGenerator(
      RegularUnionSourceGenState state,
      StringBuilder sb)
   {
      _state = state;
      _sb = sb;

      var typeMembers = OrderTypeMembers(state, _sb);
      typeMembers.RemoveAll(t => t.State.IsAbstract);
      typeMembers.Reverse();
      _typeMembers = typeMembers;
   }

   private List<TypeMember> OrderTypeMembers(
      RegularUnionSourceGenState state,
      StringBuilder sb)
   {
      var typeMembers = new List<TypeMember>();
      var unsortedTypeMembers = new List<RegularUnionTypeMemberState>();

      // Reversed for-loop because whole collection will be reversed again by the caller
      for (var i = _state.TypeMembers.Count - 1; i >= 0; i--)
      {
         var typeMember = _state.TypeMembers[i];

         if (typeMember.BaseTypeDefinitionFullyQualified == state.TypeDefinitionFullyQualified)
         {
            typeMembers.Add(MakeTypeMember(typeMember, sb));
         }
         else
         {
            unsortedTypeMembers.Add(typeMember);
         }
      }

      while (unsortedTypeMembers.Count > 0)
      {
         var numberOfUnsorted = unsortedTypeMembers.Count;

         for (var i = unsortedTypeMembers.Count - 1; i >= 0; i--)
         {
            var unsortedTypeMember = unsortedTypeMembers[i];

            for (var j = typeMembers.Count - 1; j >= 0; j--)
            {
               if (typeMembers[j].State.TypeDefinitionFullyQualified != unsortedTypeMember.BaseTypeDefinitionFullyQualified)
                  continue;

               typeMembers.Add(MakeTypeMember(unsortedTypeMember, sb));
               unsortedTypeMembers.RemoveAt(i);
               break;
            }
         }

         if (numberOfUnsorted == unsortedTypeMembers.Count)
            throw new InvalidOperationException($"Could not sort union type members of '{_state.TypeFullyQualified}' according their inheritance hierarchy.");
      }

      return typeMembers;
   }

   private static TypeMember MakeTypeMember(
      RegularUnionTypeMemberState typeMember,
      StringBuilder sb)
   {
      var argName = typeMember.ContainingTypes
                              .MakeFullyQualifiedArgumentName(typeMember.Name, skipRootContainingType: true, sb);

      return new TypeMember(typeMember, argName);
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.AppendLine(GENERATED_CODE_PREFIX);

      var hasNamespace = _state.Namespace is not null;

      if (hasNamespace)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.ContainingTypes);

      GenerateUnion(cancellationToken);

      _sb.RenderContainingTypesEnd(_state.ContainingTypes).Append(@"
");
   }

   private void GenerateUnion(CancellationToken cancellationToken)
   {
      _sb.Append(@"
[global::System.Diagnostics.CodeAnalysis.SuppressMessage(""ThinktectureRuntimeExtensionsAnalyzer"", ""TTRESG1000:Internal Thinktecture.Runtime.Extensions API usage"")]
abstract partial ").AppendTypeKind(_state).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(@" :
   global::Thinktecture.Internal.IMetadataOwner
{
   static global::Thinktecture.Internal.Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
      = new global::Thinktecture.Internal.Metadata.RegularUnion(typeof(").AppendTypeFullyQualified(_state).Append(@"))
      {
         TypeMembers = new global::System.Collections.Generic.List<global::System.Type>
                       {").AppendTypeMembers(_state.TypeMembers).Append(@"
                       }
                       .AsReadOnly()
      };");

      if (!_state.HasNonDefaultConstructor)
      {
         _sb.Append(@"

   private ").Append(_state.Name).Append(@"()
   {
   }");
      }

      cancellationToken.ThrowIfCancellationRequested();

      if (_state.Settings.SwitchMethods != SwitchMapMethodsGeneration.None)
      {
         GenerateSwitchForAction(false, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForAction(false, true);

         GenerateSwitchForAction(true, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForAction(true, true);

         GenerateSwitchForFunc(false, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForFunc(false, true);

         GenerateSwitchForFunc(true, false);

         if (_state.Settings.SwitchMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateSwitchForFunc(true, true);
      }

      if (_state.Settings.MapMethods != SwitchMapMethodsGeneration.None)
      {
         GenerateMap(false);

         if (_state.Settings.MapMethods == SwitchMapMethodsGeneration.DefaultWithPartialOverloads)
            GenerateMap(true);
      }

      GenerateConversionsFromValue();

      _sb.Append(@"
}");
   }

   private void GenerateConversionsFromValue()
   {
      if (_state.Settings.ConversionFromValue == ConversionOperatorsGeneration.None)
         return;

      for (var i = 0; i < _state.TypeMembers.Count; i++)
      {
         var memberType = _state.TypeMembers[i];

         if (memberType.IsInterface
             || memberType.SpecialType == SpecialType.System_Object
             || memberType.IsAbstract
             || memberType.HasRequiredMembers)
            continue;

         for (var j = 0; j < memberType.UniqueSingleArgumentConstructors.Count; j++)
         {
            var ctorArg = memberType.UniqueSingleArgumentConstructors[j];

            if (ctorArg.IsInterface || ctorArg.SpecialType == SpecialType.System_Object)
               continue;

            _sb.Append(@"

   /// <summary>
   /// ").Append(_state.Settings.ConversionFromValue == ConversionOperatorsGeneration.Implicit ? "Implicit" : "Explicit").Append(" conversion from type ").AppendTypeForXmlComment(ctorArg).Append(@".
   /// </summary>
   /// <param name=""").Append(ctorArg.ArgumentName).Append(@""">Value to covert from.</param>
   /// <returns>A new instance of ").AppendTypeForXmlComment(memberType).Append(@" converted from <paramref name=""").Append(ctorArg.ArgumentName).Append(@"""/>.</returns>
   public static ").AppendConversionOperator(_state.Settings.ConversionFromValue).Append(" operator ").AppendTypeFullyQualified(_state).Append("(").AppendTypeFullyQualified(ctorArg).Append(" ").AppendEscaped(ctorArg.ArgumentName).Append(@")
   {
      return new ").AppendTypeFullyQualified(memberType).Append("(").AppendEscaped(ctorArg.ArgumentName).Append(@");
   }");
         }
      }
   }

   private void GenerateSwitchForAction(bool withState, bool isPartially)
   {
      _sb.Append(@"

   /// <summary>
   /// Executes an action depending on the current type.
   /// </summary>");

      if (withState)
      {
         _sb.Append(@"
   /// <param name=""").Append(_state.Settings.SwitchMapStateParameterName).Append(@""">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
   /// <param name=""default"">The action to execute if no type-specific action is provided.</param>");
      }

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var typeMember = _typeMembers[i];

         _sb.Append(@"
   /// <param name=""").Append(typeMember.ArgumentName).Append(@""">The action to execute if the current type is ").AppendTypeForXmlComment(typeMember.State).Append(".</param>");
      }

      _sb.Append(@"
   [global::System.Diagnostics.DebuggerStepThroughAttribute]
   public void ").Append(isPartially ? "SwitchPartially" : "Switch");

      if (withState)
      {
         _sb.Append(@"<TState>(
      TState ").AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(",");
      }
      else
      {
         _sb.Append("(");
      }

      if (isPartially)
      {
         _sb.Append(@"
      global::System.Action<");

         if (withState)
            _sb.Append("TState, ");

         _sb.AppendTypeFullyQualified(_state).Append(">? @default = null,");
      }

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var memberType = _typeMembers[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
      global::System.Action<");

         if (withState)
            _sb.Append("TState, ");

         _sb.AppendTypeFullyQualified(memberType.State).Append(">");

         if (isPartially)
            _sb.Append('?');

         _sb.Append(' ').AppendEscaped(memberType.ArgumentName);

         if (isPartially)
            _sb.Append(" = null");
      }

      _sb.Append(")");

      if (withState)
      {
         _sb.Append(@"
#if NET9_0_OR_GREATER
		where TState : allows ref struct
#endif");
      }

      _sb.Append(@"
   {");

      GenerateIndexBasedActionSwitchBody(withState, isPartially);

      _sb.Append(@"
   }");
   }

   private void GenerateIndexBasedActionSwitchBody(bool withState, bool isPartially)
   {
      _sb.Append(@"
      switch (this)
      {");

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var typeMember = _typeMembers[i];

         _sb.Append(@"
         case ").AppendTypeFullyQualified(typeMember.State).Append(" value:");

         if (isPartially)
         {
            _sb.Append(@"
            if (").AppendEscaped(typeMember.ArgumentName).Append(@" is null)
               break;
");
         }

         _sb.Append(@"
            ").AppendEscaped(typeMember.ArgumentName).Append("(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(", ");

         _sb.Append(@"value);
            return;");
      }

      _sb.Append(@"
         default:
            throw new global::System.ArgumentOutOfRangeException($""Unexpected type '{this.GetType().FullName}'."");
      }");

      if (isPartially)
      {
         _sb.Append(@"

      @default?.Invoke(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(", ");

         _sb.Append("this);");
      }
   }

   private void GenerateSwitchForFunc(bool withState, bool isPartially)
   {
      _sb.Append(@"

   /// <summary>
   /// Executes a function depending on the current type.
   /// </summary>");

      if (withState)
      {
         _sb.Append(@"
   /// <param name=""").Append(_state.Settings.SwitchMapStateParameterName).Append(@""">State to be passed to the callbacks.</param>");
      }

      if (isPartially)
      {
         _sb.Append(@"
   /// <param name=""default"">The function to execute if no type-specific action is provided.</param>");
      }

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var typeMember = _typeMembers[i];

         _sb.Append(@"
   /// <param name=""").Append(typeMember.ArgumentName).Append(@""">The function to execute if the current type is ").AppendTypeForXmlComment(typeMember.State).Append(".</param>");
      }

      _sb.Append(@"
   [global::System.Diagnostics.DebuggerStepThroughAttribute]
   public TResult ").Append(isPartially ? "SwitchPartially" : "Switch");

      if (withState)
      {
         _sb.Append(@"<TState, TResult>(
      TState ").AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(",");
      }
      else
      {
         _sb.Append("<TResult>(");
      }

      if (isPartially)
      {
         _sb.Append(@"
      global::System.Func<");

         if (withState)
            _sb.Append("TState, ");

         _sb.AppendTypeFullyQualified(_state).Append(", TResult> @default,");
      }

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var memberType = _typeMembers[i];

         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
      global::System.Func<");

         if (withState)
            _sb.Append("TState, ");

         _sb.AppendTypeFullyQualified(memberType.State).Append(", TResult>");

         if (isPartially)
            _sb.Append('?');

         _sb.Append(' ').AppendEscaped(memberType.ArgumentName);

         if (isPartially)
            _sb.Append(" = null");
      }

      _sb.Append(@")
#if NET9_0_OR_GREATER
		where TResult : allows ref struct");

      if (withState)
      {
         _sb.Append(@"
		where TState : allows ref struct");
      }

      _sb.Append(@"
#endif
   {");

      GenerateIndexBasedFuncSwitchBody(withState, isPartially);

      _sb.Append(@"
   }");
   }

   private void GenerateIndexBasedFuncSwitchBody(bool withState, bool isPartially)
   {
      _sb.Append(@"
      switch (this)
      {");

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var typeMember = _typeMembers[i];

         _sb.Append(@"
         case ").AppendTypeFullyQualified(typeMember.State).Append(" value:");

         if (isPartially)
         {
            _sb.Append(@"
            if (").AppendEscaped(typeMember.ArgumentName).Append(@" is null)
               break;
");
         }

         _sb.Append(@"
            return ").AppendEscaped(typeMember.ArgumentName).Append("(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(", ");

         _sb.Append("value);");
      }

      _sb.Append(@"
         default:
            throw new global::System.ArgumentOutOfRangeException($""Unexpected type '{this.GetType().FullName}'."");
      }");

      if (isPartially)
      {
         _sb.Append(@"

      return @default(");

         if (withState)
            _sb.AppendEscaped(_state.Settings.SwitchMapStateParameterName).Append(", ");

         _sb.Append("this);");
      }
   }

   private void GenerateMap(bool isPartially)
   {
      _sb.Append(@"

   /// <summary>
   /// Maps current instance to an instance of type <typeparamref name=""TResult""/>.
   /// </summary>");

      if (isPartially)
      {
         _sb.Append(@"
   /// <param name=""default"">The instance to return if no value is provided for the current type.</param>");
      }

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var typeMember = _typeMembers[i];

         _sb.Append(@"
   /// <param name=""").Append(typeMember.ArgumentName).Append(@""">The instance to return if the current type is ").AppendTypeForXmlComment(typeMember.State).Append(".</param>");
      }

      _sb.Append(@"
   [global::System.Diagnostics.DebuggerStepThroughAttribute]
   public TResult ").Append(isPartially ? "MapPartially" : "Map").Append("<TResult>(");

      if (isPartially)
      {
         _sb.Append(@"
      TResult @default,");
      }

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         if (i != 0)
            _sb.Append(",");

         _sb.Append(@"
      ");

         if (isPartially)
            _sb.Append("global::Thinktecture.Argument<");

         _sb.Append("TResult");

         if (isPartially)
            _sb.Append(">");

         _sb.Append(" ").AppendEscaped(_typeMembers[i].ArgumentName);

         if (isPartially)
            _sb.Append(" = default");
      }

      _sb.Append(@")
#if NET9_0_OR_GREATER
		where TResult : allows ref struct
#endif
   {");

      GenerateIndexBasedMapSwitchBody(isPartially);

      _sb.Append(@"
   }");
   }

   private void GenerateIndexBasedMapSwitchBody(bool isPartially)
   {
      _sb.Append(@"
      switch (this)
      {");

      for (var i = 0; i < _typeMembers.Count; i++)
      {
         var typeMember = _typeMembers[i];

         _sb.Append(@"
         case ").AppendTypeFullyQualified(typeMember.State).Append(" value:");

         if (isPartially)
         {
            _sb.Append(@"
               if (!").AppendEscaped(typeMember.ArgumentName).Append(@".IsSet)
                  break;
");
         }

         _sb.Append(@"
               return ").AppendEscaped(typeMember.ArgumentName);

         if (isPartially)
            _sb.Append(".Value");

         _sb.Append(";");
      }

      _sb.Append(@"
            default:
               throw new global::System.ArgumentOutOfRangeException($""Unexpected type '{this.GetType().FullName}'."");
         }");

      if (isPartially)
      {
         _sb.Append(@"

         return @default;");
      }
   }
}

file static class Extensions
{
   public static StringBuilder AppendTypeMembers(this StringBuilder sb, IReadOnlyList<RegularUnionTypeMemberState> typeMembers)
   {
      for (var i = 0; i < typeMembers.Count; i++)
      {
         var member = typeMembers[i];

         if (i > 0)
            sb.Append(",");

         sb.Append(@"
                           typeof(").AppendTypeFullyQualified(member).Append(")");
      }

      return sb;
   }
}
