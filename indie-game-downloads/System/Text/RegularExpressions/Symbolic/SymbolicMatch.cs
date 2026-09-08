namespace System.Text.RegularExpressions.Symbolic;

internal readonly struct SymbolicMatch(int index, int length, int[] captureStarts = null, int[] captureEnds = null)
{
	internal static SymbolicMatch NoMatch => new SymbolicMatch(-1, -1);

	internal static SymbolicMatch MatchExists => new SymbolicMatch(0, 0);

	public int Index { get; } = index;

	public int Length { get; } = length;

	public bool Success => Index >= 0;

	public int[] CaptureStarts { get; } = captureStarts;

	public int[] CaptureEnds { get; } = captureEnds;
}
