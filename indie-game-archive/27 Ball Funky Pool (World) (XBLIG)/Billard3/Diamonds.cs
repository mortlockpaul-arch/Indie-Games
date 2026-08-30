using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Diamonds
{
	private Obj obj;

	private Matrix startMat;

	private List<Vector3> pos = new List<Vector3>();

	public Diamonds(Game game)
	{
		obj = new Obj((Obj.IDenum)(-1), game.Content.Load<Model>("Models/diamonds"));
		obj.SpecularColor.Add(Color.Gold);
		obj.SpecularPower = 40;
		startMat = Draws.defaultMat * Matrix.CreateRotationY((float)Math.PI / 4f) * Matrix.CreateScale(0.3f) * Matrix.CreateTranslation(Vector3.Up * 0.932f);
		foreach (float item in new List<float> { -22.5f, -15f, -7.5f, 7.5f, 15f, 22.5f })
		{
			float num = item;
			foreach (float item2 in new List<float> { -31.833f, 31.833f })
			{
				float num2 = item2;
				pos.Add(Vector3.UnitX * num + Vector3.UnitZ * num2);
			}
		}
		foreach (float item3 in new List<float> { -31.833f, 31.833f })
		{
			float num3 = item3;
			foreach (float item4 in new List<float> { -15f, 0f, 15f })
			{
				float num4 = item4;
				pos.Add(Vector3.UnitX * num3 + Vector3.UnitZ * num4);
			}
		}
	}

	public void Draw(GameTime gameTime)
	{
		foreach (Vector3 po in pos)
		{
			foreach (ModelMesh mesh in obj.model.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.World = startMat * Matrix.CreateTranslation(po);
					effect.View = Statics.cam.ViewMatrix;
					effect.Projection = Statics.cam.ProjMatrix;
					effect.LightingEnabled = true;
					effect.DirectionalLight0.Enabled = true;
					effect.DirectionalLight0.DiffuseColor = Color.Gold.ToVector3();
					effect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.Down + Vector3.UnitX * -2f) * 1f;
					effect.DirectionalLight1.Enabled = true;
					effect.DirectionalLight1.DiffuseColor = Color.Gold.ToVector3();
					effect.DirectionalLight1.Direction = Vector3.Normalize(Vector3.Down + Vector3.UnitX * 2f) * 1f;
					effect.DirectionalLight2.Enabled = false;
				}
				mesh.Draw();
			}
		}
	}
}
