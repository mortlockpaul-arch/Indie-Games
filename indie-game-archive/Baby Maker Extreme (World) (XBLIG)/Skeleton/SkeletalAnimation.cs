using System.Collections.Generic;
using System.Xml;
using ImportData;

namespace Skeleton;

public class SkeletalAnimation
{
	private List<AnimBone> m_lAnimBones;

	private float m_animSpeed;

	public float AnimSpeed
	{
		get
		{
			return m_animSpeed;
		}
		set
		{
			m_animSpeed = value;
		}
	}

	public SkeletalAnimation(SkeletonManager skelManager)
	{
		m_lAnimBones = new List<AnimBone>();
		Joint root = skelManager.GetRoot();
		m_animSpeed = 1f;
		BuildJointAnims(root);
	}

	private void BuildJointAnims(Joint j)
	{
		m_lAnimBones.Add(new AnimBone(j));
		StoreJointState(j, -1);
		StoreJointState(j, 100000);
		foreach (Joint child in j.GetChildren())
		{
			BuildJointAnims(child);
		}
	}

	public void StoreJointState(Joint j, int time)
	{
		foreach (AnimBone lAnimBone in m_lAnimBones)
		{
			if (lAnimBone.GetJoint() == j)
			{
				lAnimBone.AddTimedState(j.GetJointState(), time);
			}
		}
	}

	public void StoreNonzeroJointStates(int time)
	{
		foreach (AnimBone lAnimBone in m_lAnimBones)
		{
			if (lAnimBone.GetJoint().GetChildren().Count > 0 || lAnimBone.GetJoint().GetParent().GetParent() == null)
			{
				lAnimBone.AddTimedState(lAnimBone.GetJoint().GetJointState(), time);
			}
		}
	}

	public void Update(TimeTracker gameTime)
	{
		foreach (AnimBone lAnimBone in m_lAnimBones)
		{
			lAnimBone.Update(gameTime, m_animSpeed);
		}
	}

	public void SetTimeFrame(int timer)
	{
		foreach (AnimBone lAnimBone in m_lAnimBones)
		{
			lAnimBone.SetTimeFrame(timer);
		}
	}

	public List<AnimBone> GetAnimBones()
	{
		return m_lAnimBones;
	}

	public void Write(XmlWriter writer)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		foreach (AnimBone lAnimBone in m_lAnimBones)
		{
			writer.WriteStartElement("joint", "1");
			List<TimedJointState> timedJointStates = lAnimBone.GetTimedJointStates();
			foreach (TimedJointState item in timedJointStates)
			{
				writer.WriteElementString("time", item.ActivationTime.ToString());
				writer.WriteElementString("rotation", item.State.Rotation.ToString());
				float x = item.State.Offset.X;
				writer.WriteElementString("offsetX", x.ToString());
				float y = item.State.Offset.Y;
				writer.WriteElementString("offsetY", y.ToString());
				float x2 = item.State.Scale.X;
				writer.WriteElementString("scaleX", x2.ToString());
				float y2 = item.State.Scale.Y;
				writer.WriteElementString("scaleY", y2.ToString());
			}
			writer.WriteEndElement();
		}
	}

	public void Read(AnimImportData data)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		List<BoneImportData> boneData = data.GetBoneData();
		int num = 0;
		foreach (BoneImportData item in boneData)
		{
			List<TimedJointState> timedJointStates = m_lAnimBones[num].GetTimedJointStates();
			timedJointStates.Clear();
			foreach (BoneStateImportData jointState in item.GetJointStates())
			{
				m_lAnimBones[num].AddTimedState(new JointState(jointState.Rotation, jointState.Scale, jointState.Offset), jointState.Time);
			}
			num++;
		}
	}

	public void Clone(SkeletalAnimation anim, int loadOffset)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (AnimBone lAnimBone in m_lAnimBones)
		{
			List<TimedJointState> timedJointStates = lAnimBone.GetTimedJointStates();
			timedJointStates.Clear();
			lAnimBone.ClearTimedStates();
			if (loadOffset < 0)
			{
				TimedJointState nextReservedState = lAnimBone.GetNextReservedState();
				nextReservedState.ActivationTime = loadOffset;
				nextReservedState.State.Rotation = lAnimBone.GetJoint().GetJointState().Rotation;
				nextReservedState.State.Scale = lAnimBone.GetJoint().GetJointState().Scale;
				nextReservedState.State.Offset = lAnimBone.GetJoint().GetJointState().Offset;
				timedJointStates.Add(nextReservedState);
				lAnimBone.SetTimeFrame(loadOffset);
			}
			List<TimedJointState> timedJointStates2 = anim.m_lAnimBones[num].GetTimedJointStates();
			foreach (TimedJointState item in timedJointStates2)
			{
				if (item.ActivationTime >= 0 || loadOffset >= 0)
				{
					TimedJointState nextReservedState2 = lAnimBone.GetNextReservedState();
					nextReservedState2.ActivationTime = item.ActivationTime;
					nextReservedState2.State.Rotation = item.State.Rotation;
					nextReservedState2.State.Scale = item.State.Scale;
					nextReservedState2.State.Offset = item.State.Offset;
					timedJointStates.Add(nextReservedState2);
				}
			}
			num++;
			lAnimBone.SetTimeFrame(loadOffset);
		}
	}
}
