using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;
using Screens;

namespace PlayObjects.Props;

public class TrialUpseller : PropEffector
{
	private bool m_bActivated;

	private PhysicalRepresentation m_body;

	private static int m_iNumBearActivates = 0;

	private static int m_iNumFails = 0;

	private static bool m_bJustSpawned;

	public TrialUpseller(PhysicalRepresentation body)
	{
		m_body = body;
		m_bActivated = false;
	}

	public static void SpawnUpsell(bool b)
	{
		bool flag = b;
		if (!m_bJustSpawned)
		{
			if (!b)
			{
				m_iNumFails++;
				if (m_iNumFails >= 3)
				{
					m_iNumFails = 0;
					flag = true;
				}
			}
			m_bJustSpawned = false;
		}
		if (flag)
		{
			List<SpriteInstance> list = new List<SpriteInstance>();
			int iNumBearActivates = m_iNumBearActivates;
			switch (iNumBearActivates)
			{
			case 1:
				list.Add(TextureContainer.GetSprite("images/upsells/upsell4", default(Vector2), DepthConsts.PAUSE_DEPTH + 0.1f));
				if (SceneRenderer.GetScreenDim().X > 1024f)
				{
					list.Last().WidthScale *= 0.6f;
					list.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(250f, 80f);
				}
				else
				{
					list.Last().WidthScale *= 0.5f;
					list.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(200f, 100f);
				}
				list.Last().FlatColor = true;
				break;
			case 4:
				list.Add(TextureContainer.GetSprite("images/upsells/upsell5", default(Vector2), 200f));
				if (SceneRenderer.GetScreenDim().X > 1024f)
				{
					list.Last().WidthScale *= 0.9f;
					list.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(270f, 100f);
				}
				else
				{
					list.Last().WidthScale *= 0.8f;
					list.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(180f, 80f);
				}
				list.Last().FlatColor = true;
				break;
			default:
				list.Add(TextureContainer.GetSprite("images/upsells/upsell1", default(Vector2), 200f));
				list.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(250f, 60f);
				list.Last().FlatColor = true;
				break;
			}
			if (!b)
			{
				list.Add(TextureContainer.GetSprite("images/spritesheets/upsellBear", new Rectangle(14, 20, 626, 586), default(Vector2), 0f));
				list.Last().Position = SceneRenderer.GetCameraPosition() + new Vector2(250f, 0f);
			}
			new UpsellScreen(isGame: true, list, iNumBearActivates, b);
			m_iNumBearActivates++;
		}
		m_bJustSpawned = b;
		if (m_bJustSpawned)
		{
			m_iNumFails = 0;
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (!m_bActivated && SceneRenderer.GetCameraPosition().X + 300f > m_body.Position.X)
		{
			m_bActivated = true;
			SpawnUpsell(b: true);
		}
	}

	public override void Reset()
	{
		m_bActivated = false;
	}
}
