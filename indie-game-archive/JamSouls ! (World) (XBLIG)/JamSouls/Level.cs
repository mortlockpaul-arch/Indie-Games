using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;

namespace JamSouls;

public class Level
{
	public struct DummyPoint
	{
		public Vector2 Position;

		public string name;

		public SpriteEffects m_Flip;
	}

	public const int GOAL_THICKNESS = 5;

	public List<WayPoint> m_OpenList = new List<WayPoint>();

	public List<WayPoint> m_CloseList = new List<WayPoint>();

	public TriggerTrap[] m_TriggerTrap;

	public AnimatedTrap[] m_AnimatedTrap;

	public Rectangle[] m_TriggerRect;

	public List<WayPoint> m_WayPoints = new List<WayPoint>();

	public float m_RefZorder = 0.1f;

	public List<DummyPoint> DummyList = new List<DummyPoint>();

	private GameState m_GameStateInstance;

	public Level(GameState StateInstance, string Res, bool bGameLevel)
	{
		m_GameStateInstance = StateInstance;
		string text = ((!bGameLevel) ? ("Menus\\" + Res + "\\") : ("Level\\" + Res + "\\"));
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int framecount = 0;
		float speed = 0f;
		float num5 = 0f;
		SpriteEffects spriteEffects = SpriteEffects.None;
		string text2 = "";
		string text3 = "";
		bool flag = true;
		string text4 = "";
		Color red = Color.Red;
		XmlReaderSettings settings = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment,
			IgnoreWhitespace = true,
			IgnoreComments = true
		};
		XmlReader xmlReader = XmlReader.Create(StateInstance.content.RootDirectory + "\\" + text + Res + ".jlvl", settings);
		if (Res == "Vice")
		{
			m_TriggerTrap = new TriggerTrap[4];
			m_AnimatedTrap = new AnimatedTrap[4];
			m_TriggerRect = new Rectangle[4];
		}
		DummyPoint dummyPoint = default(DummyPoint);
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.AttributeCount <= 0)
			{
				continue;
			}
			switch (xmlReader.Name)
			{
			case "Box":
			{
				num = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				num3 = int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				num4 = int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				text3 = xmlReader.GetAttribute(4).ToString(CultureInfo.InvariantCulture);
				if (text3.Contains("!"))
				{
					int num8 = int.Parse(text3.Split('!')[1]);
					ref Rectangle reference = ref m_TriggerRect[num8 - 1];
					reference = new Rectangle(num, num2, num3, num4);
					break;
				}
				flag = bool.Parse(xmlReader.GetAttribute(5).ToString(CultureInfo.InvariantCulture));
				text4 = xmlReader.GetAttribute(6).ToString(CultureInfo.InvariantCulture);
				Body body = m_GameStateInstance.m_PhysicManager.CreateBody();
				body.BodyType = BodyType.Static;
				body.Position = new Vector2((float)(num + num3 / 2) / 10f, (float)(num2 + num4 / 2) / 10f);
				PolygonShape polygonShape2 = new PolygonShape();
				polygonShape2.SetAsBox((float)(num3 / 2) / 10f, (float)(num4 / 2) / 10f);
				Fixture fixture = body.CreateFixture(polygonShape2);
				fixture.UserData = null;
				if (text4 == "KILL")
				{
					fixture.CollisionCategories = CollisionCategory.Cat8;
				}
				else if (text4 == "BURN")
				{
					fixture.CollisionCategories = CollisionCategory.Cat10;
				}
				else
				{
					if (flag)
					{
						fixture.CollisionCategories = CollisionCategory.Cat2;
					}
					else
					{
						fixture.CollisionCategories = CollisionCategory.Cat3;
					}
					if (text3.EndsWith("Bound"))
					{
						fixture.CollisionCategories = CollisionCategory.Cat5;
					}
					else if (text3.StartsWith("Slow"))
					{
						fixture.CollisionCategories = CollisionCategory.Cat11;
					}
				}
				fixture.CollidesWith = CollisionCategory.All;
				if (!text3.Contains("Goal"))
				{
					break;
				}
				fixture.CollidesWith = CollisionCategory.Cat12;
				fixture.CollisionCategories = CollisionCategory.Cat13;
				if (!text3.Contains("Zone"))
				{
					break;
				}
				fixture.CollidesWith = CollisionCategory.None;
				fixture.CollisionCategories = CollisionCategory.None;
				if (GameContext.GameMode == GAME_MODE.JAM_BALL)
				{
					JamBall jamBall = (JamBall)m_GameStateInstance;
					if (text3.Contains("Red"))
					{
						jamBall.m_RedGoal = new Rectangle(num, num2, num3, num4);
					}
					else
					{
						jamBall.m_BlueGoal = new Rectangle(num, num2, num3, num4);
					}
				}
				break;
			}
			case "PlayerStart":
			{
				num = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				SpawnPoint spawnPoint = new SpawnPoint
				{
					Position = new Vector2(num, num2),
					zOrder = float.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture)
				};
				if (int.Parse(xmlReader.GetAttribute(4), CultureInfo.InvariantCulture) == 0)
				{
					spawnPoint.Team = PlayerConfig.BLUE_TEAM_COLOR;
				}
				else
				{
					spawnPoint.Team = PlayerConfig.RED_TEAM_COLOR;
				}
				m_RefZorder = spawnPoint.zOrder;
				spawnPoint.bIsFree = true;
				m_GameStateInstance.m_SpawnInfo.Add(spawnPoint);
				break;
			}
			case "Dummy":
			{
				DummyPoint item2 = new DummyPoint
				{
					name = xmlReader.GetAttribute(0).ToString(CultureInfo.InvariantCulture),
					Position = new Vector2(int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture), int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture))
				};
				if (xmlReader.AttributeCount > 3)
				{
					item2.m_Flip = (SpriteEffects)int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				}
				DummyList.Add(item2);
				break;
			}
			case "WayPoint":
			{
				WayPoint item = new WayPoint(new Vector2(int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture), int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture)), int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture), bool.Parse(xmlReader.GetAttribute(4)));
				m_WayPoints.Add(item);
				break;
			}
			case "Flag":
				if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM)
				{
					string text5 = xmlReader.GetAttribute(0).ToString(CultureInfo.InvariantCulture);
					Flag entity = new Flag(color: (text5.IndexOf("Blue") == -1) ? PlayerConfig.RED_TEAM_COLOR : PlayerConfig.BLUE_TEAM_COLOR, Position: new Vector2(int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture), int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture)), gameinstance: m_GameStateInstance)
					{
						Name = xmlReader.GetAttribute(0).ToString(CultureInfo.InvariantCulture)
					};
					m_GameStateInstance.AddEntity(entity);
				}
				break;
			case "PowerUp":
				dummyPoint.Position = new Vector2(int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture), int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture));
				m_GameStateInstance.PowerUpSpawnList.Add(dummyPoint.Position);
				break;
			case "JamPowerUp":
				if (GameContext.GameMode == GAME_MODE.DEATHMATCH || GameContext.GameMode == GAME_MODE.STORYMATCH)
				{
					m_GameStateInstance.m_SoulSpawnPoint.Add(new Vector2(int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture), int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture)));
				}
				break;
			case "Anim":
			{
				num = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				num5 = float.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				num3 = int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				num4 = int.Parse(xmlReader.GetAttribute(4), CultureInfo.InvariantCulture);
				framecount = int.Parse(xmlReader.GetAttribute(5), CultureInfo.InvariantCulture);
				speed = float.Parse(xmlReader.GetAttribute(6), CultureInfo.InvariantCulture);
				int.Parse(xmlReader.GetAttribute(7), CultureInfo.InvariantCulture);
				text3 = xmlReader.GetAttribute(8).ToString(CultureInfo.InvariantCulture);
				text2 = xmlReader.GetAttribute(9).ToString(CultureInfo.InvariantCulture);
				spriteEffects = (SpriteEffects)int.Parse(xmlReader.GetAttribute(10), CultureInfo.InvariantCulture);
				Color color = new Color
				{
					A = byte.Parse(xmlReader.GetAttribute(11), CultureInfo.InvariantCulture),
					R = byte.Parse(xmlReader.GetAttribute(12), CultureInfo.InvariantCulture),
					G = byte.Parse(xmlReader.GetAttribute(13), CultureInfo.InvariantCulture),
					B = byte.Parse(xmlReader.GetAttribute(14), CultureInfo.InvariantCulture)
				};
				float startoffset = 0f;
				if (text3.Contains(":"))
				{
					startoffset = int.Parse(text3.Split(':')[1]);
				}
				if (text3.Contains("!"))
				{
					AnimatedTrap animatedTrap = m_GameStateInstance.AddTrap(text + text2, num, num2, num3, num4, framecount, speed, text2, spriteEffects, num5);
					int num6 = int.Parse(text3.Split('!')[1]);
					m_AnimatedTrap[num6 - 1] = animatedTrap;
				}
				else
				{
					m_GameStateInstance.AddAnim(text + text2, num, num2, num3, num4, framecount, speed, text2, spriteEffects, num5, color, startoffset);
				}
				break;
			}
			case "Layer":
			{
				num = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				num5 = float.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				text3 = xmlReader.GetAttribute(3).ToString(CultureInfo.InvariantCulture);
				text2 = xmlReader.GetAttribute(4).ToString(CultureInfo.InvariantCulture);
				spriteEffects = (SpriteEffects)int.Parse(xmlReader.GetAttribute(5), CultureInfo.InvariantCulture);
				Color color2 = new Color
				{
					A = byte.Parse(xmlReader.GetAttribute(6), CultureInfo.InvariantCulture),
					R = byte.Parse(xmlReader.GetAttribute(7), CultureInfo.InvariantCulture),
					G = byte.Parse(xmlReader.GetAttribute(8), CultureInfo.InvariantCulture),
					B = byte.Parse(xmlReader.GetAttribute(9), CultureInfo.InvariantCulture)
				};
				if (text3.Contains("Scroller"))
				{
					m_GameStateInstance.AddLayerWave(text + text2, num, num2, text + "Wave", spriteEffects, num5, color2);
				}
				else if (text3.Contains("Thunder"))
				{
					m_GameStateInstance.AddLayerFadeFx(text + text2, num, num2, text + "Wave", spriteEffects, num5, color2, m_GameStateInstance.m_Entities.Count);
				}
				else if (text2.Contains("Goal"))
				{
					if (GameContext.GameMode != GAME_MODE.JAM_BALL)
					{
						break;
					}
					_ = (JamBall)m_GameStateInstance;
					if (text3.Contains("Red"))
					{
						m_GameStateInstance.AddLayer("level/" + GameContext.SelectedLevel + "/GoalRed", num, num2, text3, spriteEffects, GameContext.PLAYER_Z - 0.005f, color2);
						m_GameStateInstance.AddLayer("level/" + GameContext.SelectedLevel + "/GoalFrontRed", num, num2, text3, spriteEffects, GameContext.BALL_Z + 0.005f, color2);
					}
					else if (text3.Contains("Blue"))
					{
						m_GameStateInstance.AddLayer("level/" + GameContext.SelectedLevel + "/GoalBlue", num, num2, text3, spriteEffects, GameContext.PLAYER_Z - 0.005f, color2);
						m_GameStateInstance.AddLayer("level/" + GameContext.SelectedLevel + "/GoalFrontBlue", num, num2, text3, spriteEffects, GameContext.BALL_Z + 0.005f, color2);
					}
					if (text3.Contains("Top"))
					{
						BackgroundLayer backgroundLayer = (BackgroundLayer)m_GameStateInstance.m_Entities[m_GameStateInstance.m_Entities.Count - 1];
						Body body = m_GameStateInstance.m_PhysicManager.CreateBody();
						body.BodyType = BodyType.Static;
						num = ((backgroundLayer.GetSpriteEffect() == SpriteEffects.None) ? (num + (backgroundLayer.Width / 2 - 18)) : (num + (backgroundLayer.Width / 2 + 18)));
						num2 = 15;
						body.Position = new Vector2((float)num / 10f, (float)(num2 + 280) / 10f);
						PolygonShape polygonShape = new PolygonShape();
						polygonShape.SetAsBox((float)(backgroundLayer.Width / 2) / 10f, 0.5f);
						Fixture fixture = body.CreateFixture(polygonShape);
						fixture.UserData = null;
						fixture.CollisionCategories = CollisionCategory.Cat13;
						fixture.CollidesWith = CollisionCategory.Cat12;
						if (backgroundLayer.GetSpriteEffect() != SpriteEffects.None)
						{
							body.Rotation = -70f;
						}
						else
						{
							body.Rotation = 70f;
						}
					}
				}
				else if (text3.Contains("!"))
				{
					string text6 = text2.Replace("UP", "DN");
					TriggerTrap triggerTrap = m_GameStateInstance.AddTrigger(text + text2, text + text6, num, num2, num3, num4, framecount, speed, text2, spriteEffects, num5);
					int num7 = int.Parse(text3.Split('!')[1]);
					m_TriggerTrap[num7 - 1] = triggerTrap;
				}
				else if (!text3.Contains("FireBall"))
				{
					m_GameStateInstance.AddLayer(text + text2, num, num2, text3, spriteEffects, num5, color2);
				}
				break;
			}
			case "Particle":
			{
				num = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				num5 = float.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				bool bUseBlending = bool.Parse(xmlReader.GetAttribute(3));
				text3 = xmlReader.GetAttribute(4).ToString(CultureInfo.InvariantCulture);
				text2 = xmlReader.GetAttribute(5).ToString(CultureInfo.InvariantCulture);
				spriteEffects = (SpriteEffects)int.Parse(xmlReader.GetAttribute(6), CultureInfo.InvariantCulture);
				ParticleEffect particleEffect = new ParticleEffect();
				particleEffect = StateInstance.content.Load<ParticleEffect>(text + Path.GetFileNameWithoutExtension(text2));
				MercuryParticle mpe = new MercuryParticle(StateInstance, num, num2, particleEffect.DeepCopy(), text3, num5, bUseBlending);
				StateInstance.AddParticle(mpe);
				break;
			}
			case "Light":
			{
				num = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				int range = int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				Color white = Color.White;
				white.A = byte.Parse(xmlReader.GetAttribute(4), CultureInfo.InvariantCulture);
				white.R = byte.Parse(xmlReader.GetAttribute(5), CultureInfo.InvariantCulture);
				white.G = byte.Parse(xmlReader.GetAttribute(6), CultureInfo.InvariantCulture);
				white.B = byte.Parse(xmlReader.GetAttribute(7), CultureInfo.InvariantCulture);
				m_GameStateInstance.GetLightManager().AddLight(white, range, new Vector2(num, num2));
				break;
			}
			case "AmbientLight":
			{
				Color white = Color.White;
				white.A = byte.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				white.R = byte.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				white.G = byte.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				white.B = byte.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				if (white.R == byte.MaxValue && white.G == byte.MaxValue && white.B == byte.MaxValue)
				{
					m_GameStateInstance.GetLightManager().SetLightEnabled(bEnabled: false);
				}
				else
				{
					m_GameStateInstance.GetLightManager().SetLightEnabled(bEnabled: true);
				}
				m_GameStateInstance.GetLightManager().SetAmbientLight(white);
				break;
			}
			case "Spring":
				num = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				num2 = int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				num5 = float.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				m_GameStateInstance.AddEntity(new Spring(m_GameStateInstance, m_GameStateInstance.LoadAnimatedSpriteFromXml(text + "Spring.xml", text + "Spring"), new Vector2(num, num2), num5));
				break;
			}
		}
		if (m_TriggerTrap != null)
		{
			for (int i = 0; i < m_TriggerTrap.Length; i++)
			{
				m_TriggerTrap[i].m_TriggerRect = m_TriggerRect[i];
				m_AnimatedTrap[i].m_Trigger = m_TriggerTrap[i];
			}
		}
		if (File.Exists(StateInstance.content.RootDirectory + "\\PathLink\\" + Res + "_PathLink.xml"))
		{
			string text7 = StateInstance.content.RootDirectory + "\\PathLink\\" + Res + "_PathLink.xml";
			if (bGameLevel && File.Exists(text7))
			{
				ProcessLinksInfo(text7);
			}
		}
	}

	public void ProcessLinksInfo(string PathLinkPath)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.IgnoreWhitespace = true;
		xmlReaderSettings.IgnoreComments = true;
		XmlReader xmlReader = XmlReader.Create(PathLinkPath);
		WayPoint wayPoint = null;
		WayPoint wp = null;
		List<BotInputEvent> list = new List<BotInputEvent>();
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element)
			{
				if (xmlReader.Name.Contains("WayPoint"))
				{
					wayPoint = GetWayPointById(int.Parse(xmlReader.Name.Split('-')[1]));
				}
				else if (xmlReader.Name.Contains("NeightBor"))
				{
					wp = GetWayPointById(int.Parse(xmlReader.Name.Split('-')[1]));
				}
				else if (xmlReader.Name.Contains("InputEvent"))
				{
					list.Add(new BotInputEvent(bool.Parse(xmlReader.GetAttribute(1).ToString()), bool.Parse(xmlReader.GetAttribute(2).ToString()), bool.Parse(xmlReader.GetAttribute(3).ToString()), bool.Parse(xmlReader.GetAttribute(4).ToString()), bool.Parse(xmlReader.GetAttribute(5).ToString())));
					list[list.Count - 1].duration = float.Parse(xmlReader.GetAttribute(0).ToString(), CultureInfo.InvariantCulture);
				}
			}
			else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name.Contains("NeightBor"))
			{
				wayPoint.AddNeightBour(wp, list);
				list.Clear();
				wp = null;
			}
		}
	}

	public void Destroy()
	{
		m_TriggerRect = null;
		m_TriggerTrap = null;
		m_AnimatedTrap = null;
		m_WayPoints.Clear();
	}

	public void ClearParent()
	{
		PlayerBot.PathNodes parent = default(PlayerBot.PathNodes);
		parent.wp = null;
		parent.index = -1;
		foreach (WayPoint wayPoint in m_WayPoints)
		{
			wayPoint.m_Parent = parent;
		}
	}

	public DummyPoint GetDummyByName(string name)
	{
		for (int i = 0; i < DummyList.Count; i++)
		{
			if (DummyList[i].name == name)
			{
				return DummyList[i];
			}
		}
		return default(DummyPoint);
	}

	public WayPoint GetWayPointById(int nid)
	{
		foreach (WayPoint wayPoint in m_WayPoints)
		{
			if (wayPoint.m_nId == nid)
			{
				return wayPoint;
			}
		}
		return null;
	}
}
