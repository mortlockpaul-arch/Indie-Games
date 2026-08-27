using System;
using System.Threading;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine;

public class LevelGame : LevelBaseMenu
{
	private Texture2D RandNormal128;

	private Vector3[] frustumCorners = new Vector3[4];

	private Vector4 FinalSunPosition = Vector4.Zero;

	private Vector4 FinalSunColor = Vector4.Zero;

	private Vector4 FinalAmbientColor = Vector4.Zero;

	private Vector4 textureOffset = new Vector4(0f, 0f, 1f, 1f);

	private Vector3 lightPosition1 = Vector3.Zero;

	private Vector3 lightPosition2 = Vector3.Zero;

	private Vector3 vecWaterColor = new Vector3(61f, 102f, 94f);

	private float WaterSpecPower = 512f;

	private float WaterSpec = 1f;

	private float ReflectSpecPower = 1.05f;

	private float ReflectSpec;

	private float WaterHeight = 1200f;

	private Vector4 VecWaveFrequency = new Vector4(0.001f, -0.001f, -0.0015f, 0f);

	private Vector4 VecWaveScalar = new Vector4(1f, 0.5f, 6f, 1f);

	private Vector4 VecWaveDistortion = new Vector4(0.1f, 0.1f, 0.001f, 0.1f);

	private float xOffset;

	private float yOffset;

	private Vector3 lp = Vector3.Zero;

	private Vector4 lightColor = new Vector4(0.7490196f, 0.7490196f, 0.6784314f, 1f);

	private Vector4 ambientColor = new Vector4(0.35f, 0.35f, 0.4f, 1f);

	private Vector4 tmpTexScale = Vector4.Zero;

	private Vector2 texelOffset = Vector2.Zero;

	private Vector3 tmpTestWaterClr = Vector3.Zero;

	private static Vector4 depthClearColor = new Vector4(60000f, 60000f, 60000f, 60000f);

	private Vector3 lightPosition = Vector3.Zero;

	private Matrix lightViewTranspose = Matrix.Identity;

	private Matrix lightViewProjection = Matrix.Identity;

	private float WaveCoolDownTimer;

	private float WaveStartTimer;

	public LevelGame(GameMenus id)
		: base(id)
	{
		LevelBaseMenu.AvRai = new SimpleZombieAI();
	}

	public void NewPreLoadContent()
	{
		LevelBaseMenu.gdLoadScreen = EndGameEngine.GraphicMgr.GraphicsDevice;
		LevelBaseMenu.loadStartTime = EndGameEngine.currentEleapsedTime;
		LevelBaseMenu.LoadContentEnabled = true;
		LevelBaseMenu.LoadState = LevelLoadState.Loading;
		HeightMapPhysics.Initialize();
		HeightMapPhysics.Set(20000, 800);
		LevelBaseMenu.tmpTerrain.Initialize("");
		LevelBaseMenu.tmpTerrain.Load("null");
		LevelBaseMenu.tmpTerrainRoad.Load("models\\props\\road20");
		LevelBaseMenu.AvRai.Initialize();
		LevelBaseMenu.tmpTerrainVegitation.Initialize("");
		LevelBaseMenu.tmpTerrainVegitation.Load("");
		HeightMapPhysics.FinalizeHeightMap();
		GC.Collect();
		LevelBaseMenu.tmpTerrain.Finalize();
		LevelBaseMenu.tmpTerrainRoad.Finalize();
		LevelBaseMenu.AvRai.Finalize();
		LevelBaseMenu.tmpTerrainVegitation.Finalize();
		HeightMapPhysics.FinalizeMaps();
		GC.Collect();
		LevelBaseMenu.backgraoundTexture = null;
		PrepareUpdateload();
		Thread thread = new Thread(LoadContent);
		thread.Start();
		Thread.Sleep(5);
	}

	public void NewUpdateLoad(GameTime gameTime)
	{
		UpdateLoad(gameTime);
	}

	public override void LoadContent()
	{
		if (LevelBaseMenu.LoadContentEnabled)
		{
			PlayerBase.FarZPlane = 60000f;
			PlayerBase.FogStart = 1500f;
			PlayerBase.FogEnd = 32000f;
			PlayerBase.ApocalypseZ_Hack = true;
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.GetTotalMemory(forceFullCollection: false);
			LevelOutside.LoadContent(EndGameEngine.GameAssetMgr);
			LevelOutside.SunColor = new Vector4(0.4f, 0.4f, 0.55f, 1f);
			LevelOutside.SunShadowColor = new Vector4(0.05f, 0.05f, 0.06f, 1f);
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.EnvMap = EndGameEngine.GameAssetMgr.Load<TextureCube>("textures\\envmap");
			EndGameEngine.MaterialParams.EnvMap0.SetValue(LevelBaseMenu.EnvMap);
			LevelBaseMenu.LoadProgressCounter++;
			base.LoadContent();
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.PostEffectsClass.Initialize(9);
			EndGameEngine.menuMgr.AddMenu(new FPSGameMenu(GameMenus.FPSGame));
			EndGameEngine.menuMgr.AddMenu(new ControllerMenu(GameMenus.FPSControllerMenu));
			EndGameEngine.menuMgr.AddMenu(new XBoxLiveMenu(GameMenus.XBoxLiveMenu));
			LevelBaseMenu.viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
			LevelBaseMenu.aspectRatio = (float)LevelBaseMenu.viewport.Width / (float)LevelBaseMenu.viewport.Height;
			LevelBaseMenu.mDataQueue = new DataQueue[2];
			for (int i = 0; i < 2; i++)
			{
				LevelBaseMenu.mDataQueue[i] = new DataQueue();
				LevelBaseMenu.mDataQueue[i].status = 0;
				LevelBaseMenu.mDataQueue[i].cameraPos = Vector3.Zero;
				LevelBaseMenu.mDataQueue[i].cameralookAt = Vector3.UnitX;
				LevelBaseMenu.mDataQueue[i].cameraUp = Vector3.UnitY;
				LevelBaseMenu.mDataQueue[i].projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, LevelBaseMenu.aspectRatio, 1f, 100000f);
			}
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.physBase.LoadContent();
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.physBase.LoadRagDolls();
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.physBase.LoadBEPUSpace();
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.LoadRenderTargets();
			LevelBaseMenu.LoadProgressCounter++;
			LevelBaseMenu.debugUpdateCounter = 0;
			LevelBaseMenu.debugPhysicsCounter = 0;
			RandNormal128 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\RandNormal128");
			LevelBaseMenu.texWaterNormal = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\WaterNormal128");
			LevelBaseMenu.texWaterDetail = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\WaterNormalDetail");
			Vector3 pos = new Vector3(-1f, 1f, 0f);
			Vector3 pos2 = new Vector3(1f, 1f, 0f);
			Vector3 pos3 = new Vector3(-1f, -1f, 0f);
			Vector3 pos4 = new Vector3(1f, -1f, 0f);
			new Color(255, 255, 255, 255);
			VS_PostStruct[] data = new VS_PostStruct[4]
			{
				new VS_PostStruct(pos, new Vector2(0f + xOffset, 0f + yOffset), 0f),
				new VS_PostStruct(pos2, new Vector2(1f - xOffset, 0f + yOffset), 1f),
				new VS_PostStruct(pos3, new Vector2(0f + xOffset, 1f - yOffset), 3f),
				new VS_PostStruct(pos4, new Vector2(1f - xOffset, 1f - yOffset), 2f)
			};
			LevelBaseMenu.postVertexBuffer.SetData(data);
			LevelBaseMenu.LoadContentEnabled = false;
			LevelBaseMenu.LoadState = LevelLoadState.Loaded;
		}
	}

	public override void UnLoadContent()
	{
		base.UnLoadContent();
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		if (LevelBaseMenu.LoadState == LevelLoadState.NotLoaded)
		{
			LevelBaseMenu.gdLoadScreen = EndGameEngine.GraphicMgr.GraphicsDevice;
			LevelBaseMenu.loadStartTime = EndGameEngine.currentEleapsedTime;
			LevelBaseMenu.LoadContentEnabled = true;
			LevelBaseMenu.LoadState = LevelLoadState.Loading;
			LevelBaseMenu.backgraoundTexture = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\menus\\ss6");
			Thread thread = new Thread(base.UpdateLoadThread);
			thread.Start();
			Thread.Sleep(1);
			LevelBaseMenu.LoadContentEnabled = true;
			LoadContent();
		}
		else
		{
			_ = LevelBaseMenu.LoadState;
			_ = 2;
		}
	}

	public void UpdateThreadRunNew()
	{
		LevelBaseMenu.UpdateThreadRunning = true;
		Thread thread = new Thread(base.UpdateThreadNew);
		thread.Start();
		Thread.Sleep(1);
		Thread thread2 = new Thread(UpdateThreadPhysics);
		thread2.Start();
		Thread.Sleep(1);
		GameEngine.AudioUpdateOnLoadRunning = false;
	}

	public override void UpdateMenuInnerLoop(int qIndex)
	{
		float currentTimeStep = EndGameEngine.currentTimeStep;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort = EndGameEngine.DefualtViewport;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AspectRatio = (float)LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort.Width / (float)LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort.Height;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition.X = -900f;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition.Y = 500f;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition.Z = -900f;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.X = 0f;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.Y = 0f;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.Z = 400f;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition;
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.Normalize();
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view = Matrix.CreateLookAt(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition + LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection * 1000f, Vector3.UnitY);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AspectRatio, PlayerBase.NearZPlane, PlayerBase.FarZPlane);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Matrix = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view * LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection;
		LevelBaseMenu.mDataQueue[qIndex].world = Matrix.Identity;
		LevelOutside.Update(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 0);
		LevelBaseMenu.Emitters.Update(currentTimeStep);
		MediaEmitterClass.Update(currentTimeStep, qIndex);
		LevelBaseMenu.PointLights.Update(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
		LevelBaseMenu.Particles.Update(currentTimeStep, qIndex);
		LevelBaseMenu.Particles.UpdatePlayer(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
	}

	public override void Update(float eTime)
	{
		base.Update(eTime);
	}

	public override void DrawGameLevel(int qIndex)
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
		graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[6] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[7] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[8] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[9] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[10] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[11] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[12] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[13] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[14] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[15] = SamplerState.PointWrap;
		DrawShadowMap(qIndex);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.SetRenderTargets(LevelBaseMenu.RenderTargetBindings);
		graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.White, 1f, 0);
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialParams.matWorld.SetValue(Matrix.Identity);
		lp.X = LevelOutside.SunPosition.X - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].X;
		lp.Y = LevelOutside.SunPosition.Y;
		lp.Z = LevelOutside.SunPosition.Z - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].Z;
		lightColor.X = 0.7490196f;
		lightColor.Y = 0.7490196f;
		lightColor.Z = 0.6784314f;
		lightColor.W = 1f;
		ambientColor.X = 0.35f;
		ambientColor.Y = 0.35f;
		ambientColor.Z = 0.4f;
		ambientColor.W = 1f;
		materialParams.vecLightPosition.SetValue(lp);
		materialParams.vecLightColor.SetValue(lightColor);
		materialParams.vecAmbientLightColor.SetValue(ambientColor);
		for (int i = 0; i < 4; i++)
		{
			if (LevelBaseMenu.Players[i].IsValid)
			{
				LevelBaseMenu.Players[i].SetViewPortTestCoOp(PlayerBase.RenderPass.GBufferPass, qIndex);
				DrawPlayer(LevelBaseMenu.Players[i], qIndex, null, i);
				LevelBaseMenu.Players[i].Draw(qIndex);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (LevelBaseMenu.Players[j].IsValid)
			{
				EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(LevelBaseMenu.compositeRenderTarget);
				EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 1);
				LevelBaseMenu.Players[j].SetViewPortTestCoOp(LevelBaseMenu.compositeRenderTarget.Width, LevelBaseMenu.compositeRenderTarget.Height, qIndex);
				tmpTexScale.X = LevelBaseMenu.Players[j].vpViewPort.X;
				tmpTexScale.Y = LevelBaseMenu.Players[j].vpViewPort.Y;
				tmpTexScale.Z = LevelBaseMenu.Players[j].vpViewPort.Width;
				tmpTexScale.W = LevelBaseMenu.Players[j].vpViewPort.Height;
				LevelBaseMenu.EffectDirectionalLight.Parameters["vecTextureReadScale"].SetValue(tmpTexScale);
				graphicsDevice.BlendState = BlendState.Opaque;
				graphicsDevice.DepthStencilState = DepthStencilState.Default;
				graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
				graphicsDevice.SetVertexBuffer(LevelBaseMenu.postVertexBuffer);
				Vector3[] corners = LevelBaseMenu.Players[j].bFrustum[qIndex].GetCorners();
				for (int k = 0; k < 4; k++)
				{
					ref Vector3 reference = ref frustumCorners[k];
					reference = corners[k + 4] - LevelBaseMenu.Players[j].mDataQueue[qIndex].cameraEyePos;
				}
				LevelBaseMenu.vecEyePosition.SetValue(LevelBaseMenu.Players[j].mDataQueue[qIndex].cameraEyePos);
				FinalSunPosition.X = LevelOutside.SunPosition.X;
				FinalSunPosition.Y = LevelOutside.SunPosition.Y;
				FinalSunPosition.Z = LevelOutside.SunPosition.Z;
				FinalSunPosition.W = 1f;
				LevelBaseMenu.vecDirectLightPosition.SetValue(FinalSunPosition);
				FinalSunColor.X = LevelOutside.CurrentColor.X * LevelOutside.VideoBrightness;
				FinalSunColor.Y = LevelOutside.CurrentColor.Y * LevelOutside.VideoBrightness;
				FinalSunColor.Z = LevelOutside.CurrentColor.Z * LevelOutside.VideoBrightness;
				FinalSunColor.W = 1f;
				FinalAmbientColor.X = LevelOutside.CurrentAmbient.X * LevelOutside.VideoBrightness;
				FinalAmbientColor.Y = LevelOutside.CurrentAmbient.Y * LevelOutside.VideoBrightness;
				FinalAmbientColor.Z = LevelOutside.CurrentAmbient.Z * LevelOutside.VideoBrightness;
				FinalAmbientColor.W = 1f;
				LevelBaseMenu.vecDirectLightColor.SetValue(FinalSunColor);
				LevelBaseMenu.vecAmbientColor.SetValue(FinalAmbientColor);
				LevelBaseMenu.matViewProj.SetValue(LevelBaseMenu.Players[j].mDataQueue[qIndex].view * LevelBaseMenu.Players[j].mDataQueue[qIndex].projection);
				LevelBaseMenu.matInvViewProj.SetValue(LevelBaseMenu.Players[j].mDataQueue[qIndex].invViewProj);
				LevelBaseMenu.matInvView.SetValue(Matrix.Invert(LevelBaseMenu.Players[j].mDataQueue[qIndex].view));
				LevelBaseMenu.matInvProj.SetValue(Matrix.Invert(LevelBaseMenu.Players[j].mDataQueue[qIndex].projection));
				lightPosition1 = Vector3.Transform(-LevelBaseMenu.Players[j].mDataQueue[qIndex].lightView2[0].Translation, Matrix.Transpose(LevelBaseMenu.Players[j].mDataQueue[qIndex].lightView2[0]));
				lightPosition2 = Vector3.Transform(-LevelBaseMenu.Players[j].mDataQueue[qIndex].lightView2[1].Translation, Matrix.Transpose(LevelBaseMenu.Players[j].mDataQueue[qIndex].lightView2[1]));
				LevelBaseMenu.EffectDirectionalLight.Parameters["vecLightPos1"].SetValue(lightPosition1);
				LevelBaseMenu.EffectDirectionalLight.Parameters["vecLightPos2"].SetValue(lightPosition2);
				LevelBaseMenu.EffectDirectionalLight.Parameters["matTexProj"].SetValue(LevelBaseMenu.Players[j].mDataQueue[qIndex].lightView2[0] * LevelBaseMenu.Players[j].mDataQueue[qIndex].lightProj2[0] * LevelBaseMenu.matTextureProj);
				LevelBaseMenu.EffectDirectionalLight.Parameters["matTexProj2"].SetValue(LevelBaseMenu.Players[j].mDataQueue[qIndex].lightView2[1] * LevelBaseMenu.Players[j].mDataQueue[qIndex].lightProj2[1] * LevelBaseMenu.matTextureProj);
				LevelBaseMenu.EffectDirectionalLight.Parameters["TextureShadowMap1"].SetValue(LevelBaseMenu.shadowRenderTarget2[0]);
				LevelBaseMenu.EffectDirectionalLight.Parameters["TextureShadowMap2"].SetValue(LevelBaseMenu.shadowRenderTarget2[1]);
				LevelBaseMenu.NormalTexture.SetValue(LevelBaseMenu.NormalRenderTarget);
				LevelBaseMenu.DiffuseTexture.SetValue(LevelBaseMenu.DiffuseRenderTarget);
				LevelBaseMenu.DepthTexture.SetValue(LevelBaseMenu.DepthRenderTarget);
				LevelBaseMenu.MaterialsTexture.SetValue(LevelBaseMenu.MaterialRenderTarget);
				LevelBaseMenu.EnvMap0.SetValue(LevelBaseMenu.EnvMap);
				LevelBaseMenu.vecFrustumCorners.SetValue(frustumCorners);
				LevelBaseMenu.EffectDirectionalLight.Parameters["fFarZPlane"].SetValue(PlayerBase.FarZPlane);
				LevelBaseMenu.EffectDirectionalLight.Parameters["fogStart"].SetValue(PlayerBase.FogStart);
				LevelBaseMenu.EffectDirectionalLight.Parameters["fogEnd"].SetValue(PlayerBase.FogEnd);
				LevelBaseMenu.EffectDirectionalLight.Parameters["fogColor"].SetValue(LevelBaseMenu.FogColor * (0.1f + LevelOutside.DayLightScalar));
				LevelBaseMenu.EffectDirectionalLight.Parameters["fSpecularPower"].SetValue(32f);
				LevelBaseMenu.EffectDirectionalLight.Parameters["fSpecularIntensity"].SetValue(8f);
				texelOffset.X = -1f / (float)EndGameEngine.GameSettings.BackBufferSizeX;
				texelOffset.Y = 1f / (float)EndGameEngine.GameSettings.BackBufferSizeY;
				LevelBaseMenu.EffectDirectionalLight.Parameters["texelSpaceAdjust"].SetValue(texelOffset);
				LevelBaseMenu.EffectDirectionalLight.Parameters["vecWorldPosition"].SetValue(LevelBaseMenu.Players[j].vecHeadPosition[qIndex]);
				LevelBaseMenu.EffectDirectionalLight.CurrentTechnique = LevelBaseMenu.T_DirectLight;
				graphicsDevice.BlendState = BlendState.Opaque;
				graphicsDevice.DepthStencilState = DepthStencilState.Default;
				EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(LevelBaseMenu.compositeRenderTarget);
				EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 1);
				textureOffset += VecWaveFrequency;
				LevelBaseMenu.EffectDirectionalLight.Parameters["textureOffset"].SetValue(textureOffset);
				LevelBaseMenu.EffectDirectionalLight.Parameters["VecWaveScalar"].SetValue(VecWaveScalar);
				LevelBaseMenu.EffectDirectionalLight.Parameters["VecWaveDistortion"].SetValue(VecWaveDistortion);
				LevelBaseMenu.EffectDirectionalLight.Parameters["WaterNormalMapTexture"].SetValue(LevelBaseMenu.texWaterNormal);
				LevelBaseMenu.EffectDirectionalLight.Parameters["WaterDetailMapTexture"].SetValue(LevelBaseMenu.texWaterDetail);
				tmpTestWaterClr.X = vecWaterColor.X / 255f;
				tmpTestWaterClr.Y = vecWaterColor.Y / 255f;
				tmpTestWaterClr.Z = vecWaterColor.Z / 255f;
				LevelBaseMenu.EffectDirectionalLight.Parameters["InWaterColor"].SetValue(tmpTestWaterClr);
				LevelBaseMenu.EffectDirectionalLight.Parameters["WaterSpecPower"].SetValue(WaterSpecPower);
				LevelBaseMenu.EffectDirectionalLight.Parameters["WaterSpec"].SetValue(WaterSpec);
				LevelBaseMenu.EffectDirectionalLight.Parameters["ReflectSpecPower"].SetValue(ReflectSpecPower);
				LevelBaseMenu.EffectDirectionalLight.Parameters["ReflectSpec"].SetValue(ReflectSpec);
				LevelBaseMenu.EffectDirectionalLight.Parameters["WaterHeight"].SetValue(WaterHeight);
				LevelBaseMenu.EffectDirectionalLight.Parameters["DayLightScalar"].SetValue(LevelOutside.NightTimeScalar);
				if (LevelBaseMenu.Players[j].mDataQueue[qIndex].cameraEyePos.Y < WaterHeight)
				{
					LevelBaseMenu.EffectDirectionalLight.CurrentTechnique.Passes[9].Apply();
				}
				else
				{
					LevelBaseMenu.EffectDirectionalLight.CurrentTechnique.Passes[10].Apply();
				}
				graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			}
		}
		graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
		graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[6] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[7] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[8] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[9] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[10] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[11] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[12] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[13] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[14] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[15] = SamplerState.PointWrap;
		LevelBaseMenu.PointLights.ProcessDefferedLights(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex, 32f, 8f, 0.75f);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		LevelBaseMenu.Particles.Draw(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 1, qIndex);
		SimpleZombieAI.DrawAlpha(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
		EGENetWorkNext.DrawAlpha(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
		EGENetWorkNext.DrawMuzzleFlash(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].DrawMuzzleFlash(qIndex);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].DrawPostLens(qIndex);
		LevelOutside.Draw(RenderPass.AlphaBlend, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 0, qIndex);
		LevelBaseMenu.PostEffectsClass.Bloom(qIndex);
		LevelBaseMenu.PostEffectsClass.Particles(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		PostMenuUI.Draw(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
		DrawPost(qIndex);
	}

	public void DrawGameLevelForwardRender(int qIndex)
	{
		EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
		EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTargets((RenderTargetBinding[])null);
		EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialParams.matWorld.SetValue(Matrix.Identity);
		Vector3 zero = Vector3.Zero;
		zero.X = LevelOutside.SunPosition.X;
		zero.Y = LevelOutside.SunPosition.Y;
		zero.Z = LevelOutside.SunPosition.Z;
		Vector4 value = new Vector4(1f, 1f, 1f, 1f);
		Vector4 value2 = new Vector4(0.35f, 0.35f, 0.4f, 1f);
		materialParams.vecLightPosition.SetValue(zero);
		materialParams.vecLightColor.SetValue(value);
		materialParams.vecAmbientLightColor.SetValue(value2);
		LevelOutside.Draw(RenderPass.ForwardRender, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 0, qIndex);
		PostMenuUI.Draw(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
	}

	public override void DrawMenuLevel(int qIndex)
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		DrawShadowMap(qIndex);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.SetRenderTargets(LevelBaseMenu.RenderTargetBindings);
		graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialParams.matWorld.SetValue(Matrix.Identity);
		Vector3 zero = Vector3.Zero;
		zero.X = LevelOutside.SunPosition.X;
		zero.Y = LevelOutside.SunPosition.Y;
		zero.Z = LevelOutside.SunPosition.Z;
		Vector4 value = new Vector4(1f, 1f, 1f, 1f);
		Vector4 value2 = new Vector4(0.35f, 0.35f, 0.4f, 1f);
		materialParams.vecLightPosition.SetValue(zero);
		materialParams.vecLightColor.SetValue(value);
		materialParams.vecAmbientLightColor.SetValue(value2);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].SetViewPortForPass(PlayerBase.RenderPass.MenuGBufferPass, qIndex);
		materialParams.matWorld.SetValue(Matrix.Identity);
		materialParams.matView.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view);
		materialParams.matProj.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection);
		materialParams.matViewProj.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view * LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection);
		EndGameEngine.DefualtViewport.Width = EndGameEngine.GameSettings.GBufferSizeX;
		EndGameEngine.DefualtViewport.Height = EndGameEngine.GameSettings.GBufferSizeY;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		LevelOutside.Draw(RenderPass.Normal, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 0, qIndex);
		EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(LevelBaseMenu.compositeRenderTarget);
		EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Black, 1f, 0);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].SetViewPortForPass(PlayerBase.RenderPass.ForwardPass, qIndex);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(LevelBaseMenu.postVertexBuffer);
		Vector3 zero2 = Vector3.Zero;
		zero2 = Vector3.Transform(-LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view.Translation, Matrix.Transpose(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view));
		Vector3[] corners = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].GetCorners();
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref frustumCorners[i];
			reference = corners[i + 4] - zero2;
		}
		LevelBaseMenu.vecEyePosition.SetValue(zero2);
		LevelBaseMenu.vecDirectLightPosition.SetValue(LevelOutside.SunPosition);
		LevelBaseMenu.vecDirectLightColor.SetValue(LevelOutside.SunColor);
		LevelBaseMenu.vecAmbientColor.SetValue(LevelOutside.SunShadowColor);
		LevelBaseMenu.matViewProj.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view * LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection);
		LevelBaseMenu.matInvViewProj.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].invViewProj);
		LevelBaseMenu.matInvView.SetValue(Matrix.Invert(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view));
		LevelBaseMenu.matInvProj.SetValue(Matrix.Invert(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection));
		LevelBaseMenu.NormalTexture.SetValue(LevelBaseMenu.NormalRenderTarget);
		LevelBaseMenu.DiffuseTexture.SetValue(LevelBaseMenu.DiffuseRenderTarget);
		LevelBaseMenu.DepthTexture.SetValue(LevelBaseMenu.DepthRenderTarget);
		LevelBaseMenu.MaterialsTexture.SetValue(LevelBaseMenu.MaterialRenderTarget);
		LevelBaseMenu.EnvMap0.SetValue(LevelBaseMenu.EnvMap);
		LevelBaseMenu.vecFrustumCorners.SetValue(frustumCorners);
		LevelBaseMenu.EffectDirectionalLight.CurrentTechnique = LevelBaseMenu.T_DirectLight;
		LevelBaseMenu.EffectDirectionalLight.CurrentTechnique.Passes[0].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		LevelBaseMenu.PostEffectsClass.Bloom(qIndex);
		LevelBaseMenu.PostEffectsClass.Particles(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
	}

	public override void DrawDepthMap(int qIndex)
	{
		EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(LevelBaseMenu.DepthRenderTarget);
		EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, depthClearColor, 1f, 0);
		int playerIndex = 0;
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialParams.matWorld.SetValue(Matrix.Identity);
		materialParams.matView.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view);
		materialParams.matProj.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection);
		materialParams.matViewProj.SetValue(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view * LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection);
		Vector3 value = new Vector3(LevelBaseMenu.mDataQueue[qIndex].lightPos.X, LevelBaseMenu.mDataQueue[qIndex].lightPos.Y, LevelBaseMenu.mDataQueue[qIndex].lightPos.Z);
		Vector4 value2 = new Vector4(1f, 1f, 1f, 1f);
		Vector4 value3 = new Vector4(0.35f, 0.35f, 0.4f, 1f);
		materialParams.vecLightPosition.SetValue(value);
		materialParams.vecLightColor.SetValue(value2);
		materialParams.vecAmbientLightColor.SetValue(value3);
		LevelOutside.Draw(RenderPass.Depth, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], playerIndex, qIndex);
		if (LevelBaseMenu.gameMode != GameMode.CoOpPlayer)
		{
			LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].fpsWeapon.DrawDepth(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
			return;
		}
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].fpsWeapon.DrawDepth(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
		LevelBaseMenu.Players[1].fpsWeapon.DrawDepth(qIndex, LevelBaseMenu.Players[1]);
	}

	public override void DrawShadowMap(int qIndex)
	{
		for (int i = 0; i < 2; i++)
		{
			PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(LevelBaseMenu.shadowRenderTarget2[i]);
			EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			lightViewProjection = playerBase.mDataQueue[qIndex].lightView2[i] * playerBase.mDataQueue[qIndex].lightProj2[i];
			lightPosition = Vector3.Transform(-playerBase.mDataQueue[qIndex].lightView2[i].Translation, Matrix.Transpose(playerBase.mDataQueue[qIndex].lightView2[i]));
			LevelBaseMenu.tmpTerrainVegitation.DrawShadowMap(ref lightViewProjection, ref playerBase.mDataQueue[qIndex].lightView2[i], ref lightPosition, qIndex);
			LevelBaseMenu.AvRai.DrawShadowMap(playerBase, ref lightViewProjection, ref lightPosition, qIndex);
			if (playerBase.ThirdPersonCamera)
			{
				if (FPSGameMenu.isVisable || Guide.IsVisible)
				{
					playerBase.DrawPlayerShadow(playerBase, ref lightViewProjection, ref lightPosition, 0);
				}
				else
				{
					playerBase.DrawPlayerShadow(playerBase, ref lightViewProjection, ref lightPosition, qIndex);
				}
			}
			PlayerBase playerBase2 = null;
			int index = 0;
			while ((playerBase2 = EGENetWorkNext.NextNetPlayerReference(ref index)) != null)
			{
				playerBase2.DrawPlayerShadow(playerBase, ref lightViewProjection, ref lightPosition, qIndex);
				index++;
			}
		}
	}

	public void DrawPlayer(PlayerBase player, int qIndex, PlayerBase other, int playerIndex)
	{
		player.SetViewPortForPass(PlayerBase.RenderPass.GBufferPass, qIndex);
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialParams.matWorld.SetValue(Matrix.Identity);
		materialParams.matView.SetValue(player.mDataQueue[qIndex].view);
		materialParams.matProj.SetValue(player.mDataQueue[qIndex].projection);
		materialParams.matViewProj.SetValue(player.mDataQueue[qIndex].view * player.mDataQueue[qIndex].projection);
		LevelOutside.Draw(RenderPass.Normal, player, playerIndex, qIndex);
		other?.DrawPlayer(qIndex, player);
		if (player.mRagdoll.IsValid)
		{
			player.DrawRagdoll(qIndex, player);
		}
		if (player.ThirdPersonCamera)
		{
			if (FPSGameMenu.isVisable || Guide.IsVisible)
			{
				player.DrawPlayer(0, player);
			}
			else
			{
				player.DrawPlayer(qIndex, player);
			}
		}
		EGENetWorkNext.Draw(qIndex, player);
		DrawGameLogic(player, qIndex);
		LevelBaseMenu.tmpTerrain.Draw(player, playerIndex, qIndex);
		LevelBaseMenu.tmpTerrainVegitation.Draw(player, playerIndex, qIndex);
		LevelBaseMenu.tmpTerrainRoad.Draw(player, qIndex);
		LevelOutside.Draw(RenderPass.SkyDome, player, playerIndex, qIndex);
	}

	public void DrawPlayerForwardRender(PlayerBase player, int qIndex, PlayerBase other, int playerIndex)
	{
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		materialParams.matWorld.SetValue(Matrix.Identity);
		materialParams.matView.SetValue(player.mDataQueue[qIndex].view);
		materialParams.matProj.SetValue(player.mDataQueue[qIndex].projection);
		materialParams.matViewProj.SetValue(player.mDataQueue[qIndex].view * player.mDataQueue[qIndex].projection);
		materialParams.uvDisplacement.SetValue(player.UVDisplacement);
		LevelOutside.Draw(RenderPass.AlphaBlend, player, playerIndex, qIndex);
		LevelBaseMenu.Stickers.Draw(player, qIndex);
		materialParams.uvDisplacement.SetValue(player.UVDisplacement);
		LevelBaseMenu.Particles.Draw(player, 1, qIndex);
	}

	public override void UpdateThreadPhysics()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		try
		{
			EndGameEngine.enablePhysicsUpdate = true;
			LevelBaseMenu.PhysicsThreadTimer = (float)EndGameEngine.currentEleapsedTime.TotalGameTime.TotalMilliseconds;
			while (LevelBaseMenu.UpdateThreadRunning && EndGameEngine.ThreadExceptionArgument == null)
			{
				if (LevelBaseMenu.LoadState != LevelLoadState.Loaded || !Ragdoll.UpdateToggle)
				{
					continue;
				}
				Ragdoll.UpdateToggle = false;
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].UpdateRagdoll();
				for (int i = 0; i < 15; i++)
				{
					if (EGENetWorkNext.NetPlayers[i].NetGamerRef != null)
					{
						EGENetWorkNext.NetPlayers[i].UpdateRagdoll();
					}
				}
			}
			EndGameEngine.enablePhysicsUpdate = false;
		}
		catch (Exception threadExceptionArgument)
		{
			EndGameEngine.ThreadExceptionArgument = threadExceptionArgument;
		}
	}

	public override void UpdateThreadRun()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		EndGameEngine.enableInputUpdate = false;
		while (LevelBaseMenu.ThreadRunning)
		{
			LevelBaseMenu.debugIdleUpdateCounter++;
			UpdateThread();
		}
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Reset();
		LevelBaseMenu.Players[1].Reset();
		EndGameEngine.enableInputUpdate = true;
		EndGameEngine.enableClearTarget = true;
		if (LevelBaseMenu.returnQuickMatch)
		{
			EndGameEngine.menuMgr.MakeActive(GameMenus.MatchTypeMenu);
		}
		else
		{
			EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
		}
		LevelBaseMenu.ExitLevel = true;
		EndGameEngine.StartInLevelMenuUpdate = true;
	}

	public override void UpdateGameLevel(int qIndex)
	{
		float currentTimeStep = EndGameEngine.currentTimeStep;
		Ragdoll.UpdateToggle = true;
		LevelBaseMenu.debugUpdateCounter++;
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		float num = HeightMapPhysics.GetHeight(ref playerBase.vecPosition) + 62f;
		if (num > playerBase.vecPosition.Y)
		{
			num -= 2f;
			playerBase.GravityAccel = 0f;
			playerBase.vecPosition.Y = num;
		}
		else
		{
			playerBase.GravityAccel += 8f * EndGameEngine.fFIXED_TIME_STEP;
			playerBase.vecPosition.Y -= playerBase.GravityAccel;
		}
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Update(EndGameEngine.currentEleapsedTime, qIndex);
		_ = EndGameEngine.currentEleapsedTime.TotalGameTime.TotalSeconds;
		LevelBaseMenu.mDataQueue[qIndex].world = Matrix.Identity;
		for (int i = 0; i < WorldObject.worldObjectList.Count; i++)
		{
			WorldObject.worldObjectList[i].Update(EndGameEngine.currentEleapsedTime);
		}
		LevelOutside.Update(qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], (int)EndGameEngine.controllingPlayer.Value);
		Vector3 cameraPos = Vector3.Transform(-LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view.Translation, Matrix.Transpose(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view));
		LevelBaseMenu.tmpTerrain.Update(currentTimeStep, ref cameraPos, qIndex);
		LevelBaseMenu.tmpTerrainVegitation.Update(currentTimeStep, ref cameraPos, qIndex, (int)EndGameEngine.controllingPlayer.Value);
		LevelBaseMenu.tmpTerrainRoad.Update(currentTimeStep, qIndex, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
		UpdateGameLogic(currentTimeStep, qIndex);
		SpawnPoints.Update(EndGameEngine.currentEleapsedTime);
		TriggerPoints.Update(EndGameEngine.currentEleapsedTime);
		LevelBaseMenu.Emitters.Update(currentTimeStep);
		MediaEmitterClass.Update(currentTimeStep, qIndex);
		LevelBaseMenu.PointLights.Update(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
		particles.ViewSpaceDependantOffset.X = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].X;
		particles.ViewSpaceDependantOffset.Y = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].Z;
		LevelBaseMenu.Stickers.Update(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].cameraPos, currentTimeStep, qIndex);
		LevelBaseMenu.Particles.Update(currentTimeStep, qIndex);
		LevelBaseMenu.Particles.UpdatePlayer(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
	}

	public override void UpdateGameLogic(float eTime, int qIndex)
	{
		if (LevelBaseMenu.gameMode == GameMode.XboxLive)
		{
			UpdateXboxLive(eTime, qIndex);
		}
		else if (LevelBaseMenu.gameMode != GameMode.CoOpPlayer)
		{
			if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
			{
				UpdateCombatTraining(eTime);
			}
			else if (LevelBaseMenu.gameMode == GameMode.SurvivorLocal)
			{
				UpdateSurvivorLocal(eTime, qIndex);
			}
		}
	}

	private void UpdateXboxLive(float eTime, int qIndex)
	{
		if (!LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].RespawnTimeActive())
		{
			LevelBaseMenu.AvRai.StartWave(eTime);
			LevelBaseMenu.AvRai.Update(eTime, qIndex);
			LevelBaseMenu.AvRai.EndWave(eTime);
		}
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].MatchCoolDownTimer > 0f)
		{
			_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].MatchCoolDownTimer;
			_ = 0.5f;
		}
	}

	private void UpdateCombatTraining(float eTime)
	{
	}

	private void UpdateSurvivorLocal(float eTime, int qIndex)
	{
		if (!LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].RespawnTimeActive())
		{
			LevelBaseMenu.AvRai.Update(eTime, qIndex);
		}
	}

	public override void DrawGameLogic(PlayerBase playerRef, int qIndex)
	{
		if (LevelBaseMenu.gameMode == GameMode.XboxLive)
		{
			DrawXboxLive(playerRef, qIndex);
		}
		else if (LevelBaseMenu.gameMode != GameMode.CoOpPlayer)
		{
			if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
			{
				DrawCombatTraining(qIndex);
			}
			else if (LevelBaseMenu.gameMode == GameMode.SurvivorLocal)
			{
				DrawSurvivorLocal(playerRef, qIndex);
			}
		}
	}

	private void DrawXboxLive(PlayerBase playerRef, int qIndex)
	{
		LevelBaseMenu.AvRai.Draw(playerRef, qIndex);
	}

	private void DrawCombatTraining(int qIndex)
	{
	}

	private void DrawSurvivorLocal(PlayerBase playerRef, int qIndex)
	{
		LevelBaseMenu.AvRai.Draw(playerRef, qIndex);
	}
}
