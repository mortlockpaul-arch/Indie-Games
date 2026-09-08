using System.Collections.Generic;
using System.Threading;

namespace System.Text.RegularExpressions;

internal static class RegexTreeAnalyzer
{
	public static AnalysisResults Analyze(RegexTree regexTree)
	{
		AnalysisResults analysisResults = new AnalysisResults(regexTree);
		analysisResults._complete = TryAnalyze(regexTree.Root, analysisResults, isAtomicByAncestor: true, isInLoop: false);
		return analysisResults;
		static bool TryAnalyze(RegexNode node, AnalysisResults results, bool isAtomicByAncestor, bool isInLoop)
		{
			if (!StackHelper.TryEnsureSufficientExecutionStack())
			{
				return false;
			}
			results._hasIgnoreCase |= (node.Options & RegexOptions.IgnoreCase) != 0;
			results._hasRightToLeft |= (node.Options & RegexOptions.RightToLeft) != 0;
			if (isInLoop)
			{
				AnalysisResults analysisResults2 = results;
				(analysisResults2._inLoops ?? (analysisResults2._inLoops = new HashSet<RegexNode>())).Add(node);
			}
			if (isAtomicByAncestor)
			{
				results._isAtomicByAncestor.Add(node);
			}
			else if (node.IsBacktrackingConstruct)
			{
				AnalysisResults analysisResults2 = results;
				(analysisResults2._mayBacktrack ?? (analysisResults2._mayBacktrack = new HashSet<RegexNode>())).Add(node);
			}
			bool flag = false;
			switch (node.Kind)
			{
			case RegexNodeKind.PositiveLookaround:
			case RegexNodeKind.NegativeLookaround:
			case RegexNodeKind.Atomic:
				flag = true;
				break;
			case RegexNodeKind.Capture:
				results._containsCapture.Add(node);
				break;
			case RegexNodeKind.Loop:
			case RegexNodeKind.Lazyloop:
				isInLoop = true;
				break;
			}
			int num = node.ChildCount();
			for (int i = 0; i < num; i++)
			{
				RegexNode regexNode = node.Child(i);
				bool flag2 = isAtomicByAncestor | flag;
				if (flag2)
				{
					bool flag3;
					switch (node.Kind)
					{
					case RegexNodeKind.PositiveLookaround:
					case RegexNodeKind.NegativeLookaround:
					case RegexNodeKind.Atomic:
						flag3 = true;
						break;
					case RegexNodeKind.Alternate:
					case RegexNodeKind.BackreferenceConditional:
					case RegexNodeKind.ExpressionConditional:
						flag3 = true;
						break;
					case RegexNodeKind.Capture:
						flag3 = true;
						break;
					case RegexNodeKind.Concatenate:
						flag3 = i == num - 1;
						break;
					case RegexNodeKind.Loop:
					case RegexNodeKind.Lazyloop:
						if (node.N == 1)
						{
							flag3 = true;
							break;
						}
						goto default;
					default:
						flag3 = false;
						break;
					}
					flag2 = flag3;
				}
				bool isAtomicByAncestor2 = flag2;
				if (!TryAnalyze(regexNode, results, isAtomicByAncestor2, isInLoop))
				{
					return false;
				}
				if (results._containsCapture.Contains(regexNode))
				{
					results._containsCapture.Add(node);
				}
				if (!flag)
				{
					HashSet<RegexNode> mayBacktrack = results._mayBacktrack;
					if (mayBacktrack != null && mayBacktrack.Contains(regexNode))
					{
						AnalysisResults analysisResults2 = results;
						(analysisResults2._mayBacktrack ?? (analysisResults2._mayBacktrack = new HashSet<RegexNode>())).Add(node);
					}
				}
			}
			return true;
		}
	}
}
