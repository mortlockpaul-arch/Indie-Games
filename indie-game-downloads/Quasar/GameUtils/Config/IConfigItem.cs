using System.Xml.Linq;

namespace Quasar.GameUtils.Config;

public interface IConfigItem
{
	string Name { get; }

	void FromXml(XElement xe);

	void ToXml(XElement xe);

	void SetDefaults();

	void Apply();
}
