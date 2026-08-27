using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class AreaItemsCls
{
	public BoundingBox bBox = default(BoundingBox);

	public List<ItemCls> items = new List<ItemCls>();
}
