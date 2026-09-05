using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using MathTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysicsHandler;
using Renderer;

namespace PlayObjects;

public class DartLaunchController : PlayerController
{
	private const int PARTICLE_SPAWN_TIME = 60;

	private const int LEAP_TIME = 500;

	private const int DIVE_TIME = 1000;

	private const int DART_TIMER = 400;

	private int m_iLeapTimer;

	private int m_iDiveTimer;

	private bool m_bCanBoost;

	private bool m_bCanDive;

	private SpriteInstance m_diveButton;

	private SpriteInstance m_boostButton;

	private SpriteInstance m_fireButton;

	private float m_fBoostPerc;

	private float m_fDivePerc;

	private int m_iParticleTimer;

	private Player m_player;

	private List<PhysicalRepresentation> m_objs;

	private Prop m_outfit;

	private int m_iDartTimer;

	private List<PhysicalRepresentation> m_darts;

	private List<SpriteInstance> m_sprites;

	private int m_iDartIndex;

	private SpriteInstance m_cannon;

	public DartLaunchController(Player p)
	{
		m_iDartTimer = 0;
		m_darts = new List<PhysicalRepresentation>();
		m_sprites = new List<SpriteInstance>();
		m_iDartIndex = 0;
		for (int i = 0; i < 30; i++)
		{
			m_darts.Add(PhysicsObjectManager.CreatePhysicalRepresentation(20, default(Vector2), PhysicsObjectManager.WallCollisionGroup(), scale: true));
			m_sprites.Add(TextureContainer.GetSprite("images/ball", default(Vector2), 100f));
			m_sprites.Last().GetSpriteImage().GetSpritePage()
				.NormTex = TextureContainer.GetTexture("images/ballNorm");
			m_sprites.Last().GetSpriteImage().GetSpritePage()
				.SpecTex = TextureContainer.GetTexture("images/whitesquare");
			m_sprites.Last().WidthScale = 40f;
			m_darts.Last().SetCollisionHandler(CollisionHandler);
			m_darts[i].Position = new Vector2(-1000 + -200 * i, -1000f);
			m_darts[i].Static = true;
			m_darts[i].Enabled = false;
			m_sprites[i].Position = new Vector2(-1000f, -1000f);
		}
		m_player = p;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_fDivePerc = 0f;
		m_fBoostPerc = 0f;
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_bCanBoost = false;
		m_bCanDive = false;
		m_boostButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 1, 150, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_boostButton.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/boostDive3Norm");
		m_boostButton.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_diveButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 103, 155, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_fireButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 203, 227, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_iParticleTimer = 0;
		m_cannon = TextureContainer.GetSprite("images/spritesheets/outfitPieces", new Rectangle(710, 918, 95, 61), default(Vector2), m_outfit.GetOutfit().GetSprites()[0].Depth + 0.0001f);
	}

	public bool CollisionHandler(Fixture f1, Fixture f2, Contact contactList)
	{
		if (f2.CollisionFilter.CollisionCategories == PhysicsObjectManager.PlayerCollisionGroup())
		{
			return false;
		}
		return true;
	}

	public override void Reset()
	{
		m_iDartTimer = 0;
		m_iDartIndex = 0;
		for (int i = 0; i < m_darts.Count; i++)
		{
			m_darts[i].Position = new Vector2(-1000 + -200 * i, -1000f);
			m_darts[i].Static = true;
			m_darts[i].Enabled = false;
			m_sprites[i].Position = new Vector2(-1000f, -1000f);
		}
		m_iLeapTimer = 0;
		m_iDiveTimer = 0;
		m_bCanBoost = false;
		m_bCanDive = false;
		m_iParticleTimer = 0;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_cannon.Depth = m_outfit.GetOutfit().GetSprites()[0].Depth + 0.0001f;
	}

	public override void Update(TimeTracker gameTime, float modPow, float modBoost, Vector2 averageVel)
	{
		if (m_iDartTimer > 0)
		{
			m_iDartTimer -= gameTime.ElapsedMilli;
		}
		for (int i = 0; i < m_darts.Count; i++)
		{
			m_sprites[i].Position = m_darts[i].Position;
		}
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
						for (int j = 0; j < m_objs.Count; j++)
						{
							float num2 = Math.Max((modPow - Math.Max(0f, averageVel.X)) / modPow, 0f) * modBoost;
							Vector2 vector = new Vector2(1f, -0.8f);
							vector.Normalize();
							vector.X *= num2;
							if (averageVel.Y > 0f)
							{
								m_objs[j].Velocity = new Vector2(m_objs[j].Velocity.X, 0f);
							}
							m_objs[j].Velocity += gameTime.FractionOfSecond * vector * num;
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
				for (int k = 0; k < m_objs.Count; k++)
				{
					m_objs[k].Velocity += gameTime.FractionOfSecond * new Vector2(0f, 1f) * num3;
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
			SwapVirtual(m_fireButton);
			m_ilastEffectMode = 1 - m_ilastEffectMode;
		}
		m_cannon.Position = m_player.Position;
		m_cannon.Draw(gameTime);
		for (int i = 0; i < m_sprites.Count; i++)
		{
			m_sprites[i].Draw(gameTime);
		}
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
		m_boostButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(300f, -260f) / SceneRenderer.GetCameraZoom();
		m_diveButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(300f, -200f) / SceneRenderer.GetCameraZoom();
		m_fireButton.WidthScale = 300f / SceneRenderer.GetCameraZoom();
		m_fireButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(300f, -140f) / SceneRenderer.GetCameraZoom();
		m_boostButton.Draw(gameTime);
		m_diveButton.Draw(gameTime);
		m_fireButton.Draw(gameTime);
	}

	public override void CollisionResponse()
	{
		m_bCanBoost = true;
		m_bCanDive = true;
	}

	public override void RevertAction()
	{
		m_iDiveTimer = 0;
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		Vector2 vector = ControlManager.LeftStick(ControlManager.ActiveMenuIndex);
		if (vector.LengthSquared() > 0.25f)
		{
			vector.Y = 0f - vector.Y;
			m_cannon.Rotation = VectorTools.GetAngleFromVector(vector);
			if (m_iDartTimer <= 0)
			{
				m_iDartTimer = 400;
				vector.Normalize();
				m_darts[m_iDartIndex].Position = m_player.Position;
				m_darts[m_iDartIndex].Enabled = true;
				m_darts[m_iDartIndex].Static = false;
				m_darts[m_iDartIndex].Velocity = m_player.GetVel() + vector * 2000f;
				m_darts[m_iDartIndex].Mass = 0.8f;
				m_iDartIndex++;
				if (m_iDartIndex >= m_darts.Count)
				{
					m_iDartIndex = 0;
				}
			}
		}
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.X))
		{
			if (m_bCanBoost)
			{
				m_bCanBoost = false;
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
		m_cannon.Depth = m_outfit.GetOutfit().GetSprites()[0].Depth + 0.0001f;
	}
}
