using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SgMotion.Effects;

public class PointLightCollection : ReadOnlyCollection<PointLight>
{
	public PointLightCollection(IList<PointLight> list)
		: base(list)
	{
	}
}
