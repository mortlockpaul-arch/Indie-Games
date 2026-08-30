using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Scene;

internal class RoomDrawer
{
	private const int BG_WIDTH = 1280;

	private List<RenderSprite> m_bg;

	private List<RenderSprite> m_floor;

	private List<RenderSprite> m_roof;

	private Rectangle m_area;

	public RoomDrawer()
	{
		m_bg = new List<RenderSprite>();
		m_bg.Add(SpriteManager.GetSprite("images/roomDraw/backDrop", new Vector2(-2000f), DepthConsts.BACKGROUND_DEPTH));
		m_bg.Add(SpriteManager.GetSprite("images/roomDraw/backDrop", new Vector2(-2000f), DepthConsts.BACKGROUND_DEPTH - 0.1f));
		m_floor = new List<RenderSprite>();
		m_floor.Add(SpriteManager.GetSprite("images/roomDraw/floor", new Vector2(-2000f), DepthConsts.FLOOR_DEPTH));
		m_floor.Add(SpriteManager.GetSprite("images/roomDraw/floor", new Vector2(-2000f), DepthConsts.FLOOR_DEPTH - 0.1f));
		m_roof = new List<RenderSprite>();
		m_roof.Add(SpriteManager.GetSprite("images/roomDraw/roof", new Vector2(-2000f), DepthConsts.FLOOR_DEPTH));
		m_roof.Add(SpriteManager.GetSprite("images/roomDraw/roof", new Vector2(-2000f), DepthConsts.FLOOR_DEPTH - 0.1f));
		m_area = default(Rectangle);
	}

	public void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i < m_bg.Count; i++)
		{
			if (i == 0 || m_area.Width > 1280)
			{
				m_bg[i].Draw(gameTime);
				m_floor[i].Draw(gameTime);
				m_roof[i].Draw(gameTime);
			}
		}
	}

	public void Update(TimeTracker gameTime)
	{
		Vector2 cameraPosition = SceneRenderer.GetCameraPosition();
		if (cameraPosition.X > m_bg[1].Position.X && (float)(m_area.X + m_area.Width) > m_bg[1].Position.X + 640f)
		{
			m_bg[0].Position = m_bg[1].Position;
			m_bg[1].Position = new Vector2(m_bg[1].Position.X + 1280f, m_bg[1].Position.Y);
			m_floor[0].Position = new Vector2(m_bg[0].Position.X, m_floor[0].Position.Y);
			m_floor[1].Position = new Vector2(m_bg[1].Position.X, m_floor[1].Position.Y);
			m_roof[0].Position = new Vector2(m_bg[0].Position.X, m_roof[0].Position.Y);
			m_roof[1].Position = new Vector2(m_bg[1].Position.X, m_roof[1].Position.Y);
		}
		else if (cameraPosition.X < m_bg[0].Position.X && (float)m_area.X + 640f + 1f < m_bg[0].Position.X)
		{
			m_bg[1].Position = m_bg[0].Position;
			m_bg[0].Position = new Vector2(m_bg[1].Position.X - 1280f, m_bg[1].Position.Y);
			m_floor[0].Position = new Vector2(m_bg[0].Position.X, m_floor[0].Position.Y);
			m_floor[1].Position = new Vector2(m_bg[1].Position.X, m_floor[1].Position.Y);
			m_roof[0].Position = new Vector2(m_bg[0].Position.X, m_roof[0].Position.Y);
			m_roof[1].Position = new Vector2(m_bg[1].Position.X, m_roof[1].Position.Y);
		}
	}

	public void SetType(RoomType type)
	{
		Color color;
		Color color2;
		switch (type)
		{
		case RoomType.BIRTHROOM:
			color = new Color(205, 150, 96);
			color2 = new Color(176, 214, 210);
			break;
		case RoomType.CAFETERIA:
			color = new Color(180, 162, 126);
			color2 = new Color(214, 155, 155);
			break;
		case RoomType.HALLWAY:
			color = new Color(216, 220, 167);
			color2 = new Color(135, 220, 151);
			break;
		case RoomType.WAITINGROOM:
			color = new Color(93, 174, 145);
			color2 = new Color(200, 159, 159);
			break;
		case RoomType.BEDSROOM:
			color = new Color(162, 196, 195);
			color2 = new Color(98, 118, 118);
			break;
		case RoomType.PHYSIO:
			color = new Color(145, 114, 100);
			color2 = new Color(233, 76, 0);
			break;
		case RoomType.LAB:
			color = new Color(198, 207, 227);
			color2 = new Color(92, 92, 70);
			break;
		case RoomType.SURGERYTHEATRE:
			color = new Color(180, 207, 250);
			color2 = new Color(250, 237, 130);
			break;
		case RoomType.DIAGNOSIS:
			color = new Color(245, 245, 142);
			color2 = new Color(128, 128, 70);
			break;
		case RoomType.MORTUARY:
			color = new Color(198, 156, 199);
			color2 = new Color(84, 57, 162);
			break;
		default:
			color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
			color2 = new Color(100, 100, 100);
			break;
		}
		for (int i = 0; i < m_bg.Count; i++)
		{
			m_bg[i].Color = color;
			m_floor[i].Color = color2;
		}
	}

	public void SetArea(Rectangle r)
	{
		m_area = r;
		if (m_area.Width < 1280)
		{
			m_bg[0].GetSpriteImage().Width = m_area.Width + 1;
			m_bg[0].SurfaceScale = new Vector2(m_area.Width + 1, m_bg[0].SurfaceScale.Y);
			m_floor[0].GetSpriteImage().Width = m_area.Width + 1;
			m_floor[0].SurfaceScale = new Vector2(m_area.Width + 1, m_floor[0].SurfaceScale.Y);
			m_roof[0].GetSpriteImage().Width = m_area.Width + 1;
			m_roof[0].SurfaceScale = new Vector2(m_area.Width + 1, m_roof[0].SurfaceScale.Y);
		}
		else
		{
			m_bg[0].GetSpriteImage().Width = m_bg[0].Texture.Width;
			m_bg[0].SurfaceScale = new Vector2(m_bg[0].Texture.Width, m_bg[0].SurfaceScale.Y);
			m_floor[0].GetSpriteImage().Width = m_floor[0].Texture.Width;
			m_floor[0].SurfaceScale = new Vector2(m_floor[0].Texture.Width, m_floor[0].SurfaceScale.Y);
			m_roof[0].GetSpriteImage().Width = m_roof[0].Texture.Width;
			m_roof[0].SurfaceScale = new Vector2(m_roof[0].Texture.Width, m_roof[0].SurfaceScale.Y);
		}
		m_bg[0].Position = new Vector2((float)r.X + (m_bg[0].SurfaceScale.X - 1f) / 2f, (float)r.Bottom - m_floor[0].SurfaceScale.Y - m_bg[0].SurfaceScale.Y / 2f);
		m_bg[1].Position = new Vector2((float)r.X + (m_bg[1].SurfaceScale.X - 1f) * 1.5f, (float)r.Bottom - m_floor[0].SurfaceScale.Y - m_bg[0].SurfaceScale.Y / 2f);
		m_floor[0].Position = new Vector2((float)r.X + (m_bg[0].SurfaceScale.X - 1f) / 2f, (float)r.Bottom - m_floor[0].SurfaceScale.Y / 2f);
		m_floor[1].Position = new Vector2((float)r.X + (m_bg[1].SurfaceScale.X - 1f) * 1.5f, (float)r.Bottom - m_floor[0].SurfaceScale.Y / 2f);
		m_roof[0].Position = new Vector2((float)r.X + (m_bg[0].SurfaceScale.X - 1f) / 2f, m_bg[0].Position.Y - m_bg[0].SurfaceScale.Y / 2f - m_roof[0].SurfaceScale.Y / 2f);
		m_roof[1].Position = new Vector2((float)r.X + (m_bg[1].SurfaceScale.X - 1f) * 1.5f, m_bg[0].Position.Y - m_bg[0].SurfaceScale.Y / 2f - m_roof[0].SurfaceScale.Y / 2f);
	}
}
