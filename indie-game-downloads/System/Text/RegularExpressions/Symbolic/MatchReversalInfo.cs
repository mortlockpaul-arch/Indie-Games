namespace System.Text.RegularExpressions.Symbolic;

internal readonly struct MatchReversalInfo<TSet> where TSet : IComparable<TSet>, IEquatable<TSet>
{
	internal MatchReversalKind Kind { get; }

	internal int FixedLength { get; }

	internal MatchingState<TSet> AdjustedStartState { get; }

	internal MatchReversalInfo(MatchReversalKind kind, int fixedLength, MatchingState<TSet> adjustedStartState = null)
	{
		Kind = kind;
		FixedLength = fixedLength;
		AdjustedStartState = adjustedStartState;
	}
}
