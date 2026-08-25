using System;
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
		Utils.LoadModelFile(modelName, out m_Model, out m_ModelTexture, out m_AbsoluteTransforms, ref m_CollisionSpheres);
		m_ProximitySphere = new BoundingSphere(m_Position, m_Model.Meshes[0].BoundingSphere.Radius * m_Scale);
		m_ProximitySphere.Center.Z = 0f;
		for (int num = m_CollisionSpheres.GetLength(0) - 1; num >= 0; num--)
		{
			BoundingSphere boundingSphere = m_CollisionSpheres[num];
			ref BoundingSphere reference = ref m_CollisionSpheres[num];
			reference = m_CollisionSpheres[num].Transform(Matrix.CreateScale(m_Scale));
			ref BoundingSphere reference2 = ref m_CollisionSpheres[num];
			reference2 = m_CollisionSpheres[num].Transform(Matrix.CreateRotationZ(m_Rotation));
			m_CollisionSpheres[num].Radius = boundingSphere.Radius * m_Scale;
			m_CollisionSpheres[num].Center += m_Position;
			m_CollisionSpheres[num].Center.Z = 0f;
		}
		for (int i = 0; i < m_AbsoluteTransforms.GetLength(0); i++)
		{
			m_AbsoluteTransforms[i] *= Matrix.CreateScale(m_Scale);
			m_AbsoluteTransforms[i] *= Matrix.CreateRotationZ(m_Rotation);
			m_AbsoluteTransforms[i] *= Matrix.CreateTranslation(m_Position);
		}
		if (MainGame.MaxPixelShader < ShaderProfile.PS_3_0)
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
		return m_Position;
	}

	public void Draw()
	{
		foreach (ModelMesh mesh in m_Model.Meshes)
		{
			foreach (Effect effect in mesh.Effects)
			{
				_ = effect;
				m_LightingFXWorld.SetValue(m_AbsoluteTransforms[mesh.ParentBone.Index]);
			}
			mesh.Draw();
		}
	}

	public bool CollisionTest(BoundingSphere testSphere, ref Vector3 collisonNormal)
	{
		if (m_ProximitySphere.Intersects(testSphere))
		{
			BoundingSphere[] collisionSpheres = m_CollisionSpheres;
			for (int i = 0; i < collisionSpheres.Length; i++)
			{
				BoundingSphere boundingSphere = collisionSpheres[i];
				if (boundingSphere.Intersects(testSphere))
				{
					collisonNormal = testSphere.Center - boundingSphere.Center;
					return true;
				}
			}
		}
		return false;
	}

	public bool CollisionTest(Line line)
	{
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
