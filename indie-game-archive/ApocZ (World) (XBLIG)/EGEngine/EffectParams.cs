using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct EffectParams(Effect e, string name)
{
	public string Name = name;

	public EffectParameter TextureShadowMap = e.Parameters["TextureShadowMap"];

	public EffectParameter DepthTexture = e.Parameters["DepthTexture"];

	public EffectParameter DiffuseDefferedTexture = e.Parameters["DiffuseDefferedTexture"];

	public EffectParameter NormalDefferedTexture = e.Parameters["NormalDefferedTexture"];

	public EffectParameter EnvMap0 = e.Parameters["EnvMap0"];

	public EffectParameter vecSunPosition = e.Parameters["vecSunPosition"];

	public EffectParameter fLightIndex = e.Parameters["fLightIndex"];

	public EffectParameter numberLights = e.Parameters["numberLights"];

	public EffectParameter vecLightPositions = e.Parameters["vecLightPositions"];

	public EffectParameter vecLightColors = e.Parameters["vecLightColors"];

	public EffectParameter vecLightColor = e.Parameters["vecLightColor"];

	public EffectParameter vecAmbientLightColor = e.Parameters["vecAmbientLightColor"];

	public EffectParameter matView = e.Parameters["matView"];

	public EffectParameter matViewProj = e.Parameters["matViewProj"];

	public EffectParameter eyePosition = e.Parameters["eyePosition"];

	public EffectParameter matTexProj = e.Parameters["matTexProj"];

	public EffectParameter matLightViewProj = e.Parameters["matLightViewProj"];

	public EffectParameter vecMuzzleFlash = e.Parameters["vecMuzzleFlash"];

	public EffectParameter uvDisplacement = e.Parameters["uvDisplacement"];
}
