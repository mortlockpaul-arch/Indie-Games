using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EGEngine;

public struct RPG_Rocket
{
	public bool InUse;

	public bool useTarget;

	public float Acuuracey;

	public float Life;

	public float PosVariable;

	public float Speed;

	public Vector3 Position;

	public Vector3 Direction;

	public Vector3 Right;

	public Vector3 TargetPosition;

	public Cue RPGSound;
}
