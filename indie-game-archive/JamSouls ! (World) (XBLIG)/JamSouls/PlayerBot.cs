using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class PlayerBot : Player
{
	public struct PathNodes(WayPoint w, int idx)
	{
		public WayPoint wp = w;

		public int index = idx;
	}

	public enum Behaviour
	{
		FRAG_PLAYER,
		GET_THE_FLAG,
		GET_POWER_UP,
		GET_SOUL,
		BEHAVIOUR_COUNT
	}

	public const int TARGET_REACH_RADIUS = 150;

	public const float FEAR_TRESHOLD = 200f;

	public const float TARGET_SEEKING_PERIOD = 1000f;

	public const float CORRECTION_STEP = 40f;

	public const float PATH_CORRECTION_TIME = 500f;

	public const float STEERING_UPDATE = 400f;

	public const float PATH_UPDATE_TIME = 1000f;

	public const float MAX_WAYPOINT_HIGHNESS = 200f;

	public const int GIVE_UP_MAX = 3;

	public const int EVADE_TIME = 1500;

	public const int CHANGE_STRATEGIE = 1500;

	public const int MIN_CHANGE_TIME = 300;

	public AITarget m_CurrentTarget;

	public List<PathNodes> m_Path = new List<PathNodes>();

	private WayPoint m_CurrentWayPoint;

	private WayPoint m_WaypointTarget;

	private float m_NodeDuration;

	private int m_CurrentNode;

	private float m_PathCorrectionTime;

	private float m_PathCorrectionUpdate;

	private float m_PathUpdate;

	public bool m_bCloseToTargetMode;

	private Behaviour m_CurrentBehavior = Behaviour.GET_POWER_UP;

	private int[] m_BehaviourWeight = new int[4];

	private bool m_bSeedPlanted;

	private bool m_bFlyTargetLock;

	private float m_DuelTimer;

	private float m_EvadeTimer;

	private int m_GiveUpCounter;

	public PlayerBot(GameState GameStateInstance, int CharIdx, PlayerIndex nIndex, string name, PlayerConfig.SBIRE_DEF sbiredef)
	{
		InitPlayer(GameStateInstance, CharIdx, nIndex, name, sbiredef);
		m_bIsPlayerBot = true;
		m_bToggleName = true;
		InputManager.SetLockPad((int)nIndex, block: true);
		if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
		{
			m_BehaviourWeight[1] = 0;
			switch (GameContext.DifficultyLevel)
			{
			case 0:
				m_BehaviourWeight[0] = 80;
				m_BehaviourWeight[2] = 10;
				m_BehaviourWeight[3] = 10;
				break;
			case 1:
				m_BehaviourWeight[0] = 50;
				m_BehaviourWeight[2] = 30;
				m_BehaviourWeight[3] = 20;
				break;
			case 2:
				m_BehaviourWeight[0] = 50;
				m_BehaviourWeight[2] = 50;
				m_BehaviourWeight[3] = 50;
				break;
			}
		}
		else if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM)
		{
			m_BehaviourWeight[0] = 0;
			m_BehaviourWeight[2] = 0;
			m_BehaviourWeight[3] = 0;
			m_BehaviourWeight[1] = 100;
		}
	}

	~PlayerBot()
	{
		InputManager.SetLockPad((int)m_PlayerNum, block: false);
	}

	public override bool ManageDeath()
	{
		bool flag = base.ManageDeath();
		if (flag)
		{
			ResetNavigation();
		}
		return flag;
	}

	public override void Update(GameTime gameTime)
	{
		if (m_PokeTime <= 0f)
		{
			if (m_CurrentTarget == null)
			{
				ResetNavigation();
				ChooseTarget();
			}
			else if (!TakeDecision())
			{
				if (!ReachGoal())
				{
					GoToTarget(gameTime.ElapsedGameTime.Milliseconds);
					m_DuelTimer = 0f;
				}
				else
				{
					m_DuelTimer -= gameTime.ElapsedGameTime.Milliseconds;
				}
			}
		}
		base.Update(gameTime);
	}

	public override void Draw()
	{
		base.Draw();
	}

	public override void ManageInput()
	{
		if (!m_bSpecialEnable)
		{
			if (InputManager.GetKeyState(m_PlayerNum, 10) == ButtonState.Pressed)
			{
				if (m_bIsMorphing)
				{
					SetWalkSpeed(6f, m_WalkAnimationSpeed / 2f);
				}
				else
				{
					SetWalkSpeed(40f, m_WalkAnimationSpeed / 2f);
				}
			}
			else if (m_Speed != 25f)
			{
				if (m_bIsMorphing)
				{
					SetWalkSpeed(4f, m_WalkAnimationSpeed);
				}
				else
				{
					SetWalkSpeed(25f, m_WalkAnimationSpeed);
				}
			}
		}
		if (InputManager.GetKeyState(m_PlayerNum, 1) == ButtonState.Pressed)
		{
			if (m_bIsOnGround)
			{
				SetAnimation(AnimStates.WALK);
			}
			m_PlayerBody.ApplyForce(new Vector2(0f - m_Speed, 0f));
			m_bLeftRelease = false;
		}
		else
		{
			m_bLeftRelease = true;
		}
		if (InputManager.GetKeyState(m_PlayerNum, 3) == ButtonState.Pressed)
		{
			if (m_bIsOnGround)
			{
				SetAnimation(AnimStates.WALK);
			}
			m_PlayerBody.ApplyForce(new Vector2(m_Speed, 0f));
			m_bRightRelease = false;
		}
		else
		{
			m_bRightRelease = true;
		}
		if (InputManager.GetKeyState(m_PlayerNum, 4) == ButtonState.Pressed)
		{
			if (!m_bLockJump)
			{
				ProcessJump();
				m_bJumpRelease = false;
			}
		}
		else
		{
			m_bJumpRelease = true;
		}
		if (m_bLeftRelease && m_bRightRelease && m_bIsOnGround)
		{
			SetAnimation(AnimStates.STAND);
		}
		if (InputManager.GetKeyState(m_PlayerNum, 2) == ButtonState.Pressed && !m_bDampingEnable)
		{
			if (m_bLeftRelease && m_bRightRelease && m_bIsOnGround)
			{
				SetAnimation(AnimStates.DUCK);
				m_bIsDucked = true;
			}
			else
			{
				m_bIsDucked = false;
			}
		}
		else
		{
			m_bIsDucked = false;
		}
	}

	private void ResetNavigation()
	{
		m_CurrentTarget = null;
		InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 4, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 5, pressed: false);
		m_bCloseToTargetMode = false;
		ChooseTarget();
	}

	private void AskNextWayPoint()
	{
		m_WaypointTarget = null;
		m_CurrentWayPoint = null;
		m_NodeDuration = 0f;
		m_CurrentNode = 0;
		m_PathUpdate = 0f;
		m_GameStateInstance.m_Level.m_OpenList.Add(FindClosestWayPoint(m_CurrentTarget.GetPosition()));
		InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 0, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 2, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 4, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 10, pressed: false);
		InputManager.SetKeyState(m_PlayerNum, 2, pressed: false);
		m_Path.Clear();
		m_GameStateInstance.m_Level.m_OpenList.Clear();
		m_GameStateInstance.m_Level.m_CloseList.Clear();
		m_GameStateInstance.m_Level.ClearParent();
		WayPoint wayPoint = FindClosestWayPoint(m_CurrentTarget.GetPosition());
		WayPoint wayPoint2 = FindClosestWayPoint(GetPosition());
		m_GameStateInstance.m_Level.m_OpenList.Add(wayPoint2);
		if (wayPoint2 == null || wayPoint == null)
		{
			return;
		}
		PathNodes parent = default(PathNodes);
		while (!m_GameStateInstance.m_Level.m_CloseList.Contains(wayPoint))
		{
			float num = float.PositiveInfinity;
			WayPoint wayPoint3 = null;
			foreach (WayPoint open in m_GameStateInstance.m_Level.m_OpenList)
			{
				float num2 = Vector2.Distance(open.m_Position, wayPoint2.m_Position) + Vector2.Distance(open.m_Position, wayPoint.m_Position);
				if (num2 < num)
				{
					num = num2;
					wayPoint3 = open;
				}
			}
			m_GameStateInstance.m_Level.m_OpenList.Remove(wayPoint3);
			m_GameStateInstance.m_Level.m_CloseList.Add(wayPoint3);
			for (int i = 0; i < wayPoint3.m_NeightBour.Count; i++)
			{
				WayPoint wayPoint4 = wayPoint3.m_NeightBour[i];
				if (m_GameStateInstance.m_Level.m_CloseList.Contains(wayPoint4))
				{
					continue;
				}
				parent.wp = wayPoint3;
				parent.index = i;
				wayPoint4.m_Parent = parent;
				if (!m_GameStateInstance.m_Level.m_OpenList.Contains(wayPoint4))
				{
					m_GameStateInstance.m_Level.m_OpenList.Add(wayPoint4);
					continue;
				}
				float num3 = Vector2.Distance(wayPoint2.m_Position, wayPoint4.m_Position);
				float num4 = Vector2.Distance(wayPoint2.m_Position, wayPoint3.m_Position) + Vector2.Distance(wayPoint3.m_Position, wayPoint4.m_Position);
				if (num4 < num3)
				{
					parent.wp = wayPoint3;
					parent.index = i;
					wayPoint4.m_Parent = parent;
				}
			}
		}
		PathNodes parent2 = default(PathNodes);
		parent2.wp = wayPoint;
		parent2.index = -1;
		while (parent2.wp != wayPoint2 && parent2.wp != null)
		{
			m_Path.Add(parent2);
			parent2 = parent2.wp.m_Parent;
		}
		if (parent2.wp != null)
		{
			m_Path.Add(parent2);
		}
		m_PathCorrectionTime = 500f;
	}

	private WayPoint FindClosestWayPoint(Vector2 Pos)
	{
		float num = float.PositiveInfinity;
		float num2 = 0f;
		int num3 = -1;
		for (int i = 0; i < m_GameStateInstance.m_Level.m_WayPoints.Count; i++)
		{
			if (Math.Abs(Pos.Y - m_GameStateInstance.m_Level.m_WayPoints[i].m_Position.Y) < 200f)
			{
				num2 = Vector2.Distance(Pos, m_GameStateInstance.m_Level.m_WayPoints[i].m_Position);
				if (num2 < num)
				{
					num = num2;
					num3 = i;
				}
			}
		}
		if (num3 != -1)
		{
			return m_GameStateInstance.m_Level.m_WayPoints[num3];
		}
		return null;
	}

	private void GoToTarget(float elasped)
	{
		if (m_Tag != 0 && m_Tag != 2)
		{
			return;
		}
		GetPosition();
		GetBottomRightPosition();
		if (!UpdatePowerUpBehaviour())
		{
			if (m_Path.Count > 0)
			{
				m_PathUpdate += elasped;
				m_CurrentWayPoint = m_Path[m_Path.Count - 1].wp;
				if (m_WaypointTarget == null)
				{
					if (m_CurrentWayPoint.IsWayPointReached(this))
					{
						if (m_Path.Count >= 2)
						{
							m_WaypointTarget = m_Path[m_Path.Count - 2].wp;
							m_PathCorrectionTime = 500f;
							m_NodeDuration = m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].duration;
						}
						else
						{
							AskNextWayPoint();
						}
					}
					else if (m_PathCorrectionTime > 0f)
					{
						SearchWaypoint(m_CurrentWayPoint);
						m_PathCorrectionTime -= elasped;
					}
					else
					{
						AskNextWayPoint();
					}
				}
				else if (m_WaypointTarget.IsWayPointReached(this))
				{
					if (m_PathUpdate >= 1000f)
					{
						AskNextWayPoint();
					}
					else
					{
						m_Path.RemoveAt(m_Path.Count - 1);
						if (m_Path.Count == 1)
						{
							m_CurrentNode = 0;
							AskNextWayPoint();
						}
						else
						{
							m_WaypointTarget = m_Path[m_Path.Count - 2].wp;
							if (m_Path[m_Path.Count - 1].index != -1)
							{
								m_CurrentNode = 0;
								m_NodeDuration = m_Path[m_Path.Count - 1].wp.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].duration;
							}
						}
						m_PathCorrectionTime = 500f;
					}
				}
				else if (m_NodeDuration > 0f)
				{
					m_NodeDuration -= elasped;
					InputManager.SetKeyState(m_PlayerNum, 1, m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bLeft);
					InputManager.SetKeyState(m_PlayerNum, 3, m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bRight);
					InputManager.SetKeyState(m_PlayerNum, 2, m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bDown);
					InputManager.SetKeyState(m_PlayerNum, 4, m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bJump);
					InputManager.SetKeyState(m_PlayerNum, 10, m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].bRun);
				}
				else if (m_CurrentNode + 1 < m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index].Count)
				{
					m_CurrentNode++;
					m_NodeDuration = m_CurrentWayPoint.m_NeightBourRoadMap[m_Path[m_Path.Count - 1].index][m_CurrentNode].duration;
				}
				else if (m_PathCorrectionTime > 0f)
				{
					SearchWaypoint(m_WaypointTarget);
					m_PathCorrectionTime -= elasped;
				}
				else
				{
					AskNextWayPoint();
				}
			}
			else if (m_bIsOnGround)
			{
				AskNextWayPoint();
			}
		}
		if (m_PathCorrectionUpdate > 0f)
		{
			m_PathCorrectionUpdate -= elasped;
		}
	}

	private bool IsCloseToTarget()
	{
		if (Vector2.Distance(GetPosition(), m_CurrentTarget.GetPosition()) < 150f)
		{
			return true;
		}
		return false;
	}

	private void SelectBehaviour()
	{
		int num = 4;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			if (m_BehaviourWeight[i] == 0)
			{
				array[i] = 0;
			}
			else
			{
				array[i] = m_Randomizer.Next(1, m_BehaviourWeight[i]);
			}
		}
		int num2 = 0;
		int currentBehavior = 0;
		for (int j = 0; j < num; j++)
		{
			if (array[j] > num2)
			{
				num2 = array[j];
				currentBehavior = j;
			}
		}
		m_CurrentBehavior = (Behaviour)currentBehavior;
	}

	private void ChooseTarget()
	{
		ResetPowerUpBehaviour();
		SelectBehaviour();
		InputManager.SetKeyState(m_PlayerNum, 5, pressed: false);
		switch (m_CurrentBehavior)
		{
		case Behaviour.FRAG_PLAYER:
		{
			AITarget potentialPlayerTarget2 = GetPotentialPlayerTarget();
			if (potentialPlayerTarget2 != null)
			{
				m_CurrentTarget = potentialPlayerTarget2;
			}
			else
			{
				m_CurrentBehavior = Behaviour.GET_POWER_UP;
			}
			break;
		}
		case Behaviour.GET_POWER_UP:
			if (m_CurrentPowerUp != null)
			{
				m_CurrentBehavior = Behaviour.FRAG_PLAYER;
			}
			else if (m_GameStateInstance.m_CurrentBonus != null)
			{
				m_CurrentTarget = new AITarget(m_GameStateInstance.m_CurrentBonus);
				m_CurrentBehavior = Behaviour.GET_POWER_UP;
			}
			else
			{
				m_CurrentBehavior = Behaviour.FRAG_PLAYER;
			}
			break;
		case Behaviour.GET_SOUL:
		{
			Soul availableSoul = m_GameStateInstance.m_SoulSpawner.GetAvailableSoul();
			if (availableSoul != null)
			{
				m_CurrentTarget = new AITarget(availableSoul);
			}
			else
			{
				m_CurrentBehavior = Behaviour.FRAG_PLAYER;
			}
			break;
		}
		case Behaviour.GET_THE_FLAG:
		{
			InputManager.SetKeyState(m_PlayerNum, 5, pressed: true);
			Flag flag;
			Flag flag2;
			if (m_Team == PlayerConfig.BLUE_TEAM_COLOR)
			{
				flag = m_GameStateInstance.m_BlueFlag;
				flag2 = m_GameStateInstance.m_RedFlag;
			}
			else
			{
				flag = m_GameStateInstance.m_RedFlag;
				flag2 = m_GameStateInstance.m_BlueFlag;
			}
			if (flag.m_Owner != null)
			{
				m_CurrentTarget = new AITarget(flag.m_Owner);
				break;
			}
			if (!flag.IsAtStartPosition())
			{
				m_CurrentTarget = new AITarget(flag);
				break;
			}
			if (flag2.m_Owner == null)
			{
				m_CurrentTarget = new AITarget(flag2);
				break;
			}
			if (flag2.m_Owner == this)
			{
				m_CurrentTarget = new AITarget(flag);
				break;
			}
			AITarget potentialPlayerTarget = GetPotentialPlayerTarget();
			if (potentialPlayerTarget != null)
			{
				m_CurrentTarget = potentialPlayerTarget;
			}
			break;
		}
		}
	}

	private AITarget GetPotentialPlayerTarget()
	{
		List<Player> list = new List<Player>();
		Player player = null;
		foreach (Player player2 in m_GameStateInstance.m_Players)
		{
			if (player2 != this && player2.GetTeam() != m_Team && player2.IsVisible() && !player2.m_bSpecialEnable)
			{
				list.Add(player2);
			}
		}
		if (list.Count > 0)
		{
			player = list[m_Randomizer.Next(0, list.Count)];
			return new AITarget(player);
		}
		return null;
	}

	private bool TakeDecision()
	{
		if (m_bSpecialEnable)
		{
			if ((object)m_CurrentTarget.TargetObject.GetType().BaseType != typeof(Player))
			{
				m_CurrentBehavior = Behaviour.FRAG_PLAYER;
				m_CurrentTarget = null;
				return true;
			}
			return false;
		}
		if (m_CurrentBehavior == Behaviour.GET_THE_FLAG)
		{
			Flag flag;
			Flag flag2;
			if (m_Team == PlayerConfig.BLUE_TEAM_COLOR)
			{
				flag = m_GameStateInstance.m_BlueFlag;
				flag2 = m_GameStateInstance.m_RedFlag;
			}
			else
			{
				flag = m_GameStateInstance.m_RedFlag;
				flag2 = m_GameStateInstance.m_BlueFlag;
			}
			if (flag.m_Owner != null)
			{
				m_CurrentTarget = new AITarget(flag.m_Owner);
			}
			else if (!flag.IsAtStartPosition())
			{
				if (flag.m_Owner != null)
				{
					m_CurrentTarget = new AITarget(flag.m_Owner);
				}
				else
				{
					m_CurrentTarget = new AITarget(flag);
				}
			}
			else if (flag2.m_Owner == null)
			{
				Player player = WatchBack();
				if (player == null)
				{
					m_CurrentTarget = new AITarget(flag2);
				}
				else
				{
					m_CurrentTarget = new AITarget(player);
				}
			}
			else if (flag2.m_Owner == this)
			{
				Player player2 = WatchBack();
				if (player2 == null)
				{
					m_CurrentTarget = new AITarget(flag);
				}
				else
				{
					m_CurrentTarget = new AITarget(player2);
				}
			}
			else
			{
				AITarget potentialPlayerTarget = GetPotentialPlayerTarget();
				if (potentialPlayerTarget != null)
				{
					m_CurrentTarget = potentialPlayerTarget;
				}
			}
		}
		else if (m_CurrentBehavior != Behaviour.GET_POWER_UP && GameContext.DifficultyLevel > 0)
		{
			CheckPowerUp();
		}
		return false;
	}

	private bool ReachGoal()
	{
		m_bCloseToTargetMode = IsCloseToTarget();
		if (m_CurrentTarget.TargetObject != null)
		{
			if ((object)m_CurrentTarget.TargetObject.GetType().BaseType == typeof(Player))
			{
				Player player = (Player)m_CurrentTarget.TargetObject;
				if (!player.IsVisible() || player.m_bSpecialEnable || player.m_Tag == 1)
				{
					m_GiveUpCounter = 0;
					m_CurrentTarget = null;
					return true;
				}
				if (m_bCloseToTargetMode)
				{
					UpdatePowerUpBehaviour();
					SearchAndDestroy((Target)m_CurrentTarget.TargetObject);
					return true;
				}
			}
			if ((object)m_CurrentTarget.TargetObject.GetType() == typeof(Flag))
			{
				if (m_bCloseToTargetMode)
				{
					SearchAndDestroy((Target)m_CurrentTarget.TargetObject);
					return true;
				}
				m_GiveUpCounter = 0;
			}
			if ((object)m_CurrentTarget.TargetObject.GetType() == typeof(Soul))
			{
				Soul soul = (Soul)m_CurrentTarget.TargetObject;
				if (soul.GetOwner() != null || !soul.m_bSpawned)
				{
					m_GiveUpCounter = 0;
					m_CurrentTarget = null;
					return true;
				}
				if (m_bCloseToTargetMode)
				{
					SearchAndDestroy((Target)m_CurrentTarget.TargetObject);
					return true;
				}
			}
			if ((object)m_CurrentTarget.TargetObject.GetType().BaseType == typeof(PowerUp))
			{
				InputManager.SetKeyState(m_PlayerNum, 5, pressed: true);
				PowerUp powerUp = (PowerUp)m_CurrentTarget.TargetObject;
				if (powerUp.IsAvailable() || powerUp.HasOwner())
				{
					m_GiveUpCounter = 0;
					m_CurrentTarget = null;
					return true;
				}
				if (m_bCloseToTargetMode)
				{
					SearchAndDestroy((Target)m_CurrentTarget.TargetObject);
					return true;
				}
			}
			return false;
		}
		m_CurrentTarget = null;
		return true;
	}

	private void SearchWaypoint(WayPoint w)
	{
		Vector2 position = GetPosition();
		if (m_PathCorrectionUpdate <= 0f)
		{
			if (Vector2.Distance(position, w.m_Position) > 40f)
			{
				if (position.X > w.m_Position.X)
				{
					InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
					InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
				}
				else
				{
					InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
					InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
				}
				m_PathCorrectionUpdate = 400f;
			}
			else
			{
				InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
				InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
			}
		}
		if (position.Y > w.m_Position.Y)
		{
			InputManager.SetKeyState(m_PlayerNum, 4, pressed: true);
		}
		else
		{
			InputManager.SetKeyState(m_PlayerNum, 4, pressed: false);
		}
	}

	public void SearchAndDestroy(Target entity)
	{
		if (!(m_DuelTimer <= 0f))
		{
			return;
		}
		Vector2 posBotTL = GetTopLeftPosition();
		Vector2 bottomRightPosition = GetBottomRightPosition();
		Vector2 position = entity.GetPosition();
		Vector2 topLeftPosition = entity.GetTopLeftPosition();
		bool flag = bottomRightPosition.Y >= topLeftPosition.Y;
		bool flag2 = posBotTL.Y >= topLeftPosition.Y;
		int num = -1;
		if (GameContext.DifficultyLevel > 0)
		{
			num = DetectPredator(ref posBotTL);
		}
		if (bottomRightPosition.X < position.X)
		{
			InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
		}
		else
		{
			InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
		}
		if (posBotTL.X > position.X)
		{
			InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
		}
		else
		{
			InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
		}
		if (num != -1)
		{
			if (m_WallOnTheLeft)
			{
				InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
				InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
				m_WallOnTheLeft = false;
			}
			else if (m_WallOnTheRIght)
			{
				InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
				InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
				m_WallOnTheRIght = false;
			}
			else if (bottomRightPosition.X < m_GameStateInstance.m_Players[num].GetPosition().X)
			{
				InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
				InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
			}
			else
			{
				InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
				InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
			}
			m_DuelTimer = m_Randomizer.Next(300, 1500);
			return;
		}
		if (flag2 || (flag && (object)entity.GetType().BaseType == typeof(Player)))
		{
			InputManager.SetKeyState(m_PlayerNum, 4, pressed: true);
			m_bJumpRelease = true;
		}
		if (!(m_DuelTimer <= 0f - m_EvadeTimer))
		{
			return;
		}
		m_DuelTimer = m_Randomizer.Next(300, 1500);
		m_EvadeTimer = m_Randomizer.Next(300, 1500);
		if (m_WallOnTheLeft)
		{
			InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
			InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
			m_WallOnTheLeft = false;
			return;
		}
		if (m_WallOnTheRIght)
		{
			InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
			InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
			m_WallOnTheRIght = false;
			return;
		}
		int key = m_Randomizer.Next(1, 4);
		InputManager.SetKeyState(m_PlayerNum, key, pressed: true);
		m_GiveUpCounter++;
		if (m_GiveUpCounter >= 3)
		{
			ResetNavigation();
		}
	}

	private int DetectPredator(ref Vector2 posBotTL)
	{
		bool flag = false;
		bool flag2 = false;
		int result = -1;
		for (int i = 0; i < m_GameStateInstance.m_Players.Count; i++)
		{
			Vector2 bottomRightPosition = m_GameStateInstance.m_Players[i].GetBottomRightPosition();
			Vector2 topLeftPosition = m_GameStateInstance.m_Players[i].GetTopLeftPosition();
			if (m_GameStateInstance.m_Players[i] != this)
			{
				if (posBotTL.Y > bottomRightPosition.Y)
				{
					flag = true;
				}
				if (!(posBotTL.X > bottomRightPosition.X) || !(bottomRightPosition.X < topLeftPosition.X))
				{
					flag2 = true;
				}
				if (flag && flag2 && Vector2.Distance(GetPosition(), m_GameStateInstance.m_Players[i].GetPosition()) < 200f)
				{
					result = i;
					break;
				}
				flag = false;
				flag2 = false;
			}
		}
		return result;
	}

	private Player WatchBack()
	{
		Vector2 position = GetPosition();
		for (int i = 0; i < m_GameStateInstance.m_Players.Count; i++)
		{
			Player player = m_GameStateInstance.m_Players[i];
			if (player != this && player.IsVisible() && !player.m_bSpecialEnable && player.GetTeam() != m_Team && Vector2.Distance(player.GetPosition(), position) < 200f)
			{
				return m_GameStateInstance.m_Players[i];
			}
		}
		return null;
	}

	private void CheckPowerUp()
	{
		if (m_CurrentPowerUp == null && m_GameStateInstance.m_CurrentBonus != null && m_GameStateInstance.m_CurrentBonus.m_Player == null && Vector2.Distance(GetPosition(), m_GameStateInstance.m_CurrentBonus.m_Position) < m_GameStateInstance.m_CurrentBonus.m_GrabRadius)
		{
			InputManager.SetKeyState(m_PlayerNum, 5, pressed: true);
		}
	}

	private void SearchPowerUp(Vector2 TargetPos)
	{
		InputManager.SetKeyState(m_PlayerNum, 5, pressed: true);
		Vector2 topPosition = GetTopPosition();
		if (topPosition.X < TargetPos.X)
		{
			InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
			InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
		}
		else if (topPosition.X > TargetPos.X)
		{
			InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
			InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
		}
		if (topPosition.Y - 5f > TargetPos.Y)
		{
			InputManager.SetKeyState(m_PlayerNum, 4, pressed: true);
		}
	}

	protected bool UpdatePowerUpBehaviour()
	{
		bool flag = (object)m_CurrentTarget.TargetObject.GetType().BaseType == typeof(Player);
		if (m_CurrentPowerUp != null && m_CurrentTarget != null)
		{
			Vector2 position = m_CurrentTarget.GetPosition();
			Vector2 position2 = GetPosition();
			Type type = m_CurrentPowerUp.GetType();
			if ((object)type == typeof(Skull) || ((object)type == typeof(FireProut) && flag))
			{
				if (m_bCloseToTargetMode)
				{
					InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
				}
				else
				{
					InputManager.SetKeyState(m_PlayerNum, 6, pressed: false);
				}
			}
			else
			{
				if ((object)type == typeof(Fly) && (flag || (!flag && m_CurrentBehavior == Behaviour.GET_THE_FLAG)))
				{
					if (!m_bFlyTargetLock)
					{
						InputManager.SetKeyState(m_PlayerNum, 4, pressed: true);
						InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
					}
					else
					{
						m_bFlyTargetLock = !m_bIsOnGround;
					}
					if (!m_bIsOnGround && m_bControlHorizontalMove)
					{
						if (Math.Abs(position.X - position2.X) < 20f)
						{
							InputManager.SetKeyState(m_PlayerNum, 6, pressed: false);
							m_bControlHorizontalMove = false;
							m_bFlyTargetLock = true;
						}
						else if (position.Y - position2.Y > -40f && InputManager.GetKeyState(m_PlayerNum, 6) == ButtonState.Pressed && InputManager.GetKeyState(m_PlayerNum, 6) == ButtonState.Pressed)
						{
							if (position.X < position2.X - 60f)
							{
								InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
								InputManager.SetKeyState(m_PlayerNum, 1, pressed: true);
							}
							else if (position.X > position2.X + 60f)
							{
								InputManager.SetKeyState(m_PlayerNum, 3, pressed: true);
								InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
							}
						}
						return true;
					}
					m_bControlHorizontalMove = true;
					if (!m_bIsOnGround)
					{
						m_bFlyTargetLock = false;
					}
					InputManager.SetKeyState(m_PlayerNum, 6, pressed: false);
					return true;
				}
				if ((object)type == typeof(Cloud))
				{
					InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
				}
				else if ((object)type == typeof(Soldier) && flag)
				{
					if (position.Y - position2.Y > -50f)
					{
						InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
					}
					else
					{
						InputManager.SetKeyState(m_PlayerNum, 6, pressed: false);
					}
				}
				else if ((object)type == typeof(Seed) && flag)
				{
					if (!m_bSeedPlanted)
					{
						InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
						m_bSeedPlanted = true;
					}
					else
					{
						InputManager.SetKeyState(m_PlayerNum, 6, pressed: false);
						foreach (Player player in m_GameStateInstance.m_Players)
						{
							if (player != this && !player.m_bSpecialEnable && player.GetTeam() != GetTeam() && player.IsVisible() && Vector2.Distance(player.GetPosition(), m_CurrentPowerUp.GetNodePosition()) < 80f)
							{
								InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
								m_bSeedPlanted = false;
								break;
							}
						}
					}
				}
				else if ((object)type == typeof(BlackSugar) && flag)
				{
					if (position2.X < position.X)
					{
						m_SpriteEffect = SpriteEffects.None;
					}
					else
					{
						m_SpriteEffect = SpriteEffects.FlipHorizontally;
					}
					if (position2.Y < position.Y)
					{
						InputManager.SetKeyState(m_PlayerNum, 4, pressed: true);
					}
					InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
					InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
					InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
				}
				else if ((object)type == typeof(Vile))
				{
					if (flag)
					{
						if (position2.X < position.X)
						{
							m_SpriteEffect = SpriteEffects.None;
						}
						else
						{
							m_SpriteEffect = SpriteEffects.FlipHorizontally;
						}
						if (position2.Y < position.Y)
						{
							InputManager.SetKeyState(m_PlayerNum, 4, pressed: true);
						}
						InputManager.SetKeyState(m_PlayerNum, 6, pressed: true);
					}
					InputManager.SetKeyState(m_PlayerNum, 3, pressed: false);
					InputManager.SetKeyState(m_PlayerNum, 1, pressed: false);
				}
			}
			return false;
		}
		ResetPowerUpBehaviour();
		return false;
	}

	private void ResetPowerUpBehaviour()
	{
		if (!m_bSpecialEnable)
		{
			InputManager.SetKeyState(m_PlayerNum, 6, pressed: false);
			m_bSeedPlanted = false;
			m_bControlHorizontalMove = true;
		}
	}
}
