using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Skeleton;

public class AnimBone
{
	private Joint bone;

	private List<TimedJointState> m_lJointStates;

	private List<TimedJointState> m_lReservedJointStates;

	private int m_iReservedIndex;

	private int m_iActiveIndex;

	private int m_iCurrentTimerState;

	private List<TimedJointState> delStates;

	private JointState mergedState;

	public AnimBone(Joint j)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		delStates = new List<TimedJointState>(30);
		mergedState = new JointState(0f, default(Vector2), default(Vector2));
		base._002Ector();
		m_iReservedIndex = 0;
		bone = j;
		m_lJointStates = new List<TimedJointState>(30);
		m_iActiveIndex = 0;
		m_iCurrentTimerState = 0;
		m_lReservedJointStates = new List<TimedJointState>(30);
		for (int i = 0; i < m_lReservedJointStates.Capacity; i++)
		{
			m_lReservedJointStates.Add(new TimedJointState(new JointState(0f, default(Vector2), default(Vector2)), 0));
		}
	}

	public Joint GetJoint()
	{
		return bone;
	}

	public List<TimedJointState> GetTimedJointStates()
	{
		m_iActiveIndex = 0;
		return m_lJointStates;
	}

	public void ClearTimedStates()
	{
		m_iReservedIndex = 0;
		m_lJointStates.Clear();
	}

	public TimedJointState GetNextReservedState()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		m_iReservedIndex++;
		while (m_iReservedIndex >= m_lReservedJointStates.Count())
		{
			m_lReservedJointStates.Add(new TimedJointState(new JointState(0f, default(Vector2), default(Vector2)), 0));
		}
		return m_lReservedJointStates[m_iReservedIndex - 1];
	}

	public void AddTimedState(JointState state, int time)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		delStates.Clear();
		foreach (TimedJointState lJointState in m_lJointStates)
		{
			if (lJointState.ActivationTime == time)
			{
				delStates.Add(lJointState);
			}
		}
		foreach (TimedJointState delState in delStates)
		{
			m_lJointStates.Remove(delState);
		}
		m_lJointStates.Add(new TimedJointState(new JointState(state.Rotation, state.Scale, state.Offset), time));
		m_lJointStates.Sort(new TimedJointStateComparer());
		UpdateBoneState(m_lJointStates.Count);
	}

	public void Update(TimeTracker gameTime, float animSpeed)
	{
		int count = m_lJointStates.Count;
		if (count > 2)
		{
			m_iCurrentTimerState += (int)((float)gameTime.ElapsedMilli * animSpeed);
			int activationTime = m_lJointStates[count - 2].ActivationTime;
			if (m_iCurrentTimerState > activationTime)
			{
				m_iCurrentTimerState -= activationTime;
				while (m_iActiveIndex > 0 && m_iCurrentTimerState < m_lJointStates[m_iActiveIndex].ActivationTime)
				{
					m_iActiveIndex--;
				}
			}
		}
		UpdateBoneState(count);
	}

	public void SetTimeFrame(int timerSetting)
	{
		m_iCurrentTimerState = timerSetting;
		UpdateBoneState(m_lJointStates.Count);
	}

	private void UpdateBoneState(int jointcount)
	{
		if (jointcount > 1)
		{
			while (m_iActiveIndex < jointcount - 1 && m_iCurrentTimerState > m_lJointStates[m_iActiveIndex + 1].ActivationTime)
			{
				m_iActiveIndex++;
			}
			m_lJointStates[m_iActiveIndex + 1].State.Merge(m_lJointStates[m_iActiveIndex].State, ((float)m_iCurrentTimerState - (float)m_lJointStates[m_iActiveIndex].ActivationTime) / ((float)m_lJointStates[m_iActiveIndex + 1].ActivationTime - (float)m_lJointStates[m_iActiveIndex].ActivationTime), ref mergedState);
			bone.SetState(mergedState);
		}
	}
}
