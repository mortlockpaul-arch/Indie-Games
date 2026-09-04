using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using SpaceBlast.PathFinding;

namespace SpaceBlast;

internal class GameLevel
{
	public static int constLevelCount = 10;

	public int WorldWidth;

	public int WorldHeight;

	public StaticWorldObjectGrid StaticWorldObjects;

	public DynamicWorldObjectGrid DynamicWorldObjects;

	public WaypointList Waypoints;

	public PowerUpList PowerUps;

	private List<RespawnLocation> m_StartPositions;

	private bool m_UseFixedPositions;

	private Random m_Random = new Random();

	private int m_CurrentLevel = 6;

	private ContentManager m_ContentManager;

	public Texture2D BackgroundTex;

	public int CurrentLevel => m_CurrentLevel;

	public GameLevel()
	{
		Reset();
	}

	public int LoadLevel()
	{
		int num;
		for (num = m_CurrentLevel; num == m_CurrentLevel; num = m_Random.Next(Guide.IsTrialMode ? 2 : constLevelCount))
		{
		}
		LoadLevel(num);
		return num;
	}

	public void LoadLevel(int levelnumber)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		Reset();
		m_CurrentLevel = levelnumber;
		string path = Path.Combine(MainGame.TitlePath, "Content\\Levels\\Level" + levelnumber + ".xml");
		FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream);
		string xml = streamReader.ReadToEnd();
		fileStream.Close();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNode documentElement = xmlDocument.DocumentElement;
		XmlNode xmlNode = documentElement.SelectSingleNode("levelinfo/worldsize");
		WorldWidth = Convert.ToInt32(xmlNode.Attributes["width"].Value);
		WorldHeight = Convert.ToInt32(xmlNode.Attributes["height"].Value);
		XmlNode xmlNode2 = documentElement.SelectSingleNode("startpositions");
		m_UseFixedPositions = xmlNode2.Attributes["type"].Value == "fixed";
		XmlNodeList xmlNodeList = documentElement.SelectNodes("startpositions/startpos");
		foreach (XmlNode item2 in xmlNodeList)
		{
			string value = item2.Attributes["position"].Value;
			Vector3 position = Utils.StringToVector3(value);
			int num = Convert.ToInt16(item2.Attributes["rotation"].Value);
			float rotation = MathHelper.ToRadians((float)num);
			RespawnLocation item = new RespawnLocation
			{
				Position = position,
				Rotation = rotation
			};
			m_StartPositions.Add(item);
		}
		XmlNodeList xmlSObjects = documentElement.SelectNodes("staticobjects/sobj");
		StaticWorldObjects.LoadLevel(xmlSObjects, WorldWidth, WorldHeight);
		XmlNodeList xmlSObjects2 = documentElement.SelectNodes("dynamicobjects/dobj");
		DynamicWorldObjects.LoadLevel(xmlSObjects2, WorldWidth, WorldHeight);
		XmlNodeList xmlSObjects3 = documentElement.SelectNodes("waypoints/waypoint");
		Waypoints.LoadWaypoints(xmlSObjects3, WorldWidth, WorldHeight);
		XmlNodeList xmlPowerUps = documentElement.SelectNodes("powerups/powerup");
		PowerUps.LoadLevel(xmlPowerUps);
		if (m_ContentManager == null)
		{
			m_ContentManager = new ContentManager((IServiceProvider)((Game)MainGame.Instance).Services, "Content");
		}
		else
		{
			m_ContentManager.Unload();
		}
		XmlNode xmlNode4 = documentElement.SelectSingleNode("levelinfo/background");
		string value2 = xmlNode4.Attributes["texture"].Value;
		BackgroundTex = m_ContentManager.Load<Texture2D>("Textures/" + value2);
	}

	private void Reset()
	{
		m_CurrentLevel = 1;
		StaticWorldObjects = new StaticWorldObjectGrid();
		DynamicWorldObjects = new DynamicWorldObjectGrid();
		Waypoints = new WaypointList();
		PowerUps = new PowerUpList();
		m_StartPositions = new List<RespawnLocation>();
		m_UseFixedPositions = false;
		BackgroundTex = null;
	}

	public RespawnLocation GetPlayerStartPosition(int playerIndex)
	{
		return m_StartPositions[playerIndex];
	}

	public RespawnLocation GetPlayerRespawnPosition(byte playerid)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		RespawnLocation result = default(RespawnLocation);
		int num = 0;
		bool flag = false;
		List<Player> players = MainGame.Players.GetPlayers();
		Player player = MainGame.Players.GetPlayer(playerid);
		BoundingSphere val = default(BoundingSphere);
		while (num++ < 8 && !flag)
		{
			int index = m_Random.Next(m_StartPositions.Count - 1);
			result = m_StartPositions[index];
			((BoundingSphere)(ref val))._002Ector(result.Position, 20000f);
			flag = true;
			foreach (Player item in players)
			{
				if (item != player)
				{
					BoundingSphere boundingSphere = item.TheShip.GetBoundingSphere();
					if (((BoundingSphere)(ref boundingSphere)).Intersects(val))
					{
						flag = false;
						break;
					}
				}
			}
		}
		return result;
	}

	public int GetNextLevel()
	{
		int num = m_CurrentLevel + 1;
		if (num >= constLevelCount)
		{
			num = 0;
		}
		return num;
	}
}
