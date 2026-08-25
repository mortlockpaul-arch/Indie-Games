using System;

namespace JamSouls;

internal class Interpolator
{
	public delegate void OnFinish();

	public OnFinish OnFinishCallBack;

	public float m_StopValue;

	private float m_CurrentValue;

	public bool m_bLoop;

	private bool m_bStarted;

	private float m_Step;

	public Interpolator(float stop, float step, OnFinish FinishCallBack, bool bLoop)
	{
		m_bLoop = bLoop;
		m_StopValue = stop;
		m_CurrentValue = stop;
		OnFinishCallBack = (OnFinish)Delegate.Combine(OnFinishCallBack, FinishCallBack);
		m_Step = step;
		m_bStarted = false;
	}

	public void Reset(float stop, float step, OnFinish FinishCallBack, bool bLoop)
	{
		m_bLoop = bLoop;
		m_StopValue = stop;
		m_CurrentValue = 0f;
		OnFinishCallBack = (OnFinish)Delegate.Combine(OnFinishCallBack, FinishCallBack);
		m_Step = step;
		m_bStarted = false;
	}

	public void Restart()
	{
		m_CurrentValue = 0f;
		m_bStarted = true;
	}

	public void Update(float deltaTime)
	{
		if (!m_bStarted)
		{
			return;
		}
		m_CurrentValue += m_Step * deltaTime;
		if (m_CurrentValue >= m_StopValue)
		{
			m_bStarted = false;
			OnFinishCallBack();
			if (m_bLoop)
			{
				Restart();
			}
		}
	}

	public float GetCurrentValue()
	{
		return m_CurrentValue;
	}

	public void Start()
	{
		m_bStarted = true;
	}

	public void Stop()
	{
		m_bStarted = false;
	}
}
