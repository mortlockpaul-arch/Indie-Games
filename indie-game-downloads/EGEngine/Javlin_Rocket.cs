using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EGEngine;

public struct Javlin_Rocket
{
	public bool InUse;

	public int Stage;

	public float Life;

	public float Speed;

	public float PosVariable;

	public Vector3 Position;

	public Vector3 Direction;

	public Vector3 Right;

	public Vector3 TargetPos;

	public Cue JavlinSound;
}
