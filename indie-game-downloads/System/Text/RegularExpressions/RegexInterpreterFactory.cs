using System.Globalization;

namespace System.Text.RegularExpressions;

internal sealed class RegexInterpreterFactory(RegexTree tree) : RegexRunnerFactory
{
	private readonly RegexInterpreterCode _code = RegexWriter.Write(tree);

	private readonly CultureInfo _culture = tree.Culture;

	protected internal override RegexRunner CreateInstance()
	{
		return new RegexInterpreter(_code, _culture);
	}
}
