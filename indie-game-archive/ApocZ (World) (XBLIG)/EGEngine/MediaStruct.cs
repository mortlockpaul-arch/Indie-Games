using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EGEngine;

public struct MediaStruct
{
	public bool Render;

	public uint Flags;

	public Vector2 Offset;

	public Vector2 Position;

	public Vector3 Position3D;

	public Vector2 Direction;

	public string BallonText;

	public string SoundName;

	public Cue SoundCue;

	public float Scale;

	public float Timer;

	public float TimerDelay;
}
