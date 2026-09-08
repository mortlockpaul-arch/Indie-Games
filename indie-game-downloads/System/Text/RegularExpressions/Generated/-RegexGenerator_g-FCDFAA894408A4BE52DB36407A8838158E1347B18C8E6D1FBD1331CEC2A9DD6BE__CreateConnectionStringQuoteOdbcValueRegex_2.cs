using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
[SkipLocalsInit]
internal sealed class _003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__CreateConnectionStringQuoteOdbcValueRegex_2 : Regex
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
				if (num <= inputSpan.Length - 2 && num == 0)
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
				int pos = 0;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				if (num != 0)
				{
					return false;
				}
				if (readOnlySpan.IsEmpty || readOnlySpan[0] != '{')
				{
					return false;
				}
				num++;
				readOnlySpan = inputSpan.Slice(num);
				num2 = 0;
				while (true)
				{
					_003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.StackPush(ref runstack, ref pos, num);
					num2++;
					int num3 = num;
					char c;
					if (!readOnlySpan.IsEmpty && !(((c = readOnlySpan[0]) == '\0') | (c == '}')))
					{
						_003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.StackPush(ref runstack, ref pos, 0, num3);
						num++;
						readOnlySpan = inputSpan.Slice(num);
						continue;
					}
					while (true)
					{
						num = num3;
						readOnlySpan = inputSpan.Slice(num);
						if (readOnlySpan.StartsWith("}}".AsSpan()))
						{
							_003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.StackPush(ref runstack, ref pos, 1, num3);
							num += 2;
							readOnlySpan = inputSpan.Slice(num);
							break;
						}
						while (true)
						{
							if (--num2 < 0)
							{
								return false;
							}
							num = runstack[--pos];
							readOnlySpan = inputSpan.Slice(num);
							if (readOnlySpan.IsEmpty || readOnlySpan[0] != '}' || 2 < readOnlySpan.Length || (1 < readOnlySpan.Length && readOnlySpan[1] != '\n'))
							{
								if (_003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.s_hasTimeout)
								{
									CheckTimeout();
								}
								if (num2 != 0)
								{
									if (_003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.s_hasTimeout)
									{
										CheckTimeout();
									}
									num3 = runstack[--pos];
									int num4 = runstack[--pos];
									if (num4 == 0)
									{
										break;
									}
									if (num4 != 1)
									{
										goto end_IL_0099;
									}
									continue;
								}
								return false;
							}
							Capture(0, start, runtextpos = num + 1);
							return true;
						}
						continue;
						end_IL_0099:
						break;
					}
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__CreateConnectionStringQuoteOdbcValueRegex_2 Instance = new _003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__CreateConnectionStringQuoteOdbcValueRegex_2();

	private _003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__CreateConnectionStringQuoteOdbcValueRegex_2()
	{
		pattern = "^\\{([^\\}\0]|\\}\\})*\\}$";
		roptions = RegexOptions.ExplicitCapture;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFCDFAA894408A4BE52DB36407A8838158E1347B18C8E6D1FBD1331CEC2A9DD6BE__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
