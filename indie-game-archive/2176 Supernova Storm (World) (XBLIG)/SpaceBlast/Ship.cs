using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class Ship
{
	private const float constRotationSpeed = 0.1f;

	private const float constShipScale = 1.2f;

	private ShipColor m_Colour;

	public Vector3 Velocity = Vector3.Zero;

	public float Acceleration = 3f;

	public float MaxSpeed = 250f;

	private float m_TargetSpeed;

	private float m_Fuel;

	private float m_CurrentShields;

	private float m_MaxShields;

	private float m_ShieldRegenRate;

	private float m_CurrentStrength;

	private float m_MaxStrength;

	private float m_FrontFireDelay;

	public readonly WeaponSystem Weapons;

	public Matrix RotationMatrix = Matrix.CreateRotationX((float)Math.PI / 2f);

	private Model m_Model;

	private Matrix[] Transforms;

	private float m_ModelRadius;

	private SpriteBatch m_SpriteBatch;

	private Texture2D m_TeamHaze;

	private Texture2D m_ModelTexture;

	private Effect m_LightingFX;

	private EffectParameter m_LightingFXWorld;

	private EffectParameter m_LightingFXCamera;

	private EffectParameter m_LightingFXTintColor;

	private EffectParameter m_LightingFXTexture;

	private Color m_AmbientColor = Color.White;

	private Vector3 m_Position = Vector3.Zero;

	private float m_TargetRotation;

	private float m_Rotation;

	private Player m_OwningPlayer;

	public ShipColor Colour
	{
		get
		{
			return m_Colour;
		}
		set
		{
			m_Colour = value;
			ReloadShipTexture();
		}
	}

	public float Fuel
	{
		get
		{
			return m_Fuel;
		}
		set
		{
			m_Fuel = MathHelper.Min(200f, value);
		}
	}

	public float Shields
	{
		get
		{
			return m_CurrentShields;
		}
		set
		{
			m_CurrentShields = MathHelper.Min(m_MaxShields, value);
		}
	}

	public float ShieldRegenRate
	{
		get
		{
			return m_ShieldRegenRate;
		}
		set
		{
			m_ShieldRegenRate = MathHelper.Min(20f, value);
		}
	}

	public float Strength
	{
		get
		{
			return m_CurrentStrength;
		}
		set
		{
			m_CurrentStrength = MathHelper.Min(m_MaxStrength, value);
		}
	}

	public float FrontFireDelay
	{
		get
		{
			return m_FrontFireDelay;
		}
		set
		{
			m_FrontFireDelay = MathHelper.Max(0.1f, value);
		}
	}

	public float TargetSpeed
	{
		get
		{
			return m_TargetSpeed;
		}
		set
		{
			m_TargetSpeed = value;
		}
	}

	public float Rotation
	{
		get
		{
			return m_Rotation;
		}
		set
		{
			m_Rotation = Utils.NormaliseAngle(value);
			RecalcRotationMatrix();
		}
	}

	public float TargetRotation
	{
		get
		{
			return m_TargetRotation;
		}
		set
		{
			m_TargetRotation = Utils.NormaliseAngle(value);
		}
	}

	public Vector3 Position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	public float Radius => m_ModelRadius;

	public float Diameter => m_ModelRadius * 2f;

	public float FrontFireRate => 1f / m_FrontFireDelay;

	public Ship(Vector3 position, Player owningPlayer, ShipColor colour)
	{
		m_OwningPlayer = owningPlayer;
		Weapons = new WeaponSystem(owningPlayer);
		m_Position = position;
		m_Model = MainGame.ContentMan.Load<Model>("Models/p1_wedge");
		m_ModelTexture = MainGame.ContentMan.Load<Texture2D>("Textures/" + GetTextureNameForColour(colour));
		m_LightingFX = MainGame.ContentMan.Load<Effect>("Effects/Ship");
		m_TeamHaze = MainGame.ContentMan.Load<Texture2D>("Textures/Ship_Haze");
		m_SpriteBatch = new SpriteBatch(MainGame.Instance.GraphicsDevice);
		Reset();
		Transforms = new Matrix[m_Model.Bones.Count];
		m_Model.CopyAbsoluteBoneTransformsTo(Transforms);
		m_Colour = colour;
		foreach (ModelMesh mesh in m_Model.Meshes)
		{
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Effect = m_LightingFX;
				meshPart.Effect.Parameters["Projection"].SetValue(MainGame.ProjectionMatrix);
				meshPart.Effect.Parameters["MeshTexture"].SetValue(m_ModelTexture);
			}
		}
		m_LightingFXWorld = m_LightingFX.Parameters["World"];
		m_LightingFXCamera = m_LightingFX.Parameters["cameraPosition"];
		m_LightingFXTintColor = m_LightingFX.Parameters["TintColor"];
		m_LightingFXTexture = m_LightingFX.Parameters["MeshTexture"];
		m_ModelRadius = m_Model.Meshes[0].BoundingSphere.Transform(Transforms[0]).Radius * 1.2f * 0.8f;
	}

	private void ReloadShipTexture()
	{
		m_ModelTexture = MainGame.ContentMan.Load<Texture2D>("Textures/" + GetTextureNameForColour(m_Colour));
	}

	public Vector3 GetPredictedPos(float when)
	{
		return m_Position + Velocity * 60f * when;
	}

	public void SetPredictedPos(Vector3 pos, float when)
	{
		Velocity = (pos - Position) / (when * 60f);
	}

	public void UpdateRemoteShip()
	{
		float num = (float)TimeManager.DeltaSeconds;
		Weapons.Update();
		m_Position += Velocity * 60f * num;
		float angle = m_Rotation - m_TargetRotation;
		Utils.NormaliseAngle(ref angle);
		if ((float)Math.Abs((double)angle) < 0.1f)
		{
			Rotation = m_TargetRotation;
		}
		else if (angle < (float)Math.PI)
		{
			Rotation -= 0.1f;
		}
		else
		{
			Rotation += 0.1f;
		}
	}

	public void UpdateAIShip()
	{
		float num = (float)TimeManager.DeltaSeconds;
		Weapons.Update();
		if (!m_OwningPlayer.IsPowerCut)
		{
			float angle = m_Rotation - m_TargetRotation;
			Utils.NormaliseAngle(ref angle);
			if ((float)Math.Abs((double)angle) < 0.1f)
			{
				Rotation = m_TargetRotation;
			}
			else if (angle < (float)Math.PI)
			{
				Rotation -= 0.1f;
			}
			else
			{
				Rotation += 0.1f;
			}
		}
		float num2 = 0f;
		float num3 = Velocity.Length();
		num2 = ((!(num3 >= m_TargetSpeed) && !m_OwningPlayer.IsPowerCut) ? (1f - num3 / m_TargetSpeed) : 0f);
		float num4 = num2 * num;
		if (num4 * 1f > m_Fuel)
		{
			num4 = m_Fuel / 1f;
			m_Fuel = 0f;
		}
		else
		{
			m_Fuel -= num4 * 1f;
		}
		Velocity += RotationMatrix.Forward * Acceleration * num4 * 60f;
		Velocity *= 59.4f * num;
		if (Velocity.Length() > MaxSpeed)
		{
			Velocity.Normalize();
			Velocity *= MaxSpeed;
		}
		if (num2 > 0.1f)
		{
			MainGame.AudioMan.PlayEngines(m_OwningPlayer, Velocity.Length(), num2, ref m_Position);
		}
		else
		{
			MainGame.AudioMan.StopEngines(m_OwningPlayer);
		}
		m_Position += Velocity * 60f * num;
		m_Position += GameConstants.Gravity * num;
		if (m_CurrentShields < m_MaxShields)
		{
			m_CurrentShields = Math.Min(m_MaxShields, m_CurrentShields + m_ShieldRegenRate * num);
		}
	}

	public void UpdateHumanShip(GamePadState padState, KeyboardState keyState)
	{
		float num = (float)TimeManager.DeltaSeconds;
		Weapons.Update();
		m_Position += Velocity * 60f * num;
		m_Position += GameConstants.Gravity * num;
		float num2 = 0f;
		if (!m_OwningPlayer.IsPowerCut)
		{
			Rotation -= padState.ThumbSticks.Left.X * 0.1f * 60f * num;
			if (keyState.IsKeyDown(Keys.Left))
			{
				Rotation += 6f * num;
			}
			if (keyState.IsKeyDown(Keys.Right))
			{
				Rotation -= 6f * num;
			}
			num2 = padState.Triggers.Right;
			if (keyState.IsKeyDown(Keys.Up))
			{
				num2 = 1f;
			}
			float num3 = num2 * num;
			if (num3 * 1f > m_Fuel)
			{
				num3 = m_Fuel / 1f;
				m_Fuel = 0f;
			}
			else
			{
				m_Fuel -= num3 * 1f;
			}
			Velocity += RotationMatrix.Forward * Acceleration * num3 * 60f;
		}
		Velocity *= 59.4f * num;
		if (Velocity.Length() > MaxSpeed)
		{
			Velocity.Normalize();
			Velocity *= MaxSpeed;
		}
		if (num2 > 0.1f)
		{
			MainGame.AudioMan.PlayEngines(m_OwningPlayer, Velocity.Length(), num2, ref m_Position);
		}
		else
		{
			MainGame.AudioMan.StopEngines(m_OwningPlayer);
		}
		if (m_CurrentShields < m_MaxShields)
		{
			m_CurrentShields = Math.Min(m_MaxShields, m_CurrentShields + m_ShieldRegenRate * num);
		}
	}

	public void Reset()
	{
		Weapons.Reset();
		Rotation = 0f;
		Velocity = Vector3.Zero;
		m_CurrentStrength = 100f;
		m_MaxStrength = 100f;
		m_CurrentShields = 100f;
		m_MaxShields = 100f;
		m_ShieldRegenRate = 5f;
		m_FrontFireDelay = 0.25f;
		Acceleration = (Guide.IsTrialMode ? 3.5f : 3f);
		MaxSpeed = (Guide.IsTrialMode ? 300f : 250f);
		m_Fuel = 1000000f;
	}

	public void Die()
	{
		MainGame.ParticleMan.CreateExplosion(Position, Velocity);
		MainGame.AudioMan.StopEngines(m_OwningPlayer);
	}

	public bool HandleCollision(ref Vector3 normal)
	{
		float num = Vector3.Dot(Vector3.Normalize(normal), Vector3.Normalize(Velocity));
		float num2 = (float)Math.Acos(num);
		float num3 = num2 / (float)Math.PI;
		if (ApplyDamage((int)(num3 * Velocity.Length()) + 1))
		{
			return true;
		}
		Velocity = Vector3.Reflect(Velocity, Vector3.Normalize(normal)) * 0.6f;
		Velocity.Z = 0f;
		Position += 2f * Velocity;
		Vector3 collisionNormal = default(Vector3);
		if (MainGame.LevelData.StaticWorldObjects.CollisionTest(GetBoundingSphere(), ref collisionNormal))
		{
			return true;
		}
		return false;
	}

	public bool ApplyDamage(int damage)
	{
		if (m_OwningPlayer.IsInvincibile)
		{
			return false;
		}
		m_CurrentShields -= damage;
		if (m_CurrentShields < 0f)
		{
			m_CurrentStrength += m_CurrentShields;
			m_CurrentShields = 0f;
			if (m_CurrentStrength <= 0f)
			{
				m_CurrentStrength = 0f;
				if (m_OwningPlayer is HumanPlayer)
				{
					((HumanPlayer)m_OwningPlayer).SetControllerVibration(2.3f, 1f, 1f);
				}
				return true;
			}
		}
		if (m_OwningPlayer is HumanPlayer)
		{
			((HumanPlayer)m_OwningPlayer).SetControllerVibration(0.3f, 0.5f, 0f);
		}
		return false;
	}

	public void Draw(bool bMakeVisible)
	{
		Weapons.Draw();
		Matrix value = Matrix.CreateScale(1.2f) * RotationMatrix * Matrix.CreateTranslation(m_Position);
		Color white = Color.White;
		if (m_OwningPlayer.Team != ETeam.None)
		{
			white.PackedValue = (uint)m_OwningPlayer.Team;
		}
		white.A = m_OwningPlayer.LightColour.A;
		float scale = 0.25f + Shields / 200f;
		m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
		Vector3 vector = MainGame.Instance.GraphicsDevice.Viewport.Project(Position, MainGame.ProjectionMatrix, MainGame.ViewMatrix, Matrix.Identity);
		Vector2 position = new Vector2
		{
			X = vector.X - (float)MainGame.Instance.GraphicsDevice.Viewport.X,
			Y = vector.Y
		};
		m_SpriteBatch.Draw(m_TeamHaze, position, null, white, 0f, new Vector2(64f, 64f), scale, SpriteEffects.None, 0f);
		m_SpriteBatch.End();
		foreach (ModelMesh mesh in m_Model.Meshes)
		{
			foreach (Effect effect in mesh.Effects)
			{
				_ = effect;
				m_LightingFXTexture.SetValue(m_ModelTexture);
				m_LightingFXWorld.SetValue(value);
				m_LightingFXCamera.SetValue(MainGame.Instance.GetCameraPos());
				Vector4 value2 = m_OwningPlayer.LightColour.ToVector4();
				if (bMakeVisible)
				{
					value2.W = ((value2.W < 0.5f) ? 0.5f : value2.W);
				}
				m_LightingFXTintColor.SetValue(value2);
			}
			mesh.Draw();
		}
	}

	public BoundingSphere GetBoundingSphere()
	{
		return new BoundingSphere(m_Position, m_ModelRadius);
	}

	private void RecalcRotationMatrix()
	{
		RotationMatrix = Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateRotationZ(m_Rotation);
	}

	public void ShieldBoost(float boost)
	{
		m_CurrentShields += boost;
		if (m_CurrentShields > m_MaxShields)
		{
			m_MaxShields = m_CurrentShields;
		}
	}

	public void GetWeaponBayPositions(Vector3 centerPos, float angle, out Vector3 frontBay, out Vector3 rearBay, out Vector3 leftBay, out Vector3 rightBay)
	{
		Matrix rotMatrix = Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateRotationZ(m_Rotation);
		GetWeaponBayPositions(ref centerPos, ref rotMatrix, out frontBay, out rearBay, out leftBay, out rightBay);
	}

	public void GetWeaponBayPositions(out Vector3 frontBay, out Vector3 rearBay, out Vector3 leftBay, out Vector3 rightBay)
	{
		GetWeaponBayPositions(ref m_Position, ref RotationMatrix, out frontBay, out rearBay, out leftBay, out rightBay);
	}

	private void GetWeaponBayPositions(ref Vector3 center, ref Matrix rotMatrix, out Vector3 frontBay, out Vector3 rearBay, out Vector3 leftBay, out Vector3 rightBay)
	{
		frontBay = center + 200f * rotMatrix.Forward;
		leftBay = center + 800f * rotMatrix.Left;
		rightBay = center + 800f * rotMatrix.Right;
		rearBay = center + 200f * rotMatrix.Backward;
	}

	private string GetTextureNameForColour(ShipColor colour)
	{
		return colour switch
		{
			ShipColor.White => "Ship_White", 
			ShipColor.Red => "Ship_Red", 
			ShipColor.Orange => "Ship_Orange", 
			ShipColor.Yellow => "Ship_Yellow", 
			ShipColor.Green => "Ship_Green", 
			ShipColor.Cyan => "Ship_Cyan", 
			ShipColor.Blue => "Ship_Blue", 
			ShipColor.Purple => "Ship_Purple", 
			_ => "", 
		};
	}
}
