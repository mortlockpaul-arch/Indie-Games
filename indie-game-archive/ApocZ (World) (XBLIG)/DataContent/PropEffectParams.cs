using Microsoft.Xna.Framework.Graphics;

namespace DataContent;

public class PropEffectParams
{
	public EffectParameter texOffset;

	public EffectParameter EnvMap0;

	public EffectParameter fpsScene;

	public EffectParameter fpsBloom;

	public EffectParameter fpsAlpha;

	public EffectParameter eyePosition;

	public EffectParameter TextureShadowMap;

	public EffectParameter vecLightColor;

	public EffectParameter vecAmbientLightColor;

	public EffectParameter vecLightPosition;

	public EffectParameter fSpecularPower;

	public EffectParameter fReflectiveness;

	public EffectParameter fRayTracedLight;

	public EffectParameter matTexProj;

	public EffectParameter matWorld;

	public EffectParameter matViewProj;

	public EffectParameter matLightViewProj;

	public EffectParameter vecFPSLightPos;

	public EffectParameter vecFPSLightColor;

	public EffectParameter matSkinnedWorldTransform;

	public EffectParameter matBones;

	public EffectParameter vecMuzzleFlash;

	public EffectParameter vecUVOffset;

	public PropEffectParams(Effect e)
	{
		texOffset = e.Parameters["texOffset"];
		EnvMap0 = e.Parameters["EnvMap0"];
		fpsScene = e.Parameters["fpsScene"];
		fpsBloom = e.Parameters["fpsBloom"];
		fpsAlpha = e.Parameters["fpsAlpha"];
		eyePosition = e.Parameters["eyePosition"];
		TextureShadowMap = e.Parameters["TextureShadowMap"];
		vecLightColor = e.Parameters["vecLightColor"];
		vecAmbientLightColor = e.Parameters["vecAmbientLightColor"];
		vecLightPosition = e.Parameters["vecLightPosition"];
		fSpecularPower = e.Parameters["fSpecularPower"];
		fReflectiveness = e.Parameters["fReflectiveness"];
		fRayTracedLight = e.Parameters["fRayTracedLight"];
		matTexProj = e.Parameters["matTexProj"];
		matWorld = e.Parameters["matWorld"];
		matViewProj = e.Parameters["matViewProj"];
		matLightViewProj = e.Parameters["matLightViewProj"];
		vecFPSLightPos = e.Parameters["vecFPSLightPos"];
		vecFPSLightColor = e.Parameters["vecFPSLightColor"];
		matSkinnedWorldTransform = e.Parameters["matSkinnedWorldTransform"];
		matBones = e.Parameters["matBones"];
		vecMuzzleFlash = e.Parameters["vecMuzzleFlash"];
		vecUVOffset = e.Parameters["vecUVOffset"];
	}
}
