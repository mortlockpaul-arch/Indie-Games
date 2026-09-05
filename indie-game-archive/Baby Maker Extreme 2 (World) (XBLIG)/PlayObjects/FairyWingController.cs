using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsHandler;
using Renderer;
using Screens;

namespace PlayObjects;

public class FairyWingController : PlayerController
{
	private const int PARTICLE_SPAWN_TIME = 60;

	private const int LEAP_TIME = 1000;

	private const int DIVE_TIME = 1000;

	private int m_iLeapTimer;

	private int m_iDiveTimer;

	private bool m_bCanBoost;

	private bool m_bCanDive;

	private SpriteInstance m_diveButton;

	private SpriteInstance m_boostButton;

	private SpriteInstance m_meter;

	private SpriteInstance m_meterBG;

	private float m_fBoostPerc;

	private float m_fDivePerc;

	private int m_iParticleTimer;

	private Player m_player;

	private List<PhysicalRepresentation> m_objs;

	private Prop m_outfit;

	private float m_modPow;

	private float m_modBoost;

	private Vector2 m_averageVel;

	private SpriteInstance m_wings;

	private ParticleEmitter m_wingsEmitter;

	public FairyWingController(Player p)
	{
		m_modPow = 0f;
		m_modBoost = 0f;
		m_averageVel = default(Vector2);
		m_player = p;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_fDivePerc = 0f;
		m_fBoostPerc = 0f;
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_bCanBoost = false;
		m_bCanDive = false;
		m_boostButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 51, 167, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_boostButton.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/boostDive3Norm");
		m_boostButton.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_diveButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 103, 155, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_meterBG = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(172, 55, 79, 45), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_meter = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(176, 111, 71, 37), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_iParticleTimer = 0;
		m_wings = TextureContainer.GetSprite("images/spritesheets/outfitPieces", new Rectangle(695, 813, 102, 74), default(Vector2), m_player.GetProp().GetOutfit().GetSprites()[0].Depth - 0.0001f);
		m_wings.WidthScale *= 0.5f;
		m_wings.Origin = new Vector2(20f, 0f);
		m_wingsEmitter = new ParticleEmitter(TextureContainer.GetImage("images/particle"), m_wings.Position, m_wings.Depth - 0.01f, fades: true, additive: true, Color.Purple, Color.Orange, Color.Lime, Color.Yellow, default(Vector2), -0.0001f, default(Vector2), 300, 600, 200f, 400f, (float)Math.PI / 2f, 1f, default(Vector2), 5f, 20f, 25f, 0f, 30);
	}

	public override void Reset()
	{
		m_modPow = 0f;
		m_modBoost = 0f;
		m_averageVel = default(Vector2);
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_bCanBoost = false;
		m_bCanDive = false;
		m_iParticleTimer = 0;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_wings.Depth = m_outfit.GetOutfit().GetSprites()[0].Depth - 0.0001f;
	}

	public override void Update(TimeTracker gameTime, float modPow, float modBoost, Vector2 averageVel)
	{
		m_modPow = modPow;
		m_modBoost = modBoost;
		m_averageVel = averageVel;
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
		float num = 2200f;
		if (m_objs[0].Position.Y < 250f)
		{
			if (m_objs[0].Velocity.Y < 1000f)
			{
				for (int i = 0; i < m_objs.Count; i++)
				{
					m_objs[i].Velocity += gameTime.FractionOfSecond * new Vector2(0f, 1f) * num;
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
			SwapVirtual(m_meter);
			SwapVirtual(m_meterBG);
			m_ilastEffectMode = 1 - m_ilastEffectMode;
		}
		m_wings.Position = m_player.Position;
		m_wings.Rotation = m_player.GetProp().GetOutfit().GetSprites()[0].Rotation;
		m_wings.Draw(gameTime);
		if (m_bCanBoost)
		{
			m_fBoostPerc += gameTime.FractionOfSecond * 5f;
			if (m_fBoostPerc > 1f)
			{
				m_fBoostPerc = 1f;
			}
			m_boostButton.WidthScale = 210f * m_fBoostPerc / SceneRenderer.GetCameraZoom();
		}
		else
		{
			float num = m_fBoostPerc - gameTime.FractionOfSecond * 5f;
			if ((double)num <= 0.001)
			{
				m_fBoostPerc = 0.001f;
				m_boostButton.WidthScale = 1f;
			}
			else
			{
				m_fBoostPerc = num;
				m_boostButton.WidthScale = 210f * m_fBoostPerc / SceneRenderer.GetCameraZoom();
			}
		}
		if (m_bCanDive)
		{
			m_fDivePerc += gameTime.FractionOfSecond * 5f;
			if (m_fDivePerc > 1f)
			{
				m_fDivePerc = 1f;
			}
			m_diveButton.WidthScale = 210f * m_fDivePerc / SceneRenderer.GetCameraZoom();
		}
		else
		{
			float num2 = m_fDivePerc - gameTime.FractionOfSecond * 5f;
			if (num2 <= 0.001f)
			{
				m_fDivePerc = 0.001f;
				m_diveButton.WidthScale = 1f;
			}
			else
			{
				m_fDivePerc = num2;
				m_diveButton.WidthScale = 210f * m_fDivePerc / SceneRenderer.GetCameraZoom();
			}
		}
		m_boostButton.Alpha = m_fBoostPerc;
		m_diveButton.Alpha = m_fDivePerc;
		m_boostButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(200f, -260f) / SceneRenderer.GetCameraZoom();
		m_diveButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(200f, -200f) / SceneRenderer.GetCameraZoom();
		m_meter.Alpha = m_fBoostPerc;
		m_meterBG.Alpha = m_fBoostPerc;
		m_meter.Position = m_boostButton.Position + new Vector2(150f, 0f) / SceneRenderer.GetCameraZoom();
		m_meterBG.Position = m_boostButton.Position + new Vector2(150f, 0f) / SceneRenderer.GetCameraZoom();
		m_meter.SurfaceScale = new Vector2(71f * ((float)m_iLeapTimer / 1000f), 37f) / SceneRenderer.GetCameraZoom();
		m_meterBG.WidthScale = m_meterBG.GetSpriteImage().Width / SceneRenderer.GetCameraZoom();
		m_meter.Draw(gameTime);
		m_meterBG.Draw(gameTime);
		m_boostButton.Draw(gameTime);
		m_diveButton.Draw(gameTime);
	}

	public override void CollisionResponse()
	{
		m_bCanBoost = true;
		m_bCanDive = true;
		m_iLeapTimer = 1000;
	}

	public override void RevertAction()
	{
		m_iDiveTimer = 0;
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.X))
		{
			if (!m_bCanBoost)
			{
				return;
			}
			if (m_iLeapTimer <= 0)
			{
				m_bCanBoost = false;
			}
			else
			{
				m_iLeapTimer -= 150;
				if (m_objs[0].Position.Y > -550f)
				{
					m_iLeapTimer -= gameTime.ElapsedMilli;
					m_wingsEmitter.Position = m_player.Position;
					m_wingsEmitter.CreateBurst(20);
					float num = 250f;
					if (m_objs[0].Velocity.Y > -1000f)
					{
						for (int i = 0; i < m_objs.Count; i++)
						{
							Vector2 vector = new Vector2(0f, -1f);
							vector.Normalize();
							m_objs[i].Velocity += vector * num;
						}
					}
				}
				m_iDiveTimer = 0;
			}
			if (m_iLeapTimer <= 0)
			{
				m_bCanBoost = false;
			}
		}
		else if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.A) && m_bCanDive)
		{
			m_bCanDive = false;
			m_iDiveTimer = 1000;
		}
	}

	public override void SwapOutfit()
	{
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_wings.Depth = m_outfit.GetOutfit().GetSprites()[0].Depth - 0.0001f;
	}
}
