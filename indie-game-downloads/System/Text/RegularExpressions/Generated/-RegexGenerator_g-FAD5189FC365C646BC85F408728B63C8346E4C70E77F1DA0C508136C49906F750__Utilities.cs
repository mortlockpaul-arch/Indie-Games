using System.Buffers;
using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.37416")]
internal static class _003CRegexGenerator_g_003EFAD5189FC365C646BC85F408728B63C8346E4C70E77F1DA0C508136C49906F750__Utilities
{
	internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

	internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

	internal static readonly SearchValues<string> s_indexOfString_AC244FE541E52CFD90CCD7AB13997D32F9D5031F007841348B9EE5A65488D64E = SearchValues.Create(new ReadOnlySpan<string>("(&"), StringComparison.Ordinal);
}
