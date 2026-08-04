namespace Thinktecture.Runtime.Tests.TestTypes;

[Union<int, string>(ValueMemberName = "RawValue")]
public partial class RenamedValueUnion;
