namespace Quasar.Utils;

public interface IFuzzyValueParser<T> where T : struct
{
	T DefaultValue { get; }

	T Parse(string field);

	bool NotEmpty(T value);

	T Next(T baseVal, T rndVal);
}
