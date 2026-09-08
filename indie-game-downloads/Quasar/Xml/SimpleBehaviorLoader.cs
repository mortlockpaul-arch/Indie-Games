using System.Xml.Linq;

namespace Quasar.Xml;

internal class SimpleBehaviorLoader<T> : Quasar.Xml.BaseBehaviorLoader where T : Behavior, new()
{
	public override Behavior Load(XElement xe)
	{
		T val = new T();
		LoadCommonData(val, xe);
		return val;
	}
}
