using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class AreaItemsCls
{
	public BoundingBox bBox;

	public List<ItemCls> items;

	public AreaItemsCls()
	{
		bBox = default(BoundingBox);
		items = new List<ItemCls>();
	}
}
