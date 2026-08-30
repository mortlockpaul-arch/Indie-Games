namespace FarseerGames.FarseerPhysics.Collisions;

internal class Triangle
{
	public float[] x;

	public float[] y;

	public Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
	{
		x = new float[3];
		y = new float[3];
		float num = x2 - x1;
		float num2 = x3 - x1;
		float num3 = y2 - y1;
		float num4 = y3 - y1;
		float num5 = num * num4 - num2 * num3;
		if (num5 > 0f)
		{
			x[0] = x1;
			x[1] = x2;
			x[2] = x3;
			y[0] = y1;
			y[1] = y2;
			y[2] = y3;
		}
		else
		{
			x[0] = x1;
			x[1] = x3;
			x[2] = x2;
			y[0] = y1;
			y[1] = y3;
			y[2] = y2;
		}
	}

	public Triangle(Triangle t)
	{
		x = new float[3];
		y = new float[3];
		x[0] = t.x[0];
		x[1] = t.x[1];
		x[2] = t.x[2];
		y[0] = t.y[0];
		y[1] = t.y[1];
		y[2] = t.y[2];
	}

	public bool IsInside(float _x, float _y)
	{
		if (_x < x[0] && _x < x[1] && _x < x[2])
		{
			return false;
		}
		if (_x > x[0] && _x > x[1] && _x > x[2])
		{
			return false;
		}
		if (_y < y[0] && _y < y[1] && _y < y[2])
		{
			return false;
		}
		if (_y > y[0] && _y > y[1] && _y > y[2])
		{
			return false;
		}
		float num = _x - x[0];
		float num2 = _y - y[0];
		float num3 = x[1] - x[0];
		float num4 = y[1] - y[0];
		float num5 = x[2] - x[0];
		float num6 = y[2] - y[0];
		float num7 = num5 * num5 + num6 * num6;
		float num8 = num5 * num3 + num6 * num4;
		float num9 = num5 * num + num6 * num2;
		float num10 = num3 * num3 + num4 * num4;
		float num11 = num3 * num + num4 * num2;
		float num12 = 1f / (num7 * num10 - num8 * num8);
		float num13 = (num10 * num9 - num8 * num11) * num12;
		float num14 = (num7 * num11 - num8 * num9) * num12;
		if (num13 > 0f && num14 > 0f)
		{
			return num13 + num14 < 1f;
		}
		return false;
	}
}
