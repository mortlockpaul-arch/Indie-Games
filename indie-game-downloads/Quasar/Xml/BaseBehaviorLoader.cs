using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.Xml;

internal abstract class BaseBehaviorLoader
{
	public abstract Behavior Load(XElement xe);

	protected void LoadCommonData(Behavior e, XElement xe)
	{
		e.Enabled = XDocHelper.ParseBoolAttribute(xe, "enabled", defaultValue: true);
	}
}
