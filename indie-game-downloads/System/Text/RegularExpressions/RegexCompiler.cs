using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Text.RegularExpressions;

[RequiresDynamicCode("Compiling a RegEx requires dynamic code.")]
internal abstract class RegexCompiler
{
	private struct RentedLocalBuilder : IDisposable
	{
		private readonly Stack<LocalBuilder> _pool;

		private readonly LocalBuilder _local;

		internal RentedLocalBuilder(Stack<LocalBuilder> pool, LocalBuilder local)
		{
			_local = local;
			_pool = pool;
		}

		public static implicit operator LocalBuilder(RentedLocalBuilder local)
		{
			return local._local;
		}

		public void Dispose()
		{
			_pool.Push(_local);
			this = default(RentedLocalBuilder);
		}
	}

	[CompilerGenerated]
	private static FieldInfo _003CRuntextstartField_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CRuntextposField_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CRuntrackposField_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CRunstackField_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CCultureField_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CCaseBehaviorField_003Ek__BackingField;

	[CompilerGenerated]
	private static FieldInfo _003CSearchValuesArrayField_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCaptureMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CTransferCaptureMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CUncaptureMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CIsMatchedMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMatchLengthMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMatchIndexMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CIsBoundaryMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CIsPreWordCharBoundaryMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CIsPostWordCharBoundaryMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CIsWordCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CIsECMABoundaryMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCrawlposMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharInClassMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCheckTimeoutMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CRegexCaseEquivalencesTryFindCaseEquivalencesForCharWithIBehaviorMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsDigitMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsWhiteSpaceMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsControlMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsLetterMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiDigitMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiLetterMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiLetterLowerMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiLetterUpperMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiLetterOrDigitMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiHexDigitMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiHexDigitLowerMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsAsciiHexDigitUpperMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsLetterOrDigitMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsLowerMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsUpperMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsNumberMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsPunctuationMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsSeparatorMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharIsSymbolMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CCharGetUnicodeInfoMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanGetItemMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanGetLengthMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfSpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfSpanStringComparisonMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyCharCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnySpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnySearchValuesMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnySearchValuesStringMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyExceptCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyExceptCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyExceptCharCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyExceptSpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyExceptSearchValuesMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyInRangeMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanIndexOfAnyExceptInRangeMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyCharCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnySpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnySearchValuesMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfSpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyExceptCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyExceptCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyExceptCharCharCharMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyExceptSpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyExceptSearchValuesMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyInRangeMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanLastIndexOfAnyExceptInRangeMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanSliceIntMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanSliceIntIntMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanStartsWithSpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CSpanStartsWithSpanComparisonMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CStringAsSpanMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CStringGetCharsMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CArrayResizeMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMathMinIntIntMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CMemoryMarshalGetArrayDataReferenceSearchValuesMethod_003Ek__BackingField;

	[CompilerGenerated]
	private static MethodInfo _003CUnsafeAsMethod_003Ek__BackingField;

	protected ILGenerator _ilg;

	protected RegexOptions _options;

	protected RegexTree _regexTree;

	protected bool _hasTimeout;

	protected List<object> _searchValues;

	private Stack<LocalBuilder> _int32LocalsPool;

	private Stack<LocalBuilder> _readOnlySpanCharLocalsPool;

	private static FieldInfo RuntextstartField => _003CRuntextstartField_003Ek__BackingField ?? (_003CRuntextstartField_003Ek__BackingField = RegexRunnerField("runtextstart"));

	private static FieldInfo RuntextposField => _003CRuntextposField_003Ek__BackingField ?? (_003CRuntextposField_003Ek__BackingField = RegexRunnerField("runtextpos"));

	private static FieldInfo RuntrackposField => _003CRuntrackposField_003Ek__BackingField ?? (_003CRuntrackposField_003Ek__BackingField = RegexRunnerField("runtrackpos"));

	private static FieldInfo RunstackField => _003CRunstackField_003Ek__BackingField ?? (_003CRunstackField_003Ek__BackingField = RegexRunnerField("runstack"));

	private static FieldInfo CultureField => _003CCultureField_003Ek__BackingField ?? (_003CCultureField_003Ek__BackingField = typeof(CompiledRegexRunner).GetField("_culture", BindingFlags.Instance | BindingFlags.NonPublic));

	private static FieldInfo CaseBehaviorField => _003CCaseBehaviorField_003Ek__BackingField ?? (_003CCaseBehaviorField_003Ek__BackingField = typeof(CompiledRegexRunner).GetField("_caseBehavior", BindingFlags.Instance | BindingFlags.NonPublic));

	private static FieldInfo SearchValuesArrayField => _003CSearchValuesArrayField_003Ek__BackingField ?? (_003CSearchValuesArrayField_003Ek__BackingField = typeof(CompiledRegexRunner).GetField("_searchValues", BindingFlags.Instance | BindingFlags.NonPublic));

	private static MethodInfo CaptureMethod => _003CCaptureMethod_003Ek__BackingField ?? (_003CCaptureMethod_003Ek__BackingField = RegexRunnerMethod("Capture"));

	private static MethodInfo TransferCaptureMethod => _003CTransferCaptureMethod_003Ek__BackingField ?? (_003CTransferCaptureMethod_003Ek__BackingField = RegexRunnerMethod("TransferCapture"));

	private static MethodInfo UncaptureMethod => _003CUncaptureMethod_003Ek__BackingField ?? (_003CUncaptureMethod_003Ek__BackingField = RegexRunnerMethod("Uncapture"));

	private static MethodInfo IsMatchedMethod => _003CIsMatchedMethod_003Ek__BackingField ?? (_003CIsMatchedMethod_003Ek__BackingField = RegexRunnerMethod("IsMatched"));

	private static MethodInfo MatchLengthMethod => _003CMatchLengthMethod_003Ek__BackingField ?? (_003CMatchLengthMethod_003Ek__BackingField = RegexRunnerMethod("MatchLength"));

	private static MethodInfo MatchIndexMethod => _003CMatchIndexMethod_003Ek__BackingField ?? (_003CMatchIndexMethod_003Ek__BackingField = RegexRunnerMethod("MatchIndex"));

	private static MethodInfo IsBoundaryMethod => _003CIsBoundaryMethod_003Ek__BackingField ?? (_003CIsBoundaryMethod_003Ek__BackingField = typeof(RegexRunner).GetMethod("IsBoundary", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
	{
		typeof(ReadOnlySpan<char>),
		typeof(int)
	}));

	private static MethodInfo IsPreWordCharBoundaryMethod => _003CIsPreWordCharBoundaryMethod_003Ek__BackingField ?? (_003CIsPreWordCharBoundaryMethod_003Ek__BackingField = typeof(RegexRunner).GetMethod("IsPreWordCharBoundary", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
	{
		typeof(ReadOnlySpan<char>),
		typeof(int)
	}));

	private static MethodInfo IsPostWordCharBoundaryMethod => _003CIsPostWordCharBoundaryMethod_003Ek__BackingField ?? (_003CIsPostWordCharBoundaryMethod_003Ek__BackingField = typeof(RegexRunner).GetMethod("IsPostWordCharBoundary", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
	{
		typeof(ReadOnlySpan<char>),
		typeof(int)
	}));

	private static MethodInfo IsWordCharMethod => _003CIsWordCharMethod_003Ek__BackingField ?? (_003CIsWordCharMethod_003Ek__BackingField = RegexRunnerMethod("IsWordChar"));

	private static MethodInfo IsECMABoundaryMethod => _003CIsECMABoundaryMethod_003Ek__BackingField ?? (_003CIsECMABoundaryMethod_003Ek__BackingField = typeof(RegexRunner).GetMethod("IsECMABoundary", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
	{
		typeof(ReadOnlySpan<char>),
		typeof(int)
	}));

	private static MethodInfo CrawlposMethod => _003CCrawlposMethod_003Ek__BackingField ?? (_003CCrawlposMethod_003Ek__BackingField = RegexRunnerMethod("Crawlpos"));

	private static MethodInfo CharInClassMethod => _003CCharInClassMethod_003Ek__BackingField ?? (_003CCharInClassMethod_003Ek__BackingField = RegexRunnerMethod("CharInClass"));

	private static MethodInfo CheckTimeoutMethod => _003CCheckTimeoutMethod_003Ek__BackingField ?? (_003CCheckTimeoutMethod_003Ek__BackingField = RegexRunnerMethod("CheckTimeout"));

	private static MethodInfo RegexCaseEquivalencesTryFindCaseEquivalencesForCharWithIBehaviorMethod => _003CRegexCaseEquivalencesTryFindCaseEquivalencesForCharWithIBehaviorMethod_003Ek__BackingField ?? (_003CRegexCaseEquivalencesTryFindCaseEquivalencesForCharWithIBehaviorMethod_003Ek__BackingField = typeof(RegexCaseEquivalences).GetMethod("TryFindCaseEquivalencesForCharWithIBehavior", BindingFlags.Static | BindingFlags.Public));

	private static MethodInfo CharIsDigitMethod => _003CCharIsDigitMethod_003Ek__BackingField ?? (_003CCharIsDigitMethod_003Ek__BackingField = typeof(char).GetMethod("IsDigit", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsWhiteSpaceMethod => _003CCharIsWhiteSpaceMethod_003Ek__BackingField ?? (_003CCharIsWhiteSpaceMethod_003Ek__BackingField = typeof(char).GetMethod("IsWhiteSpace", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsControlMethod => _003CCharIsControlMethod_003Ek__BackingField ?? (_003CCharIsControlMethod_003Ek__BackingField = typeof(char).GetMethod("IsControl", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsLetterMethod => _003CCharIsLetterMethod_003Ek__BackingField ?? (_003CCharIsLetterMethod_003Ek__BackingField = typeof(char).GetMethod("IsLetter", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiDigitMethod => _003CCharIsAsciiDigitMethod_003Ek__BackingField ?? (_003CCharIsAsciiDigitMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiDigit", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiLetterMethod => _003CCharIsAsciiLetterMethod_003Ek__BackingField ?? (_003CCharIsAsciiLetterMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiLetter", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiLetterLowerMethod => _003CCharIsAsciiLetterLowerMethod_003Ek__BackingField ?? (_003CCharIsAsciiLetterLowerMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiLetterLower", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiLetterUpperMethod => _003CCharIsAsciiLetterUpperMethod_003Ek__BackingField ?? (_003CCharIsAsciiLetterUpperMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiLetterUpper", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiLetterOrDigitMethod => _003CCharIsAsciiLetterOrDigitMethod_003Ek__BackingField ?? (_003CCharIsAsciiLetterOrDigitMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiLetterOrDigit", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiHexDigitMethod => _003CCharIsAsciiHexDigitMethod_003Ek__BackingField ?? (_003CCharIsAsciiHexDigitMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiHexDigit", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiHexDigitLowerMethod => _003CCharIsAsciiHexDigitLowerMethod_003Ek__BackingField ?? (_003CCharIsAsciiHexDigitLowerMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiHexDigitLower", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsAsciiHexDigitUpperMethod => _003CCharIsAsciiHexDigitUpperMethod_003Ek__BackingField ?? (_003CCharIsAsciiHexDigitUpperMethod_003Ek__BackingField = typeof(char).GetMethod("IsAsciiHexDigitUpper", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsLetterOrDigitMethod => _003CCharIsLetterOrDigitMethod_003Ek__BackingField ?? (_003CCharIsLetterOrDigitMethod_003Ek__BackingField = typeof(char).GetMethod("IsLetterOrDigit", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsLowerMethod => _003CCharIsLowerMethod_003Ek__BackingField ?? (_003CCharIsLowerMethod_003Ek__BackingField = typeof(char).GetMethod("IsLower", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsUpperMethod => _003CCharIsUpperMethod_003Ek__BackingField ?? (_003CCharIsUpperMethod_003Ek__BackingField = typeof(char).GetMethod("IsUpper", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsNumberMethod => _003CCharIsNumberMethod_003Ek__BackingField ?? (_003CCharIsNumberMethod_003Ek__BackingField = typeof(char).GetMethod("IsNumber", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsPunctuationMethod => _003CCharIsPunctuationMethod_003Ek__BackingField ?? (_003CCharIsPunctuationMethod_003Ek__BackingField = typeof(char).GetMethod("IsPunctuation", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsSeparatorMethod => _003CCharIsSeparatorMethod_003Ek__BackingField ?? (_003CCharIsSeparatorMethod_003Ek__BackingField = typeof(char).GetMethod("IsSeparator", new Type[1] { typeof(char) }));

	private static MethodInfo CharIsSymbolMethod => _003CCharIsSymbolMethod_003Ek__BackingField ?? (_003CCharIsSymbolMethod_003Ek__BackingField = typeof(char).GetMethod("IsSymbol", new Type[1] { typeof(char) }));

	private static MethodInfo CharGetUnicodeInfoMethod => _003CCharGetUnicodeInfoMethod_003Ek__BackingField ?? (_003CCharGetUnicodeInfoMethod_003Ek__BackingField = typeof(char).GetMethod("GetUnicodeCategory", new Type[1] { typeof(char) }));

	private static MethodInfo SpanGetItemMethod => _003CSpanGetItemMethod_003Ek__BackingField ?? (_003CSpanGetItemMethod_003Ek__BackingField = typeof(ReadOnlySpan<char>).GetMethod("get_Item", new Type[1] { typeof(int) }));

	private static MethodInfo SpanGetLengthMethod => _003CSpanGetLengthMethod_003Ek__BackingField ?? (_003CSpanGetLengthMethod_003Ek__BackingField = typeof(ReadOnlySpan<char>).GetMethod("get_Length"));

	private static MethodInfo SpanIndexOfCharMethod => _003CSpanIndexOfCharMethod_003Ek__BackingField ?? (_003CSpanIndexOfCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOf", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfSpanMethod => _003CSpanIndexOfSpanMethod_003Ek__BackingField ?? (_003CSpanIndexOfSpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOf", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfSpanStringComparisonMethod => _003CSpanIndexOfSpanStringComparisonMethod_003Ek__BackingField ?? (_003CSpanIndexOfSpanStringComparisonMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOf", new Type[3]
	{
		typeof(ReadOnlySpan<char>),
		typeof(ReadOnlySpan<char>),
		typeof(StringComparison)
	}));

	private static MethodInfo SpanIndexOfAnyCharCharMethod => _003CSpanIndexOfAnyCharCharMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAny", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyCharCharCharMethod => _003CSpanIndexOfAnyCharCharCharMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyCharCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAny", new Type[4]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnySpanMethod => _003CSpanIndexOfAnySpanMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnySpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAny", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnySearchValuesMethod => _003CSpanIndexOfAnySearchValuesMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnySearchValuesMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAny", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(SearchValues<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnySearchValuesStringMethod => _003CSpanIndexOfAnySearchValuesStringMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnySearchValuesStringMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAny", new Type[2]
	{
		typeof(ReadOnlySpan<char>),
		typeof(SearchValues<string>)
	}));

	private static MethodInfo SpanIndexOfAnyExceptCharMethod => _003CSpanIndexOfAnyExceptCharMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyExceptCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyExcept", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyExceptCharCharMethod => _003CSpanIndexOfAnyExceptCharCharMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyExceptCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyExcept", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyExceptCharCharCharMethod => _003CSpanIndexOfAnyExceptCharCharCharMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyExceptCharCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyExcept", new Type[4]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyExceptSpanMethod => _003CSpanIndexOfAnyExceptSpanMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyExceptSpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyExcept", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyExceptSearchValuesMethod => _003CSpanIndexOfAnyExceptSearchValuesMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyExceptSearchValuesMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyExcept", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(SearchValues<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyInRangeMethod => _003CSpanIndexOfAnyInRangeMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyInRangeMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyInRange", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanIndexOfAnyExceptInRangeMethod => _003CSpanIndexOfAnyExceptInRangeMethod_003Ek__BackingField ?? (_003CSpanIndexOfAnyExceptInRangeMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("IndexOfAnyExceptInRange", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfCharMethod => _003CSpanLastIndexOfCharMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOf", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyCharCharMethod => _003CSpanLastIndexOfAnyCharCharMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAny", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyCharCharCharMethod => _003CSpanLastIndexOfAnyCharCharCharMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyCharCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAny", new Type[4]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnySpanMethod => _003CSpanLastIndexOfAnySpanMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnySpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAny", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnySearchValuesMethod => _003CSpanLastIndexOfAnySearchValuesMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnySearchValuesMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAny", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(SearchValues<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfSpanMethod => _003CSpanLastIndexOfSpanMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfSpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOf", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyExceptCharMethod => _003CSpanLastIndexOfAnyExceptCharMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyExceptCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyExcept", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyExceptCharCharMethod => _003CSpanLastIndexOfAnyExceptCharCharMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyExceptCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyExcept", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyExceptCharCharCharMethod => _003CSpanLastIndexOfAnyExceptCharCharCharMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyExceptCharCharCharMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyExcept", new Type[4]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyExceptSpanMethod => _003CSpanLastIndexOfAnyExceptSpanMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyExceptSpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyExcept", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyExceptSearchValuesMethod => _003CSpanLastIndexOfAnyExceptSearchValuesMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyExceptSearchValuesMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyExcept", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(SearchValues<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyInRangeMethod => _003CSpanLastIndexOfAnyInRangeMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyInRangeMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyInRange", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanLastIndexOfAnyExceptInRangeMethod => _003CSpanLastIndexOfAnyExceptInRangeMethod_003Ek__BackingField ?? (_003CSpanLastIndexOfAnyExceptInRangeMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("LastIndexOfAnyExceptInRange", new Type[3]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		Type.MakeGenericMethodParameter(0),
		Type.MakeGenericMethodParameter(0)
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanSliceIntMethod => _003CSpanSliceIntMethod_003Ek__BackingField ?? (_003CSpanSliceIntMethod_003Ek__BackingField = typeof(ReadOnlySpan<char>).GetMethod("Slice", new Type[1] { typeof(int) }));

	private static MethodInfo SpanSliceIntIntMethod => _003CSpanSliceIntIntMethod_003Ek__BackingField ?? (_003CSpanSliceIntIntMethod_003Ek__BackingField = typeof(ReadOnlySpan<char>).GetMethod("Slice", new Type[2]
	{
		typeof(int),
		typeof(int)
	}));

	private static MethodInfo SpanStartsWithSpanMethod => _003CSpanStartsWithSpanMethod_003Ek__BackingField ?? (_003CSpanStartsWithSpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("StartsWith", new Type[2]
	{
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
		typeof(ReadOnlySpan<>).MakeGenericType(Type.MakeGenericMethodParameter(0))
	}).MakeGenericMethod(typeof(char)));

	private static MethodInfo SpanStartsWithSpanComparisonMethod => _003CSpanStartsWithSpanComparisonMethod_003Ek__BackingField ?? (_003CSpanStartsWithSpanComparisonMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("StartsWith", new Type[3]
	{
		typeof(ReadOnlySpan<char>),
		typeof(ReadOnlySpan<char>),
		typeof(StringComparison)
	}));

	private static MethodInfo StringAsSpanMethod => _003CStringAsSpanMethod_003Ek__BackingField ?? (_003CStringAsSpanMethod_003Ek__BackingField = typeof(MemoryExtensions).GetMethod("AsSpan", new Type[1] { typeof(string) }));

	private static MethodInfo StringGetCharsMethod => _003CStringGetCharsMethod_003Ek__BackingField ?? (_003CStringGetCharsMethod_003Ek__BackingField = typeof(string).GetMethod("get_Chars", new Type[1] { typeof(int) }));

	private static MethodInfo ArrayResizeMethod => _003CArrayResizeMethod_003Ek__BackingField ?? (_003CArrayResizeMethod_003Ek__BackingField = typeof(Array).GetMethod("Resize").MakeGenericMethod(typeof(int)));

	private static MethodInfo MathMinIntIntMethod => _003CMathMinIntIntMethod_003Ek__BackingField ?? (_003CMathMinIntIntMethod_003Ek__BackingField = typeof(Math).GetMethod("Min", new Type[2]
	{
		typeof(int),
		typeof(int)
	}));

	private static MethodInfo MemoryMarshalGetArrayDataReferenceSearchValuesMethod => _003CMemoryMarshalGetArrayDataReferenceSearchValuesMethod_003Ek__BackingField ?? (_003CMemoryMarshalGetArrayDataReferenceSearchValuesMethod_003Ek__BackingField = typeof(MemoryMarshal).GetMethod("GetArrayDataReference", new Type[1] { Type.MakeGenericMethodParameter(0).MakeArrayType() }).MakeGenericMethod(typeof(SearchValues<char>)));

	private static MethodInfo UnsafeAsMethod => _003CUnsafeAsMethod_003Ek__BackingField ?? (_003CUnsafeAsMethod_003Ek__BackingField = typeof(Unsafe).GetMethod("As", new Type[1] { typeof(object) }));

	private static FieldInfo RegexRunnerField(string fieldname)
	{
		return typeof(RegexRunner).GetField(fieldname, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}

	private static MethodInfo RegexRunnerMethod(string methname)
	{
		return typeof(RegexRunner).GetMethod(methname, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}

	internal static RegexRunnerFactory Compile(string pattern, RegexTree regexTree, RegexOptions options, bool hasTimeout)
	{
		return new RegexLWCGCompiler().FactoryInstanceFromCode(pattern, regexTree, options, hasTimeout);
	}

	private Label DefineLabel()
	{
		return _ilg.DefineLabel();
	}

	private void MarkLabel(Label l)
	{
		_ilg.MarkLabel(l);
	}

	protected void Ldstr(string str)
	{
		_ilg.Emit(OpCodes.Ldstr, str);
	}

	protected void Ldc(int i)
	{
		_ilg.Emit(OpCodes.Ldc_I4, i);
	}

	protected void LdcI8(long i)
	{
		_ilg.Emit(OpCodes.Ldc_I8, i);
	}

	protected void Ret()
	{
		_ilg.Emit(OpCodes.Ret);
	}

	protected void Dup()
	{
		_ilg.Emit(OpCodes.Dup);
	}

	private void Ceq()
	{
		_ilg.Emit(OpCodes.Ceq);
	}

	private void CgtUn()
	{
		_ilg.Emit(OpCodes.Cgt_Un);
	}

	private void CltUn()
	{
		_ilg.Emit(OpCodes.Clt_Un);
	}

	private void Pop()
	{
		_ilg.Emit(OpCodes.Pop);
	}

	private void Add()
	{
		_ilg.Emit(OpCodes.Add);
	}

	private void Sub()
	{
		_ilg.Emit(OpCodes.Sub);
	}

	private void Mul()
	{
		_ilg.Emit(OpCodes.Mul);
	}

	private void And()
	{
		_ilg.Emit(OpCodes.And);
	}

	private void Or()
	{
		_ilg.Emit(OpCodes.Or);
	}

	private void Shl()
	{
		_ilg.Emit(OpCodes.Shl);
	}

	private void Shr()
	{
		_ilg.Emit(OpCodes.Shr);
	}

	private void Ldloc(LocalBuilder lt)
	{
		_ilg.Emit(OpCodes.Ldloc, lt);
	}

	private void Ldloca(LocalBuilder lt)
	{
		_ilg.Emit(OpCodes.Ldloca, lt);
	}

	private void LdindU2()
	{
		_ilg.Emit(OpCodes.Ldind_U2);
	}

	private void Stloc(LocalBuilder lt)
	{
		_ilg.Emit(OpCodes.Stloc, lt);
	}

	protected void Ldthis()
	{
		_ilg.Emit(OpCodes.Ldarg_0);
	}

	private void Ldarg_1()
	{
		_ilg.Emit(OpCodes.Ldarg_1);
	}

	protected void Ldthisfld(FieldInfo ft)
	{
		Ldthis();
		_ilg.Emit(OpCodes.Ldfld, ft);
	}

	protected void Ldthisflda(FieldInfo ft)
	{
		Ldthis();
		_ilg.Emit(OpCodes.Ldflda, ft);
	}

	private void Ldarga_s(int position)
	{
		_ilg.Emit(OpCodes.Ldarga_S, position);
	}

	private void Mvfldloc(FieldInfo ft, LocalBuilder lt)
	{
		Ldthisfld(ft);
		Stloc(lt);
	}

	protected void Stfld(FieldInfo ft)
	{
		_ilg.Emit(OpCodes.Stfld, ft);
	}

	protected void Call(MethodInfo mt)
	{
		_ilg.Emit(OpCodes.Call, mt);
	}

	private void Brfalse(Label l)
	{
		_ilg.Emit(OpCodes.Brfalse_S, l);
	}

	private void BrfalseFar(Label l)
	{
		_ilg.Emit(OpCodes.Brfalse, l);
	}

	private void BrtrueFar(Label l)
	{
		_ilg.Emit(OpCodes.Brtrue, l);
	}

	private void BrFar(Label l)
	{
		_ilg.Emit(OpCodes.Br, l);
	}

	private void BleFar(Label l)
	{
		_ilg.Emit(OpCodes.Ble, l);
	}

	private void BltFar(Label l)
	{
		_ilg.Emit(OpCodes.Blt, l);
	}

	private void BltUnFar(Label l)
	{
		_ilg.Emit(OpCodes.Blt_Un, l);
	}

	private void BgeFar(Label l)
	{
		_ilg.Emit(OpCodes.Bge, l);
	}

	private void BgeUnFar(Label l)
	{
		_ilg.Emit(OpCodes.Bge_Un, l);
	}

	private void BneFar(Label l)
	{
		_ilg.Emit(OpCodes.Bne_Un, l);
	}

	private void BeqFar(Label l)
	{
		_ilg.Emit(OpCodes.Beq, l);
	}

	private void Brtrue(Label l)
	{
		_ilg.Emit(OpCodes.Brtrue_S, l);
	}

	private void Br(Label l)
	{
		_ilg.Emit(OpCodes.Br_S, l);
	}

	private void Ble(Label l)
	{
		_ilg.Emit(OpCodes.Ble_S, l);
	}

	private void Blt(Label l)
	{
		_ilg.Emit(OpCodes.Blt_S, l);
	}

	private void Bge(Label l)
	{
		_ilg.Emit(OpCodes.Bge_S, l);
	}

	private void BgeUn(Label l)
	{
		_ilg.Emit(OpCodes.Bge_Un_S, l);
	}

	private void Bgt(Label l)
	{
		_ilg.Emit(OpCodes.Bgt_S, l);
	}

	private void Bne(Label l)
	{
		_ilg.Emit(OpCodes.Bne_Un_S, l);
	}

	private void Beq(Label l)
	{
		_ilg.Emit(OpCodes.Beq_S, l);
	}

	private void Ldlen()
	{
		_ilg.Emit(OpCodes.Ldlen);
	}

	private void LdelemI4()
	{
		_ilg.Emit(OpCodes.Ldelem_I4);
	}

	private void StelemI4()
	{
		_ilg.Emit(OpCodes.Stelem_I4);
	}

	private void Switch(Label[] table)
	{
		_ilg.Emit(OpCodes.Switch, table);
	}

	private LocalBuilder DeclareInt32()
	{
		return _ilg.DeclareLocal(typeof(int));
	}

	private LocalBuilder DeclareReadOnlySpanChar()
	{
		return _ilg.DeclareLocal(typeof(ReadOnlySpan<char>));
	}

	private RentedLocalBuilder RentInt32Local()
	{
		LocalBuilder result;
		return new RentedLocalBuilder(_int32LocalsPool ?? (_int32LocalsPool = new Stack<LocalBuilder>()), _int32LocalsPool.TryPop(out result) ? result : DeclareInt32());
	}

	private RentedLocalBuilder RentReadOnlySpanCharLocal()
	{
		LocalBuilder result;
		return new RentedLocalBuilder(_readOnlySpanCharLocalsPool ?? (_readOnlySpanCharLocalsPool = new Stack<LocalBuilder>(1)), _readOnlySpanCharLocalsPool.TryPop(out result) ? result : DeclareReadOnlySpanChar());
	}

	protected void EmitTryFindNextPossibleStartingPosition()
	{
		_int32LocalsPool?.Clear();
		_readOnlySpanCharLocalsPool?.Clear();
		LocalBuilder inputSpan = DeclareReadOnlySpanChar();
		LocalBuilder pos = DeclareInt32();
		bool rtl = (_options & RegexOptions.RightToLeft) != 0;
		Mvfldloc(RuntextposField, pos);
		Ldarg_1();
		Stloc(inputSpan);
		int minRequiredLength = _regexTree.FindOptimizations.MinRequiredLength;
		Label returnFalse = DefineLabel();
		Label l = DefineLabel();
		Ldloc(pos);
		if (!rtl)
		{
			Ldloca(inputSpan);
			Call(SpanGetLengthMethod);
			if (minRequiredLength > 0)
			{
				Ldc(minRequiredLength);
				Sub();
			}
			Ble(l);
		}
		else
		{
			Ldc(minRequiredLength);
			Bge(l);
		}
		MarkLabel(returnFalse);
		Ldthis();
		if (!rtl)
		{
			Ldloca(inputSpan);
			Call(SpanGetLengthMethod);
		}
		else
		{
			Ldc(0);
		}
		Stfld(RuntextposField);
		Ldc(0);
		Ret();
		MarkLabel(l);
		if (!EmitAnchors())
		{
			switch (_regexTree.FindOptimizations.FindMode)
			{
			case FindNextStartingPositionMode.LeadingString_LeftToRight:
			case FindNextStartingPositionMode.LeadingString_OrdinalIgnoreCase_LeftToRight:
			case FindNextStartingPositionMode.LeadingStrings_LeftToRight:
			case FindNextStartingPositionMode.LeadingStrings_OrdinalIgnoreCase_LeftToRight:
			case FindNextStartingPositionMode.FixedDistanceString_LeftToRight:
				EmitIndexOfString_LeftToRight();
				break;
			case FindNextStartingPositionMode.LeadingString_RightToLeft:
				EmitIndexOf_RightToLeft();
				break;
			case FindNextStartingPositionMode.LeadingSet_LeftToRight:
			case FindNextStartingPositionMode.FixedDistanceSets_LeftToRight:
				EmitFixedSet_LeftToRight();
				break;
			case FindNextStartingPositionMode.LeadingSet_RightToLeft:
				EmitFixedSet_RightToLeft();
				break;
			case FindNextStartingPositionMode.LiteralAfterLoop_LeftToRight:
				EmitLiteralAfterAtomicLoop();
				break;
			default:
				Ldc(1);
				Ret();
				break;
			}
		}
		bool EmitAnchors()
		{
			switch (_regexTree.FindOptimizations.FindMode)
			{
			case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_Beginning:
				Ldloc(pos);
				Ldc(0);
				Bne(returnFalse);
				Ldc(1);
				Ret();
				return true;
			case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_Start:
			case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Start:
				Ldloc(pos);
				Ldthisfld(RuntextstartField);
				Bne(returnFalse);
				Ldc(1);
				Ret();
				return true;
			case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_EndZ:
			{
				Label l2 = DefineLabel();
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Ldc(1);
				Sub();
				Bge(l2);
				Ldthis();
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Ldc(1);
				Sub();
				Stfld(RuntextposField);
				MarkLabel(l2);
				Ldc(1);
				Ret();
				return true;
			}
			case FindNextStartingPositionMode.LeadingAnchor_LeftToRight_End:
			{
				Label l2 = DefineLabel();
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Bge(l2);
				Ldthis();
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Stfld(RuntextposField);
				MarkLabel(l2);
				Ldc(1);
				Ret();
				return true;
			}
			case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Beginning:
			{
				Label l2 = DefineLabel();
				Ldloc(pos);
				Ldc(0);
				Beq(l2);
				Ldthis();
				Ldc(0);
				Stfld(RuntextposField);
				MarkLabel(l2);
				Ldc(1);
				Ret();
				return true;
			}
			case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_EndZ:
			{
				Label l2 = DefineLabel();
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Ldc(1);
				Sub();
				Blt(returnFalse);
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				BgeUn(l2);
				Ldloca(inputSpan);
				Ldloc(pos);
				Call(SpanGetItemMethod);
				LdindU2();
				Ldc(10);
				Bne(returnFalse);
				MarkLabel(l2);
				Ldc(1);
				Ret();
				return true;
			}
			case FindNextStartingPositionMode.LeadingAnchor_RightToLeft_End:
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Blt(returnFalse);
				Ldc(1);
				Ret();
				return true;
			case FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_End:
			case FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_EndZ:
			{
				int num2 = ((_regexTree.FindOptimizations.FindMode == FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_EndZ) ? 1 : 0);
				Label l2 = DefineLabel();
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Ldc(_regexTree.FindOptimizations.MinRequiredLength + num2);
				Sub();
				Bge(l2);
				Ldthis();
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				Ldc(_regexTree.FindOptimizations.MinRequiredLength + num2);
				Sub();
				Stfld(RuntextposField);
				MarkLabel(l2);
				Ldc(1);
				Ret();
				return true;
			}
			default:
				if (!rtl)
				{
					if (_regexTree.FindOptimizations.LeadingAnchor == RegexNodeKind.Bol)
					{
						Label l2 = DefineLabel();
						Ldloc(pos);
						Ldc(0);
						Ble(l2);
						Ldloca(inputSpan);
						Ldloc(pos);
						Ldc(1);
						Sub();
						Call(SpanGetItemMethod);
						LdindU2();
						Ldc(10);
						Beq(l2);
						Ldloca(inputSpan);
						Ldloc(pos);
						Call(SpanSliceIntMethod);
						Ldc(10);
						Call(SpanIndexOfCharMethod);
						using (RentedLocalBuilder rentedLocalBuilder = RentInt32Local())
						{
							Stloc(rentedLocalBuilder);
							Ldloc(rentedLocalBuilder);
							Ldc(0);
							Blt(returnFalse);
							Ldloc(rentedLocalBuilder);
							Ldloc(pos);
							Add();
							Ldc(1);
							Add();
							Ldloca(inputSpan);
							Call(SpanGetLengthMethod);
							Bgt(returnFalse);
							Ldloc(pos);
							Ldloc(rentedLocalBuilder);
							Add();
							Ldc(1);
							Add();
							Stloc(pos);
							Ldloca(inputSpan);
							Call(SpanGetLengthMethod);
							if (minRequiredLength != 0)
							{
								Ldc(minRequiredLength);
								Sub();
							}
							Ldloc(pos);
							BltFar(returnFalse);
						}
						MarkLabel(l2);
					}
					RegexNodeKind trailingAnchor = _regexTree.FindOptimizations.TrailingAnchor;
					if (trailingAnchor - 20 <= (RegexNodeKind)1)
					{
						int? maxPossibleLength = _regexTree.FindOptimizations.MaxPossibleLength;
						if (maxPossibleLength.HasValue)
						{
							int valueOrDefault = maxPossibleLength.GetValueOrDefault();
							int num = ((_regexTree.FindOptimizations.FindMode == FindNextStartingPositionMode.TrailingAnchor_FixedLength_LeftToRight_EndZ) ? 1 : 0);
							Label l2 = DefineLabel();
							Ldloc(pos);
							Ldloca(inputSpan);
							Call(SpanGetLengthMethod);
							Ldc(valueOrDefault + num);
							Sub();
							Bge(l2);
							Ldloca(inputSpan);
							Call(SpanGetLengthMethod);
							Ldc(valueOrDefault + num);
							Sub();
							Stloc(pos);
							MarkLabel(l2);
						}
					}
				}
				return false;
			}
		}
		void EmitFixedSet_LeftToRight()
		{
			List<RegexFindOptimizations.FixedDistanceSet> fixedDistanceSets = _regexTree.FindOptimizations.FixedDistanceSets;
			RegexFindOptimizations.FixedDistanceSet fixedDistanceSet = fixedDistanceSets[0];
			int num = Math.Min(fixedDistanceSets.Count, 4);
			using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
			using RentedLocalBuilder rentedLocalBuilder2 = RentReadOnlySpanCharLocal();
			Ldloca(inputSpan);
			Ldloc(pos);
			Call(SpanSliceIntMethod);
			Stloc(rentedLocalBuilder2);
			int i = 0;
			int num2;
			int num3;
			if (fixedDistanceSet.Set != "\u0001\u0002\0\n\v")
			{
				num2 = ((fixedDistanceSet.Set != "\0\u0001\0\0") ? 1 : 0);
				if (num2 != 0)
				{
					num3 = ((num > 1) ? 1 : 0);
					goto IL_0097;
				}
			}
			else
			{
				num2 = 0;
			}
			num3 = 1;
			goto IL_0097;
			IL_0097:
			bool flag = (byte)num3 != 0;
			Label l2 = default(Label);
			Label l3 = default(Label);
			Label l4 = default(Label);
			if (flag)
			{
				l2 = DefineLabel();
				l3 = DefineLabel();
				l4 = DefineLabel();
				Ldc(0);
				Stloc(rentedLocalBuilder);
				BrFar(l2);
				MarkLabel(l4);
			}
			if (num2 != 0)
			{
				i = 1;
				if (flag)
				{
					Ldloca(rentedLocalBuilder2);
					Ldloc(rentedLocalBuilder);
					if (fixedDistanceSet.Distance != 0)
					{
						Ldc(fixedDistanceSet.Distance);
						Add();
					}
					Call(SpanSliceIntMethod);
				}
				else if (fixedDistanceSet.Distance != 0)
				{
					Ldloca(rentedLocalBuilder2);
					Ldc(fixedDistanceSet.Distance);
					Call(SpanSliceIntMethod);
				}
				else
				{
					Ldloc(rentedLocalBuilder2);
				}
				if (fixedDistanceSet.Chars != null)
				{
					switch (fixedDistanceSet.Chars.Length)
					{
					case 1:
						Ldc(fixedDistanceSet.Chars[0]);
						Call(fixedDistanceSet.Negated ? SpanIndexOfAnyExceptCharMethod : SpanIndexOfCharMethod);
						break;
					case 2:
						Ldc(fixedDistanceSet.Chars[0]);
						Ldc(fixedDistanceSet.Chars[1]);
						Call(fixedDistanceSet.Negated ? SpanIndexOfAnyExceptCharCharMethod : SpanIndexOfAnyCharCharMethod);
						break;
					case 3:
						Ldc(fixedDistanceSet.Chars[0]);
						Ldc(fixedDistanceSet.Chars[1]);
						Ldc(fixedDistanceSet.Chars[2]);
						Call(fixedDistanceSet.Negated ? SpanIndexOfAnyExceptCharCharCharMethod : SpanIndexOfAnyCharCharCharMethod);
						break;
					default:
						EmitIndexOfAnyWithSearchValuesOrLiteral(fixedDistanceSet.Chars, last: false, fixedDistanceSet.Negated);
						break;
					}
				}
				else
				{
					(char, char)? range = fixedDistanceSet.Range;
					char[] chars;
					bool negated;
					string description;
					if (range.HasValue)
					{
						if (fixedDistanceSet.Range.Value.LowInclusive == fixedDistanceSet.Range.Value.HighInclusive)
						{
							Ldc(fixedDistanceSet.Range.Value.LowInclusive);
							Call(fixedDistanceSet.Negated ? SpanIndexOfAnyExceptCharMethod : SpanIndexOfCharMethod);
						}
						else
						{
							Ldc(fixedDistanceSet.Range.Value.LowInclusive);
							Ldc(fixedDistanceSet.Range.Value.HighInclusive);
							Call(fixedDistanceSet.Negated ? SpanIndexOfAnyExceptInRangeMethod : SpanIndexOfAnyInRangeMethod);
						}
					}
					else if (RegexCharClass.IsUnicodeCategoryOfSmallCharCount(fixedDistanceSet.Set, out chars, out negated, out description))
					{
						LoadSearchValues(chars);
						Call(negated ? SpanIndexOfAnyExceptSearchValuesMethod : SpanIndexOfAnySearchValuesMethod);
					}
					else
					{
						Span<char> scratchBuffer = stackalloc char[128];
						System.Collections.Generic.ValueListBuilder<char> valueListBuilder = new System.Collections.Generic.ValueListBuilder<char>(scratchBuffer);
						try
						{
							for (int j = 0; j < 128; j++)
							{
								if (!RegexCharClass.CharInClass((char)j, fixedDistanceSet.Set))
								{
									valueListBuilder.Append((char)j);
								}
							}
							using RentedLocalBuilder rentedLocalBuilder3 = RentReadOnlySpanCharLocal();
							using RentedLocalBuilder rentedLocalBuilder4 = RentInt32Local();
							Stloc(rentedLocalBuilder3);
							Ldloc(rentedLocalBuilder3);
							if (valueListBuilder.Length == 128)
							{
								Ldc(0);
								Ldc(127);
								Call(SpanIndexOfAnyExceptInRangeMethod);
							}
							else
							{
								LoadSearchValues(valueListBuilder.AsSpan().ToArray());
								Call(SpanIndexOfAnyExceptSearchValuesMethod);
							}
							Stloc(rentedLocalBuilder4);
							Label l5 = DefineLabel();
							Ldloc(rentedLocalBuilder4);
							Ldloca(rentedLocalBuilder3);
							Call(SpanGetLengthMethod);
							BgeUnFar(l5);
							Ldc(127);
							Ldloca(rentedLocalBuilder3);
							Ldloc(rentedLocalBuilder4);
							Call(SpanGetItemMethod);
							LdindU2();
							BgeUnFar(l5);
							Label l6 = DefineLabel();
							MarkLabel(l6);
							Ldloca(rentedLocalBuilder3);
							Ldloc(rentedLocalBuilder4);
							Call(SpanGetItemMethod);
							LdindU2();
							EmitMatchCharacterClass(fixedDistanceSet.Set);
							Brtrue(l5);
							Ldloc(rentedLocalBuilder4);
							Ldc(1);
							Add();
							Stloc(rentedLocalBuilder4);
							Ldloc(rentedLocalBuilder4);
							Ldloca(rentedLocalBuilder3);
							Call(SpanGetLengthMethod);
							BltUnFar(l6);
							Ldc(-1);
							Stloc(rentedLocalBuilder4);
							MarkLabel(l5);
							Ldloc(rentedLocalBuilder4);
						}
						finally
						{
							valueListBuilder.Dispose();
						}
					}
				}
				if (flag)
				{
					using RentedLocalBuilder rentedLocalBuilder5 = RentInt32Local();
					Stloc(rentedLocalBuilder5);
					Ldloc(rentedLocalBuilder);
					Ldloc(rentedLocalBuilder5);
					Add();
					Stloc(rentedLocalBuilder);
					Ldloc(rentedLocalBuilder5);
					Ldc(0);
					BltFar(returnFalse);
				}
				else
				{
					Stloc(rentedLocalBuilder);
					Ldloc(rentedLocalBuilder);
					Ldc(0);
					BltFar(returnFalse);
				}
				if (num > 1)
				{
					int num4 = fixedDistanceSets[1].Distance;
					for (int k = 2; k < num; k++)
					{
						num4 = Math.Max(num4, fixedDistanceSets[k].Distance);
					}
					if (num4 > fixedDistanceSet.Distance && num > 1)
					{
						Ldloc(rentedLocalBuilder);
						Ldc(num4);
						Add();
						Ldloca(rentedLocalBuilder2);
						Call(SpanGetLengthMethod);
						_ilg.Emit(OpCodes.Bge_Un, returnFalse);
					}
				}
			}
			for (; i < num; i++)
			{
				Ldloca(rentedLocalBuilder2);
				Ldloc(rentedLocalBuilder);
				if (fixedDistanceSets[i].Distance != 0)
				{
					Ldc(fixedDistanceSets[i].Distance);
					Add();
				}
				Call(SpanGetItemMethod);
				LdindU2();
				EmitMatchCharacterClass(fixedDistanceSets[i].Set);
				BrfalseFar(l3);
			}
			Ldthis();
			Ldloc(pos);
			Ldloc(rentedLocalBuilder);
			Add();
			Stfld(RuntextposField);
			Ldc(1);
			Ret();
			if (flag)
			{
				MarkLabel(l3);
				Ldloc(rentedLocalBuilder);
				Ldc(1);
				Add();
				Stloc(rentedLocalBuilder);
				MarkLabel(l2);
				Ldloc(rentedLocalBuilder);
				Ldloca(rentedLocalBuilder2);
				Call(SpanGetLengthMethod);
				if (num > 1 || fixedDistanceSet.Distance != 0)
				{
					Ldc(minRequiredLength - 1);
					Sub();
				}
				BltFar(l4);
				BrFar(returnFalse);
			}
		}
		void EmitFixedSet_RightToLeft()
		{
			RegexFindOptimizations.FixedDistanceSet fixedDistanceSet = _regexTree.FindOptimizations.FixedDistanceSets[0];
			char[] chars = fixedDistanceSet.Chars;
			if (chars != null && chars.Length == 1)
			{
				Ldloca(inputSpan);
				Ldc(0);
				Ldloc(pos);
				Call(SpanSliceIntIntMethod);
				Ldc(fixedDistanceSet.Chars[0]);
				Call(SpanLastIndexOfCharMethod);
				Stloc(pos);
				Ldloc(pos);
				Ldc(0);
				BltFar(returnFalse);
				Ldthis();
				Ldloc(pos);
				Ldc(1);
				Add();
				Stfld(RuntextposField);
				Ldc(1);
				Ret();
			}
			else
			{
				Label l2 = DefineLabel();
				MarkLabel(l2);
				Ldloc(pos);
				Ldc(1);
				Sub();
				Stloc(pos);
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				BgeUnFar(returnFalse);
				Ldloca(inputSpan);
				Ldloc(pos);
				Call(SpanGetItemMethod);
				LdindU2();
				EmitMatchCharacterClass(fixedDistanceSet.Set);
				Brfalse(l2);
				Ldthis();
				Ldloc(pos);
				Ldc(1);
				Add();
				Stfld(RuntextposField);
				Ldc(1);
				Ret();
			}
		}
		void EmitIndexOfString_LeftToRight()
		{
			RegexFindOptimizations findOptimizations = _regexTree.FindOptimizations;
			using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
			Ldloca(inputSpan);
			Ldloc(pos);
			if (findOptimizations.FindMode == FindNextStartingPositionMode.FixedDistanceString_LeftToRight)
			{
				(char, string, int) fixedDistanceLiteral = findOptimizations.FixedDistanceLiteral;
				if (fixedDistanceLiteral.Item3 > 0)
				{
					Ldc(fixedDistanceLiteral.Item3);
					Add();
				}
			}
			Call(SpanSliceIntMethod);
			FindNextStartingPositionMode findMode = findOptimizations.FindMode;
			if ((uint)(findMode - 13) <= 1u)
			{
				LoadSearchValues(findOptimizations.LeadingPrefixes, (findOptimizations.FindMode == FindNextStartingPositionMode.LeadingStrings_OrdinalIgnoreCase_LeftToRight) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
				Call(SpanIndexOfAnySearchValuesStringMethod);
			}
			else
			{
				findMode = findOptimizations.FindMode;
				bool flag = ((findMode == FindNextStartingPositionMode.LeadingString_LeftToRight || findMode == FindNextStartingPositionMode.LeadingString_OrdinalIgnoreCase_LeftToRight) ? true : false);
				string text = (flag ? findOptimizations.LeadingPrefix : findOptimizations.FixedDistanceLiteral.String);
				LoadSearchValues(new string[1] { text }, (findOptimizations.FindMode == FindNextStartingPositionMode.LeadingString_OrdinalIgnoreCase_LeftToRight) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
				Call(SpanIndexOfAnySearchValuesStringMethod);
			}
			Stloc(rentedLocalBuilder);
			Ldloc(rentedLocalBuilder);
			Ldc(0);
			BltFar(returnFalse);
			Ldthis();
			Ldloc(pos);
			Ldloc(rentedLocalBuilder);
			Add();
			Stfld(RuntextposField);
			Ldc(1);
			Ret();
		}
		void EmitIndexOf_RightToLeft()
		{
			string leadingPrefix = _regexTree.FindOptimizations.LeadingPrefix;
			Ldloca(inputSpan);
			Ldc(0);
			Ldloc(pos);
			Call(SpanSliceIntIntMethod);
			Ldstr(leadingPrefix);
			Call(StringAsSpanMethod);
			Call(SpanLastIndexOfSpanMethod);
			Stloc(pos);
			Ldloc(pos);
			Ldc(0);
			BltFar(returnFalse);
			Ldthis();
			Ldloc(pos);
			Ldc(leadingPrefix.Length);
			Add();
			Stfld(RuntextposField);
			Ldc(1);
			Ret();
		}
		void EmitLiteralAfterAtomicLoop()
		{
			(RegexNode, (char, string, StringComparison, char[])) value = _regexTree.FindOptimizations.LiteralAfterLoop.Value;
			Label l2 = DefineLabel();
			Label l3 = DefineLabel();
			MarkLabel(l2);
			using RentedLocalBuilder rentedLocalBuilder = RentReadOnlySpanCharLocal();
			Ldloca(inputSpan);
			Ldloc(pos);
			Call(SpanSliceIntMethod);
			Stloc(rentedLocalBuilder);
			using RentedLocalBuilder rentedLocalBuilder2 = RentInt32Local();
			Ldloc(rentedLocalBuilder);
			string item = value.Item2.Item2;
			if (item != null)
			{
				Ldstr(item);
				Call(StringAsSpanMethod);
				if (value.Item2.Item3 == StringComparison.OrdinalIgnoreCase)
				{
					Ldc((int)value.Item2.Item3);
					Call(SpanIndexOfSpanStringComparisonMethod);
				}
				else
				{
					Call(SpanIndexOfSpanMethod);
				}
			}
			else
			{
				char[] item2 = value.Item2.Item4;
				if (item2 == null)
				{
					Ldc(value.Item2.Item1);
					Call(SpanIndexOfCharMethod);
				}
				else
				{
					switch (item2.Length)
					{
					case 2:
						Ldc(item2[0]);
						Ldc(item2[1]);
						Call(SpanIndexOfAnyCharCharMethod);
						break;
					case 3:
						Ldc(item2[0]);
						Ldc(item2[1]);
						Ldc(item2[2]);
						Call(SpanIndexOfAnyCharCharCharMethod);
						break;
					default:
						Ldstr(new string(item2));
						Call(StringAsSpanMethod);
						Call(SpanIndexOfAnySpanMethod);
						break;
					}
				}
			}
			Stloc(rentedLocalBuilder2);
			Ldloc(rentedLocalBuilder2);
			Ldc(0);
			BltFar(l3);
			using RentedLocalBuilder rentedLocalBuilder3 = RentInt32Local();
			Ldloc(rentedLocalBuilder2);
			Stloc(rentedLocalBuilder3);
			Label l4 = DefineLabel();
			Label l5 = DefineLabel();
			MarkLabel(l4);
			Ldloc(rentedLocalBuilder3);
			Ldc(1);
			Sub();
			Stloc(rentedLocalBuilder3);
			Ldloc(rentedLocalBuilder3);
			Ldloca(rentedLocalBuilder);
			Call(SpanGetLengthMethod);
			BgeUn(l5);
			Ldloca(rentedLocalBuilder);
			Ldloc(rentedLocalBuilder3);
			Call(SpanGetItemMethod);
			LdindU2();
			EmitMatchCharacterClass(value.Item1.Str);
			BrtrueFar(l4);
			MarkLabel(l5);
			if (value.Item1.M > 0)
			{
				Label l6 = DefineLabel();
				Ldloc(rentedLocalBuilder2);
				Ldloc(rentedLocalBuilder3);
				Sub();
				Ldc(1);
				Sub();
				Ldc(value.Item1.M);
				Bge(l6);
				Ldloc(pos);
				Ldloc(rentedLocalBuilder2);
				Add();
				Ldc(1);
				Add();
				Stloc(pos);
				BrFar(l2);
				MarkLabel(l6);
			}
			Ldthis();
			Ldloc(pos);
			Ldloc(rentedLocalBuilder3);
			Add();
			Ldc(1);
			Add();
			Stfld(RuntextposField);
			Ldthis();
			Ldloc(pos);
			Ldloc(rentedLocalBuilder2);
			Add();
			Stfld(RuntrackposField);
			Ldc(1);
			Ret();
			MarkLabel(l3);
			BrFar(returnFalse);
		}
	}

	protected void EmitTryMatchAtCurrentPosition()
	{
		_int32LocalsPool?.Clear();
		_readOnlySpanCharLocalsPool?.Clear();
		RegexNode root = _regexTree.Root;
		root = root.Child(0);
		RegexNodeKind kind = root.Kind;
		if (kind - 9 <= RegexNodeKind.Oneloop)
		{
			int num = ((root.Kind != RegexNodeKind.Multi) ? 1 : root.Str.Length);
			if ((root.Options & RegexOptions.RightToLeft) != RegexOptions.None)
			{
				num = -num;
			}
			Ldthis();
			Dup();
			Ldc(0);
			Ldthisfld(RuntextposField);
			Dup();
			Ldc(num);
			Add();
			Call(CaptureMethod);
			Ldthisfld(RuntextposField);
			Ldc(num);
			Add();
			Stfld(RuntextposField);
			Ldc(1);
			Ret();
			return;
		}
		AnalysisResults analysis = RegexTreeAnalyzer.Analyze(_regexTree);
		LocalBuilder inputSpan = DeclareReadOnlySpanChar();
		LocalBuilder lt = DeclareInt32();
		LocalBuilder pos = DeclareInt32();
		LocalBuilder slice = DeclareReadOnlySpanChar();
		Label doneLabel = DefineLabel();
		Label l = doneLabel;
		Ldarg_1();
		Stloc(inputSpan);
		Ldthisfld(RuntextposField);
		Stloc(pos);
		Ldloc(pos);
		Stloc(lt);
		LocalBuilder stackpos = DeclareInt32();
		Ldc(0);
		Stloc(stackpos);
		int sliceStaticPos = 0;
		SliceInputSpan();
		bool expressionHasCaptures = analysis.MayContainCapture(root);
		EmitNode(root);
		Ldthis();
		Ldloc(pos);
		if (sliceStaticPos > 0)
		{
			Ldc(sliceStaticPos);
			Add();
			Stloc(pos);
			Ldloc(pos);
		}
		Stfld(RuntextposField);
		Ldthis();
		Ldc(0);
		Ldloc(lt);
		Ldloc(pos);
		Call(CaptureMethod);
		Ldc(1);
		Ret();
		if (expressionHasCaptures)
		{
			Label l2 = DefineLabel();
			Br(l2);
			MarkLabel(l);
			Label l3 = DefineLabel();
			Label l4 = DefineLabel();
			Br(l3);
			MarkLabel(l4);
			Ldthis();
			Call(UncaptureMethod);
			MarkLabel(l3);
			Ldthis();
			Call(CrawlposMethod);
			Brtrue(l4);
			MarkLabel(l2);
		}
		else
		{
			MarkLabel(l);
		}
		Ldc(0);
		Ret();
		static bool CanEmitIndexOf(RegexNode node, out int literalLength)
		{
			if (node.Kind == RegexNodeKind.Multi)
			{
				literalLength = node.Str.Length;
				return true;
			}
			if (node.IsOneFamily || node.IsNotoneFamily)
			{
				literalLength = 1;
				return true;
			}
			if (node.IsSetFamily)
			{
				Span<char> chars = stackalloc char[128];
				if (RegexCharClass.TryGetSingleRange(node.Str, out var _, out var _) || RegexCharClass.GetSetChars(node.Str, chars) > 0)
				{
					literalLength = 1;
					return true;
				}
			}
			literalLength = 0;
			return false;
		}
		void EmitAlternation(RegexNode node)
		{
			int num2 = node.ChildCount();
			Label label = doneLabel;
			bool flag = analysis.IsAtomicByAncestor(node);
			Label l5 = DefineLabel();
			LocalBuilder startingPos = DeclareInt32();
			Ldloc(pos);
			Stloc(startingPos);
			int num3 = sliceStaticPos;
			LocalBuilder startingCapturePos = null;
			if (expressionHasCaptures && (analysis.MayContainCapture(node) || !flag))
			{
				startingCapturePos = DeclareInt32();
				Ldthis();
				Call(CrawlposMethod);
				Stloc(startingCapturePos);
			}
			bool num4 = !flag && !analysis.IsInLoop(node);
			Label[] array = new Label[num2];
			Label label2 = DefineLabel();
			LocalBuilder localBuilder = (num4 ? DeclareInt32() : null);
			int i;
			for (i = 0; i < num2; i++)
			{
				bool num5 = i == num2 - 1;
				Label l6 = default(Label);
				if (!num5)
				{
					l6 = (doneLabel = DefineLabel());
				}
				else
				{
					doneLabel = label;
				}
				EmitNode(node.Child(i));
				if (!flag)
				{
					if (localBuilder == null)
					{
						EmitStackResizeIfNeeded(2 + ((startingCapturePos != null) ? 1 : 0));
						EmitStackPush(delegate
						{
							Ldc(i);
						});
						if (startingCapturePos != null)
						{
							EmitStackPush(delegate
							{
								Ldloc(startingCapturePos);
							});
						}
						EmitStackPush(delegate
						{
							Ldloc(startingPos);
						});
					}
					else
					{
						Ldc(i);
						Stloc(localBuilder);
					}
				}
				array[i] = doneLabel;
				TransferSliceStaticPosToPos();
				BrFar(l5);
				if (!num5)
				{
					MarkLabel(l6);
					Ldloc(startingPos);
					Stloc(pos);
					SliceInputSpan();
					sliceStaticPos = num3;
					if (startingCapturePos != null)
					{
						EmitUncaptureUntil(startingCapturePos);
					}
				}
			}
			if (flag)
			{
				doneLabel = label;
			}
			else
			{
				doneLabel = label2;
				MarkLabel(label2);
				EmitTimeoutCheckIfNeeded();
				if (localBuilder == null)
				{
					EmitStackPop();
					Stloc(startingPos);
					if (startingCapturePos != null)
					{
						EmitStackPop();
						Stloc(startingCapturePos);
					}
					EmitStackPop();
				}
				else
				{
					Ldloc(localBuilder);
				}
				Switch(array);
			}
			MarkLabel(l5);
		}
		void EmitAnchors(RegexNode node)
		{
			switch (node.Kind)
			{
			default:
				return;
			case RegexNodeKind.Beginning:
			case RegexNodeKind.Start:
				if (sliceStaticPos > 0)
				{
					BrFar(doneLabel);
				}
				else
				{
					Ldloc(pos);
					if (node.Kind == RegexNodeKind.Beginning)
					{
						Ldc(0);
					}
					else
					{
						Ldthisfld(RuntextstartField);
					}
					BneFar(doneLabel);
				}
				return;
			case RegexNodeKind.Bol:
				if (sliceStaticPos > 0)
				{
					Ldloca(slice);
					Ldc(sliceStaticPos - 1);
					Call(SpanGetItemMethod);
					LdindU2();
					Ldc(10);
					BneFar(doneLabel);
				}
				else
				{
					Label l5 = DefineLabel();
					Ldloc(pos);
					Ldc(0);
					Ble(l5);
					Ldloca(inputSpan);
					Ldloc(pos);
					Ldc(1);
					Sub();
					Call(SpanGetItemMethod);
					LdindU2();
					Ldc(10);
					BneFar(doneLabel);
					MarkLabel(l5);
				}
				return;
			case RegexNodeKind.End:
				if (sliceStaticPos > 0)
				{
					Ldc(sliceStaticPos);
					Ldloca(slice);
				}
				else
				{
					Ldloc(pos);
					Ldloca(inputSpan);
				}
				Call(SpanGetLengthMethod);
				BltUnFar(doneLabel);
				return;
			case RegexNodeKind.EndZ:
				if (sliceStaticPos > 0)
				{
					Ldc(sliceStaticPos);
					Ldloca(slice);
				}
				else
				{
					Ldloc(pos);
					Ldloca(inputSpan);
				}
				Call(SpanGetLengthMethod);
				Ldc(1);
				Sub();
				BltFar(doneLabel);
				break;
			case RegexNodeKind.Eol:
				break;
			case RegexNodeKind.Boundary:
			case RegexNodeKind.NonBoundary:
				return;
			}
			if (sliceStaticPos > 0)
			{
				Label l6 = DefineLabel();
				Ldc(sliceStaticPos);
				Ldloca(slice);
				Call(SpanGetLengthMethod);
				BgeUn(l6);
				Ldloca(slice);
				Ldc(sliceStaticPos);
				Call(SpanGetItemMethod);
				LdindU2();
				Ldc(10);
				BneFar(doneLabel);
				MarkLabel(l6);
			}
			else
			{
				Label l7 = DefineLabel();
				Ldloc(pos);
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				BgeUn(l7);
				Ldloca(inputSpan);
				Ldloc(pos);
				Call(SpanGetItemMethod);
				LdindU2();
				Ldc(10);
				BneFar(doneLabel);
				MarkLabel(l7);
			}
		}
		void EmitAtomic(RegexNode node, RegexNode subsequent)
		{
			RegexNode node2 = node.Child(0);
			if (!analysis.MayBacktrack(node2))
			{
				EmitNode(node2, subsequent);
				return;
			}
			Label label = doneLabel;
			using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
			Ldloc(stackpos);
			Stloc(rentedLocalBuilder);
			EmitNode(node2, subsequent);
			Ldloc(rentedLocalBuilder);
			Stloc(stackpos);
			doneLabel = label;
		}
		void EmitAtomicSingleCharZeroOrOne(RegexNode node)
		{
			bool num2 = (node.Options & RegexOptions.RightToLeft) != 0;
			if (num2)
			{
				TransferSliceStaticPosToPos();
			}
			Label l5 = DefineLabel();
			if (!num2)
			{
				Ldc(sliceStaticPos);
				Ldloca(slice);
				Call(SpanGetLengthMethod);
				BgeUnFar(l5);
			}
			else
			{
				Ldloc(pos);
				Ldc(0);
				BeqFar(l5);
			}
			if (!num2)
			{
				Ldloca(slice);
				Ldc(sliceStaticPos);
			}
			else
			{
				Ldloca(inputSpan);
				Ldloc(pos);
				Ldc(1);
				Sub();
			}
			Call(SpanGetItemMethod);
			LdindU2();
			if (node.IsSetFamily)
			{
				EmitMatchCharacterClass(node.Str);
				BrfalseFar(l5);
			}
			else
			{
				Ldc(node.Ch);
				if (node.IsOneFamily)
				{
					BneFar(l5);
				}
				else
				{
					BeqFar(l5);
				}
			}
			if (!num2)
			{
				Ldloca(slice);
				Ldc(1);
				Call(SpanSliceIntMethod);
				Stloc(slice);
				Ldloc(pos);
				Ldc(1);
				Add();
				Stloc(pos);
			}
			else
			{
				Ldloc(pos);
				Ldc(1);
				Sub();
				Stloc(pos);
			}
			MarkLabel(l5);
		}
		void EmitBackreference(RegexNode node)
		{
			int i = RegexParser.MapCaptureNumber(node.M, _regexTree.CaptureNumberSparseMapping);
			bool flag = (node.Options & RegexOptions.RightToLeft) != 0;
			TransferSliceStaticPosToPos();
			Label label = DefineLabel();
			Ldthis();
			Ldc(i);
			Call(IsMatchedMethod);
			BrfalseFar(((node.Options & RegexOptions.ECMAScript) == 0) ? doneLabel : label);
			using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
			using RentedLocalBuilder rentedLocalBuilder2 = RentInt32Local();
			using RentedLocalBuilder rentedLocalBuilder3 = RentInt32Local();
			Ldthis();
			Ldc(i);
			Call(MatchLengthMethod);
			Stloc(rentedLocalBuilder);
			if (!flag)
			{
				Ldloca(slice);
				Call(SpanGetLengthMethod);
			}
			else
			{
				Ldloc(pos);
			}
			Ldloc(rentedLocalBuilder);
			BltFar(doneLabel);
			Ldthis();
			Ldc(i);
			Call(MatchIndexMethod);
			Stloc(rentedLocalBuilder2);
			Label l5 = DefineLabel();
			Label l6 = DefineLabel();
			Label l7 = DefineLabel();
			LocalBuilder lt2 = _ilg.DeclareLocal(typeof(char));
			LocalBuilder lt3 = _ilg.DeclareLocal(typeof(char));
			Ldc(0);
			Stloc(rentedLocalBuilder3);
			Br(l5);
			MarkLabel(l6);
			Ldloca(inputSpan);
			Ldloc(rentedLocalBuilder2);
			Ldloc(rentedLocalBuilder3);
			Add();
			Call(SpanGetItemMethod);
			LdindU2();
			Stloc(lt2);
			if (!flag)
			{
				Ldloca(slice);
				Ldloc(rentedLocalBuilder3);
			}
			else
			{
				Ldloca(inputSpan);
				Ldloc(pos);
				Ldloc(rentedLocalBuilder);
				Sub();
				Ldloc(rentedLocalBuilder3);
				Add();
			}
			Call(SpanGetItemMethod);
			LdindU2();
			Stloc(lt3);
			if ((node.Options & RegexOptions.IgnoreCase) != RegexOptions.None)
			{
				LocalBuilder lt4 = DeclareReadOnlySpanChar();
				Ldloc(lt2);
				Ldloc(lt3);
				Ceq();
				BrtrueFar(l7);
				Ldloc(lt2);
				Ldthisfld(CultureField);
				Ldthisflda(CaseBehaviorField);
				Ldloca(lt4);
				Call(RegexCaseEquivalencesTryFindCaseEquivalencesForCharWithIBehaviorMethod);
				BrfalseFar(doneLabel);
				Ldloc(lt4);
				if (!flag)
				{
					Ldloca(slice);
					Ldloc(rentedLocalBuilder3);
				}
				else
				{
					Ldloca(inputSpan);
					Ldloc(pos);
					Ldloc(rentedLocalBuilder);
					Sub();
					Ldloc(rentedLocalBuilder3);
					Add();
				}
				Call(SpanGetItemMethod);
				LdindU2();
				Call(SpanIndexOfCharMethod);
				Ldc(0);
				BltFar(doneLabel);
			}
			else
			{
				Ldloc(lt2);
				Ldloc(lt3);
				Ceq();
				BrfalseFar(doneLabel);
			}
			MarkLabel(l7);
			Ldloc(rentedLocalBuilder3);
			Ldc(1);
			Add();
			Stloc(rentedLocalBuilder3);
			MarkLabel(l5);
			Ldloc(rentedLocalBuilder3);
			Ldloc(rentedLocalBuilder);
			Blt(l6);
			Ldloc(pos);
			Ldloc(rentedLocalBuilder);
			if (!flag)
			{
				Add();
			}
			else
			{
				Sub();
			}
			Stloc(pos);
			if (!flag)
			{
				SliceInputSpan();
			}
			MarkLabel(label);
		}
		void EmitBackreferenceConditional(RegexNode node)
		{
			bool flag = analysis.IsAtomicByAncestor(node);
			TransferSliceStaticPosToPos();
			int i = RegexParser.MapCaptureNumber(node.M, _regexTree.CaptureNumberSparseMapping);
			RegexNode node2 = node.Child(0);
			RegexNode regexNode = node.Child(1);
			RegexNode regexNode2 = ((regexNode != null && regexNode.Kind != RegexNodeKind.Empty) ? regexNode : null);
			Label label = doneLabel;
			Label l5 = DefineLabel();
			Label l6 = DefineLabel();
			LocalBuilder resumeAt = DeclareInt32();
			bool flag2 = analysis.IsInLoop(node);
			Ldthis();
			Ldc(i);
			Call(IsMatchedMethod);
			BrfalseFar(l5);
			EmitNode(node2);
			TransferSliceStaticPosToPos();
			Label label2 = doneLabel;
			if ((!flag && label2 != label) | flag2)
			{
				Ldc(0);
				Stloc(resumeAt);
			}
			bool flag3 = label2 != label || regexNode2 != null;
			if (flag3)
			{
				BrFar(l6);
			}
			MarkLabel(l5);
			Label label3 = label;
			if (regexNode2 != null)
			{
				doneLabel = label;
				EmitNode(regexNode2);
				TransferSliceStaticPosToPos();
				label3 = doneLabel;
				if ((!flag && label3 != label) | flag2)
				{
					Ldc(1);
					Stloc(resumeAt);
				}
			}
			else if ((!flag && label2 != label) | flag2)
			{
				Ldc(2);
				Stloc(resumeAt);
			}
			if (flag || (label2 == label && label3 == label))
			{
				doneLabel = label;
				if (flag3)
				{
					MarkLabel(l6);
				}
			}
			else
			{
				Br(l6);
				Label l7 = (doneLabel = DefineLabel());
				MarkLabel(l7);
				if (flag2)
				{
					EmitStackPop();
					Stloc(resumeAt);
				}
				if (label2 != label)
				{
					Ldloc(resumeAt);
					Ldc(0);
					BeqFar(label2);
				}
				if (label3 != label)
				{
					Ldloc(resumeAt);
					Ldc(1);
					BeqFar(label3);
				}
				BrFar(label);
				if (flag3)
				{
					MarkLabel(l6);
				}
				if (flag2)
				{
					EmitStackResizeIfNeeded(1);
					EmitStackPush(delegate
					{
						Ldloc(resumeAt);
					});
				}
			}
		}
		void EmitBoundary(RegexNode node)
		{
			if ((node.Options & RegexOptions.RightToLeft) != RegexOptions.None)
			{
				TransferSliceStaticPosToPos();
			}
			Ldloc(inputSpan);
			Ldloc(pos);
			if (sliceStaticPos > 0)
			{
				Ldc(sliceStaticPos);
				Add();
			}
			RegexNodeKind kind2 = node.Kind;
			if (kind2 - 16 <= (RegexNodeKind)1)
			{
				if (node.IsKnownPrecededByWordChar())
				{
					Call(IsPostWordCharBoundaryMethod);
				}
				else if (node.IsKnownSucceededByWordChar())
				{
					Call(IsPreWordCharBoundaryMethod);
				}
				else
				{
					Call(IsBoundaryMethod);
				}
				if (node.Kind == RegexNodeKind.Boundary)
				{
					BrfalseFar(doneLabel);
				}
				else
				{
					BrtrueFar(doneLabel);
				}
			}
			else
			{
				Call(IsECMABoundaryMethod);
				if (node.Kind == RegexNodeKind.ECMABoundary)
				{
					BrfalseFar(doneLabel);
				}
				else
				{
					BrtrueFar(doneLabel);
				}
			}
		}
		void EmitCapture(RegexNode node, RegexNode subsequent = null)
		{
			int i = RegexParser.MapCaptureNumber(node.M, _regexTree.CaptureNumberSparseMapping);
			int num2 = RegexParser.MapCaptureNumber(node.N, _regexTree.CaptureNumberSparseMapping);
			bool num3 = analysis.IsAtomicByAncestor(node);
			bool flag = analysis.IsInLoop(node);
			TransferSliceStaticPosToPos();
			LocalBuilder startingPos = DeclareInt32();
			Ldloc(pos);
			Stloc(startingPos);
			RegexNode node2 = node.Child(0);
			Label label = doneLabel;
			EmitNode(node2, subsequent);
			bool flag2 = doneLabel != label;
			TransferSliceStaticPosToPos();
			if (num2 == -1)
			{
				Ldthis();
				Ldc(i);
				Ldloc(startingPos);
				Ldloc(pos);
				Call(CaptureMethod);
			}
			else
			{
				Ldthis();
				Ldc(num2);
				Call(IsMatchedMethod);
				BrfalseFar(doneLabel);
				Ldthis();
				Ldc(i);
				Ldc(num2);
				Ldloc(startingPos);
				Ldloc(pos);
				Call(TransferCaptureMethod);
			}
			if (num3 || !flag2)
			{
				doneLabel = label;
			}
			else
			{
				if (flag)
				{
					EmitStackResizeIfNeeded(1);
					EmitStackPush(delegate
					{
						Ldloc(startingPos);
					});
				}
				Label l5 = DefineLabel();
				Br(l5);
				Label label2 = DefineLabel();
				MarkLabel(label2);
				if (flag)
				{
					EmitStackPop();
					Stloc(startingPos);
				}
				BrFar(doneLabel);
				doneLabel = label2;
				MarkLabel(l5);
			}
		}
		void EmitConcatenation(RegexNode node, RegexNode subsequent, bool emitLengthChecksIfRequired)
		{
			int num2 = node.ChildCount();
			for (int i = 0; i < num2; i++)
			{
				if ((((node.Options & RegexOptions.RightToLeft) == 0) & emitLengthChecksIfRequired) && node.TryGetJoinableLengthCheckChildRange(i, out var requiredLength, out var exclusiveEnd))
				{
					EmitSpanLengthCheck(requiredLength);
					for (; i < exclusiveEnd; i++)
					{
						if (node.TryGetOrdinalCaseInsensitiveString(i, exclusiveEnd, out var nodesConsumed, out var caseInsensitiveString))
						{
							if (sliceStaticPos > 0)
							{
								Ldloca(slice);
								Ldc(sliceStaticPos);
								Call(SpanSliceIntMethod);
							}
							else
							{
								Ldloc(slice);
							}
							Ldstr(caseInsensitiveString);
							Call(StringAsSpanMethod);
							Ldc(5);
							Call(SpanStartsWithSpanComparisonMethod);
							BrfalseFar(doneLabel);
							sliceStaticPos += caseInsensitiveString.Length;
							i += nodesConsumed - 1;
						}
						else
						{
							EmitNode(node.Child(i), GetSubsequent(i, node, subsequent), emitLengthChecksIfRequired: false);
						}
					}
					i--;
				}
				else
				{
					EmitNode(node.Child(i), GetSubsequent(i, node, subsequent));
				}
			}
		}
		void EmitExpressionConditional(RegexNode node)
		{
			bool flag = analysis.IsAtomicByAncestor(node);
			TransferSliceStaticPosToPos();
			RegexNode node2 = node.Child(0);
			RegexNode node3 = node.Child(1);
			RegexNode regexNode = node.Child(2);
			RegexNode regexNode2 = ((regexNode != null && regexNode.Kind != RegexNodeKind.Empty) ? regexNode : null);
			Label label = doneLabel;
			Label label2 = DefineLabel();
			Label l5 = DefineLabel();
			bool flag2 = false;
			LocalBuilder resumeAt = null;
			if (!flag)
			{
				flag2 = analysis.IsInLoop(node);
				resumeAt = DeclareInt32();
			}
			LocalBuilder localBuilder = null;
			if (analysis.MayContainCapture(node2))
			{
				localBuilder = DeclareInt32();
				Ldthis();
				Call(CrawlposMethod);
				Stloc(localBuilder);
			}
			doneLabel = label2;
			LocalBuilder lt2 = DeclareInt32();
			Ldloc(pos);
			Stloc(lt2);
			int num2 = sliceStaticPos;
			if (analysis.MayBacktrack(node2))
			{
				EmitAtomic(node, null);
			}
			else
			{
				EmitNode(node2);
			}
			doneLabel = label;
			Ldloc(lt2);
			Stloc(pos);
			SliceInputSpan();
			sliceStaticPos = num2;
			EmitNode(node3);
			TransferSliceStaticPosToPos();
			Label label3 = doneLabel;
			if (!flag && label3 != label)
			{
				Ldc(0);
				Stloc(resumeAt);
			}
			BrFar(l5);
			MarkLabel(label2);
			Ldloc(lt2);
			Stloc(pos);
			SliceInputSpan();
			sliceStaticPos = num2;
			if (localBuilder != null)
			{
				EmitUncaptureUntil(localBuilder);
			}
			Label label4 = label;
			if (regexNode2 != null)
			{
				doneLabel = label;
				EmitNode(regexNode2);
				TransferSliceStaticPosToPos();
				label4 = doneLabel;
				if (!flag && label4 != label)
				{
					Ldc(1);
					Stloc(resumeAt);
				}
			}
			else if (!flag && label3 != label)
			{
				Ldc(2);
				Stloc(resumeAt);
			}
			if (flag || (label3 == label && label4 == label))
			{
				doneLabel = label;
				MarkLabel(l5);
			}
			else
			{
				BrFar(l5);
				Label l6 = (doneLabel = DefineLabel());
				MarkLabel(l6);
				if (flag2)
				{
					EmitStackPop();
					Stloc(resumeAt);
				}
				if (label3 != label)
				{
					Ldloc(resumeAt);
					Ldc(0);
					BeqFar(label3);
				}
				if (label4 != label)
				{
					Ldloc(resumeAt);
					Ldc(1);
					BeqFar(label4);
				}
				BrFar(label);
				MarkLabel(l5);
				if (flag2)
				{
					EmitStackResizeIfNeeded(1);
					EmitStackPush(delegate
					{
						Ldloc(resumeAt);
					});
				}
			}
		}
		void EmitIndexOf(RegexNode node, bool useLast, bool negate)
		{
			if (node.Kind == RegexNodeKind.Multi)
			{
				Ldstr(node.Str);
				Call(StringAsSpanMethod);
				Call(useLast ? SpanLastIndexOfSpanMethod : SpanIndexOfSpanMethod);
			}
			else if (node.IsOneFamily || node.IsNotoneFamily)
			{
				if (node.IsNotoneFamily)
				{
					negate = !negate;
				}
				Ldc(node.Ch);
				MethodInfo mt = ((!useLast) ? (negate ? SpanIndexOfAnyExceptCharMethod : SpanIndexOfCharMethod) : (negate ? SpanLastIndexOfAnyExceptCharMethod : SpanLastIndexOfCharMethod));
				Call(mt);
			}
			else if (node.IsSetFamily)
			{
				bool flag = RegexCharClass.IsNegated(node.Str) ^ negate;
				if (RegexCharClass.TryGetSingleRange(node.Str, out var lowInclusive, out var highInclusive) && highInclusive - lowInclusive > 1)
				{
					Ldc(lowInclusive);
					Ldc(highInclusive);
					MethodInfo mt = ((!useLast) ? (flag ? SpanIndexOfAnyExceptInRangeMethod : SpanIndexOfAnyInRangeMethod) : (flag ? SpanLastIndexOfAnyExceptInRangeMethod : SpanLastIndexOfAnyInRangeMethod));
					Call(mt);
				}
				else
				{
					Span<char> span = stackalloc char[128];
					int setChars = RegexCharClass.GetSetChars(node.Str, span);
					if (setChars > 0)
					{
						span = span.Slice(0, setChars);
						switch (span.Length)
						{
						case 1:
						{
							Ldc(span[0]);
							MethodInfo mt = ((!useLast) ? (flag ? SpanIndexOfAnyExceptCharMethod : SpanIndexOfCharMethod) : (flag ? SpanLastIndexOfAnyExceptCharMethod : SpanLastIndexOfCharMethod));
							Call(mt);
							break;
						}
						case 2:
						{
							Ldc(span[0]);
							Ldc(span[1]);
							MethodInfo mt = ((!useLast) ? (flag ? SpanIndexOfAnyExceptCharCharMethod : SpanIndexOfAnyCharCharMethod) : (flag ? SpanLastIndexOfAnyExceptCharCharMethod : SpanLastIndexOfAnyCharCharMethod));
							Call(mt);
							break;
						}
						case 3:
						{
							Ldc(span[0]);
							Ldc(span[1]);
							Ldc(span[2]);
							MethodInfo mt = ((!useLast) ? (flag ? SpanIndexOfAnyExceptCharCharCharMethod : SpanIndexOfAnyCharCharCharMethod) : (flag ? SpanLastIndexOfAnyExceptCharCharCharMethod : SpanLastIndexOfAnyCharCharCharMethod));
							Call(mt);
							break;
						}
						default:
							EmitIndexOfAnyWithSearchValuesOrLiteral(span, useLast, flag);
							break;
						}
					}
				}
			}
		}
		void EmitLazy(RegexNode node)
		{
			RegexNode regexNode = node.Child(0);
			int m = node.M;
			int n = node.N;
			Label label = doneLabel;
			if (m == n)
			{
				EmitLoop(node);
			}
			else
			{
				TransferSliceStaticPosToPos();
				Label l5 = DefineLabel();
				Label l6 = DefineLabel();
				LocalBuilder iterationCount = DeclareInt32();
				Ldc(0);
				Stloc(iterationCount);
				bool flag = regexNode.ComputeMinLength() == 0;
				LocalBuilder startingPos = null;
				LocalBuilder sawEmpty = null;
				if (flag)
				{
					startingPos = DeclareInt32();
					Ldloc(pos);
					Stloc(startingPos);
					sawEmpty = DeclareInt32();
					Ldc(0);
					Stloc(sawEmpty);
				}
				if (m == 0)
				{
					BrFar(l6);
				}
				MarkLabel(l5);
				int num2 = 1 + (flag ? 2 : 0) + (expressionHasCaptures ? 1 : 0);
				EmitStackResizeIfNeeded(num2);
				EmitStackPush(delegate
				{
					Ldloc(pos);
				});
				if (flag)
				{
					EmitStackPush(delegate
					{
						Ldloc(startingPos);
					});
					EmitStackPush(delegate
					{
						Ldloc(sawEmpty);
					});
				}
				if (expressionHasCaptures)
				{
					EmitStackPush(delegate
					{
						Ldthis();
						Call(CrawlposMethod);
					});
				}
				if (flag)
				{
					Ldloc(pos);
					Stloc(startingPos);
				}
				Ldloc(iterationCount);
				Ldc(1);
				Add();
				Stloc(iterationCount);
				Label label2 = (doneLabel = DefineLabel());
				EmitNode(regexNode);
				TransferSliceStaticPosToPos();
				if (doneLabel == label2)
				{
					doneLabel = label;
				}
				if (m >= 2)
				{
					Ldloc(iterationCount);
					Ldc(m);
					BltFar(l5);
				}
				if (flag)
				{
					Label l7 = DefineLabel();
					Ldloc(pos);
					Ldloc(startingPos);
					Bne(l7);
					Ldc(1);
					Stloc(sawEmpty);
					MarkLabel(l7);
				}
				BrFar(l6);
				MarkLabel(label2);
				Ldloc(iterationCount);
				Ldc(1);
				Sub();
				Stloc(iterationCount);
				EmitUncaptureUntilPopped();
				if (flag)
				{
					EmitStackPop();
					Stloc(sawEmpty);
					EmitStackPop();
					Stloc(startingPos);
				}
				EmitStackPop();
				Stloc(pos);
				SliceInputSpan();
				if (doneLabel == label)
				{
					Ldloc(stackpos);
					Ldloc(iterationCount);
					if (num2 > 1)
					{
						Ldc(num2);
						Mul();
					}
					Sub();
					Stloc(stackpos);
					BrFar(label);
				}
				else
				{
					Ldloc(iterationCount);
					Ldc(0);
					BeqFar(label);
					if (flag)
					{
						Ldc(0);
						Stloc(sawEmpty);
					}
					BrFar(doneLabel);
				}
				MarkLabel(l6);
				bool flag2 = analysis.IsInLoop(node);
				EmitStackResizeIfNeeded(1 + (flag2 ? (1 + (flag ? 2 : 0)) : 0) + (expressionHasCaptures ? 1 : 0));
				EmitStackPush(delegate
				{
					Ldloc(pos);
				});
				if (flag2)
				{
					EmitStackPush(delegate
					{
						Ldloc(iterationCount);
					});
					if (flag)
					{
						EmitStackPush(delegate
						{
							Ldloc(startingPos);
						});
						EmitStackPush(delegate
						{
							Ldloc(sawEmpty);
						});
					}
				}
				if (expressionHasCaptures)
				{
					EmitStackPush(delegate
					{
						Ldthis();
						Call(CrawlposMethod);
					});
				}
				Label l8 = DefineLabel();
				BrFar(l8);
				Label label3 = DefineLabel();
				MarkLabel(label3);
				EmitTimeoutCheckIfNeeded();
				EmitUncaptureUntilPopped();
				if (flag2)
				{
					if (flag)
					{
						EmitStackPop();
						Stloc(sawEmpty);
						EmitStackPop();
						Stloc(startingPos);
					}
					EmitStackPop();
					Stloc(iterationCount);
				}
				EmitStackPop();
				Stloc(pos);
				SliceInputSpan();
				Label l9 = DefineLabel();
				if (flag)
				{
					Label l10 = DefineLabel();
					Ldloc(sawEmpty);
					Ldc(0);
					Beq(l10);
					Ldc(0);
					Stloc(sawEmpty);
					Br(l9);
					MarkLabel(l10);
				}
				if (n != int.MaxValue)
				{
					Ldloc(iterationCount);
					Ldc(n);
					Bge(l9);
				}
				BrFar(l5);
				MarkLabel(l9);
				if (doneLabel == label)
				{
					Ldloc(stackpos);
					Ldc(num2);
					Sub();
					Stloc(stackpos);
				}
				BrFar(doneLabel);
				doneLabel = label3;
				MarkLabel(l8);
			}
		}
		void EmitLoop(RegexNode node)
		{
			RegexNode regexNode = node.Child(0);
			int m = node.M;
			int n = node.N;
			if (m == n)
			{
				int num2 = m;
				if (num2 <= 1)
				{
					switch (num2)
					{
					case 0:
						return;
					case 1:
						EmitNode(regexNode);
						return;
					}
				}
				else if (!analysis.MayBacktrack(regexNode))
				{
					EmitNonBacktrackingRepeater(node);
					return;
				}
			}
			TransferSliceStaticPosToPos();
			bool num3 = analysis.IsAtomicByAncestor(node);
			LocalBuilder startingStackpos = null;
			if (num3 || m > 1)
			{
				startingStackpos = DeclareInt32();
				Ldloc(stackpos);
				Stloc(startingStackpos);
			}
			Label label = doneLabel;
			Label l5 = DefineLabel();
			Label l6 = DefineLabel();
			LocalBuilder iterationCount = DeclareInt32();
			bool flag = regexNode.ComputeMinLength() == 0;
			LocalBuilder startingPos = (flag ? DeclareInt32() : null);
			Ldc(0);
			Stloc(iterationCount);
			if (startingPos != null)
			{
				Ldc(0);
				Stloc(startingPos);
			}
			MarkLabel(l5);
			EmitStackResizeIfNeeded(1 + (expressionHasCaptures ? 1 : 0) + ((startingPos != null) ? 1 : 0));
			if (expressionHasCaptures)
			{
				EmitStackPush(delegate
				{
					Ldthis();
					Call(CrawlposMethod);
				});
			}
			if (startingPos != null)
			{
				EmitStackPush(delegate
				{
					Ldloc(startingPos);
				});
			}
			EmitStackPush(delegate
			{
				Ldloc(pos);
			});
			if (startingPos != null)
			{
				Ldloc(pos);
				Stloc(startingPos);
			}
			Ldloc(iterationCount);
			Ldc(1);
			Add();
			Stloc(iterationCount);
			Label label2 = (doneLabel = DefineLabel());
			EmitNode(regexNode);
			TransferSliceStaticPosToPos();
			bool flag2 = doneLabel != label2;
			bool num4 = m > 0;
			bool flag3 = n == int.MaxValue;
			if (num4)
			{
				if (flag3)
				{
					if (!flag)
					{
						goto IL_0473;
					}
					Ldloc(pos);
					Ldloc(startingPos);
					BneFar(l5);
					Ldloc(iterationCount);
					Ldc(m);
					BltFar(l5);
					BrFar(l6);
				}
				else
				{
					if (!flag)
					{
						goto IL_043a;
					}
					Ldloc(iterationCount);
					Ldc(n);
					BgeFar(l6);
					Ldloc(pos);
					Ldloc(startingPos);
					BneFar(l5);
					Ldloc(iterationCount);
					Ldc(m);
					BltFar(l5);
					BrFar(l6);
				}
			}
			else if (flag3)
			{
				if (!flag)
				{
					goto IL_0473;
				}
				Ldloc(pos);
				Ldloc(startingPos);
				BneFar(l5);
				BrFar(l6);
			}
			else
			{
				if (!flag)
				{
					goto IL_043a;
				}
				Ldloc(pos);
				Ldloc(startingPos);
				BeqFar(l6);
				Ldloc(iterationCount);
				Ldc(n);
				BgeFar(l6);
				BrFar(l5);
			}
			goto IL_0480;
			IL_0480:
			MarkLabel(label2);
			Ldloc(iterationCount);
			Ldc(1);
			Sub();
			Stloc(iterationCount);
			Ldloc(iterationCount);
			Ldc(0);
			BltFar(label);
			EmitStackPop();
			Stloc(pos);
			SliceInputSpan();
			if (startingPos != null)
			{
				EmitStackPop();
				Stloc(startingPos);
			}
			EmitUncaptureUntilPopped();
			if (m > 0)
			{
				if (flag2)
				{
					Ldloc(iterationCount);
					Ldc(0);
					BeqFar(label);
					if (m > 1)
					{
						Ldloc(iterationCount);
						Ldc(m);
						BltFar(doneLabel);
					}
				}
				else
				{
					Label l7 = DefineLabel();
					Ldloc(iterationCount);
					Ldc(m);
					Bge(l7);
					if (m > 1)
					{
						Ldloc(iterationCount);
						Ldc(0);
						BeqFar(label);
						Ldloc(startingStackpos);
						Stloc(stackpos);
					}
					BrFar(label);
					MarkLabel(l7);
				}
			}
			if (num3)
			{
				doneLabel = label;
				MarkLabel(l6);
				if (startingStackpos != null)
				{
					Ldloc(startingStackpos);
					Stloc(stackpos);
				}
			}
			else
			{
				if (flag2)
				{
					BrFar(l6);
					Label label3 = DefineLabel();
					MarkLabel(label3);
					EmitTimeoutCheckIfNeeded();
					Ldloc(iterationCount);
					Ldc(0);
					BeqFar(label);
					BrFar(doneLabel);
					doneLabel = label3;
				}
				MarkLabel(l6);
				if (analysis.IsInLoop(node))
				{
					EmitStackResizeIfNeeded(1 + ((startingPos != null) ? 1 : 0) + ((startingStackpos != null) ? 1 : 0));
					if (startingPos != null)
					{
						EmitStackPush(delegate
						{
							Ldloc(startingPos);
						});
					}
					if (startingStackpos != null)
					{
						EmitStackPush(delegate
						{
							Ldloc(startingStackpos);
						});
					}
					EmitStackPush(delegate
					{
						Ldloc(iterationCount);
					});
					Label l8 = DefineLabel();
					BrFar(l8);
					Label label4 = DefineLabel();
					MarkLabel(label4);
					EmitTimeoutCheckIfNeeded();
					EmitStackPop();
					Stloc(iterationCount);
					if (startingStackpos != null)
					{
						EmitStackPop();
						Stloc(startingStackpos);
					}
					if (startingPos != null)
					{
						EmitStackPop();
						Stloc(startingPos);
					}
					BrFar(doneLabel);
					doneLabel = label4;
					MarkLabel(l8);
				}
			}
			return;
			IL_043a:
			Ldloc(iterationCount);
			Ldc(n);
			BgeFar(l6);
			BrFar(l5);
			goto IL_0480;
			IL_0473:
			BrFar(l5);
			goto IL_0480;
		}
		void EmitMultiChar(RegexNode node, bool emitLengthCheck)
		{
			EmitMultiCharString(node.Str, emitLengthCheck, (node.Options & RegexOptions.RightToLeft) != 0);
		}
		void EmitMultiCharString(string str, bool emitLengthCheck, bool rightToLeft)
		{
			if (rightToLeft)
			{
				TransferSliceStaticPosToPos();
				Ldloc(pos);
				Ldc(str.Length);
				Sub();
				Ldloca(inputSpan);
				Call(SpanGetLengthMethod);
				BgeUnFar(doneLabel);
				for (int num2 = str.Length - 1; num2 >= 0; num2--)
				{
					Ldloc(pos);
					Ldc(1);
					Sub();
					Stloc(pos);
					Ldloca(inputSpan);
					Ldloc(pos);
					Call(SpanGetItemMethod);
					LdindU2();
					Ldc(str[num2]);
					BneFar(doneLabel);
				}
			}
			else
			{
				Ldloca(slice);
				Ldc(sliceStaticPos);
				Call(SpanSliceIntMethod);
				Ldstr(str);
				Call(StringAsSpanMethod);
				Call(SpanStartsWithSpanMethod);
				BrfalseFar(doneLabel);
				sliceStaticPos += str.Length;
			}
		}
		void EmitNegativeLookaroundAssertion(RegexNode node)
		{
			if (analysis.HasRightToLeft)
			{
				TransferSliceStaticPosToPos(forceSliceReload: true);
			}
			Label label = doneLabel;
			LocalBuilder lt2 = DeclareInt32();
			Ldloc(pos);
			Stloc(lt2);
			int num2 = sliceStaticPos;
			Label label2 = (doneLabel = DefineLabel());
			EmitTimeoutCheckIfNeeded();
			RegexNode node2 = node.Child(0);
			bool flag = false;
			LocalBuilder localBuilder = (analysis.MayContainCapture(node2) ? DeclareInt32() : null);
			if (localBuilder != null)
			{
				flag = analysis.IsInLoop(node);
				if (flag)
				{
					EmitStackResizeIfNeeded(1);
					EmitStackPush(delegate
					{
						Ldthis();
						Call(CrawlposMethod);
					});
				}
				else
				{
					Ldthis();
					Call(CrawlposMethod);
					Stloc(localBuilder);
				}
			}
			if (analysis.MayBacktrack(node2))
			{
				EmitAtomic(node, null);
			}
			else
			{
				EmitNode(node2);
			}
			if ((localBuilder != null) & flag)
			{
				Ldloc(stackpos);
				Ldc(1);
				Sub();
				Stloc(stackpos);
			}
			BrFar(label);
			MarkLabel(label2);
			if (doneLabel == label2)
			{
				doneLabel = label;
			}
			Ldloc(lt2);
			Stloc(pos);
			SliceInputSpan();
			sliceStaticPos = num2;
			if (localBuilder != null)
			{
				if (flag)
				{
					EmitStackPop();
					Stloc(localBuilder);
				}
				EmitUncaptureUntil(localBuilder);
			}
			doneLabel = label;
		}
		void EmitNode(RegexNode node, RegexNode subsequent = null, bool emitLengthChecksIfRequired = true)
		{
			if (_regexTree.FindOptimizations.FindMode == FindNextStartingPositionMode.LiteralAfterLoop_LeftToRight && _regexTree.FindOptimizations.LiteralAfterLoop?.LoopNode == node)
			{
				Mvfldloc(RuntrackposField, pos);
				SliceInputSpan();
			}
			else if (!StackHelper.TryEnsureSufficientExecutionStack())
			{
				StackHelper.CallOnEmptyStack(EmitNode, node, subsequent, emitLengthChecksIfRequired);
			}
			else
			{
				if ((node.Options & RegexOptions.RightToLeft) != RegexOptions.None)
				{
					TransferSliceStaticPosToPos();
				}
				switch (node.Kind)
				{
				case RegexNodeKind.Bol:
				case RegexNodeKind.Eol:
				case RegexNodeKind.Beginning:
				case RegexNodeKind.Start:
				case RegexNodeKind.EndZ:
				case RegexNodeKind.End:
					EmitAnchors(node);
					break;
				case RegexNodeKind.Boundary:
				case RegexNodeKind.NonBoundary:
				case RegexNodeKind.ECMABoundary:
				case RegexNodeKind.NonECMABoundary:
					EmitBoundary(node);
					break;
				case RegexNodeKind.Multi:
					EmitMultiChar(node, emitLengthChecksIfRequired);
					break;
				case RegexNodeKind.One:
				case RegexNodeKind.Notone:
				case RegexNodeKind.Set:
					EmitSingleChar(node, emitLengthChecksIfRequired);
					break;
				case RegexNodeKind.Oneloop:
				case RegexNodeKind.Notoneloop:
				case RegexNodeKind.Setloop:
					EmitSingleCharLoop(node, subsequent, emitLengthChecksIfRequired);
					break;
				case RegexNodeKind.Onelazy:
				case RegexNodeKind.Notonelazy:
				case RegexNodeKind.Setlazy:
					EmitSingleCharLazy(node, subsequent, emitLengthChecksIfRequired);
					break;
				case RegexNodeKind.Oneloopatomic:
				case RegexNodeKind.Notoneloopatomic:
				case RegexNodeKind.Setloopatomic:
					EmitSingleCharAtomicLoop(node);
					break;
				case RegexNodeKind.Loop:
					EmitLoop(node);
					break;
				case RegexNodeKind.Lazyloop:
					EmitLazy(node);
					break;
				case RegexNodeKind.Alternate:
					EmitAlternation(node);
					break;
				case RegexNodeKind.Concatenate:
					EmitConcatenation(node, subsequent, emitLengthChecksIfRequired);
					break;
				case RegexNodeKind.Atomic:
					EmitAtomic(node, subsequent);
					break;
				case RegexNodeKind.Backreference:
					EmitBackreference(node);
					break;
				case RegexNodeKind.BackreferenceConditional:
					EmitBackreferenceConditional(node);
					break;
				case RegexNodeKind.ExpressionConditional:
					EmitExpressionConditional(node);
					break;
				case RegexNodeKind.Capture:
					EmitCapture(node, subsequent);
					break;
				case RegexNodeKind.PositiveLookaround:
					EmitPositiveLookaroundAssertion(node);
					break;
				case RegexNodeKind.NegativeLookaround:
					EmitNegativeLookaroundAssertion(node);
					break;
				case RegexNodeKind.Nothing:
					BrFar(doneLabel);
					break;
				case RegexNodeKind.Empty:
					break;
				case RegexNodeKind.UpdateBumpalong:
					EmitUpdateBumpalong(node);
					break;
				case RegexNodeKind.Group:
				case (RegexNodeKind)35:
				case (RegexNodeKind)36:
				case (RegexNodeKind)37:
				case (RegexNodeKind)38:
				case (RegexNodeKind)39:
				case (RegexNodeKind)40:
					break;
				}
			}
		}
		void EmitNonBacktrackingRepeater(RegexNode node)
		{
			TransferSliceStaticPosToPos();
			Label l5 = DefineLabel();
			Label l6 = DefineLabel();
			using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
			Ldc(0);
			Stloc(rentedLocalBuilder);
			BrFar(l5);
			MarkLabel(l6);
			EmitNode(node.Child(0));
			TransferSliceStaticPosToPos();
			Ldloc(rentedLocalBuilder);
			Ldc(1);
			Add();
			Stloc(rentedLocalBuilder);
			MarkLabel(l5);
			Ldloc(rentedLocalBuilder);
			Ldc(node.M);
			BltFar(l6);
		}
		void EmitPositiveLookaroundAssertion(RegexNode node)
		{
			if (analysis.HasRightToLeft)
			{
				TransferSliceStaticPosToPos(forceSliceReload: true);
			}
			LocalBuilder lt2 = DeclareInt32();
			Ldloc(pos);
			Stloc(lt2);
			int num2 = sliceStaticPos;
			EmitTimeoutCheckIfNeeded();
			RegexNode node2 = node.Child(0);
			if (analysis.MayBacktrack(node2))
			{
				EmitAtomic(node, null);
			}
			else
			{
				EmitNode(node2);
			}
			Ldloc(lt2);
			Stloc(pos);
			SliceInputSpan();
			sliceStaticPos = num2;
		}
		void EmitSingleChar(RegexNode node, bool emitLengthCheck = true, LocalBuilder offset = null)
		{
			bool flag = (node.Options & RegexOptions.RightToLeft) != 0;
			if (emitLengthCheck)
			{
				if (!flag)
				{
					EmitSpanLengthCheck(1, offset);
				}
				else
				{
					Ldloc(pos);
					Ldc(1);
					Sub();
					Ldloca(inputSpan);
					Call(SpanGetLengthMethod);
					BgeUnFar(doneLabel);
				}
			}
			if (!flag)
			{
				Ldloca(slice);
				EmitSum(sliceStaticPos, offset);
			}
			else
			{
				Ldloca(inputSpan);
				EmitSum(-1, pos);
			}
			Call(SpanGetItemMethod);
			LdindU2();
			if (node.IsSetFamily)
			{
				EmitMatchCharacterClass(node.Str);
				BrfalseFar(doneLabel);
			}
			else
			{
				Ldc(node.Ch);
				if (node.IsOneFamily)
				{
					BneFar(doneLabel);
				}
				else
				{
					BeqFar(doneLabel);
				}
			}
			if (!flag)
			{
				sliceStaticPos++;
			}
			else
			{
				Ldloc(pos);
				Ldc(1);
				Sub();
				Stloc(pos);
			}
		}
		void EmitSingleCharAtomicLoop(RegexNode node)
		{
			if (node.M == node.N)
			{
				EmitSingleCharRepeater(node);
			}
			else
			{
				if (node.M != 0 || node.N != 1)
				{
					int m = node.M;
					int n = node.N;
					bool flag = (node.Options & RegexOptions.RightToLeft) != 0;
					using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
					Label l5 = DefineLabel();
					int literalLength;
					if (flag)
					{
						TransferSliceStaticPosToPos();
						Label l6 = DefineLabel();
						Label l7 = DefineLabel();
						Ldc(0);
						Stloc(rentedLocalBuilder);
						BrFar(l6);
						MarkLabel(l7);
						Ldloc(pos);
						Ldloc(rentedLocalBuilder);
						BleFar(l5);
						Ldloca(inputSpan);
						Ldloc(pos);
						Ldloc(rentedLocalBuilder);
						Sub();
						Ldc(1);
						Sub();
						Call(SpanGetItemMethod);
						LdindU2();
						if (node.IsSetFamily)
						{
							EmitMatchCharacterClass(node.Str);
							BrfalseFar(l5);
						}
						else
						{
							Ldc(node.Ch);
							if (node.IsOneFamily)
							{
								BneFar(l5);
							}
							else
							{
								BeqFar(l5);
							}
						}
						Ldloc(rentedLocalBuilder);
						Ldc(1);
						Add();
						Stloc(rentedLocalBuilder);
						MarkLabel(l6);
						if (n != int.MaxValue)
						{
							Ldloc(rentedLocalBuilder);
							Ldc(n);
							BltFar(l7);
						}
						else
						{
							BrFar(l7);
						}
					}
					else if (node.IsSetFamily && n == int.MaxValue && node.Str == "\0\u0001\0\0")
					{
						TransferSliceStaticPosToPos();
						Ldloca(inputSpan);
						Call(SpanGetLengthMethod);
						Ldloc(pos);
						Sub();
						Stloc(rentedLocalBuilder);
					}
					else if (n == int.MaxValue && CanEmitIndexOf(node, out literalLength))
					{
						if (sliceStaticPos > 0)
						{
							Ldloca(slice);
							Ldc(sliceStaticPos);
							Call(SpanSliceIntMethod);
						}
						else
						{
							Ldloc(slice);
						}
						EmitIndexOf(node, useLast: false, negate: true);
						Stloc(rentedLocalBuilder);
						Ldloc(rentedLocalBuilder);
						Ldc(0);
						BgeFar(l5);
						Ldloca(slice);
						Call(SpanGetLengthMethod);
						if (sliceStaticPos > 0)
						{
							Ldc(sliceStaticPos);
							Sub();
						}
						Stloc(rentedLocalBuilder);
					}
					else
					{
						TransferSliceStaticPosToPos();
						Label l8 = DefineLabel();
						Label l9 = DefineLabel();
						Ldc(0);
						Stloc(rentedLocalBuilder);
						BrFar(l8);
						MarkLabel(l9);
						Ldloc(rentedLocalBuilder);
						Ldloca(slice);
						Call(SpanGetLengthMethod);
						BgeUnFar(l5);
						Ldloca(slice);
						Ldloc(rentedLocalBuilder);
						Call(SpanGetItemMethod);
						LdindU2();
						if (node.IsSetFamily)
						{
							EmitMatchCharacterClass(node.Str);
							BrfalseFar(l5);
						}
						else
						{
							Ldc(node.Ch);
							if (node.IsOneFamily)
							{
								BneFar(l5);
							}
							else
							{
								BeqFar(l5);
							}
						}
						Ldloc(rentedLocalBuilder);
						Ldc(1);
						Add();
						Stloc(rentedLocalBuilder);
						MarkLabel(l8);
						if (n != int.MaxValue)
						{
							Ldloc(rentedLocalBuilder);
							Ldc(n);
							BltFar(l9);
						}
						else
						{
							BrFar(l9);
						}
					}
					MarkLabel(l5);
					if (m > 0)
					{
						Ldloc(rentedLocalBuilder);
						Ldc(m);
						BltFar(doneLabel);
					}
					if (!flag)
					{
						Ldloca(slice);
						Ldloc(rentedLocalBuilder);
						Call(SpanSliceIntMethod);
						Stloc(slice);
						Ldloc(pos);
						Ldloc(rentedLocalBuilder);
						Add();
						Stloc(pos);
					}
					else
					{
						Ldloc(pos);
						Ldloc(rentedLocalBuilder);
						Sub();
						Stloc(pos);
					}
					return;
				}
				EmitAtomicSingleCharZeroOrOne(node);
			}
		}
		void EmitSingleCharLazy(RegexNode node, RegexNode subsequent = null, bool emitLengthChecksIfRequired = true)
		{
			if (node.M > 0)
			{
				EmitSingleCharRepeater(node, emitLengthChecksIfRequired);
			}
			if (node.M == node.N || analysis.IsAtomicByAncestor(node))
			{
				return;
			}
			bool flag = (node.Options & RegexOptions.RightToLeft) != 0;
			TransferSliceStaticPosToPos();
			LocalBuilder iterationCount = null;
			int? num2 = null;
			if (node.N != int.MaxValue)
			{
				num2 = node.N - node.M;
				iterationCount = DeclareInt32();
				Ldc(0);
				Stloc(iterationCount);
			}
			LocalBuilder capturepos = (expressionHasCaptures ? DeclareInt32() : null);
			LocalBuilder startingPos = DeclareInt32();
			Ldloc(pos);
			Stloc(startingPos);
			Label l5 = DefineLabel();
			BrFar(l5);
			Label label = DefineLabel();
			MarkLabel(label);
			if (capturepos != null)
			{
				EmitUncaptureUntil(capturepos);
			}
			if (num2.HasValue)
			{
				Ldloc(iterationCount);
				Ldc(num2.Value);
				BgeFar(doneLabel);
				Ldloc(iterationCount);
				Ldc(1);
				Add();
				Stloc(iterationCount);
			}
			EmitTimeoutCheckIfNeeded();
			Ldloc(startingPos);
			Stloc(pos);
			SliceInputSpan();
			EmitSingleChar(node);
			TransferSliceStaticPosToPos();
			if (!flag && iterationCount == null && node.Kind == RegexNodeKind.Notonelazy)
			{
				RegexNode.StartingLiteralData? startingLiteralData = subsequent?.FindStartingLiteral();
				if (startingLiteralData.HasValue)
				{
					RegexNode.StartingLiteralData valueOrDefault = startingLiteralData.GetValueOrDefault();
					if (!valueOrDefault.Negated && (valueOrDefault.String != null || valueOrDefault.SetChars != null || valueOrDefault.Range.LowInclusive == valueOrDefault.Range.HighInclusive || (valueOrDefault.Range.LowInclusive <= node.Ch && node.Ch <= valueOrDefault.Range.HighInclusive)))
					{
						Ldloc(slice);
						bool flag2;
						if (valueOrDefault.String != null)
						{
							flag2 = valueOrDefault.String[0] == node.Ch;
							if (flag2)
							{
								Ldc(node.Ch);
								Call(SpanIndexOfCharMethod);
							}
							else
							{
								Ldc(node.Ch);
								Ldc(valueOrDefault.String[0]);
								Call(SpanIndexOfAnyCharCharMethod);
							}
						}
						else if (valueOrDefault.SetChars != null)
						{
							flag2 = valueOrDefault.SetChars.Contains(node.Ch);
							int length = valueOrDefault.SetChars.Length;
							if (flag2)
							{
								switch (length)
								{
								case 2:
									Ldc(valueOrDefault.SetChars[0]);
									Ldc(valueOrDefault.SetChars[1]);
									Call(SpanIndexOfAnyCharCharMethod);
									break;
								case 3:
									Ldc(valueOrDefault.SetChars[0]);
									Ldc(valueOrDefault.SetChars[1]);
									Ldc(valueOrDefault.SetChars[2]);
									Call(SpanIndexOfAnyCharCharCharMethod);
									break;
								default:
									EmitIndexOfAnyWithSearchValuesOrLiteral(valueOrDefault.SetChars.AsSpan());
									break;
								}
							}
							else if (length == 2)
							{
								Ldc(node.Ch);
								Ldc(valueOrDefault.SetChars[0]);
								Ldc(valueOrDefault.SetChars[1]);
								Call(SpanIndexOfAnyCharCharCharMethod);
							}
							else
							{
								EmitIndexOfAnyWithSearchValuesOrLiteral($"{node.Ch}{valueOrDefault.SetChars}".AsSpan());
							}
						}
						else if (valueOrDefault.Range.LowInclusive == valueOrDefault.Range.HighInclusive)
						{
							flag2 = valueOrDefault.Range.LowInclusive == node.Ch;
							if (flag2)
							{
								Ldc(node.Ch);
								Call(SpanIndexOfCharMethod);
							}
							else
							{
								Ldc(node.Ch);
								Ldc(valueOrDefault.Range.LowInclusive);
								Call(SpanIndexOfAnyCharCharMethod);
							}
						}
						else
						{
							flag2 = true;
							Ldc(valueOrDefault.Range.LowInclusive);
							Ldc(valueOrDefault.Range.HighInclusive);
							Call(SpanIndexOfAnyInRangeMethod);
						}
						Stloc(startingPos);
						if (flag2)
						{
							Ldloc(startingPos);
							Ldc(0);
							BltFar(doneLabel);
						}
						else
						{
							Ldloc(startingPos);
							Ldloca(slice);
							Call(SpanGetLengthMethod);
							BgeUnFar(doneLabel);
							Ldloca(slice);
							Ldloc(startingPos);
							Call(SpanGetItemMethod);
							LdindU2();
							Ldc(node.Ch);
							BeqFar(doneLabel);
						}
						Ldloc(pos);
						Ldloc(startingPos);
						Add();
						Stloc(pos);
						SliceInputSpan();
						goto IL_07e2;
					}
				}
			}
			if (!flag && iterationCount == null && node.Kind == RegexNodeKind.Setlazy && node.Str == "\0\u0001\0\0")
			{
				RegexNode regexNode = subsequent?.FindStartingLiteralNode();
				if (regexNode != null && CanEmitIndexOf(regexNode, out var _))
				{
					Ldloc(slice);
					EmitIndexOf(regexNode, useLast: false, negate: false);
					Stloc(startingPos);
					Ldloc(startingPos);
					Ldc(0);
					BltFar(doneLabel);
					Ldloc(pos);
					Ldloc(startingPos);
					Add();
					Stloc(pos);
					SliceInputSpan();
				}
			}
			goto IL_07e2;
			IL_07e2:
			Ldloc(pos);
			Stloc(startingPos);
			_ = doneLabel;
			doneLabel = label;
			MarkLabel(l5);
			if (capturepos != null)
			{
				Ldthis();
				Call(CrawlposMethod);
				Stloc(capturepos);
			}
			if (analysis.IsInLoop(node))
			{
				EmitStackResizeIfNeeded(1 + ((capturepos != null) ? 1 : 0) + ((iterationCount != null) ? 1 : 0));
				EmitStackPush(delegate
				{
					Ldloc(startingPos);
				});
				if (capturepos != null)
				{
					EmitStackPush(delegate
					{
						Ldloc(capturepos);
					});
				}
				if (iterationCount != null)
				{
					EmitStackPush(delegate
					{
						Ldloc(iterationCount);
					});
				}
				Label l6 = DefineLabel();
				BrFar(l6);
				Label label2 = DefineLabel();
				MarkLabel(label2);
				if (iterationCount != null)
				{
					EmitStackPop();
					Stloc(iterationCount);
				}
				if (capturepos != null)
				{
					EmitStackPop();
					Stloc(capturepos);
				}
				EmitStackPop();
				Stloc(startingPos);
				BrFar(doneLabel);
				doneLabel = label2;
				MarkLabel(l6);
			}
		}
		void EmitSingleCharLoop(RegexNode node, RegexNode subsequent = null, bool emitLengthChecksIfRequired = true)
		{
			if (analysis.IsAtomicByAncestor(node))
			{
				EmitSingleCharAtomicLoop(node);
				return;
			}
			if (node.M == node.N)
			{
				EmitSingleCharRepeater(node, emitLengthChecksIfRequired);
				return;
			}
			Label label = DefineLabel();
			Label l5 = DefineLabel();
			LocalBuilder startingPos = DeclareInt32();
			LocalBuilder endingPos = DeclareInt32();
			LocalBuilder localBuilder = (expressionHasCaptures ? DeclareInt32() : null);
			bool flag = (node.Options & RegexOptions.RightToLeft) != 0;
			bool num2 = analysis.IsInLoop(node);
			TransferSliceStaticPosToPos();
			Ldloc(pos);
			Stloc(startingPos);
			EmitSingleCharAtomicLoop(node);
			TransferSliceStaticPosToPos();
			Ldloc(pos);
			Stloc(endingPos);
			if (node.M > 0)
			{
				Ldloc(startingPos);
				Ldc((!flag) ? node.M : (-node.M));
				Add();
				Stloc(startingPos);
			}
			BrFar(l5);
			MarkLabel(label);
			if (num2)
			{
				if (localBuilder != null)
				{
					EmitStackPop();
					Stloc(localBuilder);
					EmitUncaptureUntil(localBuilder);
				}
				EmitStackPop();
				Stloc(endingPos);
				EmitStackPop();
				Stloc(startingPos);
			}
			else if (localBuilder != null)
			{
				EmitUncaptureUntil(localBuilder);
			}
			EmitTimeoutCheckIfNeeded();
			Ldloc(startingPos);
			Ldloc(endingPos);
			if (!flag)
			{
				BgeFar(doneLabel);
			}
			else
			{
				BleFar(doneLabel);
			}
			if (!flag && node.N > 1)
			{
				RegexNode regexNode = subsequent?.FindStartingLiteralNode();
				if (regexNode != null && CanEmitIndexOf(regexNode, out var literalLength))
				{
					Ldloca(inputSpan);
					Ldloc(startingPos);
					if (literalLength > 1)
					{
						Ldloca(inputSpan);
						Call(SpanGetLengthMethod);
						Ldloc(endingPos);
						Ldc(literalLength - 1);
						Add();
						Call(MathMinIntIntMethod);
					}
					else
					{
						Ldloc(endingPos);
					}
					Ldloc(startingPos);
					Sub();
					Call(SpanSliceIntIntMethod);
					EmitIndexOf(regexNode, useLast: true, negate: false);
					Stloc(endingPos);
					Ldloc(endingPos);
					Ldc(0);
					BltFar(doneLabel);
					Ldloc(endingPos);
					Ldloc(startingPos);
					Add();
					Stloc(endingPos);
					goto IL_03da;
				}
			}
			Ldloc(endingPos);
			Ldc((!flag) ? 1 : (-1));
			Sub();
			Stloc(endingPos);
			goto IL_03da;
			IL_03da:
			Ldloc(endingPos);
			Stloc(pos);
			if (!flag)
			{
				SliceInputSpan();
			}
			MarkLabel(l5);
			if (num2)
			{
				EmitStackResizeIfNeeded(2 + ((localBuilder != null) ? 1 : 0));
				EmitStackPush(delegate
				{
					Ldloc(startingPos);
				});
				EmitStackPush(delegate
				{
					Ldloc(endingPos);
				});
				if (localBuilder != null)
				{
					EmitStackPush(delegate
					{
						Ldthis();
						Call(CrawlposMethod);
					});
				}
			}
			else if (localBuilder != null)
			{
				Ldthis();
				Call(CrawlposMethod);
				Stloc(localBuilder);
			}
			doneLabel = label;
		}
		void EmitSingleCharRepeater(RegexNode node, bool emitLengthChecksIfRequired = true)
		{
			int m = node.M;
			bool flag = (node.Options & RegexOptions.RightToLeft) != 0;
			int literalLength = m;
			if (literalLength <= 64)
			{
				switch (literalLength)
				{
				case 0:
					return;
				case 1:
					EmitSingleChar(node, emitLengthChecksIfRequired);
					return;
				}
				if (node.IsOneFamily)
				{
					EmitMultiCharString(new string(node.Ch, m), emitLengthChecksIfRequired, flag);
					return;
				}
			}
			if (flag)
			{
				TransferSliceStaticPosToPos();
				Label l5 = DefineLabel();
				Label l6 = DefineLabel();
				using RentedLocalBuilder rentedLocalBuilder = RentInt32Local();
				Ldc(0);
				Stloc(rentedLocalBuilder);
				BrFar(l5);
				MarkLabel(l6);
				EmitSingleChar(node);
				Ldloc(rentedLocalBuilder);
				Ldc(1);
				Add();
				Stloc(rentedLocalBuilder);
				MarkLabel(l5);
				Ldloc(rentedLocalBuilder);
				Ldc(m);
				BltFar(l6);
				return;
			}
			if (emitLengthChecksIfRequired)
			{
				EmitSpanLengthCheck(m);
			}
			if (node.IsSetFamily && node.Str == "\0\u0001\0\0")
			{
				sliceStaticPos += m;
			}
			else if (m <= 16)
			{
				for (int i = 0; i < m; i++)
				{
					EmitSingleChar(node, emitLengthCheck: false);
				}
			}
			else
			{
				Ldloca(slice);
				Ldc(sliceStaticPos);
				Ldc(m);
				Call(SpanSliceIntIntMethod);
				if (CanEmitIndexOf(node, out literalLength))
				{
					EmitIndexOf(node, useLast: false, negate: true);
					Ldc(0);
					BgeFar(doneLabel);
				}
				else
				{
					using RentedLocalBuilder rentedLocalBuilder2 = RentReadOnlySpanCharLocal();
					Stloc(rentedLocalBuilder2);
					Label l7 = DefineLabel();
					Label l8 = DefineLabel();
					using RentedLocalBuilder rentedLocalBuilder3 = RentInt32Local();
					Ldc(0);
					Stloc(rentedLocalBuilder3);
					BrFar(l7);
					MarkLabel(l8);
					LocalBuilder localBuilder = slice;
					int num2 = sliceStaticPos;
					slice = rentedLocalBuilder2;
					sliceStaticPos = 0;
					EmitSingleChar(node, emitLengthCheck: false, rentedLocalBuilder3);
					slice = localBuilder;
					sliceStaticPos = num2;
					Ldloc(rentedLocalBuilder3);
					Ldc(1);
					Add();
					Stloc(rentedLocalBuilder3);
					MarkLabel(l7);
					Ldloc(rentedLocalBuilder3);
					Ldloca(rentedLocalBuilder2);
					Call(SpanGetLengthMethod);
					BltFar(l8);
				}
				sliceStaticPos += m;
			}
		}
		void EmitSpanLengthCheck(int requiredLength, LocalBuilder dynamicRequiredLength = null)
		{
			EmitSum(sliceStaticPos + requiredLength - 1, dynamicRequiredLength);
			Ldloca(slice);
			Call(SpanGetLengthMethod);
			BgeUnFar(doneLabel);
		}
		void EmitStackPop()
		{
			Ldthisfld(RunstackField);
			Ldloc(stackpos);
			Ldc(1);
			Sub();
			Stloc(stackpos);
			Ldloc(stackpos);
			LdelemI4();
		}
		void EmitStackPush(Action load)
		{
			Ldthisfld(RunstackField);
			Ldloc(stackpos);
			load();
			StelemI4();
			Ldloc(stackpos);
			Ldc(1);
			Add();
			Stloc(stackpos);
		}
		void EmitStackResizeIfNeeded(int count)
		{
			Label l5 = DefineLabel();
			Ldloc(stackpos);
			Ldthisfld(RunstackField);
			Ldlen();
			if (count > 1)
			{
				Ldc(count - 1);
				Sub();
			}
			Blt(l5);
			Ldthis();
			_ilg.Emit(OpCodes.Ldflda, RunstackField);
			Ldthisfld(RunstackField);
			Ldlen();
			Ldc(2);
			Mul();
			Call(ArrayResizeMethod);
			MarkLabel(l5);
		}
		void EmitSum(int constant, LocalBuilder local)
		{
			if (local == null)
			{
				Ldc(constant);
			}
			else if (constant == 0)
			{
				Ldloc(local);
			}
			else
			{
				Ldloc(local);
				Ldc(constant);
				Add();
			}
		}
		void EmitUncaptureUntil(LocalBuilder startingCapturePos)
		{
			Label l5 = DefineLabel();
			Label l6 = DefineLabel();
			Br(l5);
			MarkLabel(l6);
			Ldthis();
			Call(UncaptureMethod);
			MarkLabel(l5);
			Ldthis();
			Call(CrawlposMethod);
			Ldloc(startingCapturePos);
			Bgt(l6);
		}
		void EmitUncaptureUntilPopped()
		{
			if (expressionHasCaptures)
			{
				using (RentedLocalBuilder rentedLocalBuilder = RentInt32Local())
				{
					EmitStackPop();
					Stloc(rentedLocalBuilder);
					EmitUncaptureUntil(rentedLocalBuilder);
				}
			}
		}
		void EmitUpdateBumpalong(RegexNode node)
		{
			TransferSliceStaticPosToPos();
			Ldthisfld(RuntextposField);
			Ldloc(pos);
			Label l5 = DefineLabel();
			Bge(l5);
			Ldthis();
			Ldloc(pos);
			Stfld(RuntextposField);
			MarkLabel(l5);
		}
		static RegexNode GetSubsequent(int index, RegexNode node, RegexNode subsequent)
		{
			int num2 = node.ChildCount();
			for (int i = index + 1; i < num2; i++)
			{
				RegexNode regexNode = node.Child(i);
				if (regexNode.Kind != RegexNodeKind.UpdateBumpalong)
				{
					return regexNode;
				}
			}
			return subsequent;
		}
		void SliceInputSpan()
		{
			Ldloca(inputSpan);
			Ldloc(pos);
			Call(SpanSliceIntMethod);
			Stloc(slice);
		}
		void TransferSliceStaticPosToPos(bool forceSliceReload = false)
		{
			if (sliceStaticPos > 0)
			{
				Ldloc(pos);
				Ldc(sliceStaticPos);
				Add();
				Stloc(pos);
				sliceStaticPos = 0;
				SliceInputSpan();
			}
			else if (forceSliceReload)
			{
				SliceInputSpan();
			}
		}
	}

	protected void EmitScan(RegexOptions options, MethodInfo tryFindNextStartingPositionMethod, MethodInfo tryMatchAtCurrentPositionMethod)
	{
		bool flag = (options & RegexOptions.RightToLeft) != 0;
		RegexNode regexNode = _regexTree.Root.Child(0);
		Label l = DefineLabel();
		RegexNodeKind kind = regexNode.Kind;
		if (kind - 9 <= RegexNodeKind.Oneloop)
		{
			Ldthis();
			Ldarg_1();
			Call(tryFindNextStartingPositionMethod);
			Brfalse(l);
			LocalBuilder lt = DeclareInt32();
			Mvfldloc(RuntextposField, lt);
			LocalBuilder lt2 = DeclareInt32();
			Ldloc(lt);
			Ldc(((regexNode.Kind != RegexNodeKind.Multi) ? 1 : regexNode.Str.Length) * ((!flag) ? 1 : (-1)));
			Add();
			Stloc(lt2);
			Ldthis();
			Ldloc(lt2);
			Stfld(RuntextposField);
			Ldthis();
			Ldc(0);
			Ldloc(lt);
			Ldloc(lt2);
			Call(CaptureMethod);
		}
		else
		{
			FindNextStartingPositionMode findMode = _regexTree.FindOptimizations.FindMode;
			if (((uint)findMode <= 1u || findMode == FindNextStartingPositionMode.LeadingAnchor_RightToLeft_Start || findMode == FindNextStartingPositionMode.LeadingAnchor_RightToLeft_End) ? true : false)
			{
				Ldthis();
				Ldarg_1();
				Call(tryFindNextStartingPositionMethod);
				Brfalse(l);
				Ldthis();
				Ldarg_1();
				Call(tryMatchAtCurrentPositionMethod);
				Brtrue(l);
				Ldthis();
				if (!flag)
				{
					Ldarga_s(1);
					Call(SpanGetLengthMethod);
				}
				else
				{
					Ldc(0);
				}
				Stfld(RuntextposField);
			}
			else
			{
				Label l2 = DefineLabel();
				MarkLabel(l2);
				Ldthis();
				Ldarg_1();
				Call(tryFindNextStartingPositionMethod);
				BrfalseFar(l);
				Ldthis();
				Ldarg_1();
				Call(tryMatchAtCurrentPositionMethod);
				BrtrueFar(l);
				Ldthisfld(RuntextposField);
				if (!flag)
				{
					Ldarga_s(1);
					Call(SpanGetLengthMethod);
				}
				else
				{
					Ldc(0);
				}
				Ceq();
				BrtrueFar(l);
				Ldthis();
				Ldthisfld(RuntextposField);
				Ldc((!flag) ? 1 : (-1));
				Add();
				Stfld(RuntextposField);
				EmitTimeoutCheckIfNeeded();
				BrFar(l2);
			}
		}
		MarkLabel(l);
		Ret();
	}

	private void EmitMatchCharacterClass(string charClass)
	{
		switch (charClass)
		{
		case "\0\u0001\0\0":
			Pop();
			Ldc(1);
			return;
		case "\0\0\u0001\t":
		case "\0\0\u0001\ufff7":
			Call(CharIsDigitMethod);
			NegateIf(charClass == "\0\0\u0001\ufff7");
			return;
		case "\0\0\u0001d":
		case "\0\0\u0001ﾜ":
			Call(CharIsWhiteSpaceMethod);
			NegateIf(charClass == "\0\0\u0001ﾜ");
			return;
		case "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0":
		case "\0\0\n\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0":
			Call(IsWordCharMethod);
			NegateIf(charClass == "\0\0\n\0\ufffe￼\ufffb\ufffd\uffff\ufffa\ufff7￭\0");
			return;
		case "\0\0\u0001\u000f":
		case "\0\0\u0001\ufff1":
			Call(CharIsControlMethod);
			NegateIf(charClass == "\0\0\u0001\ufff1");
			return;
		case "\0\0\a\0\u0002\u0004\u0005\u0003\u0001\0":
		case "\0\0\a\0\ufffe￼\ufffb\ufffd\uffff\0":
			Call(CharIsLetterMethod);
			NegateIf(charClass == "\0\0\a\0\ufffe￼\ufffb\ufffd\uffff\0");
			return;
		case "\0\0\b\0\u0002\u0004\u0005\u0003\u0001\0\t":
		case "\u0001\0\b\0\u0002\u0004\u0005\u0003\u0001\0\t":
			Call(CharIsLetterOrDigitMethod);
			NegateIf(charClass == "\u0001\0\b\0\u0002\u0004\u0005\u0003\u0001\0\t");
			return;
		case "\0\0\u0001\u0002":
		case "\0\0\u0001\ufffe":
			Call(CharIsLowerMethod);
			NegateIf(charClass == "\0\0\u0001\ufffe");
			return;
		case "\0\0\u0001\u0001":
		case "\0\0\u0001\uffff":
			Call(CharIsUpperMethod);
			NegateIf(charClass == "\0\0\u0001\uffff");
			return;
		case "\0\0\u0005\0\ufff7\ufff6\ufff5\0":
		case "\0\0\u0005\0\t\n\v\0":
			Call(CharIsNumberMethod);
			NegateIf(charClass == "\0\0\u0005\0\ufff7\ufff6\ufff5\0");
			return;
		case "\0\0\t\0\u0013\u0014\u0016\u0019\u0015\u0018\u0017\0":
		case "\0\0\t\0￭￬￪\uffe7￫￨￩\0":
			Call(CharIsPunctuationMethod);
			NegateIf(charClass == "\0\0\t\0￭￬￪\uffe7￫￨￩\0");
			return;
		case "\0\0\u0005\0\ufff3\ufff2\ufff4\0":
		case "\0\0\u0005\0\r\u000e\f\0":
			Call(CharIsSeparatorMethod);
			NegateIf(charClass == "\0\0\u0005\0\ufff3\ufff2\ufff4\0");
			return;
		case "\0\0\u0006\0￥￤￦\uffe3\0":
		case "\0\0\u0006\0\u001b\u001c\u001a\u001d\0":
			Call(CharIsSymbolMethod);
			NegateIf(charClass == "\0\0\u0006\0￥￤￦\uffe3\0");
			return;
		case "\0\u0004\0A[a{":
		case "\u0001\u0004\0A[a{":
			Call(CharIsAsciiLetterMethod);
			NegateIf(charClass == "\u0001\u0004\0A[a{");
			return;
		case "\0\u0006\00:A[a{":
		case "\u0001\u0006\00:A[a{":
			Call(CharIsAsciiLetterOrDigitMethod);
			NegateIf(charClass == "\u0001\u0006\00:A[a{");
			return;
		case "\0\u0006\00:AGag":
		case "\u0001\u0006\00:AGag":
			Call(CharIsAsciiHexDigitMethod);
			NegateIf(charClass == "\u0001\u0006\00:AGag");
			return;
		case "\0\u0004\00:ag":
		case "\u0001\u0004\00:ag":
			Call(CharIsAsciiHexDigitLowerMethod);
			NegateIf(charClass == "\u0001\u0004\00:ag");
			return;
		case "\0\u0004\00:AG":
		case "\u0001\u0004\00:AG":
			Call(CharIsAsciiHexDigitUpperMethod);
			NegateIf(charClass == "\u0001\u0004\00:AG");
			return;
		}
		if (RegexCharClass.TryGetSingleRange(charClass, out var lowInclusive, out var highInclusive))
		{
			if (lowInclusive == highInclusive)
			{
				Ldc(lowInclusive);
				Ceq();
			}
			else
			{
				Ldc(lowInclusive);
				Sub();
				Ldc(highInclusive - lowInclusive + 1);
				CltUn();
			}
			NegateIf(RegexCharClass.IsNegated(charClass));
			return;
		}
		Span<UnicodeCategory> categories = stackalloc UnicodeCategory[1];
		if (RegexCharClass.TryGetOnlyCategories(charClass, categories, out var _, out var negated))
		{
			Call(CharGetUnicodeInfoMethod);
			Ldc((int)categories[0]);
			Ceq();
			NegateIf(negated);
			return;
		}
		RentedLocalBuilder tempLocal = RentInt32Local();
		RentedLocalBuilder resultLocal;
		Label doneLabel;
		Label comparisonLabel;
		try
		{
			Stloc(tempLocal);
			Span<char> chars = stackalloc char[3];
			int setChars = RegexCharClass.GetSetChars(charClass, chars);
			if ((uint)(setChars - 2) <= 1u)
			{
				if (RegexCharClass.DifferByOneBit(chars[0], chars[1], out var mask))
				{
					Ldloc(tempLocal);
					Ldc(mask);
					Or();
					Ldc(chars[1] | mask);
					Ceq();
				}
				else
				{
					Ldloc(tempLocal);
					Ldc(chars[0]);
					Ceq();
					Ldloc(tempLocal);
					Ldc(chars[1]);
					Ceq();
					Or();
				}
				if (setChars == 3)
				{
					Ldloc(tempLocal);
					Ldc(chars[2]);
					Ceq();
					Or();
				}
				NegateIf(RegexCharClass.IsNegated(charClass));
				return;
			}
			if (RegexCharClass.TryGetDoubleRange(charClass, out (char, char) range, out (char, char) range2) && char.IsAsciiLetter(range2.Item1) && char.IsAsciiLetter(range2.Item2) && (range.Item1 | 0x20) == range2.Item1 && (range.Item2 | 0x20) == range2.Item2)
			{
				bool condition = RegexCharClass.IsNegated(charClass);
				Ldloc(tempLocal);
				Ldc(32);
				Or();
				Ldc(range2.Item1);
				Sub();
				Ldc(range2.Item2 - range2.Item1 + 1);
				CltUn();
				NegateIf(condition);
				return;
			}
			RegexCharClass.CharClassAnalysisResults charClassAnalysisResults = RegexCharClass.Analyze(charClass);
			if (charClassAnalysisResults.OnlyRanges && charClassAnalysisResults.UpperBoundExclusiveIfOnlyRanges - charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges <= 32)
			{
				uint num = 0u;
				bool flag = RegexCharClass.IsNegated(charClass);
				for (int i = charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges; i < charClassAnalysisResults.UpperBoundExclusiveIfOnlyRanges; i++)
				{
					if (RegexCharClass.CharInClass((char)i, charClass) ^ flag)
					{
						num |= (uint)(1 << 31 - (i - charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges));
					}
				}
				LocalBuilder lt = _ilg.DeclareLocal(typeof(uint));
				Ldloc(tempLocal);
				Ldc(charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges);
				Sub();
				_ilg.Emit(OpCodes.Conv_U2);
				Stloc(lt);
				_ilg.Emit(OpCodes.Ldc_I4, num);
				Ldloc(lt);
				_ilg.Emit(OpCodes.Conv_I2);
				Ldc(31);
				And();
				Shl();
				Ldloc(lt);
				Ldc(32);
				_ilg.Emit(OpCodes.Conv_I4);
				Sub();
				And();
				Ldc(0);
				_ilg.Emit(OpCodes.Conv_I4);
				_ilg.Emit(OpCodes.Clt);
				NegateIf(flag);
				return;
			}
			if (IntPtr.Size == 8 && charClassAnalysisResults.OnlyRanges && charClassAnalysisResults.UpperBoundExclusiveIfOnlyRanges - charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges <= 64)
			{
				ulong num2 = 0uL;
				bool flag2 = RegexCharClass.IsNegated(charClass);
				for (int j = charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges; j < charClassAnalysisResults.UpperBoundExclusiveIfOnlyRanges; j++)
				{
					if (RegexCharClass.CharInClass((char)j, charClass) ^ flag2)
					{
						num2 |= (ulong)(1L << 63 - (j - charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges));
					}
				}
				LocalBuilder lt2 = _ilg.DeclareLocal(typeof(ulong));
				Ldloc(tempLocal);
				Ldc(charClassAnalysisResults.LowerBoundInclusiveIfOnlyRanges);
				Sub();
				_ilg.Emit(OpCodes.Conv_U8);
				Stloc(lt2);
				LdcI8((long)num2);
				Ldloc(lt2);
				_ilg.Emit(OpCodes.Conv_I4);
				Ldc(63);
				And();
				Shl();
				Ldloc(lt2);
				Ldc(64);
				_ilg.Emit(OpCodes.Conv_I8);
				Sub();
				And();
				Ldc(0);
				_ilg.Emit(OpCodes.Conv_I8);
				_ilg.Emit(OpCodes.Clt);
				NegateIf(flag2);
				return;
			}
			if (RegexCharClass.TryGetDoubleRange(charClass, out (char, char) range3, out (char, char) range4))
			{
				bool flag3 = RegexCharClass.IsNegated(charClass);
				if (range3.Item1 == range3.Item2)
				{
					Ldloc(tempLocal);
					Ldc(range3.Item1);
					Ceq();
				}
				else
				{
					Ldloc(tempLocal);
					Ldc(range3.Item1);
					Sub();
					Ldc(range3.Item2 - range3.Item1 + 1);
					CltUn();
				}
				NegateIf(flag3);
				if (range4.Item1 == range4.Item2)
				{
					Ldloc(tempLocal);
					Ldc(range4.Item1);
					Ceq();
				}
				else
				{
					Ldloc(tempLocal);
					Ldc(range4.Item1);
					Sub();
					Ldc(range4.Item2 - range4.Item1 + 1);
					CltUn();
				}
				NegateIf(flag3);
				if (flag3)
				{
					And();
				}
				else
				{
					Or();
				}
				return;
			}
			resultLocal = RentInt32Local();
			try
			{
				doneLabel = DefineLabel();
				comparisonLabel = DefineLabel();
				if (charClassAnalysisResults.ContainsNoAscii)
				{
					EmitContainsNoAscii();
					return;
				}
				if (charClassAnalysisResults.AllAsciiContained)
				{
					EmitAllAsciiContained();
					return;
				}
				string text = string.Create(8, charClass, delegate(Span<char> dest, string set)
				{
					for (int k = 0; k < 128; k++)
					{
						if (RegexCharClass.CharInClass((char)k, set))
						{
							dest[k >> 4] |= (char)(ushort)(1 << (k & 0xF));
						}
					}
				});
				if (!(text == "\0\0\0\0\0\0\0\0"))
				{
					if (text == "\uffff\uffff\uffff\uffff\uffff\uffff\uffff\uffff")
					{
						EmitAllAsciiContained();
						return;
					}
					Ldloc(tempLocal);
					Ldc(charClassAnalysisResults.ContainsOnlyAscii ? charClassAnalysisResults.UpperBoundExclusiveIfOnlyRanges : 128);
					Bge(comparisonLabel);
					switch (text)
					{
					case "\0\0\0Ͽ\ufffe߿\ufffe߿":
						Ldloc(tempLocal);
						Call(CharIsAsciiLetterOrDigitMethod);
						break;
					case "\0\0\0Ͽ\0\0\0\0":
						Ldloc(tempLocal);
						Call(CharIsAsciiDigitMethod);
						break;
					case "\0\0\0\0\ufffe߿\ufffe߿":
						Ldloc(tempLocal);
						Call(CharIsAsciiLetterMethod);
						break;
					case "\0\0\0\0\0\0\ufffe߿":
						Ldloc(tempLocal);
						Call(CharIsAsciiLetterLowerMethod);
						break;
					case "\0\0\0\0\ufffe߿\0\0":
						Ldloc(tempLocal);
						Call(CharIsAsciiLetterUpperMethod);
						break;
					case "\0\0\0Ͽ~\0~\0":
						Ldloc(tempLocal);
						Call(CharIsAsciiHexDigitMethod);
						break;
					case "\0\0\0Ͽ\0\0~\0":
						Ldloc(tempLocal);
						Call(CharIsAsciiHexDigitLowerMethod);
						break;
					case "\0\0\0Ͽ~\0\0\0":
						Ldloc(tempLocal);
						Call(CharIsAsciiHexDigitUpperMethod);
						break;
					default:
						Ldstr(text);
						Ldloc(tempLocal);
						Ldc(4);
						Shr();
						Call(StringGetCharsMethod);
						Ldc(1);
						Ldloc(tempLocal);
						Ldc(15);
						And();
						Ldc(31);
						And();
						Shl();
						And();
						Ldc(0);
						CgtUn();
						break;
					}
					Stloc(resultLocal);
					Br(doneLabel);
					MarkLabel(comparisonLabel);
					if (charClassAnalysisResults.ContainsOnlyAscii)
					{
						Ldc(0);
						Stloc(resultLocal);
					}
					else if (charClassAnalysisResults.AllNonAsciiContained)
					{
						Ldc(1);
						Stloc(resultLocal);
					}
					else
					{
						EmitCharInClass();
					}
					MarkLabel(doneLabel);
					Ldloc(resultLocal);
				}
				else
				{
					EmitContainsNoAscii();
				}
			}
			finally
			{
				((IDisposable)resultLocal/*cast due to constrained. prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)tempLocal/*cast due to constrained. prefix*/).Dispose();
		}
		void EmitAllAsciiContained()
		{
			Ldloc(tempLocal);
			Ldc(128);
			Blt(comparisonLabel);
			EmitCharInClass();
			Br(doneLabel);
			MarkLabel(comparisonLabel);
			Ldc(1);
			Stloc(resultLocal);
			MarkLabel(doneLabel);
			Ldloc(resultLocal);
		}
		void EmitCharInClass()
		{
			Ldloc(tempLocal);
			Ldstr(charClass);
			Call(CharInClassMethod);
			Stloc(resultLocal);
		}
		void EmitContainsNoAscii()
		{
			Ldloc(tempLocal);
			Ldc(128);
			Blt(comparisonLabel);
			EmitCharInClass();
			Br(doneLabel);
			MarkLabel(comparisonLabel);
			Ldc(0);
			Stloc(resultLocal);
			MarkLabel(doneLabel);
			Ldloc(resultLocal);
		}
	}

	private void NegateIf(bool condition)
	{
		if (condition)
		{
			Ldc(0);
			Ceq();
		}
	}

	private void EmitTimeoutCheckIfNeeded()
	{
		if (_hasTimeout)
		{
			Ldthis();
			Call(CheckTimeoutMethod);
		}
	}

	private void EmitIndexOfAnyWithSearchValuesOrLiteral(ReadOnlySpan<char> chars, bool last = false, bool except = false)
	{
		int length = chars.Length;
		bool flag = (uint)(length - 4) <= 1u;
		if (flag && !RegexCharClass.IsAscii(chars))
		{
			Ldstr(chars.ToString());
			Call(StringAsSpanMethod);
			MethodInfo mt = ((!last) ? (except ? SpanIndexOfAnyExceptSpanMethod : SpanIndexOfAnySpanMethod) : (except ? SpanLastIndexOfAnyExceptSpanMethod : SpanLastIndexOfAnySpanMethod));
			Call(mt);
		}
		else
		{
			LoadSearchValues(chars.ToArray());
			MethodInfo mt = ((!last) ? (except ? SpanIndexOfAnyExceptSearchValuesMethod : SpanIndexOfAnySearchValuesMethod) : (except ? SpanLastIndexOfAnyExceptSearchValuesMethod : SpanLastIndexOfAnySearchValuesMethod));
			Call(mt);
		}
	}

	private void LoadSearchValues<T>(T[] values, StringComparison comparison = StringComparison.Ordinal)
	{
		List<object> list = _searchValues ?? (_searchValues = new List<object>());
		int count = list.Count;
		object item;
		if (!(typeof(T) == typeof(char)))
		{
			if (!(typeof(T) == typeof(string)))
			{
				throw new UnreachableException();
			}
			item = SearchValues.Create((string[])(object)values, comparison);
		}
		else
		{
			item = SearchValues.Create((char[])(object)values);
		}
		list.Add(item);
		Ldthisfld(SearchValuesArrayField);
		Call(MemoryMarshalGetArrayDataReferenceSearchValuesMethod);
		Ldc(count * IntPtr.Size);
		Add();
		_ilg.Emit(OpCodes.Ldind_Ref);
		Call(MakeUnsafeAs(list[count].GetType()));
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2060:MakeGenericMethod", Justification = "Calling Unsafe.As<T> is safe since the T doesn't have trimming annotations.")]
		static MethodInfo MakeUnsafeAs(Type type)
		{
			return UnsafeAsMethod.MakeGenericMethod(type);
		}
	}
}
