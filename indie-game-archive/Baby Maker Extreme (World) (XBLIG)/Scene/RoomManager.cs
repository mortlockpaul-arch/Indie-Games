using System.Collections.Generic;
using BabyMaker;
using Microsoft.Xna.Framework;
using PlayerData;
using Renderer;

namespace Scene;

public class RoomManager
{
	private const int TRIAL_ROOMS = 15;

	private List<Room> m_rooms;

	private int m_iNextRoomIndex;

	private int m_iLastRoomIndex;

	private PropPool m_propPool;

	private RoomType m_lastRoomType;

	private int m_iRoomsCreated;

	public RoomManager()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (m_propPool != null)
		{
			for (int i = 0; i < m_rooms.Count; i++)
			{
				m_rooms[i].CleanRoom(m_propPool);
			}
		}
		else
		{
			m_rooms = new List<Room>(5);
			for (int j = 0; j < 5; j++)
			{
				m_rooms.Add(new Room());
			}
		}
		if (m_propPool == null)
		{
			m_propPool = new PropPool();
		}
		else
		{
			m_propPool.ResetProps();
		}
		m_iNextRoomIndex = 0;
		m_iLastRoomIndex = -1;
		m_lastRoomType = RoomType.BIRTHROOM;
		m_iRoomsCreated = 0;
	}

	public PropPool GetPropPool()
	{
		return m_propPool;
	}

	public void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i < m_rooms.Count; i++)
		{
			m_rooms[i].Draw(gameTime);
		}
	}

	public void Update(TimeTracker gameTime, Player p)
	{
		if (m_iLastRoomIndex < 0 || (float)m_rooms[m_iLastRoomIndex].RoomArea.Right < p.Position.X + 1300f)
		{
			GetRandomRoom();
		}
		for (int i = 0; i < m_rooms.Count; i++)
		{
			m_rooms[i].Update(gameTime);
		}
		m_propPool.UpdateEnabled();
	}

	private Room GetRandomRoom()
	{
		m_iRoomsCreated++;
		Rectangle r = new Rectangle(0, 0, 1300, 768);
		RoomType roomType;
		for (roomType = (RoomType)SceneRenderer.GetRand(1f, 10f); roomType == m_lastRoomType; roomType = (RoomType)SceneRenderer.GetRand(1f, 10f))
		{
		}
		if (m_iLastRoomIndex == -1)
		{
			roomType = RoomType.BIRTHROOM;
		}
		switch (roomType)
		{
		case RoomType.BIRTHROOM:
			r.Width = 1400;
			break;
		case RoomType.CAFETERIA:
			r.Width = (int)SceneRenderer.GetRand(2000f, 4000f);
			break;
		case RoomType.HALLWAY:
			r.Width = (int)SceneRenderer.GetRand(1500f, 2500f);
			break;
		case RoomType.WAITINGROOM:
			r.Width = (int)SceneRenderer.GetRand(1300f, 3000f);
			break;
		case RoomType.BEDSROOM:
			r.Width = (int)SceneRenderer.GetRand(1300f, 3000f);
			break;
		case RoomType.PHYSIO:
			r.Width = 2000;
			break;
		case RoomType.LAB:
			r.Width = (int)SceneRenderer.GetRand(2000f, 4000f);
			break;
		case RoomType.SURGERYTHEATRE:
			r.Width = 2750;
			break;
		case RoomType.DIAGNOSIS:
			r.Width = 1200;
			break;
		case RoomType.MORTUARY:
			r.Width = (int)SceneRenderer.GetRand(2000f, 4000f);
			break;
		}
		if (m_iLastRoomIndex >= 0)
		{
			r.X = m_rooms[m_iLastRoomIndex].RoomArea.Right;
		}
		Room room = m_rooms[m_iNextRoomIndex];
		room.CleanRoom(m_propPool);
		room.Initialize(roomType, r, m_propPool);
		if (m_iRoomsCreated == 15 && Game1.IsTrial())
		{
			room.InitializeWall(m_propPool);
		}
		m_iLastRoomIndex = m_iNextRoomIndex;
		m_iNextRoomIndex++;
		if (m_iNextRoomIndex >= m_rooms.Count)
		{
			m_iNextRoomIndex = 0;
		}
		m_lastRoomType = roomType;
		return room;
	}
}
