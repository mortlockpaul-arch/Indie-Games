using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__ParameterReplacementRegex_0 : Regex
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
					if (_003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 4)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__Utilities.s_indexOfString_AC244FE541E52CFD90CCD7AB13997D32F9D5031F007841348B9EE5A65488D64E);
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
				if ((uint)span.Length < 4u || !span.StartsWith("(&".AsSpan(), StringComparison.OrdinalIgnoreCase) || span[2] == '\n' || span[3] != ')')
				{
					return false;
				}
				Capture(0, start, runtextpos = num + 4);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__ParameterReplacementRegex_0 Instance = new _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__ParameterReplacementRegex_0();

	private _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__ParameterReplacementRegex_0()
	{
		pattern = "\\(\\&.\\)";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
