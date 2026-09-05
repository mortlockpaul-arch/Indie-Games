using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using MusicPlayer;
using PhysicsHandler;
using PlayObjects;
using PlayObjects.Props;
using Renderer;
using Screens;

namespace Scene;

public class SceneContainer
{
	private const float DIST_BETWEEN_LIGHTS = 1100f;

	private Player m_player;

	private List<Prop> m_objs;

	private List<ObstacleRect> m_floors;

	private List<ObstacleRect> m_roofs;

	private SceneObjectSpawner m_spawner;

	private List<RenderLight> m_lights;

	private RenderLight m_outdoorLight;

	private LaunchHelper m_launchHelper;

	private List<AmbianceElement> m_ambiance;

	private List<AmbianceElement> m_ambianceRemover;

	private Vector2 m_lastCamPos;

	private bool m_bSpawnedScoreScreen;

	private float m_fOutdoorAlpha;

	public SceneContainer()
	{
		PhysicsObjectManager.Initialize(200f);
		m_player = new Player();
		InitLights();
		m_objs = new List<Prop>();
		m_floors = new List<ObstacleRect>();
		m_roofs = new List<ObstacleRect>();
		m_ambiance = new List<AmbianceElement>();
		m_ambianceRemover = new List<AmbianceElement>();
		m_spawner = new SceneObjectSpawner(m_player, m_objs, m_floors, m_roofs, m_ambiance);
		m_launchHelper = new LaunchHelper();
		m_lastCamPos = default(Vector2);
		m_bSpawnedScoreScreen = false;
		m_fOutdoorAlpha = 0f;
	}

	public void Update(TimeTracker gameTime)
	{
		if (m_launchHelper.IsCompleted())
		{
			PhysicsObjectManager.Update(gameTime);
		}
		else
		{
			m_launchHelper.Update(gameTime);
			if (m_launchHelper.IsCompleted())
			{
				m_player.Launch(m_launchHelper.Pow);
				Mp3MusicPlayer.Initialize("sounds/incompetech/Big Rock", shouldReplay: true, forceReplay: true);
			}
		}
		m_player.Update(gameTime);
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_objs[i].Update(gameTime);
			m_objs[i].UpdateEnabled();
		}
		for (int j = 0; j < m_floors.Count; j++)
		{
			m_floors[j].Update(gameTime);
		}
		for (int k = 0; k < m_roofs.Count; k++)
		{
			m_roofs[k].Update(gameTime);
		}
		if (m_lights[3].pos.X < SceneRenderer.GetCameraPosition().X)
		{
			RenderLight renderLight = m_lights[0];
			m_lights.RemoveAt(0);
			renderLight.pos = m_lights.Last().pos + new Vector3(1100f, 0f, 0f);
			m_lights.Add(renderLight);
		}
		for (int l = 0; l < m_ambiance.Count; l++)
		{
			m_ambiance[l].Update(gameTime, m_lastCamPos - SceneRenderer.GetCameraPosition());
			if (m_ambiance[l].Sprite.Position.X + m_ambiance[l].Sprite.SurfaceScale.X < SceneRenderer.GetCameraPosition().X - SceneRenderer.GetScreenDim().X / 2f)
			{
				m_ambianceRemover.Add(m_ambiance[l]);
			}
		}
		m_lastCamPos = SceneRenderer.GetCameraPosition();
		for (int m = 0; m < m_ambianceRemover.Count; m++)
		{
			m_ambiance.Remove(m_ambianceRemover[m]);
		}
		m_ambianceRemover.Clear();
		UpdateSpawner(gameTime);
		if (m_launchHelper.IsCompleted() && m_player.IsStopped() && !m_bSpawnedScoreScreen)
		{
			m_bSpawnedScoreScreen = true;
			List<string> list = new List<string>();
			MasterOfUnlocking.GetNewAvailableModes(list, new List<int>(), this);
			MasterOfUnlocking.GetNewAvailablePowerups(list, new List<int>(), this);
			MasterOfUnlocking.GetNewAvailableOutfits(list, new List<int>(), this);
			new ScoreScreen(m_player, this);
			if (list.Count > 0)
			{
				new UnlockScreen(list);
			}
			if (Game1.IsTrial())
			{
				TrialUpseller.SpawnUpsell(b: false);
			}
		}
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (m_launchHelper.IsCompleted())
		{
			m_player.HandleInput(gameTime);
		}
		else
		{
			m_launchHelper.HandleInput(gameTime);
		}
	}

	public void Draw(TimeTracker gameTime, float fadeAmount)
	{
		m_player.Draw(gameTime, fadeAmount);
		bool flag = false;
		for (int i = 0; i < m_roofs.Count; i++)
		{
			if (m_roofs[i].Position.X - 500f < m_player.Position.X && m_roofs[i].Position.X + 500f > m_player.Position.X)
			{
				flag = true;
			}
		}
		if (0 == 0)
		{
			m_fOutdoorAlpha += gameTime.FractionOfSecond * 3f;
			if (m_fOutdoorAlpha > 1f)
			{
				m_fOutdoorAlpha = 1f;
			}
		}
		else
		{
			m_fOutdoorAlpha -= gameTime.FractionOfSecond * 3f;
			if (m_fOutdoorAlpha < 0f)
			{
				m_fOutdoorAlpha = 0f;
			}
		}
		m_outdoorLight.color.A = (byte)(255f * m_fOutdoorAlpha);
		Vector2 value = SceneRenderer.GetCameraPosition() + new Vector2(640f, -200f);
		value.Y = 0f - value.Y;
		m_outdoorLight.pos = new Vector3(value, 1250f);
		m_outdoorLight.Draw(gameTime);
		for (int j = 0; j < m_objs.Count; j++)
		{
			m_objs[j].Draw(gameTime);
		}
		for (int k = 0; k < m_floors.Count; k++)
		{
			m_floors[k].Draw(gameTime);
		}
		for (int l = 0; l < m_roofs.Count; l++)
		{
			m_roofs[l].Draw(gameTime);
		}
		if (!m_launchHelper.IsCompleted())
		{
			m_launchHelper.Draw(gameTime);
		}
		for (int m = 0; m < m_ambiance.Count; m++)
		{
			m_ambiance[m].Draw(gameTime);
		}
	}

	private void UpdateSpawner(TimeTracker gameTime)
	{
		m_spawner.Update(gameTime);
	}

	public void Reset()
	{
		Mp3MusicPlayer.Pause();
		m_player.Reset();
		m_ambiance.Clear();
		m_spawner.Reset();
		m_launchHelper = new LaunchHelper();
		InitLights();
		m_bSpawnedScoreScreen = false;
		m_fOutdoorAlpha = 0f;
	}

	public Player GetPlayer()
	{
		return m_player;
	}

	private void InitLights()
	{
		m_lights = new List<RenderLight>();
		for (int i = 0; i < 5; i++)
		{
			m_lights.Add(new RenderLight(new Vector3(-1600f + (float)i * 1100f, 700f, 350f), 0.2f, 1600, new Color(0.6f, 0.6f, 0.3f)));
		}
		m_outdoorLight = new RenderLight(new Vector3(-1600f, 700f, 1250f), 0.2f, 2500, new Color(0.6f, 0.6f, 0.3f));
	}

	public void SetDefaultWorld(int i)
	{
		m_spawner.SetDefaultWorld(i);
	}

	public void SetInfiniteWorld(bool b)
	{
		m_spawner.SetInfiniteWorld(b);
	}

	public int SceneType()
	{
		return m_spawner.GetWorldType();
	}

	public SceneObjectSpawner GetSceneObjectSpawner()
	{
		return m_spawner;
	}
}
