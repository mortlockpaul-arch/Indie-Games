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

	public Vector3 Position => m_Position;

	public DynamicWorldObject(XmlNode node)
	{
		string value = node.Attributes["pos"].Value;
		m_Position = Utils.StringToVector3(value);
		string value2 = node.Attributes["rotation"].Value;
		m_Direction = Utils.StringToVector3(value2);
		string value3 = node.Attributes["model"].Value;
		m_Model = MainGame.ContentMan.Load<Model>("Models/" + value3);
		m_AbsoluteTransforms = new Matrix[m_Model.Bones.Count];
		m_Model.CopyAbsoluteBoneTransformsTo(m_AbsoluteTransforms);
		foreach (ModelMesh mesh in m_Model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.EnableDefaultLighting();
				effect.Projection = MainGame.ProjectionMatrix;
				effect.View = MainGame.ViewMatrix;
			}
		}
	}

	public void Draw()
	{
		Matrix matrix = Matrix.CreateTranslation(m_Position);
		foreach (ModelMesh mesh in m_Model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.World = m_AbsoluteTransforms[mesh.ParentBone.Index] * matrix;
				effect.View = MainGame.ViewMatrix;
			}
			mesh.Draw();
		}
	}

	public void Update(GameTime gameTime)
	{
	}

	public bool CollisionTest(BoundingSphere otherSphere)
	{
		if (new BoundingSphere(m_Position, m_Model.Meshes[0].BoundingSphere.Radius).Intersects(otherSphere))
		{
			return true;
		}
		return false;
	}
}
