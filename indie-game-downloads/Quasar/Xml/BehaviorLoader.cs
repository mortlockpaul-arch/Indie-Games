using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.Behaviors;

namespace Quasar.Xml;

internal static class BehaviorLoader
{
	private static Dictionary<string, Quasar.Xml.BaseBehaviorLoader> factories;

	static BehaviorLoader()
	{
		factories = new Dictionary<string, Quasar.Xml.BaseBehaviorLoader>(4);
		factories.Add("Billboard", new Quasar.Xml.SimpleBehaviorLoader<CilindricalBillboardBehavior>());
		factories.Add("SphericalBillboard", new Quasar.Xml.SimpleBehaviorLoader<SphericalBillboardBehavior>());
	}

	public static Quasar.Xml.BaseBehaviorLoader GetLoader(string name)
	{
		Quasar.Xml.BaseBehaviorLoader value = null;
		factories.TryGetValue(name, out value);
		return value;
	}

	public static Behavior Load(XElement xe)
	{
		return GetLoader(xe.Name.LocalName)?.Load(xe);
	}
}
