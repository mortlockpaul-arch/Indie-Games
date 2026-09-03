using System;
using Microsoft.Xna.Framework;

namespace OluXNA;

[Serializable]
public struct VertexTransform
{
	public Vector3 position1;

	public Vector3 position2;

	public Vector3 normal1;

	public Vector3 normal2;

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

	public VertexTransform(VertexNormalTex fromModel, VertexNormalTex toModel)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		position1 = new Vector3(fromModel.position.X, fromModel.position.Y, fromModel.position.Z);
		position2 = new Vector3(toModel.position.X, toModel.position.Y, toModel.position.Z);
		normal1 = new Vector3(fromModel.normal.X, fromModel.normal.Y, fromModel.normal.Z);
		normal2 = new Vector3(toModel.normal.X, toModel.normal.Y, toModel.normal.Z);
		tex = default(Vector2);
		a = toModel.a;
		b = toModel.b;
		c = toModel.c;
		d = toModel.d;
		e = toModel.e;
		f = toModel.f;
		g = toModel.g;
		h = toModel.h;
	}

	public static int SizeInBytes()
	{
		return 64;
	}
}
