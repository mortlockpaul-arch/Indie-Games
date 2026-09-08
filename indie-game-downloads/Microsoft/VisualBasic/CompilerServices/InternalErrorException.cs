using System;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class InternalErrorException : Exception
{
	public InternalErrorException()
		: base(System.SR.InternalError_VisualBasicRuntime)
	{
	}
}
