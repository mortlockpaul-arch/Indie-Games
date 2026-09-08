namespace Quasar.Utils;

public interface IKeyedValueParser<T> where T : struct
{
	void Lerp(ref T from, ref T to, float value, out T result);

	T Parse(string field);
}
