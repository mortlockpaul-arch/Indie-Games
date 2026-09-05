using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;

namespace PlayObjects;

public class Player
{
	public const int SCORE_ITER = 30;

	public const float DIST_DIV = 75f;

	private const int NUM_SAVE_POS = 180;

	private const float STOP_DIST = 70f;

	private const float STOP_SPEED = 900f;

	public const int ROOF_POS = -550;

	public const int FLOOR_POS = 250;

	private Prop m_outfit;

	private bool m_bRevertData;

	private List<PhysicalRepresentation> m_objs;

	private List<Vector2> m_vSavedVel;

	private int m_iCollisionScore;

	private StringBuilder m_MutatedDistanceString;

	private char[] m_distanceCharArray;

	private StringBuilder m_MutatedScoreString;

	private char[] m_scoreCharArray;

	private int m_iLastDistance;

	private int m_iLastScore;

	private string m_DistanceString;

	private string m_ScoreString;

	private List<SpriteInstance> m_sprites;

	private List<ScoreParticle> m_scoreParticles;

	private Vector2 m_scorePos;

	private List<Vector2> m_positionsSaved;

	private List<OutfitPiece> m_outfitPieces;

	private Vector2 m_vAverageVel;

	private List<AwardPopup> m_popups;

	private bool m_bLaunched;

	private List<int> m_scoreCounters;

	private float m_ceilHeight;

	private List<Prop>[] m_SavedBabyTypes;

	private int m_iSavedTypeIndex;

	private PropType m_BabyPropType;

	private Vector2 m_vPlayerCamPos;

	private float m_fPlayerCamZoom;

	private bool m_bCamDoesScale;

	private PlayerController m_playerController;

	private List<PlayerController> m_controllers;

	public PropType BabyType => m_BabyPropType;

	public Vector2 Position
	{
		get
		{
			return m_outfit.GetOutfit().GetPhysicsObjects()[0].Position;
		}
		set
		{
			m_outfit.GetOutfit().GetPhysicsObjects()[0].Position = value;
		}
	}

	public int DistanceTravelled => (int)(m_sprites[0].Position.X / 75f);

	public float CeilHeight
	{
		set
		{
			m_ceilHeight = value;
		}
	}

	public bool CamScale
	{
		get
		{
			return m_bCamDoesScale;
		}
		set
		{
			m_bCamDoesScale = value;
		}
	}

	public Player()
	{
		m_bCamDoesScale = false;
		m_iSavedTypeIndex = 0;
		m_SavedBabyTypes = new List<Prop>[2];
		m_SavedBabyTypes[0] = new List<Prop>();
		m_SavedBabyTypes[1] = new List<Prop>();
		for (int i = 0; i <= 7; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				m_SavedBabyTypes[j].Add(new Prop((PropType)i));
				m_SavedBabyTypes[j].Last().SetDepth(30f);
				m_SavedBabyTypes[j].Last().ResetToLocation(new Vector2(-2000f, 0f));
				m_SavedBabyTypes[j].Last().GetOutfit().Disable();
				if (i == 1)
				{
					List<SpriteInstance> sprites = m_SavedBabyTypes[j].Last().GetOutfit().GetSprites();
					for (int k = 0; k < sprites.Count; k++)
					{
						sprites[k].Alpha = 0f;
					}
				}
			}
		}
		m_ceilHeight = -1850f;
		m_vAverageVel = default(Vector2);
		m_outfit = null;
		SetBody(PropType.BABY);
		m_bRevertData = false;
		m_DistanceString = " feet";
		m_ScoreString = " points";
		m_iCollisionScore = 0;
		m_iLastDistance = 0;
		m_iLastScore = 0;
		m_MutatedDistanceString = new StringBuilder("0", 32);
		m_distanceCharArray = new char[32];
		m_distanceCharArray[0] = '\0';
		m_MutatedScoreString = new StringBuilder("0", 32);
		m_scoreCharArray = new char[32];
		m_scoreCharArray[0] = '\0';
		m_scoreParticles = new List<ScoreParticle>();
		for (int l = 0; l < 40; l++)
		{
			m_scoreParticles.Add(new ScoreParticle());
		}
		m_scorePos = SceneRenderer.GetScreenDim() * 0.3f - new Vector2(150f, 40f);
		m_positionsSaved = new List<Vector2>(180);
		m_outfitPieces = new List<OutfitPiece>();
		m_popups = new List<AwardPopup>();
		SetupPyroTechnics();
		m_scoreCounters = new List<int>();
		for (int m = 0; m < 171; m++)
		{
			m_scoreCounters.Add(0);
		}
		m_bLaunched = false;
		Vector2 position = m_outfit.GetOutfit().GetPhysicsObjects()[0].Position;
		m_vPlayerCamPos = new Vector2(Math.Max(700f, position.X - SceneRenderer.GetScreenDim().X / 2f + 800f), Math.Min(100f, Math.Max(-175f, position.Y + 350f)));
		m_fPlayerCamZoom = 1f;
		m_controllers = new List<PlayerController>();
		m_controllers.Add(new PlayerDefaultController(this));
		m_controllers.Add(new PlayerDefaultController(this));
		m_controllers.Add(new PlayerDefaultController(this));
		m_controllers.Add(new DartLaunchController(this));
		m_controllers.Add(new TripleBooster(this));
		m_controllers.Add(new PlayerDefaultController(this));
		m_controllers.Add(new FairyWingController(this));
		m_controllers.Add(new DirectionJetController(this));
		SetBody(PropType.BABY);
	}

	public void ClearOutfit()
	{
		m_outfitPieces.Clear();
	}

	public List<OutfitPiece> GetOutfitPieces()
	{
		return m_outfitPieces;
	}

	public void SwapOutfits()
	{
		_ = Position;
		List<PhysicalRepresentation> physicsObjects;
		if (m_outfit != null)
		{
			m_outfit.GetOutfit().CollisionCategory = PhysicsObjectManager.WallCollisionGroup();
			physicsObjects = m_outfit.GetOutfit().GetPhysicsObjects();
			Prop prop = m_SavedBabyTypes[1 - m_iSavedTypeIndex][(int)m_BabyPropType];
			List<PhysicalRepresentation> physicsObjects2 = prop.GetOutfit().GetPhysicsObjects();
			List<SpriteInstance> sprites = prop.GetOutfit().GetSprites();
			for (int i = 0; i < physicsObjects.Count; i++)
			{
				Body geom = physicsObjects[i].GetGeom();
				Body geom2 = physicsObjects2[i].GetGeom();
				geom2.Position = geom.Position;
				geom2.Rotation = geom.Rotation;
				geom2.AngularVelocity = geom.AngularVelocity;
				geom2.Inertia = geom.Inertia;
				geom2.LinearDamping = geom.LinearDamping;
				geom2.LinearVelocity = geom.LinearVelocity;
				sprites[i].Position = m_sprites[i].Position;
				sprites[i].Rotation = m_sprites[i].Rotation;
				PhysicsObjectManager.RemovePlayerGeom(physicsObjects[i]);
			}
			m_outfit.ResetToLocation(new Vector2(-2000f, 170f));
			m_outfit.GetOutfit().Disable();
		}
		m_iSavedTypeIndex = 1 - m_iSavedTypeIndex;
		m_outfit = m_SavedBabyTypes[m_iSavedTypeIndex][(int)m_BabyPropType];
		PhysicsOutfit outfit = m_outfit.GetOutfit();
		m_outfit.UpdateEnabled();
		m_outfit.GetOutfit().DisableStatic();
		physicsObjects = outfit.GetPhysicsObjects();
		for (int j = 0; j < physicsObjects.Count; j++)
		{
			PhysicsObjectManager.AddPlayerGeom(physicsObjects[j], this);
		}
		m_outfit.GetOutfit().CollisionCategory = PhysicsObjectManager.PlayerCollisionGroup();
		m_objs = physicsObjects;
		m_sprites = m_outfit.GetOutfit().GetSprites();
		for (int k = 0; k < m_outfitPieces.Count; k++)
		{
			m_outfitPieces[k].AttachedTo = m_outfit.GetOutfit().GetSprites()[m_outfitPieces[k].Slot];
		}
		m_playerController.SwapOutfit();
	}

	public void SetBody(PropType bodyType)
	{
		if (m_playerController != null)
		{
			m_playerController.Reset();
		}
		m_BabyPropType = bodyType;
		List<PhysicalRepresentation> physicsObjects;
		if (m_outfit != null)
		{
			m_outfit.GetOutfit().CollisionCategory = PhysicsObjectManager.WallCollisionGroup();
			physicsObjects = m_outfit.GetOutfit().GetPhysicsObjects();
			for (int i = 0; i < physicsObjects.Count; i++)
			{
				PhysicsObjectManager.RemovePlayerGeom(physicsObjects[i]);
				m_vSavedVel.Add(Vector2.Zero);
			}
			m_outfit.ResetToLocation(new Vector2(-2000f, 170f));
			m_outfit.GetOutfit().Disable();
		}
		m_iSavedTypeIndex = 0;
		m_outfit = m_SavedBabyTypes[m_iSavedTypeIndex][(int)bodyType];
		PhysicsOutfit outfit = m_outfit.GetOutfit();
		m_outfit.ResetToLocation(new Vector2(270f, 170f));
		m_outfit.UpdateEnabled();
		physicsObjects = outfit.GetPhysicsObjects();
		m_vSavedVel = new List<Vector2>();
		for (int j = 0; j < physicsObjects.Count; j++)
		{
			PhysicsObjectManager.AddPlayerGeom(physicsObjects[j], this);
			m_vSavedVel.Add(Vector2.Zero);
		}
		m_outfit.GetOutfit().CollisionCategory = PhysicsObjectManager.PlayerCollisionGroup();
		m_objs = physicsObjects;
		m_sprites = m_outfit.GetOutfit().GetSprites();
		if (m_controllers != null)
		{
			m_playerController = m_controllers[(int)bodyType];
			m_playerController.SwapOutfit();
		}
	}

	public void AddOutfitPiece(OutfitPiece p)
	{
		m_outfitPieces.Add(p);
	}

	private void SetupPyroTechnics()
	{
	}

	public void Reset()
	{
		m_bLaunched = false;
		m_outfit.ResetToLocation(new Vector2(270f, 170f));
		Vector2 position = m_objs[0].Position;
		m_bCamDoesScale = false;
		m_fPlayerCamZoom = 1f;
		m_vPlayerCamPos = new Vector2(Math.Max(700f, position.X - SceneRenderer.GetScreenDim().X / 2f + 800f), Math.Min(100f, Math.Max(-200f, position.Y + 200f)));
		SceneRenderer.MoveCamera(m_vPlayerCamPos, 0f, m_fPlayerCamZoom);
		m_outfit.GetOutfit().DisableStatic();
		m_positionsSaved.Clear();
		m_DistanceString = " feet";
		m_ScoreString = " points";
		m_iCollisionScore = 0;
		m_iLastDistance = 0;
		m_iLastScore = 0;
		for (int i = 0; i < m_scoreCounters.Count; i++)
		{
			m_scoreCounters[i] = 0;
		}
		m_playerController.Reset();
	}

	public void Update(TimeTracker gameTime)
	{
		m_outfit.Update(gameTime);
		Vector2 position = m_outfit.GetOutfit().GetPhysicsObjects()[0].Position;
		m_vAverageVel = default(Vector2);
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_vAverageVel += m_objs[i].Velocity;
		}
		m_vAverageVel /= (float)m_objs.Count;
		if (position.Y < m_ceilHeight && m_vAverageVel.Y < 0f)
		{
			float num = m_ceilHeight - position.Y;
			for (int j = 0; j < m_objs.Count; j++)
			{
				if (num > 200f)
				{
					m_objs[j].Velocity = new Vector2(m_objs[j].Velocity.X, 0f);
				}
				else
				{
					m_objs[j].ApplyImpulse(new Vector2(0f, 5f * num * gameTime.FractionOfSecond));
				}
			}
		}
		float num2 = 1000f + Math.Min(1f, Math.Min(0f, position.X) / 2000f) * 1000f;
		float num3 = 1f + 0.5f * ((1000f - num2) / 1000f);
		if (m_bRevertData)
		{
			SwapOutfits();
			m_bRevertData = false;
			m_playerController.RevertAction();
			for (int k = 0; k < m_objs.Count; k++)
			{
				m_objs[k].Velocity = m_vSavedVel[k];
			}
			float num4 = Math.Max((num2 - Math.Max(0f, m_vAverageVel.X)) / num2 * 400f, 0f) * num3;
			if (m_vAverageVel.Y > 0f)
			{
				for (int l = 0; l < m_objs.Count; l++)
				{
					float y = Math.Max(-500f, Math.Min(-0.6f * m_objs[l].Velocity.Y, -200f));
					m_objs[l].Velocity = new Vector2(m_objs[l].Velocity.X + num4, y);
				}
				m_vAverageVel.Y = 0f - m_vAverageVel.Y;
			}
			else
			{
				for (int m = 0; m < m_objs.Count; m++)
				{
					float y2 = Math.Min(m_objs[m].Velocity.Y, -500f);
					m_objs[m].Velocity = new Vector2(m_objs[m].Velocity.X + num4, y2);
				}
			}
		}
		m_playerController.Update(gameTime, num2, num3, m_vAverageVel);
		CalcScoreStrings();
		for (int n = 0; n < m_scoreParticles.Count; n++)
		{
			if (m_scoreParticles[n].Enabled)
			{
				m_scoreParticles[n].Update(gameTime);
			}
		}
		if (m_positionsSaved.Count >= 180)
		{
			m_positionsSaved.RemoveAt(0);
		}
		m_positionsSaved.Add(Position);
		for (int num5 = 0; num5 < m_outfitPieces.Count; num5++)
		{
			m_outfitPieces[num5].Update(gameTime);
		}
		AdjustCamera(m_outfit.GetOutfit().GetPhysicsObjects()[0].Position, gameTime);
		SceneRenderer.MoveCamera(m_vPlayerCamPos, 0f, m_fPlayerCamZoom);
		for (int num6 = m_popups.Count - 1; num6 >= 0; num6--)
		{
			m_popups[num6].Update(gameTime);
			if (!m_popups[num6].IsActive())
			{
				m_popups.RemoveAt(num6);
			}
		}
		UpdateAvatar();
	}

	private void AdjustCamera(Vector2 pos, TimeTracker gameTime)
	{
		Vector2 vector2;
		float num;
		if (m_bCamDoesScale)
		{
			Vector2 vector = new Vector2(Math.Max(700f, pos.X - SceneRenderer.GetScreenDim().X / 2f + 800f), Math.Min(100f, Math.Max(-175f, pos.Y + 350f)));
			vector2 = new Vector2(Math.Max(700f, pos.X - SceneRenderer.GetScreenDim().X / 2f + 800f), Math.Min(100f, Math.Max(-175f, pos.Y + 350f)));
			num = Math.Max(0.7f, (vector.Y + 600f) / 700f);
		}
		else
		{
			vector2 = new Vector2(Math.Max(700f, pos.X - SceneRenderer.GetScreenDim().X / 2f + 800f), Math.Min(100f, Math.Max(-200f, pos.Y + 200f)));
			num = 1f;
		}
		Vector2 cameraPosition = SceneRenderer.GetCameraPosition();
		float num2 = SceneRenderer.GetCameraZoom();
		if (num != num2)
		{
			num2 = ((!(Math.Abs(num - num2) < gameTime.FractionOfSecond)) ? (num2 + (float)Math.Sign(num - num2) * gameTime.FractionOfSecond) : num);
		}
		float num3 = Math.Max(1000f, 2f * m_vAverageVel.X);
		float num4 = Math.Max(1000f, 2f * m_vAverageVel.Y);
		if (Math.Abs(vector2.X - cameraPosition.X) <= gameTime.FractionOfSecond * num3)
		{
			cameraPosition.X = vector2.X;
		}
		else
		{
			float num5 = Math.Sign(vector2.X - cameraPosition.X);
			cameraPosition.X += num5 * gameTime.FractionOfSecond * num3;
		}
		if (Math.Abs(vector2.Y - cameraPosition.Y) <= gameTime.FractionOfSecond * num4)
		{
			cameraPosition.Y = vector2.Y;
		}
		else
		{
			float num6 = Math.Sign(vector2.Y - cameraPosition.Y);
			cameraPosition.Y += num6 * gameTime.FractionOfSecond * num4;
		}
		m_vPlayerCamPos = cameraPosition;
		m_fPlayerCamZoom = num2;
	}

	private void UpdateAvatar()
	{
		AvatarHandler avatar = SceneRenderer.Avatar;
		if (avatar != null)
		{
			Vector2 position = SceneRenderer.GetCameraPosition() - m_sprites[0].Position;
			avatar.SetRotations(m_sprites[1].Rotation - m_sprites[0].Rotation, m_sprites[3].Rotation - m_sprites[0].Rotation, m_sprites[2].Rotation - m_sprites[0].Rotation, m_sprites[0].Rotation, position, 1001f, SceneRenderer.GetCameraZoom());
		}
	}

	public bool IsStopped()
	{
		if (m_objs[0].Velocity.LengthSquared() < 900f && m_positionsSaved.Count == 180)
		{
			return (m_positionsSaved.First() - m_positionsSaved.Last()).Length() < 70f;
		}
		return false;
	}

	private void CalcScoreStrings()
	{
		if (m_iLastDistance != DistanceTravelled)
		{
			int num = DistanceTravelled;
			int num2 = 0;
			if (num == 0)
			{
				m_distanceCharArray[num2] = '0';
				num2++;
			}
			else
			{
				while (num >= 1)
				{
					int num3 = num % 10;
					m_distanceCharArray[num2] = (char)(48 + num3);
					num /= 10;
					num2++;
				}
			}
			for (int i = 0; i < num2 / 2; i++)
			{
				char c = m_distanceCharArray[i];
				m_distanceCharArray[i] = m_distanceCharArray[num2 - 1 - i];
				m_distanceCharArray[num2 - 1 - i] = c;
			}
			m_distanceCharArray[num2] = '\0';
			m_MutatedDistanceString.Length = 0;
			m_MutatedDistanceString.Insert(0, m_distanceCharArray, 0, num2);
			m_MutatedDistanceString.Length = num2;
			m_iLastDistance = DistanceTravelled;
			if (m_iLastDistance != 1)
			{
				m_MutatedDistanceString.Append(m_DistanceString);
			}
			else
			{
				m_MutatedDistanceString.Append(" foot");
			}
		}
		if (m_iLastScore == GetScore())
		{
			return;
		}
		int num4 = GetScore();
		int num5 = 0;
		if (num4 == 0)
		{
			m_scoreCharArray[num5] = '0';
			num5++;
		}
		else
		{
			while (num4 >= 1)
			{
				int num6 = num4 % 10;
				m_scoreCharArray[num5] = (char)(48 + num6);
				num4 /= 10;
				num5++;
			}
		}
		for (int j = 0; j < num5 / 2; j++)
		{
			char c2 = m_scoreCharArray[j];
			m_scoreCharArray[j] = m_scoreCharArray[num5 - 1 - j];
			m_scoreCharArray[num5 - 1 - j] = c2;
		}
		m_scoreCharArray[num5] = '\0';
		m_MutatedScoreString.Length = 0;
		m_MutatedScoreString.Insert(0, m_scoreCharArray, 0, num5);
		m_MutatedScoreString.Length = num5;
		m_iLastScore = GetScore();
		if (m_iLastScore != 1)
		{
			m_MutatedScoreString.Append(m_ScoreString);
		}
		else
		{
			m_MutatedScoreString.Append(" point");
		}
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.ActiveMenuIndex < 0)
		{
			if (ControlManager.DetectInput() >= 0)
			{
				ControlManager.ActiveMenuIndex = ControlManager.DetectInput();
			}
		}
		else
		{
			m_playerController.HandleInput(gameTime);
		}
	}

	public void Draw(TimeTracker gameTime, float fadeAmount)
	{
		if (m_BabyPropType == PropType.BABY_AVATAR)
		{
			SceneRenderer.Avatar.ShouldDraw = true;
		}
		else
		{
			SceneRenderer.Avatar.ShouldDraw = false;
		}
		if (m_bLaunched)
		{
			m_outfit.Draw(gameTime);
			Color c = Color.Black;
			if (SceneRenderer.GetEffectMode() == 1)
			{
				c = Color.Lime;
			}
			c.A = (byte)(255f * (1f - fadeAmount));
			SceneRenderer.DrawString(fonts.GRUNGE_FONT, m_MutatedDistanceString, SceneRenderer.GetCameraPosition() + m_scorePos + new Vector2(0f, 0f), c, new Vector2(1f, 1f), DepthConsts.LOGO_DEPTH);
			SceneRenderer.DrawString(fonts.GRUNGE_FONT, m_MutatedScoreString, SceneRenderer.GetCameraPosition() + m_scorePos + new Vector2(0f, 70f), c, new Vector2(1f, 1f), DepthConsts.LOGO_DEPTH);
			for (int i = 0; i < m_scoreParticles.Count; i++)
			{
				if (m_scoreParticles[i].Enabled)
				{
					m_scoreParticles[i].Draw(gameTime);
				}
			}
			if (m_BabyPropType != PropType.BABY_AVATAR)
			{
				for (int j = 0; j < m_outfitPieces.Count; j++)
				{
					m_outfitPieces[j].Draw(gameTime);
				}
			}
			for (int k = 0; k < m_popups.Count; k++)
			{
				m_popups[k].Draw(gameTime);
			}
			m_playerController.Draw(gameTime);
		}
		else
		{
			SceneRenderer.Avatar.ShouldDraw = false;
		}
	}

	public void SaveFrameData(PropType type)
	{
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_vSavedVel[i] = m_objs[i].Velocity;
		}
		m_playerController.CollisionResponse();
		m_bRevertData = true;
		IterateScoreForCollision(type);
		SpawnCollisionInfo(type);
		ControlManager.SetVibration(ControlManager.ActiveMenuIndex, 1.4f);
	}

	private void SpawnCollisionInfo(PropType type)
	{
		for (int i = 0; i < m_popups.Count; i++)
		{
			m_popups[i].ForceExit();
		}
		m_popups.Add(new AwardPopup(type));
	}

	private void IterateScoreForCollision(PropType type)
	{
		m_iCollisionScore += 30;
		switch (type)
		{
		case PropType.TOY_BEAR:
			m_scoreCounters[18]++;
			break;
		case PropType.COFFEESTOOLTABLE2:
		case PropType.COFFEECOUCHTABLE2:
			m_scoreCounters[144]++;
			break;
		case PropType.RUNNER2:
			m_scoreCounters[104]++;
			break;
		case PropType.CHANGING_TABLE:
			m_scoreCounters[39]++;
			break;
		case PropType.GIANT_TV:
		case PropType.EASY_GLOW:
		case PropType.MED_GLOW:
		case PropType.HARD_GLOW:
		case PropType.VHARD_GLOW:
			m_scoreCounters[163]++;
			break;
		default:
			m_scoreCounters[(int)type]++;
			break;
		}
		int num = 1;
		float rand = SceneRenderer.GetRand(400f, 600f);
		float rand2 = SceneRenderer.GetRand(-0.3f, 0.3f);
		float rand3 = SceneRenderer.GetRand(0.7f, 1.4f);
		for (int i = 0; i < m_scoreParticles.Count; i++)
		{
			if (!m_scoreParticles[i].Enabled)
			{
				m_scoreParticles[i].ResetTo(m_scorePos, rand2 + 0.05f * ((float)num - 1.5f), rand + (float)(50 * num), rand3);
				num++;
				if (num > 3)
				{
					break;
				}
			}
		}
	}

	public List<int> GetScoreCounters()
	{
		return m_scoreCounters;
	}

	public void Launch(float pow)
	{
		Vector2 vector = new Vector2(1f, -0.5f);
		vector.Normalize();
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_objs[i].ApplyImpulse(vector * pow);
		}
		m_positionsSaved.Clear();
		m_bLaunched = true;
	}

	public int GetScore()
	{
		return DistanceTravelled + m_iCollisionScore;
	}

	public Prop GetProp()
	{
		return m_outfit;
	}

	public Vector2 GetVel()
	{
		return m_vAverageVel;
	}

	public List<SpriteInstance> GetFirstOutfitSprites()
	{
		return m_SavedBabyTypes[0][0].GetOutfit().GetSprites();
	}
}
