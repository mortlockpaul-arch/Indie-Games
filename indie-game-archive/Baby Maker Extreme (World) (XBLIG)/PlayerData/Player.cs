using System;
using System.Collections.Generic;
using System.Text;
using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;
using Renderer;
using Scene;

namespace PlayerData;

public class Player
{
	private const int PARTICLE_SPAWN_TIME = 60;

	private const int DIVE_VAL = 5000;

	private const int DIVE_TIME = 1000;

	private const int LEAP_TIME = 500;

	private const int ROOF_POS = -50;

	private const int FLOOR_POS = 600;

	private const int SCORE_COLLIDE_ITER = 30;

	private List<PhysicalRepresentation> m_objs;

	private List<Vector2> m_vSavedVel;

	private bool m_bRevertData;

	private Vector2 m_vVelMod;

	private PhysicsOutfit m_physicsOutfit;

	private bool m_bRevertVel;

	private List<AwardPopup> m_popups;

	private List<AwardPopup> m_popupRemoves;

	private int m_iDiveCharge;

	private int m_iParticleTimer;

	private List<Vector2> m_distTravelledPerSec;

	private bool m_bBabyStopped;

	private bool m_bBabyLaunched;

	private Dictionary<int, int> m_typeCounts;

	private List<RenderSprite> m_sprites;

	private List<RenderSprite> m_propDecor;

	private List<int> m_propDecorSprite;

	private List<bool> m_propDecorEnabled;

	private string m_sex;

	private bool m_bBadBreathing;

	private bool m_bBadAiming;

	private bool m_bAimedHigh;

	private bool m_bBadPushing;

	private SoundEffect m_popSound;

	private bool m_bAvatar;

	private bool m_bCanDive;

	private bool m_bCanLeap;

	private int m_iLeapTimer;

	private int m_iDiveTimer;

	private RenderSprite m_diveSpr;

	private RenderSprite m_boostSpr;

	private RenderSprite m_flare1;

	private RenderSprite m_flare2;

	private int m_iLastDistance;

	private int m_iLastScore;

	private StringBuilder m_MutatedDistanceString;

	private char[] m_distanceCharArray;

	private StringBuilder m_MutatedScoreString;

	private char[] m_scoreCharArray;

	private string m_DistanceString;

	private string m_ScoreString;

	private int m_iCollisionPoints;

	public bool Avatar
	{
		get
		{
			return m_bAvatar;
		}
		set
		{
			if (value != m_bAvatar && value)
			{
				Game1.LoadAvatar();
			}
			else if (value != m_bAvatar)
			{
				Game1.ResetAvatar();
			}
			m_bAvatar = value;
		}
	}

	public bool BadBreathing
	{
		get
		{
			return m_bBadBreathing;
		}
		set
		{
			m_bBadBreathing = value;
		}
	}

	public bool BadAiming
	{
		get
		{
			return m_bBadAiming;
		}
		set
		{
			m_bBadAiming = value;
		}
	}

	public bool AimedHigh
	{
		get
		{
			return m_bAimedHigh;
		}
		set
		{
			m_bAimedHigh = value;
		}
	}

	public bool BadPushing
	{
		get
		{
			return m_bBadPushing;
		}
		set
		{
			m_bBadPushing = value;
		}
	}

	public int DistanceTravelled => (int)(m_sprites[0].Position.X / 75f);

	public Vector2 Position => m_sprites[0].Position;

	public Player()
	{
		m_DistanceString = " feet";
		m_ScoreString = " points";
		m_physicsOutfit = new PhysicsOutfit(1);
		PropGenerator.CreateBaby(m_physicsOutfit);
		m_objs = m_physicsOutfit.GetPhysicsObjects();
		m_physicsOutfit.ResetToPosition(new Vector2(100f, 480f));
		m_physicsOutfit.SetDepth(100f);
		InitPropsDecor();
		Initialize();
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_objs[i].AirDrag = 0f;
			m_objs[i].Bounciness = 0.2f;
			m_objs[i].FrictionCoeff = 0.5f;
			m_objs[i].Mass = 1f;
			m_objs[i].CollisionCategory = PhysicsObjectManager.PlayerCollisionGroup();
			PhysicsObjectManager.AddPlayerGeom(m_objs[i], this);
		}
		m_popSound = SoundManager.GetSoundEffect("sounds/pop");
		m_bAvatar = false;
		m_iLastDistance = 0;
		m_iLastScore = 0;
	}

	public void InitPropsDecor()
	{
		m_sprites = m_physicsOutfit.GetSprites(0);
		m_propDecor = new List<RenderSprite>();
		m_propDecorEnabled = new List<bool>();
		m_propDecorSprite = new List<int>();
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(25, 5, 82, 84), default(Vector2), m_sprites[1].Depth + 0.0005f));
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(296, 5, 82, 84), default(Vector2), m_sprites[1].Depth + 0.0005f));
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(112, 5, 82, 84), default(Vector2), m_sprites[1].Depth + 0.0004f));
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(204, 5, 82, 84), default(Vector2), m_sprites[1].Depth + 0.0006f));
		m_propDecorSprite.Add(1);
		m_propDecorSprite.Add(1);
		m_propDecorSprite.Add(1);
		m_propDecorSprite.Add(1);
		m_propDecorEnabled.Add(item: false);
		m_propDecorEnabled.Add(item: false);
		m_propDecorEnabled.Add(item: false);
		m_propDecorEnabled.Add(item: false);
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(13, 99, 92, 76), default(Vector2), m_sprites[3].Depth + 0.0001f));
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(13, 181, 92, 76), default(Vector2), m_sprites[3].Depth + 0.0001f));
		m_propDecorSprite.Add(3);
		m_propDecorSprite.Add(3);
		m_propDecorEnabled.Add(item: false);
		m_propDecorEnabled.Add(item: false);
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(122, 95, 100, 106), default(Vector2), m_sprites[0].Depth + 0.0001f));
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(122, 211, 100, 106), default(Vector2), m_sprites[0].Depth + 0.0001f));
		m_propDecorSprite.Add(0);
		m_propDecorSprite.Add(0);
		m_propDecorEnabled.Add(item: false);
		m_propDecorEnabled.Add(item: false);
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(230, 105, 83, 89), default(Vector2), m_sprites[2].Depth + 0.0001f));
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(228, 106, 83, 87), default(Vector2), m_sprites[4].Depth + 0.0001f));
		m_propDecorSprite.Add(2);
		m_propDecorSprite.Add(4);
		m_propDecorEnabled.Add(item: false);
		m_propDecorEnabled.Add(item: false);
		m_sex = "boy";
	}

	public List<bool> GetEnabledDecor()
	{
		return m_propDecorEnabled;
	}

	public string GetSex()
	{
		return m_sex;
	}

	public void Initialize()
	{
		if (SceneRenderer.GetRand(0f, 1f) < 0.5f || (m_bAvatar && Game1.GetAvatar().IsMale()))
		{
			m_sex = "boy";
			if (m_propDecorEnabled[0] || m_propDecorEnabled[1])
			{
				m_propDecorEnabled[0] = true;
				m_propDecorEnabled[1] = false;
			}
		}
		else
		{
			m_sex = "girl";
			if (m_propDecorEnabled[0] || m_propDecorEnabled[1])
			{
				m_propDecorEnabled[0] = false;
				m_propDecorEnabled[1] = true;
			}
		}
		m_physicsOutfit.ResetToPosition(new Vector2(100f, 477f));
		m_physicsOutfit.RemoveStatic();
		m_bRevertData = false;
		m_vSavedVel = new List<Vector2>();
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_vSavedVel.Add(default(Vector2));
		}
		m_vVelMod = default(Vector2);
		m_bRevertVel = false;
		m_popups = new List<AwardPopup>();
		m_popupRemoves = new List<AwardPopup>();
		m_diveSpr = SpriteManager.GetSprite("images/UI/diveButton", default(Vector2), DepthConsts.LOGO_DEPTH);
		m_diveSpr.Alpha = 0f;
		m_boostSpr = SpriteManager.GetSprite("images/UI/boostButton", default(Vector2), DepthConsts.LOGO_DEPTH);
		m_boostSpr.Alpha = 0f;
		m_flare1 = SpriteManager.GetSprite("images/UI/flare", default(Vector2), DepthConsts.LOGO_DEPTH + 1f);
		m_flare1.Alpha = 0f;
		m_flare1.SurfaceScale = new Vector2(200f, 150f);
		m_flare2 = SpriteManager.GetSprite("images/UI/flare", default(Vector2), DepthConsts.LOGO_DEPTH + 1f);
		m_flare2.Alpha = 0f;
		m_flare2.SurfaceScale = new Vector2(200f, 150f);
		m_iParticleTimer = 0;
		m_distTravelledPerSec = new List<Vector2>();
		m_bBabyStopped = false;
		m_bBabyLaunched = false;
		m_typeCounts = new Dictionary<int, int>();
		m_bBadBreathing = false;
		m_bBadAiming = false;
		m_bAimedHigh = false;
		m_bBadPushing = false;
		m_bCanDive = false;
		m_bCanLeap = false;
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_iLastDistance = 0;
		m_MutatedDistanceString = new StringBuilder("0", 32);
		m_distanceCharArray = new char[32];
		m_distanceCharArray[0] = '\0';
		m_MutatedScoreString = new StringBuilder("0", 32);
		m_scoreCharArray = new char[32];
		m_scoreCharArray[0] = '\0';
		m_iCollisionPoints = 0;
	}

	private List<PhysicalRepresentation> GetPhysicsObj()
	{
		return m_objs;
	}

	public void Launch(Vector2 angle, float pow)
	{
		SoundManager.AddSoundToPlay(m_popSound, Math.Max(0.5f, Math.Min(pow / 1000f, 1f)), SceneRenderer.GetRand(-0.2f, 0.2f), 0);
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_objs[i].ApplyImpulse(angle * pow);
		}
		m_distTravelledPerSec.Clear();
		m_bBabyStopped = false;
		m_bBabyLaunched = true;
	}

	public void Update(TimeTracker gameTime, bool isActive)
	{
		m_physicsOutfit.Update(gameTime);
		Vector2 vector = default(Vector2);
		for (int i = 0; i < m_objs.Count; i++)
		{
			vector += m_objs[i].Velocity;
		}
		vector /= (float)m_objs.Count;
		float num = 1000 + Math.Min(1, DistanceTravelled / 2000) * 1000;
		float num2 = 1f + 0.5f * ((1000f - num) / 1000f);
		if (m_bRevertData)
		{
			m_bRevertData = false;
			for (int j = 0; j < m_objs.Count; j++)
			{
				m_objs[j].Velocity = m_vSavedVel[j];
			}
			float num3 = Math.Max((num - Math.Max(0f, vector.X)) / num * 400f, 0f) * num2;
			if (vector.Y > 0f)
			{
				for (int k = 0; k < m_objs.Count; k++)
				{
					float y = Math.Max(-400f, Math.Min(-0.4f * m_objs[k].Velocity.Y, -150f));
					m_objs[k].Velocity = new Vector2(m_objs[k].Velocity.X + num3, y);
				}
				vector.Y = 0f - vector.Y;
			}
			else
			{
				for (int l = 0; l < m_objs.Count; l++)
				{
					float y2 = Math.Min(m_objs[l].Velocity.Y, -150f);
					m_objs[l].Velocity = new Vector2(m_objs[l].Velocity.X + num3, y2);
				}
			}
		}
		if (vector.Y <= 0f)
		{
			m_vVelMod = default(Vector2);
		}
		if (m_bRevertVel)
		{
			for (int m = 0; m < m_objs.Count; m++)
			{
				m_objs[m].Velocity -= m_vVelMod;
			}
			m_vVelMod = default(Vector2);
			m_bRevertVel = false;
		}
		for (int n = 0; n < m_popups.Count; n++)
		{
			m_popups[n].Update(gameTime);
			if (!m_popups[n].IsActive())
			{
				m_popupRemoves.Add(m_popups[n]);
			}
		}
		for (int num4 = 0; num4 < m_popupRemoves.Count; num4++)
		{
			m_popups.Remove(m_popupRemoves[num4]);
		}
		m_popupRemoves.Clear();
		m_distTravelledPerSec.Add(Position);
		if (m_distTravelledPerSec.Count > 180)
		{
			m_distTravelledPerSec.RemoveAt(0);
		}
		if (m_distTravelledPerSec.Count == 180 && ((!m_bCanDive && !m_bCanLeap && m_iDiveTimer <= 0 && m_iLeapTimer <= 0 && (m_distTravelledPerSec[120] - m_distTravelledPerSec[179]).Length() < 10f) || ((m_bCanLeap || m_bCanDive) && (m_distTravelledPerSec[0] - m_distTravelledPerSec[59]).Length() < 10f && (m_distTravelledPerSec[60] - m_distTravelledPerSec[119]).Length() < 10f && (m_distTravelledPerSec[120] - m_distTravelledPerSec[179]).Length() < 10f)))
		{
			m_bBabyStopped = true;
		}
		else
		{
			m_bBabyStopped = false;
		}
		for (int num5 = 0; num5 < m_propDecorSprite.Count; num5++)
		{
			m_propDecor[num5].Origin = m_sprites[m_propDecorSprite[num5]].Origin;
			m_propDecor[num5].Position = m_sprites[m_propDecorSprite[num5]].Position;
			m_propDecor[num5].Rotation = m_sprites[m_propDecorSprite[num5]].Rotation;
		}
		AvatarHandler avatar = Game1.GetAvatar();
		if (avatar != null)
		{
			Vector2 position = SceneRenderer.GetCameraPosition() - m_sprites[0].Position;
			avatar.SetRotations(m_sprites[1].Rotation - m_sprites[0].Rotation, m_sprites[3].Rotation - m_sprites[0].Rotation, m_sprites[2].Rotation - m_sprites[0].Rotation, m_sprites[0].Rotation, position, 1001f);
			avatar.ShouldDraw = m_bBabyLaunched;
		}
		if (m_iLeapTimer > 0)
		{
			m_iLeapTimer -= gameTime.ElapsedMilli;
			if (m_iLeapTimer > 0 && m_objs[0].Position.Y > -50f)
			{
				float num6 = 1300f;
				m_iParticleTimer += gameTime.ElapsedMilli;
				if (m_iParticleTimer > 60)
				{
					m_iParticleTimer -= 60;
					m_physicsOutfit.GenerateParticles(new Color((Color.Blue.ToVector3() + Color.White.ToVector3()) / 2f));
				}
				if (m_objs[0].Velocity.Y > -1000f)
				{
					for (int num7 = 0; num7 < m_objs.Count; num7++)
					{
						float num8 = Math.Max((num - Math.Max(0f, vector.X)) / num, 0f) * num2;
						Vector2 vector2 = new Vector2(1f, -0.8f);
						vector2.Normalize();
						vector2.X *= num8;
						if (vector.Y > 0f)
						{
							m_objs[num7].Velocity = new Vector2(m_objs[num7].Velocity.X, 0f);
						}
						m_objs[num7].Velocity += gameTime.FractionOfSecond * vector2 * num6;
					}
				}
			}
		}
		if (m_iDiveTimer > 0)
		{
			m_iDiveTimer -= gameTime.ElapsedMilli;
			if (m_iDiveTimer > 0)
			{
				float num9 = 1500f;
				if (m_objs[0].Position.Y < 600f)
				{
					m_iParticleTimer += gameTime.ElapsedMilli;
					if (m_iParticleTimer > 60)
					{
						m_iParticleTimer -= 60;
						m_physicsOutfit.GenerateParticles(new Color((Color.Lime.ToVector3() + Color.White.ToVector3()) / 2f));
					}
					if (m_objs[0].Velocity.Y < 1000f)
					{
						m_vVelMod += gameTime.FractionOfSecond * new Vector2(0f, 1f) * num9;
						for (int num10 = 0; num10 < m_objs.Count; num10++)
						{
							m_objs[num10].Velocity += gameTime.FractionOfSecond * new Vector2(0f, 1f) * num9;
						}
					}
				}
			}
		}
		if (m_iLastDistance != DistanceTravelled)
		{
			int num11 = DistanceTravelled;
			int num12 = 0;
			if (num11 == 0)
			{
				m_distanceCharArray[num12] = '0';
				num12++;
			}
			else
			{
				while (num11 >= 1)
				{
					int num13 = num11 % 10;
					m_distanceCharArray[num12] = (char)(48 + num13);
					num11 /= 10;
					num12++;
				}
			}
			for (int num14 = 0; num14 < num12 / 2; num14++)
			{
				char c = m_distanceCharArray[num14];
				m_distanceCharArray[num14] = m_distanceCharArray[num12 - 1 - num14];
				m_distanceCharArray[num12 - 1 - num14] = c;
			}
			m_distanceCharArray[num12] = '\0';
			m_MutatedDistanceString.Length = 0;
			m_MutatedDistanceString.Insert(0, m_distanceCharArray, 0, num12);
			m_MutatedDistanceString.Length = num12;
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
		int num15 = GetScore();
		int num16 = 0;
		if (num15 == 0)
		{
			m_scoreCharArray[num16] = '0';
			num16++;
		}
		else
		{
			while (num15 >= 1)
			{
				int num17 = num15 % 10;
				m_scoreCharArray[num16] = (char)(48 + num17);
				num15 /= 10;
				num16++;
			}
		}
		for (int num18 = 0; num18 < num16 / 2; num18++)
		{
			char c2 = m_scoreCharArray[num18];
			m_scoreCharArray[num18] = m_scoreCharArray[num16 - 1 - num18];
			m_scoreCharArray[num16 - 1 - num18] = c2;
		}
		m_scoreCharArray[num16] = '\0';
		m_MutatedScoreString.Length = 0;
		m_MutatedScoreString.Insert(0, m_scoreCharArray, 0, num16);
		m_MutatedScoreString.Length = num16;
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

	public bool IsStopped()
	{
		return m_bBabyStopped;
	}

	public int GetScore()
	{
		return DistanceTravelled + m_iCollisionPoints;
	}

	public void Draw(TimeTracker gameTime)
	{
		if (!m_bAvatar)
		{
			m_physicsOutfit.Draw(gameTime);
			for (int i = 0; i < m_propDecorSprite.Count; i++)
			{
				if (m_propDecorEnabled[i])
				{
					m_propDecor[i].Draw(gameTime);
				}
			}
		}
		for (int j = 0; j < m_popups.Count; j++)
		{
			m_popups[j].Draw(gameTime);
		}
		m_boostSpr.Position = SceneRenderer.GetCameraPosition() - new Vector2(-300f, 250f);
		m_flare1.Position = m_boostSpr.Position;
		m_diveSpr.Position = SceneRenderer.GetCameraPosition() - new Vector2(-300f, 200f);
		m_flare2.Position = m_diveSpr.Position;
		if (m_bCanDive)
		{
			m_diveSpr.Alpha += gameTime.FractionOfSecond * 6f;
		}
		else
		{
			m_diveSpr.Alpha -= gameTime.FractionOfSecond * 6f;
		}
		if (m_diveSpr.Alpha < 0f)
		{
			m_diveSpr.Alpha = 0f;
		}
		else if (m_diveSpr.Alpha > 1f)
		{
			m_diveSpr.Alpha = 1f;
		}
		if (m_diveSpr.Alpha == 1f || !m_bCanDive)
		{
			m_flare2.Alpha -= gameTime.FractionOfSecond * 6f;
			if (m_flare2.Alpha < 0f)
			{
				m_flare2.Alpha = 0f;
			}
		}
		else if (m_bCanDive)
		{
			m_flare2.Alpha = m_diveSpr.Alpha;
		}
		if (m_bCanLeap)
		{
			m_boostSpr.Alpha += gameTime.FractionOfSecond * 6f;
		}
		else
		{
			m_boostSpr.Alpha -= gameTime.FractionOfSecond * 6f;
		}
		if (m_boostSpr.Alpha < 0f)
		{
			m_boostSpr.Alpha = 0f;
		}
		else if (m_boostSpr.Alpha > 1f)
		{
			m_boostSpr.Alpha = 1f;
		}
		if (m_boostSpr.Alpha == 1f || !m_bCanLeap)
		{
			m_flare1.Alpha -= gameTime.FractionOfSecond * 6f;
			if (m_flare1.Alpha < 0f)
			{
				m_flare1.Alpha = 0f;
			}
		}
		else if (m_bCanLeap)
		{
			m_flare1.Alpha = m_boostSpr.Alpha;
		}
		m_flare1.Draw(gameTime);
		m_flare2.Draw(gameTime);
		m_boostSpr.Draw(gameTime);
		m_diveSpr.Draw(gameTime);
		SceneRenderer.DrawString(fonts.GRUNGE_FONT, m_MutatedDistanceString, SceneRenderer.GetCameraPosition() + SceneRenderer.GetScreenDim() * 0.4f - new Vector2(200f, 60f), Color.Black, new Vector2(1f, 1f), DepthConsts.LOGO_DEPTH);
		SceneRenderer.DrawString(fonts.GRUNGE_FONT, m_MutatedScoreString, SceneRenderer.GetCameraPosition() + SceneRenderer.GetScreenDim() * 0.4f - new Vector2(200f, 30f), Color.Black, new Vector2(1f, 1f), DepthConsts.LOGO_DEPTH);
	}

	public void HandleInput(TimeTracker gametime)
	{
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.X))
		{
			if (m_bCanLeap)
			{
				m_bCanLeap = false;
				m_iLeapTimer = 500;
				m_iDiveTimer = 0;
			}
		}
		else if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.A) && m_bCanDive)
		{
			m_bCanDive = false;
			m_iDiveTimer = 1000;
			m_iLeapTimer = 0;
		}
	}

	public void SaveFrameData(PropType type)
	{
		m_iDiveCharge = 5000;
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_vSavedVel[i] = m_objs[i].Velocity;
		}
		m_bRevertData = true;
		for (int j = 0; j < m_popups.Count; j++)
		{
			m_popups[j].ForceExit();
		}
		m_popups.Add(new AwardPopup(type));
		if (m_typeCounts.ContainsKey((int)type))
		{
			m_typeCounts[(int)type]++;
		}
		else
		{
			m_typeCounts[(int)type] = 1;
		}
		ControlManager.SetVibration(ControlManager.ActiveMenuIndex, 0.3f);
		m_bCanDive = true;
		m_bCanLeap = true;
		if (m_iLeapTimer > 0)
		{
			m_iLeapTimer = 1;
		}
		if (m_iDiveTimer > 0)
		{
			m_iDiveTimer = 1;
		}
		m_iCollisionPoints += 30;
	}

	public int GetTypeCount(PropType type)
	{
		if (m_typeCounts.ContainsKey((int)type))
		{
			return m_typeCounts[(int)type];
		}
		return 0;
	}
}
