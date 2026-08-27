using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class WeaponEffectParams
{
	public WeaponPart weaponPart;

	public EffectParameter texOffset;

	public EffectParameter EnvMap0;

	public EffectParameter fpsScene;

	public EffectParameter fpsBloom;

	public EffectParameter fpsAlpha;

	public EffectParameter TextureShadowMap;

	public EffectParameter vecLightColor;

	public EffectParameter vecAmbientLightColor;

	public EffectParameter vecLightPosition;

	public EffectParameter fSpecularPower;

	public EffectParameter fReflectiveness;

	public EffectParameter matTexProj;

	public EffectParameter matView;

	public EffectParameter matProj;

	public EffectParameter matWorld;

	public EffectParameter matViewProj;

	public EffectParameter vecFPSLightPos;

	public EffectParameter vecFPSLightColor;

	public EffectParameter matSkinnedWorldTransform;

	public EffectParameter matBones;

	public EffectParameter vecMuzzleFlash;

	public EffectParameter matQuatBones;

	public WeaponEffectParams(Effect e, WeaponData data)
	{
		texOffset = e.Parameters["texOffset"];
		EnvMap0 = e.Parameters["EnvMap0"];
		fpsScene = e.Parameters["fpsScene"];
		fpsBloom = e.Parameters["fpsBloom"];
		fpsAlpha = e.Parameters["fpsAlpha"];
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
		matQuatBones = e.Parameters["matQuatBones"];
		SetConstants();
		if (data != null)
		{
			fSpecularPower.SetValue(2f);
			fReflectiveness.SetValue(data.ReflectivePower);
			fSpecularPower.SetValue(data.SpecularPower);
		}
		else
		{
			fSpecularPower.SetValue(2f);
			fReflectiveness.SetValue(0.2f);
			fSpecularPower.SetValue(64f);
		}
	}

	public void SetConstants()
	{
		Vector3 zero = Vector3.Zero;
		zero.X = LevelOutside.SunPosition.X;
		zero.Y = LevelOutside.SunPosition.Y;
		zero.Z = LevelOutside.SunPosition.Z;
		Vector4 value = new Vector4(1f, 1f, 1f, 1f);
		Vector4 value2 = new Vector4(0.6f, 0.6f, 0.64f, 1f);
		if (EnvMap0 != null)
		{
			EnvMap0.SetValue(LevelBaseMenu.EnvMap);
		}
		vecLightPosition.SetValue(zero);
		vecLightColor.SetValue(value);
		vecAmbientLightColor.SetValue(value2);
	}
}
