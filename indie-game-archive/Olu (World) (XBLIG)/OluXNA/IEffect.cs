using Microsoft.Xna.Framework;

namespace OluXNA;

internal class IEffect
{
	private Vector3 _pos;

	private Vector3 _rotAxis;

	private float _rotAngle;

	private float _rotDelta;

	public Vector3 pos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _pos;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_pos = value;
		}
	}

	public Vector3 rotAxis
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _rotAxis;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_rotAxis = value;
		}
	}

	public float rotAngle
	{
		get
		{
			return _rotAngle;
		}
		set
		{
			_rotAngle = value;
		}
	}

	public float rotDelta
	{
		get
		{
			return _rotDelta;
		}
		set
		{
			_rotDelta = value;
		}
	}

	public virtual void draw()
	{
	}
}
