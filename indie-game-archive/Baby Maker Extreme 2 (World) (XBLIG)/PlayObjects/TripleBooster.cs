using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsHandler;
using Renderer;

namespace PlayObjects;

public class TripleBooster : PlayerController
{
	private const int PARTICLE_SPAWN_TIME = 60;

	private const int LEAP_TIME = 500;

	private const int DIVE_TIME = 1000;

	private int m_iLeapTimer;

	private int m_iDiveTimer;

	private bool m_bCanBoost;

	private bool m_bCanDive;

	private SpriteInstance m_diveButton;

	private SpriteInstance m_boostButton;

	private List<SpriteInstance> m_nums;

	private float m_fBoostPerc;

	private float m_fDivePerc;

	private int m_iParticleTimer;

	private Player m_player;

	private List<PhysicalRepresentation> m_objs;

	private Prop m_outfit;

	private int m_iBoostCount;

	private string m_count;

	private int m_iClickOverCounter;

	private List<SpriteInstance> m_balls;

	private List<SpriteInstance> m_ballsGlows;

	public TripleBooster(Player p)
	{
		m_player = p;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_fDivePerc = 0f;
		m_fBoostPerc = 0f;
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_bCanBoost = false;
		m_bCanDive = false;
		m_boostButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 1, 181, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_boostButton.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/boostDive3Norm");
		m_boostButton.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_diveButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 103, 155, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_nums = new List<SpriteInstance>();
		m_nums.Add(TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(183, 1, 21, 50), default(Vector2), DepthConsts.LOGO_DEPTH));
		m_nums.Add(TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(203, 1, 25, 50), default(Vector2), DepthConsts.LOGO_DEPTH));
		m_nums.Add(TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(228, 1, 27, 50), default(Vector2), DepthConsts.LOGO_DEPTH));
		m_iParticleTimer = 0;
		m_iBoostCount = 0;
		m_count = "";
		m_iClickOverCounter = 0;
		m_balls = new List<SpriteInstance>();
		m_ballsGlows = new List<SpriteInstance>();
		for (int i = 0; i < 3; i++)
		{
			m_balls.Add(TextureContainer.GetSprite("images/ball", default(Vector2), DepthConsts.LOGO_DEPTH));
			m_balls.Last().GetSpriteImage().GetSpritePage()
				.NormTex = TextureContainer.GetTexture("images/ballNorm");
			m_balls.Last().WidthScale = 10f;
			m_ballsGlows.Add(TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(233, 213, 21, 20), default(Vector2), DepthConsts.LOGO_DEPTH - 1f));
			m_ballsGlows.Last().FlatColor = true;
			m_ballsGlows.Last().Origin = new Vector2(3f, 0f);
			m_ballsGlows.Last().WidthScale = 13f;
		}
	}

	public override void Reset()
	{
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_bCanBoost = false;
		m_bCanDive = false;
		m_iParticleTimer = 0;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_iBoostCount = 0;
		m_count = "";
		m_iClickOverCounter = 0;
	}

	public override void Update(TimeTracker gameTime, float modPow, float modBoost, Vector2 averageVel)
	{
		if (m_iLeapTimer > 0)
		{
			if (m_objs[0].Position.Y > -550f)
			{
				m_iLeapTimer -= gameTime.ElapsedMilli;
				if (m_iLeapTimer > 0)
				{
					m_iParticleTimer += gameTime.ElapsedMilli;
					if (m_iParticleTimer > 60)
					{
						m_iParticleTimer -= 60;
						m_outfit.GetOutfit().GenerateParticles(new Color((Color.Blue.ToVector3() + Color.White.ToVector3()) / 2f));
					}
					float num = 2000f;
					if (m_objs[0].Velocity.Y > -1000f)
					{
						for (int i = 0; i < m_objs.Count; i++)
						{
							float num2 = Math.Max((modPow - Math.Max(0f, averageVel.X)) / modPow, 0f) * modBoost;
							Vector2 vector = new Vector2(1f, -0.8f);
							vector.Normalize();
							vector.X *= num2;
							if (averageVel.Y > 0f)
							{
								m_objs[i].Velocity = new Vector2(m_objs[i].Velocity.X, 0f);
							}
							m_objs[i].Velocity += gameTime.FractionOfSecond * vector * num;
						}
					}
				}
			}
			else
			{
				m_iLeapTimer = 0;
			}
		}
		if (m_iDiveTimer <= 0)
		{
			return;
		}
		m_iDiveTimer -= gameTime.ElapsedMilli;
		if (m_iDiveTimer <= 0)
		{
			return;
		}
		m_iParticleTimer += gameTime.ElapsedMilli;
		if (m_iParticleTimer > 60)
		{
			m_iParticleTimer -= 60;
			m_outfit.GetOutfit().GenerateParticles(new Color((Color.Lime.ToVector3() + Color.White.ToVector3()) / 2f));
		}
		float num3 = 2200f;
		if (m_objs[0].Position.Y < 250f)
		{
			if (m_objs[0].Velocity.Y < 1000f)
			{
				for (int j = 0; j < m_objs.Count; j++)
				{
					m_objs[j].Velocity += gameTime.FractionOfSecond * new Vector2(0f, 1f) * num3;
				}
			}
		}
		else
		{
			m_iDiveTimer = 0;
		}
	}

	public override void Draw(TimeTracker gameTime)
	{
		if (SceneRenderer.GetEffectMode() != m_ilastEffectMode)
		{
			SwapVirtual(m_boostButton);
			SwapVirtual(m_diveButton);
			SwapVirtual(m_nums[0]);
			SwapVirtual(m_nums[1]);
			SwapVirtual(m_nums[2]);
			m_ilastEffectMode = 1 - m_ilastEffectMode;
		}
		if (m_bCanBoost)
		{
			m_fBoostPerc += gameTime.FractionOfSecond * 4f;
			if (m_fBoostPerc > 1f)
			{
				m_fBoostPerc = 1f;
			}
		}
		else
		{
			m_fBoostPerc -= gameTime.FractionOfSecond * 4f;
			if (m_fBoostPerc < 0f)
			{
				m_fBoostPerc = 0f;
			}
		}
		if (m_bCanDive)
		{
			m_fDivePerc += gameTime.FractionOfSecond * 4f;
			if (m_fDivePerc > 1f)
			{
				m_fDivePerc = 1f;
			}
		}
		else
		{
			m_fDivePerc -= gameTime.FractionOfSecond * 4f;
			if (m_fDivePerc < 0f)
			{
				m_fDivePerc = 0f;
			}
		}
		m_boostButton.Alpha = m_fBoostPerc;
		m_boostButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(250f, -260f) / SceneRenderer.GetCameraZoom();
		m_boostButton.WidthScale = m_boostButton.GetSpriteImage().Width * 1.4f / SceneRenderer.GetCameraZoom();
		m_boostButton.Draw(gameTime);
		m_nums[Math.Max(0, m_iBoostCount - 1)].Alpha = m_fBoostPerc;
		m_nums[Math.Max(0, m_iBoostCount - 1)].WidthScale = m_nums[Math.Max(0, m_iBoostCount - 1)].GetSpriteImage().Width * 1.4f / SceneRenderer.GetCameraZoom();
		m_nums[Math.Max(m_iBoostCount - 1, 0)].Position = m_boostButton.Position + new Vector2(140f, 0f) / SceneRenderer.GetCameraZoom();
		m_nums[Math.Max(m_iBoostCount - 1, 0)].Draw(gameTime);
		for (int i = 0; i < 3; i++)
		{
			m_balls[i].Position = m_boostButton.Position - new Vector2(160f, 20 - i * 50) / SceneRenderer.GetCameraZoom();
			m_ballsGlows[i].Position = m_balls[i].Position;
			m_balls[i].WidthScale = 30f / SceneRenderer.GetCameraZoom();
			m_ballsGlows[i].WidthScale = 100f / SceneRenderer.GetCameraZoom();
			if (m_iBoostCount < 3)
			{
				m_balls[i].Draw(gameTime);
			}
			if (m_iClickOverCounter > i)
			{
				m_ballsGlows[i].Draw(gameTime);
			}
		}
		m_diveButton.Alpha = m_fDivePerc;
		m_diveButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(250f, -200f) / SceneRenderer.GetCameraZoom();
		m_diveButton.WidthScale = m_diveButton.GetSpriteImage().Width * 1.4f / SceneRenderer.GetCameraZoom();
		m_diveButton.Draw(gameTime);
	}

	public override void CollisionResponse()
	{
		m_bCanDive = true;
		if (m_iBoostCount < 3)
		{
			m_iClickOverCounter++;
			if (m_iClickOverCounter >= 3)
			{
				m_iClickOverCounter = 0;
				m_iBoostCount++;
				if (m_iBoostCount > 0)
				{
					m_bCanBoost = true;
				}
			}
		}
		m_count = "x" + m_iBoostCount;
	}

	public override void RevertAction()
	{
		m_iDiveTimer = 0;
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.X))
		{
			if (m_bCanBoost)
			{
				m_iBoostCount--;
				if (m_iBoostCount == 0)
				{
					m_bCanBoost = false;
				}
				m_count = "x" + m_iBoostCount;
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

	public override void SwapOutfit()
	{
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
	}
}
