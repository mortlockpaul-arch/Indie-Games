using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EGEngine;

public class EndGameEngine : Game
{
	public class MaterialEffectParams
	{
		public EffectParameter matInvView;

		public EffectParameter matWorld;

		public EffectParameter matView;

		public EffectParameter matProj;

		public EffectParameter matViewProj;

		public EffectParameter matWorldViewProj;

		public EffectParameter matLightViewProj;

		public EffectParameter matTexProj;

		public EffectParameter matLightViewProj2;

		public EffectParameter matTexProj2;

		public EffectParameter matRotation;

		public EffectParameter matBones;

		public EffectParameter matFPSWeaponTransform;

		public EffectParameter matSkinnedWorldTransform;

		public EffectParameter vecLightDirection;

		public EffectParameter vecLightPosition;

		public EffectParameter vecLightColor;

		public EffectParameter vecAmbientLightColor;

		public EffectParameter vecFogColor;

		public EffectParameter vecWorldOffset;

		public EffectParameter vecEyePosition;

		public EffectParameter vecViewDirection;

		public EffectParameter vecTerrianParams;

		public EffectParameter uvDisplacement;

		public EffectParameter vecFrustumCorners;

		public EffectParameter vecMuzzleFlash;

		public EffectParameter numberLights;

		public EffectParameter vecLightPositions;

		public EffectParameter vecLightColors;

		public EffectParameter vecFPSLightPos;

		public EffectParameter vecFPSLightColor;

		public EffectParameter vecDefLightPosition;

		public EffectParameter vecDefLightColor;

		public EffectParameter fDamage;

		public EffectParameter fFadeToBlack;

		public EffectParameter fShininess;

		public EffectParameter fSpecularPower;

		public EffectParameter hGaussianScale;

		public EffectParameter vGaussianScale;

		public EffectParameter fBloomThresh;

		public EffectParameter fBloomIntensity;

		public EffectParameter fBaseIntensity;

		public EffectParameter fBloomSaturation;

		public EffectParameter fBaseSaturation;

		public EffectParameter fEditTimer;

		public EffectParameter fReflectiveness;

		public EffectParameter fBlurFactor;

		public EffectParameter reticlePosition;

		public EffectParameter fpsHandsDiffuse;

		public EffectParameter fpsHandsNormal;

		public EffectParameter fpsWeaponDiffuse;

		public EffectParameter fpsWeaponNormal;

		public EffectParameter laserTexture;

		public EffectParameter propDiffuse1;

		public EffectParameter propDiffuse2;

		public EffectParameter propNormal1;

		public EffectParameter propNormal2;

		public EffectParameter decalsDiffuse;

		public EffectParameter decalsNormal;

		public EffectParameter skinnedDiffuse;

		public EffectParameter skinnedNormal;

		public EffectParameter worldNormals;

		public EffectParameter worldTangents;

		public EffectParameter diffuseBlend;

		public EffectParameter diffuseTextures;

		public EffectParameter diffuseNorm1;

		public EffectParameter diffuseNorm2;

		public EffectParameter diffuseNorm3;

		public EffectParameter diffuseCliffTexture;

		public EffectParameter stickersTexture;

		public EffectParameter stickersNormTexture;

		public EffectParameter editTexture;

		public EffectParameter texCoords;

		public EffectParameter Texture3;

		public EffectParameter Texture4;

		public EffectParameter Texture5;

		public EffectParameter Texture6;

		public EffectParameter Texture7;

		public EffectParameter Texture8;

		public EffectParameter Texture9;

		public EffectParameter TexParticle0;

		public EffectParameter TexParticle1;

		public EffectParameter EnvMap0;

		public EffectParameter TextureShadowMap;

		public EffectParameter TextureShadowMap2;

		public EffectTechnique T_FPSWeapon;

		public EffectTechnique T_PropTwoTextures;

		public EffectTechnique T_DecalsMaterial;

		public EffectTechnique T_ShadowMapProps;

		public EffectTechnique T_ShadowMapProps2;

		public EffectTechnique T_Terrain;

		public EffectTechnique T_DrawReticle;

		public EffectTechnique T_DrawStickers;

		public EffectTechnique T_DrawDebugPathing;

		public EffectTechnique T_EditObject;

		public EffectTechnique T_GimbalObject;

		public EffectTechnique T_PropFoilage;

		public EffectTechnique T_Skinned;

		public EffectTechnique T_SkinnedShadow;

		public EffectTechnique T_PropObject;

		public EffectTechnique T_WorldObject;

		public EffectTechnique T_RenderDepth;

		public EffectTechnique T_CopyQuad;

		public EffectTechnique T_SkinnedModel;

		public EffectTechnique T_WeaponModel;

		public EffectTechnique T_Particle;

		public EffectTechnique T_DebugTriangle;

		public EffectTechnique T_HDRDownSample;

		public EffectTechnique T_HDRBlur;

		public EffectTechnique T_HDRToneMap;

		public EffectTechnique T_UI;

		public void InitEffectParams(Effect mEffect)
		{
			matInvView = mEffect.Parameters["matInvView"];
			matWorld = mEffect.Parameters["matWorld"];
			matView = mEffect.Parameters["matView"];
			matProj = mEffect.Parameters["matProj"];
			matViewProj = mEffect.Parameters["matViewProj"];
			matWorldViewProj = mEffect.Parameters["matWorldViewProj"];
			matLightViewProj = mEffect.Parameters["matLightViewProj"];
			matTexProj = mEffect.Parameters["matTexProj"];
			matLightViewProj2 = mEffect.Parameters["matLightViewProj2"];
			matTexProj2 = mEffect.Parameters["matTexProj2"];
			matRotation = mEffect.Parameters["matRotation"];
			matBones = mEffect.Parameters["matBones"];
			matFPSWeaponTransform = mEffect.Parameters["matFPSWeaponTransform"];
			matSkinnedWorldTransform = mEffect.Parameters["matSkinnedWorldTransform"];
			vecLightDirection = mEffect.Parameters["vecLightDirection"];
			vecLightPosition = mEffect.Parameters["vecLightPosition"];
			vecLightColor = mEffect.Parameters["vecLightColor"];
			vecAmbientLightColor = mEffect.Parameters["vecAmbientLightColor"];
			vecFogColor = mEffect.Parameters["vecFogColor"];
			vecWorldOffset = mEffect.Parameters["vecWorldOffset"];
			vecEyePosition = mEffect.Parameters["vecEyePosition"];
			vecViewDirection = mEffect.Parameters["vecViewDirection"];
			vecTerrianParams = mEffect.Parameters["vecTerrianParams"];
			uvDisplacement = mEffect.Parameters["uvDisplacement"];
			vecFrustumCorners = mEffect.Parameters["vecFrustumCorners"];
			vecMuzzleFlash = mEffect.Parameters["vecMuzzleFlash"];
			numberLights = mEffect.Parameters["numberLights"];
			vecLightPositions = mEffect.Parameters["vecLightPositions"];
			vecLightColors = mEffect.Parameters["vecLightColors"];
			vecFPSLightPos = mEffect.Parameters["vecFPSLightPos"];
			vecFPSLightColor = mEffect.Parameters["vecFPSLightColor"];
			vecDefLightPosition = mEffect.Parameters["vecDefLightPosition"];
			vecDefLightColor = mEffect.Parameters["vecDefLightColor"];
			fDamage = mEffect.Parameters["fDamage"];
			fFadeToBlack = mEffect.Parameters["fFadeToBlack"];
			fShininess = mEffect.Parameters["fShininess"];
			fSpecularPower = mEffect.Parameters["fSpecularPower"];
			hGaussianScale = mEffect.Parameters["hGaussianScale"];
			vGaussianScale = mEffect.Parameters["vGaussianScale"];
			fBloomThresh = mEffect.Parameters["fBloomThresh"];
			fBloomIntensity = mEffect.Parameters["fBloomIntensity"];
			fBaseIntensity = mEffect.Parameters["fBaseIntensity"];
			fBloomSaturation = mEffect.Parameters["fBloomSaturation"];
			fBaseSaturation = mEffect.Parameters["fBaseSaturation"];
			fEditTimer = mEffect.Parameters["fEditTimer"];
			fReflectiveness = mEffect.Parameters["fReflectiveness"];
			fBlurFactor = mEffect.Parameters["fBlurFactor"];
			reticlePosition = mEffect.Parameters["reticlePosition"];
			fpsHandsDiffuse = mEffect.Parameters["fpsHandsDiffuse"];
			fpsHandsNormal = mEffect.Parameters["fpsHandsNormal"];
			fpsWeaponDiffuse = mEffect.Parameters["fpsWeaponDiffuse"];
			fpsWeaponNormal = mEffect.Parameters["fpsWeaponNormal"];
			laserTexture = mEffect.Parameters["laserTexture"];
			propDiffuse1 = mEffect.Parameters["propDiffuse1"];
			propDiffuse2 = mEffect.Parameters["propDiffuse2"];
			propNormal1 = mEffect.Parameters["propNormal1"];
			propNormal2 = mEffect.Parameters["propNormal2"];
			decalsDiffuse = mEffect.Parameters["decalsDiffuse"];
			decalsNormal = mEffect.Parameters["decalsNormal"];
			skinnedDiffuse = mEffect.Parameters["skinnedDiffuse"];
			skinnedNormal = mEffect.Parameters["skinnedNormal"];
			worldNormals = mEffect.Parameters["worldNormals"];
			worldTangents = mEffect.Parameters["worldTangents"];
			diffuseBlend = mEffect.Parameters["diffuseBlend"];
			diffuseTextures = mEffect.Parameters["diffuseTextures"];
			diffuseNorm1 = mEffect.Parameters["diffuseNorm1"];
			diffuseNorm2 = mEffect.Parameters["diffuseNorm2"];
			diffuseNorm3 = mEffect.Parameters["diffuseNorm3"];
			diffuseCliffTexture = mEffect.Parameters["diffuseCliffTexture"];
			stickersTexture = mEffect.Parameters["stickersTexture"];
			stickersNormTexture = mEffect.Parameters["stickersNormTexture"];
			editTexture = mEffect.Parameters["editTexture"];
			texCoords = mEffect.Parameters["texCoords"];
			Texture3 = mEffect.Parameters["Texture3"];
			Texture4 = mEffect.Parameters["Texture4"];
			Texture5 = mEffect.Parameters["Texture5"];
			Texture6 = mEffect.Parameters["Texture6"];
			Texture7 = mEffect.Parameters["Texture7"];
			Texture8 = mEffect.Parameters["Texture8"];
			Texture9 = mEffect.Parameters["Texture9"];
			TexParticle0 = mEffect.Parameters["TexParticle0"];
			TexParticle1 = mEffect.Parameters["TexParticle1"];
			EnvMap0 = mEffect.Parameters["EnvMap0"];
			TextureShadowMap = mEffect.Parameters["TextureShadowMap"];
			TextureShadowMap2 = mEffect.Parameters["TextureShadowMap2"];
			T_FPSWeapon = mEffect.Techniques["T_FPSWeapon"];
			T_PropTwoTextures = mEffect.Techniques["T_PropTwoTextures"];
			T_DecalsMaterial = mEffect.Techniques["T_DecalsMaterial"];
			T_ShadowMapProps = mEffect.Techniques["T_ShadowMapProps"];
			T_ShadowMapProps2 = mEffect.Techniques["T_ShadowMapProps2"];
			T_Terrain = mEffect.Techniques["T_Terrain"];
			T_DrawReticle = mEffect.Techniques["T_DrawReticle"];
			T_DrawStickers = mEffect.Techniques["T_DrawStickers"];
			T_DrawDebugPathing = mEffect.Techniques["T_DrawDebugPathing"];
			T_EditObject = mEffect.Techniques["T_EditObject"];
			T_GimbalObject = mEffect.Techniques["T_GimbalObject"];
			T_PropFoilage = mEffect.Techniques["T_PropFoilage"];
			T_Skinned = mEffect.Techniques["T_Skinned"];
			T_SkinnedShadow = mEffect.Techniques["T_SkinnedShadow"];
			T_PropObject = mEffect.Techniques["T_PropObject"];
			T_WorldObject = mEffect.Techniques["T_WorldObject"];
			T_RenderDepth = mEffect.Techniques["T_RenderDepth"];
			T_CopyQuad = mEffect.Techniques["T_CopyQuad"];
			T_SkinnedModel = mEffect.Techniques["T_SkinnedModel"];
			T_WeaponModel = mEffect.Techniques["T_WeaponModel"];
			T_Particle = mEffect.Techniques["T_Particle"];
			T_DebugTriangle = mEffect.Techniques["T_DebugTriangle"];
			T_HDRDownSample = mEffect.Techniques["T_HDRDownSample"];
			T_HDRBlur = mEffect.Techniques["T_HDRBlur"];
			T_HDRToneMap = mEffect.Techniques["T_HDRToneMap"];
			T_UI = mEffect.Techniques["T_UI"];
		}
	}

	public static Exception EngineCrashException = null;

	public static Game EGEGame;

	public static PlayerIndex? controllingPlayer = null;

	public static PlayerIndex? guestPlayer = null;

	public static float currentTimeStep = 0.01667f;

	public static GameTime currentEleapsedTime = new GameTime();

	public static bool enableInputUpdate = true;

	public static bool enablePhysicsUpdate = true;

	public static bool enableClearTarget = true;

	public static bool StartInLevelMenuUpdate = false;

	public static bool GamerSigningIn = false;

	public static Random randGenerator = new Random();

	public static Exception ThreadExceptionArgument = null;

	public static int FIXED_TIME_STEP = 333400;

	public static float fFIXED_TIME_STEP = 0.03334f;

	public static MyContentManager ContentMgr;

	public static MyContentManager GameAssetMgr;

	public static GraphicsDeviceManager GraphicMgr;

	public static GameSettingClass GameSettings;

	public static Effect MaterialEffect;

	public static MaterialEffectParams MaterialParams = new MaterialEffectParams();

	public static bool LogoPlayed = false;

	public static bool LogoDonePlayed = false;

	public static bool StoryPlayed = false;

	public static Video logoVideo;

	public static Video storyVideo;

	public static Texture2D videoTexture;

	public static VideoPlayer videoPlayer;

	public static Rectangle LogoVideoSrcRectangle = new Rectangle(0, 0, 1280, 720);

	public static Rectangle LogoVideoDstRectangle = new Rectangle(64, 36, 1152, 648);

	public static MenuMgr menuMgr = new MenuMgr();

	public static LevelBaseMenu LevelMgr = null;

	public static BlendState BlendOpaque = new BlendState();

	public static BlendState BlendOpaqueNoColorChannel = new BlendState();

	public static BlendState BlendPreAlphaNoWriteAlpha = new BlendState();

	public static BlendState BlendAlphaNoWriteAlpha = new BlendState();

	public static BlendState BlendAlphaTestNoWriteAlpha = new BlendState();

	public static BlendState BlendStickers = new BlendState();

	public static BlendState BlendNoWriteColor = new BlendState();

	public static DepthStencilState DepthEnabled = new DepthStencilState();

	public static DepthStencilState DepthDisabled = new DepthStencilState();

	public static DepthStencilState DepthWriteOnly = new DepthStencilState();

	public static DepthStencilState DepthNoWrite = new DepthStencilState();

	public static DepthStencilState DepthRender = new DepthStencilState();

	public static DepthStencilState DepthOutSidePointLight = new DepthStencilState();

	public static DepthStencilState DepthInSidePointLight = new DepthStencilState();

	public static RasterizerState RasterCullCW = new RasterizerState();

	public static RasterizerState RasterCullCC = new RasterizerState();

	public static RasterizerState RasterCullNone = new RasterizerState();

	public static SamplerState SamplerLinearWrap = new SamplerState();

	public static AudioEngine AudioEng;

	public static WaveBank WaveBnk;

	public static WaveBank WaveBnkStreaming;

	public static SoundBank SoundBnk;

	public static Viewport DefualtViewport;

	public static GamerServicesComponent GamerServices { get; private set; }

	public EndGameEngine()
	{
		EGEGame = this;
		GraphicMgr = new GraphicsDeviceManager(this);
		ContentMgr = new MyContentManager(base.Content.ServiceProvider);
		ContentMgr.RootDirectory = "EngineContent";
		GameAssetMgr = new MyContentManager(base.Content.ServiceProvider, "GameContent");
		GameSettings = GameAssetMgr.Load<GameSettingClass>("data\\GameSettingsXML");
		LogoPlayed = !GameSettings.PlayLogoVideo;
		StoryPlayed = !GameSettings.PlayLogoVideo;
		Guide.SimulateTrialMode = GameSettings.SimulateTrialMode;
		FIXED_TIME_STEP = GameSettings.TimeStepMilliseconds;
		fFIXED_TIME_STEP = (float)GameSettings.TimeStepMilliseconds / 10000000f;
		GamerServices = new GamerServicesComponent(this);
		base.Components.Add(GamerServices);
		GraphicMgr.PreferredBackBufferWidth = GameSettings.RenderTargetSizeX;
		GraphicMgr.PreferredBackBufferHeight = GameSettings.RenderTargetSizeY;
		GraphicMgr.PreferMultiSampling = false;
		GraphicMgr.PreferredBackBufferFormat = SurfaceFormat.HdrBlendable;
		GraphicMgr.SynchronizeWithVerticalRetrace = GameSettings.VerticalSync;
		GraphicMgr.ApplyChanges();
		BlendOpaqueNoColorChannel.ColorWriteChannels = ColorWriteChannels.None;
		BlendPreAlphaNoWriteAlpha.AlphaBlendFunction = BlendFunction.Add;
		BlendPreAlphaNoWriteAlpha.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		BlendPreAlphaNoWriteAlpha.AlphaSourceBlend = Blend.One;
		BlendPreAlphaNoWriteAlpha.ColorBlendFunction = BlendFunction.Add;
		BlendPreAlphaNoWriteAlpha.ColorDestinationBlend = Blend.InverseSourceAlpha;
		BlendPreAlphaNoWriteAlpha.ColorSourceBlend = Blend.One;
		BlendPreAlphaNoWriteAlpha.MultiSampleMask = -1;
		BlendPreAlphaNoWriteAlpha.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		BlendAlphaNoWriteAlpha.AlphaBlendFunction = BlendFunction.Add;
		BlendAlphaNoWriteAlpha.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		BlendAlphaNoWriteAlpha.AlphaSourceBlend = Blend.SourceAlpha;
		BlendAlphaNoWriteAlpha.ColorBlendFunction = BlendFunction.Add;
		BlendAlphaNoWriteAlpha.ColorDestinationBlend = Blend.InverseSourceAlpha;
		BlendAlphaNoWriteAlpha.ColorSourceBlend = Blend.SourceAlpha;
		BlendAlphaNoWriteAlpha.MultiSampleMask = -1;
		BlendAlphaNoWriteAlpha.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		BlendAlphaNoWriteAlpha.ColorWriteChannels1 = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		BlendAlphaNoWriteAlpha.ColorWriteChannels2 = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		BlendAlphaNoWriteAlpha.ColorWriteChannels3 = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		BlendAlphaTestNoWriteAlpha.AlphaBlendFunction = BlendFunction.Add;
		BlendAlphaTestNoWriteAlpha.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		BlendAlphaTestNoWriteAlpha.AlphaSourceBlend = Blend.One;
		BlendAlphaTestNoWriteAlpha.ColorBlendFunction = BlendFunction.Add;
		BlendAlphaTestNoWriteAlpha.ColorDestinationBlend = Blend.InverseSourceAlpha;
		BlendAlphaTestNoWriteAlpha.ColorSourceBlend = Blend.One;
		BlendAlphaTestNoWriteAlpha.MultiSampleMask = -1;
		BlendAlphaTestNoWriteAlpha.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
		BlendStickers.AlphaBlendFunction = BlendFunction.Add;
		BlendStickers.AlphaDestinationBlend = Blend.One;
		BlendStickers.AlphaSourceBlend = Blend.Zero;
		BlendStickers.BlendFactor = new Color(255, 255, 255, 255);
		BlendStickers.ColorBlendFunction = BlendFunction.Add;
		BlendStickers.ColorDestinationBlend = Blend.SourceColor;
		BlendStickers.ColorSourceBlend = Blend.Zero;
		BlendStickers.MultiSampleMask = -1;
		BlendStickers.ColorWriteChannels = ColorWriteChannels.All;
		DepthEnabled.DepthBufferEnable = true;
		DepthEnabled.DepthBufferWriteEnable = true;
		DepthDisabled.DepthBufferEnable = false;
		DepthDisabled.DepthBufferWriteEnable = false;
		DepthWriteOnly.DepthBufferEnable = false;
		DepthWriteOnly.DepthBufferWriteEnable = true;
		DepthNoWrite.DepthBufferEnable = true;
		DepthNoWrite.DepthBufferWriteEnable = false;
		DepthRender.DepthBufferEnable = true;
		DepthRender.DepthBufferWriteEnable = true;
		DepthInSidePointLight.DepthBufferEnable = true;
		DepthInSidePointLight.DepthBufferWriteEnable = false;
		DepthInSidePointLight.DepthBufferFunction = CompareFunction.Greater;
		DepthInSidePointLight.StencilEnable = true;
		DepthInSidePointLight.StencilFunction = CompareFunction.Equal;
		DepthInSidePointLight.StencilPass = StencilOperation.Replace;
		DepthInSidePointLight.ReferenceStencil = 1;
		DepthOutSidePointLight.DepthBufferEnable = true;
		DepthOutSidePointLight.DepthBufferWriteEnable = false;
		DepthOutSidePointLight.DepthBufferFunction = CompareFunction.LessEqual;
		DepthOutSidePointLight.StencilEnable = true;
		DepthOutSidePointLight.StencilFunction = CompareFunction.Equal;
		DepthOutSidePointLight.StencilPass = StencilOperation.Replace;
		DepthOutSidePointLight.ReferenceStencil = 1;
		RasterCullCW.CullMode = CullMode.CullClockwiseFace;
		RasterCullCW.FillMode = FillMode.Solid;
		RasterCullCW.MultiSampleAntiAlias = true;
		RasterCullCC.CullMode = CullMode.CullCounterClockwiseFace;
		RasterCullCC.FillMode = FillMode.Solid;
		RasterCullCC.MultiSampleAntiAlias = true;
		RasterCullNone.CullMode = CullMode.None;
		RasterCullNone.FillMode = FillMode.Solid;
		RasterCullNone.MultiSampleAntiAlias = true;
		SamplerLinearWrap.AddressU = TextureAddressMode.Mirror;
		SamplerLinearWrap.AddressV = TextureAddressMode.Mirror;
		SamplerLinearWrap.AddressW = TextureAddressMode.Mirror;
		SamplerLinearWrap.Filter = TextureFilter.Linear;
		DefualtViewport = GraphicMgr.GraphicsDevice.Viewport;
	}

	protected override void LoadContent()
	{
		base.IsFixedTimeStep = GameSettings.FixedTimeStep;
		base.TargetElapsedTime = TimeSpan.FromSeconds((float)FIXED_TIME_STEP * 1E-07f);
		GraphicMgr.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
		GraphicMgr.ApplyChanges();
		MaterialEffect = ContentMgr.Load<Effect>("shaders\\ShaderMaterials");
		MaterialParams.InitEffectParams(MaterialEffect);
		menuMgr.LoadContent();
		LevelBaseMenu.LoadBaseContent();
		LevelBaseMenu.InputUpdate.LoadContent();
		videoPlayer = new VideoPlayer();
	}

	protected override void UnloadContent()
	{
		base.UnloadContent();
		GraphicMgr.GraphicsDevice.Dispose();
		ContentMgr.Dispose();
		GameAssetMgr.Dispose();
		GamerServices.Dispose();
	}

	public virtual void HandleInput()
	{
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(PlayerIndex.One);
		if (state.IsKeyDown(Keys.Escape) || state2.Buttons.Back == ButtonState.Pressed)
		{
			Exit();
		}
	}

	public static void UpdatePresence(GamerPresenceMode e)
	{
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			signedInGamer.Presence.PresenceMode = e;
		}
	}

	public virtual void GetControllingPlayer()
	{
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed)
			{
				LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
				LevelBaseMenu.InputUpdate.menuInputContinuos = MenuInput.None;
				LevelBaseMenu.InputUpdate.menuInputRightStick = MenuInput.None;
				GamePad.GetState(playerIndex);
				controllingPlayer = playerIndex;
				SignedInGamer signedInGamer = Gamer.SignedInGamers[controllingPlayer.Value];
				if (signedInGamer == null && !Guide.IsVisible)
				{
					Guide.ShowSignIn(1, onlineOnly: false);
					GamerSigningIn = true;
				}
				break;
			}
		}
	}

	public virtual bool SkipVideo()
	{
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.Back == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed)
			{
				LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
				LevelBaseMenu.InputUpdate.menuInputContinuos = MenuInput.None;
				LevelBaseMenu.InputUpdate.menuInputRightStick = MenuInput.None;
				GamePad.GetState(playerIndex);
				return true;
			}
		}
		return false;
	}
}
