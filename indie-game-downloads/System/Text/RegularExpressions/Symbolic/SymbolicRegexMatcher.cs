using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Text.RegularExpressions.Symbolic;

internal sealed class SymbolicRegexMatcher<TSet> : SymbolicRegexMatcher where TSet : IComparable<TSet>, IEquatable<TSet>
{
	internal struct Registers(int[] captureStarts, int[] captureEnds)
	{
		public int[] CaptureStarts { get; } = captureStarts;

		public int[] CaptureEnds { get; } = captureEnds;

		public void ApplyEffects(DerivativeEffect[] effects, int pos)
		{
			foreach (DerivativeEffect effect in effects)
			{
				ApplyEffect(effect, pos);
			}
		}

		public void ApplyEffect(DerivativeEffect effect, int pos)
		{
			switch (effect.Kind)
			{
			case DerivativeEffectKind.CaptureStart:
				CaptureStarts[effect.CaptureNumber] = pos;
				break;
			case DerivativeEffectKind.CaptureEnd:
				CaptureEnds[effect.CaptureNumber] = pos;
				break;
			}
		}

		public Registers Clone()
		{
			return new Registers((int[])CaptureStarts.Clone(), (int[])CaptureEnds.Clone());
		}
	}

	internal sealed class PerThreadData
	{
		public readonly NfaMatchingState NfaState;

		public readonly SparseIntMap<Registers> Current;

		public readonly SparseIntMap<Registers> Next;

		public readonly Registers InitialRegisters;

		public PerThreadData(int capsize)
		{
			NfaState = new NfaMatchingState();
			if (capsize > 1)
			{
				Current = new SparseIntMap<Registers>();
				Next = new SparseIntMap<Registers>();
				InitialRegisters = new Registers(new int[capsize], new int[capsize]);
			}
		}
	}

	internal sealed class NfaMatchingState
	{
		public SparseIntMap<int> NfaStateSet = new SparseIntMap<int>();

		public SparseIntMap<int> NfaStateSetScratch = new SparseIntMap<int>();

		public void InitializeFrom(SymbolicRegexMatcher<TSet> matcher, MatchingState<TSet> dfaMatchingState)
		{
			NfaStateSet.Clear();
			matcher.ForEachNfaState(dfaMatchingState.Node, dfaMatchingState.PrevCharKind, NfaStateSet, delegate(int nfaId, SparseIntMap<int> nfaStateSet)
			{
				nfaStateSet.Add(nfaId, out var _);
			});
		}
	}

	private struct CurrentState
	{
		public int DfaStateId;

		public NfaMatchingState NfaState;

		public CurrentState(MatchingState<TSet> dfaState)
		{
			DfaStateId = dfaState.Id;
			NfaState = null;
		}

		public CurrentState(NfaMatchingState nfaState)
		{
			DfaStateId = -1;
			NfaState = nfaState;
		}
	}

	private interface IStateHandler
	{
		static abstract bool IsNullableFor(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, uint nextCharKind);

		static abstract StateFlags GetStateFlags(SymbolicRegexMatcher<TSet> matcher, in CurrentState state);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct DfaStateHandler : IStateHandler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullableFor(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, uint nextCharKind)
		{
			if (matcher._nullabilityArray[state.DfaStateId] > 0)
			{
				return ((byte)(1 << (int)nextCharKind) & matcher._nullabilityArray[state.DfaStateId]) > 0;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryTakeTransition(SymbolicRegexMatcher<TSet> matcher, ref int dfaStateId, int mintermId, long timeoutOccursAt = 0L)
		{
			int num = matcher.DeltaOffset(dfaStateId, mintermId);
			int num2 = matcher._dfaDelta[num];
			if (num2 > 0)
			{
				dfaStateId = num2;
				return true;
			}
			if (matcher.TryCreateNewTransition(matcher.GetState(dfaStateId), mintermId, num, checkThreshold: true, timeoutOccursAt, out var nextState))
			{
				dfaStateId = nextState.Id;
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StateFlags GetStateFlags(SymbolicRegexMatcher<TSet> matcher, in CurrentState state)
		{
			return matcher._stateFlagsArray[state.DfaStateId];
		}

		static bool IStateHandler.IsNullableFor(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, uint nextCharKind)
		{
			return IsNullableFor(matcher, in state, nextCharKind);
		}

		static StateFlags IStateHandler.GetStateFlags(SymbolicRegexMatcher<TSet> matcher, in CurrentState state)
		{
			return GetStateFlags(matcher, in state);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NfaStateHandler : IStateHandler
	{
		public static bool IsNullableFor(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, uint nextCharKind)
		{
			Span<KeyValuePair<int, int>> span = CollectionsMarshal.AsSpan(state.NfaState.NfaStateSet.Values);
			for (int i = 0; i < span.Length; i++)
			{
				if (matcher.GetState(matcher.GetCoreStateId(span[i].Key)).IsNullableFor(nextCharKind))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryTakeTransition(SymbolicRegexMatcher<TSet> matcher, ref CurrentState state, int mintermId)
		{
			NfaMatchingState nfaState = state.NfaState;
			SparseIntMap<int> nfaStateSetScratch = nfaState.NfaStateSetScratch;
			SparseIntMap<int> nfaStateSet = nfaState.NfaStateSet;
			nfaState.NfaStateSet = nfaStateSetScratch;
			nfaState.NfaStateSetScratch = nfaStateSet;
			nfaStateSetScratch.Clear();
			if (nfaStateSet.Count == 1)
			{
				int[] array = GetNextStates(nfaStateSet.Values[0].Key, mintermId, matcher);
				foreach (int key in array)
				{
					nfaStateSetScratch.Add(key, out var _);
				}
			}
			else
			{
				uint positionKind = matcher.GetPositionKind(mintermId);
				Span<KeyValuePair<int, int>> span = CollectionsMarshal.AsSpan(nfaStateSet.Values);
				for (int i = 0; i < span.Length; i++)
				{
					ref KeyValuePair<int, int> reference = ref span[i];
					int[] array = GetNextStates(reference.Key, mintermId, matcher);
					foreach (int key2 in array)
					{
						nfaStateSetScratch.Add(key2, out var _);
					}
					int coreStateId = matcher.GetCoreStateId(reference.Key);
					StateFlags info = matcher._stateFlagsArray[coreStateId];
					if (info.SimulatesBacktracking() && (info.IsNullable() || (info.CanBeNullable() && matcher.GetState(coreStateId).IsNullableFor(positionKind))))
					{
						break;
					}
				}
			}
			return true;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static int[] GetNextStates(int sourceState, int mintermId2, SymbolicRegexMatcher<TSet> symbolicRegexMatcher)
			{
				int num = symbolicRegexMatcher.DeltaOffset(sourceState, mintermId2);
				return symbolicRegexMatcher._nfaDelta[num] ?? symbolicRegexMatcher.CreateNewNfaTransition(sourceState, mintermId2, num);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StateFlags GetStateFlags(SymbolicRegexMatcher<TSet> matcher, in CurrentState state)
		{
			StateFlags stateFlags = StateFlags.None;
			Span<KeyValuePair<int, int>> span = CollectionsMarshal.AsSpan(state.NfaState.NfaStateSet.Values);
			for (int i = 0; i < span.Length; i++)
			{
				ref KeyValuePair<int, int> reference = ref span[i];
				stateFlags |= matcher._stateFlagsArray[matcher.GetCoreStateId(reference.Key)];
			}
			return stateFlags & (StateFlags.IsNullableFlag | StateFlags.CanBeNullableFlag | StateFlags.SimulatesBacktrackingFlag);
		}

		static bool IStateHandler.IsNullableFor(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, uint nextCharKind)
		{
			return IsNullableFor(matcher, in state, nextCharKind);
		}

		static StateFlags IStateHandler.GetStateFlags(SymbolicRegexMatcher<TSet> matcher, in CurrentState state)
		{
			return GetStateFlags(matcher, in state);
		}
	}

	private interface IInputReader
	{
		static abstract int GetPositionId(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, int pos);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct DefaultInputReader : IInputReader
	{
		public static int GetPositionId(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, int pos)
		{
			if ((uint)pos < (uint)input.Length)
			{
				int num = input[pos];
				if (num != 10 || pos != input.Length - 1)
				{
					return matcher._mintermClassifier.GetMintermID(num);
				}
				return matcher._minterms.Length;
			}
			return -1;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NoZAnchorOptimizedInputReader : IInputReader
	{
		public static int GetPositionId(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, int pos)
		{
			if ((uint)pos >= (uint)input.Length)
			{
				return -1;
			}
			return matcher._mintermClassifier.GetMintermID(input[pos]);
		}
	}

	private interface IInitialStateHandler
	{
		static abstract bool IsOptimized { get; }

		static abstract bool TryFindNextStartingPosition(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, ref int currentStateId, ref int pos, byte[] lookup);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NoOptimizationsInitialStateHandler : IInitialStateHandler
	{
		public static bool IsOptimized => false;

		public static bool TryFindNextStartingPosition(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, ref int currentStateId, ref int pos, byte[] lookup)
		{
			return true;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct FindOptimizationsInitialStateHandler : IInitialStateHandler
	{
		public static bool IsOptimized => true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryFindNextStartingPosition(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, ref int currentStateId, ref int pos, byte[] lookup)
		{
			if (matcher._findOpts.TryFindNextStartingPositionLeftToRight(input, ref pos, 0))
			{
				currentStateId = matcher._dotstarredInitialStates[matcher.GetCharKind(input, pos - 1)].Id;
				return true;
			}
			currentStateId = matcher._deadStateId;
			return false;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NoZAnchorFindOptimizationsInitialStateHandler : IInitialStateHandler
	{
		public static bool IsOptimized => true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryFindNextStartingPosition(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, ref int currentStateId, ref int pos, byte[] lookup)
		{
			if (matcher._findOpts.TryFindNextStartingPositionLeftToRight(input, ref pos, 0))
			{
				currentStateId = matcher._dotstarredInitialStates[matcher._positionKinds[SymbolicRegexMatcher<TSet>.GetMintermId(lookup, input[pos - 1]) + 1]].Id;
				return true;
			}
			currentStateId = matcher._deadStateId;
			return false;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NoAnchorsFindOptimizationsInitialStateHandler : IInitialStateHandler
	{
		public static bool IsOptimized => true;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryFindNextStartingPosition(SymbolicRegexMatcher<TSet> matcher, ReadOnlySpan<char> input, ref int currentStateId, ref int pos, byte[] lookup)
		{
			if (matcher._findOpts.TryFindNextStartingPositionLeftToRight(input, ref pos, 0))
			{
				return true;
			}
			currentStateId = matcher._deadStateId;
			return false;
		}
	}

	private interface INullabilityHandler
	{
		static abstract bool IsNullableAt<TStateHandler>(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, int positionId) where TStateHandler : struct, IStateHandler;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct DefaultNullabilityHandler : INullabilityHandler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullableAt<TStateHandler>(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, int positionId) where TStateHandler : struct, IStateHandler
		{
			StateFlags stateFlags = TStateHandler.GetStateFlags(matcher, in state);
			if (!stateFlags.IsNullable())
			{
				if (stateFlags.CanBeNullable())
				{
					return TStateHandler.IsNullableFor(matcher, in state, matcher.GetPositionKind(positionId));
				}
				return false;
			}
			return true;
		}

		static bool INullabilityHandler.IsNullableAt<TStateHandler>(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, int positionId)
		{
			return IsNullableAt<TStateHandler>(matcher, in state, positionId);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NoAnchorsNullabilityHandler : INullabilityHandler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullableAt<TStateHandler>(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, int positionId) where TStateHandler : struct, IStateHandler
		{
			return TStateHandler.GetStateFlags(matcher, in state).IsNullable();
		}

		static bool INullabilityHandler.IsNullableAt<TStateHandler>(SymbolicRegexMatcher<TSet> matcher, in CurrentState state, int positionId)
		{
			return IsNullableAt<TStateHandler>(matcher, in state, positionId);
		}
	}

	private interface IDfaNoZAnchorOptimizedNullabilityHandler
	{
		static abstract bool IsNullable(SymbolicRegexMatcher<TSet> matcher, byte stateNullability, char c, byte[] lookup);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct DefaultDfaNoZAnchorOptimizedNullabilityHandler : IDfaNoZAnchorOptimizedNullabilityHandler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullable(SymbolicRegexMatcher<TSet> matcher, byte stateNullability, char c, byte[] lookup)
		{
			if (stateNullability != 0)
			{
				return matcher.IsNullableWithContext(stateNullability, ((uint)c < (uint)lookup.Length) ? lookup[(uint)c] : 0);
			}
			return false;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct NoAnchorDfaOptimizedNullabilityHandler : IDfaNoZAnchorOptimizedNullabilityHandler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullable(SymbolicRegexMatcher<TSet> matcher, byte stateNullability, char c, byte[] lookup)
		{
			return stateNullability != 0;
		}
	}

	private readonly Dictionary<(SymbolicRegexNode<TSet> Node, uint PrevCharKind), MatchingState<TSet>> _stateCache = new Dictionary<(SymbolicRegexNode<TSet>, uint), MatchingState<TSet>>();

	private MatchingState<TSet>[] _stateArray;

	private StateFlags[] _stateFlagsArray;

	private byte[] _nullabilityArray;

	private int[] _dfaDelta;

	private int[] _nfaCoreIdArray = Array.Empty<int>();

	private readonly Dictionary<int, int> _nfaIdByCoreId = new Dictionary<int, int>();

	private int[][] _nfaDelta = Array.Empty<int[]>();

	private (int, DerivativeEffect[])[][] _capturingNfaDelta = Array.Empty<(int, DerivativeEffect[])[]>();

	internal readonly SymbolicRegexBuilder<TSet> _builder;

	private readonly MintermClassifier _mintermClassifier;

	internal readonly SymbolicRegexNode<TSet> _dotStarredPattern;

	internal readonly SymbolicRegexNode<TSet> _pattern;

	internal readonly SymbolicRegexNode<TSet> _reversePattern;

	private readonly bool _checkTimeout;

	private readonly int _timeout;

	private readonly RegexFindOptimizations _findOpts;

	private readonly int _deadStateId;

	private readonly int _initialStateId;

	private readonly bool _containsAnyAnchor;

	private readonly bool _containsEndZAnchor;

	private readonly MatchingState<TSet>[] _initialStates;

	private readonly MatchingState<TSet>[] _dotstarredInitialStates;

	private readonly MatchingState<TSet>[] _reverseInitialStates;

	private readonly MatchReversalInfo<TSet> _optimizedReversalInfo;

	private readonly TSet[] _minterms;

	private readonly uint[] _positionKinds;

	private readonly int _mintermsLog;

	private readonly int _capsize;

	internal bool HasSubcaptures => _capsize > 1;

	private ISolver<TSet> Solver => _builder._solver;

	private static void ArrayResizeAndVolatilePublish<T>(ref T[] array, int newSize)
	{
		T[] array2 = new T[newSize];
		Array.Copy(array, array2, array.Length);
		Volatile.Write(ref array, array2);
	}

	private int DeltaOffset(int stateId, int mintermId)
	{
		return (stateId << _mintermsLog) | mintermId;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsNullableWithContext(byte stateNullability, int mintermId)
	{
		return (stateNullability & (1 << (int)GetPositionKind(mintermId))) != 0;
	}

	private MatchingState<TSet> GetOrCreateState(SymbolicRegexNode<TSet> node, uint prevCharKind)
	{
		return GetOrCreateState_NoLock(node, prevCharKind);
	}

	private MatchReversalInfo<TSet> CreateOptimizedReversal(SymbolicRegexNode<TSet> node)
	{
		int num = 0;
		while (true)
		{
			if (node._info.ContainsSomeAnchor)
			{
				num = 0;
				break;
			}
			if (node._kind != SymbolicRegexNodeKind.Concat)
			{
				if (node._kind == SymbolicRegexNodeKind.CaptureStart)
				{
					node = _builder.Epsilon;
				}
				break;
			}
			SymbolicRegexNode<TSet> left = node._left;
			SymbolicRegexNodeKind kind = left._kind;
			if ((kind == SymbolicRegexNodeKind.Singleton || kind == SymbolicRegexNodeKind.BoundaryAnchor || kind == SymbolicRegexNodeKind.CaptureEnd) ? true : false)
			{
				node = node._right;
				if (left._kind == SymbolicRegexNodeKind.Singleton)
				{
					num++;
				}
				continue;
			}
			if (left._kind != SymbolicRegexNodeKind.Loop || left._lower <= 0 || left._left.Kind != SymbolicRegexNodeKind.Singleton)
			{
				break;
			}
			node = ((left._lower == left._upper) ? node._right : _builder.CreateConcat(_builder.CreateLoop(left._left, left.IsLazy, 0, left._upper - left._lower), node._right));
			num += left._lower;
		}
		if (num != 0)
		{
			if (node != _builder.Epsilon)
			{
				return new MatchReversalInfo<TSet>(MatchReversalKind.PartialFixedLength, num, GetOrCreateState_NoLock(_builder.CreateDisableBacktrackingSimulation(node), 0u));
			}
			return new MatchReversalInfo<TSet>(MatchReversalKind.FixedLength, num);
		}
		return new MatchReversalInfo<TSet>(MatchReversalKind.MatchStart, 0);
	}

	private MatchingState<TSet> GetOrCreateState_NoLock(SymbolicRegexNode<TSet> node, uint prevCharKind, bool isInitialState = false)
	{
		(SymbolicRegexNode<TSet>, uint) key = (node.PruneAnchors(_builder, prevCharKind), prevCharKind);
		if (!_stateCache.TryGetValue(key, out var value))
		{
			value = new MatchingState<TSet>(key.Item1, key.Item2);
			_stateCache.Add(key, value);
			value.Id = _stateCache.Count;
			if (value.Id == _stateArray.Length)
			{
				int num = _stateArray.Length * 2;
				ArrayResizeAndVolatilePublish(ref _stateArray, num);
				ArrayResizeAndVolatilePublish(ref _dfaDelta, num << _mintermsLog);
				ArrayResizeAndVolatilePublish(ref _stateFlagsArray, num);
				ArrayResizeAndVolatilePublish(ref _nullabilityArray, num);
			}
			_stateArray[value.Id] = value;
			_stateFlagsArray[value.Id] = value.BuildStateFlags(isInitialState);
			_nullabilityArray[value.Id] = (byte)value.NullabilityInfo;
		}
		return value;
	}

	private int? CreateNfaState(SymbolicRegexNode<TSet> node, uint prevCharKind)
	{
		MatchingState<TSet> orCreateState = GetOrCreateState(node, prevCharKind);
		if (orCreateState.IsDeadend(Solver))
		{
			return null;
		}
		if (!_nfaIdByCoreId.TryGetValue(orCreateState.Id, out var value))
		{
			value = _nfaIdByCoreId.Count;
			if (value == _nfaCoreIdArray.Length)
			{
				int num = Math.Max(_nfaCoreIdArray.Length * 2, 64);
				ArrayResizeAndVolatilePublish(ref _nfaCoreIdArray, num);
				ArrayResizeAndVolatilePublish(ref _nfaDelta, num << _mintermsLog);
				ArrayResizeAndVolatilePublish(ref _capturingNfaDelta, num << _mintermsLog);
			}
			_nfaCoreIdArray[value] = orCreateState.Id;
			_nfaIdByCoreId.Add(orCreateState.Id, value);
		}
		return value;
	}

	private MatchingState<TSet> GetState(int stateId)
	{
		return _stateArray[stateId];
	}

	private int GetCoreStateId(int nfaStateId)
	{
		return _nfaCoreIdArray[nfaStateId];
	}

	private bool TryCreateNewTransition(MatchingState<TSet> sourceState, int mintermId, int offset, bool checkThreshold, long timeoutOccursAt, [NotNullWhen(true)] out MatchingState<TSet> nextState)
	{
		lock (this)
		{
			MatchingState<TSet> matchingState = _stateArray[_dfaDelta[offset]];
			if (matchingState == null)
			{
				if ((timeoutOccursAt != 0L && Environment.TickCount64 > timeoutOccursAt) || (checkThreshold && _builder._nodeCache.Count >= 125000))
				{
					nextState = null;
					return false;
				}
				TSet mintermFromId = GetMintermFromId(mintermId);
				uint positionKind = GetPositionKind(mintermId);
				matchingState = GetOrCreateState(sourceState.Next(_builder, mintermFromId, positionKind), positionKind);
				Volatile.Write(ref _dfaDelta[offset], matchingState.Id);
			}
			nextState = matchingState;
			return true;
		}
	}

	private int[] CreateNewNfaTransition(int nfaStateId, int mintermId, int nfaOffset)
	{
		lock (this)
		{
			int[] array = _nfaDelta[nfaOffset];
			if (array == null)
			{
				int coreStateId = GetCoreStateId(nfaStateId);
				int num = (coreStateId << _mintermsLog) | mintermId;
				int num2 = _dfaDelta[num];
				MatchingState<TSet> state = GetState(coreStateId);
				TSet mintermFromId = GetMintermFromId(mintermId);
				uint positionKind = GetPositionKind(mintermId);
				SymbolicRegexNode<TSet> node = ((num2 > 0) ? GetState(num2).Node : state.Next(_builder, mintermFromId, positionKind));
				List<int> list = new List<int>();
				ForEachNfaState(node, positionKind, list, delegate(int nfaId, List<int> targetsList)
				{
					targetsList.Add(nfaId);
				});
				array = list.ToArray();
				Volatile.Write(ref _nfaDelta[nfaOffset], array);
			}
			return array;
		}
	}

	private (int, DerivativeEffect[])[] CreateNewCapturingTransition(int nfaStateId, int mintermId, int offset)
	{
		lock (this)
		{
			(int, DerivativeEffect[])[] array = _capturingNfaDelta[offset];
			if (array == null)
			{
				MatchingState<TSet> state = GetState(GetCoreStateId(nfaStateId));
				TSet mintermFromId = GetMintermFromId(mintermId);
				uint positionKind = GetPositionKind(mintermId);
				List<(SymbolicRegexNode<TSet> Node, DerivativeEffect[] Effects)> list = state.NfaNextWithEffects(_builder, mintermFromId, positionKind);
				List<(int, DerivativeEffect[])> list2 = new List<(int, DerivativeEffect[])>();
				foreach (var item in list)
				{
					ForEachNfaState(item.Node, positionKind, (list2, item.Effects), delegate(int nfaId, (List<(int, DerivativeEffect[])> Targets, DerivativeEffect[] Effects) args)
					{
						args.Targets.Add((nfaId, args.Effects));
					});
				}
				array = list2.ToArray();
				Volatile.Write(ref _capturingNfaDelta[offset], array);
			}
			return array;
		}
	}

	private void ForEachNfaState<T>(SymbolicRegexNode<TSet> node, uint prevCharKind, T arg, Action<int, T> action)
	{
		lock (this)
		{
			foreach (SymbolicRegexNode<TSet> item in node.EnumerateAlternationBranches(_builder))
			{
				int? num = CreateNfaState(item, prevCharKind);
				if (num.HasValue)
				{
					int valueOrDefault = num.GetValueOrDefault();
					action(valueOrDefault, arg);
				}
			}
		}
	}

	public static SymbolicRegexMatcher<TSet> Create(int captureCount, RegexFindOptimizations findOptimizations, SymbolicRegexBuilder<BDD> bddBuilder, SymbolicRegexNode<BDD> rootBddNode, ISolver<TSet> solver, TimeSpan matchTimeout)
	{
		CharSetSolver charSetSolver = (CharSetSolver)bddBuilder._solver;
		SymbolicRegexBuilder<TSet> builder = new SymbolicRegexBuilder<TSet>(solver, charSetSolver)
		{
			_wordLetterForBoundariesSet = solver.ConvertFromBDD(bddBuilder._wordLetterForBoundariesSet, charSetSolver),
			_newLineSet = solver.ConvertFromBDD(bddBuilder._newLineSet, charSetSolver)
		};
		SymbolicRegexNode<TSet> rootNode = bddBuilder.Transform(rootBddNode, builder, (SymbolicRegexBuilder<TSet> symbolicRegexBuilder, BDD bdd) => symbolicRegexBuilder._solver.ConvertFromBDD(bdd, charSetSolver));
		return new SymbolicRegexMatcher<TSet>(builder, rootNode, captureCount, findOptimizations, matchTimeout);
	}

	private SymbolicRegexMatcher(SymbolicRegexBuilder<TSet> builder, SymbolicRegexNode<TSet> rootNode, int captureCount, RegexFindOptimizations findOptimizations, TimeSpan matchTimeout)
	{
		_pattern = rootNode;
		_builder = builder;
		_checkTimeout = Regex.InfiniteMatchTimeout != matchTimeout;
		_timeout = (int)(matchTimeout.TotalMilliseconds + 0.5);
		TSet[] minterms = builder._solver.GetMinterms();
		_minterms = minterms;
		_mintermsLog = BitOperations.Log2((uint)_minterms.Length) + 1;
		_mintermClassifier = ((builder._solver is UInt64Solver uInt64Solver) ? uInt64Solver._classifier : ((BitVectorSolver)(object)builder._solver)._classifier);
		_capsize = captureCount;
		_stateArray = new MatchingState<TSet>[1024];
		_stateFlagsArray = new StateFlags[1024];
		_nullabilityArray = new byte[1024];
		_dfaDelta = new int[1024 << _mintermsLog];
		_positionKinds = new uint[_minterms.Length + 2];
		for (int i = -1; i < _positionKinds.Length - 1; i++)
		{
			_positionKinds[i + 1] = CalculateMintermIdKind(i);
			uint CalculateMintermIdKind(int mintermId)
			{
				if (_pattern._info.ContainsSomeAnchor)
				{
					if (mintermId == -1)
					{
						return 1u;
					}
					if (mintermId == _minterms.Length)
					{
						return 3u;
					}
					TSet val = _minterms[mintermId];
					if (_builder._newLineSet.Equals(val))
					{
						return 2u;
					}
					if (!Solver.IsEmpty(Solver.And(_builder._wordLetterForBoundariesSet, val)))
					{
						return 4u;
					}
				}
				return 0u;
			}
		}
		_optimizedReversalInfo = CreateOptimizedReversal(_pattern.Reverse(builder));
		if (findOptimizations.IsUseful && findOptimizations.LeadingAnchor != RegexNodeKind.Beginning)
		{
			_findOpts = findOptimizations;
		}
		int num = ((!_pattern._info.ContainsSomeAnchor) ? 1 : 5);
		_containsAnyAnchor = _pattern._info.ContainsSomeAnchor;
		_containsEndZAnchor = _pattern._info.ContainsEndZAnchor;
		MatchingState<TSet>[] array = new MatchingState<TSet>[num];
		for (uint num2 = 0u; num2 < array.Length; num2++)
		{
			array[num2] = GetOrCreateState_NoLock(_pattern, num2);
		}
		_initialStates = array;
		_dotStarredPattern = builder.CreateConcat(builder._anyStarLazy, _pattern);
		MatchingState<TSet>[] array2 = new MatchingState<TSet>[num];
		for (uint num3 = 0u; num3 < array2.Length; num3++)
		{
			array2[num3] = GetOrCreateState_NoLock(_dotStarredPattern, num3, isInitialState: true);
		}
		_dotstarredInitialStates = array2;
		_deadStateId = GetOrCreateState_NoLock(_builder._nothing, 0u).Id;
		_initialStateId = _dotstarredInitialStates[0].Id;
		_reversePattern = builder.CreateDisableBacktrackingSimulation(_pattern.Reverse(builder));
		MatchingState<TSet>[] array3 = new MatchingState<TSet>[num];
		for (uint num4 = 0u; num4 < array3.Length; num4++)
		{
			array3[num4] = GetOrCreateState_NoLock(_reversePattern, num4);
		}
		_reverseInitialStates = array3;
	}

	internal PerThreadData CreatePerThreadData()
	{
		return new PerThreadData(_capsize);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint GetPositionKind(int positionId)
	{
		return _positionKinds[positionId + 1];
	}

	internal TSet GetMintermFromId(int mintermId)
	{
		TSet[] minterms = _minterms;
		if ((uint)mintermId < (uint)minterms.Length)
		{
			return minterms[mintermId];
		}
		return _builder._newLineSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint GetCharKind(ReadOnlySpan<char> input, int i)
	{
		if (_pattern._info.ContainsSomeAnchor)
		{
			return GetPositionKind(DefaultInputReader.GetPositionId(this, input, i));
		}
		return 0u;
	}

	private void CheckTimeout(long timeoutOccursAt)
	{
		if (Environment.TickCount64 >= timeoutOccursAt)
		{
			ThrowRegexTimeout();
		}
		void ThrowRegexTimeout()
		{
			throw new RegexMatchTimeoutException(string.Empty, string.Empty, TimeSpan.FromMilliseconds(_timeout));
		}
	}

	public SymbolicMatch FindMatch(RegexRunnerMode mode, ReadOnlySpan<char> input, int startat, PerThreadData perThreadData)
	{
		long timeoutOccursAt = 0L;
		if (_checkTimeout)
		{
			timeoutOccursAt = Environment.TickCount64 + _timeout;
		}
		int num3;
		if (!_containsEndZAnchor && _mintermClassifier.ByteLookup != null)
		{
			bool num = _findOpts != null;
			bool containsAnyAnchor = _containsAnyAnchor;
			int num2 = (num ? ((!containsAnyAnchor) ? FindEndPositionOptimized<NoAnchorsFindOptimizationsInitialStateHandler, NoAnchorDfaOptimizedNullabilityHandler>(input, startat, timeoutOccursAt, mode, perThreadData) : FindEndPositionOptimized<NoZAnchorFindOptimizationsInitialStateHandler, DefaultDfaNoZAnchorOptimizedNullabilityHandler>(input, startat, timeoutOccursAt, mode, perThreadData)) : ((!containsAnyAnchor) ? FindEndPositionOptimized<NoOptimizationsInitialStateHandler, NoAnchorDfaOptimizedNullabilityHandler>(input, startat, timeoutOccursAt, mode, perThreadData) : FindEndPositionOptimized<NoOptimizationsInitialStateHandler, DefaultDfaNoZAnchorOptimizedNullabilityHandler>(input, startat, timeoutOccursAt, mode, perThreadData)));
			num3 = num2;
		}
		else
		{
			num3 = ((_findOpts != null) ? FindEndPositionFallback<FindOptimizationsInitialStateHandler, DefaultNullabilityHandler>(input, startat, timeoutOccursAt, mode, perThreadData) : FindEndPositionFallback<NoOptimizationsInitialStateHandler, DefaultNullabilityHandler>(input, startat, timeoutOccursAt, mode, perThreadData));
		}
		if (num3 == -2)
		{
			return SymbolicMatch.NoMatch;
		}
		if (mode == RegexRunnerMode.ExistenceRequired)
		{
			return SymbolicMatch.MatchExists;
		}
		int num4 = 0;
		switch (_optimizedReversalInfo.Kind)
		{
		case MatchReversalKind.MatchStart:
		case MatchReversalKind.PartialFixedLength:
		{
			int initialLastStart = -1;
			int num5 = num3;
			CurrentState state;
			if (_optimizedReversalInfo.Kind == MatchReversalKind.MatchStart)
			{
				state = new CurrentState(_reverseInitialStates[GetCharKind(input, num3)]);
			}
			else
			{
				num5 -= _optimizedReversalInfo.FixedLength;
				state = new CurrentState(_optimizedReversalInfo.AdjustedStartState);
				if (_containsAnyAnchor && _nullabilityArray[state.DfaStateId] > 0 && DefaultNullabilityHandler.IsNullableAt<DfaStateHandler>(this, in state, DefaultInputReader.GetPositionId(this, input, num5)))
				{
					initialLastStart = num5;
				}
			}
			int num2;
			if (num3 < startat)
			{
				num2 = startat;
			}
			else
			{
				bool containsEndZAnchor = _containsEndZAnchor;
				bool containsAnyAnchor = _containsAnyAnchor;
				int num6 = (containsEndZAnchor ? ((!containsAnyAnchor) ? FindStartPosition<DefaultInputReader, NoAnchorsNullabilityHandler>(state, initialLastStart, input, num5, startat, perThreadData) : FindStartPosition<DefaultInputReader, DefaultNullabilityHandler>(state, initialLastStart, input, num5, startat, perThreadData)) : ((!containsAnyAnchor) ? FindStartPosition<NoZAnchorOptimizedInputReader, NoAnchorsNullabilityHandler>(state, initialLastStart, input, num5, startat, perThreadData) : FindStartPosition<NoZAnchorOptimizedInputReader, DefaultNullabilityHandler>(state, initialLastStart, input, num5, startat, perThreadData)));
				num2 = num6;
			}
			num4 = num2;
			break;
		}
		case MatchReversalKind.FixedLength:
			num4 = num3 - _optimizedReversalInfo.FixedLength;
			break;
		}
		if (!HasSubcaptures || mode < RegexRunnerMode.FullMatchRequired)
		{
			return new SymbolicMatch(num4, num3 - num4);
		}
		Registers registers = (_containsAnyAnchor ? FindSubcaptures<DefaultInputReader>(input, num4, num3, perThreadData) : FindSubcaptures<NoZAnchorOptimizedInputReader>(input, num4, num3, perThreadData));
		return new SymbolicMatch(num4, num3 - num4, registers.CaptureStarts, registers.CaptureEnds);
	}

	private int FindEndPositionOptimized<TInitialStateHandler, TOptimizedNullabilityHandler>(ReadOnlySpan<char> input, int pos, long timeoutOccursAt, RegexRunnerMode mode, PerThreadData perThreadData) where TInitialStateHandler : struct, IInitialStateHandler where TOptimizedNullabilityHandler : struct, IDfaNoZAnchorOptimizedNullabilityHandler
	{
		CurrentState state = new CurrentState(_dotstarredInitialStates[GetCharKind(input, pos - 1)]);
		int endPosRef = -2;
		int num = input.Length - 1;
		while (true)
		{
			int num2;
			bool flag;
			if (state.NfaState == null)
			{
				num2 = ((_checkTimeout && num - pos > 100000) ? (pos + 100000) : num);
				flag = FindEndPositionDeltasDFAOptimized<TInitialStateHandler, TOptimizedNullabilityHandler>(input, num2, mode, timeoutOccursAt, ref pos, ref state.DfaStateId, ref endPosRef);
			}
			else
			{
				num2 = ((_checkTimeout && input.Length - pos > 1000) ? (pos + 1000) : input.Length);
				flag = FindEndPositionDeltasNFA<DefaultNullabilityHandler>(input, num2, mode, ref pos, ref state, ref endPosRef);
			}
			if (flag || pos >= input.Length)
			{
				break;
			}
			if (pos < num2)
			{
				NfaMatchingState nfaState = perThreadData.NfaState;
				nfaState.InitializeFrom(this, GetState(state.DfaStateId));
				state = new CurrentState(nfaState);
			}
			if (_checkTimeout)
			{
				CheckTimeout(timeoutOccursAt);
			}
		}
		return endPosRef;
	}

	private int FindEndPositionFallback<TInitialStateHandler, TNullabilityHandler>(ReadOnlySpan<char> input, int pos, long timeoutOccursAt, RegexRunnerMode mode, PerThreadData perThreadData) where TInitialStateHandler : struct, IInitialStateHandler where TNullabilityHandler : struct, INullabilityHandler
	{
		CurrentState state = new CurrentState(_dotstarredInitialStates[GetCharKind(input, pos - 1)]);
		int endPosRef = -2;
		while (true)
		{
			int num;
			bool flag;
			if (state.NfaState == null)
			{
				num = ((_checkTimeout && input.Length - pos > 25000) ? (pos + 25000) : input.Length);
				flag = FindEndPositionDeltasDFA<TInitialStateHandler, TNullabilityHandler>(input, num, mode, ref pos, ref state, ref endPosRef);
			}
			else
			{
				num = ((_checkTimeout && input.Length - pos > 1000) ? (pos + 1000) : input.Length);
				flag = FindEndPositionDeltasNFA<TNullabilityHandler>(input, num, mode, ref pos, ref state, ref endPosRef);
			}
			if (flag || pos >= input.Length)
			{
				break;
			}
			if (pos < num)
			{
				NfaMatchingState nfaState = perThreadData.NfaState;
				nfaState.InitializeFrom(this, GetState(state.DfaStateId));
				state = new CurrentState(nfaState);
			}
			if (_checkTimeout)
			{
				CheckTimeout(timeoutOccursAt);
			}
		}
		return endPosRef;
	}

	private bool FindEndPositionDeltasDFAOptimized<TInitialStateHandler, TOptimizedNullabilityHandler>(ReadOnlySpan<char> input, int lengthMinus1, RegexRunnerMode mode, long timeoutOccursAt, ref int posRef, ref int currentStateIdRef, ref int endPosRef) where TInitialStateHandler : struct, IInitialStateHandler where TOptimizedNullabilityHandler : struct, IDfaNoZAnchorOptimizedNullabilityHandler
	{
		int pos = posRef;
		if (pos == input.Length)
		{
			if (_stateArray[currentStateIdRef].IsNullableFor(_positionKinds[0]))
			{
				endPosRef = pos;
			}
			return true;
		}
		int currentStateId = currentStateIdRef;
		int num = endPosRef;
		byte[] byteLookup = _mintermClassifier.ByteLookup;
		int deadStateId = _deadStateId;
		int initialStateId = _initialStateId;
		bool result = true;
		while (currentStateId != deadStateId)
		{
			if (TInitialStateHandler.IsOptimized && currentStateId == initialStateId)
			{
				TInitialStateHandler.TryFindNextStartingPosition(this, input, ref currentStateId, ref pos, byteLookup);
				if (pos == input.Length)
				{
					if (_stateArray[currentStateId].IsNullableFor(_positionKinds[0]))
					{
						num = pos;
					}
					currentStateId = deadStateId;
					break;
				}
			}
			char c = input[pos];
			if (TOptimizedNullabilityHandler.IsNullable(this, _nullabilityArray[currentStateId], c, byteLookup))
			{
				num = pos;
				if (mode == RegexRunnerMode.ExistenceRequired)
				{
					break;
				}
			}
			if (!DfaStateHandler.TryTakeTransition(this, ref currentStateId, GetMintermId(byteLookup, c), timeoutOccursAt) || pos >= lengthMinus1)
			{
				if (pos + 1 < input.Length)
				{
					result = false;
					break;
				}
				pos++;
				if (_stateFlagsArray[currentStateId].IsNullable() || _stateArray[currentStateId].IsNullableFor(_positionKinds[0]))
				{
					num = pos;
				}
				break;
			}
			pos++;
		}
		posRef = pos;
		endPosRef = num;
		currentStateIdRef = currentStateId;
		return result;
	}

	private bool FindEndPositionDeltasDFA<TInitialStateHandler, TNullabilityHandler>(ReadOnlySpan<char> input, int length, RegexRunnerMode mode, ref int posRef, ref CurrentState stateRef, ref int endPosRef) where TInitialStateHandler : struct, IInitialStateHandler where TNullabilityHandler : struct, INullabilityHandler
	{
		int pos = posRef;
		int num = endPosRef;
		CurrentState state = stateRef;
		int deadStateId = _deadStateId;
		int initialStateId = _initialStateId;
		bool result = true;
		while (state.DfaStateId != deadStateId && (state.DfaStateId != initialStateId || TInitialStateHandler.TryFindNextStartingPosition(this, input, ref state.DfaStateId, ref pos, null)))
		{
			int positionId = DefaultInputReader.GetPositionId(this, input, pos);
			if (TNullabilityHandler.IsNullableAt<DfaStateHandler>(this, in state, positionId))
			{
				num = pos;
				if (mode == RegexRunnerMode.ExistenceRequired)
				{
					break;
				}
			}
			if (pos >= length || !DfaStateHandler.TryTakeTransition(this, ref state.DfaStateId, positionId, 0L))
			{
				result = false;
				break;
			}
			pos++;
		}
		posRef = pos;
		endPosRef = num;
		stateRef = state;
		return result;
	}

	private bool FindEndPositionDeltasNFA<TNullabilityHandler>(ReadOnlySpan<char> input, int length, RegexRunnerMode mode, ref int posRef, ref CurrentState state, ref int endPosRef) where TNullabilityHandler : struct, INullabilityHandler
	{
		int num = posRef;
		int num2 = endPosRef;
		bool result = true;
		while (state.NfaState.NfaStateSet.Count != 0)
		{
			int positionId = DefaultInputReader.GetPositionId(this, input, num);
			if (TNullabilityHandler.IsNullableAt<NfaStateHandler>(this, in state, positionId))
			{
				num2 = num;
				if (mode == RegexRunnerMode.ExistenceRequired)
				{
					break;
				}
			}
			if (num >= length || !NfaStateHandler.TryTakeTransition(this, ref state, positionId))
			{
				break;
			}
			num++;
		}
		posRef = num;
		endPosRef = num2;
		return result;
	}

	private int FindStartPosition<TInputReader, TNullabilityHandler>(CurrentState startState, int initialLastStart, ReadOnlySpan<char> input, int i, int matchStartBoundary, PerThreadData perThreadData) where TInputReader : struct, IInputReader where TNullabilityHandler : struct, INullabilityHandler
	{
		CurrentState state = startState;
		int lastStart = initialLastStart;
		while (!((state.NfaState != null) ? FindStartPositionDeltasNFA<TInputReader, TNullabilityHandler>(input, ref i, matchStartBoundary, ref state, ref lastStart) : FindStartPositionDeltasDFA<TInputReader, TNullabilityHandler>(input, ref i, matchStartBoundary, ref state, ref lastStart)))
		{
			NfaMatchingState nfaState = perThreadData.NfaState;
			nfaState.InitializeFrom(this, GetState(state.DfaStateId));
			state = new CurrentState(nfaState);
		}
		return lastStart;
	}

	private bool FindStartPositionDeltasDFA<TInputReader, TNullabilityHandler>(ReadOnlySpan<char> input, ref int i, int startThreshold, ref CurrentState stateRef, ref int lastStart) where TInputReader : struct, IInputReader where TNullabilityHandler : struct, INullabilityHandler
	{
		int num = i;
		CurrentState state = stateRef;
		bool result = true;
		while (true)
		{
			int positionId = TInputReader.GetPositionId(this, input, num - 1);
			if (_nullabilityArray[state.DfaStateId] != 0 && TNullabilityHandler.IsNullableAt<DfaStateHandler>(this, in state, positionId))
			{
				lastStart = num;
			}
			if (num <= startThreshold || state.DfaStateId == _deadStateId)
			{
				break;
			}
			if (!DfaStateHandler.TryTakeTransition(this, ref state.DfaStateId, positionId, 0L))
			{
				result = false;
				break;
			}
			num--;
		}
		stateRef = state;
		i = num;
		return result;
	}

	private bool FindStartPositionDeltasNFA<TInputReader, TNullabilityHandler>(ReadOnlySpan<char> input, ref int i, int startThreshold, ref CurrentState state, ref int lastStart) where TInputReader : struct, IInputReader where TNullabilityHandler : struct, INullabilityHandler
	{
		int num = i;
		bool result = true;
		while (true)
		{
			int positionId = TInputReader.GetPositionId(this, input, num - 1);
			if (TNullabilityHandler.IsNullableAt<NfaStateHandler>(this, in state, positionId))
			{
				lastStart = num;
			}
			if (num <= startThreshold || state.DfaStateId == _deadStateId)
			{
				break;
			}
			if (!NfaStateHandler.TryTakeTransition(this, ref state, positionId))
			{
				result = false;
				break;
			}
			num--;
		}
		i = num;
		return result;
	}

	private Registers FindSubcaptures<TInputReader>(ReadOnlySpan<char> input, int i, int iEnd, PerThreadData perThreadData) where TInputReader : struct, IInputReader
	{
		MatchingState<TSet> matchingState = _initialStates[GetCharKind(input, i - 1)];
		Registers initialRegisters = perThreadData.InitialRegisters;
		Array.Fill(initialRegisters.CaptureStarts, -1);
		Array.Fill(initialRegisters.CaptureEnds, -1);
		SparseIntMap<Registers> sparseIntMap = perThreadData.Current;
		SparseIntMap<Registers> sparseIntMap2 = perThreadData.Next;
		sparseIntMap.Clear();
		sparseIntMap2.Clear();
		ForEachNfaState(matchingState.Node, matchingState.PrevCharKind, (sparseIntMap, initialRegisters), delegate(int nfaId, (SparseIntMap<Registers> Current, Registers InitialRegisters) args)
		{
			args.Current.Add(nfaId, args.InitialRegisters.Clone());
		});
		int key;
		Registers value;
		while ((uint)i < (uint)iEnd)
		{
			int positionId = TInputReader.GetPositionId(this, input, i);
			foreach (KeyValuePair<int, Registers> value3 in sparseIntMap.Values)
			{
				value3.Deconstruct(out key, out value);
				int num = key;
				Registers registers = value;
				int num2 = DeltaOffset(num, positionId);
				(int, DerivativeEffect[])[] array = _capturingNfaDelta[num2] ?? CreateNewCapturingTransition(num, positionId, num2);
				for (int num3 = 0; num3 < array.Length; num3++)
				{
					var (num4, effects) = array[num3];
					if (sparseIntMap2.Add(num4, out var index))
					{
						Registers value2 = ((num3 != array.Length - 1) ? registers.Clone() : registers);
						value2.ApplyEffects(effects, i);
						sparseIntMap2.Update(index, num4, value2);
						int coreStateId = GetCoreStateId(num4);
						StateFlags info = _stateFlagsArray[coreStateId];
						if (info.IsNullable() || (info.CanBeNullable() && GetState(coreStateId).IsNullableFor(GetCharKind(input, i + 1))))
						{
							goto end_IL_019e;
						}
					}
				}
				continue;
				end_IL_019e:
				break;
			}
			SparseIntMap<Registers> sparseIntMap3 = sparseIntMap;
			sparseIntMap = sparseIntMap2;
			sparseIntMap2 = sparseIntMap3;
			sparseIntMap2.Clear();
			i++;
		}
		foreach (KeyValuePair<int, Registers> value4 in sparseIntMap.Values)
		{
			value4.Deconstruct(out key, out value);
			int nfaStateId = key;
			Registers registers2 = value;
			MatchingState<TSet> state = GetState(GetCoreStateId(nfaStateId));
			if (state.IsNullableFor(GetCharKind(input, iEnd)))
			{
				state.Node.ApplyEffects(delegate(DerivativeEffect effect, (Registers Registers, int Pos) args)
				{
					args.Registers.ApplyEffect(effect, args.Pos);
				}, CharKind.Context(state.PrevCharKind, GetCharKind(input, iEnd)), (registers2, iEnd));
				return registers2;
			}
		}
		return default(Registers);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMintermId(byte[] mintermLookup, char c)
	{
		if ((uint)c >= (uint)mintermLookup.Length)
		{
			return 0;
		}
		return mintermLookup[(uint)c];
	}
}
internal abstract class SymbolicRegexMatcher
{
}
