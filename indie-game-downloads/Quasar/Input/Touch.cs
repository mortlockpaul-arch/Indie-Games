using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;
using Quasar.Global;

namespace Quasar.Input;

public class Touch
{
	public TouchCollection TouchState;

	public readonly List<GestureSample> Gestures = new List<GestureSample>();

	private static Touch instance;

	private Dictionary<int, object> touchReserves = new Dictionary<int, object>(10);

	public static Touch Instance => instance;

	public bool TouchAvailable(int index, object reserveObject)
	{
		if (TouchState.Count <= index)
		{
			return false;
		}
		if (touchReserves.TryGetValue(TouchState[index].Id, out var value))
		{
			return value == reserveObject;
		}
		return true;
	}

	public bool TouchIdAvailable(int id, object reserveObject)
	{
		if (touchReserves.TryGetValue(id, out var value))
		{
			return value == reserveObject;
		}
		return true;
	}

	public void ReserveTouch(int index, object reserveObject)
	{
		if (TouchState.Count > index && !touchReserves.ContainsKey(TouchState[index].Id))
		{
			touchReserves.Add(TouchState[index].Id, reserveObject);
		}
	}

	public void ReserveTouchId(int id, object reserveObject)
	{
		if (!touchReserves.ContainsKey(id))
		{
			touchReserves.Add(id, reserveObject);
		}
	}

	static Touch()
	{
		instance = new Touch();
	}

	private Touch()
	{
		TouchState = TouchPanel.GetState();
		TouchPanel.EnabledGestures = GestureType.None;
		TouchPanel.DisplayWidth = Engine.BackBufferWidth;
		TouchPanel.DisplayHeight = Engine.BackBufferHeight;
	}

	public void UpdateStatus()
	{
		foreach (TouchLocation item in TouchState)
		{
			if (item.State == TouchLocationState.Released)
			{
				touchReserves.Remove(item.Id);
			}
		}
		TouchState = TouchPanel.GetState();
		Gestures.Clear();
		while (TouchPanel.IsGestureAvailable)
		{
			Gestures.Add(TouchPanel.ReadGesture());
		}
	}
}
