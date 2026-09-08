using System.Collections.Generic;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ArgInfos
{
	public int carg;

	public TypeArray types;

	public List<Expr> prgexpr;
}
