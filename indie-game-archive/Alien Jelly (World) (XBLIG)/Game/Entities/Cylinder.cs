using Microsoft.Xna.Framework;

namespace Game.Entities;

public class Cylinder
{
	private Vector2 _temp_vector2 = default(Vector2);

	public Vector3 position;

	public float radius;

	public float height;

	public Cylinder(Vector3 vPosition, float xRadius, float xHeight)
	{
		position = vPosition;
		radius = xRadius;
		height = xHeight;
	}

	public Cylinder()
	{
		position = Vector3.Zero;
		radius = 0f;
		height = 0f;
	}

	public bool Collide(Vector3 vPosition, float xObjectRadius, float xObjectHeight)
	{
		bool result = false;
		if ((vPosition.Y >= position.Y && vPosition.Y <= position.Y + height) || (vPosition.Y + xObjectHeight >= position.Y && vPosition.Y + xObjectHeight <= position.Y + height) || (vPosition.Y < position.Y && vPosition.Y + xObjectHeight > position.Y + height))
		{
			_temp_vector2.X = vPosition.X - position.X;
			_temp_vector2.Y = vPosition.Z - position.Z;
			if (_temp_vector2.Length() < radius + xObjectRadius)
			{
				result = true;
			}
		}
		return result;
	}

	public bool CollideXZ(float xX, float xZ, float xObjectRadius)
	{
		bool result = false;
		_temp_vector2.X = xX - position.X;
		_temp_vector2.Y = xZ - position.Z;
		if (_temp_vector2.Length() < radius + xObjectRadius)
		{
			result = true;
		}
		return result;
	}
}
