using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;

namespace Skeleton;

public class SkeletonManager
{
	private Joint root;

	private int numJoints;

	public SkeletonManager()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		root = new Joint(0, new Vector2(0f, 0f), null);
		numJoints = 1;
	}

	public void SetWorldScale(float f)
	{
		root.SetWorldScale(f);
	}

	public void SetWorldPosition(Vector2 v)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		root.SetWorldOffset(v);
	}

	public void SetWorldRotation(float f)
	{
		root.SetWorldRotate(f);
	}

	public Joint AddJoint(Vector2 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return AddJoint(pos, root);
	}

	public void ResetJointState()
	{
		root.ResetJointState();
	}

	public Joint AddJoint(Vector2 pos, Joint parent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Joint joint = new Joint(numJoints, pos, parent);
		parent.AddJoint(joint);
		numJoints++;
		return joint;
	}

	public Joint FindJointAtPos(Vector2 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return root.GetJoint(pos);
	}

	public Joint FindJointById(int id)
	{
		return root.GetJoint(id);
	}

	public Joint GetRoot()
	{
		return root;
	}

	public void Draw(GameTime gameTime)
	{
	}

	public int GetNumJoints()
	{
		return numJoints;
	}

	public void DrawBounding()
	{
		root.DrawBounding();
	}

	public void Write(XmlWriter writer)
	{
		root.Write(writer);
	}

	public void Read(SkeletonImportData data)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		JointImportData jointImportData = data.GetRoot();
		root = new Joint(jointImportData.Id, jointImportData.Position, null);
		root.BoundingBoxOffset = jointImportData.Offset;
		root.BoundingBoxScale = jointImportData.Scale;
		numJoints = root.ReadChildJoints(jointImportData) + 1;
	}

	public void Clone(SkeletonManager skel)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		root = new Joint(skel.root.GetJointId(), skel.root.GetRelativePosition(), null);
		numJoints = root.Clone(skel.root);
	}
}
