using System.Xml.Linq;

namespace Quasar.GameUtils.Config;

public interface IValueParser<T>
{
	T Parse(XElement element, T defaultValue);

	void ToXml(XElement element, T value);
}
