using System;
using GKEngine.Scenes;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Entity2D : Base2D
{
	public Scene scene;

	public bool visible = true;

	public Color tint = new Color(255, 255, 255, 255);

	public GUID guid;

	public int depth;

	private float _alpha = 1f;

	public float alpha
	{
		get
		{
			return _alpha;
		}
		set
		{
			_alpha = value;
			tint = new Color(tint.R, tint.G, tint.B, Convert.ToByte(255f * _alpha));
		}
	}

	public Entity2D()
	{
		guid = new GUID();
	}

	public virtual void Load()
	{
	}

	public virtual void Dispose()
	{
	}

	public virtual void Render(GameTime oGameTime)
	{
	}

	public virtual void Tint_SetAll(byte xValue)
	{
		tint.A = xValue;
		tint.R = xValue;
		tint.G = xValue;
		tint.B = xValue;
	}
}
