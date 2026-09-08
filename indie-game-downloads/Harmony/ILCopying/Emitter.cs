using System;
using System.Linq;
using System.Reflection.Emit;

namespace Harmony.ILCopying;

internal static class Emitter
{
	public static string FormatArgument(object argument)
	{
		if (argument == null)
		{
			return "NULL";
		}
		Type type = argument.GetType();
		if (type == typeof(string))
		{
			return string.Concat("\"", argument, "\"");
		}
		if (type == typeof(Label))
		{
			return "Label" + ((Label)argument/*cast due to constrained. prefix*/).GetHashCode();
		}
		if (type == typeof(Label[]))
		{
			return "Labels" + string.Join(",", ((Label[])argument).Select((Label l) => l.GetHashCode().ToString()).ToArray());
		}
		if (type == typeof(LocalBuilder))
		{
			return string.Concat(((LocalBuilder)argument).LocalIndex, " (", ((LocalBuilder)argument).LocalType, ")");
		}
		return argument.ToString().Trim();
	}

	public static void MarkBlockBefore(ILGenerator il, ExceptionBlock block, out Label? label)
	{
		label = null;
		switch (block.blockType)
		{
		case ExceptionBlockType.BeginExceptionBlock:
			label = il.BeginExceptionBlock();
			break;
		case ExceptionBlockType.BeginCatchBlock:
			il.BeginCatchBlock(block.catchType);
			break;
		case ExceptionBlockType.BeginExceptFilterBlock:
			il.BeginExceptFilterBlock();
			break;
		case ExceptionBlockType.BeginFaultBlock:
			il.BeginFaultBlock();
			break;
		case ExceptionBlockType.BeginFinallyBlock:
			il.BeginFinallyBlock();
			break;
		}
	}

	public static void MarkBlockAfter(ILGenerator il, ExceptionBlock block)
	{
		if (block.blockType == ExceptionBlockType.EndExceptionBlock)
		{
			il.EndExceptionBlock();
		}
	}
}
