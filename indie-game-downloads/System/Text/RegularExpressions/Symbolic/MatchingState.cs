using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Symbolic;

internal sealed class MatchingState<TSet> where TSet : IComparable<TSet>, IEquatable<TSet>
{
	internal SymbolicRegexNode<TSet> Node { get; }

	internal uint PrevCharKind { get; }

	internal int Id { get; set; }

	internal int NullabilityInfo { get; }

	internal MatchingState(SymbolicRegexNode<TSet> node, uint prevCharKind)
	{
		Node = node;
		PrevCharKind = prevCharKind;
		NullabilityInfo = BuildNullabilityInfo();
	}

	internal bool IsDeadend(ISolver<TSet> solver)
	{
		return Node.IsNothing(solver);
	}

	internal SymbolicRegexNode<TSet> Next(SymbolicRegexBuilder<TSet> builder, TSet minterm, uint nextCharKind)
	{
		uint context = CharKind.Context(PrevCharKind, nextCharKind);
		return Node.CreateDerivativeWithoutEffects(builder, minterm, context);
	}

	internal List<(SymbolicRegexNode<TSet> Node, DerivativeEffect[] Effects)> NfaNextWithEffects(SymbolicRegexBuilder<TSet> builder, TSet minterm, uint nextCharKind)
	{
		uint context = CharKind.Context(PrevCharKind, nextCharKind);
		return Node.CreateNfaDerivativeWithEffects(builder, minterm, context);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool IsNullableFor(uint nextCharKind)
	{
		return (NullabilityInfo & (1 << (int)nextCharKind)) != 0;
	}

	internal StateFlags BuildStateFlags(bool isInitial)
	{
		StateFlags stateFlags = StateFlags.None;
		if (isInitial)
		{
			stateFlags |= StateFlags.IsInitialFlag;
		}
		if (Node.CanBeNullable)
		{
			stateFlags |= StateFlags.CanBeNullableFlag;
			if (Node.IsNullable)
			{
				stateFlags |= StateFlags.IsNullableFlag;
			}
		}
		if (Node.Kind != SymbolicRegexNodeKind.DisableBacktrackingSimulation)
		{
			stateFlags |= StateFlags.SimulatesBacktrackingFlag;
		}
		return stateFlags;
	}

	private byte BuildNullabilityInfo()
	{
		byte b = 0;
		if (Node.CanBeNullable)
		{
			for (uint num = 0u; num < 5; num++)
			{
				b |= (byte)(Node.IsNullableFor(CharKind.Context(PrevCharKind, num)) ? (1 << (int)num) : 0);
			}
		}
		return b;
	}

	public override bool Equals(object obj)
	{
		if (obj is MatchingState<TSet> matchingState && PrevCharKind == matchingState.PrevCharKind)
		{
			return Node.Equals(matchingState.Node);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(PrevCharKind, Node);
	}
}
