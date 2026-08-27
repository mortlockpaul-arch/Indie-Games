using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class ePointLights
{
	private static float TIME_STEP = 1f / 60f;

	private static float LIGHT_SCALE = 0.8f;

	private static float LUMINANCE_SCALE = 0.3f;

	private static int MAX_LIGHTS_IN_GRID = 48;

	private static int MAX_DYNAMIC_LIGHTS = 64;

	public static eLevelLight[][] DynamicLights = new eLevelLight[2][];

	private ePointLightsList[][] CurrentLights = new ePointLightsList[2][];

	private static float TileCountX = 4f;

	private static float TileCountY = 4f;

	private static Vector2[] xyOffset;

	private static ePointLightGrid[][] lightGrid;

	public static VertexBuffer PointLightVertexBuffer;

	private Model SphereModel;

	private BoundingSphere bSphere = default(BoundingSphere);

	private Vector3 SpherePosition = Vector3.Zero;

	private Matrix SphereWorld = Matrix.Identity;

	private Vector4 vecLightPos = Vector4.Zero;

	private Vector4 vecLightCol = Vector4.Zero;

	private EffectParameter vecViewPortScale;

	private EffectParameter vecEyePosition;

	private EffectParameter matView;

	private EffectParameter matViewProj;

	private EffectParameter matInvView;

	private EffectParameter matInvProj;

	private EffectParameter matInvViewProj;

	private EffectParameter NormalTexture;

	private EffectParameter DiffuseTexture;

	private EffectParameter DepthTexture;

	private EffectParameter MaterialTexture;

	private EffectParameter SpecularPower;

	private EffectParameter SpecularIntensity;

	private EffectParameter SpotLightDirection;

	public Effect EffectPointLight;

	private int maxLightsInGrid;

	private Vector4[] vecLightPositions = new Vector4[MAX_LIGHTS_IN_GRID];

	private Vector4[] vecLightColors = new Vector4[MAX_LIGHTS_IN_GRID];

	private Matrix matWorld = Matrix.Identity;

	private void CalculateLuminance(eLevelLight lightRef, out float radius, out float luminance)
	{
		luminance = ((lightRef.Multiplyer > lightRef.DecayRadius) ? lightRef.DecayRadius : lightRef.Multiplyer);
		luminance = luminance / lightRef.DecayRadius + LUMINANCE_SCALE;
		lightRef.Luminance = ((luminance > LIGHT_SCALE) ? LIGHT_SCALE : luminance);
		radius = lightRef.Luminance * lightRef.DecayRadius * (LIGHT_SCALE + 0.02f);
	}

	public ePointLights()
	{
		for (int i = 0; i < 2; i++)
		{
			DynamicLights[i] = new eLevelLight[MAX_DYNAMIC_LIGHTS];
			for (int j = 0; j < MAX_DYNAMIC_LIGHTS; j++)
			{
				DynamicLights[i][j] = new eLevelLight();
				DynamicLights[i][j].Timer = 0f;
				DynamicLights[i][j].IsDynamic = true;
			}
		}
		float num = TileCountX * TileCountY;
		xyOffset = new Vector2[(int)num];
		lightGrid = new ePointLightGrid[2][];
		for (int k = 0; k < 2; k++)
		{
			lightGrid[k] = new ePointLightGrid[(int)num];
		}
		for (int l = 0; l < 2; l++)
		{
			CurrentLights[l] = new ePointLightsList[4];
			for (int m = 0; m < 4; m++)
			{
				CurrentLights[l][m] = new ePointLightsList();
				CurrentLights[l][m].lights = new List<eLevelLight>();
			}
		}
		VS_PostStruct[] array = new VS_PostStruct[(int)(4f * num)];
		PointLightVertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VS_PostStruct), (int)(4f * num), BufferUsage.None);
		int num2 = 0;
		for (float num3 = 0f; num3 < TileCountX; num3++)
		{
			for (float num4 = 0f; num4 < TileCountY; num4++)
			{
				float x = -1f + num3 * (2f / TileCountX);
				float x2 = -1f + (num3 * (2f / TileCountX) + 2f / TileCountX);
				float y = 1f - num4 * (2f / TileCountY);
				float y2 = 1f - (num4 * (2f / TileCountY) + 2f / TileCountY);
				float x3 = num3 * (1f / TileCountX);
				float x4 = num3 * (1f / TileCountX) + 1f / TileCountX;
				float y3 = num4 * (1f / TileCountY);
				float y4 = num4 * (1f / TileCountY) + 1f / TileCountY;
				Vector3 pos = new Vector3(x, y, 0f);
				Vector3 pos2 = new Vector3(x2, y, 0f);
				Vector3 pos3 = new Vector3(x, y2, 0f);
				Vector3 pos4 = new Vector3(x2, y2, 0f);
				ref VS_PostStruct reference = ref array[num2++];
				reference = new VS_PostStruct(pos, new Vector2(x3, y3), 0f);
				ref VS_PostStruct reference2 = ref array[num2++];
				reference2 = new VS_PostStruct(pos2, new Vector2(x4, y3), 1f);
				ref VS_PostStruct reference3 = ref array[num2++];
				reference3 = new VS_PostStruct(pos3, new Vector2(x3, y4), 3f);
				ref VS_PostStruct reference4 = ref array[num2++];
				reference4 = new VS_PostStruct(pos4, new Vector2(x4, y4), 2f);
			}
		}
		PointLightVertexBuffer.SetData(array);
		for (int n = 0; n < 2; n++)
		{
			int num5 = 0;
			int num6 = 0;
			for (float num7 = 0f; num7 < TileCountX; num7++)
			{
				for (float num8 = 0f; num8 < TileCountY; num8++)
				{
					lightGrid[n][num5] = new ePointLightGrid();
					lightGrid[n][num5].Min = new Vector2(array[num6 + 2].position.X, array[num6 + 2].position.Y);
					lightGrid[n][num5].Max = new Vector2(array[num6 + 1].position.X, array[num6 + 1].position.Y);
					ref Vector2 reference5 = ref xyOffset[num5];
					reference5 = new Vector2(num7, num8);
					num5++;
					num6 += 4;
				}
			}
		}
	}

	public void AddDynamicPointLight(ref Vector3 position, ref Color color, float radius, float timer, int qIndex)
	{
		for (int i = 0; i < MAX_DYNAMIC_LIGHTS; i++)
		{
			if (DynamicLights[qIndex][i].Timer <= 0f)
			{
				DynamicLights[qIndex][i].eType = LightTypes.Omnilight;
				DynamicLights[qIndex][i].Position = position;
				DynamicLights[qIndex][i].LightColor = color;
				DynamicLights[qIndex][i].DecayRadius = radius;
				DynamicLights[qIndex][i].Multiplyer = radius;
				DynamicLights[qIndex][i].Intensity = 1f;
				DynamicLights[qIndex][i].Timer = timer;
				float num = timer / TIME_STEP;
				DynamicLights[qIndex][i].DecayRate = 255f / num;
				break;
			}
		}
	}

	public void AddDynamicSpotLight(ref Vector3 position, ref Vector3 spotDirection, ref Vector3 spotParams, ref Color color, float radius, float timer, int qIndex)
	{
		for (int i = 0; i < MAX_DYNAMIC_LIGHTS; i++)
		{
			if (DynamicLights[qIndex][i].Timer <= 0f)
			{
				DynamicLights[qIndex][i].eType = LightTypes.SpotLight;
				DynamicLights[qIndex][i].Position = position;
				DynamicLights[qIndex][i].SpotDirection = spotDirection;
				DynamicLights[qIndex][i].SpotParameters = spotParams;
				DynamicLights[qIndex][i].LightColor = color;
				DynamicLights[qIndex][i].DecayRadius = radius;
				DynamicLights[qIndex][i].Multiplyer = radius;
				DynamicLights[qIndex][i].Intensity = 1f;
				DynamicLights[qIndex][i].Timer = timer;
				float num = timer / TIME_STEP;
				DynamicLights[qIndex][i].DecayRate = 255f / num;
				break;
			}
		}
	}

	public void Update(PlayerBase playerRef, int qIndex)
	{
		for (int i = 0; i < MAX_DYNAMIC_LIGHTS; i++)
		{
			if (DynamicLights[qIndex][i].Timer > 0f)
			{
				DynamicLights[qIndex][i].Timer -= TIME_STEP;
				float num = (float)(int)DynamicLights[qIndex][i].LightColor.A - DynamicLights[qIndex][i].DecayRate;
				float num2 = (float)(int)DynamicLights[qIndex][i].LightColor.R - DynamicLights[qIndex][i].DecayRate;
				float num3 = (float)(int)DynamicLights[qIndex][i].LightColor.G - DynamicLights[qIndex][i].DecayRate;
				float num4 = (float)(int)DynamicLights[qIndex][i].LightColor.B - DynamicLights[qIndex][i].DecayRate;
				DynamicLights[qIndex][i].LightColor.A = (byte)((num > 0f) ? num : 0f);
				DynamicLights[qIndex][i].LightColor.R = (byte)((num2 > 0f) ? num2 : 0f);
				DynamicLights[qIndex][i].LightColor.G = (byte)((num3 > 0f) ? num3 : 0f);
				DynamicLights[qIndex][i].LightColor.B = (byte)((num4 > 0f) ? num4 : 0f);
			}
		}
		UpdateDefferedLights(playerRef, qIndex);
		for (int j = 0; j < MAX_DYNAMIC_LIGHTS; j++)
		{
			if (DynamicLights[qIndex][j].Timer > 1000f)
			{
				DynamicLights[qIndex][j].Timer = -1f;
			}
		}
	}

	public void UpdateDefferedLights(PlayerBase playerRef, int qIndex)
	{
		Vector3 zero = Vector3.Zero;
		zero = Vector3.Transform(-playerRef.mDataQueue[qIndex].view.Translation, Matrix.Transpose(playerRef.mDataQueue[qIndex].view));
		int playerIndex = (int)playerRef.playerIndex;
		CurrentLights[qIndex][playerIndex].lights.Clear();
		AddLightsToGrid(playerRef, DynamicLights[qIndex], MAX_DYNAMIC_LIGHTS, ref zero, CurrentLights[qIndex][playerIndex].lights, qIndex);
		AddLightsToGrid(playerRef, LevelOutside.Lights, LevelOutside.Lights.Length, ref zero, CurrentLights[qIndex][playerIndex].lights, qIndex);
	}

	public void AddLightsToGrid(PlayerBase playerRef, eLevelLight[] lights, int numberLights, ref Vector3 eyePos, List<eLevelLight> DstLights, int qIndex)
	{
		int num = 0;
		for (int i = 0; i < numberLights; i++)
		{
			if (DstLights.Count >= MAX_LIGHTS_IN_GRID)
			{
				break;
			}
			if (!(lights[i].Timer > 0f) || (lights[i].eType != LightTypes.Omnilight && lights[i].eType != LightTypes.SpotLight))
			{
				continue;
			}
			float radius = 0f;
			float luminance = 0f;
			CalculateLuminance(lights[i], out radius, out luminance);
			Matrix.CreateScale(radius, out SphereWorld);
			SpherePosition.X = lights[i].Position.X;
			SpherePosition.Y = lights[i].Position.Y;
			SpherePosition.Z = lights[i].Position.Z;
			SphereWorld.Translation = SpherePosition;
			bool result = false;
			bSphere.Center = SpherePosition;
			bSphere.Center.X -= playerRef.vecHeadPosition[qIndex].X;
			bSphere.Center.Z -= playerRef.vecHeadPosition[qIndex].Z;
			bSphere.Radius = radius;
			playerRef.bFrustum[qIndex].Intersects(ref bSphere, out result);
			if (result && num < MAX_LIGHTS_IN_GRID)
			{
				num++;
				DstLights.Add(lights[i]);
				if (lights[i].IsDynamic)
				{
					num++;
				}
			}
		}
	}

	public void SetLightBuffer(PlayerBase playerRef, int qIndex)
	{
	}

	public void ProcessDefferedLights(PlayerBase playerRef, int qIndex, float specPower, float specIntensity, float diffuseIntensity)
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		if (SphereModel == null)
		{
			SphereModel = EndGameEngine.GameAssetMgr.Load<Model>("models\\objects\\sphere");
		}
		if (EffectPointLight == null)
		{
			EffectPointLight = EndGameEngine.ContentMgr.Load<Effect>("shaders\\PointLight");
			vecViewPortScale = EffectPointLight.Parameters["vecViewPortScale"];
			vecEyePosition = EffectPointLight.Parameters["vecEyePosition"];
			matView = EffectPointLight.Parameters["matView"];
			matViewProj = EffectPointLight.Parameters["matViewProj"];
			matInvView = EffectPointLight.Parameters["matInvView"];
			matInvProj = EffectPointLight.Parameters["matInvProj"];
			matInvViewProj = EffectPointLight.Parameters["matInvViewProj"];
			NormalTexture = EffectPointLight.Parameters["NormalTexture"];
			DiffuseTexture = EffectPointLight.Parameters["DiffuseTexture"];
			DepthTexture = EffectPointLight.Parameters["DepthTexture"];
			MaterialTexture = EffectPointLight.Parameters["MaterialTexture"];
			SpecularPower = EffectPointLight.Parameters["fSpecularPower"];
			SpecularIntensity = EffectPointLight.Parameters["fSpecularIntensity"];
			SpotLightDirection = EffectPointLight.Parameters["SpotLightDirection"];
		}
		graphicsDevice.BlendState = BlendState.Additive;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(PointLightVertexBuffer);
		Vector3 zero = Vector3.Zero;
		zero = playerRef.vecHeadPosition[qIndex];
		vecViewPortScale.SetValue(new Vector2(1f / (float)graphicsDevice.Viewport.Width, 1f / (float)graphicsDevice.Viewport.Height));
		vecEyePosition.SetValue(zero);
		matView.SetValue(playerRef.mDataQueue[qIndex].view);
		matViewProj.SetValue(playerRef.mDataQueue[qIndex].view * playerRef.mDataQueue[qIndex].projection);
		matInvView.SetValue(Matrix.Invert(playerRef.mDataQueue[qIndex].view));
		matInvProj.SetValue(Matrix.Invert(playerRef.mDataQueue[qIndex].projection));
		matInvViewProj.SetValue(Matrix.Invert(playerRef.mDataQueue[qIndex].view * playerRef.mDataQueue[qIndex].projection));
		SpecularPower.SetValue(specPower);
		SpecularIntensity.SetValue(specIntensity);
		NormalTexture.SetValue(LevelBaseMenu.NormalRenderTarget);
		DiffuseTexture.SetValue(LevelBaseMenu.DiffuseRenderTarget);
		DepthTexture.SetValue(LevelBaseMenu.DepthRenderTarget);
		MaterialTexture.SetValue(LevelBaseMenu.MaterialRenderTarget);
		graphicsDevice.BlendState = BlendState.Additive;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		EffectPointLight.CurrentTechnique = EffectPointLight.Techniques["T_PointLight"];
		int num = 0;
		int num2 = 1;
		int playerIndex = (int)playerRef.playerIndex;
		for (int i = 0; i < CurrentLights[qIndex][playerIndex].lights.Count; i++)
		{
			eLevelLight eLevelLight2 = CurrentLights[qIndex][playerIndex].lights[i];
			if (eLevelLight2.eType != LightTypes.Omnilight && eLevelLight2.eType != LightTypes.SpotLight)
			{
				continue;
			}
			float radius = 0f;
			float luminance = 0f;
			CalculateLuminance(eLevelLight2, out radius, out luminance);
			float num3 = (zero - eLevelLight2.Position).LengthSquared();
			if (num3 <= radius * radius)
			{
				graphicsDevice.RasterizerState = EndGameEngine.RasterCullCW;
				graphicsDevice.DepthStencilState = EndGameEngine.DepthInSidePointLight;
			}
			else
			{
				graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
				graphicsDevice.DepthStencilState = EndGameEngine.DepthOutSidePointLight;
			}
			num++;
			num2++;
			foreach (ModelMesh mesh in SphereModel.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
					graphicsDevice.Indices = meshPart.IndexBuffer;
					Matrix.CreateScale(radius, out matWorld);
					float x = eLevelLight2.Position.X - playerRef.vecHeadPosition[qIndex].X;
					float z = eLevelLight2.Position.Z - playerRef.vecHeadPosition[qIndex].Z;
					matWorld.Translation = new Vector3(x, eLevelLight2.Position.Y, z);
					EffectPointLight.Parameters["matWorld"].SetValue(matWorld);
					vecLightPos.X = eLevelLight2.Position.X;
					vecLightPos.Y = eLevelLight2.Position.Y;
					vecLightPos.Z = eLevelLight2.Position.Z;
					vecLightPos.W = radius * 0.9f;
					float num4 = eLevelLight2.Intensity * diffuseIntensity;
					vecLightCol.X = (float)(int)eLevelLight2.LightColor.R * 0.003922f * num4;
					vecLightCol.Y = (float)(int)eLevelLight2.LightColor.G * 0.003922f * num4;
					vecLightCol.Z = (float)(int)eLevelLight2.LightColor.B * 0.003922f * num4;
					vecLightCol.W = 1f;
					vecLightCol.X *= LevelOutside.VideoBrightness;
					vecLightCol.Y *= LevelOutside.VideoBrightness;
					vecLightCol.Z *= LevelOutside.VideoBrightness;
					if (eLevelLight2.eType == LightTypes.SpotLight)
					{
						EffectPointLight.Parameters["SpotLightDirection"].SetValue(eLevelLight2.SpotDirection);
						vecLightCol.W = 1f;
						EffectPointLight.Parameters["vecLightPosition"].SetValue(vecLightPos);
						EffectPointLight.Parameters["vecLightColor"].SetValue(vecLightCol);
						EffectPointLight.Parameters["SpotLightParameters"].SetValue(eLevelLight2.SpotParameters);
						EffectPointLight.CurrentTechnique.Passes[3].Apply();
						graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, 0, meshPart.PrimitiveCount);
					}
					else if (num3 > 3240000f)
					{
						EffectPointLight.Parameters["vecLightPosition"].SetValue(vecLightPos);
						EffectPointLight.Parameters["vecLightColor"].SetValue(vecLightCol);
						EffectPointLight.CurrentTechnique.Passes[1].Apply();
						graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, 0, meshPart.PrimitiveCount);
					}
					else
					{
						vecLightCol.W = (1f - num3 / 3240000f) * 8f;
						vecLightCol.W = ((vecLightCol.W < 1f) ? vecLightCol.W : 1f);
						EffectPointLight.Parameters["vecLightPosition"].SetValue(vecLightPos);
						EffectPointLight.Parameters["vecLightColor"].SetValue(vecLightCol);
						EffectPointLight.CurrentTechnique.Passes[0].Apply();
						graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, 0, meshPart.PrimitiveCount);
					}
				}
			}
		}
	}
}
