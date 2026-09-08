using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace System;

public static class MemoryExtensions
{
	public ref struct SpanSplitEnumerator<T> : IEnumerator<Range>, IDisposable, IEnumerator where T : IEquatable<T>
	{
		private readonly ReadOnlySpan<T> _source;

		private readonly T _separator;

		private readonly ReadOnlySpan<T> _separatorBuffer;

		private readonly SearchValues<T> _searchValues;

		private SpanSplitEnumeratorMode _splitMode;

		private int _startCurrent;

		private int _endCurrent;

		private int _startNext;

		public readonly ReadOnlySpan<T> Source => _source;

		public Range Current => _startCurrent.._endCurrent;

		object IEnumerator.Current => Current;

		public SpanSplitEnumerator<T> GetEnumerator()
		{
			return this;
		}

		internal SpanSplitEnumerator(ReadOnlySpan<T> source, SearchValues<T> searchValues)
		{
			_separatorBuffer = default(ReadOnlySpan<T>);
			_separator = default(T);
			_searchValues = null;
			_startCurrent = 0;
			_endCurrent = 0;
			_startNext = 0;
			_source = source;
			_splitMode = SpanSplitEnumeratorMode.SearchValues;
			_searchValues = searchValues;
		}

		internal SpanSplitEnumerator(ReadOnlySpan<T> source, ReadOnlySpan<T> separators)
		{
			_separatorBuffer = default(ReadOnlySpan<T>);
			_separator = default(T);
			_searchValues = null;
			_startCurrent = 0;
			_endCurrent = 0;
			_startNext = 0;
			_source = source;
			if (typeof(T) == typeof(char) && separators.Length == 0)
			{
				_searchValues = Unsafe.As<SearchValues<T>>(string.SearchValuesStorage.WhiteSpaceChars);
				_splitMode = SpanSplitEnumeratorMode.SearchValues;
			}
			else
			{
				_separatorBuffer = separators;
				_splitMode = SpanSplitEnumeratorMode.Any;
			}
		}

		internal SpanSplitEnumerator(ReadOnlySpan<T> source, ReadOnlySpan<T> separator, bool treatAsSingleSeparator)
		{
			_separator = default(T);
			_searchValues = null;
			_startCurrent = 0;
			_endCurrent = 0;
			_startNext = 0;
			_source = source;
			_separatorBuffer = separator;
			_splitMode = ((separator.Length == 0) ? SpanSplitEnumeratorMode.EmptySequence : SpanSplitEnumeratorMode.Sequence);
		}

		internal SpanSplitEnumerator(ReadOnlySpan<T> source, T separator)
		{
			_separatorBuffer = default(ReadOnlySpan<T>);
			_separator = default(T);
			_searchValues = null;
			_startCurrent = 0;
			_endCurrent = 0;
			_startNext = 0;
			_source = source;
			_separator = separator;
			_splitMode = SpanSplitEnumeratorMode.SingleElement;
		}

		public bool MoveNext()
		{
			int num;
			int num2;
			switch (_splitMode)
			{
			case SpanSplitEnumeratorMode.None:
				return false;
			case SpanSplitEnumeratorMode.SingleElement:
				num = _source.Slice(_startNext).IndexOf(_separator);
				num2 = 1;
				break;
			case SpanSplitEnumeratorMode.Any:
				num = _source.Slice(_startNext).IndexOfAny(_separatorBuffer);
				num2 = 1;
				break;
			case SpanSplitEnumeratorMode.Sequence:
				num = _source.Slice(_startNext).IndexOf(_separatorBuffer);
				num2 = _separatorBuffer.Length;
				break;
			case SpanSplitEnumeratorMode.EmptySequence:
				num = -1;
				num2 = 1;
				break;
			default:
				num = _source.Slice(_startNext).IndexOfAny(_searchValues);
				num2 = 1;
				break;
			}
			_startCurrent = _startNext;
			if (num >= 0)
			{
				_endCurrent = _startCurrent + num;
				_startNext = _endCurrent + num2;
			}
			else
			{
				_startNext = (_endCurrent = _source.Length);
				_splitMode = SpanSplitEnumeratorMode.None;
			}
			return true;
		}

		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		void IDisposable.Dispose()
		{
		}
	}

	private enum SpanSplitEnumeratorMode
	{
		None,
		SingleElement,
		Any,
		Sequence,
		EmptySequence,
		SearchValues
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[InterpolatedStringHandler]
	public ref struct TryWriteInterpolatedStringHandler
	{
		private readonly Span<char> _destination;

		private readonly IFormatProvider _provider;

		internal int _pos;

		internal bool _success;

		private readonly bool _hasCustomFormatter;

		public TryWriteInterpolatedStringHandler(int literalLength, int formattedCount, Span<char> destination, out bool shouldAppend)
		{
			_destination = destination;
			_provider = null;
			_pos = 0;
			_success = (shouldAppend = destination.Length >= literalLength);
			_hasCustomFormatter = false;
		}

		public TryWriteInterpolatedStringHandler(int literalLength, int formattedCount, Span<char> destination, IFormatProvider? provider, out bool shouldAppend)
		{
			_destination = destination;
			_provider = provider;
			_pos = 0;
			_success = (shouldAppend = destination.Length >= literalLength);
			_hasCustomFormatter = provider != null && DefaultInterpolatedStringHandler.HasCustomFormatter(provider);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendLiteral(string value)
		{
			if (value.TryCopyTo(_destination.Slice(_pos)))
			{
				_pos += value.Length;
				return true;
			}
			return Fail();
		}

		public bool AppendFormatted<T>(T value)
		{
			if (_hasCustomFormatter)
			{
				return AppendCustomFormatter(value, null);
			}
			if (value == null)
			{
				return true;
			}
			string text;
			if (value is IFormattable)
			{
				if (typeof(T).IsEnum)
				{
					if (Enum.TryFormatUnconstrained(value, _destination.Slice(_pos), out var charsWritten))
					{
						_pos += charsWritten;
						return true;
					}
					return Fail();
				}
				if (value is ISpanFormattable)
				{
					if (((ISpanFormattable)(object)value).TryFormat(_destination.Slice(_pos), out var charsWritten2, default(ReadOnlySpan<char>), _provider))
					{
						_pos += charsWritten2;
						return true;
					}
					return Fail();
				}
				text = ((IFormattable)(object)value).ToString(null, _provider);
			}
			else
			{
				text = value.ToString();
			}
			if (text != null)
			{
				return AppendLiteral(text);
			}
			return true;
		}

		public bool AppendFormatted<T>(T value, string? format)
		{
			if (_hasCustomFormatter)
			{
				return AppendCustomFormatter(value, format);
			}
			if (value == null)
			{
				return true;
			}
			string text;
			if (value is IFormattable)
			{
				if (typeof(T).IsEnum)
				{
					if (Enum.TryFormatUnconstrained(value, _destination.Slice(_pos), out var charsWritten, format.AsSpan()))
					{
						_pos += charsWritten;
						return true;
					}
					return Fail();
				}
				if (value is ISpanFormattable)
				{
					if (((ISpanFormattable)(object)value).TryFormat(_destination.Slice(_pos), out var charsWritten2, format.AsSpan(), _provider))
					{
						_pos += charsWritten2;
						return true;
					}
					return Fail();
				}
				text = ((IFormattable)(object)value).ToString(format, _provider);
			}
			else
			{
				text = value.ToString();
			}
			if (text != null)
			{
				return AppendLiteral(text);
			}
			return true;
		}

		public bool AppendFormatted<T>(T value, int alignment)
		{
			int pos = _pos;
			if (AppendFormatted(value))
			{
				if (alignment != 0)
				{
					return TryAppendOrInsertAlignmentIfNeeded(pos, alignment);
				}
				return true;
			}
			return Fail();
		}

		public bool AppendFormatted<T>(T value, int alignment, string? format)
		{
			int pos = _pos;
			if (AppendFormatted(value, format))
			{
				if (alignment != 0)
				{
					return TryAppendOrInsertAlignmentIfNeeded(pos, alignment);
				}
				return true;
			}
			return Fail();
		}

		public bool AppendFormatted(scoped ReadOnlySpan<char> value)
		{
			if (value.TryCopyTo(_destination.Slice(_pos)))
			{
				_pos += value.Length;
				return true;
			}
			return Fail();
		}

		public bool AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
		{
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num = alignment - value.Length;
			if (num <= 0)
			{
				return AppendFormatted(value);
			}
			if (alignment <= _destination.Length - _pos)
			{
				if (flag)
				{
					value.CopyTo(_destination.Slice(_pos));
					_pos += value.Length;
					_destination.Slice(_pos, num).Fill(' ');
					_pos += num;
				}
				else
				{
					_destination.Slice(_pos, num).Fill(' ');
					_pos += num;
					value.CopyTo(_destination.Slice(_pos));
					_pos += value.Length;
				}
				return true;
			}
			return Fail();
		}

		public bool AppendFormatted(string? value)
		{
			if (_hasCustomFormatter)
			{
				return AppendCustomFormatter(value, null);
			}
			if (value == null)
			{
				return true;
			}
			if (value.TryCopyTo(_destination.Slice(_pos)))
			{
				_pos += value.Length;
				return true;
			}
			return Fail();
		}

		public bool AppendFormatted(string? value, int alignment = 0, string? format = null)
		{
			return this.AppendFormatted<string>(value, alignment, format);
		}

		public bool AppendFormatted(object? value, int alignment = 0, string? format = null)
		{
			return this.AppendFormatted<object>(value, alignment, format);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool AppendCustomFormatter<T>(T value, string format)
		{
			ICustomFormatter customFormatter = (ICustomFormatter)_provider.GetFormat(typeof(ICustomFormatter));
			if (customFormatter != null)
			{
				string text = customFormatter.Format(format, value, _provider);
				if (text != null)
				{
					return AppendLiteral(text);
				}
			}
			return true;
		}

		private bool TryAppendOrInsertAlignmentIfNeeded(int startingPos, int alignment)
		{
			int num = _pos - startingPos;
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num2 = alignment - num;
			if (num2 <= 0)
			{
				return true;
			}
			if (num2 <= _destination.Length - _pos)
			{
				if (flag)
				{
					_destination.Slice(_pos, num2).Fill(' ');
				}
				else
				{
					_destination.Slice(startingPos, num).CopyTo(_destination.Slice(startingPos + num2));
					_destination.Slice(startingPos, num2).Fill(' ');
				}
				_pos += num2;
				return true;
			}
			return Fail();
		}

		private bool Fail()
		{
			_success = false;
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this T[]? array, int start)
	{
		if (array == null)
		{
			if (start != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return default(Span<T>);
		}
		if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
		{
			ThrowHelper.ThrowArrayTypeMismatchException();
		}
		if ((uint)start > (uint)array.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException();
		}
		return new Span<T>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)start), array.Length - start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this T[]? array, Index startIndex)
	{
		if (array == null)
		{
			if (!startIndex.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			return default(Span<T>);
		}
		if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
		{
			ThrowHelper.ThrowArrayTypeMismatchException();
		}
		int offset = startIndex.GetOffset(array.Length);
		if ((uint)offset > (uint)array.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException();
		}
		return new Span<T>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)offset), array.Length - offset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this T[]? array, Range range)
	{
		if (array == null)
		{
			Index start = range.Start;
			Index end = range.End;
			if (!start.Equals(Index.Start) || !end.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			return default(Span<T>);
		}
		if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
		{
			ThrowHelper.ThrowArrayTypeMismatchException();
		}
		var (num, length) = range.GetOffsetAndLength(array.Length);
		return new Span<T>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)num), length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static ReadOnlySpan<char> AsSpan(this string? text)
	{
		if (text == null)
		{
			return default(ReadOnlySpan<char>);
		}
		return new ReadOnlySpan<char>(ref text.GetRawStringData(), text.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> AsSpan(this string? text, int start)
	{
		if (text == null)
		{
			if (start != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return default(ReadOnlySpan<char>);
		}
		if ((uint)start > (uint)text.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		return new ReadOnlySpan<char>(ref Unsafe.Add(ref text.GetRawStringData(), (nint)(uint)start), text.Length - start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> AsSpan(this string? text, Index startIndex)
	{
		if (text == null)
		{
			if (!startIndex.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex);
			}
			return default(ReadOnlySpan<char>);
		}
		int offset = startIndex.GetOffset(text.Length);
		if ((uint)offset > (uint)text.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex);
		}
		return new ReadOnlySpan<char>(ref Unsafe.Add(ref text.GetRawStringData(), (nint)(uint)offset), text.Length - offset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> AsSpan(this string? text, Range range)
	{
		if (text == null)
		{
			Index start = range.Start;
			Index end = range.End;
			if (!start.Equals(Index.Start) || !end.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
			}
			return default(ReadOnlySpan<char>);
		}
		var (num, length) = range.GetOffsetAndLength(text.Length);
		return new ReadOnlySpan<char>(ref Unsafe.Add(ref text.GetRawStringData(), (nint)(uint)num), length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> AsSpan(this string? text, int start, int length)
	{
		if (text == null)
		{
			if (start != 0 || length != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return default(ReadOnlySpan<char>);
		}
		if ((ulong)((long)(uint)start + (long)(uint)length) > (ulong)(uint)text.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		return new ReadOnlySpan<char>(ref Unsafe.Add(ref text.GetRawStringData(), (nint)(uint)start), length);
	}

	public static ReadOnlyMemory<char> AsMemory(this string? text)
	{
		if (text == null)
		{
			return default(ReadOnlyMemory<char>);
		}
		return new ReadOnlyMemory<char>(text, 0, text.Length);
	}

	public static ReadOnlyMemory<char> AsMemory(this string? text, int start)
	{
		if (text == null)
		{
			if (start != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return default(ReadOnlyMemory<char>);
		}
		if ((uint)start > (uint)text.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		return new ReadOnlyMemory<char>(text, start, text.Length - start);
	}

	public static ReadOnlyMemory<char> AsMemory(this string? text, Index startIndex)
	{
		if (text == null)
		{
			if (!startIndex.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
			}
			return default(ReadOnlyMemory<char>);
		}
		int offset = startIndex.GetOffset(text.Length);
		if ((uint)offset > (uint)text.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException();
		}
		return new ReadOnlyMemory<char>(text, offset, text.Length - offset);
	}

	public static ReadOnlyMemory<char> AsMemory(this string? text, int start, int length)
	{
		if (text == null)
		{
			if (start != 0 || length != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return default(ReadOnlyMemory<char>);
		}
		if ((ulong)((long)(uint)start + (long)(uint)length) > (ulong)(uint)text.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		return new ReadOnlyMemory<char>(text, start, length);
	}

	public static ReadOnlyMemory<char> AsMemory(this string? text, Range range)
	{
		if (text == null)
		{
			Index start = range.Start;
			Index end = range.End;
			if (!start.Equals(Index.Start) || !end.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
			}
			return default(ReadOnlyMemory<char>);
		}
		var (start2, length) = range.GetOffsetAndLength(text.Length);
		return new ReadOnlyMemory<char>(text, start2, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool Contains<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).Contains(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.ContainsValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.ContainsValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return SpanHelpers.ContainsValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return SpanHelpers.ContainsValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
			}
		}
		return SpanHelpers.Contains(ref MemoryMarshal.GetReference(span), value, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOf(value, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAny<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAny(value0, value1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAny<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAny(value0, value1, value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAny<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAny<T>(this Span<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAny(this Span<char> span, SearchValues<string> values)
	{
		return ((ReadOnlySpan<char>)span).ContainsAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyExcept<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyExcept(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyExcept<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyExcept(value0, value1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyExcept<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyExcept(value0, value1, value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyExcept<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyExcept(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyExcept<T>(this Span<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyExcept(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyInRange<T>(this Span<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyInRange(lowInclusive, highInclusive);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool ContainsAnyExceptInRange<T>(this Span<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return ((ReadOnlySpan<T>)span).ContainsAnyExceptInRange(lowInclusive, highInclusive);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return span.IndexOfAny(value0, value1) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, T value0, T value1, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAny(value0, value1, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return span.IndexOfAny(value0, value1, value2) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAny(value0, value1, value2, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return span.IndexOfAny(values) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAny(values, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.ContainsAny(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAny(this ReadOnlySpan<char> span, SearchValues<string> values)
	{
		return span.IndexOfAny(values) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		return span.IndexOfAnyExcept(value) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAnyExcept(value, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return span.IndexOfAnyExcept(value0, value1) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAnyExcept(value0, value1, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return span.IndexOfAnyExcept(value0, value1, value2) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAnyExcept(value0, value1, value2, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return span.IndexOfAnyExcept(values) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		return span.IndexOfAnyExcept(values, comparer) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExcept<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.ContainsAnyExcept(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyInRange<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return span.IndexOfAnyInRange(lowInclusive, highInclusive) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ContainsAnyExceptInRange<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return span.IndexOfAnyExceptInRange(lowInclusive, highInclusive) >= 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOf<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOf(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOf(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOf<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOf(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOf<T>(this Span<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOf(value);
	}

	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyExcept<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyExcept(value);
	}

	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyExcept<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyExcept(value0, value1);
	}

	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyExcept<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyExcept(value0, value1, value2);
	}

	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyExcept<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyExcept(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyExcept<T>(this Span<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyExcept(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
			}
		}
		return SpanHelpers.IndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
				}
			}
			return IndexOfAnyExceptDefaultComparer(span, value);
		}
		return IndexOfAnyExceptComparer(span, value, comparer);
		static int IndexOfAnyExceptComparer(ReadOnlySpan<T> readOnlySpan, T y, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (!equalityComparer.Equals(readOnlySpan[i], y))
				{
					return i;
				}
			}
			return -1;
		}
		static int IndexOfAnyExceptDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (!EqualityComparer<T>.Default.Equals(readOnlySpan[i], y))
				{
					return i;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
			}
		}
		return SpanHelpers.IndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value0, value1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
				}
			}
			return IndexOfAnyExceptDefaultComparer(span, value0, value1);
		}
		return IndexOfAnyExceptComparer(span, value0, value1, comparer);
		static int IndexOfAnyExceptComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (!equalityComparer.Equals(readOnlySpan[i], y) && !equalityComparer.Equals(readOnlySpan[i], y2))
				{
					return i;
				}
			}
			return -1;
		}
		static int IndexOfAnyExceptDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (!EqualityComparer<T>.Default.Equals(readOnlySpan[i], y) && !EqualityComparer<T>.Default.Equals(readOnlySpan[i], y2))
				{
					return i;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
			}
		}
		return SpanHelpers.IndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value0, value1, value2, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
				}
			}
			return IndexOfAnyExceptDefaultComparer(span, value0, value1, value2);
		}
		return IndexOfAnyExceptComparer(span, value0, value1, value2, comparer);
		static int IndexOfAnyExceptComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (!equalityComparer.Equals(readOnlySpan[i], y) && !equalityComparer.Equals(readOnlySpan[i], y2) && !equalityComparer.Equals(readOnlySpan[i], y3))
				{
					return i;
				}
			}
			return -1;
		}
		static int IndexOfAnyExceptDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (!EqualityComparer<T>.Default.Equals(readOnlySpan[i], y) && !EqualityComparer<T>.Default.Equals(readOnlySpan[i], y2) && !EqualityComparer<T>.Default.Equals(readOnlySpan[i], y3))
				{
					return i;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, T value3) where T : IEquatable<T>
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), Unsafe.BitCast<T, byte>(value3), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), Unsafe.BitCast<T, short>(value3), span.Length);
			}
		}
		return SpanHelpers.IndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value0, value1, value2, value3, span.Length);
	}

	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		switch (values.Length)
		{
		case 0:
			if (!span.IsEmpty)
			{
				return 0;
			}
			return -1;
		case 1:
			return span.IndexOfAnyExcept(values[0]);
		case 2:
			return span.IndexOfAnyExcept(values[0], values[1]);
		case 3:
			return span.IndexOfAnyExcept(values[0], values[1], values[2]);
		case 4:
			return span.IndexOfAnyExcept<T>(values[0], values[1], values[2], values[3]);
		default:
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1 && values.Length == 5)
				{
					ref byte reference = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values));
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), Unsafe.Add(ref reference, 3), Unsafe.Add(ref reference, 4), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2 && values.Length == 5)
				{
					ref short reference2 = ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(values));
					return SpanHelpers.IndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), reference2, Unsafe.Add(ref reference2, 1), Unsafe.Add(ref reference2, 2), Unsafe.Add(ref reference2, 3), Unsafe.Add(ref reference2, 4), span.Length);
				}
			}
			if (RuntimeHelpers.IsBitwiseEquatable<T>() && Unsafe.SizeOf<T>() == 2)
			{
				return ProbabilisticMap.IndexOfAnyExcept(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(values)), values.Length);
			}
			for (int i = 0; i < span.Length; i++)
			{
				if (!values.Contains(span[i]))
				{
					return i;
				}
			}
			return -1;
		}
		}
	}

	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		switch (values.Length)
		{
		case 0:
			if (!span.IsEmpty)
			{
				return 0;
			}
			return -1;
		case 1:
			return span.IndexOfAnyExcept(values[0], comparer);
		case 2:
			return span.IndexOfAnyExcept(values[0], values[1], comparer);
		case 3:
			return span.IndexOfAnyExcept(values[0], values[1], values[2], comparer);
		default:
		{
			for (int i = 0; i < span.Length; i++)
			{
				if (!values.Contains(span[i], comparer))
				{
					return i;
				}
			}
			return -1;
		}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.IndexOfAnyExcept(span);
	}

	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyExcept<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyExcept(value);
	}

	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyExcept<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyExcept(value0, value1);
	}

	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyExcept<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyExcept(value0, value1, value2);
	}

	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyExcept<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyExcept(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyExcept<T>(this Span<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyExcept(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
				}
			}
			return LastIndexOfAnyExceptDefaultComparer(span, value);
		}
		return LastIndexOfAnyExceptComparer(span, value, comparer);
		static int LastIndexOfAnyExceptComparer(ReadOnlySpan<T> readOnlySpan, T y, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (!equalityComparer.Equals(readOnlySpan[num], y))
				{
					return num;
				}
			}
			return -1;
		}
		static int LastIndexOfAnyExceptDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y)
		{
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (!EqualityComparer<T>.Default.Equals(readOnlySpan[num], y))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value0, value1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
				}
			}
			return LastIndexOfAnyExceptDefaultComparer(span, value0, value1);
		}
		return LastIndexOfAnyExceptComparer(span, value0, value1, comparer);
		static int LastIndexOfAnyExceptComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (!equalityComparer.Equals(readOnlySpan[num], y) && !equalityComparer.Equals(readOnlySpan[num], y2))
				{
					return num;
				}
			}
			return -1;
		}
		static int LastIndexOfAnyExceptDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2)
		{
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (!EqualityComparer<T>.Default.Equals(readOnlySpan[num], y) && !EqualityComparer<T>.Default.Equals(readOnlySpan[num], y2))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value0, value1, value2, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
				}
			}
			return LastIndexOfAnyExceptDefaultComparer(span, value0, value1, value2);
		}
		return LastIndexOfAnyExceptComparer(span, value0, value1, value2, comparer);
		static int LastIndexOfAnyExceptComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (!equalityComparer.Equals(readOnlySpan[num], y) && !equalityComparer.Equals(readOnlySpan[num], y2) && !equalityComparer.Equals(readOnlySpan[num], y3))
				{
					return num;
				}
			}
			return -1;
		}
		static int LastIndexOfAnyExceptDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3)
		{
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (!EqualityComparer<T>.Default.Equals(readOnlySpan[num], y) && !EqualityComparer<T>.Default.Equals(readOnlySpan[num], y2) && !EqualityComparer<T>.Default.Equals(readOnlySpan[num], y3))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, T value3) where T : IEquatable<T>
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), Unsafe.BitCast<T, byte>(value3), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), Unsafe.BitCast<T, short>(value3), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAnyExcept(ref MemoryMarshal.GetReference(span), value0, value1, value2, value3, span.Length);
	}

	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		switch (values.Length)
		{
		case 0:
			return span.Length - 1;
		case 1:
			return span.LastIndexOfAnyExcept(values[0]);
		case 2:
			return span.LastIndexOfAnyExcept(values[0], values[1]);
		case 3:
			return span.LastIndexOfAnyExcept(values[0], values[1], values[2]);
		case 4:
			return span.LastIndexOfAnyExcept<T>(values[0], values[1], values[2], values[3]);
		default:
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1 && values.Length == 5)
				{
					ref byte reference = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values));
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), Unsafe.Add(ref reference, 3), Unsafe.Add(ref reference, 4), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2 && values.Length == 5)
				{
					ref short reference2 = ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(values));
					return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), reference2, Unsafe.Add(ref reference2, 1), Unsafe.Add(ref reference2, 2), Unsafe.Add(ref reference2, 3), Unsafe.Add(ref reference2, 4), span.Length);
				}
			}
			if (RuntimeHelpers.IsBitwiseEquatable<T>() && Unsafe.SizeOf<T>() == 2)
			{
				return ProbabilisticMap.LastIndexOfAnyExcept(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(values)), values.Length);
			}
			for (int num = span.Length - 1; num >= 0; num--)
			{
				if (!values.Contains(span[num]))
				{
					return num;
				}
			}
			return -1;
		}
		}
	}

	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		switch (values.Length)
		{
		case 0:
			return span.Length - 1;
		case 1:
			return span.LastIndexOfAnyExcept(values[0], comparer);
		case 2:
			return span.LastIndexOfAnyExcept(values[0], values[1], comparer);
		case 3:
			return span.LastIndexOfAnyExcept(values[0], values[1], values[2], comparer);
		default:
		{
			for (int num = span.Length - 1; num >= 0; num--)
			{
				if (!values.Contains(span[num], comparer))
				{
					return num;
				}
			}
			return -1;
		}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExcept<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.LastIndexOfAnyExcept(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyInRange<T>(this Span<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyInRange(lowInclusive, highInclusive);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyInRange<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		if (lowInclusive == null || highInclusive == null)
		{
			ThrowNullLowHighInclusive<T>(lowInclusive, highInclusive);
		}
		if (Vector128.IsHardwareAccelerated)
		{
			if ((lowInclusive is byte || lowInclusive is sbyte) ? true : false)
			{
				return SpanHelpers.IndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(lowInclusive), Unsafe.BitCast<T, byte>(highInclusive), span.Length);
			}
			if ((lowInclusive is short || lowInclusive is ushort || lowInclusive is char) ? true : false)
			{
				return SpanHelpers.IndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ushort>(lowInclusive), Unsafe.BitCast<T, ushort>(highInclusive), span.Length);
			}
			bool flag = ((lowInclusive is int || lowInclusive is uint) ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = 8 == 4;
				if (flag3)
				{
				}
				flag2 = flag3;
			}
			if (flag2)
			{
				return SpanHelpers.IndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, uint>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, uint>(lowInclusive), Unsafe.BitCast<T, uint>(highInclusive), span.Length);
			}
			flag2 = ((lowInclusive is long || lowInclusive is ulong) ? true : false);
			flag = flag2;
			if (!flag)
			{
				bool flag3 = 8 == 8;
				bool flag4 = ((lowInclusive is nint || lowInclusive is nuint) ? true : false);
				flag3 = flag4;
				flag = flag3;
			}
			if (flag)
			{
				return SpanHelpers.IndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ulong>(lowInclusive), Unsafe.BitCast<T, ulong>(highInclusive), span.Length);
			}
		}
		return SpanHelpers.IndexOfAnyInRange(ref MemoryMarshal.GetReference(span), lowInclusive, highInclusive, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAnyExceptInRange<T>(this Span<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return ((ReadOnlySpan<T>)span).IndexOfAnyExceptInRange(lowInclusive, highInclusive);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAnyExceptInRange<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		if (lowInclusive == null || highInclusive == null)
		{
			ThrowNullLowHighInclusive<T>(lowInclusive, highInclusive);
		}
		if (Vector128.IsHardwareAccelerated)
		{
			if ((lowInclusive is byte || lowInclusive is sbyte) ? true : false)
			{
				return SpanHelpers.IndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(lowInclusive), Unsafe.BitCast<T, byte>(highInclusive), span.Length);
			}
			if ((lowInclusive is short || lowInclusive is ushort || lowInclusive is char) ? true : false)
			{
				return SpanHelpers.IndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ushort>(lowInclusive), Unsafe.BitCast<T, ushort>(highInclusive), span.Length);
			}
			bool flag = ((lowInclusive is int || lowInclusive is uint) ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = 8 == 4;
				if (flag3)
				{
				}
				flag2 = flag3;
			}
			if (flag2)
			{
				return SpanHelpers.IndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, uint>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, uint>(lowInclusive), Unsafe.BitCast<T, uint>(highInclusive), span.Length);
			}
			flag2 = ((lowInclusive is long || lowInclusive is ulong) ? true : false);
			flag = flag2;
			if (!flag)
			{
				bool flag3 = 8 == 8;
				bool flag4 = ((lowInclusive is nint || lowInclusive is nuint) ? true : false);
				flag3 = flag4;
				flag = flag3;
			}
			if (flag)
			{
				return SpanHelpers.IndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ulong>(lowInclusive), Unsafe.BitCast<T, ulong>(highInclusive), span.Length);
			}
		}
		return SpanHelpers.IndexOfAnyExceptInRange(ref MemoryMarshal.GetReference(span), lowInclusive, highInclusive, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyInRange<T>(this Span<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyInRange(lowInclusive, highInclusive);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyInRange<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		if (lowInclusive == null || highInclusive == null)
		{
			ThrowNullLowHighInclusive<T>(lowInclusive, highInclusive);
		}
		if (Vector128.IsHardwareAccelerated)
		{
			if ((lowInclusive is byte || lowInclusive is sbyte) ? true : false)
			{
				return SpanHelpers.LastIndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(lowInclusive), Unsafe.BitCast<T, byte>(highInclusive), span.Length);
			}
			if ((lowInclusive is short || lowInclusive is ushort || lowInclusive is char) ? true : false)
			{
				return SpanHelpers.LastIndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ushort>(lowInclusive), Unsafe.BitCast<T, ushort>(highInclusive), span.Length);
			}
			bool flag = ((lowInclusive is int || lowInclusive is uint) ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = 8 == 4;
				if (flag3)
				{
				}
				flag2 = flag3;
			}
			if (flag2)
			{
				return SpanHelpers.LastIndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, uint>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, uint>(lowInclusive), Unsafe.BitCast<T, uint>(highInclusive), span.Length);
			}
			flag2 = ((lowInclusive is long || lowInclusive is ulong) ? true : false);
			flag = flag2;
			if (!flag)
			{
				bool flag3 = 8 == 8;
				bool flag4 = ((lowInclusive is nint || lowInclusive is nuint) ? true : false);
				flag3 = flag4;
				flag = flag3;
			}
			if (flag)
			{
				return SpanHelpers.LastIndexOfAnyInRangeUnsignedNumber(ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ulong>(lowInclusive), Unsafe.BitCast<T, ulong>(highInclusive), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAnyInRange(ref MemoryMarshal.GetReference(span), lowInclusive, highInclusive, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAnyExceptInRange<T>(this Span<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAnyExceptInRange(lowInclusive, highInclusive);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAnyExceptInRange<T>(this ReadOnlySpan<T> span, T lowInclusive, T highInclusive) where T : IComparable<T>
	{
		if (lowInclusive == null || highInclusive == null)
		{
			ThrowNullLowHighInclusive<T>(lowInclusive, highInclusive);
		}
		if (Vector128.IsHardwareAccelerated)
		{
			if ((lowInclusive is byte || lowInclusive is sbyte) ? true : false)
			{
				return SpanHelpers.LastIndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(lowInclusive), Unsafe.BitCast<T, byte>(highInclusive), span.Length);
			}
			if ((lowInclusive is short || lowInclusive is ushort || lowInclusive is char) ? true : false)
			{
				return SpanHelpers.LastIndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ushort>(lowInclusive), Unsafe.BitCast<T, ushort>(highInclusive), span.Length);
			}
			bool flag = ((lowInclusive is int || lowInclusive is uint) ? true : false);
			bool flag2 = flag;
			if (!flag2)
			{
				bool flag3 = 8 == 4;
				if (flag3)
				{
				}
				flag2 = flag3;
			}
			if (flag2)
			{
				return SpanHelpers.LastIndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, uint>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, uint>(lowInclusive), Unsafe.BitCast<T, uint>(highInclusive), span.Length);
			}
			flag2 = ((lowInclusive is long || lowInclusive is ulong) ? true : false);
			flag = flag2;
			if (!flag)
			{
				bool flag3 = 8 == 8;
				bool flag4 = ((lowInclusive is nint || lowInclusive is nuint) ? true : false);
				flag3 = flag4;
				flag = flag3;
			}
			if (flag)
			{
				return SpanHelpers.LastIndexOfAnyExceptInRangeUnsignedNumber(ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, ulong>(lowInclusive), Unsafe.BitCast<T, ulong>(highInclusive), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAnyExceptInRange(ref MemoryMarshal.GetReference(span), lowInclusive, highInclusive, span.Length);
	}

	[DoesNotReturn]
	private static void ThrowNullLowHighInclusive<T>(T lowInclusive, T highInclusive)
	{
		throw new ArgumentNullException((lowInclusive == null) ? "lowInclusive" : "highInclusive");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[OverloadResolutionPriority(-1)]
	public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).SequenceEqual(other);
	}

	[OverloadResolutionPriority(-1)]
	public static int SequenceCompareTo<T>(this Span<T> span, ReadOnlySpan<T> other) where T : IComparable<T>?
	{
		return ((ReadOnlySpan<T>)span).SequenceCompareTo(other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOf<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
			}
		}
		return SpanHelpers.IndexOf(ref MemoryMarshal.GetReference(span), value, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					return SpanHelpers.IndexOfValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
				}
			}
			return IndexOfDefaultComparer(span, value);
		}
		return IndexOfComparer(span, value, comparer);
		static int IndexOfComparer(ReadOnlySpan<T> readOnlySpan, T y, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (equalityComparer.Equals(readOnlySpan[i], y))
				{
					return i;
				}
			}
			return -1;
		}
		static int IndexOfDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[i], y))
				{
					return i;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOf(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOf(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
		}
		return SpanHelpers.IndexOf(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(value), value.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>() && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOf(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOf(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
		}
		return IndexOfComparer(span, value, comparer);
		static int IndexOfComparer(ReadOnlySpan<T> span2, ReadOnlySpan<T> readOnlySpan, IEqualityComparer<T> equalityComparer)
		{
			if (readOnlySpan.Length == 0)
			{
				return 0;
			}
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			int num = 0;
			while (!span2.IsEmpty)
			{
				int num2 = span2.IndexOf(readOnlySpan[0], equalityComparer);
				if (num2 < 0)
				{
					break;
				}
				if (span2.Slice(num2 + 1).StartsWith(readOnlySpan.Slice(1), equalityComparer))
				{
					return num + num2;
				}
				num += num2 + 1;
				span2 = span2.Slice(num2 + 1);
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
			}
		}
		return SpanHelpers.LastIndexOf(ref MemoryMarshal.GetReference(span), value, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOf<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
				}
			}
			return LastIndexOfDefaultComparer(span, value);
		}
		return LastIndexOfComparer(span, value, comparer);
		static int LastIndexOfComparer(ReadOnlySpan<T> readOnlySpan, T y, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (equalityComparer.Equals(readOnlySpan[num], y))
				{
					return num;
				}
			}
			return -1;
		}
		static int LastIndexOfDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y)
		{
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[num], y))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOf(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOf(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
		}
		return SpanHelpers.LastIndexOf(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(value), value.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOf(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOf(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(value)), value.Length);
			}
		}
		return LastIndexOfComparer(span, value, comparer);
		static int LastIndexOfComparer(ReadOnlySpan<T> readOnlySpan2, ReadOnlySpan<T> readOnlySpan, IEqualityComparer<T> equalityComparer)
		{
			if (readOnlySpan.Length == 0)
			{
				return readOnlySpan2.Length;
			}
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			int num = readOnlySpan2.Length;
			while (true)
			{
				num = readOnlySpan2.Slice(0, num).LastIndexOf(readOnlySpan[0], equalityComparer);
				if (num < 0)
				{
					break;
				}
				if (readOnlySpan2.Slice(num + 1).StartsWith(readOnlySpan.Slice(1), equalityComparer))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAny<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAny(value0, value1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAny<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAny(value0, value1, value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAny<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAny<T>(this Span<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).IndexOfAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int IndexOfAny(this Span<char> span, SearchValues<string> values)
	{
		return ((ReadOnlySpan<char>)span).IndexOfAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
			}
		}
		return SpanHelpers.IndexOfAny(ref MemoryMarshal.GetReference(span), value0, value1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
				}
			}
			return IndexOfAnyDefaultComparer(span, value0, value1);
		}
		return IndexOfAnyComparer(span, value0, value1, comparer);
		static int IndexOfAnyComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (equalityComparer.Equals(readOnlySpan[i], y) || equalityComparer.Equals(readOnlySpan[i], y2))
				{
					return i;
				}
			}
			return -1;
		}
		static int IndexOfAnyDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[i], y) || EqualityComparer<T>.Default.Equals(readOnlySpan[i], y2))
				{
					return i;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
			}
		}
		return SpanHelpers.IndexOfAny(ref MemoryMarshal.GetReference(span), value0, value1, value2, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.IndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
				}
			}
			return IndexOfAnyDefaultComparer(span, value0, value1, value2);
		}
		return IndexOfAnyComparer(span, value0, value1, value2, comparer);
		static int IndexOfAnyComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (equalityComparer.Equals(readOnlySpan[i], y) || equalityComparer.Equals(readOnlySpan[i], y2) || equalityComparer.Equals(readOnlySpan[i], y3))
				{
					return i;
				}
			}
			return -1;
		}
		static int IndexOfAnyDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[i], y) || EqualityComparer<T>.Default.Equals(readOnlySpan[i], y2) || EqualityComparer<T>.Default.Equals(readOnlySpan[i], y3))
				{
					return i;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				ref byte searchSpace = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));
				ref byte reference = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values));
				switch (values.Length)
				{
				case 0:
					return -1;
				case 1:
					return SpanHelpers.IndexOfValueType(ref searchSpace, reference, span.Length);
				case 2:
					return SpanHelpers.IndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), span.Length);
				case 3:
					return SpanHelpers.IndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), span.Length);
				case 4:
					return SpanHelpers.IndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), Unsafe.Add(ref reference, 3), span.Length);
				case 5:
					return SpanHelpers.IndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), Unsafe.Add(ref reference, 3), Unsafe.Add(ref reference, 4), span.Length);
				}
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				ref short reference2 = ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span));
				ref short reference3 = ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(values));
				return values.Length switch
				{
					0 => -1, 
					1 => SpanHelpers.IndexOfValueType(ref reference2, reference3, span.Length), 
					2 => SpanHelpers.IndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), span.Length), 
					3 => SpanHelpers.IndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), Unsafe.Add(ref reference3, 2), span.Length), 
					4 => SpanHelpers.IndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), Unsafe.Add(ref reference3, 2), Unsafe.Add(ref reference3, 3), span.Length), 
					5 => SpanHelpers.IndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), Unsafe.Add(ref reference3, 2), Unsafe.Add(ref reference3, 3), Unsafe.Add(ref reference3, 4), span.Length), 
					_ => ProbabilisticMap.IndexOfAny(ref Unsafe.As<short, char>(ref reference2), span.Length, ref Unsafe.As<short, char>(ref reference3), values.Length), 
				};
			}
		}
		return SpanHelpers.IndexOfAny(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(values), values.Length);
	}

	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		switch (values.Length)
		{
		case 0:
			return -1;
		case 1:
			return span.IndexOf(values[0], comparer);
		case 2:
			return span.IndexOfAny(values[0], values[1], comparer);
		case 3:
			return span.IndexOfAny(values[0], values[1], values[2], comparer);
		default:
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < span.Length; i++)
			{
				if (values.Contains(span[i], comparer))
				{
					return i;
				}
			}
			return -1;
		}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.IndexOfAny(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IndexOfAny(this ReadOnlySpan<char> span, SearchValues<string> values)
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.IndexOfAnyMultiString(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAny<T>(this Span<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAny(value0, value1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAny<T>(this Span<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAny(value0, value1, value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAny<T>(this Span<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int LastIndexOfAny<T>(this Span<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).LastIndexOfAny(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAny(ref MemoryMarshal.GetReference(span), value0, value1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), span.Length);
				}
			}
			return LastIndexOfAnyDefaultComparer(span, value0, value1);
		}
		return LastIndexOfAnyComparer(span, value0, value1, comparer);
		static int LastIndexOfAnyComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (equalityComparer.Equals(readOnlySpan[num], y) || equalityComparer.Equals(readOnlySpan[num], y2))
				{
					return num;
				}
			}
			return -1;
		}
		static int LastIndexOfAnyDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2)
		{
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[num], y) || EqualityComparer<T>.Default.Equals(readOnlySpan[num], y2))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
			}
		}
		return SpanHelpers.LastIndexOfAny(ref MemoryMarshal.GetReference(span), value0, value1, value2, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, T value0, T value1, T value2, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value0), Unsafe.BitCast<T, byte>(value1), Unsafe.BitCast<T, byte>(value2), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value0), Unsafe.BitCast<T, short>(value1), Unsafe.BitCast<T, short>(value2), span.Length);
				}
			}
			return LastIndexOfAnyDefaultComparer(span, value0, value1, value2);
		}
		return LastIndexOfAnyComparer(span, value0, value1, value2, comparer);
		static int LastIndexOfAnyComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (equalityComparer.Equals(readOnlySpan[num], y) || equalityComparer.Equals(readOnlySpan[num], y2) || equalityComparer.Equals(readOnlySpan[num], y3))
				{
					return num;
				}
			}
			return -1;
		}
		static int LastIndexOfAnyDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y, T y2, T y3)
		{
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[num], y) || EqualityComparer<T>.Default.Equals(readOnlySpan[num], y2) || EqualityComparer<T>.Default.Equals(readOnlySpan[num], y3))
				{
					return num;
				}
			}
			return -1;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				ref byte searchSpace = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));
				ref byte reference = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values));
				switch (values.Length)
				{
				case 0:
					return -1;
				case 1:
					return SpanHelpers.LastIndexOfValueType(ref searchSpace, reference, span.Length);
				case 2:
					return SpanHelpers.LastIndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), span.Length);
				case 3:
					return SpanHelpers.LastIndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), span.Length);
				case 4:
					return SpanHelpers.LastIndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), Unsafe.Add(ref reference, 3), span.Length);
				case 5:
					return SpanHelpers.LastIndexOfAnyValueType(ref searchSpace, reference, Unsafe.Add(ref reference, 1), Unsafe.Add(ref reference, 2), Unsafe.Add(ref reference, 3), Unsafe.Add(ref reference, 4), span.Length);
				}
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				ref short reference2 = ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span));
				ref short reference3 = ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(values));
				return values.Length switch
				{
					0 => -1, 
					1 => SpanHelpers.LastIndexOfValueType(ref reference2, reference3, span.Length), 
					2 => SpanHelpers.LastIndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), span.Length), 
					3 => SpanHelpers.LastIndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), Unsafe.Add(ref reference3, 2), span.Length), 
					4 => SpanHelpers.LastIndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), Unsafe.Add(ref reference3, 2), Unsafe.Add(ref reference3, 3), span.Length), 
					5 => SpanHelpers.LastIndexOfAnyValueType(ref reference2, reference3, Unsafe.Add(ref reference3, 1), Unsafe.Add(ref reference3, 2), Unsafe.Add(ref reference3, 3), Unsafe.Add(ref reference3, 4), span.Length), 
					_ => ProbabilisticMap.LastIndexOfAny(ref Unsafe.As<short, char>(ref reference2), span.Length, ref Unsafe.As<short, char>(ref reference3), values.Length), 
				};
			}
		}
		return SpanHelpers.LastIndexOfAny(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(values), values.Length);
	}

	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		switch (values.Length)
		{
		case 0:
			return -1;
		case 1:
			return span.LastIndexOf(values[0], comparer);
		case 2:
			return span.LastIndexOfAny(values[0], values[1], comparer);
		case 3:
			return span.LastIndexOfAny(values[0], values[1], values[2], comparer);
		default:
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}
			for (int num = span.Length - 1; num >= 0; num--)
			{
				if (values.Contains(span[num], comparer))
				{
					return num;
				}
			}
			return -1;
		}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LastIndexOfAny<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		if (values == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
		}
		return values.LastIndexOfAny(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T : IEquatable<T>?
	{
		int length = span.Length;
		int length2 = other.Length;
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (length == length2)
			{
				return SpanHelpers.SequenceEqual(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(other)), (nuint)(uint)length2 * (nuint)Unsafe.SizeOf<T>());
			}
			return false;
		}
		if (length == length2)
		{
			return SpanHelpers.SequenceEqual(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(other), length);
		}
		return false;
	}

	[OverloadResolutionPriority(-1)]
	public static bool SequenceEqual<T>(this Span<T> span, ReadOnlySpan<T> other, IEqualityComparer<T>? comparer = null)
	{
		return ((ReadOnlySpan<T>)span).SequenceEqual(other, comparer);
	}

	public static bool SequenceEqual<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, IEqualityComparer<T>? comparer = null)
	{
		if (span.Length != other.Length)
		{
			return false;
		}
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				return SpanHelpers.SequenceEqual(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(other)), (nuint)(uint)span.Length * (nuint)Unsafe.SizeOf<T>());
			}
			for (int i = 0; i < span.Length; i++)
			{
				if (!EqualityComparer<T>.Default.Equals(span[i], other[i]))
				{
					return false;
				}
			}
			return true;
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		for (int j = 0; j < span.Length; j++)
		{
			if (!comparer.Equals(span[j], other[j]))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int SequenceCompareTo<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other) where T : IComparable<T>?
	{
		if (typeof(T) == typeof(byte))
		{
			return SpanHelpers.SequenceCompareTo(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(other)), other.Length);
		}
		if (typeof(T) == typeof(char))
		{
			return SpanHelpers.SequenceCompareTo(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(span)), span.Length, ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(other)), other.Length);
		}
		return SpanHelpers.SequenceCompareTo(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(other), other.Length);
	}

	public static int SequenceCompareTo<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, IComparer<T>? comparer = null)
	{
		int num = Math.Min(span.Length, other.Length);
		if (comparer == null)
		{
			comparer = Comparer<T>.Default;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = comparer.Compare(span[i], other[i]);
			if (num2 != 0)
			{
				return num2;
			}
		}
		return span.Length.CompareTo(other.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[OverloadResolutionPriority(-1)]
	public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).StartsWith(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		int length = value.Length;
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (length <= span.Length)
			{
				return SpanHelpers.SequenceEqual(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)), (nuint)(uint)length * (nuint)Unsafe.SizeOf<T>());
			}
			return false;
		}
		if (length <= span.Length)
		{
			return SpanHelpers.SequenceEqual(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(value), length);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
	{
		if (value.Length <= span.Length)
		{
			return span.Slice(0, value.Length).SequenceEqual(value, comparer);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[OverloadResolutionPriority(-1)]
	public static bool EndsWith<T>(this Span<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).EndsWith(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		int length = span.Length;
		int length2 = value.Length;
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (length2 <= length)
			{
				return SpanHelpers.SequenceEqual(ref Unsafe.As<T, byte>(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)(length - length2))), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(value)), (nuint)(uint)length2 * (nuint)Unsafe.SizeOf<T>());
			}
			return false;
		}
		if (length2 <= length)
		{
			return SpanHelpers.SequenceEqual(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)(length - length2)), ref MemoryMarshal.GetReference(value), length2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
	{
		if (value.Length <= span.Length)
		{
			return span.Slice(span.Length - value.Length).SequenceEqual(value, comparer);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (span.Length != 0)
		{
			T val = span[0];
			if (val == null)
			{
				return value == null;
			}
			return val.Equals(value);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (span.Length != 0)
		{
			return comparer?.Equals(span[0], value) ?? EqualityComparer<T>.Default.Equals(span[0], value);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWith<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (span.Length != 0)
		{
			T val = span[span.Length - 1];
			if (val == null)
			{
				return value == null;
			}
			return val.Equals(value);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EndsWith<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (span.Length != 0)
		{
			if (comparer != null)
			{
				return comparer.Equals(span[span.Length - 1], value);
			}
			return EqualityComparer<T>.Default.Equals(span[span.Length - 1], value);
		}
		return false;
	}

	public static void Reverse<T>(this Span<T> span)
	{
		if (span.Length > 1)
		{
			SpanHelpers.Reverse(ref MemoryMarshal.GetReference(span), (nuint)span.Length);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this T[]? array)
	{
		return new Span<T>(array);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this T[]? array, int start, int length)
	{
		return new Span<T>(array, start, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this ArraySegment<T> segment)
	{
		return new Span<T>(segment.Array, segment.Offset, segment.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this ArraySegment<T> segment, int start)
	{
		if ((uint)start > (uint)segment.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		return new Span<T>(segment.Array, segment.Offset + start, segment.Count - start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this ArraySegment<T> segment, Index startIndex)
	{
		int offset = startIndex.GetOffset(segment.Count);
		return segment.AsSpan(offset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this ArraySegment<T> segment, int start, int length)
	{
		if ((uint)start > (uint)segment.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		if ((uint)length > (uint)(segment.Count - start))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
		}
		return new Span<T>(segment.Array, segment.Offset + start, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(this ArraySegment<T> segment, Range range)
	{
		var (num, length) = range.GetOffsetAndLength(segment.Count);
		return new Span<T>(segment.Array, segment.Offset + num, length);
	}

	public static Memory<T> AsMemory<T>(this T[]? array)
	{
		return new Memory<T>(array);
	}

	public static Memory<T> AsMemory<T>(this T[]? array, int start)
	{
		return new Memory<T>(array, start);
	}

	public static Memory<T> AsMemory<T>(this T[]? array, Index startIndex)
	{
		if (array == null)
		{
			if (!startIndex.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			return default(Memory<T>);
		}
		int offset = startIndex.GetOffset(array.Length);
		return new Memory<T>(array, offset);
	}

	public static Memory<T> AsMemory<T>(this T[]? array, int start, int length)
	{
		return new Memory<T>(array, start, length);
	}

	public static Memory<T> AsMemory<T>(this T[]? array, Range range)
	{
		if (array == null)
		{
			Index start = range.Start;
			Index end = range.End;
			if (!start.Equals(Index.Start) || !end.Equals(Index.Start))
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			return default(Memory<T>);
		}
		var (start2, length) = range.GetOffsetAndLength(array.Length);
		return new Memory<T>(array, start2, length);
	}

	public static Memory<T> AsMemory<T>(this ArraySegment<T> segment)
	{
		return new Memory<T>(segment.Array, segment.Offset, segment.Count);
	}

	public static Memory<T> AsMemory<T>(this ArraySegment<T> segment, int start)
	{
		if ((uint)start > (uint)segment.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		return new Memory<T>(segment.Array, segment.Offset + start, segment.Count - start);
	}

	public static Memory<T> AsMemory<T>(this ArraySegment<T> segment, int start, int length)
	{
		if ((uint)start > (uint)segment.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		if ((uint)length > (uint)(segment.Count - start))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
		}
		return new Memory<T>(segment.Array, segment.Offset + start, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this T[]? source, Span<T> destination)
	{
		new ReadOnlySpan<T>(source).CopyTo(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this T[]? source, Memory<T> destination)
	{
		source.CopyTo(destination.Span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool Overlaps<T>(this Span<T> span, ReadOnlySpan<T> other)
	{
		return ((ReadOnlySpan<T>)span).Overlaps(other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static bool Overlaps<T>(this Span<T> span, ReadOnlySpan<T> other, out int elementOffset)
	{
		return ((ReadOnlySpan<T>)span).Overlaps(other, out elementOffset);
	}

	public static bool Overlaps<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
	{
		if (span.IsEmpty || other.IsEmpty)
		{
			return false;
		}
		nint num = Unsafe.ByteOffset(in MemoryMarshal.GetReference(span), in MemoryMarshal.GetReference(other));
		if ((nuint)num >= (nuint)((nint)span.Length * (nint)Unsafe.SizeOf<T>()))
		{
			return (nuint)num > (nuint)(-((nint)other.Length * (nint)Unsafe.SizeOf<T>()));
		}
		return true;
	}

	public static bool Overlaps<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, out int elementOffset)
	{
		if (span.IsEmpty || other.IsEmpty)
		{
			elementOffset = 0;
			return false;
		}
		nint num = Unsafe.ByteOffset(in MemoryMarshal.GetReference(span), in MemoryMarshal.GetReference(other));
		if ((nuint)num < (nuint)((nint)span.Length * (nint)Unsafe.SizeOf<T>()) || (nuint)num > (nuint)(-((nint)other.Length * (nint)Unsafe.SizeOf<T>())))
		{
			if (num % Unsafe.SizeOf<T>() != 0)
			{
				ThrowHelper.ThrowArgumentException_OverlapAlignmentMismatch();
			}
			elementOffset = (int)(num / Unsafe.SizeOf<T>());
			return true;
		}
		elementOffset = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int BinarySearch<T>(this Span<T> span, IComparable<T> comparable)
	{
		return ((ReadOnlySpan<T>)span).BinarySearch(comparable);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int BinarySearch<T, TComparable>(this Span<T> span, TComparable comparable) where TComparable : IComparable<T>, allows ref struct
	{
		return ((ReadOnlySpan<T>)span).BinarySearch(comparable);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[OverloadResolutionPriority(-1)]
	public static int BinarySearch<T, TComparer>(this Span<T> span, T value, TComparer comparer) where TComparer : IComparer<T>, allows ref struct
	{
		return ((ReadOnlySpan<T>)span).BinarySearch(value, comparer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T>(this ReadOnlySpan<T> span, IComparable<T> comparable)
	{
		return span.BinarySearch<T, IComparable<T>>(comparable);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T, TComparable>(this ReadOnlySpan<T> span, TComparable comparable) where TComparable : IComparable<T>, allows ref struct
	{
		return SpanHelpers.BinarySearch(span, comparable);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int BinarySearch<T, TComparer>(this ReadOnlySpan<T> span, T value, TComparer comparer) where TComparer : IComparer<T>, allows ref struct
	{
		if (comparer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparer);
		}
		SpanHelpers.ComparerComparable<T, TComparer> comparable = new SpanHelpers.ComparerComparable<T, TComparer>(value, comparer);
		return span.BinarySearch<T, SpanHelpers.ComparerComparable<T, TComparer>>(comparable);
	}

	public static void Sort<T>(this Span<T> span)
	{
		span.Sort((IComparer<T>)null);
	}

	public static void Sort<T, TComparer>(this Span<T> span, TComparer comparer) where TComparer : IComparer<T>?
	{
		if (span.Length > 1)
		{
			ArraySortHelper<T>.Default.Sort(span, comparer);
		}
	}

	public static void Sort<T>(this Span<T> span, Comparison<T> comparison)
	{
		if (comparison == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
		}
		if (span.Length > 1)
		{
			ArraySortHelper<T>.Sort(span, comparison);
		}
	}

	public static void Sort<TKey, TValue>(this Span<TKey> keys, Span<TValue> items)
	{
		keys.Sort<TKey, TValue, IComparer<TKey>>(items, null);
	}

	public static void Sort<TKey, TValue, TComparer>(this Span<TKey> keys, Span<TValue> items, TComparer comparer) where TComparer : IComparer<TKey>?
	{
		if (keys.Length != items.Length)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_SpansMustHaveSameLength);
		}
		if (keys.Length > 1)
		{
			ArraySortHelper<TKey, TValue>.Default.Sort(keys, items, comparer);
		}
	}

	public static void Sort<TKey, TValue>(this Span<TKey> keys, Span<TValue> items, Comparison<TKey> comparison)
	{
		if (comparison == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
		}
		if (keys.Length != items.Length)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_SpansMustHaveSameLength);
		}
		if (keys.Length > 1)
		{
			ArraySortHelper<TKey, TValue>.Default.Sort(keys, items, new ComparisonComparer<TKey>(comparison));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Replace<T>(this Span<T> span, T oldValue, T newValue) where T : IEquatable<T>?
	{
		nuint length = (uint)span.Length;
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				ref byte reference = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));
				SpanHelpers.ReplaceValueType(ref reference, ref reference, Unsafe.BitCast<T, byte>(oldValue), Unsafe.BitCast<T, byte>(newValue), length);
				return;
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				ref ushort reference2 = ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(span));
				SpanHelpers.ReplaceValueType(ref reference2, ref reference2, Unsafe.BitCast<T, ushort>(oldValue), Unsafe.BitCast<T, ushort>(newValue), length);
				return;
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				ref int reference3 = ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span));
				SpanHelpers.ReplaceValueType(ref reference3, ref reference3, Unsafe.BitCast<T, int>(oldValue), Unsafe.BitCast<T, int>(newValue), length);
				return;
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				ref long reference4 = ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span));
				SpanHelpers.ReplaceValueType(ref reference4, ref reference4, Unsafe.BitCast<T, long>(oldValue), Unsafe.BitCast<T, long>(newValue), length);
				return;
			}
		}
		ref T reference5 = ref MemoryMarshal.GetReference(span);
		SpanHelpers.Replace(ref reference5, ref reference5, oldValue, newValue, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Replace<T>(this Span<T> span, T oldValue, T newValue, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					ref byte reference = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span));
					SpanHelpers.ReplaceValueType(ref reference, ref reference, Unsafe.BitCast<T, byte>(oldValue), Unsafe.BitCast<T, byte>(newValue), (uint)span.Length);
					return;
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					ref ushort reference2 = ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(span));
					SpanHelpers.ReplaceValueType(ref reference2, ref reference2, Unsafe.BitCast<T, ushort>(oldValue), Unsafe.BitCast<T, ushort>(newValue), (uint)span.Length);
					return;
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					ref int reference3 = ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span));
					SpanHelpers.ReplaceValueType(ref reference3, ref reference3, Unsafe.BitCast<T, int>(oldValue), Unsafe.BitCast<T, int>(newValue), (uint)span.Length);
					return;
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					ref long reference4 = ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span));
					SpanHelpers.ReplaceValueType(ref reference4, ref reference4, Unsafe.BitCast<T, long>(oldValue), Unsafe.BitCast<T, long>(newValue), (uint)span.Length);
					return;
				}
			}
			ReplaceDefaultComparer(span, oldValue, newValue);
		}
		else
		{
			ReplaceComparer(span, oldValue, newValue, comparer);
		}
		static void ReplaceComparer(Span<T> span2, T y, T val, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < span2.Length; i++)
			{
				if (equalityComparer.Equals(span2[i], y))
				{
					span2[i] = val;
				}
			}
		}
		static void ReplaceDefaultComparer(Span<T> span2, T y, T val)
		{
			for (int i = 0; i < span2.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(span2[i], y))
				{
					span2[i] = val;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Replace<T>(this ReadOnlySpan<T> source, Span<T> destination, T oldValue, T newValue) where T : IEquatable<T>?
	{
		nuint num = (uint)source.Length;
		if (num == 0)
		{
			return;
		}
		if (num > (uint)destination.Length)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		ref T reference = ref MemoryMarshal.GetReference(source);
		ref T reference2 = ref MemoryMarshal.GetReference(destination);
		nint num2 = Unsafe.ByteOffset(in reference, in reference2);
		if (num2 != 0 && ((nuint)num2 < (nuint)((nint)source.Length * (nint)Unsafe.SizeOf<T>()) || (nuint)num2 > (nuint)(-((nint)destination.Length * (nint)Unsafe.SizeOf<T>()))))
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				SpanHelpers.ReplaceValueType(ref Unsafe.As<T, byte>(ref reference), ref Unsafe.As<T, byte>(ref reference2), Unsafe.BitCast<T, byte>(oldValue), Unsafe.BitCast<T, byte>(newValue), num);
				return;
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				SpanHelpers.ReplaceValueType(ref Unsafe.As<T, ushort>(ref reference), ref Unsafe.As<T, ushort>(ref reference2), Unsafe.BitCast<T, ushort>(oldValue), Unsafe.BitCast<T, ushort>(newValue), num);
				return;
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				SpanHelpers.ReplaceValueType(ref Unsafe.As<T, int>(ref reference), ref Unsafe.As<T, int>(ref reference2), Unsafe.BitCast<T, int>(oldValue), Unsafe.BitCast<T, int>(newValue), num);
				return;
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				SpanHelpers.ReplaceValueType(ref Unsafe.As<T, long>(ref reference), ref Unsafe.As<T, long>(ref reference2), Unsafe.BitCast<T, long>(oldValue), Unsafe.BitCast<T, long>(newValue), num);
				return;
			}
		}
		SpanHelpers.Replace(ref reference, ref reference2, oldValue, newValue, num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Replace<T>(this ReadOnlySpan<T> source, Span<T> destination, T oldValue, T newValue, IEqualityComparer<T>? comparer = null)
	{
		nuint num = (uint)source.Length;
		if (num == 0)
		{
			return;
		}
		if (num > (uint)destination.Length)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		ref T reference = ref MemoryMarshal.GetReference(source);
		ref T reference2 = ref MemoryMarshal.GetReference(destination);
		nint num2 = Unsafe.ByteOffset(in reference, in reference2);
		if (num2 != 0 && ((nuint)num2 < (nuint)((nint)source.Length * (nint)Unsafe.SizeOf<T>()) || (nuint)num2 > (nuint)(-((nint)destination.Length * (nint)Unsafe.SizeOf<T>()))))
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					SpanHelpers.ReplaceValueType(ref Unsafe.As<T, byte>(ref reference), ref Unsafe.As<T, byte>(ref reference2), Unsafe.BitCast<T, byte>(oldValue), Unsafe.BitCast<T, byte>(newValue), num);
					return;
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					SpanHelpers.ReplaceValueType(ref Unsafe.As<T, ushort>(ref reference), ref Unsafe.As<T, ushort>(ref reference2), Unsafe.BitCast<T, ushort>(oldValue), Unsafe.BitCast<T, ushort>(newValue), num);
					return;
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					SpanHelpers.ReplaceValueType(ref Unsafe.As<T, int>(ref reference), ref Unsafe.As<T, int>(ref reference2), Unsafe.BitCast<T, int>(oldValue), Unsafe.BitCast<T, int>(newValue), num);
					return;
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					SpanHelpers.ReplaceValueType(ref Unsafe.As<T, long>(ref reference), ref Unsafe.As<T, long>(ref reference2), Unsafe.BitCast<T, long>(oldValue), Unsafe.BitCast<T, long>(newValue), num);
					return;
				}
			}
			ReplaceDefaultComparer(source, destination, oldValue, newValue);
		}
		else
		{
			ReplaceComparer(source, destination, oldValue, newValue, comparer);
		}
		static void ReplaceComparer(ReadOnlySpan<T> readOnlySpan, Span<T> span, T y, T val, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				span[i] = (equalityComparer.Equals(readOnlySpan[i], y) ? val : readOnlySpan[i]);
			}
		}
		static void ReplaceDefaultComparer(ReadOnlySpan<T> readOnlySpan, Span<T> span, T y, T val)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				span[i] = (EqualityComparer<T>.Default.Equals(readOnlySpan[i], y) ? val : readOnlySpan[i]);
			}
		}
	}

	public static void ReplaceAny<T>(this ReadOnlySpan<T> source, Span<T> destination, SearchValues<T> values, T newValue) where T : IEquatable<T>?
	{
		if (source.Length > destination.Length)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		if (!Unsafe.AreSame(in source._reference, in destination._reference) && source.Overlaps(destination))
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		source.CopyTo(destination);
		destination.Slice(0, source.Length).ReplaceAny(values, newValue);
	}

	public static void ReplaceAny<T>(this Span<T> span, SearchValues<T> values, T newValue) where T : IEquatable<T>?
	{
		int num;
		while ((num = ((ReadOnlySpan<T>)span).IndexOfAny(values)) >= 0)
		{
			span[num] = newValue;
			span = span.Slice(num + 1);
		}
	}

	public static void ReplaceAnyExcept<T>(this ReadOnlySpan<T> source, Span<T> destination, SearchValues<T> values, T newValue) where T : IEquatable<T>?
	{
		if (source.Length > destination.Length)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		if (!Unsafe.AreSame(in source._reference, in destination._reference) && source.Overlaps(destination))
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		source.CopyTo(destination);
		destination.Slice(0, source.Length).ReplaceAnyExcept(values, newValue);
	}

	public static void ReplaceAnyExcept<T>(this Span<T> span, SearchValues<T> values, T newValue) where T : IEquatable<T>?
	{
		int num;
		while ((num = ((ReadOnlySpan<T>)span).IndexOfAnyExcept(values)) >= 0)
		{
			span[num] = newValue;
			span = span.Slice(num + 1);
		}
	}

	[OverloadResolutionPriority(-1)]
	public static int CommonPrefixLength<T>(this Span<T> span, ReadOnlySpan<T> other)
	{
		return ((ReadOnlySpan<T>)span).CommonPrefixLength(other);
	}

	[OverloadResolutionPriority(-1)]
	public static int CommonPrefixLength<T>(this Span<T> span, ReadOnlySpan<T> other, IEqualityComparer<T>? comparer)
	{
		return ((ReadOnlySpan<T>)span).CommonPrefixLength(other, comparer);
	}

	public static int CommonPrefixLength<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other)
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			nuint num = Math.Min((nuint)(uint)span.Length, (nuint)(uint)other.Length);
			nuint num2 = (uint)Unsafe.SizeOf<T>();
			return (int)(SpanHelpers.CommonPrefixLength(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(other)), num * num2) / num2);
		}
		SliceLongerSpanToMatchShorterLength(ref span, ref other);
		for (int i = 0; i < span.Length; i++)
		{
			if (!EqualityComparer<T>.Default.Equals(span[i], other[i]))
			{
				return i;
			}
		}
		return span.Length;
	}

	public static int CommonPrefixLength<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> other, IEqualityComparer<T>? comparer)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			return span.CommonPrefixLength(other);
		}
		SliceLongerSpanToMatchShorterLength(ref span, ref other);
		if (comparer == null)
		{
			comparer = EqualityComparer<T>.Default;
		}
		for (int i = 0; i < span.Length; i++)
		{
			if (!comparer.Equals(span[i], other[i]))
			{
				return i;
			}
		}
		return span.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SliceLongerSpanToMatchShorterLength<T>(ref ReadOnlySpan<T> span, ref ReadOnlySpan<T> other)
	{
		if (other.Length > span.Length)
		{
			other = other.Slice(0, span.Length);
		}
		else if (span.Length > other.Length)
		{
			span = span.Slice(0, other.Length);
		}
	}

	public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, T separator) where T : IEquatable<T>
	{
		return new SpanSplitEnumerator<T>(source, separator);
	}

	public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> source, ReadOnlySpan<T> separator) where T : IEquatable<T>
	{
		return new SpanSplitEnumerator<T>(source, separator, treatAsSingleSeparator: true);
	}

	public static SpanSplitEnumerator<T> SplitAny<T>(this ReadOnlySpan<T> source, [UnscopedRef] params ReadOnlySpan<T> separators) where T : IEquatable<T>
	{
		return new SpanSplitEnumerator<T>(source, separators);
	}

	public static SpanSplitEnumerator<T> SplitAny<T>(this ReadOnlySpan<T> source, SearchValues<T> separators) where T : IEquatable<T>
	{
		return new SpanSplitEnumerator<T>(source, separators);
	}

	public static int Split(this ReadOnlySpan<char> source, Span<Range> destination, char separator, StringSplitOptions options = StringSplitOptions.None)
	{
		string.CheckStringSplitOptions(options);
		return SplitCore(source, destination, new ReadOnlySpan<char>(in separator), default(ReadOnlySpan<string>), isAny: true, options);
	}

	public static int Split(this ReadOnlySpan<char> source, Span<Range> destination, ReadOnlySpan<char> separator, StringSplitOptions options = StringSplitOptions.None)
	{
		string.CheckStringSplitOptions(options);
		if (separator.IsEmpty)
		{
			if (!destination.IsEmpty)
			{
				int num = 0;
				int num2 = source.Length;
				if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None)
				{
					(num, num2) = TrimSplitEntry(source, num, num2);
				}
				if (num != num2 || (options & StringSplitOptions.RemoveEmptyEntries) == 0)
				{
					destination[0] = num..num2;
					return 1;
				}
			}
			return 0;
		}
		return SplitCore(source, destination, separator, default(ReadOnlySpan<string>), isAny: false, options);
	}

	public static int SplitAny(this ReadOnlySpan<char> source, Span<Range> destination, ReadOnlySpan<char> separators, StringSplitOptions options = StringSplitOptions.None)
	{
		string.CheckStringSplitOptions(options);
		if (separators.IsEmpty && destination.Length > source.Length)
		{
			options &= ~StringSplitOptions.TrimEntries;
		}
		return SplitCore(source, destination, separators, default(ReadOnlySpan<string>), isAny: true, options);
	}

	public static int SplitAny(this ReadOnlySpan<char> source, Span<Range> destination, ReadOnlySpan<string> separators, StringSplitOptions options = StringSplitOptions.None)
	{
		string.CheckStringSplitOptions(options);
		if (separators.IsEmpty && destination.Length > source.Length)
		{
			options &= ~StringSplitOptions.TrimEntries;
		}
		return SplitCore(source, destination, default(ReadOnlySpan<char>), separators, isAny: true, options);
	}

	private static int SplitCore(ReadOnlySpan<char> source, Span<Range> destination, ReadOnlySpan<char> separatorOrSeparators, ReadOnlySpan<string> stringSeparators, bool isAny, StringSplitOptions options)
	{
		if (destination.IsEmpty)
		{
			return 0;
		}
		bool flag = (options & StringSplitOptions.RemoveEmptyEntries) == 0;
		bool flag2 = (options & StringSplitOptions.TrimEntries) != 0;
		if (source.Length == 0)
		{
			if (flag)
			{
				destination[0] = default(Range);
				return 1;
			}
			return 0;
		}
		int num = 0;
		if (destination.Length == 1)
		{
			int num2 = source.Length;
			if (flag2)
			{
				(num, num2) = TrimSplitEntry(source, num, num2);
			}
			if ((num != num2) | flag)
			{
				destination[0] = num..num2;
				return 1;
			}
			return 0;
		}
		Span<int> scratchBuffer = stackalloc int[128];
		ValueListBuilder<int> sepListBuilder = new ValueListBuilder<int>(scratchBuffer);
		ValueListBuilder<int> lengthListBuilder = default(ValueListBuilder<int>);
		int num3 = 0;
		int num4;
		if (!stringSeparators.IsEmpty)
		{
			scratchBuffer = stackalloc int[128];
			lengthListBuilder = new ValueListBuilder<int>(scratchBuffer);
			string.MakeSeparatorListAny(source, stringSeparators, ref sepListBuilder, ref lengthListBuilder);
			num4 = -1;
		}
		else if (isAny)
		{
			string.MakeSeparatorListAny(source, separatorOrSeparators, ref sepListBuilder);
			num4 = 1;
		}
		else
		{
			string.MakeSeparatorList(source, separatorOrSeparators, ref sepListBuilder);
			num4 = separatorOrSeparators.Length;
		}
		int num5 = 0;
		Span<Range> span = destination.Slice(0, destination.Length - 1);
		while (num5 < sepListBuilder.Length && (num3 < span.Length || !flag))
		{
			int num2 = sepListBuilder[num5];
			if (num5 < lengthListBuilder.Length)
			{
				num4 = lengthListBuilder[num5];
			}
			num5++;
			int num6 = num2;
			if (flag2)
			{
				(num, num2) = TrimSplitEntry(source, num, num2);
			}
			if ((num != num2) | flag)
			{
				if ((uint)num3 >= (uint)span.Length)
				{
					break;
				}
				span[num3] = num..num2;
				num3++;
			}
			num = num6 + num4;
		}
		sepListBuilder.Dispose();
		lengthListBuilder.Dispose();
		if ((uint)num3 < (uint)destination.Length)
		{
			int num2 = source.Length;
			if (flag2)
			{
				(num, num2) = TrimSplitEntry(source, num, num2);
			}
			if ((num != num2) | flag)
			{
				destination[num3] = num..num2;
				num3++;
			}
		}
		return num3;
	}

	private static (int StartInclusive, int EndExclusive) TrimSplitEntry(ReadOnlySpan<char> source, int startInclusive, int endExclusive)
	{
		while (startInclusive < endExclusive && char.IsWhiteSpace(source[startInclusive]))
		{
			startInclusive++;
		}
		while (endExclusive > startInclusive && char.IsWhiteSpace(source[endExclusive - 1]))
		{
			endExclusive--;
		}
		return (StartInclusive: startInclusive, EndExclusive: endExclusive);
	}

	[OverloadResolutionPriority(-1)]
	public static int Count<T>(this Span<T> span, T value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).Count(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Count<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
	{
		if (RuntimeHelpers.IsBitwiseEquatable<T>())
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return SpanHelpers.CountValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return SpanHelpers.CountValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return SpanHelpers.CountValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return SpanHelpers.CountValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
			}
		}
		return SpanHelpers.Count(ref MemoryMarshal.GetReference(span), value, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Count<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer = null)
	{
		if (typeof(T).IsValueType && (comparer == null || comparer == EqualityComparer<T>.Default))
		{
			if (RuntimeHelpers.IsBitwiseEquatable<T>())
			{
				if (Unsafe.SizeOf<T>() == 1)
				{
					return SpanHelpers.CountValueType(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, byte>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 2)
				{
					return SpanHelpers.CountValueType(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, short>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 4)
				{
					return SpanHelpers.CountValueType(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, int>(value), span.Length);
				}
				if (Unsafe.SizeOf<T>() == 8)
				{
					return SpanHelpers.CountValueType(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)), Unsafe.BitCast<T, long>(value), span.Length);
				}
			}
			return CountDefaultComparer(span, value);
		}
		return CountComparer(span, value, comparer);
		static int CountComparer(ReadOnlySpan<T> readOnlySpan, T y, IEqualityComparer<T> equalityComparer)
		{
			if (equalityComparer == null)
			{
				equalityComparer = EqualityComparer<T>.Default;
			}
			int num = 0;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (equalityComparer.Equals(readOnlySpan[i], y))
				{
					num++;
				}
			}
			return num;
		}
		static int CountDefaultComparer(ReadOnlySpan<T> readOnlySpan, T y)
		{
			int num = 0;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(readOnlySpan[i], y))
				{
					num++;
				}
			}
			return num;
		}
	}

	[OverloadResolutionPriority(-1)]
	public static int Count<T>(this Span<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		return ((ReadOnlySpan<T>)span).Count(value);
	}

	public static int Count<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>?
	{
		switch (value.Length)
		{
		case 0:
			return 0;
		case 1:
			return span.Count(value[0]);
		default:
		{
			int num = 0;
			int num2;
			while ((num2 = span.IndexOf(value)) >= 0)
			{
				span = span.Slice(num2 + value.Length);
				num++;
			}
			return num;
		}
		}
	}

	public static int Count<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
	{
		switch (value.Length)
		{
		case 0:
			return 0;
		case 1:
			return span.Count(value[0], comparer);
		default:
		{
			int num = 0;
			int num2;
			while ((num2 = span.IndexOf(value, comparer)) >= 0)
			{
				span = span.Slice(num2 + value.Length);
				num++;
			}
			return num;
		}
		}
	}

	public static int CountAny<T>(this ReadOnlySpan<T> span, SearchValues<T> values) where T : IEquatable<T>?
	{
		int num = 0;
		int num2;
		while ((num2 = span.IndexOfAny(values)) >= 0)
		{
			num++;
			span = span.Slice(num2 + 1);
		}
		return num;
	}

	public static int CountAny<T>(this ReadOnlySpan<T> span, params ReadOnlySpan<T> values) where T : IEquatable<T>?
	{
		int num = 0;
		int num2;
		while ((num2 = span.IndexOfAny(values)) >= 0)
		{
			num++;
			span = span.Slice(num2 + 1);
		}
		return num;
	}

	public static int CountAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values, IEqualityComparer<T>? comparer = null)
	{
		int num = 0;
		int num2;
		while ((num2 = span.IndexOfAny(values, comparer)) >= 0)
		{
			num++;
			span = span.Slice(num2 + 1);
		}
		return num;
	}

	public static bool TryWrite(this Span<char> destination, [InterpolatedStringHandlerArgument("destination")] ref TryWriteInterpolatedStringHandler handler, out int charsWritten)
	{
		if (handler._success)
		{
			charsWritten = handler._pos;
			return true;
		}
		charsWritten = 0;
		return false;
	}

	public static bool TryWrite(this Span<char> destination, IFormatProvider? provider, [InterpolatedStringHandlerArgument(new string[] { "destination", "provider" })] ref TryWriteInterpolatedStringHandler handler, out int charsWritten)
	{
		return destination.TryWrite(ref handler, out charsWritten);
	}

	public static bool TryWrite<TArg0>(this Span<char> destination, IFormatProvider? provider, CompositeFormat format, out int charsWritten, TArg0 arg0)
	{
		ArgumentNullException.ThrowIfNull(format, "format");
		format.ValidateNumberOfArgs(1);
		return TryWrite(destination, provider, format, out charsWritten, arg0, 0, 0, default(ReadOnlySpan<object>));
	}

	public static bool TryWrite<TArg0, TArg1>(this Span<char> destination, IFormatProvider? provider, CompositeFormat format, out int charsWritten, TArg0 arg0, TArg1 arg1)
	{
		ArgumentNullException.ThrowIfNull(format, "format");
		format.ValidateNumberOfArgs(2);
		return TryWrite(destination, provider, format, out charsWritten, arg0, arg1, 0, default(ReadOnlySpan<object>));
	}

	public static bool TryWrite<TArg0, TArg1, TArg2>(this Span<char> destination, IFormatProvider? provider, CompositeFormat format, out int charsWritten, TArg0 arg0, TArg1 arg1, TArg2 arg2)
	{
		ArgumentNullException.ThrowIfNull(format, "format");
		format.ValidateNumberOfArgs(3);
		return TryWrite(destination, provider, format, out charsWritten, arg0, arg1, arg2, default(ReadOnlySpan<object>));
	}

	public static bool TryWrite(this Span<char> destination, IFormatProvider? provider, CompositeFormat format, out int charsWritten, params object?[] args)
	{
		ArgumentNullException.ThrowIfNull(format, "format");
		ArgumentNullException.ThrowIfNull(args, "args");
		return destination.TryWrite(provider, format, out charsWritten, (ReadOnlySpan<object?>)args);
	}

	public static bool TryWrite(this Span<char> destination, IFormatProvider? provider, CompositeFormat format, out int charsWritten, params ReadOnlySpan<object?> args)
	{
		ArgumentNullException.ThrowIfNull(format, "format");
		format.ValidateNumberOfArgs(args.Length);
		return args.Length switch
		{
			0 => TryWrite(destination, provider, format, out charsWritten, 0, 0, 0, args), 
			1 => TryWrite(destination, provider, format, out charsWritten, args[0], 0, 0, args), 
			2 => TryWrite(destination, provider, format, out charsWritten, args[0], args[1], 0, args), 
			_ => TryWrite(destination, provider, format, out charsWritten, args[0], args[1], args[2], args), 
		};
	}

	private static bool TryWrite<TArg0, TArg1, TArg2>(Span<char> destination, IFormatProvider provider, CompositeFormat format, out int charsWritten, TArg0 arg0, TArg1 arg1, TArg2 arg2, ReadOnlySpan<object> args)
	{
		TryWriteInterpolatedStringHandler handler = new TryWriteInterpolatedStringHandler(format._literalLength, format._formattedCount, destination, provider, out var shouldAppend);
		if (shouldAppend)
		{
			(string, int, int, string)[] segments = format._segments;
			for (int i = 0; i < segments.Length; i++)
			{
				(string, int, int, string) tuple = segments[i];
				var (text, _, _, _) = tuple;
				bool flag;
				if (text != null)
				{
					flag = handler.AppendLiteral(text);
				}
				else
				{
					int item = tuple.Item2;
					flag = item switch
					{
						0 => handler.AppendFormatted(arg0, tuple.Item3, tuple.Item4), 
						1 => handler.AppendFormatted(arg1, tuple.Item3, tuple.Item4), 
						2 => handler.AppendFormatted(arg2, tuple.Item3, tuple.Item4), 
						_ => handler.AppendFormatted(args[item], tuple.Item3, tuple.Item4), 
					};
				}
				if (!flag)
				{
					break;
				}
			}
		}
		return destination.TryWrite(provider, ref handler, out charsWritten);
	}

	public static bool IsWhiteSpace(this ReadOnlySpan<char> span)
	{
		for (int i = 0; i < span.Length; i++)
		{
			if (!char.IsWhiteSpace(span[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool Contains(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		return span.IndexOf(value, comparisonType) >= 0;
	}

	[Intrinsic]
	public static bool Equals(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.Compare(span, other, string.GetCaseCompareOfComparisonCulture(comparisonType)) == 0;
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.Compare(span, other, string.GetCaseCompareOfComparisonCulture(comparisonType)) == 0;
		case StringComparison.Ordinal:
			return span.EqualsOrdinal(other);
		default:
			return span.EqualsOrdinalIgnoreCase(other);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool EqualsOrdinal(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
	{
		if (span.Length != value.Length)
		{
			return false;
		}
		if (value.Length == 0)
		{
			return true;
		}
		return span.SequenceEqual(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
	{
		if (span.Length != value.Length)
		{
			return false;
		}
		if (value.Length == 0)
		{
			return true;
		}
		return Ordinal.EqualsIgnoreCase(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(value), span.Length);
	}

	public static int CompareTo(this ReadOnlySpan<char> span, ReadOnlySpan<char> other, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.Compare(span, other, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.Compare(span, other, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.Ordinal:
			if (span.Length == 0 || other.Length == 0)
			{
				return span.Length - other.Length;
			}
			return string.CompareOrdinal(span, other);
		default:
			return Ordinal.CompareStringIgnoreCase(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(other), other.Length);
		}
	}

	public static int IndexOf(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.Ordinal:
			return SpanHelpers.IndexOf(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(value), value.Length);
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.IndexOf(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		default:
			return Ordinal.IndexOfOrdinalIgnoreCase(span, value);
		}
	}

	public static int LastIndexOf(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.Ordinal:
			return SpanHelpers.LastIndexOf(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(value), value.Length);
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.LastIndexOf(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		default:
			return Ordinal.LastIndexOfOrdinalIgnoreCase(span, value);
		}
	}

	public static int ToLower(this ReadOnlySpan<char> source, Span<char> destination, CultureInfo? culture)
	{
		if (source.Overlaps(destination))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		if (destination.Length < source.Length)
		{
			return -1;
		}
		if (GlobalizationMode.Invariant)
		{
			InvariantModeCasing.ToLower(source, destination);
		}
		else
		{
			culture.TextInfo.ChangeCaseToLower(source, destination);
		}
		return source.Length;
	}

	public static int ToLowerInvariant(this ReadOnlySpan<char> source, Span<char> destination)
	{
		if (source.Overlaps(destination))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		if (destination.Length < source.Length)
		{
			return -1;
		}
		if (GlobalizationMode.Invariant)
		{
			InvariantModeCasing.ToLower(source, destination);
		}
		else
		{
			TextInfo.Invariant.ChangeCaseToLower(source, destination);
		}
		return source.Length;
	}

	public static int ToUpper(this ReadOnlySpan<char> source, Span<char> destination, CultureInfo? culture)
	{
		if (source.Overlaps(destination))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		if (destination.Length < source.Length)
		{
			return -1;
		}
		if (GlobalizationMode.Invariant)
		{
			InvariantModeCasing.ToUpper(source, destination);
		}
		else
		{
			culture.TextInfo.ChangeCaseToUpper(source, destination);
		}
		return source.Length;
	}

	public static int ToUpperInvariant(this ReadOnlySpan<char> source, Span<char> destination)
	{
		if (source.Overlaps(destination))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		if (destination.Length < source.Length)
		{
			return -1;
		}
		if (GlobalizationMode.Invariant)
		{
			InvariantModeCasing.ToUpper(source, destination);
		}
		else
		{
			TextInfo.Invariant.ChangeCaseToUpper(source, destination);
		}
		return source.Length;
	}

	[Intrinsic]
	public static bool EndsWith(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.IsSuffix(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.Ordinal:
			return span.EndsWith(value);
		default:
			return span.EndsWithOrdinalIgnoreCase(value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool EndsWithOrdinalIgnoreCase(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
	{
		if (value.Length <= span.Length)
		{
			return Ordinal.EqualsIgnoreCase(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), span.Length - value.Length), ref MemoryMarshal.GetReference(value), value.Length);
		}
		return false;
	}

	[Intrinsic]
	public static bool StartsWith(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.IsPrefix(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.Ordinal:
			return span.StartsWith(value);
		default:
			return span.StartsWithOrdinalIgnoreCase(value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool StartsWithOrdinalIgnoreCase(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
	{
		if (value.Length <= span.Length)
		{
			return Ordinal.EqualsIgnoreCase(ref MemoryMarshal.GetReference(span), ref MemoryMarshal.GetReference(value), value.Length);
		}
		return false;
	}

	public static SpanRuneEnumerator EnumerateRunes(this ReadOnlySpan<char> span)
	{
		return new SpanRuneEnumerator(span);
	}

	[OverloadResolutionPriority(-1)]
	public static SpanRuneEnumerator EnumerateRunes(this Span<char> span)
	{
		return new SpanRuneEnumerator(span);
	}

	public static SpanLineEnumerator EnumerateLines(this ReadOnlySpan<char> span)
	{
		return new SpanLineEnumerator(span);
	}

	[OverloadResolutionPriority(-1)]
	public static SpanLineEnumerator EnumerateLines(this Span<char> span)
	{
		return new SpanLineEnumerator(span);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool EqualsOrdinalIgnoreCaseUtf8(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> value)
	{
		if ((span.Length | value.Length) == 0)
		{
			return true;
		}
		return Ordinal.EqualsIgnoreCaseUtf8(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(value), value.Length);
	}

	internal static bool StartsWithUtf8(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> value, StringComparison comparisonType)
	{
		string.CheckStringComparison(comparisonType);
		switch (comparisonType)
		{
		case StringComparison.CurrentCulture:
		case StringComparison.CurrentCultureIgnoreCase:
			return CultureInfo.CurrentCulture.CompareInfo.IsPrefixUtf8(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.InvariantCulture:
		case StringComparison.InvariantCultureIgnoreCase:
			return CompareInfo.Invariant.IsPrefixUtf8(span, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
		case StringComparison.Ordinal:
			return span.StartsWith(value);
		default:
			return span.StartsWithOrdinalIgnoreCaseUtf8(value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool StartsWithOrdinalIgnoreCaseUtf8(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> value)
	{
		if ((span.Length | value.Length) == 0)
		{
			return true;
		}
		return Ordinal.StartsWithIgnoreCaseUtf8(ref MemoryMarshal.GetReference(span), span.Length, ref MemoryMarshal.GetReference(value), value.Length);
	}

	public static Memory<T> Trim<T>(this Memory<T> memory, T trimElement) where T : IEquatable<T>?
	{
		ReadOnlySpan<T> span = memory.Span;
		int start = ClampStart<T>(span, trimElement);
		int length = ClampEnd<T>(span, start, trimElement);
		return memory.Slice(start, length);
	}

	public static Memory<T> TrimStart<T>(this Memory<T> memory, T trimElement) where T : IEquatable<T>?
	{
		return memory.Slice(ClampStart<T>(memory.Span, trimElement));
	}

	public static Memory<T> TrimEnd<T>(this Memory<T> memory, T trimElement) where T : IEquatable<T>?
	{
		return memory.Slice(0, ClampEnd<T>(memory.Span, 0, trimElement));
	}

	public static ReadOnlyMemory<T> Trim<T>(this ReadOnlyMemory<T> memory, T trimElement) where T : IEquatable<T>?
	{
		ReadOnlySpan<T> span = memory.Span;
		int start = ClampStart<T>(span, trimElement);
		int length = ClampEnd<T>(span, start, trimElement);
		return memory.Slice(start, length);
	}

	public static ReadOnlyMemory<T> TrimStart<T>(this ReadOnlyMemory<T> memory, T trimElement) where T : IEquatable<T>?
	{
		return memory.Slice(ClampStart<T>(memory.Span, trimElement));
	}

	public static ReadOnlyMemory<T> TrimEnd<T>(this ReadOnlyMemory<T> memory, T trimElement) where T : IEquatable<T>?
	{
		return memory.Slice(0, ClampEnd<T>(memory.Span, 0, trimElement));
	}

	public static Span<T> Trim<T>(this Span<T> span, T trimElement) where T : IEquatable<T>?
	{
		int start = ClampStart<T>(span, trimElement);
		int length = ClampEnd<T>(span, start, trimElement);
		return span.Slice(start, length);
	}

	public static Span<T> TrimStart<T>(this Span<T> span, T trimElement) where T : IEquatable<T>?
	{
		return span.Slice(ClampStart<T>(span, trimElement));
	}

	public static Span<T> TrimEnd<T>(this Span<T> span, T trimElement) where T : IEquatable<T>?
	{
		return span.Slice(0, ClampEnd<T>(span, 0, trimElement));
	}

	public static ReadOnlySpan<T> Trim<T>(this ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
	{
		int start = ClampStart<T>(span, trimElement);
		int length = ClampEnd<T>(span, start, trimElement);
		return span.Slice(start, length);
	}

	public static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
	{
		return span.Slice(ClampStart<T>(span, trimElement));
	}

	public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>?
	{
		return span.Slice(0, ClampEnd<T>(span, 0, trimElement));
	}

	private static int ClampStart<T>(ReadOnlySpan<T> span, T trimElement) where T : IEquatable<T>
	{
		int i = 0;
		if (trimElement != null)
		{
			for (; i < span.Length && trimElement.Equals(span[i]); i++)
			{
			}
		}
		else
		{
			for (; i < span.Length && span[i] == null; i++)
			{
			}
		}
		return i;
	}

	private static int ClampEnd<T>(ReadOnlySpan<T> span, int start, T trimElement) where T : IEquatable<T>
	{
		int num = span.Length - 1;
		if (trimElement != null)
		{
			while (num >= start && trimElement.Equals(span[num]))
			{
				num--;
			}
		}
		else
		{
			while (num >= start && span[num] == null)
			{
				num--;
			}
		}
		return num - start + 1;
	}

	public static Memory<T> Trim<T>(this Memory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			ReadOnlySpan<T> span = memory.Span;
			int start = ClampStart<T>(span, trimElements);
			int length = ClampEnd<T>(span, start, trimElements);
			return memory.Slice(start, length);
		}
		if (trimElements.Length == 1)
		{
			return memory.Trim(trimElements[0]);
		}
		return memory;
	}

	public static Memory<T> TrimStart<T>(this Memory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return memory.Slice(ClampStart<T>(memory.Span, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return memory.TrimStart(trimElements[0]);
		}
		return memory;
	}

	public static Memory<T> TrimEnd<T>(this Memory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return memory.Slice(0, ClampEnd<T>(memory.Span, 0, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return memory.TrimEnd(trimElements[0]);
		}
		return memory;
	}

	public static ReadOnlyMemory<T> Trim<T>(this ReadOnlyMemory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			ReadOnlySpan<T> span = memory.Span;
			int start = ClampStart<T>(span, trimElements);
			int length = ClampEnd<T>(span, start, trimElements);
			return memory.Slice(start, length);
		}
		if (trimElements.Length == 1)
		{
			return memory.Trim(trimElements[0]);
		}
		return memory;
	}

	public static ReadOnlyMemory<T> TrimStart<T>(this ReadOnlyMemory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return memory.Slice(ClampStart<T>(memory.Span, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return memory.TrimStart(trimElements[0]);
		}
		return memory;
	}

	public static ReadOnlyMemory<T> TrimEnd<T>(this ReadOnlyMemory<T> memory, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return memory.Slice(0, ClampEnd<T>(memory.Span, 0, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return memory.TrimEnd(trimElements[0]);
		}
		return memory;
	}

	public static Span<T> Trim<T>(this Span<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			int start = ClampStart<T>(span, trimElements);
			int length = ClampEnd<T>(span, start, trimElements);
			return span.Slice(start, length);
		}
		if (trimElements.Length == 1)
		{
			return span.Trim(trimElements[0]);
		}
		return span;
	}

	public static Span<T> TrimStart<T>(this Span<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return span.Slice(ClampStart<T>(span, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return span.TrimStart(trimElements[0]);
		}
		return span;
	}

	public static Span<T> TrimEnd<T>(this Span<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return span.Slice(0, ClampEnd<T>(span, 0, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return span.TrimEnd(trimElements[0]);
		}
		return span;
	}

	public static ReadOnlySpan<T> Trim<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			int start = ClampStart<T>(span, trimElements);
			int length = ClampEnd<T>(span, start, trimElements);
			return span.Slice(start, length);
		}
		if (trimElements.Length == 1)
		{
			return span.Trim(trimElements[0]);
		}
		return span;
	}

	public static ReadOnlySpan<T> TrimStart<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return span.Slice(ClampStart<T>(span, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return span.TrimStart(trimElements[0]);
		}
		return span;
	}

	public static ReadOnlySpan<T> TrimEnd<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>?
	{
		if (trimElements.Length > 1)
		{
			return span.Slice(0, ClampEnd<T>(span, 0, trimElements));
		}
		if (trimElements.Length == 1)
		{
			return span.TrimEnd(trimElements[0]);
		}
		return span;
	}

	private static int ClampStart<T>(ReadOnlySpan<T> span, ReadOnlySpan<T> trimElements) where T : IEquatable<T>
	{
		int i;
		for (i = 0; i < span.Length && trimElements.Contains(span[i]); i++)
		{
		}
		return i;
	}

	private static int ClampEnd<T>(ReadOnlySpan<T> span, int start, ReadOnlySpan<T> trimElements) where T : IEquatable<T>
	{
		int num = span.Length - 1;
		while (num >= start && trimElements.Contains(span[num]))
		{
			num--;
		}
		return num - start + 1;
	}

	public static Memory<char> Trim(this Memory<char> memory)
	{
		ReadOnlySpan<char> span = memory.Span;
		int start = ClampStart(span);
		int length = ClampEnd(span, start);
		return memory.Slice(start, length);
	}

	public static Memory<char> TrimStart(this Memory<char> memory)
	{
		return memory.Slice(ClampStart(memory.Span));
	}

	public static Memory<char> TrimEnd(this Memory<char> memory)
	{
		return memory.Slice(0, ClampEnd(memory.Span, 0));
	}

	public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> memory)
	{
		ReadOnlySpan<char> span = memory.Span;
		int start = ClampStart(span);
		int length = ClampEnd(span, start);
		return memory.Slice(start, length);
	}

	public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> memory)
	{
		return memory.Slice(ClampStart(memory.Span));
	}

	public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> memory)
	{
		return memory.Slice(0, ClampEnd(memory.Span, 0));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span)
	{
		if (span.Length != 0)
		{
			if (!char.IsWhiteSpace(span[0]))
			{
				if (!char.IsWhiteSpace(span[span.Length - 1]))
				{
					goto IL_0030;
				}
			}
			return TrimFallback(span);
		}
		goto IL_0030;
		IL_0030:
		return span;
		[MethodImpl(MethodImplOptions.NoInlining)]
		static ReadOnlySpan<char> TrimFallback(ReadOnlySpan<char> readOnlySpan)
		{
			int i;
			for (i = 0; i < readOnlySpan.Length && char.IsWhiteSpace(readOnlySpan[i]); i++)
			{
			}
			int num = readOnlySpan.Length - 1;
			while (num > i && char.IsWhiteSpace(readOnlySpan[num]))
			{
				num--;
			}
			return readOnlySpan.Slice(i, num - i + 1);
		}
	}

	public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span)
	{
		int i;
		for (i = 0; i < span.Length && char.IsWhiteSpace(span[i]); i++)
		{
		}
		return span.Slice(i);
	}

	public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span)
	{
		int num = span.Length - 1;
		while (num >= 0 && char.IsWhiteSpace(span[num]))
		{
			num--;
		}
		return span.Slice(0, num + 1);
	}

	public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, char trimChar)
	{
		int i;
		for (i = 0; i < span.Length && span[i] == trimChar; i++)
		{
		}
		int num = span.Length - 1;
		while (num > i && span[num] == trimChar)
		{
			num--;
		}
		return span.Slice(i, num - i + 1);
	}

	public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, char trimChar)
	{
		int i;
		for (i = 0; i < span.Length && span[i] == trimChar; i++)
		{
		}
		return span.Slice(i);
	}

	public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, char trimChar)
	{
		int num = span.Length - 1;
		while (num >= 0 && span[num] == trimChar)
		{
			num--;
		}
		return span.Slice(0, num + 1);
	}

	public static ReadOnlySpan<char> Trim(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
	{
		return span.TrimStart(trimChars).TrimEnd(trimChars);
	}

	public static ReadOnlySpan<char> TrimStart(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
	{
		if (trimChars.IsEmpty)
		{
			return span.TrimStart();
		}
		int i;
		for (i = 0; i < span.Length; i++)
		{
			int num = 0;
			while (num < trimChars.Length)
			{
				if (span[i] != trimChars[num])
				{
					num++;
					continue;
				}
				goto IL_003c;
			}
			break;
			IL_003c:;
		}
		return span.Slice(i);
	}

	public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> span, ReadOnlySpan<char> trimChars)
	{
		if (trimChars.IsEmpty)
		{
			return span.TrimEnd();
		}
		int num;
		for (num = span.Length - 1; num >= 0; num--)
		{
			int num2 = 0;
			while (num2 < trimChars.Length)
			{
				if (span[num] != trimChars[num2])
				{
					num2++;
					continue;
				}
				goto IL_0044;
			}
			break;
			IL_0044:;
		}
		return span.Slice(0, num + 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<char> Trim(this Span<char> span)
	{
		if (span.Length != 0)
		{
			if (!char.IsWhiteSpace(span[0]))
			{
				if (!char.IsWhiteSpace(span[span.Length - 1]))
				{
					goto IL_0030;
				}
			}
			return TrimFallback(span);
		}
		goto IL_0030;
		IL_0030:
		return span;
		[MethodImpl(MethodImplOptions.NoInlining)]
		static Span<char> TrimFallback(Span<char> span2)
		{
			int i;
			for (i = 0; i < span2.Length && char.IsWhiteSpace(span2[i]); i++)
			{
			}
			int num = span2.Length - 1;
			while (num > i && char.IsWhiteSpace(span2[num]))
			{
				num--;
			}
			return span2.Slice(i, num - i + 1);
		}
	}

	public static Span<char> TrimStart(this Span<char> span)
	{
		return span.Slice(ClampStart(span));
	}

	public static Span<char> TrimEnd(this Span<char> span)
	{
		return span.Slice(0, ClampEnd(span, 0));
	}

	private static int ClampStart(ReadOnlySpan<char> span)
	{
		int i;
		for (i = 0; i < span.Length && char.IsWhiteSpace(span[i]); i++)
		{
		}
		return i;
	}

	private static int ClampEnd(ReadOnlySpan<char> span, int start)
	{
		int num = span.Length - 1;
		while (num >= start && char.IsWhiteSpace(span[num]))
		{
			num--;
		}
		return num - start + 1;
	}

	internal static ReadOnlySpan<byte> TrimUtf8(this ReadOnlySpan<byte> span)
	{
		if (span.Length == 0)
		{
			return span;
		}
		Rune.DecodeFromUtf8(span, out var result, out var bytesConsumed);
		if (Rune.IsWhiteSpace(result))
		{
			int num = bytesConsumed;
			span = span.Slice(num, span.Length - num);
			return TrimFallback(span);
		}
		Rune.DecodeLastFromUtf8(span, out var value, out var bytesConsumed2);
		if (Rune.IsWhiteSpace(value))
		{
			int num = bytesConsumed2;
			span = span.Slice(0, span.Length - num);
			return TrimFallback(span);
		}
		return span;
		[MethodImpl(MethodImplOptions.NoInlining)]
		static ReadOnlySpan<byte> TrimFallback(ReadOnlySpan<byte> readOnlySpan)
		{
			while (readOnlySpan.Length != 0)
			{
				Rune.DecodeFromUtf8(readOnlySpan, out var result2, out var bytesConsumed3);
				if (!Rune.IsWhiteSpace(result2))
				{
					break;
				}
				int num2 = bytesConsumed3;
				readOnlySpan = readOnlySpan.Slice(num2, readOnlySpan.Length - num2);
			}
			while (readOnlySpan.Length != 0)
			{
				Rune.DecodeLastFromUtf8(readOnlySpan, out var value2, out var bytesConsumed4);
				if (!Rune.IsWhiteSpace(value2))
				{
					break;
				}
				int num2 = bytesConsumed4;
				readOnlySpan = readOnlySpan.Slice(0, readOnlySpan.Length - num2);
			}
			return readOnlySpan;
		}
	}
}
