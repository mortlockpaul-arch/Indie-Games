using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFEBA907BFC8DA5891021ECFC4ACF51500CA89E8FDCD0895B025EB5D8CE91E77E9__CreateIntegerRegex_0 : Regex
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
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num != 0)
				{
					return false;
				}
				int i;
				for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
				{
				}
				span = span.Slice(i);
				num += i;
				char c;
				if (!span.IsEmpty && (((c = span[0]) == '+') | (c == '-')))
				{
					span = span.Slice(1);
					num++;
				}
				int num4 = span.IndexOfAnyExceptInRange('0', '9');
				if (num4 < 0)
				{
					num4 = span.Length;
				}
				if (num4 == 0)
				{
					return false;
				}
				span = span.Slice(num4);
				num += num4;
				num2 = num;
				int j;
				for (j = 0; (uint)j < (uint)span.Length && char.IsWhiteSpace(span[j]); j++)
				{
				}
				span = span.Slice(j);
				num += j;
				num3 = num;
				while (num < inputSpan.Length - 1 || ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n'))
				{
					CheckTimeout();
					if (num2 >= num3)
					{
						return false;
					}
					num = --num3;
					span = inputSpan.Slice(num);
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

	internal static readonly _003CRegexGenerator_g_003EFEBA907BFC8DA5891021ECFC4ACF51500CA89E8FDCD0895B025EB5D8CE91E77E9__CreateIntegerRegex_0 Instance = new _003CRegexGenerator_g_003EFEBA907BFC8DA5891021ECFC4ACF51500CA89E8FDCD0895B025EB5D8CE91E77E9__CreateIntegerRegex_0();

	private _003CRegexGenerator_g_003EFEBA907BFC8DA5891021ECFC4ACF51500CA89E8FDCD0895B025EB5D8CE91E77E9__CreateIntegerRegex_0()
	{
		pattern = "^\\s*(?:\\+|\\-)?[0-9]+\\s*$";
		roptions = RegexOptions.None;
		internalMatchTimeout = TimeSpan.FromMilliseconds(200L);
		factory = new RunnerFactory();
		capsize = 1;
	}
}
