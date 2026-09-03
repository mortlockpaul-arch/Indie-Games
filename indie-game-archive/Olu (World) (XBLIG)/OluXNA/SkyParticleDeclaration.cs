using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

[Serializable]
public struct SkyParticleDeclaration
{
	public Vector3 Velocity;

	public Vector2 TexCoord;

	public Vector3 Center;

	public Color Color;

	public SkyParticleDeclaration(Vector3 _vel, Vector2 _tex, Vector3 _center, Color _col)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Velocity = _vel;
		TexCoord = _tex;
		Center = _center;
		Color = _col;
	}

	public static int SizeInBytes()
	{
		return 36;
	}
}
