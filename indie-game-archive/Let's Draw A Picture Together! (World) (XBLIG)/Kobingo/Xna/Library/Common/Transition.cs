using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library.Common;

public class Transition<T>
{
	private class State<U>
	{
		public U Value { get; private set; }

		public TimeSpan LifeTime { get; private set; }

		public TimeSpan ElapsedLifeTime { get; private set; }

		public TimeSpan TransitionTime { get; private set; }

		public TimeSpan ElapsedTransitionTime { get; private set; }

		public bool TransitionWait { get; private set; }

		public float Transition
		{
			get
			{
				if (TransitionTime <= TimeSpan.Zero)
				{
					return 1f;
				}
				return (float)(ElapsedTransitionTime.TotalSeconds / TransitionTime.TotalSeconds);
			}
		}

		public State(U value, TimeSpan transitionTime, bool transitionWait, TimeSpan lifetime)
		{
			Value = value;
			TransitionTime = transitionTime;
			TransitionWait = transitionWait;
			LifeTime = lifetime;
		}

		public void Update(GameTime gameTime, bool current)
		{
			if (LifeTime > TimeSpan.Zero && ElapsedLifeTime >= LifeTime)
			{
				ElapsedTransitionTime -= gameTime.ElapsedGameTime;
				if (ElapsedTransitionTime < TimeSpan.Zero)
				{
					ElapsedTransitionTime = TimeSpan.Zero;
				}
			}
			else if (current)
			{
				ElapsedTransitionTime += gameTime.ElapsedGameTime;
				if (ElapsedTransitionTime > TransitionTime)
				{
					ElapsedTransitionTime = TransitionTime;
				}
			}
			else
			{
				ElapsedTransitionTime -= gameTime.ElapsedGameTime;
				if (ElapsedTransitionTime < TimeSpan.Zero)
				{
					ElapsedTransitionTime = TimeSpan.Zero;
				}
			}
			if (Transition >= 1f)
			{
				ElapsedLifeTime += gameTime.ElapsedGameTime;
			}
		}

		public void Reset()
		{
			ElapsedLifeTime = TimeSpan.Zero;
		}
	}

	private List<State<T>> InternalStates { get; set; }

	public T Current
	{
		get
		{
			if (InternalStates.Count > 0)
			{
				return InternalStates[InternalStates.Count - 1].Value;
			}
			return default(T);
		}
	}

	public IEnumerable<TransitionState<T>> States
	{
		get
		{
			for (int i = 0; i < InternalStates.Count; i++)
			{
				yield return new TransitionState<T>
				{
					Transition = InternalStates[i].Transition,
					Value = InternalStates[i].Value
				};
			}
		}
	}

	public Transition()
	{
		InternalStates = new List<State<T>>();
	}

	public void Update(GameTime gameTime)
	{
		for (int num = InternalStates.Count - 1; num >= 0; num--)
		{
			State<T> state = InternalStates[num];
			if (num < InternalStates.Count - 1)
			{
				state.Update(gameTime, current: false);
				if (state.Transition <= 0f)
				{
					InternalStates.RemoveAt(num);
				}
			}
			else if (!state.TransitionWait || InternalStates.Count == 1)
			{
				state.Update(gameTime, current: true);
				if (state.Transition <= 0f)
				{
					InternalStates.RemoveAt(num);
				}
			}
		}
	}

	public void Change(T state, TimeSpan time, bool wait)
	{
		Change(state, time, wait, TimeSpan.Zero);
	}

	public void Change(T state, TimeSpan time, bool wait, TimeSpan life)
	{
		InternalStates.Add(new State<T>(state, time, wait, life));
	}

	public void ResetCurrent()
	{
		if (InternalStates.Count > 0)
		{
			InternalStates[InternalStates.Count - 1].Reset();
		}
	}

	public void Clear()
	{
		InternalStates.Clear();
	}
}
