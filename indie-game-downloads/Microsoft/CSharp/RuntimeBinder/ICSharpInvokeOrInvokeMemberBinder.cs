using System;

namespace Microsoft.CSharp.RuntimeBinder;

internal interface ICSharpInvokeOrInvokeMemberBinder : ICSharpBinder
{
	bool StaticCall { get; }

	bool ResultDiscarded { get; }

	CSharpCallFlags Flags { get; }

	Type[] TypeArguments { get; }
}
