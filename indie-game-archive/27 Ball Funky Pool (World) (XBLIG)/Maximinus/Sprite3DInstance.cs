using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Sprite3DInstance
{
	private static Model model;

	private Texture2D tex;

	public static void LoadContent()
	{
		if (model != null)
		{
			throw new Exception("Content already loaded");
		}
		model = MaximinusGame.ContentManager.Load<Model>("Models/Sprite");
	}

	public Sprite3DInstance(Texture2D tex)
	{
		this.tex = tex;
	}

	public void DrawInstances(Matrix[] transforms)
	{
		Drawing3D_V2.DrawModelHWInstances(model, transforms);
	}
}
