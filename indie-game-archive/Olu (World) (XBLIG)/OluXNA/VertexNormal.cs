using Microsoft.Xna.Framework;

namespace OluXNA;

public struct VertexNormal
{
	public Vector3 position;

	public Vector3 normal;

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
			2 => c, 
			1 => b, 
			0 => a, 
			_ => d, 
		};
	}

	public float weight(int index)
	{
		return (float)(index switch
		{
			2 => e, 
			1 => f, 
			0 => g, 
			_ => h, 
		}) / 255f;
	}

	public VertexNormal(VertexNormal other)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		position = new Vector3(other.position.X, other.position.Y, other.position.Z);
		normal = new Vector3(other.normal.X, other.normal.Y, other.normal.Z);
		a = other.a;
		b = other.b;
		c = other.c;
		d = other.d;
		e = other.e;
		f = other.f;
		g = other.g;
		h = other.h;
	}

	public VertexNormal(VertexNormalTex other)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		position = new Vector3(other.position.X, other.position.Y, other.position.Z);
		normal = new Vector3(other.normal.X, other.normal.Y, other.normal.Z);
		a = other.a;
		b = other.b;
		c = other.c;
		d = other.d;
		e = other.e;
		f = other.f;
		g = other.g;
		h = other.h;
	}
}
