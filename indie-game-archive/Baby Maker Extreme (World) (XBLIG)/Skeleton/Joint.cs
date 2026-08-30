using System;
using System.Collections.Generic;
using System.Xml;
using ImportData;
using MathTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Skeleton;

public class Joint
{
	private Vector2 position;

	private Vector2 cumulativePosition;

	private float cumulativeRotation;

	private List<Joint> children;

	private Joint parent;

	private Vector2 boxOffset;

	private Vector2 boxSize;

	private int jointId;

	private float worldScale;

	private float worldRotate;

	private Vector2 worldOffset;

	private JointState m_state;

	private bool changeColor;

	public Vector2 BoundingBoxScale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return boxSize;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			boxSize = value;
		}
	}

	public Vector2 BoundingBoxOffset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return boxOffset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			boxOffset = value;
		}
	}

	public Vector2 BoneOffset
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return m_state.Offset;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_state.Offset = value;
		}
	}

	public Vector2 BoneScale
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return m_state.Scale * worldScale;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			m_state.Scale = value;
		}
	}

	public float Rotation
	{
		get
		{
			return m_state.Rotation;
		}
		set
		{
			m_state.Rotation = value;
		}
	}

	public Joint(int jId, Vector2 pos, Joint par)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		jointId = jId;
		position = pos;
		children = new List<Joint>();
		parent = par;
		cumulativePosition = pos;
		cumulativeRotation = 0f;
		m_state = new JointState(0f, new Vector2(1f, 1f), default(Vector2));
		boxOffset = default(Vector2);
		boxSize = default(Vector2);
		RecalculatePosition();
		worldScale = 1f;
		worldRotate = 0f;
		worldOffset = default(Vector2);
	}

	public void SetWorldScale(float f)
	{
		worldScale = f;
		foreach (Joint child in children)
		{
			child.SetWorldScale(f);
		}
	}

	public void SetWorldOffset(Vector2 v)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		worldOffset = v;
	}

	public void SetWorldRotate(float f)
	{
		worldRotate = f;
	}

	public void ResetJointState()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_state.Rotation = 0f;
		m_state.Offset = default(Vector2);
		m_state.Scale = new Vector2(1f, 1f);
		RecalculatePosition();
		foreach (Joint child in children)
		{
			child.ResetJointState();
		}
	}

	public void AddJoint(Joint j)
	{
		if (j.parent != null)
		{
			j.parent.children.Remove(j);
		}
		j.parent = this;
		children.Add(j);
		RecalculatePosition();
	}

	public void MoveJoint(Vector2 v)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		position -= v;
		RecalculatePosition();
	}

	public Joint GetJoint(Vector2 checkPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = checkPosition - cumulativePosition;
		if (((Vector2)(ref val)).Length() <= 10f)
		{
			return this;
		}
		foreach (Joint child in children)
		{
			Joint joint = child.GetJoint(checkPosition);
			if (joint != null)
			{
				return joint;
			}
		}
		return null;
	}

	public Joint GetJoint(int id)
	{
		if (jointId == id)
		{
			return this;
		}
		foreach (Joint child in children)
		{
			Joint joint = child.GetJoint(id);
			if (joint != null)
			{
				return joint;
			}
		}
		return null;
	}

	public void Highlight(bool b)
	{
		changeColor = b;
	}

	public void Rotate(float f)
	{
		m_state.Rotation += f;
		RecalculatePosition();
	}

	public Vector2 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return cumulativePosition;
	}

	public Vector2 GetRelativePosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return position;
	}

	public void DrawBounding()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		Color c = Color.Cyan;
		if (changeColor)
		{
			c = Color.Red;
		}
		if (parent != null)
		{
			SceneRenderer.DrawLine(worldOffset + cumulativePosition, worldOffset + parent.cumulativePosition, c);
		}
		SceneRenderer.DrawPoint(cumulativePosition, c);
		if (((Vector2)(ref boxSize)).LengthSquared() > 0f && parent != null)
		{
			Vector2[] array = (Vector2[])(object)new Vector2[4]
			{
				-boxSize / 2f,
				new Vector2(boxSize.X / 2f, (0f - boxSize.Y) / 2f),
				boxSize / 2f,
				new Vector2((0f - boxSize.X) / 2f, boxSize.Y / 2f)
			};
			float angleFromVector = VectorTools.GetAngleFromVector(cumulativePosition - parent.cumulativePosition);
			Vector2 val = cumulativePosition - parent.cumulativePosition;
			((Vector2)(ref val)).Normalize();
			if (m_state.Scale.X < 0f)
			{
				val = -val;
			}
			float num = ((Vector2)(ref position)).Length();
			Vector2 val2 = val * boxOffset.Y + new Vector2(val.Y, 0f - val.X) * boxOffset.X;
			for (int i = 0; i < 4; i++)
			{
				VectorTools.ScaleAtAngle(angleFromVector, m_state.Scale, val * num / 2f + val2, out var _);
				VectorTools.Rotate(ref array[i], angleFromVector, out array[i]);
				ref Vector2 reference = ref array[i];
				reference = parent.cumulativePosition + val + array[i];
			}
			SceneRenderer.DrawLine(array[0], array[1], c);
			SceneRenderer.DrawLine(array[1], array[2], c);
			SceneRenderer.DrawLine(array[2], array[3], c);
			SceneRenderer.DrawLine(array[3], array[0], c);
		}
		foreach (Joint child in children)
		{
			child.DrawBounding();
		}
	}

	public Vector2 ConvertToLocalizedPosition(Vector2 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		pos -= GetPosition();
		VectorTools.Rotate(ref pos, cumulativeRotation, out var result);
		result.X = (float)(Math.Cos(0f - cumulativeRotation) * (double)pos.X - Math.Sin(0f - cumulativeRotation) * (double)pos.Y);
		result.Y = (float)(Math.Sin(0f - cumulativeRotation) * (double)pos.X + Math.Cos(0f - cumulativeRotation) * (double)pos.Y);
		return result;
	}

	public Joint GetParent()
	{
		return parent;
	}

	public List<Joint> GetChildren()
	{
		return children;
	}

	public void RecalculatePosition()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (parent != null)
		{
			cumulativePosition = position;
			Vector2 vec = BoneScale.X * (position + m_state.Offset);
			VectorTools.Rotate(ref vec, parent.cumulativeRotation, out cumulativePosition);
			cumulativePosition = parent.cumulativePosition + cumulativePosition;
			cumulativeRotation = parent.cumulativeRotation + Rotation;
		}
		else
		{
			cumulativePosition = position + m_state.Offset + worldOffset;
			cumulativeRotation = Rotation + worldRotate;
		}
		foreach (Joint child in children)
		{
			child.RecalculatePosition();
		}
	}

	public JointState GetJointState()
	{
		return m_state;
	}

	public void SetState(JointState state)
	{
		m_state = state;
	}

	public void DisconnectChild(Joint joint)
	{
		children.Remove(joint);
	}

	public float GetCumulativeRotation()
	{
		return cumulativeRotation;
	}

	public void Write(XmlWriter writer)
	{
		writer.WriteStartElement("joint", "1");
		writer.WriteElementString("id", jointId.ToString());
		writer.WriteElementString("posX", position.X.ToString());
		writer.WriteElementString("posY", position.Y.ToString());
		writer.WriteElementString("boxOffsetX", boxOffset.X.ToString());
		writer.WriteElementString("boxOffsetY", boxOffset.Y.ToString());
		writer.WriteElementString("boxSizeX", boxSize.X.ToString());
		writer.WriteElementString("boxSizeY", boxSize.Y.ToString());
		foreach (Joint child in children)
		{
			child.Write(writer);
		}
		writer.WriteEndElement();
	}

	public int ReadChildJoints(XmlReader reader)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Joint joint = null;
		Vector2 pos = default(Vector2);
		Vector2 boundingBoxOffset = default(Vector2);
		Vector2 boundingBoxScale = default(Vector2);
		while (reader.IsStartElement() && reader.Name.Equals("joint"))
		{
			reader.ReadStartElement("joint");
			int jId = int.Parse(reader.ReadElementString("id"));
			((Vector2)(ref pos))._002Ector(float.Parse(reader.ReadElementString("posX")), float.Parse(reader.ReadElementString("posY")));
			((Vector2)(ref boundingBoxOffset))._002Ector(float.Parse(reader.ReadElementString("boxOffsetX")), float.Parse(reader.ReadElementString("boxOffsetY")));
			((Vector2)(ref boundingBoxScale))._002Ector(float.Parse(reader.ReadElementString("boxSizeX")), float.Parse(reader.ReadElementString("boxSizeY")));
			joint = new Joint(jId, pos, this);
			AddJoint(joint);
			joint.BoundingBoxOffset = boundingBoxOffset;
			joint.BoundingBoxScale = boundingBoxScale;
			num++;
			if (reader.IsStartElement())
			{
				num += joint.ReadChildJoints(reader);
			}
			reader.ReadEndElement();
		}
		return num;
	}

	public int ReadChildJoints(JointImportData j)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Joint joint = null;
		foreach (JointImportData child in j.Children)
		{
			joint = new Joint(child.Id, child.Position, this);
			AddJoint(joint);
			joint.BoundingBoxOffset = child.Offset;
			joint.BoundingBoxScale = child.Scale;
			num++;
			if (child.Children.Count > 0)
			{
				num += joint.ReadChildJoints(child);
			}
		}
		return num;
	}

	public int GetJointId()
	{
		return jointId;
	}

	public int Clone(Joint j)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		int num = 1;
		boxSize = j.boxSize;
		boxOffset = j.boxOffset;
		children.Clear();
		foreach (Joint child in j.children)
		{
			Joint joint = new Joint(child.jointId, child.position, this);
			AddJoint(joint);
			num += joint.Clone(child);
		}
		return num;
	}
}
