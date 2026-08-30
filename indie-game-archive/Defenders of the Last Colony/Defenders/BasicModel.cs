using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class BasicModel
{
	protected Matrix world = Matrix.Identity;

	public Model model { get; protected set; }

	public BasicModel(Model m)
	{
		model = m;
	}

	public virtual void Update()
	{
	}

	public void Draw(Camera3D camera3D)
	{
		Matrix[] destinationBoneTransforms = new Matrix[model.Bones.Count];
		model.CopyAbsoluteBoneTransformsTo(destinationBoneTransforms);
		foreach (ModelMesh mesh in model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.EnableDefaultLighting();
				effect.Projection = camera3D.projection;
				effect.View = camera3D.view;
				effect.World = GetWorld() * mesh.ParentBone.Transform;
			}
			mesh.Draw();
		}
	}

	public virtual Matrix GetWorld()
	{
		return world;
	}
}
