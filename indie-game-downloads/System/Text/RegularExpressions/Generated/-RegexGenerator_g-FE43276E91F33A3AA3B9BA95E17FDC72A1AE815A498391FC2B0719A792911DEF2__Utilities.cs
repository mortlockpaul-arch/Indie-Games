using System.Buffers;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
internal static class _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities
{
	internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

	internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

	internal static readonly SearchValues<string> s_indexOfString_24E3F0A96FA8DE99FB25D242D87DAC774B6CA6628023D99447766181AE8B5C1F = SearchValues.Create(new ReadOnlySpan<string>("paramsRead["), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_49E5939642BED022FF4CCC68711C353694EC5E15398AE4B384C29A17BD819BDF = SearchValues.Create(new ReadOnlySpan<string>("UnknownNode((object)"), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_A33692426CD23EBB54EC2CE470CB74EA3BB2120AA9FB6C85181C764DD117847E = SearchValues.Create(new ReadOnlySpan<string>("UnknownNode(null, @\""), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString_C94AE607B1711C3F90CF8B8C224548ED74A865FE986797F62276464CE6F9271D = SearchValues.Create(new ReadOnlySpan<string>("(("), StringComparison.Ordinal);

	internal static readonly SearchValues<string> s_indexOfString__x_OrdinalIgnoreCase = SearchValues.Create(new ReadOnlySpan<string>("_x"), StringComparison.OrdinalIgnoreCase);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StackPush(ref int[] stack, ref int pos, int arg0)
	{
		int[] array = stack;
		int num = pos;
		if ((uint)num < (uint)array.Length)
		{
			array[num] = arg0;
			pos++;
		}
		else
		{
			WithResize(ref stack, ref pos, arg0);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void WithResize(ref int[] reference, ref int reference2, int arg1)
		{
			Array.Resize(ref reference, reference2 * 2);
			StackPush(ref reference, ref reference2, arg1);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StackPush(ref int[] stack, ref int pos, int arg0, int arg1)
	{
		int[] array = stack;
		int num = pos;
		if ((uint)(num + 1) < (uint)array.Length)
		{
			array[num] = arg0;
			array[num + 1] = arg1;
			pos += 2;
		}
		else
		{
			WithResize(ref stack, ref pos, arg0, arg1);
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void WithResize(ref int[] reference, ref int reference2, int arg2, int arg3)
		{
			Array.Resize(ref reference, (reference2 + 1) * 2);
			StackPush(ref reference, ref reference2, arg2, arg3);
		}
	}
}
