using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GKEngine.Input;

public class InputEntity
{
	public enum Type
	{
		Button,
		Analog1D,
		Analog2D
	}

	public enum Scope
	{
		Game,
		Scene
	}

	private Vector2 _vector2_analog2D = default(Vector2);

	private Vector2 _vector2_analog2D_previous = default(Vector2);

	public bool active;

	public Type type;

	public Scope scope;

	public string name = "";

	public List<InputButton> buttons = new List<InputButton>();

	private int buttonsCount;

	public List<InputAnalog1D> analog1D = new List<InputAnalog1D>();

	private int analog1DCount;

	public List<InputAnalog2D> analog2D = new List<InputAnalog2D>();

	private int analog2DCount;

	public bool pressed
	{
		get
		{
			bool result = false;
			if (type == Type.Button)
			{
				for (int i = 0; i < buttonsCount; i++)
				{
					if (buttons[i].pressed)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		set
		{
			if (type == Type.Button)
			{
				for (int i = 0; i < buttonsCount; i++)
				{
					buttons[i].pressed = value;
				}
			}
		}
	}

	public bool downed
	{
		get
		{
			bool result = false;
			if (type == Type.Button)
			{
				for (int i = 0; i < buttonsCount; i++)
				{
					if (buttons[i].downed)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		set
		{
			if (type == Type.Button)
			{
				for (int i = 0; i < buttonsCount; i++)
				{
					buttons[i].downed = value;
				}
			}
		}
	}

	public bool isDown
	{
		get
		{
			bool result = false;
			if (type == Type.Button)
			{
				for (int i = 0; i < buttonsCount; i++)
				{
					if (buttons[i].isDown)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}

	public float value1D
	{
		get
		{
			float result = 0f;
			if (type == Type.Analog1D)
			{
				for (int i = 0; i < analog1DCount; i++)
				{
					if (analog1D[i].value >= 0f || (analog1D[i].type == InputAnalog1D.Type.Mouse && analog1D[i].value != 0f))
					{
						result = analog1D[i].value;
						break;
					}
				}
			}
			return result;
		}
		set
		{
			if (type == Type.Analog1D)
			{
				for (int i = 0; i < analog1DCount; i++)
				{
					analog1D[i].value = value;
				}
			}
		}
	}

	public float previous1D
	{
		get
		{
			float result = 0f;
			if (type == Type.Analog1D)
			{
				for (int i = 0; i < analog1DCount; i++)
				{
					if (analog1D[i].previous >= 0f || (analog1D[i].type == InputAnalog1D.Type.Mouse && analog1D[i].previous != 0f))
					{
						result = analog1D[i].previous;
						break;
					}
				}
			}
			return result;
		}
		set
		{
			if (type == Type.Analog1D)
			{
				for (int i = 0; i < analog1DCount; i++)
				{
					analog1D[i].previous = value;
				}
			}
		}
	}

	public Vector2 value2D
	{
		get
		{
			_vector2_analog2D.X = 0f;
			_vector2_analog2D.Y = 0f;
			if (type == Type.Analog2D)
			{
				for (int i = 0; i < analog2DCount; i++)
				{
					if (analog2D[i].value.X >= -1f && analog2D[i].value.Y >= -1f)
					{
						_vector2_analog2D.X = analog2D[i].value.X;
						_vector2_analog2D.Y = analog2D[i].value.Y;
						break;
					}
				}
			}
			return _vector2_analog2D;
		}
		set
		{
			if (type == Type.Analog2D)
			{
				for (int i = 0; i < analog2DCount; i++)
				{
					analog2D[i].value = value;
				}
			}
		}
	}

	public Vector2 previous2D
	{
		get
		{
			_vector2_analog2D.X = 0f;
			_vector2_analog2D.Y = 0f;
			if (type == Type.Analog2D)
			{
				int num = analog2DCount;
				for (int i = 0; i < num; i++)
				{
					if (analog2D[i].previous.X >= -1f && analog2D[i].previous.Y >= -1f)
					{
						_vector2_analog2D.X = analog2D[i].previous.X;
						_vector2_analog2D.Y = analog2D[i].previous.Y;
						break;
					}
				}
			}
			return _vector2_analog2D;
		}
		set
		{
			if (type == Type.Analog2D)
			{
				for (int i = 0; i < analog2DCount; i++)
				{
					analog2D[i].previous = value;
				}
			}
		}
	}

	public InputEntity(Type xType, string xName, Scope xScope)
	{
		type = xType;
		name = xName;
		scope = xScope;
	}

	public InputEntity(Type xType, string xName)
	{
		type = xType;
		name = xName;
		scope = Scope.Scene;
	}

	public void Update(GameTime oGameTime)
	{
		if (type == Type.Button)
		{
			for (int i = 0; i < buttonsCount; i++)
			{
				buttons[i].Update(oGameTime);
			}
		}
		else if (type == Type.Analog1D)
		{
			for (int j = 0; j < analog1DCount; j++)
			{
				analog1D[j].Update(oGameTime);
			}
		}
		else if (type == Type.Analog2D)
		{
			for (int k = 0; k < analog2DCount; k++)
			{
				analog2D[k].Update(oGameTime);
			}
		}
	}

	public void Add(InputButton oButton)
	{
		if (type == Type.Button)
		{
			buttons.Add(oButton);
			buttonsCount = buttons.Count;
		}
	}

	public void Add(InputAnalog1D oAnalog1D)
	{
		if (type == Type.Analog1D)
		{
			analog1D.Add(oAnalog1D);
			analog1DCount = analog1D.Count;
		}
	}

	public void Add(InputAnalog2D oAnalog2D)
	{
		if (type == Type.Analog2D)
		{
			analog2D.Add(oAnalog2D);
			analog2DCount = analog2D.Count;
		}
	}

	public void Remove(InputButton oButton)
	{
		if (type == Type.Button)
		{
			buttons.Remove(oButton);
			buttonsCount = buttons.Count;
		}
	}

	public void Remove(InputAnalog1D oAnalog1D)
	{
		if (type == Type.Analog1D)
		{
			analog1D.Remove(oAnalog1D);
			analog1DCount = analog1D.Count;
		}
	}

	public void Remove(InputAnalog2D oAnalog2D)
	{
		if (type == Type.Analog2D)
		{
			analog2D.Remove(oAnalog2D);
			analog2DCount = analog2D.Count;
		}
	}

	public void SetPrimaryGamePadIndex(int xIndex)
	{
		if (type == Type.Button)
		{
			for (int i = 0; i < buttonsCount; i++)
			{
				buttons[i].gamePadIndex = xIndex;
			}
		}
		else if (type == Type.Analog1D)
		{
			for (int j = 0; j < analog1DCount; j++)
			{
				analog1D[j].gamePadIndex = xIndex;
			}
		}
		else if (type == Type.Analog2D)
		{
			for (int k = 0; k < analog2DCount; k++)
			{
				analog2D[k].gamePadIndex = xIndex;
			}
		}
	}

	public void Flush()
	{
		if (type == Type.Button)
		{
			for (int i = 0; i < buttonsCount; i++)
			{
				buttons[i].pressed = false;
				buttons[i].downed = false;
			}
		}
	}
}
