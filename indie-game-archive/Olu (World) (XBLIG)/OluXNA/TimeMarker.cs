using System;
using Microsoft.Xna.Framework;

namespace OluXNA;

[Serializable]
public struct TimeMarker
{
	public float gameTime;

	public Vector3 direction;

	public Vector3 facingDir;

	public Vector3 up;

	public TimeMarker(float _g, Vector3 _dir, Vector3 _facingDir, Vector3 _up)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		gameTime = _g;
		direction = _dir;
		facingDir = _facingDir;
		up = _up;
	}
}
