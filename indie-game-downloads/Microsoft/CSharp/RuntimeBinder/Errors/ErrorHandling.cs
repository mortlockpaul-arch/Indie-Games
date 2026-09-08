using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal static class ErrorHandling
{
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static RuntimeBinderException Error(ErrorCode id, params ErrArg[] args)
	{
		string[] array = new string[args.Length];
		int[] array2 = new int[args.Length];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		UserStringBuilder userStringBuilder = default(UserStringBuilder);
		for (int i = 0; i < args.Length; i++)
		{
			ErrArg errArg = args[i];
			if ((errArg.eaf & ErrArgFlags.NoStr) == 0)
			{
				if (!userStringBuilder.ErrArgToString(out array[num], errArg, out var fUserStrings) && errArg.eak == ErrArgKind.Int)
				{
					array[num] = errArg.n.ToString(CultureInfo.InvariantCulture);
				}
				num++;
				int num4;
				if (!fUserStrings || (errArg.eaf & ErrArgFlags.Unique) == 0)
				{
					num4 = -1;
				}
				else
				{
					num4 = i;
					num3++;
				}
				array2[num2] = num4;
				num2++;
			}
		}
		int num5 = num;
		if (num3 > 1)
		{
			string[] array3 = new string[num5];
			Array.Copy(array, array3, num5);
			for (int j = 0; j < num5; j++)
			{
				if (array2[j] < 0 || array3[j] != array[j])
				{
					continue;
				}
				ErrArg errArg2 = args[array2[j]];
				Symbol symbol = null;
				CType cType = null;
				switch (errArg2.eak)
				{
				case ErrArgKind.Sym:
					symbol = errArg2.sym;
					break;
				case ErrArgKind.Type:
					cType = errArg2.pType;
					break;
				case ErrArgKind.SymWithType:
					symbol = errArg2.swtMemo.sym;
					break;
				case ErrArgKind.MethWithInst:
					symbol = errArg2.mpwiMemo.sym;
					break;
				default:
					continue;
				}
				bool flag = false;
				for (int k = j + 1; k < num5; k++)
				{
					if (array2[k] < 0 || array[j] != array[k])
					{
						continue;
					}
					if (array3[k] != array[k])
					{
						flag = true;
						continue;
					}
					ErrArg errArg3 = args[array2[k]];
					Symbol symbol2 = null;
					CType cType2 = null;
					switch (errArg3.eak)
					{
					case ErrArgKind.Sym:
						symbol2 = errArg3.sym;
						break;
					case ErrArgKind.Type:
						cType2 = errArg3.pType;
						break;
					case ErrArgKind.SymWithType:
						symbol2 = errArg3.swtMemo.sym;
						break;
					case ErrArgKind.MethWithInst:
						symbol2 = errArg3.mpwiMemo.sym;
						break;
					default:
						continue;
					}
					if (symbol2 != symbol || cType2 != cType || flag)
					{
						array3[k] = array[k];
						flag = true;
					}
				}
				if (flag)
				{
					array3[j] = array[j];
				}
			}
			array = array3;
		}
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		string message = ErrorFacts.GetMessage(id);
		object[] array4 = array;
		return new RuntimeBinderException(string.Format((IFormatProvider?)invariantCulture, message, (ReadOnlySpan<object?>)array4));
	}
}
