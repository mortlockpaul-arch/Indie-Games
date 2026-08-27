using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EGEngine;

public class Javlin : PropModelBase
{
	private static int MaxNumberJavlins = 8;

	private static Javlin_Rocket[] allJavlins = new Javlin_Rocket[MaxNumberJavlins];

	private Vector3 tmpPartPos = Vector3.Zero;

	private Vector3 tmpPartDir = Vector3.Zero;

	private static IntersectSegmentParams tmpSegmentParams = default(IntersectSegmentParams);

	private static Cue grenadeSound = null;

	public override void Load(string n)
	{
		base.Load(n);
		for (int i = 0; i < MaxNumberJavlins; i++)
		{
			allJavlins[i].InUse = false;
			allJavlins[i].JavlinSound = EndGameEngine.SoundBnk.GetCue("rpgfly");
		}
	}

	public void Add(ref Vector3 pos, ref Vector3 dir, ref Vector3 targetPos)
	{
		for (int i = 0; i < MaxNumberJavlins; i++)
		{
			if (!allJavlins[i].InUse)
			{
				allJavlins[i].InUse = true;
				allJavlins[i].Life = 0.3f;
				allJavlins[i].Speed = 20f;
				allJavlins[i].Stage = 1;
				allJavlins[i].PosVariable = 0f;
				allJavlins[i].TargetPos = targetPos;
				allJavlins[i].Position = pos;
				allJavlins[i].Direction = Vector3.Normalize(dir);
				allJavlins[i].Right = Vector3.Cross(allJavlins[i].Direction, Vector3.UnitY);
				EndGameEngine.SoundBnk.GetCue("rpglaunch").Play();
				if (!allJavlins[i].JavlinSound.IsDisposed)
				{
					allJavlins[i].JavlinSound.Stop(AudioStopOptions.Immediate);
					allJavlins[i].JavlinSound.Dispose();
				}
				allJavlins[i].JavlinSound = EndGameEngine.SoundBnk.GetCue("rpgfly");
				allJavlins[i].JavlinSound.Play();
				break;
			}
		}
	}

	public override void Update(float eTime, int qIndex)
	{
		for (int i = 0; i < MaxNumberJavlins; i++)
		{
			if (allJavlins[i].InUse)
			{
				allJavlins[i].Life -= eTime;
				if (allJavlins[i].Stage == 1)
				{
					Stage1(ref allJavlins[i], eTime, qIndex);
				}
				else if (allJavlins[i].Stage == 2)
				{
					Stage2(ref allJavlins[i], eTime, qIndex);
				}
				else if (allJavlins[i].Stage == 3)
				{
					Stage3(ref allJavlins[i], eTime, qIndex);
				}
				particles.SpawnRPGTrial(ref allJavlins[i].Position, ref allJavlins[i].Direction);
			}
		}
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
		for (int i = 0; i < MaxNumberJavlins; i++)
		{
			if (allJavlins[i].InUse)
			{
				ref Matrix reference = ref matWorld[qIndex];
				reference = Matrix.Identity;
				matWorld[qIndex].Forward = allJavlins[i].Direction;
				matWorld[qIndex].Right = allJavlins[i].Right;
				matWorld[qIndex].Up = Vector3.Cross(allJavlins[i].Direction, matWorld[qIndex].Right);
				matWorld[qIndex].Translation = allJavlins[i].Position;
				base.Draw(viewer, qIndex);
			}
		}
	}

	private void BlowGrenade(ref Javlin_Rocket jr, int qIndex)
	{
		AIBase.GrenadeExplode(qIndex, ref jr.Position);
		particles.SpawnGrenadeExplosion(jr.Position, 1f);
		if (!jr.JavlinSound.IsDisposed)
		{
			jr.JavlinSound.Stop(AudioStopOptions.Immediate);
			jr.JavlinSound.Dispose();
		}
		float num = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - jr.Position).LengthSquared() / 500000f;
		if (grenadeSound != null)
		{
			grenadeSound.Stop(AudioStopOptions.Immediate);
			grenadeSound.Dispose();
		}
		grenadeSound = EndGameEngine.SoundBnk.GetCue("grenade00");
		grenadeSound.Play();
		grenadeSound.SetVariable("Distance", num * 10000f);
	}

	private void Stage1(ref Javlin_Rocket jr, float eTime, int qIndex)
	{
		if (jr.Life > 0f)
		{
			jr.Speed += 2f;
			tmpSegmentParams.OnlyWalkable = true;
			tmpSegmentParams.SegmentDirection = jr.Direction;
			tmpSegmentParams.SegmentLength = 140f;
			tmpSegmentParams.SegmentStart = jr.Position;
			tmpSegmentParams.SegmentEnd = jr.Position + jr.Direction * tmpSegmentParams.SegmentLength;
			tmpSegmentParams.PreComputeParameters();
			if (LevelOutside.RayCast(qIndex, ref tmpSegmentParams, spawnSparks: true) != MaterialType.Undefined)
			{
				jr.InUse = false;
				jr.Position = tmpSegmentParams.hitPosition;
				BlowGrenade(ref jr, qIndex);
				return;
			}
			jr.Position += jr.Direction * jr.Speed;
			if (jr.JavlinSound.IsPlaying)
			{
				float num = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - jr.Position).LengthSquared();
				num /= 500000f;
				jr.JavlinSound.SetVariable("Distance", num * 10000f);
			}
		}
		else
		{
			jr.Stage = 2;
			jr.Life = 2f;
			Vector3 vector = jr.Position + (jr.TargetPos - jr.Position) * 0.5f;
			vector.Y += 2500f;
			jr.Direction = Vector3.Normalize(vector - jr.Position);
		}
	}

	private void Stage2(ref Javlin_Rocket jr, float eTime, int qIndex)
	{
		if (jr.Life > 0f)
		{
			jr.Speed += 2f;
			if (jr.Speed > 100f)
			{
				jr.Speed = 100f;
			}
			tmpSegmentParams.OnlyWalkable = true;
			tmpSegmentParams.SegmentDirection = jr.Direction;
			tmpSegmentParams.SegmentLength = 140f;
			tmpSegmentParams.SegmentStart = jr.Position;
			tmpSegmentParams.SegmentEnd = jr.Position + jr.Direction * tmpSegmentParams.SegmentLength;
			tmpSegmentParams.PreComputeParameters();
			if (LevelOutside.RayCast(qIndex, ref tmpSegmentParams, spawnSparks: true) != MaterialType.Undefined)
			{
				jr.InUse = false;
				jr.Position = tmpSegmentParams.hitPosition;
				BlowGrenade(ref jr, qIndex);
				return;
			}
			jr.Position += jr.Direction * jr.Speed;
			if (jr.JavlinSound.IsPlaying)
			{
				float num = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - jr.Position).LengthSquared();
				num /= 500000f;
				jr.JavlinSound.SetVariable("Distance", num * 10000f);
			}
		}
		else
		{
			jr.Stage = 3;
			jr.Life = 3f;
			jr.Direction = Vector3.Normalize(jr.TargetPos - jr.Position);
		}
	}

	private void Stage3(ref Javlin_Rocket jr, float eTime, int qIndex)
	{
		if (jr.Life > 0f)
		{
			jr.Speed += 2f;
			if (jr.Speed > 100f)
			{
				jr.Speed = 100f;
			}
			tmpSegmentParams.OnlyWalkable = true;
			tmpSegmentParams.SegmentDirection = jr.Direction;
			tmpSegmentParams.SegmentLength = 140f;
			tmpSegmentParams.SegmentStart = jr.Position;
			tmpSegmentParams.SegmentEnd = jr.Position + jr.Direction * tmpSegmentParams.SegmentLength;
			tmpSegmentParams.PreComputeParameters();
			if (LevelOutside.RayCast(qIndex, ref tmpSegmentParams, spawnSparks: true) != MaterialType.Undefined)
			{
				jr.InUse = false;
				jr.Position = tmpSegmentParams.hitPosition;
				BlowGrenade(ref jr, qIndex);
				return;
			}
			tmpPartDir = Vector3.Normalize(jr.TargetPos - jr.Position) * 100f;
			jr.Direction = Vector3.Lerp(jr.Direction * 100f, tmpPartDir, 0.005f);
			jr.Direction.Normalize();
			jr.Position += jr.Direction * jr.Speed;
			if (jr.JavlinSound.IsPlaying)
			{
				float num = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - jr.Position).LengthSquared();
				num /= 500000f;
				jr.JavlinSound.SetVariable("Distance", num * 10000f);
			}
		}
		else
		{
			jr.InUse = false;
			BlowGrenade(ref jr, qIndex);
		}
	}

	public void KillSound()
	{
		for (int i = 0; i < MaxNumberJavlins; i++)
		{
			if (allJavlins[i].JavlinSound != null && !allJavlins[i].JavlinSound.IsDisposed)
			{
				allJavlins[i].JavlinSound.Stop(AudioStopOptions.Immediate);
				allJavlins[i].JavlinSound.Dispose();
			}
		}
	}
}
