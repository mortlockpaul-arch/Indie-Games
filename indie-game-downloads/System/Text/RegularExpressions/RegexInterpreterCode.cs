namespace System.Text.RegularExpressions;

internal sealed class RegexInterpreterCode(RegexFindOptimizations findOptimizations, RegexOptions options, int[] codes, string[] strings, int trackcount)
{
	public readonly RegexFindOptimizations FindOptimizations = findOptimizations;

	public readonly RegexOptions Options = options;

	public readonly int[] Codes = codes;

	public readonly string[] Strings = strings;

	public readonly uint[][] StringsAsciiLookup = new uint[strings.Length][];

	public readonly int TrackCount = trackcount;

	public static bool OpcodeBacktracks(RegexOpcode opcode)
	{
		opcode &= RegexOpcode.OperatorMask;
		switch (opcode)
		{
		case RegexOpcode.Oneloop:
		case RegexOpcode.Notoneloop:
		case RegexOpcode.Setloop:
		case RegexOpcode.Onelazy:
		case RegexOpcode.Notonelazy:
		case RegexOpcode.Setlazy:
		case RegexOpcode.Lazybranch:
		case RegexOpcode.Branchmark:
		case RegexOpcode.Lazybranchmark:
		case RegexOpcode.Nullcount:
		case RegexOpcode.Setcount:
		case RegexOpcode.Branchcount:
		case RegexOpcode.Lazybranchcount:
		case RegexOpcode.Setmark:
		case RegexOpcode.Capturemark:
		case RegexOpcode.Getmark:
		case RegexOpcode.Setjump:
		case RegexOpcode.Backjump:
		case RegexOpcode.Forejump:
		case RegexOpcode.Goto:
			return true;
		default:
			return false;
		}
	}
}
