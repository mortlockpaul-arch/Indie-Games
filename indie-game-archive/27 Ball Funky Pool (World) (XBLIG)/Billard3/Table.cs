using System;
using System.Collections.Generic;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Table
{
	public List<Obj> obj;

	public static readonly Matrix CustomTransform = Matrix.CreateRotationY(4.712389f);

	public void LoadContent(ContentManager Content)
	{
		this.obj = new List<Obj>();
		Obj obj = new Obj(Obj.IDenum.TablePlan, Content.Load<Model>("Models/table"));
		obj.Alpha = 1f;
		List<ModelMesh> list = new List<ModelMesh>();
		list.Add(obj.model.Meshes[1]);
		foreach (ModelMesh item in list)
		{
			foreach (ModelMeshPart meshPart in item.MeshParts)
			{
				VertexBuffer vertexBuffer = meshPart.VertexBuffer;
				VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[vertexBuffer.VertexCount];
				vertexBuffer.GetData(array);
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 position = array[i].Position;
					if (Math.Abs(position.X) > 5f)
					{
						array[i].Position.X += 10f * (float)Math.Sign(array[i].Position.X);
					}
					if (Math.Abs(position.Y) > 5f)
					{
						array[i].Position.Y += 5f * (float)Math.Sign(array[i].Position.Y);
					}
				}
				vertexBuffer.SetData(array);
			}
		}
		if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball || MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			ModelMesh modelMesh = obj.model.Meshes[1];
			foreach (BasicEffect effect in modelMesh.Effects)
			{
				if (effect.TextureEnabled && effect.Texture.Width == 2000 && effect.Texture.Height == 1040)
				{
					effect.Texture = Statics.clothAlternate;
				}
				else if (effect.TextureEnabled && effect.Texture.Width == 2048 && effect.Texture.Height == 2048)
				{
					effect.Texture = Statics.trouCentralAlternate;
				}
			}
		}
		this.obj.Add(obj);
	}
}
