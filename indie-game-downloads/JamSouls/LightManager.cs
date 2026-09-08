using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class LightManager
{
	private bool m_bLightEnabled;

	public List<LightSource> m_lights;

	private RenderTarget2D m_lightMap;

	private Texture2D m_lightTexture;

	private Texture2D m_alphaClearTexture;

	private Color m_AmbientColor;

	private Rectangle m_ScreenSource;

	private ScreenManager m_ScreenManager;

	private ContentManager m_content;

	private BlendState m_LightmapBlend;

	private BlendState m_LightMixBlend;

	public LightManager(ScreenManager ScreenMgr, ContentManager ContenMgr)
	{
		m_ScreenManager = ScreenMgr;
		m_content = ContenMgr;
		m_bLightEnabled = false;
		m_AmbientColor = new Color(35, 35, 35);
		m_lights = new List<LightSource>();
		LoadTexture();
		m_LightmapBlend = new BlendState();
		m_LightmapBlend.AlphaDestinationBlend = Blend.One;
		m_LightmapBlend.AlphaSourceBlend = Blend.DestinationAlpha;
		m_LightmapBlend.AlphaBlendFunction = BlendFunction.Add;
		m_LightmapBlend.ColorBlendFunction = BlendFunction.Add;
		m_LightmapBlend.ColorSourceBlend = Blend.DestinationAlpha;
		m_LightmapBlend.ColorDestinationBlend = Blend.One;
		m_LightMixBlend = new BlendState();
		m_LightMixBlend.AlphaSourceBlend = Blend.Zero;
		m_LightMixBlend.AlphaDestinationBlend = Blend.SourceAlpha;
		m_LightMixBlend.ColorSourceBlend = Blend.Zero;
		m_LightMixBlend.ColorDestinationBlend = Blend.SourceColor;
		m_LightMixBlend.AlphaBlendFunction = BlendFunction.Add;
		m_ScreenSource = new Rectangle(0, 0, 1280, 720);
	}

	public void LoadTexture()
	{
		_ = m_ScreenManager.GraphicsDevice.PresentationParameters;
		m_lightMap = new RenderTarget2D(m_ScreenManager.GraphicsDevice, 1280, 720);
		m_lightTexture = m_content.Load<Texture2D>("Fx/light/light");
		m_alphaClearTexture = m_content.Load<Texture2D>("Fx/light/AlphaOne");
	}

	public void SetAmbientLight(Color col)
	{
		m_AmbientColor = col;
	}

	public LightSource AddLight(Color color, int range, Vector2 Position)
	{
		m_lights.Add(new LightSource(m_ScreenManager.SpriteBatch, m_lightTexture, color, range, Position));
		return m_lights[m_lights.Count - 1];
	}

	public int GetLightCount()
	{
		return m_lights.Count;
	}

	public void SetLightEnabled(bool bEnabled)
	{
		m_bLightEnabled = bEnabled;
	}

	public void DrawLightMap()
	{
		if (m_bLightEnabled)
		{
			m_ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, m_LightMixBlend);
			m_ScreenManager.SpriteBatch.Draw(m_lightMap, Vector2.Zero, Color.White);
			m_ScreenManager.SpriteBatch.End();
		}
	}

	public void BuildLightMap()
	{
		if (!m_bLightEnabled)
		{
			return;
		}
		m_ScreenManager.GraphicsDevice.SetRenderTarget(m_lightMap);
		m_ScreenManager.GraphicsDevice.Clear(m_AmbientColor);
		foreach (LightSource light in m_lights)
		{
			m_ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, m_LightmapBlend);
			light.Draw();
			m_ScreenManager.SpriteBatch.End();
		}
		m_ScreenManager.GraphicsDevice.SetRenderTarget(m_ScreenManager.ViewPort);
	}

	private void ClearAlphaToOne()
	{
		BlendState blendState = new BlendState();
		blendState.ColorWriteChannels = ColorWriteChannels.Alpha;
		m_ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, blendState);
		m_ScreenManager.SpriteBatch.Draw(m_alphaClearTexture, m_ScreenSource, Color.White);
		m_ScreenManager.SpriteBatch.End();
	}

	public void Clear()
	{
		m_lights.Clear();
		m_bLightEnabled = false;
		m_alphaClearTexture.Dispose();
		m_lightMap.Dispose();
		m_lightTexture.Dispose();
		LoadTexture();
	}
}
