using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class PathBuilder
{
	private PathBuildingScreen m_PathScreen;

	public WayPoint m_CurrentWayPointRecord;

	public WayPoint m_LastWayPointRecord;

	private float m_PathCorrectionTime;

	public List<BotInputEvent> m_InputEvent = new List<BotInputEvent>();

	private WayPoint m_LastStartWayPoint;

	private WayPoint m_LastTargetWaypoint;

	public List<PlayerBot.PathNodes> m_Path = new List<PlayerBot.PathNodes>();

	private bool m_testmode;

	private WayPoint m_WaypointTarget;

	private WayPoint m_CurrentWayPoint;

	private WayPoint m_PathCloser;

	private int m_CurrentNode;

	private float m_NodeDuration;

	public PathBuilder(PathBuildingScreen PathScreen)
	{
		m_PathScreen = PathScreen;
	}

	public void Update(GameTime time)
	{
		if (m_testmode)
		{
			if (m_Path.Count <= 0)
			{
				return;
			}
			m_CurrentWayPoint = m_Path[m_Path.Count - 1].wp;
			if (m_WaypointTarget == null)
			{
				Vector2 position = m_PathScreen.m_Players[0].GetPosition();
				Vector2 position2 = m_CurrentWayPoint.m_Position;
				if (m_CurrentWayPoint.IsWayPointReached(m_PathScreen.m_Players[0]))
				{
					if (m_Path.Count >= 2)
					{
						m_WaypointTarget = m_Path[m_Path.Count - 2].wp;
					}
				}
				else if (position.X > position2.X)
				{
					InputManager.Controller[0][1] = ButtonState.Pressed;
				}
				else
				{
					InputManager.Controller[0][3] = ButtonState.Pressed;
				}
			}
			else if (m_WaypointTarget.IsWayPointReached(m_PathScreen.m_Players[0]))
			{
				InputManager.Controller[0][1] = ButtonState.Pressed;
				InputManager.Controller[0][3] = ButtonState.Pressed;
				m_Path.RemoveAt(m_Path.Count - 1);
				if (m_Path.Count == 1)
				{
					m_CurrentNode = 0;
					m_testmode = false;
					InputManager.m_bAllowControl = true;
					return;
				}
				m_WaypointTarget = m_Path[m_Path.Count - 2].wp;
				if (m_Path[m_Path.Count - 1].index != -1)
				{
					m_CurrentNode = 0;
					m_NodeDuration = m_Path[m_Path.Count - 1].wp.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].duration;
				}
			}
			else if (m_NodeDuration > 0f)
			{
				m_NodeDuration -= time.ElapsedGameTime.Milliseconds;
				InputManager.Controller[0][1] = (m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bLeft ? ButtonState.Pressed : ButtonState.Released);
				InputManager.Controller[0][3] = (m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bRight ? ButtonState.Pressed : ButtonState.Released);
				InputManager.Controller[0][2] = (m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bDown ? ButtonState.Pressed : ButtonState.Released);
				InputManager.Controller[0][4] = (m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bJump ? ButtonState.Pressed : ButtonState.Released);
				InputManager.Controller[0][10] = (m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bRun ? ButtonState.Pressed : ButtonState.Released);
			}
			else if (m_CurrentNode + 1 < m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index].Count)
			{
				m_CurrentNode++;
				m_NodeDuration = m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].duration;
				m_PathCorrectionTime = 500f;
			}
			else if (m_PathCorrectionTime > 0f)
			{
				m_PathCorrectionTime -= time.ElapsedGameTime.Milliseconds;
				Vector2 position3 = m_PathScreen.m_Players[0].GetPosition();
				if (m_WaypointTarget.m_Position.X + 40f < position3.X)
				{
					InputManager.Controller[0][1] = ButtonState.Pressed;
					InputManager.Controller[0][3] = ButtonState.Released;
				}
				else if (m_WaypointTarget.m_Position.X - 40f > position3.X)
				{
					InputManager.Controller[0][1] = ButtonState.Released;
					InputManager.Controller[0][3] = ButtonState.Pressed;
				}
			}
			else
			{
				m_PathCorrectionTime = 500f;
				m_LastStartWayPoint = m_CurrentWayPoint;
				m_LastTargetWaypoint = m_WaypointTarget;
				m_Path.Clear();
				m_CurrentNode = 0;
				m_NodeDuration = 0f;
				m_testmode = false;
				InputManager.m_bAllowControl = true;
			}
		}
		else
		{
			if (m_PathScreen.m_Players[0].m_Tag != 0)
			{
				return;
			}
			if (m_CurrentWayPointRecord == null)
			{
				if (NewWayPointReach())
				{
					m_InputEvent.Add(new BotInputEvent(Key(1), Key(3), Key(2), Key(4), Key(10)));
				}
			}
			else if (NewWayPointReach())
			{
				float num = 0f;
				for (int i = 0; i < m_InputEvent.Count; i++)
				{
					num += m_InputEvent[i].duration;
				}
				if (num < m_LastWayPointRecord.GetRoadTime(m_CurrentWayPointRecord) || (m_LastStartWayPoint == m_LastWayPointRecord && m_CurrentWayPointRecord == m_LastTargetWaypoint))
				{
					m_LastWayPointRecord.AddNeightBour(m_CurrentWayPointRecord, m_InputEvent);
				}
				PlayerBot.PathNodes item = default(PlayerBot.PathNodes);
				item.wp = m_LastWayPointRecord;
				item.index = m_LastWayPointRecord.GetPathIndex(m_CurrentWayPointRecord);
				m_Path.Add(item);
				m_PathCloser = m_CurrentWayPointRecord;
				m_CurrentWayPointRecord = null;
				m_LastWayPointRecord = null;
				m_InputEvent.Clear();
			}
			else
			{
				int index = m_InputEvent.Count - 1;
				if (Key(1) != m_InputEvent[index].bLeft || Key(3) != m_InputEvent[index].bRight || Key(2) != m_InputEvent[index].bDown || Key(4) != m_InputEvent[index].bJump || Key(10) != m_InputEvent[index].bRun)
				{
					m_InputEvent.Add(new BotInputEvent(Key(1), Key(3), Key(2), Key(4), Key(10)));
				}
				else
				{
					m_InputEvent[index].duration += time.ElapsedGameTime.Milliseconds;
				}
			}
			RecordInput();
		}
	}

	private bool NewWayPointReach()
	{
		foreach (WayPoint wayPoint in m_PathScreen.m_Level.m_WayPoints)
		{
			if (wayPoint.IsWayPointReached(m_PathScreen.m_Players[0]) && wayPoint != m_CurrentWayPointRecord)
			{
				m_LastWayPointRecord = m_CurrentWayPointRecord;
				m_CurrentWayPointRecord = wayPoint;
				return true;
			}
		}
		return false;
	}

	private bool Key(int entry)
	{
		return InputManager.GetKeyState(PlayerIndex.One, entry) == ButtonState.Pressed;
	}

	private void RecordInput()
	{
		if (InputManager.GetKeyState(PlayerIndex.One, 9) == ButtonState.Pressed && m_Path.Count > 0)
		{
			m_testmode = true;
			InputManager.m_bAllowControl = false;
			m_PathScreen.m_Players[0].SetPosition(m_Path[0].wp.m_Position - new Vector2(0f, m_PathScreen.m_Players[0].m_Height / 2));
			m_WaypointTarget = null;
			m_CurrentWayPoint = null;
			m_CurrentNode = 0;
			m_CurrentWayPointRecord = null;
			m_LastWayPointRecord = null;
			m_InputEvent.Clear();
			m_LastStartWayPoint = null;
			m_LastTargetWaypoint = null;
			PlayerBot.PathNodes item = default(PlayerBot.PathNodes);
			item.wp = m_PathCloser;
			item.index = -1;
			m_Path.Add(item);
			m_Path.Reverse();
			InputManager.Controller[0][1] = ButtonState.Released;
			InputManager.Controller[0][3] = ButtonState.Released;
			InputManager.Controller[0][2] = ButtonState.Released;
			InputManager.Controller[0][4] = ButtonState.Released;
			InputManager.Controller[0][10] = ButtonState.Released;
		}
		else
		{
			if (InputManager.GetKeyState(PlayerIndex.One, 8) != ButtonState.Pressed)
			{
				return;
			}
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "\t";
			xmlWriterSettings.Encoding = Encoding.UTF8;
			FileStream fileStream = new FileStream(m_PathScreen.content.RootDirectory + "\\PathLink\\" + GameContext.SelectedLevel + "_PathLink.xml", FileMode.Create);
			XmlWriter xmlWriter = XmlWriter.Create(fileStream, xmlWriterSettings);
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("XnaContent");
			foreach (WayPoint wayPoint in m_PathScreen.m_Level.m_WayPoints)
			{
				xmlWriter.WriteStartElement("WayPoint-" + wayPoint.m_nId.ToString(CultureInfo.InvariantCulture));
				for (int i = 0; i < wayPoint.m_NeightBour.Count; i++)
				{
					xmlWriter.WriteStartElement("NeightBor-" + wayPoint.m_NeightBour[i].m_nId);
					for (int j = 0; j < wayPoint.m_NeightBourRoadMap[i].Count; j++)
					{
						xmlWriter.WriteStartElement("InputEvent" + j);
						xmlWriter.WriteAttributeString("Duration", wayPoint.m_NeightBourRoadMap[i][j].duration.ToString(CultureInfo.InvariantCulture));
						xmlWriter.WriteAttributeString("Left", wayPoint.m_NeightBourRoadMap[i][j].bLeft.ToString());
						xmlWriter.WriteAttributeString("Right", wayPoint.m_NeightBourRoadMap[i][j].bRight.ToString());
						xmlWriter.WriteAttributeString("Down", wayPoint.m_NeightBourRoadMap[i][j].bDown.ToString());
						xmlWriter.WriteAttributeString("Jump", wayPoint.m_NeightBourRoadMap[i][j].bJump.ToString());
						xmlWriter.WriteAttributeString("Run", wayPoint.m_NeightBourRoadMap[i][j].bRun.ToString());
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
			fileStream.Close();
		}
	}

	public void Draw()
	{
	}
}
