using System.Collections.Generic;
using MathTools;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;
using Screens;

namespace PlayObjects;

public class DirectionJetController : PlayerController
{
	private const int PARTICLE_SPAWN_TIME = 30;

	private const int LEAP_TIME = 700;

	private int m_iLeapTimer;

	private bool m_bCanBoost;

	private SpriteInstance m_boostButton;

	private SpriteInstance m_meterBG;

	private SpriteInstance m_meter;

	private float m_fBoostPerc;

	private Player m_player;

	private List<PhysicalRepresentation> m_objs;

	private Prop m_outfit;

	private SpriteInstance m_jet;

	private ParticleEmitter m_jetEmitter;

	public DirectionJetController(Player p)
	{
		m_player = p;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_fBoostPerc = 0f;
		m_iLeapTimer = 0;
		m_bCanBoost = false;
		m_boostButton = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(1, 152, 255, 50), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_boostButton.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/boostDive3Norm");
		m_boostButton.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_meterBG = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(172, 55, 79, 45), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_meter = TextureContainer.GetSprite("images/Launcher/boostDive3", new Rectangle(176, 111, 71, 37), default(Vector2), DepthConsts.LOGO_DEPTH);
		m_jet = TextureContainer.GetSprite("images/spritesheets/outfitPieces", new Rectangle(841, 785, 82, 114), default(Vector2), m_player.GetProp().GetOutfit().GetSprites()[0].Depth + 0.0001f);
		m_jet.WidthScale *= 0.7f;
		m_jet.Origin = new Vector2(20f, -10f);
		m_jetEmitter = new ParticleEmitter(TextureContainer.GetImage("images/particle"), m_jet.Position, m_jet.Depth - 0.01f, fades: true, additive: true, Color.Red, Color.Orange, Color.Yellow, Color.LightYellow, default(Vector2), -0.0001f, default(Vector2), 300, 600, 0f, 0f, 0f, 0f, default(Vector2), 10f, 50f, 70f, 0f, 30);
	}

	public override void Reset()
	{
		m_iLeapTimer = 0;
		m_bCanBoost = false;
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_jet.Depth = m_outfit.GetOutfit().GetSprites()[0].Depth + 0.0001f;
	}

	public override void Update(TimeTracker gameTime, float modPow, float modBoost, Vector2 averageVel)
	{
	}

	public override void Draw(TimeTracker gameTime)
	{
		if (SceneRenderer.GetEffectMode() != m_ilastEffectMode)
		{
			SwapVirtual(m_boostButton);
			SwapVirtual(m_meter);
			SwapVirtual(m_meterBG);
			m_ilastEffectMode = 1 - m_ilastEffectMode;
		}
		m_jet.Position = m_player.Position;
		m_jet.Rotation = m_player.GetProp().GetOutfit().GetSprites()[0].Rotation;
		m_jet.Draw(gameTime);
		if (m_bCanBoost)
		{
			m_fBoostPerc += gameTime.FractionOfSecond * 2f;
			if (m_fBoostPerc > 1f)
			{
				m_fBoostPerc = 1f;
			}
		}
		else
		{
			m_fBoostPerc -= gameTime.FractionOfSecond * 2f;
			if (m_fBoostPerc <= 0f)
			{
				m_fBoostPerc = 0f;
			}
		}
		m_meter.Alpha = m_fBoostPerc;
		m_meterBG.Alpha = m_fBoostPerc;
		m_boostButton.Alpha = m_fBoostPerc;
		m_boostButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(250f, -260f) / SceneRenderer.GetCameraZoom();
		m_boostButton.WidthScale = 340f / SceneRenderer.GetCameraZoom();
		m_boostButton.Draw(gameTime);
		m_meterBG.Position = m_boostButton.Position + new Vector2(0f, 70f) / SceneRenderer.GetCameraZoom();
		m_meter.Position = m_boostButton.Position + new Vector2(0f, 70f) / SceneRenderer.GetCameraZoom();
		m_meter.WidthScale = 71f / SceneRenderer.GetCameraZoom();
		m_meterBG.WidthScale = 79f / SceneRenderer.GetCameraZoom();
		m_meterBG.Draw(gameTime);
		m_meter.SurfaceScale = new Vector2(71f * ((float)(700 - m_iLeapTimer) / 700f), 37f) / SceneRenderer.GetCameraZoom();
		m_meter.Draw(gameTime);
	}

	public override void CollisionResponse()
	{
		m_bCanBoost = true;
		m_iLeapTimer = 0;
	}

	public override void RevertAction()
	{
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		Vector2 vector = ControlManager.LeftStick(ControlManager.ActiveMenuIndex);
		vector.X = 0f;
		if (!m_bCanBoost || !(vector.LengthSquared() >= 0.09f))
		{
			return;
		}
		m_iLeapTimer += gameTime.ElapsedMilli;
		if (m_iLeapTimer > 700)
		{
			m_bCanBoost = false;
			return;
		}
		m_jetEmitter.Position = m_jet.Position + VectorTools.Rotate(new Vector2(-20f, 20f), m_jet.Rotation);
		m_jetEmitter.Update(gameTime);
		float num = 1500f;
		vector.Normalize();
		vector.Y = 0f - vector.Y;
		for (int i = 0; i < m_objs.Count; i++)
		{
			m_objs[i].Velocity += gameTime.FractionOfSecond * vector * num;
		}
	}

	public override void SwapOutfit()
	{
		m_outfit = m_player.GetProp();
		m_objs = m_outfit.GetOutfit().GetPhysicsObjects();
		m_jet.Depth = m_outfit.GetOutfit().GetSprites()[0].Depth + 0.0001f;
	}
}
