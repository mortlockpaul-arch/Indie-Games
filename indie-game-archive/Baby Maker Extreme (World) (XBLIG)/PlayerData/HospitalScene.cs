using System;
using Microsoft.Xna.Framework;
using Physics;
using Renderer;
using Scene;

namespace PlayerData;

public class HospitalScene
{
	private const int FLOOR_WIDTH = 3000;

	private Player m_baby;

	private PhysicalRepresentation m_obj;

	private PhysicalRepresentation m_objRoof;

	private PhysicalRepresentation m_obj2;

	private PhysicalRepresentation m_objRoof2;

	private RoomManager m_roomManager;

	public HospitalScene()
	{
		m_obj = PhysicsObjectManager.CreatePhysicalRepresentation(new Rectangle(0, 668, 3000, 100), PhysicsObjectManager.WallCollisionGroup());
		m_obj.Static = true;
		m_obj.FrictionCoeff = 0.5f;
		m_obj.Bounciness = 0.5f;
		m_obj.Mass = 1f;
		m_objRoof = PhysicsObjectManager.CreatePhysicalRepresentation(new Rectangle(0, -232, 3000, 100), PhysicsObjectManager.WallCollisionGroup());
		m_objRoof.Static = true;
		m_objRoof.FrictionCoeff = 0f;
		m_objRoof.Bounciness = 0.5f;
		m_objRoof.Mass = 1f;
		m_obj2 = PhysicsObjectManager.CreatePhysicalRepresentation(new Rectangle(3000, 668, 3000, 100), PhysicsObjectManager.WallCollisionGroup());
		m_obj2.Static = true;
		m_obj2.FrictionCoeff = 0.5f;
		m_obj2.Bounciness = 0.5f;
		m_obj2.Mass = 1f;
		m_objRoof2 = PhysicsObjectManager.CreatePhysicalRepresentation(new Rectangle(3000, -232, 3000, 100), PhysicsObjectManager.WallCollisionGroup());
		m_objRoof2.Static = true;
		m_objRoof2.FrictionCoeff = 0f;
		m_objRoof2.Bounciness = 0.5f;
		m_objRoof2.Mass = 1f;
		m_baby = new Player();
		m_roomManager = new RoomManager();
	}

	public PropPool GetPropPool()
	{
		return m_roomManager.GetPropPool();
	}

	public void Initialize()
	{
		m_baby.Initialize();
		m_roomManager.Initialize();
		m_obj.Position = new Vector2(1500f, m_obj.Position.Y);
		m_objRoof.Position = new Vector2(1500f, m_objRoof.Position.Y);
		m_obj2.Position = new Vector2(m_obj.Position.X + 3000f, m_obj.Position.Y);
		m_objRoof2.Position = new Vector2(m_objRoof.Position.X + 3000f, m_objRoof.Position.Y);
		m_obj.ResetSimulation();
		m_objRoof.ResetSimulation();
		m_obj2.ResetSimulation();
		m_objRoof2.ResetSimulation();
	}

	public void Update(TimeTracker gameTime, bool isActive)
	{
		Vector2 position = m_baby.Position;
		position = new Vector2(Math.Max(position.X + 200f, SceneRenderer.GetScreenDim().X / 2f), Math.Min(position.Y + 200f, SceneRenderer.GetScreenDim().Y / 2f));
		SceneRenderer.MoveCamera(position, 0f, 1f);
		m_baby.Update(gameTime, isActive);
		if (SceneRenderer.GetCameraPosition().X > m_obj2.Position.X)
		{
			PhysicalRepresentation obj = m_obj;
			PhysicalRepresentation objRoof = m_objRoof;
			Vector2 position2 = m_obj2.Position;
			Vector2 position3 = m_objRoof2.Position;
			m_obj = m_obj2;
			m_objRoof = m_objRoof2;
			m_obj.GetGeom().Body.ResetDynamics();
			m_objRoof.GetGeom().Body.ResetDynamics();
			m_obj.Position = position2;
			m_objRoof.Position = position3;
			m_obj2 = obj;
			m_obj2.GetGeom().Body.ResetDynamics();
			m_obj2.Position = m_obj.Position + new Vector2(3000f, 0f);
			m_objRoof2 = objRoof;
			m_objRoof2.GetGeom().Body.ResetDynamics();
			m_objRoof2.Position = m_objRoof.Position + new Vector2(3000f, 0f);
		}
		if (SceneRenderer.GetCameraPosition().X < m_obj.Position.X)
		{
			PhysicalRepresentation obj2 = m_obj;
			PhysicalRepresentation objRoof2 = m_objRoof;
			Vector2 position4 = m_obj.Position;
			Vector2 position5 = m_objRoof.Position;
			m_obj = m_obj2;
			m_obj.GetGeom().Body.ResetDynamics();
			m_obj.Position = obj2.Position - new Vector2(3000f, 0f);
			m_objRoof = m_objRoof2;
			m_objRoof.GetGeom().Body.ResetDynamics();
			m_objRoof.Position = objRoof2.Position - new Vector2(3000f, 0f);
			m_obj2 = obj2;
			m_objRoof2 = objRoof2;
			m_obj2.GetGeom().Body.ResetDynamics();
			m_objRoof2.GetGeom().Body.ResetDynamics();
			m_obj2.Position = position4;
			m_objRoof2.Position = position5;
		}
		m_roomManager.Update(gameTime, m_baby);
	}

	public void Draw(TimeTracker gameTime, bool activeScene)
	{
		if (activeScene)
		{
			m_baby.Draw(gameTime);
		}
		m_roomManager.Draw(gameTime);
	}

	public void HandleInput(TimeTracker gameTime)
	{
		m_baby.HandleInput(gameTime);
	}

	public void Launch(Vector2 angle, float power)
	{
		m_baby.Launch(angle, power);
	}

	public bool IsComplete()
	{
		return m_baby.IsStopped();
	}

	public Player GetPlayer()
	{
		return m_baby;
	}
}
