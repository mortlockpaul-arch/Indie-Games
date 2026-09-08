using System.Globalization;

namespace System.Text.RegularExpressions;

internal sealed class CompiledRegexRunner(CompiledRegexRunner.ScanDelegate scan, object[] searchValues, CultureInfo culture) : RegexRunner
{
	internal delegate void ScanDelegate(RegexRunner runner, ReadOnlySpan<char> text);

	private readonly ScanDelegate _scanMethod = scan;

	private readonly object[] _searchValues = searchValues;

	private readonly CultureInfo _culture = culture;

	private RegexCaseBehavior _caseBehavior;

	protected internal override void Scan(ReadOnlySpan<char> text)
	{
		_scanMethod(this, text);
	}
}
