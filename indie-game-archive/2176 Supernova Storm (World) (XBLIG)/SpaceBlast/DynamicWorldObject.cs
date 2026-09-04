using System;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

internal class DynamicWorldObject
{
	private Model m_Model;

	private Vector3 m_Position;

	private Vector3 m_Direction;

	private Matrix[] m_AbsoluteTransforms;

	public Vector3 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Position;
		}
	}

	public unsafe DynamicWorldObject(XmlNode node)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		string value = node.Attributes["pos"].Value;
		m_Position = Utils.StringToVector3(value);
		string value2 = node.Attributes["rotation"].Value;
		m_Direction = Utils.StringToVector3(value2);
		string value3 = node.Attributes["model"].Value;
		m_Model = MainGame.ContentMan.Load<Model>("Models/" + value3);
		m_AbsoluteTransforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)m_Model.Bones).Count];
		m_Model.CopyAbsoluteBoneTransformsTo(m_AbsoluteTransforms);
		Enumerator enumerator = m_Model.Meshes.GetEnumerator();
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
	}

	public unsafe void Draw()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.CreateTranslation(m_Position);
		Enumerator enumerator = m_Model.Meshes.GetEnumerator();
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
						val2.World = m_AbsoluteTransforms[current.ParentBone.Index] * val;
						val2.View = MainGame.ViewMatrix;
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

	public void Update(GameTime gameTime)
	{
	}

	public bool CollisionTest(BoundingSphere otherSphere)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		BoundingSphere val = default(BoundingSphere);
		((BoundingSphere)(ref val))._002Ector(m_Position, ((ReadOnlyCollection<ModelMesh>)(object)m_Model.Meshes)[0].BoundingSphere.Radius);
		if (((BoundingSphere)(ref val)).Intersects(otherSphere))
		{
			return true;
		}
		return false;
	}
}
