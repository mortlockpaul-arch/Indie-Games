using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony.ILCopying;

internal class MethodCopier
{
	private readonly MethodBodyReader reader;

	public MethodCopier(MethodBase fromMethod, ILGenerator toILGenerator, LocalBuilder[] existingVariables = null)
	{
		if (fromMethod == null)
		{
			throw new ArgumentNullException("Method cannot be null");
		}
		reader = new MethodBodyReader(fromMethod, toILGenerator);
		reader.DeclareVariables(existingVariables);
		reader.ReadInstructions();
	}

	public void Finalize(List<Label> endLabels, List<ExceptionBlock> endBlocks)
	{
		reader.FinalizeILCodes(endLabels, endBlocks);
	}
}
