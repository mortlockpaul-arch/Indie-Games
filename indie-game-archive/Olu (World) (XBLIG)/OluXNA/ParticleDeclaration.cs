using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

[Serializable]
public struct ParticleDeclaration
{
	public Vector3 Velocity;

	public Vector2 TexCoord;

	public Vector3 Center;

	public Color Color;

	public Vector2 GlowCoord;

	public float Glow;

	public ParticleDeclaration(Vector3 _vel, Vector2 _tex, Vector4 _center, Color _col, Vector2 _glow, float _boolGlow)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Velocity = _vel;
		TexCoord = _tex;
		Center = new Vector3(_center.X, _center.Y, _center.Z);
		Color = _col;
		GlowCoord = _glow;
		Glow = _boolGlow;
	}

	public static int SizeInBytes()
	{
		return 48;
	}
}
