using System.Collections.Generic;
using System.Text;

namespace System.Buffers;

internal ref struct AhoCorasickBuilder
{
	private readonly ReadOnlySpan<string> _values;

	private readonly bool _ignoreCase;

	private ValueListBuilder<AhoCorasickNode> _nodes;

	private ValueListBuilder<int> _parents;

	private IndexOfAnyAsciiSearcher.AsciiState _startingAsciiChars;

	public AhoCorasickBuilder(ReadOnlySpan<string> values, bool ignoreCase, ref HashSet<string> unreachableValues)
	{
		_nodes = default(ValueListBuilder<AhoCorasickNode>);
		_parents = default(ValueListBuilder<int>);
		_startingAsciiChars = default(IndexOfAnyAsciiSearcher.AsciiState);
		_values = values;
		_ignoreCase = ignoreCase;
		BuildTrie(ref unreachableValues);
	}

	public AhoCorasick Build()
	{
		AddSuffixLinks();
		for (int i = 0; i < _nodes.Length; i++)
		{
			_nodes[i].OptimizeChildren();
		}
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported)
		{
			GenerateStartingAsciiCharsBitmap();
		}
		return new AhoCorasick(_nodes.AsSpan().ToArray(), _startingAsciiChars);
	}

	public void Dispose()
	{
		_nodes.Dispose();
		_parents.Dispose();
	}

	private void BuildTrie(ref HashSet<string> unreachableValues)
	{
		_nodes.Append(new AhoCorasickNode());
		_parents.Append(0);
		ReadOnlySpan<string> values = _values;
		for (int i = 0; i < values.Length; i++)
		{
			string text = values[i];
			int num = 0;
			ref AhoCorasickNode reference = ref _nodes[num];
			for (int j = 0; j < text.Length; j++)
			{
				char c = text[j];
				if (!reference.TryGetChild(c, out var index))
				{
					index = _nodes.Length;
					reference.AddChild(c, index);
					_nodes.Append(new AhoCorasickNode());
					_parents.Append(num);
				}
				reference = ref _nodes[index];
				num = index;
				if (reference.MatchLength != 0)
				{
					if (unreachableValues == null)
					{
						unreachableValues = new HashSet<string>(StringComparer.Ordinal);
					}
					unreachableValues.Add(text);
					break;
				}
				if (j == text.Length - 1)
				{
					reference.MatchLength = text.Length;
					break;
				}
			}
		}
	}

	private void AddSuffixLinks()
	{
		Queue<(char, int)> queue = new Queue<(char, int)>();
		queue.Enqueue(('\0', 0));
		(char, int) result;
		while (queue.TryDequeue(out result))
		{
			ref AhoCorasickNode reference = ref _nodes[result.Item2];
			int num = _parents[result.Item2];
			int num2 = _nodes[num].SuffixLink;
			if (num != 0)
			{
				while (num2 >= 0)
				{
					ref AhoCorasickNode reference2 = ref _nodes[num2];
					if (reference2.TryGetChild(result.Item1, out var index))
					{
						num2 = index;
						break;
					}
					if (num2 == 0)
					{
						break;
					}
					num2 = reference2.SuffixLink;
				}
			}
			if (reference.MatchLength != 0)
			{
				reference.SuffixLink = -1;
				continue;
			}
			reference.SuffixLink = num2;
			if (num2 >= 0)
			{
				reference.MatchLength = _nodes[num2].MatchLength;
			}
			reference.AddChildrenToQueue(queue);
		}
	}

	private void GenerateStartingAsciiCharsBitmap()
	{
		Span<char> scratchBuffer = stackalloc char[128];
		ValueListBuilder<char> valueListBuilder = new ValueListBuilder<char>(scratchBuffer);
		ReadOnlySpan<string> values = _values;
		for (int i = 0; i < values.Length; i++)
		{
			char c = values[i][0];
			if (_ignoreCase)
			{
				valueListBuilder.Append(char.ToLowerInvariant(c));
				valueListBuilder.Append(char.ToUpperInvariant(c));
			}
			else
			{
				valueListBuilder.Append(c);
			}
		}
		if (Ascii.IsValid(valueListBuilder.AsSpan()))
		{
			IndexOfAnyAsciiSearcher.ComputeAsciiState(valueListBuilder.AsSpan(), out _startingAsciiChars);
		}
		valueListBuilder.Dispose();
	}
}
