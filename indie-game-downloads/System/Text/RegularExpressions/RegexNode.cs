using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace System.Text.RegularExpressions;

internal sealed class RegexNode
{
	public readonly struct StartingLiteralData
	{
		public readonly (char LowInclusive, char HighInclusive) Range;

		public readonly string String;

		public readonly string SetChars;

		public readonly bool Negated;

		public StartingLiteralData((char LowInclusive, char HighInclusive) range, bool negated)
		{
			String = null;
			SetChars = null;
			Range = range;
			Negated = negated;
		}

		public StartingLiteralData(string @string)
		{
			Range = default((char, char));
			SetChars = null;
			Negated = false;
			String = @string;
		}

		public StartingLiteralData(string setChars, bool negated)
		{
			Range = default((char, char));
			String = null;
			SetChars = setChars;
			Negated = negated;
		}
	}

	private object Children;

	public RegexOptions Options;

	public RegexNode Parent;

	public RegexNodeKind Kind { get; private set; }

	public string Str { get; private set; }

	public char Ch { get; private set; }

	public int M { get; private set; }

	public int N { get; private set; }

	public bool IsBacktrackingConstruct
	{
		get
		{
			switch (Kind)
			{
			case RegexNodeKind.Alternate:
				return true;
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (M != N)
				{
					return true;
				}
				break;
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Setlazy:
				if (M != N)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}

	[MemberNotNullWhen(true, "Str")]
	public bool IsSetFamily
	{
		[MemberNotNullWhen(true, "Str")]
		get
		{
			switch (Kind)
			{
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Setlazy:
			case RegexNodeKind.Set:
			case RegexNodeKind.Setloopatomic:
				return true;
			default:
				return false;
			}
		}
	}

	public bool IsOneFamily
	{
		get
		{
			switch (Kind)
			{
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.One:
			case RegexNodeKind.Oneloopatomic:
				return true;
			default:
				return false;
			}
		}
	}

	public bool IsNotoneFamily
	{
		get
		{
			switch (Kind)
			{
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Notone:
			case RegexNodeKind.Notoneloopatomic:
				return true;
			default:
				return false;
			}
		}
	}

	public RegexNode(RegexNodeKind kind, RegexOptions options)
	{
		Kind = kind;
		Options = options;
	}

	public RegexNode(RegexNodeKind kind, RegexOptions options, char ch)
	{
		Kind = kind;
		Options = options;
		Ch = ch;
	}

	public RegexNode(RegexNodeKind kind, RegexOptions options, string str)
	{
		Kind = kind;
		Options = options;
		Str = str;
	}

	public RegexNode(RegexNodeKind kind, RegexOptions options, int m)
	{
		Kind = kind;
		Options = options;
		M = m;
	}

	public RegexNode(RegexNodeKind kind, RegexOptions options, int m, int n)
	{
		Kind = kind;
		Options = options;
		M = m;
		N = n;
	}

	public static RegexNode CreateOneWithCaseConversion(char ch, RegexOptions options, CultureInfo culture, ref RegexCaseBehavior caseBehavior)
	{
		if ((options & RegexOptions.IgnoreCase) != RegexOptions.None)
		{
			if (!RegexCaseEquivalences.TryFindCaseEquivalencesForCharWithIBehavior(ch, culture, ref caseBehavior, out var equivalences))
			{
				return new RegexNode(RegexNodeKind.One, options & ~RegexOptions.IgnoreCase, ch);
			}
			string str = RegexCharClass.CharsToStringClass(equivalences);
			return new RegexNode(RegexNodeKind.Set, options & ~RegexOptions.IgnoreCase, str);
		}
		return new RegexNode(RegexNodeKind.One, options, ch);
	}

	public RegexNode ReverseConcatenationIfRightToLeft()
	{
		if ((Options & RegexOptions.RightToLeft) != RegexOptions.None && Kind == RegexNodeKind.Concatenate && ChildCount() > 1)
		{
			((List<RegexNode>)Children).Reverse();
		}
		return this;
	}

	private void MakeRep(RegexNodeKind kind, int min, int max)
	{
		Kind += (byte)(kind - 9);
		M = min;
		N = max;
	}

	private void MakeLoopAtomic(bool noBacktrackingByPosition = false)
	{
		RegexNodeKind kind = Kind;
		if (kind <= RegexNodeKind.Setlazy)
		{
			switch (kind)
			{
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Setloop:
				Kind += 40;
				break;
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Setlazy:
				Kind += 37;
				if (noBacktrackingByPosition)
				{
					N = M;
				}
				if (N == 0)
				{
					Kind = RegexNodeKind.Empty;
					Str = null;
					Ch = '\0';
				}
				else if (Kind == RegexNodeKind.Oneloopatomic)
				{
					int n = N;
					if (n >= 2 && n <= 64)
					{
						Kind = RegexNodeKind.Multi;
						Str = new string(Ch, N);
						Ch = '\0';
						n = (N = 0);
						M = n;
					}
				}
				break;
			}
			return;
		}
		if (kind != RegexNodeKind.Loop)
		{
			if (kind != RegexNodeKind.Lazyloop)
			{
				return;
			}
			if (noBacktrackingByPosition)
			{
				N = M;
			}
			if (N == 0)
			{
				Kind = RegexNodeKind.Empty;
				Children = null;
				return;
			}
		}
		RegexNode parent = Parent;
		if (parent == null || parent.Kind != RegexNodeKind.Atomic)
		{
			RegexNode regexNode = new RegexNode(RegexNodeKind.Loop, Options, M, N);
			Kind = RegexNodeKind.Atomic;
			int n = (N = 0);
			M = n;
			regexNode.AddChild(Child(0));
			ReplaceChild(0, regexNode);
		}
	}

	internal RegexNode FinalOptimize()
	{
		if ((Options & (RegexOptions.RightToLeft | RegexOptions.NonBacktracking)) == 0)
		{
			FindAndMakeLoopsAtomic();
			EliminateEndingBacktracking();
			RegexNode regexNode = Child(0);
			bool flag = true;
			while (true)
			{
				RegexNodeKind kind = regexNode.Kind;
				if (kind <= RegexNodeKind.Setlazy)
				{
					if (kind - 3 > (RegexNodeKind)2)
					{
						if (kind - 6 > (RegexNodeKind)2 || regexNode.N != int.MaxValue || flag)
						{
							break;
						}
						goto IL_0091;
					}
				}
				else
				{
					if (kind == RegexNodeKind.Concatenate)
					{
						flag = false;
						regexNode = regexNode.Child(0);
						continue;
					}
					if (kind == RegexNodeKind.Atomic)
					{
						regexNode = regexNode.Child(0);
						continue;
					}
					if (kind - 43 > (RegexNodeKind)2)
					{
						break;
					}
				}
				if (regexNode.N != int.MaxValue)
				{
					break;
				}
				goto IL_0091;
				IL_0091:
				RegexNode parent = regexNode.Parent;
				if (parent != null && parent.Kind == RegexNodeKind.Concatenate)
				{
					parent.InsertChild(1, new RegexNode(RegexNodeKind.UpdateBumpalong, regexNode.Options));
				}
				break;
			}
		}
		return this;
	}

	private void EliminateEndingBacktracking()
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack() || (Options & RegexOptions.NonBacktracking) != RegexOptions.None)
		{
			return;
		}
		RegexNode regexNode = this;
		while (true)
		{
			bool flag = (regexNode.Options & RegexOptions.RightToLeft) != 0;
			switch (regexNode.Kind)
			{
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Setlazy:
				regexNode.MakeLoopAtomic(noBacktrackingByPosition: true);
				return;
			case RegexNodeKind.PositiveLookaround:
			case RegexNodeKind.NegativeLookaround:
			case RegexNodeKind.Atomic:
				regexNode = regexNode.Child(0);
				break;
			case RegexNodeKind.Concatenate:
				if (flag)
				{
					return;
				}
				goto case RegexNodeKind.Capture;
			case RegexNodeKind.Capture:
			{
				RegexNode regexNode2 = regexNode.Child(regexNode.ChildCount() - 1);
				RegexNodeKind kind = regexNode2.Kind;
				if ((kind == RegexNodeKind.Alternate || kind - 26 <= (RegexNodeKind)1 || kind - 33 <= (RegexNodeKind)1) ? true : false)
				{
					RegexNode parent = regexNode.Parent;
					if (parent == null || parent.Kind != RegexNodeKind.Atomic)
					{
						RegexNode regexNode3 = new RegexNode(RegexNodeKind.Atomic, regexNode2.Options);
						regexNode3.AddChild(regexNode2);
						regexNode.ReplaceChild(regexNode.ChildCount() - 1, regexNode3);
					}
				}
				regexNode = regexNode2;
				break;
			}
			case RegexNodeKind.Alternate:
			case RegexNodeKind.BackreferenceConditional:
			case RegexNodeKind.ExpressionConditional:
			{
				int num = regexNode.ChildCount();
				for (int i = 1; i < num; i++)
				{
					regexNode.Child(i).EliminateEndingBacktracking();
				}
				if (regexNode.Kind != RegexNodeKind.ExpressionConditional)
				{
					regexNode = regexNode.Child(0);
					break;
				}
				return;
			}
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
			{
				regexNode.MakeLoopAtomic(noBacktrackingByPosition: true);
				if (regexNode.Kind == RegexNodeKind.Atomic)
				{
					regexNode = regexNode.Child(0);
				}
				RegexNodeKind kind = regexNode.Kind;
				bool flag2 = kind - 26 <= (RegexNodeKind)1;
				if (flag2 && (regexNode.N == 1 || CanBeMadeAtomic(regexNode.Child(0), regexNode.Child(0), iterateNullableSubsequent: false, allowLazy: false)))
				{
					regexNode = regexNode.Child(0);
					break;
				}
				return;
			}
			default:
				return;
			}
		}
	}

	internal RegexNode Reduce()
	{
		if (Kind != RegexNodeKind.Backreference)
		{
			Options &= ~RegexOptions.IgnoreCase;
		}
		switch (Kind)
		{
		case RegexNodeKind.Alternate:
			return ReduceAlternation();
		case RegexNodeKind.Atomic:
			return ReduceAtomic();
		case RegexNodeKind.Concatenate:
			return ReduceConcatenation();
		case RegexNodeKind.Group:
			return ReduceGroup();
		case RegexNodeKind.Loop:
		case RegexNodeKind.Lazyloop:
			return ReduceLoops();
		case RegexNodeKind.PositiveLookaround:
		case RegexNodeKind.NegativeLookaround:
			return ReduceLookaround();
		case RegexNodeKind.Setloop:
		case RegexNodeKind.Setlazy:
		case RegexNodeKind.Set:
		case RegexNodeKind.Setloopatomic:
			return ReduceSet();
		case RegexNodeKind.ExpressionConditional:
			return ReduceExpressionConditional();
		case RegexNodeKind.BackreferenceConditional:
			return ReduceBackreferenceConditional();
		default:
			return this;
		}
	}

	private RegexNode ReplaceNodeIfUnnecessary()
	{
		return ChildCount() switch
		{
			0 => new RegexNode((Kind == RegexNodeKind.Alternate) ? RegexNodeKind.Nothing : RegexNodeKind.Empty, Options), 
			1 => Child(0), 
			_ => this, 
		};
	}

	private RegexNode ReduceGroup()
	{
		RegexNode regexNode = this;
		while (regexNode.Kind == RegexNodeKind.Group)
		{
			regexNode = regexNode.Child(0);
		}
		return regexNode;
	}

	private RegexNode ReduceAtomic()
	{
		if ((Options & RegexOptions.NonBacktracking) != RegexOptions.None)
		{
			return this;
		}
		RegexNode regexNode = this;
		RegexNode regexNode2 = Child(0);
		while (regexNode2.Kind == RegexNodeKind.Atomic)
		{
			regexNode = regexNode2;
			regexNode2 = regexNode.Child(0);
		}
		switch (regexNode2.Kind)
		{
		case RegexNodeKind.Nothing:
		case RegexNodeKind.Empty:
			return regexNode2;
		case RegexNodeKind.Oneloopatomic:
		case RegexNodeKind.Notoneloopatomic:
		case RegexNodeKind.Setloopatomic:
			return regexNode2;
		case RegexNodeKind.Oneloop:
		case RegexNodeKind.Notoneloop:
		case RegexNodeKind.Setloop:
		case RegexNodeKind.Onelazy:
		case RegexNodeKind.Notonelazy:
		case RegexNodeKind.Setlazy:
			regexNode2.MakeLoopAtomic(noBacktrackingByPosition: true);
			return regexNode2;
		case RegexNodeKind.Alternate:
		{
			if ((Options & RegexOptions.RightToLeft) != RegexOptions.None)
			{
				break;
			}
			List<RegexNode> list = regexNode2.Children as List<RegexNode>;
			if (list[0].Kind == RegexNodeKind.Empty)
			{
				return new RegexNode(RegexNodeKind.Empty, regexNode2.Options);
			}
			for (int i = 1; i < list.Count - 1; i++)
			{
				if (list[i].Kind == RegexNodeKind.Empty)
				{
					list.RemoveRange(i + 1, list.Count - (i + 1));
					break;
				}
			}
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].FindBranchOneOrMultiStart() == null)
				{
					continue;
				}
				int k;
				for (k = j + 1; k < list.Count && list[k].FindBranchOneOrMultiStart() != null; k++)
				{
				}
				if (k - j >= 3)
				{
					int l = j;
					while (l < k)
					{
						char c;
						for (c = list[l].FindBranchOneOrMultiStart().FirstCharOfOneOrMulti(); l < k && list[l].FindBranchOneOrMultiStart().FirstCharOfOneOrMulti() == c; l++)
						{
						}
						if (l >= k)
						{
							continue;
						}
						for (int m = l + 1; m < k; m++)
						{
							RegexNode regexNode3 = list[m];
							if (regexNode3.FindBranchOneOrMultiStart().FirstCharOfOneOrMulti() == c)
							{
								list.RemoveAt(m);
								list.Insert(l++, regexNode3);
								flag = true;
							}
						}
					}
				}
				j = k;
			}
			if (flag | (regexNode2.ChildCount() == 2 && (regexNode2.Child(0).Kind == RegexNodeKind.Empty || regexNode2.Child(1).Kind == RegexNodeKind.Empty)))
			{
				regexNode.ReplaceChild(0, regexNode2);
				regexNode2 = regexNode.Child(0);
			}
			break;
		}
		}
		regexNode2.EliminateEndingBacktracking();
		if (regexNode2.Kind != RegexNodeKind.Empty)
		{
			return regexNode;
		}
		return regexNode2;
	}

	private RegexNode ReduceLoops()
	{
		RegexNode regexNode = this;
		RegexNodeKind kind = Kind;
		int num = M;
		int num2 = N;
		while (regexNode.ChildCount() > 0)
		{
			RegexNode regexNode2 = regexNode.Child(0);
			if (regexNode2.Kind != kind)
			{
				bool flag = false;
				if (kind == RegexNodeKind.Loop)
				{
					RegexNodeKind kind2 = regexNode2.Kind;
					if (kind2 - 3 <= (RegexNodeKind)2 || kind2 - 43 <= (RegexNodeKind)2)
					{
						flag = true;
					}
				}
				else
				{
					RegexNodeKind kind2 = regexNode2.Kind;
					if (kind2 - 6 <= (RegexNodeKind)2)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if ((regexNode.M == 0 && regexNode2.M > 1) || regexNode2.N < regexNode2.M * 2)
			{
				break;
			}
			regexNode = regexNode2;
			if (regexNode.M > 0)
			{
				num = (regexNode.M = ((2147483646 / regexNode.M < num) ? int.MaxValue : (regexNode.M * num)));
			}
			if (regexNode.N > 0)
			{
				num2 = (regexNode.N = ((2147483646 / regexNode.N < num2) ? int.MaxValue : (regexNode.N * num2)));
			}
		}
		if (num == int.MaxValue)
		{
			return new RegexNode(RegexNodeKind.Nothing, Options);
		}
		if (regexNode.ChildCount() == 1)
		{
			RegexNode regexNode3 = regexNode.Child(0);
			switch (regexNode3.Kind)
			{
			case RegexNodeKind.One:
			case RegexNodeKind.Notone:
			case RegexNodeKind.Set:
				regexNode3.MakeRep((regexNode.Kind == RegexNodeKind.Lazyloop) ? RegexNodeKind.Onelazy : RegexNodeKind.Oneloop, regexNode.M, regexNode.N);
				regexNode = regexNode3;
				break;
			case RegexNodeKind.Empty:
				regexNode = regexNode3;
				break;
			case RegexNodeKind.PositiveLookaround:
				if (ContainsKind(regexNode3, new RegexNodeKind[1] { RegexNodeKind.Capture }) ?? true)
				{
					break;
				}
				goto case RegexNodeKind.Bol;
			case RegexNodeKind.Bol:
			case RegexNodeKind.Eol:
			case RegexNodeKind.Boundary:
			case RegexNodeKind.NonBoundary:
			case RegexNodeKind.Beginning:
			case RegexNodeKind.Start:
			case RegexNodeKind.EndZ:
			case RegexNodeKind.End:
			case RegexNodeKind.NegativeLookaround:
			case RegexNodeKind.ECMABoundary:
			case RegexNodeKind.NonECMABoundary:
				regexNode = ((regexNode.M == 0) ? new RegexNode(RegexNodeKind.Empty, Options) : regexNode3);
				break;
			}
		}
		return regexNode;
	}

	private RegexNode ReduceSet()
	{
		if (RegexCharClass.IsEmpty(Str))
		{
			Kind = RegexNodeKind.Nothing;
			Str = null;
		}
		else if (RegexCharClass.IsSingleton(Str))
		{
			Ch = RegexCharClass.SingletonChar(Str);
			Str = null;
			Kind = ((Kind == RegexNodeKind.Set) ? RegexNodeKind.One : ((Kind == RegexNodeKind.Setloop) ? RegexNodeKind.Oneloop : ((Kind == RegexNodeKind.Setloopatomic) ? RegexNodeKind.Oneloopatomic : RegexNodeKind.Onelazy)));
		}
		else if (RegexCharClass.IsSingletonInverse(Str))
		{
			Ch = RegexCharClass.SingletonChar(Str);
			Str = null;
			Kind = ((Kind == RegexNodeKind.Set) ? RegexNodeKind.Notone : ((Kind == RegexNodeKind.Setloop) ? RegexNodeKind.Notoneloop : ((Kind == RegexNodeKind.Setloopatomic) ? RegexNodeKind.Notoneloopatomic : RegexNodeKind.Notonelazy)));
		}
		string str = Str;
		if (str != null)
		{
			int length = str.Length;
			if (length <= 5)
			{
				if (length != 4)
				{
					if (length == 5)
					{
						char c = str[3];
						if ((uint)c <= 100u)
						{
							if (c != '\t')
							{
								if (c == 'd' && str == "\0\0\u0002dﾜ")
								{
									goto IL_01f4;
								}
							}
							else if (str == "\0\0\u0002\t\ufff7")
							{
								goto IL_01f4;
							}
						}
						else if (c != 'ﾜ')
						{
							if (c == '\ufff7' && str == "\0\0\u0002\ufff7\t")
							{
								goto IL_01f4;
							}
						}
						else if (str == "\0\0\u0002ﾜd")
						{
							goto IL_01f4;
						}
					}
				}
				else
				{
					switch (str[3])
					{
					case '\t':
						if (str == "\u0001\0\u0001\t")
						{
							Str = "\0\0\u0001\ufff7";
						}
						break;
					case 'd':
						if (str == "\u0001\0\u0001d")
						{
							Str = "\0\0\u0001ﾜ";
						}
						break;
					}
				}
			}
			else if (length != 13)
			{
				if (length == 23)
				{
					char c = str[4];
					if (c != '\u0002')
					{
						if (c == '\ufffe' && str == "\0\0\u0014\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0")
						{
							goto IL_01f4;
						}
					}
					else if (str == "\0\0\u0014\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0")
					{
						goto IL_01f4;
					}
				}
			}
			else if (str == "\u0001\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0")
			{
				Str = "\0\0\n\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0";
			}
		}
		goto IL_0226;
		IL_01f4:
		Str = "\0\u0001\0\0";
		goto IL_0226;
		IL_0226:
		return this;
	}

	private RegexNode ReduceAlternation()
	{
		switch (ChildCount())
		{
		case 0:
			return new RegexNode(RegexNodeKind.Nothing, Options);
		case 1:
			return Child(0);
		default:
		{
			ReduceSingleLetterAndNestedAlternations();
			RegexNode regexNode = ReplaceNodeIfUnnecessary();
			if (regexNode.Kind == RegexNodeKind.Alternate)
			{
				regexNode = ExtractCommonPrefixText(regexNode);
				if (regexNode.Kind == RegexNodeKind.Alternate)
				{
					regexNode = ExtractCommonPrefixNode(regexNode);
					if (regexNode.Kind == RegexNodeKind.Alternate)
					{
						regexNode = RemoveRedundantEmptiesAndNothings(regexNode);
						if (regexNode.Kind == RegexNodeKind.Alternate && regexNode.ChildCount() == 2)
						{
							if (regexNode.Child(1).Kind == RegexNodeKind.Empty)
							{
								regexNode = regexNode.Child(0).MakeQuantifier(lazy: false, 0, 1);
							}
							else if (regexNode.Child(0).Kind == RegexNodeKind.Empty)
							{
								regexNode = regexNode.Child(1).MakeQuantifier(lazy: true, 0, 1);
							}
						}
					}
				}
			}
			return regexNode;
		}
		}
		static RegexNode ExtractCommonPrefixNode(RegexNode alternation)
		{
			List<RegexNode> list = (List<RegexNode>)alternation.Children;
			if ((alternation.Options & RegexOptions.RightToLeft) != RegexOptions.None)
			{
				return alternation;
			}
			foreach (RegexNode item in list)
			{
				if (item.Kind != RegexNodeKind.Concatenate || item.ChildCount() < 2)
				{
					return alternation;
				}
			}
			for (int i = 0; i < list.Count - 1; i++)
			{
				RegexNode regexNode2 = list[i].Child(0);
				switch (regexNode2.Kind)
				{
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Notoneloop:
				case RegexNodeKind.Setloop:
				case RegexNodeKind.Onelazy:
				case RegexNodeKind.Notonelazy:
				case RegexNodeKind.Setlazy:
					if (regexNode2.M != regexNode2.N)
					{
						continue;
					}
					break;
				case RegexNodeKind.One:
				case RegexNodeKind.Notone:
				case RegexNodeKind.Set:
				case RegexNodeKind.Bol:
				case RegexNodeKind.Eol:
				case RegexNodeKind.Boundary:
				case RegexNodeKind.NonBoundary:
				case RegexNodeKind.Beginning:
				case RegexNodeKind.Start:
				case RegexNodeKind.EndZ:
				case RegexNodeKind.End:
				case RegexNodeKind.ECMABoundary:
				case RegexNodeKind.NonECMABoundary:
				case RegexNodeKind.Oneloopatomic:
				case RegexNodeKind.Notoneloopatomic:
				case RegexNodeKind.Setloopatomic:
					break;
				default:
					continue;
				}
				int j;
				for (j = i + 1; j < list.Count; j++)
				{
					RegexNode regexNode3 = list[j].Child(0);
					if (regexNode2.Kind != regexNode3.Kind || regexNode2.Options != regexNode3.Options || regexNode2.M != regexNode3.M || regexNode2.N != regexNode3.N || regexNode2.Ch != regexNode3.Ch || regexNode2.Str != regexNode3.Str)
					{
						break;
					}
				}
				if (j - i > 1)
				{
					RegexNode regexNode4 = new RegexNode(RegexNodeKind.Alternate, alternation.Options);
					for (int k = i; k < j; k++)
					{
						((List<RegexNode>)list[k].Children).RemoveAt(0);
						regexNode4.AddChild(list[k]);
					}
					RegexNode parent = alternation.Parent;
					if (parent != null && parent.Kind == RegexNodeKind.Atomic)
					{
						RegexNode regexNode5 = new RegexNode(RegexNodeKind.Atomic, alternation.Options);
						regexNode5.AddChild(regexNode4);
						regexNode4 = regexNode5;
					}
					RegexNode regexNode6 = new RegexNode(RegexNodeKind.Concatenate, alternation.Options);
					regexNode6.AddChild(regexNode2);
					regexNode6.AddChild(regexNode4);
					alternation.ReplaceChild(i, regexNode6);
					list.RemoveRange(i + 1, j - i - 1);
				}
			}
			return alternation.ReplaceNodeIfUnnecessary();
		}
		static RegexNode ExtractCommonPrefixText(RegexNode alternation)
		{
			List<RegexNode> list = (List<RegexNode>)alternation.Children;
			if ((alternation.Options & RegexOptions.RightToLeft) != RegexOptions.None)
			{
				return alternation;
			}
			Span<char> span = stackalloc char[1];
			for (int i = 0; i < list.Count - 1; i++)
			{
				RegexNode regexNode2 = list[i].FindBranchOneOrMultiStart();
				if (regexNode2 == null)
				{
					return alternation;
				}
				RegexOptions options = regexNode2.Options;
				ReadOnlySpan<char> startingSpan = regexNode2.Str.AsSpan();
				if (regexNode2.Kind == RegexNodeKind.One)
				{
					span[0] = regexNode2.Ch;
					startingSpan = span;
				}
				int j;
				for (j = i + 1; j < list.Count; j++)
				{
					regexNode2 = list[j].FindBranchOneOrMultiStart();
					if (regexNode2 == null || regexNode2.Options != options)
					{
						break;
					}
					if (regexNode2.Kind == RegexNodeKind.One)
					{
						if (startingSpan[0] != regexNode2.Ch)
						{
							break;
						}
						if (startingSpan.Length != 1)
						{
							startingSpan = startingSpan.Slice(0, 1);
						}
					}
					else
					{
						int num = Math.Min(startingSpan.Length, regexNode2.Str.Length);
						int k;
						for (k = 0; k < num && startingSpan[k] == regexNode2.Str[k]; k++)
						{
						}
						if (k == 0)
						{
							break;
						}
						startingSpan = startingSpan.Slice(0, k);
					}
				}
				if (j - i > 1)
				{
					RegexNode newChild = ((startingSpan.Length == 1) ? new RegexNode(RegexNodeKind.One, options, startingSpan[0]) : new RegexNode(RegexNodeKind.Multi, options, startingSpan.ToString()));
					RegexNode regexNode3 = new RegexNode(RegexNodeKind.Alternate, options);
					for (int l = i; l < j; l++)
					{
						RegexNode regexNode4 = list[l];
						ProcessOneOrMulti((regexNode4.Kind == RegexNodeKind.Concatenate) ? regexNode4.Child(0) : regexNode4, startingSpan);
						regexNode4 = regexNode4.Reduce();
						regexNode3.AddChild(regexNode4);
					}
					RegexNode parent = alternation.Parent;
					if (parent != null && parent.Kind == RegexNodeKind.Atomic)
					{
						RegexNode regexNode5 = new RegexNode(RegexNodeKind.Atomic, options);
						regexNode5.AddChild(regexNode3);
						regexNode3 = regexNode5;
					}
					RegexNode regexNode6 = new RegexNode(RegexNodeKind.Concatenate, options);
					regexNode6.AddChild(newChild);
					regexNode6.AddChild(regexNode3);
					alternation.ReplaceChild(i, regexNode6);
					list.RemoveRange(i + 1, j - i - 1);
				}
			}
			if (alternation.ChildCount() != 1)
			{
				return alternation;
			}
			return alternation.Child(0);
		}
		static void ProcessOneOrMulti(RegexNode node, ReadOnlySpan<char> startingSpan)
		{
			if (node.Kind == RegexNodeKind.One)
			{
				node.Kind = RegexNodeKind.Empty;
				node.Ch = '\0';
			}
			else if (node.Str.Length == startingSpan.Length)
			{
				node.Kind = RegexNodeKind.Empty;
				node.Str = null;
			}
			else if (node.Str.Length - 1 == startingSpan.Length)
			{
				node.Kind = RegexNodeKind.One;
				node.Ch = node.Str[node.Str.Length - 1];
				node.Str = null;
			}
			else
			{
				node.Str = node.Str.Substring(startingSpan.Length);
			}
		}
		void ReduceSingleLetterAndNestedAlternations()
		{
			bool flag = false;
			bool flag2 = false;
			RegexOptions regexOptions = RegexOptions.None;
			List<RegexNode> list = (List<RegexNode>)Children;
			int i = 0;
			int j;
			for (j = 0; i < list.Count; i++, j++)
			{
				RegexNode regexNode2 = list[i];
				if (j < i)
				{
					list[j] = regexNode2;
				}
				if (regexNode2.Kind == RegexNodeKind.Alternate)
				{
					if (regexNode2.Children is List<RegexNode> list2)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							list2[k].Parent = this;
						}
						list.InsertRange(i + 1, list2);
					}
					else
					{
						RegexNode regexNode3 = (RegexNode)regexNode2.Children;
						regexNode3.Parent = this;
						list.Insert(i + 1, regexNode3);
					}
					j--;
				}
				else
				{
					RegexNodeKind kind = regexNode2.Kind;
					if ((kind == RegexNodeKind.One || kind == RegexNodeKind.Set) ? true : false)
					{
						RegexOptions regexOptions2 = regexNode2.Options & (RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
						if (regexNode2.Kind == RegexNodeKind.Set)
						{
							if (((!flag || regexOptions != regexOptions2) | flag2) || !RegexCharClass.IsMergeable(regexNode2.Str))
							{
								flag = true;
								flag2 = !RegexCharClass.IsMergeable(regexNode2.Str);
								regexOptions = regexOptions2;
								continue;
							}
						}
						else if ((!flag || regexOptions != regexOptions2) | flag2)
						{
							flag = true;
							flag2 = false;
							regexOptions = regexOptions2;
							continue;
						}
						j--;
						RegexNode regexNode4 = list[j];
						RegexCharClass regexCharClass;
						if (regexNode4.Kind == RegexNodeKind.One)
						{
							regexCharClass = new RegexCharClass();
							regexCharClass.AddChar(regexNode4.Ch);
						}
						else
						{
							regexCharClass = RegexCharClass.Parse(regexNode4.Str);
						}
						if (regexNode2.Kind == RegexNodeKind.One)
						{
							regexCharClass.AddChar(regexNode2.Ch);
						}
						else
						{
							RegexCharClass cc = RegexCharClass.Parse(regexNode2.Str);
							regexCharClass.AddCharClass(cc);
						}
						regexNode4.Kind = RegexNodeKind.Set;
						regexNode4.Str = regexCharClass.ToStringClass();
						if ((regexNode4.Options & RegexOptions.IgnoreCase) != RegexOptions.None)
						{
							regexNode4.Options &= ~RegexOptions.IgnoreCase;
						}
					}
					else if (regexNode2.Kind == RegexNodeKind.Nothing)
					{
						j--;
					}
					else
					{
						flag = false;
						flag2 = false;
					}
				}
			}
			if (j < i)
			{
				list.RemoveRange(j, i - j);
			}
		}
		static RegexNode RemoveRedundantEmptiesAndNothings(RegexNode node)
		{
			List<RegexNode> list = (List<RegexNode>)node.Children;
			int num = 0;
			int num2 = 0;
			bool flag = false;
			while (num < list.Count)
			{
				RegexNodeKind kind = list[num].Kind;
				if (kind != RegexNodeKind.Nothing)
				{
					if (kind == RegexNodeKind.Empty)
					{
						if (flag)
						{
							goto IL_0035;
						}
						flag = true;
					}
					list[num2] = list[num];
					num++;
					num2++;
					continue;
				}
				goto IL_0035;
				IL_0035:
				num++;
			}
			list.RemoveRange(num2, list.Count - num2);
			return node.ReplaceNodeIfUnnecessary();
		}
	}

	public RegexNode FindBranchOneOrMultiStart()
	{
		RegexNode regexNode = ((Kind == RegexNodeKind.Concatenate) ? Child(0) : this);
		RegexNodeKind kind = regexNode.Kind;
		if ((kind != RegexNodeKind.One && kind != RegexNodeKind.Multi) || 1 == 0)
		{
			return null;
		}
		return regexNode;
	}

	public char FirstCharOfOneOrMulti()
	{
		if (!IsOneFamily)
		{
			return Str[0];
		}
		return Ch;
	}

	public RegexNode FindStartingLiteralNode(bool allowZeroWidth = true)
	{
		for (RegexNode regexNode = this; regexNode != null && (regexNode.Options & RegexOptions.RightToLeft) == 0; regexNode = regexNode.Child(0))
		{
			switch (regexNode.Kind)
			{
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Oneloopatomic:
				if (regexNode.M <= 0)
				{
					break;
				}
				goto case RegexNodeKind.One;
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Notoneloopatomic:
				if (regexNode.M <= 0)
				{
					break;
				}
				goto case RegexNodeKind.One;
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Setlazy:
			case RegexNodeKind.Setloopatomic:
				if (regexNode.M <= 0)
				{
					break;
				}
				goto case RegexNodeKind.One;
			case RegexNodeKind.One:
			case RegexNodeKind.Notone:
			case RegexNodeKind.Set:
			case RegexNodeKind.Multi:
				return regexNode;
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (regexNode.M > 0)
				{
					continue;
				}
				break;
			case RegexNodeKind.PositiveLookaround:
				if (allowZeroWidth)
				{
					continue;
				}
				break;
			case RegexNodeKind.Concatenate:
			case RegexNodeKind.Capture:
			case RegexNodeKind.Group:
			case RegexNodeKind.Atomic:
				continue;
			}
			break;
		}
		return null;
	}

	public StartingLiteralData? FindStartingLiteral()
	{
		RegexNode regexNode = FindStartingLiteralNode();
		if (regexNode != null)
		{
			switch (regexNode.Kind)
			{
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.One:
			case RegexNodeKind.Oneloopatomic:
				return new StartingLiteralData((LowInclusive: regexNode.Ch, HighInclusive: regexNode.Ch), negated: false);
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Notone:
			case RegexNodeKind.Notoneloopatomic:
				return new StartingLiteralData((LowInclusive: regexNode.Ch, HighInclusive: regexNode.Ch), negated: true);
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Setlazy:
			case RegexNodeKind.Set:
			case RegexNodeKind.Setloopatomic:
			{
				if (RegexCharClass.TryGetSingleRange(regexNode.Str, out var lowInclusive, out var highInclusive) && highInclusive - lowInclusive > 1)
				{
					return new StartingLiteralData((LowInclusive: lowInclusive, HighInclusive: highInclusive), RegexCharClass.IsNegated(regexNode.Str));
				}
				Span<char> chars = stackalloc char[128];
				int setChars;
				if ((setChars = RegexCharClass.GetSetChars(regexNode.Str, chars)) != 0)
				{
					return new StartingLiteralData(chars.Slice(0, setChars).ToString(), RegexCharClass.IsNegated(regexNode.Str));
				}
				break;
			}
			case RegexNodeKind.Multi:
				return new StartingLiteralData(regexNode.Str);
			}
		}
		return null;
	}

	private RegexNode ReduceConcatenation()
	{
		switch (ChildCount())
		{
		case 0:
			return new RegexNode(RegexNodeKind.Empty, Options);
		case 1:
			return Child(0);
		default:
		{
			int num = ChildCount();
			for (int i = 0; i < num; i++)
			{
				RegexNode regexNode = Child(i);
				if (regexNode.Kind == RegexNodeKind.Nothing)
				{
					return regexNode;
				}
			}
			ReduceConcatenationWithAdjacentLoops();
			ReduceConcatenationWithAdjacentStrings();
			return ReplaceNodeIfUnnecessary();
		}
		}
	}

	private void ReduceConcatenationWithAdjacentStrings()
	{
		bool flag = false;
		RegexOptions regexOptions = RegexOptions.None;
		List<RegexNode> list = (List<RegexNode>)Children;
		int num = 0;
		int num2 = 0;
		while (num < list.Count)
		{
			RegexNode regexNode = list[num];
			if (num2 < num)
			{
				list[num2] = regexNode;
			}
			if (regexNode.Kind == RegexNodeKind.Concatenate && (regexNode.Options & RegexOptions.RightToLeft) == (Options & RegexOptions.RightToLeft))
			{
				if (regexNode.Children is List<RegexNode> list2)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						list2[i].Parent = this;
					}
					list.InsertRange(num + 1, list2);
				}
				else
				{
					RegexNode regexNode2 = (RegexNode)regexNode.Children;
					regexNode2.Parent = this;
					list.Insert(num + 1, regexNode2);
				}
				num2--;
			}
			else
			{
				RegexNodeKind kind = regexNode.Kind;
				if ((kind == RegexNodeKind.One || kind == RegexNodeKind.Multi) ? true : false)
				{
					RegexOptions regexOptions2 = regexNode.Options & (RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
					if (!flag || regexOptions != regexOptions2)
					{
						flag = true;
						regexOptions = regexOptions2;
					}
					else
					{
						RegexNode regexNode3 = list[--num2];
						if (regexNode3.Kind == RegexNodeKind.One)
						{
							regexNode3.Kind = RegexNodeKind.Multi;
							regexNode3.Str = regexNode3.Ch.ToString();
						}
						regexNode3.Str = (((regexOptions2 & RegexOptions.RightToLeft) != RegexOptions.None) ? ((regexNode.Kind == RegexNodeKind.One) ? $"{regexNode.Ch}{regexNode3.Str}" : (regexNode.Str + regexNode3.Str)) : ((regexNode.Kind == RegexNodeKind.One) ? $"{regexNode3.Str}{regexNode.Ch}" : (regexNode3.Str + regexNode.Str)));
					}
				}
				else if (regexNode.Kind == RegexNodeKind.Empty)
				{
					num2--;
				}
				else
				{
					flag = false;
				}
			}
			num++;
			num2++;
		}
		if (num2 < num)
		{
			list.RemoveRange(num2, num - num2);
		}
	}

	private void ReduceConcatenationWithAdjacentLoops()
	{
		List<RegexNode> list = (List<RegexNode>)Children;
		int index = 0;
		int num = 1;
		int num2 = 1;
		while (num < list.Count)
		{
			RegexNode regexNode = list[index];
			RegexNode regexNode2 = list[num];
			if (regexNode.Options == regexNode2.Options)
			{
				int num3;
				bool flag;
				switch (regexNode.Kind)
				{
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Onelazy:
					num3 = 0;
					goto IL_0118;
				case RegexNodeKind.Oneloopatomic:
				case RegexNodeKind.Notoneloopatomic:
					num3 = 3;
					goto IL_0118;
				case RegexNodeKind.Notoneloop:
				case RegexNodeKind.Notonelazy:
					num3 = 4;
					goto IL_0118;
				case RegexNodeKind.Setloopatomic:
					num3 = 6;
					goto IL_0153;
				case RegexNodeKind.Setloop:
				case RegexNodeKind.Setlazy:
					num3 = 7;
					goto IL_0153;
				case RegexNodeKind.One:
				{
					RegexNodeKind kind = regexNode2.Kind;
					flag = ((kind == RegexNodeKind.Oneloop || kind == RegexNodeKind.Onelazy || kind == RegexNodeKind.Oneloopatomic) ? true : false);
					if (!flag || regexNode.Ch != regexNode2.Ch)
					{
						break;
					}
					goto IL_04e7;
				}
				case RegexNodeKind.Notone:
				{
					RegexNodeKind kind = regexNode2.Kind;
					flag = ((kind == RegexNodeKind.Notoneloop || kind == RegexNodeKind.Notonelazy || kind == RegexNodeKind.Notoneloopatomic) ? true : false);
					if (flag && regexNode.Ch == regexNode2.Ch)
					{
						goto IL_04e7;
					}
					if (regexNode2.Kind != regexNode.Kind || regexNode.Ch != regexNode2.Ch)
					{
						break;
					}
					goto IL_058f;
				}
				case RegexNodeKind.Set:
				{
					RegexNodeKind kind = regexNode2.Kind;
					flag = ((kind == RegexNodeKind.Setloop || kind == RegexNodeKind.Setlazy || kind == RegexNodeKind.Setloopatomic) ? true : false);
					if (flag && regexNode.Str == regexNode2.Str)
					{
						goto IL_04e7;
					}
					if (regexNode2.Kind != RegexNodeKind.Set || !(regexNode.Str == regexNode2.Str))
					{
						break;
					}
					goto IL_058f;
				}
				case RegexNodeKind.Bol:
				case RegexNodeKind.Eol:
				case RegexNodeKind.Boundary:
				case RegexNodeKind.NonBoundary:
				case RegexNodeKind.Beginning:
				case RegexNodeKind.Start:
				case RegexNodeKind.EndZ:
				case RegexNodeKind.End:
				case RegexNodeKind.ECMABoundary:
				case RegexNodeKind.NonECMABoundary:
					{
						if (regexNode2.Kind == regexNode.Kind)
						{
							num++;
							continue;
						}
						break;
					}
					IL_025c:
					if (regexNode2.Kind != RegexNodeKind.Notone || regexNode.Ch != regexNode2.Ch)
					{
						break;
					}
					goto IL_02a5;
					IL_058f:
					regexNode.MakeRep(RegexNodeKind.Oneloop, 2, 2);
					num++;
					continue;
					IL_0239:
					if (regexNode2.Kind != RegexNodeKind.One || regexNode.Ch != regexNode2.Ch)
					{
						if (regexNode2.Kind != RegexNodeKind.Multi || (regexNode2.Options & RegexOptions.RightToLeft) != RegexOptions.None || regexNode.Ch != regexNode2.Str[0])
						{
							break;
						}
						int i;
						for (i = 1; i < regexNode2.Str.Length && regexNode.Ch == regexNode2.Str[i]; i++)
						{
						}
						if (CanCombineCounts(regexNode.M, regexNode.N, i, i))
						{
							regexNode.M += i;
							if (regexNode.N != int.MaxValue)
							{
								regexNode.N += i;
							}
							if (regexNode2.Str.Length == i)
							{
								num++;
								continue;
							}
							if (regexNode2.Str.Length - i == 1)
							{
								regexNode2.Kind = RegexNodeKind.One;
								regexNode2.Ch = regexNode2.Str[regexNode2.Str.Length - 1];
								regexNode2.Str = null;
							}
							else
							{
								regexNode2.Str = regexNode2.Str.Substring(i);
							}
						}
						break;
					}
					goto IL_02a5;
					IL_0118:
					if (regexNode2.Kind != regexNode.Kind || regexNode.Ch != regexNode2.Ch)
					{
						switch (num3)
						{
						case 0:
							goto IL_0239;
						case 4:
							goto IL_025c;
						case 3:
							goto end_IL_0049;
						}
						goto IL_0153;
					}
					goto IL_0188;
					IL_04e7:
					if (CanCombineCounts(1, 1, regexNode2.M, regexNode2.N))
					{
						regexNode.Kind = regexNode2.Kind;
						regexNode.M = regexNode2.M + 1;
						regexNode.N = ((regexNode2.N == int.MaxValue) ? int.MaxValue : (regexNode2.N + 1));
						num++;
						continue;
					}
					break;
					IL_02a5:
					if (CanCombineCounts(regexNode.M, regexNode.N, 1, 1))
					{
						regexNode.M++;
						if (regexNode.N != int.MaxValue)
						{
							regexNode.N++;
						}
						num++;
						continue;
					}
					break;
					IL_0188:
					flag = regexNode2.M > 0;
					if (flag)
					{
						RegexNodeKind kind = regexNode.Kind;
						bool flag2 = kind - 43 <= (RegexNodeKind)2;
						flag = flag2;
					}
					if (!flag && CanCombineCounts(regexNode.M, regexNode.N, regexNode2.M, regexNode2.N))
					{
						regexNode.M += regexNode2.M;
						if (regexNode.N != int.MaxValue)
						{
							regexNode.N = ((regexNode2.N == int.MaxValue) ? int.MaxValue : (regexNode.N + regexNode2.N));
						}
						num++;
						continue;
					}
					break;
					IL_0153:
					if (regexNode2.Kind != regexNode.Kind || !(regexNode.Str == regexNode2.Str))
					{
						if (num3 == 6)
						{
							break;
						}
						if (num3 == 7)
						{
							if (regexNode2.Kind != RegexNodeKind.Set || !(regexNode.Str == regexNode2.Str))
							{
								break;
							}
							goto IL_02a5;
						}
					}
					goto IL_0188;
					end_IL_0049:
					break;
				}
			}
			list[num2++] = list[num];
			index = num;
			num++;
		}
		if (num2 < list.Count)
		{
			list.RemoveRange(num2, list.Count - num2);
		}
		static bool CanCombineCounts(int nodeMin, int nodeMax, int nextMin, int nextMax)
		{
			if (nodeMin == int.MaxValue || nextMin == int.MaxValue || (uint)(nodeMin + nextMin) >= 2147483647u)
			{
				return false;
			}
			if (nodeMax != int.MaxValue && nextMax != int.MaxValue && (uint)(nodeMax + nextMax) >= 2147483647u)
			{
				return false;
			}
			return true;
		}
	}

	private void FindAndMakeLoopsAtomic()
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return;
		}
		int num = ChildCount();
		if (num != 0)
		{
			for (int i = 0; i < num; i++)
			{
				Child(i).FindAndMakeLoopsAtomic();
			}
		}
		if (Kind == RegexNodeKind.Concatenate)
		{
			List<RegexNode> list = (List<RegexNode>)Children;
			for (int j = 0; j < num - 1; j++)
			{
				ProcessNode(list[j], list[j + 1]);
			}
		}
		static void ProcessNode(RegexNode node, RegexNode subsequent)
		{
			if (StackHelper.TryEnsureSufficientExecutionStack() && (node.Options & RegexOptions.RightToLeft) == 0)
			{
				while (true)
				{
					RegexNodeKind kind = node.Kind;
					if ((kind != RegexNodeKind.Concatenate && kind != RegexNodeKind.Capture) || 1 == 0)
					{
						break;
					}
					node = node.Child(node.ChildCount() - 1);
				}
				RegexNode regexNode;
				bool flag;
				RegexNodeKind kind2;
				RegexNode regexNode3;
				switch (node.Kind)
				{
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Notoneloop:
				case RegexNodeKind.Setloop:
					if (CanBeMadeAtomic(node, subsequent, iterateNullableSubsequent: true, allowLazy: false))
					{
						node.MakeLoopAtomic();
					}
					break;
				case RegexNodeKind.Onelazy:
				case RegexNodeKind.Notonelazy:
				case RegexNodeKind.Setlazy:
					if (CanBeMadeAtomic(node, subsequent, iterateNullableSubsequent: false, allowLazy: true))
					{
						node.Kind -= 3;
						node.MakeLoopAtomic();
					}
					break;
				case RegexNodeKind.Loop:
					if (CanBeMadeAtomic(node, subsequent, iterateNullableSubsequent: true, allowLazy: false))
					{
						goto IL_00d2;
					}
					goto IL_0156;
				case RegexNodeKind.Lazyloop:
					if (!CanBeMadeAtomic(node, subsequent, iterateNullableSubsequent: false, allowLazy: true))
					{
						RegexNode regexNode2 = node.Child(0);
						if (node.N == 1 || CanBeMadeAtomic(regexNode2, regexNode2, iterateNullableSubsequent: false, allowLazy: true))
						{
							ProcessNode(regexNode2, subsequent);
						}
						break;
					}
					goto IL_00d2;
				case RegexNodeKind.Alternate:
				case RegexNodeKind.BackreferenceConditional:
				case RegexNodeKind.ExpressionConditional:
					{
						int num2 = node.ChildCount();
						for (int k = ((node.Kind == RegexNodeKind.ExpressionConditional) ? 1 : 0); k < num2; k++)
						{
							ProcessNode(node.Child(k), subsequent);
						}
						break;
					}
					IL_00d2:
					regexNode = node.Child(0);
					while (true)
					{
						kind2 = regexNode.Kind;
						if ((kind2 != RegexNodeKind.Concatenate && kind2 != RegexNodeKind.Capture) || 1 == 0)
						{
							break;
						}
						regexNode = regexNode.Child(regexNode.ChildCount() - 1);
					}
					kind2 = regexNode.Kind;
					flag = ((kind2 - 9 <= RegexNodeKind.Oneloop || kind2 == RegexNodeKind.Boundary || kind2 == RegexNodeKind.ECMABoundary) ? true : false);
					if (flag && !MayContainBacktracking(node.Child(0)))
					{
						node.MakeLoopAtomic();
						break;
					}
					kind2 = node.Kind;
					if (kind2 - 26 > (RegexNodeKind)1)
					{
						break;
					}
					goto IL_0156;
					IL_0156:
					regexNode3 = node.Child(0);
					if (node.N == 1 || CanBeMadeAtomic(regexNode3, regexNode3, iterateNullableSubsequent: false, allowLazy: false))
					{
						ProcessNode(regexNode3, subsequent);
					}
					break;
				}
			}
		}
	}

	private RegexNode ReduceLookaround()
	{
		if (Kind == RegexNodeKind.NegativeLookaround)
		{
			bool? flag = ContainsKind(Child(0), new RegexNodeKind[2]
			{
				RegexNodeKind.Backreference,
				RegexNodeKind.BackreferenceConditional
			});
			if (flag.HasValue && flag != true && RemoveCaptures(this, 0))
			{
				ReplaceChild(0, Child(0));
			}
		}
		EliminateEndingBacktracking();
		RegexNode regexNode = Child(0);
		if (Kind == RegexNodeKind.PositiveLookaround)
		{
			if (((Options & RegexOptions.RightToLeft) == 0 && IsZeroWidthAssertion(regexNode.Kind)) || regexNode.Kind == RegexNodeKind.Empty)
			{
				return regexNode;
			}
		}
		else if (Kind == RegexNodeKind.NegativeLookaround && regexNode.Kind == RegexNodeKind.Empty)
		{
			Kind = RegexNodeKind.Nothing;
			Children = null;
		}
		return this;
		static bool RemoveCaptures(RegexNode parent, int nodeIndex)
		{
			RegexNode regexNode2 = parent.Child(nodeIndex);
			if (regexNode2 != null && regexNode2.Kind == RegexNodeKind.Capture && regexNode2.N == -1)
			{
				parent.ReplaceChild(nodeIndex, regexNode2.Child(0));
				RemoveCaptures(parent, nodeIndex);
				return true;
			}
			bool flag2 = false;
			if (StackHelper.TryEnsureSufficientExecutionStack())
			{
				int num = regexNode2.ChildCount();
				for (int i = 0; i < num; i++)
				{
					flag2 |= RemoveCaptures(regexNode2, i);
				}
			}
			return flag2;
		}
	}

	private static bool IsZeroWidthAssertion(RegexNodeKind kind)
	{
		switch (kind)
		{
		case RegexNodeKind.Bol:
		case RegexNodeKind.Eol:
		case RegexNodeKind.Boundary:
		case RegexNodeKind.NonBoundary:
		case RegexNodeKind.Beginning:
		case RegexNodeKind.Start:
		case RegexNodeKind.EndZ:
		case RegexNodeKind.End:
		case RegexNodeKind.PositiveLookaround:
		case RegexNodeKind.NegativeLookaround:
		case RegexNodeKind.ECMABoundary:
		case RegexNodeKind.NonECMABoundary:
		case RegexNodeKind.UpdateBumpalong:
			return true;
		default:
			return false;
		}
	}

	private static bool? ContainsKind(RegexNode node, ReadOnlySpan<RegexNodeKind> kinds)
	{
		ReadOnlySpan<RegexNodeKind> readOnlySpan = kinds;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			RegexNodeKind regexNodeKind = readOnlySpan[i];
			if (node.Kind == regexNodeKind)
			{
				return true;
			}
		}
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return null;
		}
		int num = node.ChildCount();
		for (int j = 0; j < num; j++)
		{
			if (ContainsKind(node.Child(j), kinds) ?? false)
			{
				return true;
			}
		}
		return false;
	}

	private RegexNode ReduceBackreferenceConditional()
	{
		if (ChildCount() == 1)
		{
			AddChild(new RegexNode(RegexNodeKind.Empty, Options));
		}
		return this;
	}

	private RegexNode ReduceExpressionConditional()
	{
		if (ChildCount() == 2)
		{
			AddChild(new RegexNode(RegexNodeKind.Empty, Options));
		}
		RegexNode regexNode = Child(0);
		if (regexNode.Kind == RegexNodeKind.PositiveLookaround && (regexNode.Options & RegexOptions.RightToLeft) == 0)
		{
			ReplaceChild(0, regexNode.Child(0));
		}
		regexNode = Child(0);
		regexNode.EliminateEndingBacktracking();
		return this;
	}

	private static bool CanBeMadeAtomic(RegexNode node, RegexNode subsequent, bool iterateNullableSubsequent, bool allowLazy)
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return false;
		}
		while (true)
		{
			RegexNodeKind kind = node.Kind;
			if ((kind != RegexNodeKind.Concatenate && kind != RegexNodeKind.Capture) || 1 == 0)
			{
				break;
			}
			node = node.Child(node.ChildCount() - 1);
		}
		while (true)
		{
			switch (subsequent.Kind)
			{
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				if (subsequent.M <= 0)
				{
					break;
				}
				goto case RegexNodeKind.Concatenate;
			case RegexNodeKind.PositiveLookaround:
				if ((subsequent.Options & RegexOptions.RightToLeft) != RegexOptions.None)
				{
					break;
				}
				goto case RegexNodeKind.Concatenate;
			case RegexNodeKind.Concatenate:
			case RegexNodeKind.Capture:
			case RegexNodeKind.Atomic:
				subsequent = subsequent.Child(0);
				continue;
			}
			if (node.Options != subsequent.Options)
			{
				return false;
			}
			int num = subsequent.ChildCount();
			RegexNodeKind kind = subsequent.Kind;
			if (kind == RegexNodeKind.Alternate || (kind == RegexNodeKind.ExpressionConditional && num == 3))
			{
				for (int i = 0; i < num; i++)
				{
					if (!CanBeMadeAtomic(node, subsequent.Child(i), iterateNullableSubsequent, allowLazy: false))
					{
						return false;
					}
				}
				return true;
			}
			switch (node.Kind)
			{
			case RegexNodeKind.Onelazy:
				if (allowLazy)
				{
					goto case RegexNodeKind.Oneloop;
				}
				goto default;
			case RegexNodeKind.Oneloop:
				switch (subsequent.Kind)
				{
				case RegexNodeKind.One:
					if (node.Ch != subsequent.Ch)
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Notone:
					if (node.Ch == subsequent.Ch)
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Set:
					if (!RegexCharClass.CharInClass(node.Ch, subsequent.Str))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Onelazy:
				case RegexNodeKind.Oneloopatomic:
					if (subsequent.M > 0 && node.Ch != subsequent.Ch)
					{
						goto case RegexNodeKind.End;
					}
					if (subsequent.M == 0 && node.Ch != subsequent.Ch)
					{
						break;
					}
					goto default;
				case RegexNodeKind.Notoneloop:
				case RegexNodeKind.Notonelazy:
				case RegexNodeKind.Notoneloopatomic:
					if (subsequent.M > 0 && node.Ch == subsequent.Ch)
					{
						goto case RegexNodeKind.End;
					}
					if (subsequent.M == 0 && node.Ch == subsequent.Ch)
					{
						break;
					}
					goto default;
				case RegexNodeKind.Setloop:
				case RegexNodeKind.Setlazy:
				case RegexNodeKind.Setloopatomic:
					if (subsequent.M > 0 && !RegexCharClass.CharInClass(node.Ch, subsequent.Str))
					{
						goto case RegexNodeKind.End;
					}
					if (subsequent.M == 0 && !RegexCharClass.CharInClass(node.Ch, subsequent.Str))
					{
						break;
					}
					goto default;
				case RegexNodeKind.Multi:
					if (node.Ch != subsequent.Str[0])
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Eol:
				case RegexNodeKind.EndZ:
					if (node.Ch != '\n')
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Boundary:
					if (node.M > 0 && RegexCharClass.IsBoundaryWordChar(node.Ch))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.NonBoundary:
					if (node.M > 0 && !RegexCharClass.IsBoundaryWordChar(node.Ch))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.ECMABoundary:
					if (node.M > 0 && RegexCharClass.IsECMAWordChar(node.Ch))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.NonECMABoundary:
					if (node.M > 0 && !RegexCharClass.IsECMAWordChar(node.Ch))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.End:
					return true;
				default:
					return false;
				}
				break;
			case RegexNodeKind.Notonelazy:
				if (allowLazy)
				{
					goto case RegexNodeKind.Notoneloop;
				}
				goto default;
			case RegexNodeKind.Notoneloop:
			{
				RegexNodeKind kind2 = subsequent.Kind;
				if (kind2 <= RegexNodeKind.One)
				{
					if (kind2 == RegexNodeKind.Oneloop || kind2 == RegexNodeKind.Onelazy)
					{
						goto IL_037b;
					}
					if (kind2 == RegexNodeKind.One && node.Ch == subsequent.Ch)
					{
						goto IL_03a8;
					}
				}
				else if (kind2 != RegexNodeKind.Multi)
				{
					if (kind2 == RegexNodeKind.End)
					{
						goto IL_03a8;
					}
					if (kind2 == RegexNodeKind.Oneloopatomic)
					{
						goto IL_037b;
					}
				}
				else if (node.Ch == subsequent.Str[0])
				{
					goto IL_03a8;
				}
				goto IL_03c3;
			}
			case RegexNodeKind.Setlazy:
				if (allowLazy)
				{
					goto case RegexNodeKind.Setloop;
				}
				goto default;
			case RegexNodeKind.Setloop:
				switch (subsequent.Kind)
				{
				case RegexNodeKind.One:
					if (!RegexCharClass.CharInClass(subsequent.Ch, node.Str))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Set:
					if (!RegexCharClass.MayOverlap(node.Str, subsequent.Str))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Onelazy:
				case RegexNodeKind.Oneloopatomic:
					if (subsequent.M > 0 && !RegexCharClass.CharInClass(subsequent.Ch, node.Str))
					{
						goto case RegexNodeKind.End;
					}
					if (subsequent.M == 0 && !RegexCharClass.CharInClass(subsequent.Ch, node.Str))
					{
						break;
					}
					goto default;
				case RegexNodeKind.Setloop:
				case RegexNodeKind.Setlazy:
				case RegexNodeKind.Setloopatomic:
					if (subsequent.M > 0 && !RegexCharClass.MayOverlap(node.Str, subsequent.Str))
					{
						goto case RegexNodeKind.End;
					}
					if (subsequent.M == 0 && !RegexCharClass.MayOverlap(node.Str, subsequent.Str))
					{
						break;
					}
					goto default;
				case RegexNodeKind.Multi:
					if (!RegexCharClass.CharInClass(subsequent.Str[0], node.Str))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Eol:
				case RegexNodeKind.EndZ:
					if (!RegexCharClass.CharInClass('\n', node.Str))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.Boundary:
					if (node.M > 0 && RegexCharClass.IsKnownWordClassSubset(node.Str))
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				case RegexNodeKind.NonBoundary:
				{
					bool flag = node.M > 0;
					if (flag)
					{
						string text = node.Str;
						bool flag3 = ((text == "\0\0\n\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0" || text == "\0\0\u0001\ufff7") ? true : false);
						flag = flag3;
					}
					if (flag)
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				}
				case RegexNodeKind.ECMABoundary:
				{
					bool flag = node.M > 0;
					if (flag)
					{
						string text = node.Str;
						bool flag3 = ((text == "\0\n\00:A[_`a{İı" || text == "\0\u0002\00:") ? true : false);
						flag = flag3;
					}
					if (flag)
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				}
				case RegexNodeKind.NonECMABoundary:
				{
					bool flag = node.M > 0;
					if (flag)
					{
						string text = node.Str;
						bool flag3 = ((text == "\u0001\n\00:A[_`a{İı" || text == "\0\0\u0001\ufff7") ? true : false);
						flag = flag3;
					}
					if (flag)
					{
						goto case RegexNodeKind.End;
					}
					goto default;
				}
				case RegexNodeKind.End:
					return true;
				default:
					return false;
				}
				break;
			case RegexNodeKind.Lazyloop:
				if (allowLazy)
				{
					goto case RegexNodeKind.Loop;
				}
				goto default;
			case RegexNodeKind.Loop:
			{
				RegexNode root = node.Child(0);
				string loopStartingSet = RegexPrefixAnalyzer.FindFirstCharClass(root);
				if (loopStartingSet != null)
				{
					string loopEndingSet = RegexPrefixAnalyzer.FindLastCharClass(root);
					if (loopEndingSet != null && (node.N <= 1 || !RegexCharClass.MayOverlap(loopStartingSet, loopEndingSet)))
					{
						switch (subsequent.Kind)
						{
						case RegexNodeKind.One:
							if (!CharInStartingOrEndingSet(subsequent.Ch))
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						case RegexNodeKind.Set:
							if (!MayOverlapStartingOrEndingSet(subsequent.Str))
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						case RegexNodeKind.Oneloop:
						case RegexNodeKind.Onelazy:
						case RegexNodeKind.Oneloopatomic:
							if (subsequent.M > 0 && !CharInStartingOrEndingSet(subsequent.Ch))
							{
								goto case RegexNodeKind.End;
							}
							if (subsequent.M == 0 && !CharInStartingOrEndingSet(subsequent.Ch))
							{
								break;
							}
							goto default;
						case RegexNodeKind.Setloop:
						case RegexNodeKind.Setlazy:
						case RegexNodeKind.Setloopatomic:
							if (subsequent.M > 0 && !MayOverlapStartingOrEndingSet(subsequent.Str))
							{
								goto case RegexNodeKind.End;
							}
							if (subsequent.M == 0 && !MayOverlapStartingOrEndingSet(subsequent.Str))
							{
								break;
							}
							goto default;
						case RegexNodeKind.Multi:
							if (!CharInStartingOrEndingSet(subsequent.Str[0]))
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						case RegexNodeKind.Eol:
						case RegexNodeKind.EndZ:
							if (!CharInStartingOrEndingSet('\n'))
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						case RegexNodeKind.Boundary:
							if (node.M > 0 && RegexCharClass.IsKnownWordClassSubset(loopStartingSet) && RegexCharClass.IsKnownWordClassSubset(loopEndingSet))
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						case RegexNodeKind.NonBoundary:
						{
							bool flag = node.M > 0;
							if (flag)
							{
								string text = loopStartingSet;
								bool flag2 = ((text == "\0\0\n\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0" || text == "\0\0\u0001\ufff7") ? true : false);
								flag = flag2;
							}
							bool flag3 = flag;
							if (flag3)
							{
								string text = loopEndingSet;
								bool flag2 = ((text == "\0\0\n\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0" || text == "\0\0\u0001\ufff7") ? true : false);
								flag3 = flag2;
							}
							if (flag3)
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						}
						case RegexNodeKind.ECMABoundary:
						{
							bool flag3 = node.M > 0;
							if (flag3)
							{
								string text = loopStartingSet;
								bool flag2 = ((text == "\0\n\00:A[_`a{İı" || text == "\0\u0002\00:") ? true : false);
								flag3 = flag2;
							}
							bool flag = flag3;
							if (flag)
							{
								string text = loopEndingSet;
								bool flag2 = ((text == "\0\n\00:A[_`a{İı" || text == "\0\u0002\00:") ? true : false);
								flag = flag2;
							}
							if (flag)
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						}
						case RegexNodeKind.NonECMABoundary:
						{
							bool flag = node.M > 0;
							if (flag)
							{
								string text = loopStartingSet;
								bool flag2 = ((text == "\u0001\n\00:A[_`a{İı" || text == "\0\0\u0001\ufff7") ? true : false);
								flag = flag2;
							}
							bool flag3 = flag;
							if (flag3)
							{
								string text = loopEndingSet;
								bool flag2 = ((text == "\u0001\n\00:A[_`a{İı" || text == "\0\0\u0001\ufff7") ? true : false);
								flag3 = flag2;
							}
							if (flag3)
							{
								goto case RegexNodeKind.End;
							}
							goto default;
						}
						case RegexNodeKind.End:
							return true;
						default:
							return false;
						}
						break;
					}
				}
				return false;
			}
			default:
				{
					return false;
				}
				IL_03a8:
				return true;
				IL_037b:
				if (subsequent.M > 0 && node.Ch == subsequent.Ch)
				{
					goto IL_03a8;
				}
				if (subsequent.M == 0 && node.Ch == subsequent.Ch)
				{
					break;
				}
				goto IL_03c3;
				IL_03c3:
				return false;
			}
			if (!iterateNullableSubsequent)
			{
				break;
			}
			List<RegexNode> list;
			int num2;
			while (true)
			{
				RegexNode parent = subsequent.Parent;
				switch (parent?.Kind)
				{
				case RegexNodeKind.Alternate:
				case RegexNodeKind.Capture:
				case RegexNodeKind.Atomic:
					subsequent = parent;
					continue;
				case RegexNodeKind.Concatenate:
					break;
				case null:
					return true;
				default:
					return false;
				}
				list = (List<RegexNode>)parent.Children;
				num2 = list.IndexOf(subsequent);
				if (num2 + 1 != list.Count)
				{
					break;
				}
				subsequent = parent;
			}
			subsequent = list[num2 + 1];
		}
		return false;
	}

	private static bool MayContainBacktracking(RegexNode node)
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return true;
		}
		if (node.IsBacktrackingConstruct)
		{
			return true;
		}
		int num = node.ChildCount();
		for (int i = 0; i < num; i++)
		{
			if (MayContainBacktracking(node.Child(i)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsKnownPrecededByWordChar()
	{
		return IsKnownPrecededOrSucceededByWordChar(succeeded: false);
	}

	public bool IsKnownSucceededByWordChar()
	{
		return IsKnownPrecededOrSucceededByWordChar(succeeded: true);
	}

	private bool IsKnownPrecededOrSucceededByWordChar(bool succeeded)
	{
		RegexNode regexNode = this;
		while ((regexNode.Options & RegexOptions.RightToLeft) == 0)
		{
			RegexNode parent = regexNode.Parent;
			if (parent == null)
			{
				break;
			}
			switch (parent.Kind)
			{
			case RegexNodeKind.Alternate:
			case RegexNodeKind.Capture:
			case RegexNodeKind.Atomic:
				regexNode = parent;
				break;
			case RegexNodeKind.Concatenate:
			{
				List<RegexNode> list = (List<RegexNode>)parent.Children;
				int num = list.IndexOf(regexNode) + (succeeded ? 1 : (-1));
				if ((uint)num < (uint)list.Count)
				{
					string text = RegexPrefixAnalyzer.FindFirstOrLastCharClass(list[num], succeeded);
					if (text != null)
					{
						return RegexCharClass.IsKnownWordClassSubset(text);
					}
					return false;
				}
				regexNode = parent;
				break;
			}
			default:
				return false;
			}
		}
		return false;
	}

	public int ComputeMinLength()
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return 0;
		}
		switch (Kind)
		{
		case RegexNodeKind.One:
		case RegexNodeKind.Notone:
		case RegexNodeKind.Set:
			return 1;
		case RegexNodeKind.Multi:
			return Str.Length;
		case RegexNodeKind.Oneloop:
		case RegexNodeKind.Notoneloop:
		case RegexNodeKind.Setloop:
		case RegexNodeKind.Onelazy:
		case RegexNodeKind.Notonelazy:
		case RegexNodeKind.Setlazy:
		case RegexNodeKind.Oneloopatomic:
		case RegexNodeKind.Notoneloopatomic:
		case RegexNodeKind.Setloopatomic:
			return M;
		case RegexNodeKind.Loop:
		case RegexNodeKind.Lazyloop:
			return (int)Math.Min(2147483646L, (long)M * (long)Child(0).ComputeMinLength());
		case RegexNodeKind.Alternate:
		{
			int num3 = ChildCount();
			int num4 = Child(0).ComputeMinLength();
			for (int j = 1; j < num3; j++)
			{
				if (num4 <= 0)
				{
					break;
				}
				num4 = Math.Min(num4, Child(j).ComputeMinLength());
			}
			return num4;
		}
		case RegexNodeKind.BackreferenceConditional:
			return Math.Min(Child(0).ComputeMinLength(), Child(1).ComputeMinLength());
		case RegexNodeKind.ExpressionConditional:
			return Math.Min(Child(1).ComputeMinLength(), Child(2).ComputeMinLength());
		case RegexNodeKind.Concatenate:
		{
			long num = 0L;
			int num2 = ChildCount();
			for (int i = 0; i < num2; i++)
			{
				num += Child(i).ComputeMinLength();
			}
			return (int)Math.Min(2147483646L, num);
		}
		case RegexNodeKind.Capture:
		case RegexNodeKind.Group:
		case RegexNodeKind.Atomic:
			return Child(0).ComputeMinLength();
		default:
			return 0;
		}
	}

	public int? ComputeMaxLength()
	{
		if (!StackHelper.TryEnsureSufficientExecutionStack())
		{
			return null;
		}
		switch (Kind)
		{
		case RegexNodeKind.One:
		case RegexNodeKind.Notone:
		case RegexNodeKind.Set:
			return 1;
		case RegexNodeKind.Multi:
			return Str.Length;
		case RegexNodeKind.Oneloop:
		case RegexNodeKind.Notoneloop:
		case RegexNodeKind.Setloop:
		case RegexNodeKind.Onelazy:
		case RegexNodeKind.Notonelazy:
		case RegexNodeKind.Setlazy:
		case RegexNodeKind.Oneloopatomic:
		case RegexNodeKind.Notoneloopatomic:
		case RegexNodeKind.Setloopatomic:
			if (N != int.MaxValue)
			{
				return N;
			}
			return null;
		case RegexNodeKind.Loop:
		case RegexNodeKind.Lazyloop:
			if (N != int.MaxValue)
			{
				int? num3 = Child(0).ComputeMaxLength();
				if (num3.HasValue)
				{
					int valueOrDefault3 = num3.GetValueOrDefault();
					long num6 = (long)N * (long)valueOrDefault3;
					if (num6 < int.MaxValue)
					{
						return (int)num6;
					}
				}
			}
			return null;
		case RegexNodeKind.Alternate:
		{
			int num4 = ChildCount();
			int? num3 = Child(0).ComputeMaxLength();
			if (num3.HasValue)
			{
				int num5 = num3.GetValueOrDefault();
				for (int j = 1; j < num4; j++)
				{
					num3 = Child(j).ComputeMaxLength();
					if (num3.HasValue)
					{
						int valueOrDefault2 = num3.GetValueOrDefault();
						num5 = Math.Max(num5, valueOrDefault2);
						continue;
					}
					return null;
				}
				return num5;
			}
			return null;
		}
		case RegexNodeKind.BackreferenceConditional:
		case RegexNodeKind.ExpressionConditional:
		{
			int num7 = ((Kind != RegexNodeKind.BackreferenceConditional) ? 1 : 0);
			int? num3 = Child(num7).ComputeMaxLength();
			if (num3.HasValue)
			{
				int valueOrDefault4 = num3.GetValueOrDefault();
				num3 = Child(num7 + 1).ComputeMaxLength();
				if (num3.HasValue)
				{
					int valueOrDefault5 = num3.GetValueOrDefault();
					return Math.Max(valueOrDefault4, valueOrDefault5);
				}
			}
			return null;
		}
		case RegexNodeKind.Concatenate:
		{
			long num = 0L;
			int num2 = ChildCount();
			for (int i = 0; i < num2; i++)
			{
				int? num3 = Child(i).ComputeMaxLength();
				if (num3.HasValue)
				{
					int valueOrDefault = num3.GetValueOrDefault();
					num += valueOrDefault;
					continue;
				}
				return null;
			}
			if (num < int.MaxValue)
			{
				return (int)num;
			}
			return null;
		}
		case RegexNodeKind.Capture:
		case RegexNodeKind.Atomic:
			return Child(0).ComputeMaxLength();
		default:
			return 0;
		case RegexNodeKind.Backreference:
			return null;
		}
	}

	public bool TryGetOrdinalCaseInsensitiveString(int childIndex, int exclusiveChildBound, out int nodesConsumed, [NotNullWhen(true)] out string caseInsensitiveString, bool consumeZeroWidthNodes = false)
	{
		Span<char> initialBuffer = stackalloc char[32];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		Span<char> twoChars = stackalloc char[2];
		int i;
		for (i = childIndex; i < exclusiveChildBound; i++)
		{
			RegexNode regexNode = Child(i);
			if (regexNode.Kind == RegexNodeKind.One)
			{
				if (regexNode.Ch >= '\u0080' || RegexCharClass.ParticipatesInCaseConversion(regexNode.Ch))
				{
					break;
				}
				valueStringBuilder.Append(regexNode.Ch);
				continue;
			}
			if (regexNode.Kind == RegexNodeKind.Multi)
			{
				if (!RegexCharClass.IsAscii(regexNode.Str.AsSpan()) || RegexCharClass.ParticipatesInCaseConversion(regexNode.Str.AsSpan()))
				{
					break;
				}
				valueStringBuilder.Append(regexNode.Str);
				continue;
			}
			bool flag = regexNode.Kind == RegexNodeKind.Set;
			if (!flag)
			{
				RegexNodeKind kind = regexNode.Kind;
				bool flag2 = ((kind == RegexNodeKind.Setloop || kind == RegexNodeKind.Setlazy || kind == RegexNodeKind.Setloopatomic) ? true : false);
				flag = flag2 && regexNode.M == regexNode.N;
			}
			if (flag)
			{
				if (!RegexCharClass.SetContainsAsciiOrdinalIgnoreCaseCharacter(regexNode.Str, twoChars))
				{
					break;
				}
				valueStringBuilder.Append((char)(twoChars[0] | 0x20), (regexNode.Kind == RegexNodeKind.Set) ? 1 : regexNode.M);
			}
			else if (regexNode.Kind != RegexNodeKind.Empty && (!consumeZeroWidthNodes || !IsZeroWidthAssertion(regexNode.Kind)))
			{
				break;
			}
		}
		if (valueStringBuilder.Length >= 2)
		{
			caseInsensitiveString = valueStringBuilder.ToString();
			nodesConsumed = i - childIndex;
			return true;
		}
		caseInsensitiveString = null;
		nodesConsumed = 0;
		valueStringBuilder.Dispose();
		return false;
	}

	public bool TryGetJoinableLengthCheckChildRange(int childIndex, out int requiredLength, out int exclusiveEnd)
	{
		RegexNode regexNode = Child(childIndex);
		if (CanJoinLengthCheck(regexNode))
		{
			requiredLength = regexNode.ComputeMinLength();
			int num = ChildCount();
			exclusiveEnd = childIndex + 1;
			while (exclusiveEnd < num)
			{
				regexNode = Child(exclusiveEnd);
				if (!CanJoinLengthCheck(regexNode))
				{
					break;
				}
				requiredLength += regexNode.ComputeMinLength();
				exclusiveEnd++;
			}
			if (exclusiveEnd - childIndex > 1)
			{
				return true;
			}
		}
		requiredLength = 0;
		exclusiveEnd = 0;
		return false;
		static bool CanJoinLengthCheck(RegexNode node)
		{
			switch (node.Kind)
			{
			case RegexNodeKind.One:
			case RegexNodeKind.Notone:
			case RegexNodeKind.Set:
				return true;
			case RegexNodeKind.Multi:
				return true;
			case RegexNodeKind.Oneloop:
			case RegexNodeKind.Notoneloop:
			case RegexNodeKind.Setloop:
			case RegexNodeKind.Onelazy:
			case RegexNodeKind.Notonelazy:
			case RegexNodeKind.Setlazy:
			case RegexNodeKind.Oneloopatomic:
			case RegexNodeKind.Notoneloopatomic:
			case RegexNodeKind.Setloopatomic:
				if (node.M == node.N)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}

	public RegexNode MakeQuantifier(bool lazy, int min, int max)
	{
		if (min == max)
		{
			if (max <= 64)
			{
				switch (max)
				{
				case 0:
					return new RegexNode(RegexNodeKind.Empty, Options);
				case 1:
					return this;
				}
				if (Kind == RegexNodeKind.One)
				{
					Kind = RegexNodeKind.Multi;
					Str = new string(Ch, max);
					Ch = '\0';
					return this;
				}
			}
		}
		RegexNodeKind kind = Kind;
		if (kind - 9 <= (RegexNodeKind)2)
		{
			MakeRep(lazy ? RegexNodeKind.Onelazy : RegexNodeKind.Oneloop, min, max);
			return this;
		}
		RegexNode regexNode = new RegexNode(lazy ? RegexNodeKind.Lazyloop : RegexNodeKind.Loop, Options, min, max);
		regexNode.AddChild(this);
		return regexNode;
	}

	public void AddChild(RegexNode newChild)
	{
		newChild.Parent = this;
		newChild = newChild.Reduce();
		newChild.Parent = this;
		if (Children == null)
		{
			Children = newChild;
		}
		else if (Children is RegexNode item)
		{
			Children = new List<RegexNode> { item, newChild };
		}
		else
		{
			((List<RegexNode>)Children).Add(newChild);
		}
	}

	public void InsertChild(int index, RegexNode newChild)
	{
		newChild.Parent = this;
		newChild = newChild.Reduce();
		newChild.Parent = this;
		((List<RegexNode>)Children).Insert(index, newChild);
	}

	public void ReplaceChild(int index, RegexNode newChild)
	{
		newChild.Parent = this;
		newChild = newChild.Reduce();
		newChild.Parent = this;
		if (Children is RegexNode)
		{
			Children = newChild;
		}
		else
		{
			((List<RegexNode>)Children)[index] = newChild;
		}
	}

	public RegexNode Child(int i)
	{
		if (!(Children is RegexNode result))
		{
			return ((List<RegexNode>)Children)[i];
		}
		return result;
	}

	public int ChildCount()
	{
		if (Children == null)
		{
			return 0;
		}
		if (Children is List<RegexNode> list)
		{
			return list.Count;
		}
		return 1;
	}

	internal bool SupportsCompilation([NotNullWhen(false)] out string reason)
	{
		if ((Options & RegexOptions.NonBacktracking) != RegexOptions.None)
		{
			reason = "RegexOptions.NonBacktracking isn't supported";
			return false;
		}
		if (ExceedsMaxDepthAllowedDepth(this, 40))
		{
			reason = "the expression may result exceeding run-time or compiler limits";
			return false;
		}
		reason = null;
		return true;
		static bool ExceedsMaxDepthAllowedDepth(RegexNode node, int allowedDepth)
		{
			if (allowedDepth <= 0)
			{
				return true;
			}
			int num = node.ChildCount();
			for (int i = 0; i < num; i++)
			{
				if (ExceedsMaxDepthAllowedDepth(node.Child(i), allowedDepth - 1))
				{
					return true;
				}
			}
			return false;
		}
	}
}
