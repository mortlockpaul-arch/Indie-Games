using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Weapons;

internal class GunRound : WeaponRound
{
	private const int constGunDamage = 10;

	private Vector3 m_Position;

	private Vector3 m_Velocity;

	private double m_Expires;

	private static Model s_Model = null;

	private static Matrix[] s_Transforms;

	public GunRound(Vector3 position, Vector3 velocity, double expires)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_Position = position;
		m_Velocity = velocity;
		m_Expires = expires;
	}

	public unsafe static void SetupStatics()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (s_Model != null)
		{
			return;
		}
		s_Model = MainGame.ContentMan.Load<Model>("Models/Bullet");
		Enumerator enumerator = s_Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.Projection = MainGame.ProjectionMatrix;
						val.View = MainGame.ViewMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		s_Transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)s_Model.Bones).Count];
		s_Model.CopyAbsoluteBoneTransformsTo(s_Transforms);
	}

	public override bool Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_Position += m_Velocity * 60f * (float)TimeManager.DeltaSeconds;
		if (m_Position.X < 0f || m_Position.X > (float)MainGame.LevelData.WorldWidth || m_Position.Y < 0f || m_Position.Y > (float)MainGame.LevelData.WorldHeight || TimeManager.TotalSeconds > m_Expires)
		{
			return false;
		}
		return true;
	}

	public unsafe override void Draw()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.CreateTranslation(m_Position);
		Enumerator enumerator = s_Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val2 = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val2.World = s_Transforms[current.ParentBone.Index] * val;
						val2.View = MainGame.ViewMatrix;
						val2.Projection = MainGame.ProjectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public override BoundingSphere GetBoundingSphere()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new BoundingSphere(m_Position, ((ReadOnlyCollection<ModelMesh>)(object)s_Model.Meshes)[0].BoundingSphere.Radius);
	}

	public override int GetHitDamage()
	{
		return 10;
	}
}
