using Microsoft.Xna.Framework;

namespace OluXNA;

public struct VertexNormalTex
{
	public Vector3 position;

	public Vector3 normal;

	public Vector2 tex;

	public byte a;

	public byte b;

	public byte c;

	public byte d;

	public byte e;

	public byte f;

	public byte g;

	public byte h;

	public int boneNum(int index)
	{
		return index switch
		{
			2 => b, 
			1 => c, 
			0 => d, 
			_ => a, 
		};
	}

	public float weight(int index)
	{
		return (float)(index switch
		{
			2 => h, 
			1 => g, 
			0 => f, 
			_ => e, 
		}) / 255f;
	}

	public VertexNormalTex(VertexNormalTex other)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		position = new Vector3(other.position.X, other.position.Y, other.position.Z);
		normal = new Vector3(other.normal.X, other.normal.Y, other.normal.Z);
		tex = new Vector2(other.tex.X, other.tex.Y);
		a = other.a;
		b = other.b;
		c = other.c;
		d = other.d;
		e = other.e;
		f = other.f;
		g = other.g;
		h = other.h;
	}

	public VertexNormalTex(VertexNormal other)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		position = new Vector3(other.position.X, other.position.Y, other.position.Z);
		normal = new Vector3(other.normal.X, other.normal.Y, other.normal.Z);
		tex = default(Vector2);
		a = other.a;
		b = other.b;
		c = other.c;
		d = other.d;
		e = other.e;
		f = other.f;
		g = other.g;
		h = other.h;
	}

	public VertexNormalTex(VectorPositionNormal other)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		position = new Vector3(other.position.X, other.position.Y, other.position.Z);
		normal = new Vector3(other.normal.X, other.normal.Y, other.normal.Z);
		tex = default(Vector2);
		a = (b = (c = (d = (e = (f = (g = (h = 0)))))));
	}
}
