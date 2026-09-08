using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__UnknownNodeObjectRegex_10 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					if (_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 23)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_indexOfString_49E5939642BED022FF4CCC68711C353694EC5E15398AE4B384C29A17BD819BDF);
					if (num2 >= 0)
					{
						runtextpos = num + num2;
						return true;
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (!span.StartsWith("UnknownNode((object)".AsSpan()))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 20;
				span = inputSpan.Slice(num);
				num2 = num;
				int num3 = span.IndexOf(')');
				if (num3 < 0)
				{
					num3 = span.Length;
				}
				if (num3 == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(num3);
				num += num3;
				Capture(1, num2, num);
				if (!span.StartsWith(");".AsSpan()))
				{
					UncaptureUntil(0);
					return false;
				}
				Capture(0, start, runtextpos = num + 2);
				return true;
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void UncaptureUntil(int capturePosition)
				{
					while (Crawlpos() > capturePosition)
					{
						Uncapture();
					}
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__UnknownNodeObjectRegex_10 Instance = new _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__UnknownNodeObjectRegex_10();

	private _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__UnknownNodeObjectRegex_10()
	{
		pattern = "UnknownNode[(][(]object[)](?<o>[^)]+)[)];";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		base.CapNames = new Hashtable
		{
			{ "0", 0 },
			{ "o", 1 }
		};
		capslist = new string[2] { "0", "o" };
		capsize = 2;
	}
}
