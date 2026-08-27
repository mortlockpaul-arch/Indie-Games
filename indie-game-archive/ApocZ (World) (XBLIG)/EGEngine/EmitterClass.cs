using System;
using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class EmitterClass
{
	public virtual void LoadContent()
	{
		_ = EndGameEngine.GameSettings.LevelEmitterName == "null";
	}

	public void Update(float eTimeMS)
	{
		if (LevelOutside.Emitters == null)
		{
			return;
		}
		for (int i = 0; i < LevelOutside.Emitters.Length; i++)
		{
			eLevelEmitter eLevelEmitter2 = LevelOutside.Emitters[i];
			Vector3 spawnDir = Vector3.UnitY;
			eLevelEmitter2.Timer += eTimeMS;
			if (eLevelEmitter2.Flicker)
			{
				for (int j = 0; j < eLevelEmitter2.ChildLights.Count; j++)
				{
					int num = eLevelEmitter2.ChildLights[j];
					if (Math.Abs(LevelOutside.Lights[num].FlickerIntensity - LevelOutside.Lights[num].Intensity) <= 0.15f)
					{
						LevelOutside.Lights[num].FlickerIntensity = 1f + ((float)MyMath.m_Rand.NextDouble() * 0.8f - 0.3f);
					}
					else
					{
						LevelOutside.Lights[num].Intensity = MathHelper.Lerp(LevelOutside.Lights[num].Intensity, LevelOutside.Lights[num].FlickerIntensity, 0.5f);
					}
				}
			}
			if (eLevelEmitter2.eType == EmitterType.Fire)
			{
				float num2 = float.MaxValue;
				for (int k = 0; k < 4; k++)
				{
					float num3 = (LevelBaseMenu.Players[k].vecPosition - eLevelEmitter2.Position).LengthSquared();
					if (num3 < num2)
					{
						num2 = num3;
					}
				}
				num2 = num2 / 6250000f + 0.2f;
				num2 = ((num2 > 1f) ? 1f : num2) * 0.25f;
				if (eLevelEmitter2.Timer > num2)
				{
					eLevelEmitter2.Timer -= num2;
					particles.SpawnFire(ref eLevelEmitter2.Position, eLevelEmitter2.Scale);
				}
			}
			if (eLevelEmitter2.eType == EmitterType.FireLooping)
			{
				if (eLevelEmitter2.Timer < 1000f)
				{
					eLevelEmitter2.Timer = 2000f;
					particles.SpawnLoopingFire(ref eLevelEmitter2.Position, ref spawnDir, eLevelEmitter2.Scale);
				}
			}
			else if (eLevelEmitter2.eType == EmitterType.GroundSmokePart && eLevelEmitter2.Timer > 4f)
			{
				eLevelEmitter2.Timer = 0f;
				particles.SpawnDistantGroundSmoke(ref eLevelEmitter2.Position, ref spawnDir);
			}
		}
	}

	public virtual void Draw()
	{
	}
}
