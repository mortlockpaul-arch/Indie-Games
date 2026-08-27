using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class SkinnedEffectParams
{
	public EffectParameter EnvMap0;

	public EffectParameter TextureShadowMap;

	public EffectParameter vecLightColor;

	public EffectParameter vecAmbientLightColor;

	public EffectParameter vecLightPosition;

	public EffectParameter fSpecularPower;

	public EffectParameter fReflectiveness;

	public EffectParameter matTexProj;

	public EffectParameter matView;

	public EffectParameter matProj;

	public EffectParameter matViewProj;

	public EffectParameter matWorld;

	public EffectParameter vecFPSLightPos;

	public EffectParameter vecFPSLightColor;

	public EffectParameter matSkinnedWorldTransform;

	public EffectParameter matBones;

	public EffectParameter vecMuzzleFlash;

	public SkinnedEffectParams(Effect e)
	{
		EnvMap0 = e.Parameters["EnvMap0"];
		TextureShadowMap = e.Parameters["TextureShadowMap"];
		vecLightColor = e.Parameters["vecLightColor"];
		vecAmbientLightColor = e.Parameters["vecAmbientLightColor"];
		vecLightPosition = e.Parameters["vecLightPosition"];
		fSpecularPower = e.Parameters["fSpecularPower"];
		fReflectiveness = e.Parameters["fReflectiveness"];
		matTexProj = e.Parameters["matTexProj"];
		matView = e.Parameters["matView"];
		matProj = e.Parameters["matProj"];
		matWorld = e.Parameters["matWorld"];
		matViewProj = e.Parameters["matViewProj"];
		vecFPSLightPos = e.Parameters["vecFPSLightPos"];
		vecFPSLightColor = e.Parameters["vecFPSLightColor"];
		matSkinnedWorldTransform = e.Parameters["matSkinnedWorldTransform"];
		matBones = e.Parameters["matBones"];
		vecMuzzleFlash = e.Parameters["vecMuzzleFlash"];
		SetConstants();
	}

	public void SetConstants()
	{
		Vector3 zero = Vector3.Zero;
		zero.X = LevelOutside.SunPosition.X;
		zero.Y = LevelOutside.SunPosition.Y;
		zero.Z = LevelOutside.SunPosition.Z;
		Vector4 value = new Vector4(1f, 1f, 1f, 1f);
		Vector4 value2 = new Vector4(0.35f, 0.35f, 0.4f, 1f);
		if (EnvMap0 != null)
		{
			EnvMap0.SetValue(LevelBaseMenu.EnvMap);
		}
		vecLightPosition.SetValue(zero);
		fSpecularPower.SetValue(2f);
		vecLightColor.SetValue(value);
		vecAmbientLightColor.SetValue(value2);
	}
}
