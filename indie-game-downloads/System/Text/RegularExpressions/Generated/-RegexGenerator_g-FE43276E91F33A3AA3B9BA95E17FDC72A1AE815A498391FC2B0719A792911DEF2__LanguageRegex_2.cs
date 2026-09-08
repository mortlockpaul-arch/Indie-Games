using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__LanguageRegex_2 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				if (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan))
				{
					runtextpos = inputSpan.Length;
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if ((uint)num < (uint)inputSpan.Length && num == 0)
				{
					return true;
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int pos = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				if (num != 0)
				{
					return false;
				}
				num2 = num;
				int i;
				for (i = 0; i < 8 && (uint)i < (uint)readOnlySpan.Length && char.IsAsciiLetter(readOnlySpan[i]); i++)
				{
				}
				if (i == 0)
				{
					return false;
				}
				readOnlySpan = readOnlySpan.Slice(i);
				num += i;
				num3 = num;
				num2++;
				while (true)
				{
					num4 = 0;
					while (true)
					{
						_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.StackPush(ref runstack, ref pos, num);
						num4++;
						if (readOnlySpan.IsEmpty || readOnlySpan[0] != '-')
						{
							break;
						}
						num++;
						readOnlySpan = inputSpan.Slice(num);
						int j;
						for (j = 0; j < 8 && (uint)j < (uint)readOnlySpan.Length && char.IsAsciiLetterOrDigit(readOnlySpan[j]); j++)
						{
						}
						if (j == 0)
						{
							break;
						}
						readOnlySpan = readOnlySpan.Slice(j);
						num += j;
					}
					while (--num4 >= 0)
					{
						num = runstack[--pos];
						readOnlySpan = inputSpan.Slice(num);
						if (num >= inputSpan.Length - 1 && ((uint)num >= (uint)inputSpan.Length || inputSpan[num] == '\n'))
						{
							runtextpos = num;
							Capture(0, start, num);
							return true;
						}
					}
					if (_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num2 >= num3)
					{
						break;
					}
					num = --num3;
					readOnlySpan = inputSpan.Slice(num);
				}
				return false;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__LanguageRegex_2 Instance = new _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__LanguageRegex_2();

	private _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__LanguageRegex_2()
	{
		pattern = "^([a-zA-Z]{1,8})(-[a-zA-Z0-9]{1,8})*$";
		roptions = RegexOptions.ExplicitCapture;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFE43276E91F33A3AA3B9BA95E17FDC72A1AE815A498391FC2B0719A792911DEF2__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
