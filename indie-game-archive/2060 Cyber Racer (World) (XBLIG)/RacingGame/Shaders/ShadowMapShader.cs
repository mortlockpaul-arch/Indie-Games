using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;

namespace RacingGame.Shaders;

public class ShadowMapShader : ShaderEffect
{
	private const string ShaderFilename = "ShadowMap.fx";

	internal RenderToTexture shadowMapTexture;

	internal float shadowNearPlane;

	internal float shadowFarPlane;

	internal float virtualLightDistance;

	internal float virtualVisibleRange;

	private Vector3 shadowLightPos;

	private float texelWidth;

	private float texelHeight;

	private float texOffsetX;

	private float texOffsetY;

	internal float compareDepthBias;

	internal float texExtraScale;

	internal float shadowMapDepthBiasValue;

	private Matrix texScaleBiasMatrix;

	internal Matrix lightProjectionMatrix;

	internal Matrix lightViewMatrix;

	private EffectParameter shadowTexTransform;

	private EffectParameter worldViewProjLight;

	private EffectParameter nearPlane;

	private EffectParameter farPlane;

	private EffectParameter depthBias;

	private EffectParameter shadowMapDepthBias;

	private EffectParameter shadowMap;

	private EffectParameter shadowMapTexelSize;

	private EffectParameter shadowDistanceFadeoutTexture;

	internal ShadowMapBlur shadowMapBlur;

	public float ShadowDistance => virtualLightDistance;

	public Vector3 ShadowLightPos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return shadowLightPos;
		}
	}

	internal void CalcShadowMapBiasMatrix()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		texelWidth = 1f / (float)shadowMapTexture.Width;
		texelHeight = 1f / (float)shadowMapTexture.Height;
		texOffsetX = 0.5f + 0.5f / (float)shadowMapTexture.Width;
		texOffsetY = 0.5f + 0.5f / (float)shadowMapTexture.Height;
		texScaleBiasMatrix = new Matrix(0.5f * texExtraScale, 0f, 0f, 0f, 0f, -0.5f * texExtraScale, 0f, 0f, 0f, 0f, texExtraScale, 0f, texOffsetX, texOffsetY, 0f, 1f);
	}

	public ShadowMapShader()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		shadowNearPlane = 1f;
		shadowFarPlane = 28f;
		virtualLightDistance = 24f;
		virtualVisibleRange = 23.5f;
		shadowLightPos = Vector3.Zero;
		texelWidth = 0.0009765625f;
		texelHeight = 0.0009765625f;
		texOffsetX = 0.5f;
		texOffsetY = 0.5f;
		compareDepthBias = 0.00025f;
		texExtraScale = 1f;
		shadowMapDepthBiasValue = 0.00025f;
		base._002Ector("ShadowMap.fx");
		compareDepthBias = 0.0001f;
		shadowMapTexture = new RenderToTexture(RenderToTexture.SizeType.ShadowMap);
		CalcShadowMapBiasMatrix();
		shadowMapBlur = new ShadowMapBlur();
	}

	protected override void GetParameters()
	{
		if (effect != null)
		{
			base.GetParameters();
			shadowTexTransform = effect.Parameters["shadowTexTransform"];
			worldViewProjLight = effect.Parameters["worldViewProjLight"];
			nearPlane = effect.Parameters["nearPlane"];
			farPlane = effect.Parameters["farPlane"];
			depthBias = effect.Parameters["depthBias"];
			shadowMapDepthBias = effect.Parameters["shadowMapDepthBias"];
			shadowMap = effect.Parameters["shadowMap"];
			shadowMapTexelSize = effect.Parameters["shadowMapTexelSize"];
			shadowDistanceFadeoutTexture = effect.Parameters["shadowDistanceFadeoutTexture"];
			if (shadowDistanceFadeoutTexture != null)
			{
				shadowDistanceFadeoutTexture.SetValue((Texture)(object)new Texture("ShadowDistanceFadeoutMap").XnaTexture);
			}
		}
	}

	public override void SetParameters(Material setMat)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (effect != null)
		{
			shadowNearPlane = 1f;
			shadowFarPlane = 218.75f;
			virtualLightDistance = 171.59999f;
			virtualVisibleRange = 129.25f;
			compareDepthBias = 0.00065f;
			shadowMapDepthBiasValue = 0.00065f;
			base.SetParameters(setMat);
			depthBias.SetValue(compareDepthBias);
			shadowMapDepthBias.SetValue(shadowMapDepthBiasValue);
			shadowMapTexelSize.SetValue(new Vector2(texelWidth, texelHeight));
			nearPlane.SetValue(shadowNearPlane);
			farPlane.SetValue(shadowFarPlane);
		}
	}

	private void CalcSimpleDirectionalShadowMappingMatrix()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Math.Atan2(virtualVisibleRange, virtualLightDistance);
		lightProjectionMatrix = Matrix.CreatePerspective(num, 1f, shadowNearPlane, shadowFarPlane);
		Vector3 val = (RacingGameManager.InMenu ? RacingGameManager.Player.CarPosition : (RacingGameManager.Player.CarPosition + RacingGameManager.Player.CarDirection * virtualVisibleRange / 6f));
		lightViewMatrix = Matrix.CreateLookAt(val + BaseGame.LightDirection * virtualVisibleRange, val, new Vector3(0f, 0f, 1f));
		Matrix val2 = Matrix.Invert(lightViewMatrix);
		shadowLightPos = new Vector3(val2.M41, val2.M42, val2.M43);
	}

	internal void UpdateGenerateShadowWorldMatrix(Matrix setWorldMatrix)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = (base.WorldMatrix = setWorldMatrix);
		base.WorldViewProjMatrix = val * lightViewMatrix * lightProjectionMatrix;
		effect.CommitChanges();
	}

	internal void GenerateShadows(BaseGame.RenderHandler renderObjects)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (effect != null)
		{
			SetParameters(null);
			Matrix viewMatrix = BaseGame.ViewMatrix;
			Matrix projectionMatrix = BaseGame.ProjectionMatrix;
			CalcSimpleDirectionalShadowMappingMatrix();
			DepthStencilBuffer depthStencilBuffer = null;
			shadowMapTexture.SetRenderTarget();
			if (shadowMapTexture.ZBufferSurface != null)
			{
				depthStencilBuffer = BaseGame.Device.DepthStencilBuffer;
				BaseGame.Device.DepthStencilBuffer = shadowMapTexture.ZBufferSurface;
			}
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.AlphaBlendEnable = false;
			shadowMapTexture.Clear(Color.White);
			effect.CurrentTechnique = effect.Techniques["GenerateShadowMap20"];
			RenderSinglePassShader(renderObjects);
			shadowMapTexture.Resolve();
			BaseGame.ResetRenderTarget(fullResetToBackBuffer: false);
			if (shadowMapTexture.ZBufferSurface != null)
			{
				BaseGame.Device.DepthStencilBuffer = depthStencilBuffer;
			}
			BaseGame.ViewMatrix = viewMatrix;
			BaseGame.ProjectionMatrix = projectionMatrix;
		}
	}

	internal void UpdateCalcShadowWorldMatrix(Matrix setWorldMatrix)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base.WorldMatrix = setWorldMatrix;
		base.WorldViewProjMatrix = setWorldMatrix * BaseGame.ViewMatrix * BaseGame.ProjectionMatrix;
		Matrix value = setWorldMatrix * lightViewMatrix * lightProjectionMatrix * texScaleBiasMatrix;
		shadowTexTransform.SetValue(value);
		Matrix value2 = setWorldMatrix * lightViewMatrix * lightProjectionMatrix;
		worldViewProjLight.SetValue(value2);
		effect.CommitChanges();
	}

	public void RenderShadows(BaseGame.RenderHandler renderObjects)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (effect != null)
		{
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
			shadowMapBlur.RenderShadows(delegate
			{
				effect.CurrentTechnique = effect.Techniques["UseShadowMap20"];
				SetParameters(null);
				shadowMap.SetValue((Texture)(object)shadowMapTexture.XnaTexture);
				RenderSinglePassShader(renderObjects);
			});
			shadowMapBlur.RenderShadows();
			BaseGame.Device.Clear((ClearOptions)2, Color.Black, 1f, 0);
		}
	}

	public static void PrepareGameShadows()
	{
		_ = BaseGame.AllowShadowMapping;
	}

	public void ShowShadows()
	{
		shadowMapBlur.ShowShadows();
	}
}
