using System.Globalization;
using System.Reflection.Emit;

namespace System.Text.RegularExpressions;

internal sealed class CompiledRegexRunnerFactory(DynamicMethod scanMethod, object[] searchValues, CultureInfo culture) : RegexRunnerFactory
{
	private readonly DynamicMethod _scanMethod = scanMethod;

	private readonly object[] _searchValues = searchValues;

	private readonly CultureInfo _culture = culture;

	private CompiledRegexRunner.ScanDelegate _scan;

	protected internal override RegexRunner CreateInstance()
	{
		return new CompiledRegexRunner(_scan ?? (_scan = _scanMethod.CreateDelegate<CompiledRegexRunner.ScanDelegate>()), _searchValues, _culture);
	}
}
