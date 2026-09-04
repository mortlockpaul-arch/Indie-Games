using System;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

internal class StaticWorldObject
{
	private Model m_Model;

	private Texture2D m_ModelTexture;

	private Texture2D m_DetailTexture;

	private Effect m_LightingFX;

	private EffectParameter m_LightingFXWorld;

	private Vector3 m_Position;

	private float m_Rotation;

	private float m_Scale;

	private Matrix[] m_AbsoluteTransforms;

	private BoundingSphere m_ProximitySphere;

	private BoundingSphere[] m_CollisionSpheres;

	public StaticWorldObject(XmlNode node)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		string value = node.Attributes["position"].Value;
		m_Position = Utils.StringToVector3(value);
		m_Rotation = Convert.ToSingle(node.Attributes["rotation"].Value);
		m_Scale = Convert.ToSingle(node.Attributes["scale"].Value);
		m_DetailTexture = MainGame.ContentMan.Load<Texture2D>("Textures/AsteroidDetail");
		string value2 = node.Attributes["model"].Value;
		LoadModelFile(value2);
	}

	private void LoadModelFile(string modelName)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Invalid comparison between Unknown and I4
		Utils.LoadModelFile(modelName, out m_Model, out m_ModelTexture, out m_AbsoluteTransforms, ref m_CollisionSpheres);
		m_ProximitySphere = new BoundingSphere(m_Position, ((ReadOnlyCollection<ModelMesh>)(object)m_Model.Meshes)[0].BoundingSphere.Radius * m_Scale);
		m_ProximitySphere.Center.Z = 0f;
		for (int num = m_CollisionSpheres.GetLength(0) - 1; num >= 0; num--)
		{
			BoundingSphere val = m_CollisionSpheres[num];
			ref BoundingSphere reference = ref m_CollisionSpheres[num];
			reference = ((BoundingSphere)(ref m_CollisionSpheres[num])).Transform(Matrix.CreateScale(m_Scale));
			ref BoundingSphere reference2 = ref m_CollisionSpheres[num];
			reference2 = ((BoundingSphere)(ref m_CollisionSpheres[num])).Transform(Matrix.CreateRotationZ(m_Rotation));
			m_CollisionSpheres[num].Radius = val.Radius * m_Scale;
			ref BoundingSphere reference3 = ref m_CollisionSpheres[num];
			reference3.Center += m_Position;
			m_CollisionSpheres[num].Center.Z = 0f;
		}
		for (int i = 0; i < m_AbsoluteTransforms.GetLength(0); i++)
		{
			ref Matrix reference4 = ref m_AbsoluteTransforms[i];
			reference4 *= Matrix.CreateScale(m_Scale);
			ref Matrix reference5 = ref m_AbsoluteTransforms[i];
			reference5 *= Matrix.CreateRotationZ(m_Rotation);
			ref Matrix reference6 = ref m_AbsoluteTransforms[i];
			reference6 *= Matrix.CreateTranslation(m_Position);
		}
		if ((int)MainGame.MaxPixelShader < 8)
		{
			m_LightingFX = MainGame.ContentMan.Load<Effect>("Effects/LightingPS2");
		}
		else
		{
			m_LightingFX = MainGame.ContentMan.Load<Effect>("Effects/Lighting");
		}
		m_LightingFXWorld = m_LightingFX.Parameters["World"];
		Utils.RemapModelEffects(m_Model, m_LightingFX, m_ModelTexture, m_DetailTexture);
	}

	public Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_Position;
	}

	public unsafe void Draw()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
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
						_ = ((Enumerator)(ref enumerator2)).Current;
						m_LightingFXWorld.SetValue(m_AbsoluteTransforms[current.ParentBone.Index]);
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

	public bool CollisionTest(BoundingSphere testSphere, ref Vector3 collisonNormal)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (((BoundingSphere)(ref m_ProximitySphere)).Intersects(testSphere))
		{
			BoundingSphere[] collisionSpheres = m_CollisionSpheres;
			for (int i = 0; i < collisionSpheres.Length; i++)
			{
				BoundingSphere val = collisionSpheres[i];
				if (((BoundingSphere)(ref val)).Intersects(testSphere))
				{
					collisonNormal = testSphere.Center - val.Center;
					return true;
				}
			}
		}
		return false;
	}

	public bool CollisionTest(Line line)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (line.Intersects(m_ProximitySphere))
		{
			BoundingSphere[] collisionSpheres = m_CollisionSpheres;
			foreach (BoundingSphere sphere in collisionSpheres)
			{
				if (line.Intersects(sphere))
				{
					MainGame.DebugObj = this;
					return true;
				}
			}
		}
		return false;
	}
}
