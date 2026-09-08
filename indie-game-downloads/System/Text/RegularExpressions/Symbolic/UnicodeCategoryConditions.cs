using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Text.RegularExpressions.Symbolic;

internal static class UnicodeCategoryConditions
{
	private static readonly BDD[] s_categories = new BDD[30];

	private static volatile BDD s_wordLetter;

	private static volatile BDD s_wordLetterForAnchors;

	[CompilerGenerated]
	private static BDD _003CWhiteSpace_003Ek__BackingField;

	public static BDD WhiteSpace => _003CWhiteSpace_003Ek__BackingField ?? Interlocked.CompareExchange(ref _003CWhiteSpace_003Ek__BackingField, BDD.Deserialize(UnicodeCategoryRanges.SerializedWhitespaceBDD), null) ?? _003CWhiteSpace_003Ek__BackingField;

	public static BDD GetCategory(UnicodeCategory category)
	{
		return s_categories[(int)category] ?? Interlocked.CompareExchange(ref s_categories[(int)category], BDD.Deserialize(UnicodeCategoryRanges.GetSerializedCategory(category)), null) ?? s_categories[(int)category];
	}

	public static BDD WordLetter(CharSetSolver solver)
	{
		object obj = s_wordLetter;
		if (obj == null)
		{
			global::_003C_003Ey__InlineArray8<BDD> buffer = default(global::_003C_003Ey__InlineArray8<BDD>);
			buffer[0] = GetCategory(UnicodeCategory.UppercaseLetter);
			buffer[1] = GetCategory(UnicodeCategory.LowercaseLetter);
			buffer[2] = GetCategory(UnicodeCategory.TitlecaseLetter);
			buffer[3] = GetCategory(UnicodeCategory.ModifierLetter);
			buffer[4] = GetCategory(UnicodeCategory.OtherLetter);
			buffer[5] = GetCategory(UnicodeCategory.NonSpacingMark);
			buffer[6] = GetCategory(UnicodeCategory.DecimalDigitNumber);
			buffer[7] = GetCategory(UnicodeCategory.ConnectorPunctuation);
			obj = Interlocked.CompareExchange(ref s_wordLetter, solver.Or(buffer), null) ?? s_wordLetter;
		}
		return (BDD)obj;
	}

	public static BDD WordLetterForAnchors(CharSetSolver solver)
	{
		return s_wordLetterForAnchors ?? Interlocked.CompareExchange(ref s_wordLetterForAnchors, solver.Or(WordLetter(solver), solver.CreateBDDFromRange('\u200c', '\u200d')), null) ?? s_wordLetterForAnchors;
	}
}
