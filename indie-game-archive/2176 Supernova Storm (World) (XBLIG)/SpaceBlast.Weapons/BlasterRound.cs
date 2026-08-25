using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Weapons;

internal class BlasterRound : WeaponRound
{
	private const int constDamage = 15;

	private Vector3 m_Position;

	private Vector3 m_Velocity;

	private double m_Expires;

	private static Model s_Model = null;

	private static Matrix[] s_Transforms;

	public BlasterRound(Vector3 position, Vector3 velocity, double expires)
	{
		m_Position = position;
		m_Velocity = velocity;
		m_Expires = expires;
	}

	public static void SetupStatics()
	{
		if (s_Model != null)
		{
			return;
		}
		s_Model = MainGame.ContentMan.Load<Model>("Models/Blaster");
		foreach (ModelMesh mesh in s_Model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.EnableDefaultLighting();
				effect.Projection = MainGame.ProjectionMatrix;
				effect.View = MainGame.ViewMatrix;
			}
		}
		s_Transforms = new Matrix[s_Model.Bones.Count];
		s_Model.CopyAbsoluteBoneTransformsTo(s_Transforms);
	}

	public override bool Update()
	{
		m_Position += m_Velocity * 60f * (float)TimeManager.DeltaSeconds;
		if (m_Position.X < 0f || m_Position.X > (float)MainGame.LevelData.WorldWidth || m_Position.Y < 0f || m_Position.Y > (float)MainGame.LevelData.WorldHeight || TimeManager.TotalSeconds > m_Expires)
		{
			return false;
		}
		return true;
	}

	public override void Draw()
	{
		Matrix matrix = Matrix.CreateTranslation(m_Position);
		foreach (ModelMesh mesh in s_Model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.World = s_Transforms[mesh.ParentBone.Index] * matrix;
				effect.View = MainGame.ViewMatrix;
				effect.Projection = MainGame.ProjectionMatrix;
			}
			mesh.Draw();
		}
	}

	public override BoundingSphere GetBoundingSphere()
	{
		return new BoundingSphere(m_Position, s_Model.Meshes[0].BoundingSphere.Radius);
	}

	public override int GetHitDamage()
	{
		return 15;
	}
}
