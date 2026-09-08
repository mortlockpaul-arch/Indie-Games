using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__DecodeCharRegex_0 : Regex
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
				if (num <= inputSpan.Length - 7)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_indexOfString__x_OrdinalIgnoreCase);
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
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if ((uint)span.Length < 6u || !span.StartsWith("_x".AsSpan(), StringComparison.OrdinalIgnoreCase) || !char.IsAsciiHexDigit(span[2]) || !char.IsAsciiHexDigit(span[3]) || !char.IsAsciiHexDigit(span[4]) || !char.IsAsciiHexDigit(span[5]))
				{
					return false;
				}
				if ((uint)span.Length < 7u)
				{
					return false;
				}
				switch (span[6])
				{
				case '_':
					num += 7;
					span = inputSpan.Slice(num);
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					if ((uint)span.Length < 11u || !char.IsAsciiHexDigit(span[6]) || !char.IsAsciiHexDigit(span[7]) || !char.IsAsciiHexDigit(span[8]) || !char.IsAsciiHexDigit(span[9]) || span[10] != '_')
					{
						return false;
					}
					num += 11;
					span = inputSpan.Slice(num);
					break;
				default:
					return false;
				}
				runtextpos = num;
				Capture(0, start, num);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__DecodeCharRegex_0 Instance = new _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__DecodeCharRegex_0();

	private _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__DecodeCharRegex_0()
	{
		pattern = "_[Xx][0-9a-fA-F]{4}(?:_|[0-9a-fA-F]{4}_)";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
