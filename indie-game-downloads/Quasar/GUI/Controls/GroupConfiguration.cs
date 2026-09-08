using System;
using Microsoft.Xna.Framework;

namespace Quasar.GUI.Controls;

public class GroupConfiguration : IDisposable
{
	private GroupNavigateMode navigateMode;

	private bool loop = true;

	private Vector2 position;

	private float margin;

	private GroupHorizontalLayoutMode horizontalMode = GroupHorizontalLayoutMode.Right;

	public GroupNavigateMode NavigateMode
	{
		get
		{
			return navigateMode;
		}
		set
		{
			navigateMode = value;
		}
	}

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public float Margin
	{
		get
		{
			return margin;
		}
		set
		{
			margin = value;
		}
	}

	public GroupHorizontalLayoutMode HorizontalMode
	{
		get
		{
			return horizontalMode;
		}
		set
		{
			horizontalMode = value;
		}
	}

	public void Dispose()
	{
	}
}
