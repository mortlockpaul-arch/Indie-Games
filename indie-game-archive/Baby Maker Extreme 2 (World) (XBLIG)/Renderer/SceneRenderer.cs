using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public static class SceneRenderer
{
	private const int TEXT_RENDER_POOL_COUNT = 100;

	private static Effect m_effect;

	private static Effect m_StoredEffect1;

	private static Effect m_StoredEffect2;

	private static List<DrawableComponent> m_drawSprites;

	private static GraphicsDevice m_gd;

	private static int MAX_PER_DRAW = 1000;

	private static VertexPositionColoredNBTTextured[] m_cornerPoints;

	private static int num_draw;

	private static int[] m_triIndexes;

	private static Matrix worldMatrix;

	private static Matrix viewMatrix;

	private static Matrix projectionMatrix;

	private static EffectParameter viewParameter;

	private static EffectParameter projectionParameter;

	private static EffectParameter cameraPositionParameter;

	private static SpriteBatch m_spriteBatch;

	private static Random sm_rand;

	private static CameraController sm_cameraManager;

	private static RenderTarget2D m_RenderTarget;

	private static List<RenderLight> m_lights;

	private static ContentManager m_content;

	private static Dictionary<int, SpriteFont> fontMap;

	private static BlendState m_blendState;

	private static DepthStencilState m_depthState;

	private static TextureContainer.DrawComponentComparer m_drawComparer;

	private static List<RenderText> m_renderTextPool;

	private static int m_iPoolIndex;

	private static AvatarHandler m_avatar;

	private static List<RenderText> m_text = new List<RenderText>();

	public static Matrix World
	{
		get
		{
			return worldMatrix;
		}
		set
		{
			worldMatrix = value;
		}
	}

	public static AvatarHandler Avatar
	{
		get
		{
			return m_avatar;
		}
		set
		{
			m_avatar = value;
		}
	}

	public static void Initialize(GraphicsDevice gd, ContentManager Content)
	{
		m_content = Content;
		m_StoredEffect1 = Content.Load<Effect>("Effects\\MaterialShader30");
		m_StoredEffect2 = Content.Load<Effect>("Effects\\VirtualMaterialShader30");
		m_effect = m_StoredEffect1;
		fontMap = new Dictionary<int, SpriteFont>();
		SpriteFont value = Content.Load<SpriteFont>("BasicFont");
		fontMap.Add(0, value);
		value = Content.Load<SpriteFont>("ButtonFont");
		fontMap.Add(1, value);
		value = Content.Load<SpriteFont>("ItemCountFont");
		fontMap.Add(2, value);
		value = Content.Load<SpriteFont>("CashCountFont");
		fontMap.Add(3, value);
		value = Content.Load<SpriteFont>("Grunge1");
		fontMap.Add(4, value);
		m_spriteBatch = new SpriteBatch(gd);
		viewMatrix = Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up);
		projectionMatrix = Matrix.CreateOrthographic(gd.Viewport.Width, gd.Viewport.Height, -1000f, 1000f);
		worldMatrix = Matrix.Identity;
		for (int i = 0; i < 2; i++)
		{
			if (i == 0)
			{
				m_effect = m_StoredEffect2;
			}
			else
			{
				m_effect = m_StoredEffect1;
			}
			m_effect.Parameters["materialColor"].SetValue(Color.White.ToVector4());
			m_effect.Parameters["specularPower"].SetValue(32);
			m_effect.Parameters["specularIntensity"].SetValue(1f);
			viewParameter = m_effect.Parameters["view"];
			projectionParameter = m_effect.Parameters["projection"];
			cameraPositionParameter = m_effect.Parameters["cameraPosition"];
			m_effect.Parameters["ambientLightColor"].SetValue(new Vector4(0.3f, 0.3f, 0.6f, 1f));
			m_effect.Parameters["numLightsPerPass"].SetValue(1);
			projectionParameter.SetValue(projectionMatrix);
		}
		m_drawSprites = new List<DrawableComponent>(2000);
		m_gd = gd;
		m_cornerPoints = new VertexPositionColoredNBTTextured[MAX_PER_DRAW * 4];
		num_draw = 0;
		m_triIndexes = new int[MAX_PER_DRAW * 2 * 3];
		for (int j = 0; j < MAX_PER_DRAW; j++)
		{
			m_triIndexes[j * 2 * 3] = j * 4;
			m_triIndexes[j * 2 * 3 + 1] = j * 4 + 1;
			m_triIndexes[j * 2 * 3 + 2] = j * 4 + 2;
			m_triIndexes[j * 2 * 3 + 3] = j * 4 + 3;
			m_triIndexes[j * 2 * 3 + 4] = j * 4 + 2;
			m_triIndexes[j * 2 * 3 + 5] = j * 4 + 1;
		}
		sm_rand = new Random();
		sm_cameraManager = new CameraController(m_effect);
		m_RenderTarget = new RenderTarget2D(m_gd, m_gd.Viewport.Width, m_gd.Viewport.Height, mipMap: true, SurfaceFormat.Color, DepthFormat.Depth24);
		m_lights = new List<RenderLight>();
		m_blendState = new BlendState();
		m_blendState.ColorDestinationBlend = Blend.One;
		m_blendState.ColorSourceBlend = Blend.One;
		m_blendState.ColorBlendFunction = BlendFunction.Add;
		m_blendState.AlphaSourceBlend = Blend.SourceAlpha;
		m_blendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		m_blendState.AlphaBlendFunction = BlendFunction.Add;
		m_depthState = new DepthStencilState();
		m_depthState.DepthBufferEnable = true;
		m_depthState.DepthBufferFunction = CompareFunction.LessEqual;
		m_depthState.DepthBufferWriteEnable = true;
		m_drawComparer = new TextureContainer.DrawComponentComparer();
		m_renderTextPool = new List<RenderText>();
		for (int k = 0; k < 100; k++)
		{
			m_renderTextPool.Add(new RenderText());
		}
		m_iPoolIndex = 0;
		m_avatar = null;
	}

	public static Vector2 GetScreenDim()
	{
		return new Vector2(m_gd.Viewport.Width, m_gd.Viewport.Height);
	}

	public static void AddSpriteToDraw(SpriteInstance spr)
	{
		m_drawSprites.Add(spr);
	}

	public static void AddLightToDraw(RenderLight light)
	{
		m_lights.Add(light);
	}

	public static void SetRendering()
	{
		SetupState();
		m_gd.Clear(Color.Gray);
		m_gd.SetRenderTarget(m_RenderTarget);
		m_gd.Clear(ClearOptions.Target, Color.Gray, 1f, 0);
	}

	public static void EndRendering()
	{
		m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		for (int i = 0; i < m_text.Count; i++)
		{
			DrawFinalString(m_text[i], sm_cameraManager.Position);
		}
		m_spriteBatch.End();
		m_gd.SetRenderTarget(null);
		m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
		m_gd.Clear(Color.Red);
		m_spriteBatch.Draw(m_RenderTarget, default(Vector2), Color.White);
		m_spriteBatch.End();
	}

	public static void RenderScene()
	{
		if (m_drawSprites.Count <= 0)
		{
			return;
		}
		m_drawSprites.Sort(m_drawComparer);
		m_effect.CurrentTechnique.Passes["PointLight"].Apply();
		SpritePage spritePage = null;
		m_text.Clear();
		bool flag = false;
		bool flag2 = true;
		bool flag3 = false;
		Vector2 screenDim = GetScreenDim();
		Rectangle r = new Rectangle((int)(sm_cameraManager.Position.X - screenDim.X / 2f), (int)(0f - sm_cameraManager.Position.Y - screenDim.Y / 2f), (int)screenDim.X, (int)screenDim.Y);
		float zoom = sm_cameraManager.GetZoom();
		r.X = (int)((float)r.Center.X - (float)r.Width / 2f / zoom);
		r.Width = (int)((float)r.Width / zoom);
		r.Y = (int)((float)r.Center.Y - (float)r.Height / 2f / zoom);
		r.Height = (int)((float)r.Height / zoom);
		bool flag4 = m_avatar == null || !m_avatar.ShouldDraw;
		for (int i = 0; i < m_drawSprites.Count; i++)
		{
			if (m_drawSprites[i] is SpriteInstance)
			{
				SpriteInstance spriteInstance = (SpriteInstance)m_drawSprites[i];
				if (!spriteInstance.OnScreen(ref r))
				{
					continue;
				}
				if (spriteInstance.GetSpriteImage().GetSpritePage() != spritePage || spriteInstance.FlatColor != flag || spriteInstance.Shadowed != flag2 || spriteInstance.Additive != flag3 || num_draw >= MAX_PER_DRAW)
				{
					if (spritePage != null)
					{
						m_gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, m_cornerPoints, 0, num_draw * 6, m_triIndexes, 0, num_draw * 2);
					}
					flag = spriteInstance.FlatColor;
					flag2 = spriteInstance.Shadowed;
					spritePage = spriteInstance.GetSpriteImage().GetSpritePage();
					SetupEffect(spriteInstance, m_effect);
					if (spriteInstance.Additive)
					{
						m_effect.CurrentTechnique.Passes["AddPointLight"].Apply();
					}
					else
					{
						m_effect.CurrentTechnique.Passes["PointLight"].Apply();
					}
					ref VertexPositionColoredNBTTextured reference = ref m_cornerPoints[0];
					reference = spriteInstance.GetCornerPoints()[0];
					ref VertexPositionColoredNBTTextured reference2 = ref m_cornerPoints[1];
					reference2 = spriteInstance.GetCornerPoints()[1];
					ref VertexPositionColoredNBTTextured reference3 = ref m_cornerPoints[2];
					reference3 = spriteInstance.GetCornerPoints()[2];
					ref VertexPositionColoredNBTTextured reference4 = ref m_cornerPoints[3];
					reference4 = spriteInstance.GetCornerPoints()[3];
					num_draw = 1;
				}
				else
				{
					ref VertexPositionColoredNBTTextured reference5 = ref m_cornerPoints[num_draw * 4];
					reference5 = spriteInstance.GetCornerPoints()[0];
					ref VertexPositionColoredNBTTextured reference6 = ref m_cornerPoints[num_draw * 4 + 1];
					reference6 = spriteInstance.GetCornerPoints()[1];
					ref VertexPositionColoredNBTTextured reference7 = ref m_cornerPoints[num_draw * 4 + 2];
					reference7 = spriteInstance.GetCornerPoints()[2];
					ref VertexPositionColoredNBTTextured reference8 = ref m_cornerPoints[num_draw * 4 + 3];
					reference8 = spriteInstance.GetCornerPoints()[3];
					num_draw++;
				}
			}
			else if (m_drawSprites[i] is RenderText)
			{
				m_text.Add((RenderText)m_drawSprites[i]);
			}
		}
		if (spritePage != null)
		{
			m_gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, m_cornerPoints, 0, num_draw * 6, m_triIndexes, 0, num_draw * 2);
		}
		m_drawSprites.Clear();
		num_draw = 0;
		if (!flag4)
		{
			m_avatar.Draw();
		}
	}

	private static void SetupEffect(SpriteInstance spr, Effect e)
	{
		e.Parameters["diffuseTexture"].SetValue(spr.GetSpriteImage().GetSpritePage().DiffuseTex);
		e.Parameters["diffuseTexEnabled"].SetValue(spr.GetSpriteImage().GetSpritePage().DiffuseTex != null);
		e.Parameters["normTexture"].SetValue(spr.GetSpriteImage().GetSpritePage().NormTex);
		e.Parameters["normTexEnabled"].SetValue(spr.GetSpriteImage().GetSpritePage().NormTex != null);
		e.Parameters["specularTexture"].SetValue(spr.GetSpriteImage().GetSpritePage().SpecTex);
		e.Parameters["specularTexEnabled"].SetValue(spr.GetSpriteImage().GetSpritePage().SpecTex != null);
		e.Parameters["flatColor"].SetValue(spr.FlatColor);
		e.Parameters["shadowed"].SetValue(spr.Shadowed);
	}

	public static void ResetWorldParam()
	{
		m_effect.Parameters["world"].SetValue(worldMatrix);
	}

	private static void SetupState()
	{
		projectionParameter.SetValue(projectionMatrix);
		viewParameter.SetValue(viewMatrix);
		cameraPositionParameter.SetValue(new Vector3(sm_cameraManager.Position.X, sm_cameraManager.Position.Y, -1f));
		m_effect.Parameters["world"].SetValue(worldMatrix);
		m_effect.Parameters["numLightsPerPass"].SetValue(m_lights.Count);
		for (int i = 0; i < m_lights.Count; i++)
		{
			EffectParameter effectParameter = m_effect.Parameters["lights"].Elements[i];
			Vector4 value = new Vector4(m_lights[i].pos - new Vector3(sm_cameraManager.Position.X, 0f - sm_cameraManager.Position.Y, 0f), 0f);
			effectParameter.StructureMembers["position"].SetValue(value);
			effectParameter.StructureMembers["falloff"].SetValue(m_lights[i].falloff);
			effectParameter.StructureMembers["range"].SetValue(m_lights[i].range);
			effectParameter.StructureMembers["color"].SetValue(m_lights[i].color.ToVector4());
		}
		m_lights.Clear();
		m_gd.BlendState = m_blendState;
		m_gd.DepthStencilState = m_depthState;
		m_gd.RasterizerState = RasterizerState.CullClockwise;
	}

	public static float GetRand(float startRange, float endRange)
	{
		float num = (float)sm_rand.NextDouble();
		return num * (endRange - startRange) + startRange;
	}

	public static float GetRandSqr(float startRange, float endRange)
	{
		float num = (float)sm_rand.NextDouble();
		num *= num;
		return num * (endRange - startRange) + startRange;
	}

	public static void Update(TimeTracker gameTime)
	{
		sm_cameraManager.Update(gameTime);
		if (m_avatar != null)
		{
			m_avatar.Update();
		}
	}

	public static void MoveCamera(Vector2 pos, float rotation, float zoom)
	{
		sm_cameraManager.MoveCamera(pos, rotation, zoom);
	}

	public static void PushCamera(Vector2 pos)
	{
		sm_cameraManager.PushCamera(pos);
	}

	public static void ZoomCamera(float amount)
	{
		sm_cameraManager.ZoomCamera(amount);
	}

	public static void RotateCamera(float amount)
	{
		sm_cameraManager.RotateCamera(amount);
	}

	public static float GetCameraZoom()
	{
		return sm_cameraManager.GetZoom();
	}

	public static Vector2 GetCameraPosition()
	{
		return sm_cameraManager.Position;
	}

	public static void DrawString(fonts f, StringBuilder s, Vector2 pos, Color c, Vector2 size, float depth)
	{
		DrawString(f, s, pos, c, size, isScreenSpace: false, 0f, depth);
	}

	public static void DrawString(fonts f, StringBuilder s, Vector2 pos, Color c, Vector2 size, bool isScreenSpace, float rotation, float depth)
	{
		textData data = new textData
		{
			f = f,
			b = s,
			pos = pos,
			c = c,
			size = size,
			isScreenSpace = isScreenSpace,
			rot = rotation
		};
		RenderText renderText = GetRenderText();
		renderText.Initialize(data, depth);
		m_drawSprites.Add(renderText);
	}

	public static void DrawString(fonts f, string s, Vector2 pos, Color c, Vector2 size, float depth)
	{
		DrawString(f, s, pos, c, size, isScreenSpace: false, 0f, depth);
	}

	public static void DrawString(fonts f, string s, Vector2 pos, Color c, Vector2 size, bool isScreenSpace, float rotation, float depth)
	{
		textData data = new textData
		{
			f = f,
			s = s,
			pos = pos,
			c = c,
			size = size,
			isScreenSpace = isScreenSpace,
			rot = rotation
		};
		RenderText renderText = GetRenderText();
		renderText.Initialize(data, depth);
		m_drawSprites.Add(renderText);
	}

	public static RenderText GetRenderText()
	{
		m_iPoolIndex++;
		if (m_iPoolIndex >= m_renderTextPool.Count)
		{
			m_iPoolIndex = 0;
		}
		return m_renderTextPool[m_iPoolIndex];
	}

	public static void DrawString(fonts f, string s, Vector2 pos, Color c, float depth)
	{
		DrawString(f, s, pos, c, new Vector2(1f, 1f), depth);
	}

	public static void DrawStringCentered(fonts f, string s, Vector2 pos, Color c, Vector2 size, bool isScreenSpace, float depth)
	{
		DrawString(f, s, pos - size * (fontMap[(int)f].MeasureString(s) / 2f), c, size, isScreenSpace, 0f, depth);
	}

	public static void DrawStringCentered(fonts f, string s, Vector2 pos, Color c, float depth)
	{
		DrawString(f, s, pos - fontMap[(int)f].MeasureString(s) / 2f, c, depth);
	}

	public static void DrawStringCentered(fonts f, StringBuilder s, Vector2 pos, Color c, Vector2 size, float depth)
	{
		DrawString(f, s, pos - size * (fontMap[(int)f].MeasureString(s) / 2f), c, size, depth);
	}

	public static void DrawStringCentered(fonts f, StringBuilder s, Vector2 pos, Color c, Vector2 size, bool isScreenSpace, float depth)
	{
		DrawString(f, s, pos - size * (fontMap[(int)f].MeasureString(s) / 2f), c, size, isScreenSpace, 0f, depth);
	}

	public static void DrawStringCentered(fonts f, string s, Vector2 pos, Color c, Vector2 size, float depth)
	{
		DrawString(f, s, pos - size * (fontMap[(int)f].MeasureString(s) / 2f), c, size, depth);
	}

	public static void DrawFinalString(RenderText text, Vector2 cameraPosition)
	{
		textData textData2 = text.GetTextData();
		if (textData2.s != null)
		{
			m_spriteBatch.DrawString(fontMap[(int)textData2.f], textData2.s, GetScreenDim() / 2f + textData2.pos - cameraPosition, textData2.c, textData2.rot, default(Vector2), textData2.size, SpriteEffects.None, 0f);
		}
		else
		{
			m_spriteBatch.DrawString(fontMap[(int)textData2.f], textData2.b, GetScreenDim() / 2f + textData2.pos - cameraPosition, textData2.c, textData2.rot, default(Vector2), textData2.size, SpriteEffects.None, 0f);
		}
	}

	public static ContentManager GetContentManager()
	{
		return m_content;
	}

	public static GraphicsDevice GetGraphicsDevice()
	{
		return m_gd;
	}

	public static void SetEffect(int i)
	{
		Effect effect = m_effect;
		if (i == 0)
		{
			m_effect = m_StoredEffect1;
		}
		else
		{
			m_effect = m_StoredEffect2;
		}
		if (effect != m_effect)
		{
			Vector2 position = sm_cameraManager.Position;
			sm_cameraManager = new CameraController(m_effect);
			sm_cameraManager.MoveCamera(position, 0f, 1f);
			viewParameter = m_effect.Parameters["view"];
			projectionParameter = m_effect.Parameters["projection"];
			cameraPositionParameter = m_effect.Parameters["cameraPosition"];
			projectionParameter.SetValue(projectionMatrix);
			viewParameter.SetValue(viewMatrix);
			cameraPositionParameter.SetValue(new Vector3(sm_cameraManager.Position.X, sm_cameraManager.Position.Y, -1f));
			m_effect.Parameters["world"].SetValue(worldMatrix);
		}
	}

	public static int GetEffectMode()
	{
		if (m_effect == m_StoredEffect1)
		{
			return 0;
		}
		return 1;
	}
}
