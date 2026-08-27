using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace EGEngine;

public class RPG : PropModelBase
{
	public static int MaxNumberRPGs = 8;

	public static RPG_Rocket[] allRPGs = new RPG_Rocket[MaxNumberRPGs];

	private static Cue launchSound = null;

	private static Vector3 dummy = Vector3.Zero;

	private Vector3 tmpPartPos = Vector3.Zero;

	private Vector3 tmpPartDir = Vector3.Zero;

	private static IntersectSegmentParams tmpSegmentParams = default(IntersectSegmentParams);

	private static Cue grenadeSound = null;

	public override void Load(string n)
	{
		base.Load(n);
		for (int i = 0; i < MaxNumberRPGs; i++)
		{
			allRPGs[i].InUse = false;
			allRPGs[i].RPGSound = EndGameEngine.SoundBnk.GetCue("rpgfly");
		}
	}

	public void Add(ref Vector3 pos, ref Vector3 dir, float life, float speed)
	{
		Add(ref pos, ref dir, life, speed, ref dummy, useT: false);
	}

	public void Add(ref Vector3 pos, ref Vector3 dir, float life, float speed, ref Vector3 targetPos, bool useT)
	{
		for (int i = 0; i < MaxNumberRPGs; i++)
		{
			if (!allRPGs[i].InUse)
			{
				allRPGs[i].InUse = true;
				allRPGs[i].Life = life;
				allRPGs[i].PosVariable = 0f;
				allRPGs[i].Speed = speed;
				allRPGs[i].Position = pos;
				allRPGs[i].Direction = Vector3.Normalize(dir);
				allRPGs[i].Right = Vector3.Cross(allRPGs[i].Direction, Vector3.UnitY);
				allRPGs[i].useTarget = useT;
				allRPGs[i].Acuuracey = 0f;
				allRPGs[i].TargetPosition = targetPos;
				float num = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - allRPGs[i].Position).LengthSquared();
				num /= 500000f;
				if (launchSound != null)
				{
					launchSound.Stop(AudioStopOptions.Immediate);
					launchSound.Dispose();
				}
				launchSound = EndGameEngine.SoundBnk.GetCue("rpglaunch");
				launchSound.Play();
				launchSound.SetVariable("Distance", num * 10000f);
				if (!allRPGs[i].RPGSound.IsDisposed)
				{
					allRPGs[i].RPGSound.Stop(AudioStopOptions.Immediate);
					allRPGs[i].RPGSound.Dispose();
				}
				allRPGs[i].RPGSound = EndGameEngine.SoundBnk.GetCue("rpgfly");
				allRPGs[i].RPGSound.Play();
				allRPGs[i].RPGSound.SetVariable("Distance", num * 10000f);
				break;
			}
		}
	}

	public override void Update(float eTime, int qIndex)
	{
		for (int i = 0; i < MaxNumberRPGs; i++)
		{
			if (!allRPGs[i].InUse)
			{
				continue;
			}
			allRPGs[i].Life -= eTime;
			if (allRPGs[i].Life > 0f)
			{
				if (allRPGs[i].useTarget)
				{
					if (allRPGs[i].Acuuracey < 0.1f)
					{
						allRPGs[i].Acuuracey += 0.0005f;
					}
					tmpPartDir = allRPGs[i].TargetPosition - allRPGs[i].Position;
					tmpPartDir.Normalize();
					allRPGs[i].Direction = Vector3.Lerp(allRPGs[i].Direction * 100f, tmpPartDir * 100f, allRPGs[i].Acuuracey);
					allRPGs[i].Direction.Normalize();
				}
				tmpSegmentParams.OnlyWalkable = true;
				tmpSegmentParams.SegmentDirection = allRPGs[i].Direction;
				tmpSegmentParams.SegmentLength = allRPGs[i].Speed + 40f;
				tmpSegmentParams.SegmentStart = allRPGs[i].Position;
				tmpSegmentParams.SegmentEnd = allRPGs[i].Position + allRPGs[i].Direction * tmpSegmentParams.SegmentLength;
				tmpSegmentParams.PreComputeParameters();
				if (LevelOutside.RayCast(qIndex, ref tmpSegmentParams, spawnSparks: true) != MaterialType.Undefined)
				{
					allRPGs[i].InUse = false;
					BlowGrenade(i, qIndex);
					continue;
				}
				allRPGs[i].Position += allRPGs[i].Direction * allRPGs[i].Speed;
				if (!allRPGs[i].useTarget)
				{
					allRPGs[i].PosVariable += eTime * 0.5f;
					if (allRPGs[i].PosVariable > 1f)
					{
						allRPGs[i].PosVariable = 1f;
					}
					float num = (float)Math.Cos(allRPGs[i].Life * 10f) * 8f * allRPGs[i].PosVariable;
					tmpPartPos = allRPGs[i].Position;
					tmpPartPos += allRPGs[i].Right * num;
					tmpPartPos += Vector3.UnitY * num;
					tmpPartDir = allRPGs[i].Right * 2f;
					allRPGs[i].Position = tmpPartPos;
				}
				else
				{
					tmpPartDir = allRPGs[i].Right * 2f;
					tmpPartPos = allRPGs[i].Position;
				}
				particles.SpawnSmallRPGTrial(ref tmpPartPos, ref tmpPartDir);
				if (allRPGs[i].RPGSound.IsPlaying)
				{
					float num2 = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - allRPGs[i].Position).LengthSquared();
					num2 /= 500000f;
					allRPGs[i].RPGSound.SetVariable("Distance", num2 * 10000f);
				}
			}
			else
			{
				allRPGs[i].InUse = false;
				BlowGrenade(i, qIndex);
			}
		}
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
		for (int i = 0; i < MaxNumberRPGs; i++)
		{
			if (allRPGs[i].InUse)
			{
				ref Matrix reference = ref matWorld[qIndex];
				reference = Matrix.Identity;
				matWorld[qIndex].Forward = allRPGs[i].Direction;
				matWorld[qIndex].Right = allRPGs[i].Right;
				matWorld[qIndex].Up = Vector3.Cross(allRPGs[i].Direction, matWorld[qIndex].Right);
				matWorld[qIndex].Translation = allRPGs[i].Position;
				base.Draw(viewer, qIndex);
			}
		}
	}

	private void BlowGrenade(int rpgIndex, int qIndex)
	{
		AIBase.GrenadeExplode(qIndex, ref allRPGs[rpgIndex].Position);
		particles.SpawnGrenadeExplosion(allRPGs[rpgIndex].Position, 1f);
		if (!allRPGs[rpgIndex].RPGSound.IsDisposed)
		{
			allRPGs[rpgIndex].RPGSound.Stop(AudioStopOptions.Immediate);
			allRPGs[rpgIndex].RPGSound.Dispose();
		}
		float num = (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - allRPGs[rpgIndex].Position).LengthSquared() / 500000f;
		if (grenadeSound != null)
		{
			grenadeSound.Stop(AudioStopOptions.Immediate);
			grenadeSound.Dispose();
		}
		grenadeSound = EndGameEngine.SoundBnk.GetCue("grenade00");
		grenadeSound.Play();
		grenadeSound.SetVariable("Distance", num * 10000f);
	}

	public void KillSound()
	{
		for (int i = 0; i < MaxNumberRPGs; i++)
		{
			if (allRPGs[i].RPGSound != null && !allRPGs[i].RPGSound.IsDisposed)
			{
				allRPGs[i].RPGSound.Stop(AudioStopOptions.Immediate);
				allRPGs[i].RPGSound.Dispose();
			}
		}
	}

	public void KillAllrockets()
	{
		for (int i = 0; i < MaxNumberRPGs; i++)
		{
			allRPGs[i].Life = 0f;
			allRPGs[i].InUse = false;
		}
		KillSound();
	}
}
