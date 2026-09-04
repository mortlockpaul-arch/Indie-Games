using System;
using System.Collections.ObjectModel;
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

	public Vector3 Velocity;

	public float Acceleration;

	public float MaxSpeed;

	private float m_TargetSpeed;

	private float m_Fuel;

	private float m_CurrentShields;

	private float m_MaxShields;

	private float m_ShieldRegenRate;

	private float m_CurrentStrength;

	private float m_MaxStrength;

	private float m_FrontFireDelay;

	public readonly WeaponSystem Weapons;

	public Matrix RotationMatrix;

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

	private Color m_AmbientColor;

	private Vector3 m_Position;

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
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_Position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_Position = value;
		}
	}

	public float Radius => m_ModelRadius;

	public float Diameter => m_ModelRadius * 2f;

	public float FrontFireRate => 1f / m_FrontFireDelay;

	public unsafe Ship(Vector3 position, Player owningPlayer, ShipColor colour)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		Velocity = Vector3.Zero;
		Acceleration = 3f;
		MaxSpeed = 250f;
		RotationMatrix = Matrix.CreateRotationX((float)Math.PI / 2f);
		m_AmbientColor = Color.White;
		m_Position = Vector3.Zero;
		base._002Ector();
		m_OwningPlayer = owningPlayer;
		Weapons = new WeaponSystem(owningPlayer);
		m_Position = position;
		m_Model = MainGame.ContentMan.Load<Model>("Models/p1_wedge");
		m_ModelTexture = MainGame.ContentMan.Load<Texture2D>("Textures/" + GetTextureNameForColour(colour));
		m_LightingFX = MainGame.ContentMan.Load<Effect>("Effects/Ship");
		m_TeamHaze = MainGame.ContentMan.Load<Texture2D>("Textures/Ship_Haze");
		m_SpriteBatch = new SpriteBatch(((Game)MainGame.Instance).GraphicsDevice);
		Reset();
		Transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)m_Model.Bones).Count];
		m_Model.CopyAbsoluteBoneTransformsTo(Transforms);
		m_Colour = colour;
		Enumerator enumerator = m_Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						current2.Effect = m_LightingFX;
						current2.Effect.Parameters["Projection"].SetValue(MainGame.ProjectionMatrix);
						current2.Effect.Parameters["MeshTexture"].SetValue((Texture)(object)m_ModelTexture);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		m_LightingFXWorld = m_LightingFX.Parameters["World"];
		m_LightingFXCamera = m_LightingFX.Parameters["cameraPosition"];
		m_LightingFXTintColor = m_LightingFX.Parameters["TintColor"];
		m_LightingFXTexture = m_LightingFX.Parameters["MeshTexture"];
		BoundingSphere boundingSphere = ((ReadOnlyCollection<ModelMesh>)(object)m_Model.Meshes)[0].BoundingSphere;
		m_ModelRadius = ((BoundingSphere)(ref boundingSphere)).Transform(Transforms[0]).Radius * 1.2f * 0.8f;
	}

	private void ReloadShipTexture()
	{
		m_ModelTexture = MainGame.ContentMan.Load<Texture2D>("Textures/" + GetTextureNameForColour(m_Colour));
	}

	public Vector3 GetPredictedPos(float when)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return m_Position + Velocity * 60f * when;
	}

	public void SetPredictedPos(Vector3 pos, float when)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Velocity = (pos - Position) / (when * 60f);
	}

	public void UpdateRemoteShip()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
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
		float num3 = ((Vector3)(ref Velocity)).Length();
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
		Velocity += ((Matrix)(ref RotationMatrix)).Forward * Acceleration * num4 * 60f;
		Velocity *= 59.4f * num;
		if (((Vector3)(ref Velocity)).Length() > MaxSpeed)
		{
			((Vector3)(ref Velocity)).Normalize();
			Velocity *= MaxSpeed;
		}
		if (num2 > 0.1f)
		{
			MainGame.AudioMan.PlayEngines(m_OwningPlayer, ((Vector3)(ref Velocity)).Length(), num2, ref m_Position);
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)TimeManager.DeltaSeconds;
		Weapons.Update();
		m_Position += Velocity * 60f * num;
		m_Position += GameConstants.Gravity * num;
		float num2 = 0f;
		if (!m_OwningPlayer.IsPowerCut)
		{
			float rotation = Rotation;
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref padState)).ThumbSticks;
			Rotation = rotation - ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 0.1f * 60f * num;
			if (((KeyboardState)(ref keyState)).IsKeyDown((Keys)37))
			{
				Rotation += 6f * num;
			}
			if (((KeyboardState)(ref keyState)).IsKeyDown((Keys)39))
			{
				Rotation -= 6f * num;
			}
			GamePadTriggers triggers = ((GamePadState)(ref padState)).Triggers;
			num2 = ((GamePadTriggers)(ref triggers)).Right;
			if (((KeyboardState)(ref keyState)).IsKeyDown((Keys)38))
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
			Velocity += ((Matrix)(ref RotationMatrix)).Forward * Acceleration * num3 * 60f;
		}
		Velocity *= 59.4f * num;
		if (((Vector3)(ref Velocity)).Length() > MaxSpeed)
		{
			((Vector3)(ref Velocity)).Normalize();
			Velocity *= MaxSpeed;
		}
		if (num2 > 0.1f)
		{
			MainGame.AudioMan.PlayEngines(m_OwningPlayer, ((Vector3)(ref Velocity)).Length(), num2, ref m_Position);
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		MainGame.ParticleMan.CreateExplosion(Position, Velocity);
		MainGame.AudioMan.StopEngines(m_OwningPlayer);
	}

	public bool HandleCollision(ref Vector3 normal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(Vector3.Normalize(normal), Vector3.Normalize(Velocity));
		float num2 = (float)Math.Acos(num);
		float num3 = num2 / (float)Math.PI;
		if (ApplyDamage((int)(num3 * ((Vector3)(ref Velocity)).Length()) + 1))
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

	public unsafe void Draw(bool bMakeVisible)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		Weapons.Draw();
		Matrix value = Matrix.CreateScale(1.2f) * RotationMatrix * Matrix.CreateTranslation(m_Position);
		Color white = Color.White;
		if (m_OwningPlayer.Team != ETeam.None)
		{
			((Color)(ref white)).PackedValue = (uint)m_OwningPlayer.Team;
		}
		((Color)(ref white)).A = ((Color)(ref m_OwningPlayer.LightColour)).A;
		float num = 0.25f + Shields / 200f;
		m_SpriteBatch.Begin((SpriteBlendMode)1);
		Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		Vector3 val = ((Viewport)(ref viewport)).Project(Position, MainGame.ProjectionMatrix, MainGame.ViewMatrix, Matrix.Identity);
		Vector2 val2 = default(Vector2);
		float x = val.X;
		Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		val2.X = x - (float)((Viewport)(ref viewport2)).X;
		val2.Y = val.Y;
		m_SpriteBatch.Draw(m_TeamHaze, val2, (Rectangle?)null, white, 0f, new Vector2(64f, 64f), num, (SpriteEffects)0, 0f);
		m_SpriteBatch.End();
		Enumerator enumerator = m_Model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						_ = ((Enumerator)(ref enumerator2)).Current;
						m_LightingFXTexture.SetValue((Texture)(object)m_ModelTexture);
						m_LightingFXWorld.SetValue(value);
						m_LightingFXCamera.SetValue(MainGame.Instance.GetCameraPos());
						Vector4 value2 = ((Color)(ref m_OwningPlayer.LightColour)).ToVector4();
						if (bMakeVisible)
						{
							value2.W = ((value2.W < 0.5f) ? 0.5f : value2.W);
						}
						m_LightingFXTintColor.SetValue(value2);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public BoundingSphere GetBoundingSphere()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new BoundingSphere(m_Position, m_ModelRadius);
	}

	private void RecalcRotationMatrix()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Matrix rotMatrix = Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateRotationZ(m_Rotation);
		GetWeaponBayPositions(ref centerPos, ref rotMatrix, out frontBay, out rearBay, out leftBay, out rightBay);
	}

	public void GetWeaponBayPositions(out Vector3 frontBay, out Vector3 rearBay, out Vector3 leftBay, out Vector3 rightBay)
	{
		GetWeaponBayPositions(ref m_Position, ref RotationMatrix, out frontBay, out rearBay, out leftBay, out rightBay);
	}

	private void GetWeaponBayPositions(ref Vector3 center, ref Matrix rotMatrix, out Vector3 frontBay, out Vector3 rearBay, out Vector3 leftBay, out Vector3 rightBay)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		frontBay = center + 200f * ((Matrix)(ref rotMatrix)).Forward;
		leftBay = center + 800f * ((Matrix)(ref rotMatrix)).Left;
		rightBay = center + 800f * ((Matrix)(ref rotMatrix)).Right;
		rearBay = center + 200f * ((Matrix)(ref rotMatrix)).Backward;
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
