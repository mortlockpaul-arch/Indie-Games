using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions;

internal sealed class RegexReplacement
{
	[InlineArray(4)]
	private struct FourStackStrings
	{
		private string _item1;
	}

	private readonly string[] _strings;

	private readonly int[] _rules;

	private readonly bool _hasBackreferences;

	public string Pattern { get; }

	public RegexReplacement(string rep, RegexNode concat, Hashtable _caps)
	{
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		FourStackStrings buffer = default(FourStackStrings);
		System.Collections.Generic.ValueListBuilder<string> valueListBuilder = new System.Collections.Generic.ValueListBuilder<string>(buffer);
		Span<int> scratchBuffer = stackalloc int[64];
		System.Collections.Generic.ValueListBuilder<int> valueListBuilder2 = new System.Collections.Generic.ValueListBuilder<int>(scratchBuffer);
		int num = concat.ChildCount();
		for (int i = 0; i < num; i++)
		{
			RegexNode regexNode = concat.Child(i);
			switch (regexNode.Kind)
			{
			case RegexNodeKind.Multi:
				valueStringBuilder.Append(regexNode.Str);
				break;
			case RegexNodeKind.One:
				valueStringBuilder.Append(regexNode.Ch);
				break;
			case RegexNodeKind.Backreference:
			{
				if (valueStringBuilder.Length > 0)
				{
					valueListBuilder2.Append(valueListBuilder.Length);
					valueListBuilder.Append(valueStringBuilder.AsSpan().ToString());
					valueStringBuilder.Length = 0;
				}
				int num2 = regexNode.M;
				if (_caps != null && num2 >= 0)
				{
					num2 = (int)_caps[num2];
				}
				valueListBuilder2.Append(-5 - num2);
				_hasBackreferences = true;
				break;
			}
			}
		}
		if (valueStringBuilder.Length > 0)
		{
			valueListBuilder2.Append(valueListBuilder.Length);
			valueListBuilder.Append(valueStringBuilder.ToString());
		}
		valueStringBuilder.Dispose();
		Pattern = rep;
		_strings = valueListBuilder.AsSpan().ToArray();
		_rules = valueListBuilder2.AsSpan().ToArray();
		valueListBuilder2.Dispose();
	}

	public static RegexReplacement GetOrCreate(WeakReference<RegexReplacement> replRef, string replacement, Hashtable caps, int capsize, Hashtable capnames, RegexOptions roptions)
	{
		if (!replRef.TryGetTarget(out var target) || !target.Pattern.Equals(replacement))
		{
			target = RegexParser.ParseReplacement(replacement, roptions, caps, capsize, capnames);
			replRef.SetTarget(target);
		}
		return target;
	}

	public void ReplacementImpl(ref StructListBuilder<ReadOnlyMemory<char>> segments, Match match)
	{
		int[] rules = _rules;
		foreach (int num in rules)
		{
			ReadOnlyMemory<char> readOnlyMemory;
			if (num >= 0)
			{
				readOnlyMemory = _strings[num].AsMemory();
			}
			else
			{
				ReadOnlyMemory<char> readOnlyMemory2 = ((num >= -4) ? ((-5 - num) switch
				{
					-1 => match.GetLeftSubstring(), 
					-2 => match.GetRightSubstring(), 
					-3 => match.LastGroupToStringImpl(), 
					-4 => match.Text.AsMemory(), 
					_ => default(ReadOnlyMemory<char>), 
				}) : match.GroupToStringImpl(-5 - num));
				readOnlyMemory = readOnlyMemory2;
			}
			ReadOnlyMemory<char> item = readOnlyMemory;
			if (item.Length != 0)
			{
				segments.Add(item);
			}
		}
	}

	public void ReplacementImplRTL(ref StructListBuilder<ReadOnlyMemory<char>> segments, Match match)
	{
		for (int num = _rules.Length - 1; num >= 0; num--)
		{
			int num2 = _rules[num];
			ReadOnlyMemory<char> readOnlyMemory;
			if (num2 >= 0)
			{
				readOnlyMemory = _strings[num2].AsMemory();
			}
			else
			{
				ReadOnlyMemory<char> readOnlyMemory2 = ((num2 >= -4) ? ((-5 - num2) switch
				{
					-1 => match.GetLeftSubstring(), 
					-2 => match.GetRightSubstring(), 
					-3 => match.LastGroupToStringImpl(), 
					-4 => match.Text.AsMemory(), 
					_ => default(ReadOnlyMemory<char>), 
				}) : match.GroupToStringImpl(-5 - num2));
				readOnlyMemory = readOnlyMemory2;
			}
			ReadOnlyMemory<char> item = readOnlyMemory;
			if (item.Length != 0)
			{
				segments.Add(item);
			}
		}
	}

	public string Replace(Regex regex, string input, int count, int startat)
	{
		if (count < -1)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.CountTooSmall);
		}
		if ((uint)startat > (uint)input.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startat, ExceptionResource.BeginIndexNotNegative);
		}
		if (count == 0)
		{
			return input;
		}
		if (!regex.RightToLeft && !_hasBackreferences)
		{
			return ReplaceSimpleText(regex, input, (_rules.Length != 0) ? _strings[0] : "", count, startat);
		}
		return ReplaceNonSimpleText(regex, input, count, startat);
	}

	private unsafe static string ReplaceSimpleText(Regex regex, string input, string replacement, int count, int startat)
	{
		(string, string, StructListBuilder<int>, ReadOnlyMemory<char>, int, int) state = (input, replacement, new StructListBuilder<int>(), input.AsMemory(), 0, count);
		string result = input;
		regex.RunAllMatchesWithCallback<(string, string, StructListBuilder<int>, ReadOnlyMemory<char>, int, int)>(input, startat, ref state, delegate(ref (string input, string replacement, StructListBuilder<int> segments, ReadOnlyMemory<char> inputMemory, int prevat, int count) reference, Match match)
		{
			reference.segments.Add(reference.prevat);
			reference.segments.Add(match.Index - reference.prevat);
			reference.prevat = match.Index + match.Length;
			return --reference.count != 0;
		}, RegexRunnerMode.BoundsRequired, reuseMatchObject: true);
		if (state.Item3.Count != 0)
		{
			state.Item3.Add(state.Item5);
			state.Item3.Add(input.Length - state.Item5);
			Span<int> span = state.Item3.AsSpan();
			int num = (span.Length / 2 - 1) * replacement.Length;
			for (int num2 = 1; num2 < span.Length; num2 += 2)
			{
				num += span[num2];
			}
			ReadOnlySpan<int> readOnlySpan = span;
			result = string.Create(num, ((nint)(&readOnlySpan), input, replacement), delegate(Span<char> dest, (nint, string input, string replacement) tuple)
			{
				ReadOnlySpan<int> item = Unsafe.Read<ReadOnlySpan<int>>((void*)tuple.Item1);
				for (int i = 0; i < item.Length; i += 2)
				{
					if (i != 0)
					{
						tuple.replacement.CopyTo(dest);
						dest = dest.Slice(tuple.replacement.Length);
					}
					int num3 = item[i];
					int num4 = item[i + 1];
					int start = num3;
					tuple.input.AsSpan(start, num4).CopyTo(dest);
					dest = dest.Slice(num4);
				}
			});
		}
		state.Item3.Dispose();
		return result;
	}

	private string ReplaceNonSimpleText(Regex regex, string input, int count, int startat)
	{
		(RegexReplacement, StructListBuilder<ReadOnlyMemory<char>>, ReadOnlyMemory<char>, int, int) state = (this, new StructListBuilder<ReadOnlyMemory<char>>(), input.AsMemory(), 0, count);
		if (!regex.RightToLeft)
		{
			regex.RunAllMatchesWithCallback<(RegexReplacement, StructListBuilder<ReadOnlyMemory<char>>, ReadOnlyMemory<char>, int, int)>(input, startat, ref state, delegate(ref (RegexReplacement thisRef, StructListBuilder<ReadOnlyMemory<char>> segments, ReadOnlyMemory<char> inputMemory, int prevat, int count) reference, Match match)
			{
				reference.segments.Add(reference.inputMemory.Slice(reference.prevat, match.Index - reference.prevat));
				reference.prevat = match.Index + match.Length;
				reference.thisRef.ReplacementImpl(ref reference.segments, match);
				return --reference.count != 0;
			}, (!_hasBackreferences) ? RegexRunnerMode.BoundsRequired : RegexRunnerMode.FullMatchRequired, reuseMatchObject: true);
			if (state.Item2.Count == 0)
			{
				return input;
			}
			state.Item2.Add(state.Item3.Slice(state.Item4));
		}
		else
		{
			state.Item4 = input.Length;
			regex.RunAllMatchesWithCallback<(RegexReplacement, StructListBuilder<ReadOnlyMemory<char>>, ReadOnlyMemory<char>, int, int)>(input, startat, ref state, delegate(ref (RegexReplacement thisRef, StructListBuilder<ReadOnlyMemory<char>> segments, ReadOnlyMemory<char> inputMemory, int prevat, int count) reference, Match match)
			{
				reference.segments.Add(reference.inputMemory.Slice(match.Index + match.Length, reference.prevat - match.Index - match.Length));
				reference.prevat = match.Index;
				reference.thisRef.ReplacementImplRTL(ref reference.segments, match);
				return --reference.count != 0;
			}, (!_hasBackreferences) ? RegexRunnerMode.BoundsRequired : RegexRunnerMode.FullMatchRequired, reuseMatchObject: true);
			if (state.Item2.Count == 0)
			{
				return input;
			}
			state.Item2.Add(state.Item3.Slice(0, state.Item4));
			state.Item2.AsSpan().Reverse();
		}
		return Regex.SegmentsToStringAndDispose(ref state.Item2);
	}
}
