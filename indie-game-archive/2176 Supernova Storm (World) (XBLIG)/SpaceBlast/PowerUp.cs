using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceBlast.PathFinding;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class PowerUp
{
	private const int constMinTimeBetweenAppearances = 10;

	private PowerUpType m_Type;

	private PowerupCategory m_Category;

	private bool m_RandomPosition;

	private double m_LifeTime;

	private double m_MaxTime;

	private double m_NextAppearance;

	private bool m_IsActive;

	private Vector3 m_Position;

	private double m_Expires;

	private bool m_IsGrowing;

	private bool m_IsShrinking;

	private double m_ScaleStartTime;

	private float m_Scale;

	private Model m_Model;

	private Matrix[] m_AbsoluteTransforms;

	private float m_ModelRadius;

	private float m_XSpin;

	private float m_YSpin;

	private float m_ZSpin;

	private Random m_Random;

	private int m_PowerupID;

	public bool IsActive => m_IsActive;

	public Vector3 Position => m_Position;

	public PowerUpType Type => m_Type;

	public PowerUp(XmlNode node, int powerupID)
	{
		m_PowerupID = powerupID;
		m_Random = new Random(GetHashCode() + DateTime.Now.Millisecond);
		m_IsActive = false;
		m_IsGrowing = false;
		m_IsShrinking = false;
		m_ScaleStartTime = 0.0;
		m_Scale = 1f;
		m_XSpin = 0f;
		m_YSpin = 0f;
		m_ZSpin = 0f;
		m_NextAppearance = double.MaxValue;
		string value = node.Attributes["type"].Value;
		m_Type = StringToPowerUpType(value);
		m_Category = CategoryFromType(m_Type);
		m_MaxTime = Convert.ToInt32(node.Attributes["maxtime"].Value);
		m_LifeTime = Convert.ToInt32(node.Attributes["lifetime"].Value);
		m_RandomPosition = false;
		if (node.Attributes["randompos"].Value == "true")
		{
			m_RandomPosition = true;
		}
		string value2 = node.Attributes["position"].Value;
		m_Position = Utils.StringToVector3(value2);
		if (!MainGame.NetMan.IsNetworkGame || MainGame.NetMan.IsHost)
		{
			ScheduleNextAppearance();
		}
		BoundingSphere[] collisionSpheres = null;
		Utils.LoadModelFile(ModelNameFromType(m_Type), out m_Model, out m_AbsoluteTransforms, ref collisionSpheres);
		Utils.PrepareBasicEffectModel(m_Model);
		m_ModelRadius = m_Model.Meshes[0].BoundingSphere.Transform(m_AbsoluteTransforms[0]).Radius;
	}

	private PowerUpType StringToPowerUpType(string strValue)
	{
		switch (strValue)
		{
		case "Acceleration":
			return PowerUpType.Acceleration;
		case "TopSpeed":
			return PowerUpType.TopSpeed;
		case "Refuel":
			return PowerUpType.Refuel;
		case "Repair":
			return PowerUpType.Repair;
		case "ShieldBoost":
			return PowerUpType.ShieldBoost;
		case "ShieldRegenRate":
			return PowerUpType.ShieldRegenRate;
		case "FrontAmmo":
			return PowerUpType.FrontAmmo;
		case "FrontGun":
		case "Gun":
			return PowerUpType.FrontGun;
		case "Blaster":
			return PowerUpType.FrontBlaster;
		case "VBlaster":
			return PowerUpType.FrontVBlaster;
		case "RearGun":
			return PowerUpType.RearGun;
		case "FrontBlaster":
			return PowerUpType.FrontBlaster;
		case "FrontVBlaster":
			return PowerUpType.FrontVBlaster;
		case "IncreaseFireRate":
			return PowerUpType.IncreaseFireRate;
		case "MegaDamage":
			return PowerUpType.MegaDamage;
		case "Starburst":
			return PowerUpType.Starburst;
		case "Shockwave":
			return PowerUpType.Shockwave;
		case "EMP":
			return PowerUpType.EMP;
		case "Cloak":
			return PowerUpType.Cloak;
		case "Invincible":
			return PowerUpType.Invincible;
		default:
			return PowerUpType.epowUnknown;
		}
	}

	private string ModelNameFromType(PowerUpType value)
	{
		return value switch
		{
			PowerUpType.Acceleration => "PU_Acceleration", 
			PowerUpType.TopSpeed => "PU_TopSpeed", 
			PowerUpType.Refuel => "PU_Refuel", 
			PowerUpType.Repair => "PU_Health", 
			PowerUpType.ShieldBoost => "PU_ShieldBoost", 
			PowerUpType.ShieldRegenRate => "PU_ShieldRegenRate", 
			PowerUpType.FrontAmmo => "PU_Ammo2", 
			PowerUpType.FrontGun => "PU_Gun2", 
			PowerUpType.RearGun => "PU_RearGun2", 
			PowerUpType.FrontBlaster => "PU_Blaster2", 
			PowerUpType.FrontVBlaster => "PU_VBlaster", 
			PowerUpType.IncreaseFireRate => "PU_RapidFire2", 
			PowerUpType.MegaDamage => "PU_MegaDamage2", 
			PowerUpType.Starburst => "PU_Starburst", 
			PowerUpType.Shockwave => "PU_Shockwave", 
			PowerUpType.EMP => "PU_EMP", 
			PowerUpType.Cloak => "PU_Cloak", 
			PowerUpType.Invincible => "PU_Invincible", 
			_ => "Unknown", 
		};
	}

	private string ScreenNameFromType(PowerUpType value)
	{
		return value switch
		{
			PowerUpType.Acceleration => "n Acceleration", 
			PowerUpType.TopSpeed => " Top Speed", 
			PowerUpType.Refuel => " Refuel", 
			PowerUpType.Repair => " Health Pack", 
			PowerUpType.ShieldBoost => " Shield Boost", 
			PowerUpType.ShieldRegenRate => " Shield Regeneration Rate", 
			PowerUpType.FrontAmmo => "n Ammo", 
			PowerUpType.FrontGun => " Gun Upgrade", 
			PowerUpType.RearGun => " Rear Gun", 
			PowerUpType.FrontBlaster => " Blaster Upgrade", 
			PowerUpType.FrontVBlaster => " VBlaster Upgrade", 
			PowerUpType.IncreaseFireRate => " Rapid Fire", 
			PowerUpType.MegaDamage => " Mega Damage", 
			PowerUpType.Starburst => " Starburst Mega Weapon", 
			PowerUpType.Shockwave => " Shockwave Mega Weapon", 
			PowerUpType.EMP => "EMP Mega Weapon", 
			PowerUpType.Cloak => " Cloak", 
			PowerUpType.Invincible => "n Invincibility", 
			_ => "n Unknown", 
		};
	}

	public void Update()
	{
		if (m_IsActive)
		{
			if (m_IsGrowing)
			{
				m_Scale = (float)(TimeManager.TotalSeconds - m_ScaleStartTime) / 5f;
				if (m_Scale > 1f)
				{
					m_Scale = 1f;
					m_IsGrowing = false;
				}
			}
			else if (m_IsShrinking)
			{
				m_Scale = 1f - (float)(TimeManager.TotalSeconds - m_ScaleStartTime) / 5f;
				if (m_Scale < 0f)
				{
					m_Scale = 0f;
					m_IsShrinking = false;
					m_IsActive = false;
					ScheduleNextAppearance();
				}
			}
			else if (TimeManager.TotalSeconds > m_Expires)
			{
				m_IsShrinking = true;
				m_ScaleStartTime = TimeManager.TotalSeconds;
				m_Scale = 1f;
			}
		}
		else if (TimeManager.TotalSeconds > m_NextAppearance)
		{
			m_IsActive = true;
			m_IsGrowing = true;
			m_ScaleStartTime = TimeManager.TotalSeconds;
			m_Scale = 0f;
			m_XSpin = (float)m_Random.Next(100) / 100f;
			m_YSpin = (float)m_Random.Next(100) / 200f;
			m_ZSpin = (float)m_Random.Next(100) / 300f;
			MainGame.AudioMan.Play(Sound.PowerUpAppear, m_Position);
		}
	}

	private void ScheduleNextAppearance()
	{
		if (!MainGame.NetMan.IsNetworkGame || MainGame.NetMan.IsHost)
		{
			int num = 10 + m_Random.Next((int)m_MaxTime - 10);
			m_NextAppearance = TimeManager.TotalSeconds + (double)num;
			m_Expires = m_NextAppearance + m_LifeTime;
			if (m_RandomPosition)
			{
				int index = m_Random.Next(MainGame.LevelData.Waypoints.Count - 1);
				Waypoint waypoint = MainGame.LevelData.Waypoints[index];
				m_Position = new Vector3(waypoint.Position, 0f);
			}
			if (MainGame.NetMan.IsNetworkGame)
			{
				MainGame.NetMan.SendShowPowerupPacket(m_PowerupID, m_NextAppearance, m_Position);
			}
		}
	}

	public void SetNextAppearanceTime(double when, Vector3 where)
	{
		m_IsActive = false;
		m_Scale = 0f;
		m_IsShrinking = false;
		m_NextAppearance = when;
		m_Expires = m_NextAppearance + m_LifeTime;
		m_Position = where;
	}

	public void Draw()
	{
		if (!m_IsActive)
		{
			return;
		}
		Matrix matrix = Matrix.CreateScale(m_Scale) * Matrix.CreateRotationY((float)TimeManager.TotalSeconds * m_YSpin) * Matrix.CreateRotationZ((float)TimeManager.TotalSeconds * m_ZSpin) * Matrix.CreateRotationX((float)TimeManager.TotalSeconds * m_XSpin) * Matrix.CreateTranslation(m_Position);
		foreach (ModelMesh mesh in m_Model.Meshes)
		{
			foreach (BasicEffect effect in mesh.Effects)
			{
				effect.World = m_AbsoluteTransforms[mesh.ParentBone.Index] * matrix;
				effect.View = MainGame.ViewMatrix;
			}
			mesh.Draw();
		}
	}

	public BoundingSphere GetBoundingSphere()
	{
		return new BoundingSphere(m_Position, m_Model.Meshes[0].BoundingSphere.Radius * m_Scale);
	}

	public void PlayerCollisionTest(LocalPlayer player, BoundingSphere shipSphere)
	{
		BoundingSphere sphere = new BoundingSphere(m_Position, m_ModelRadius);
		if (shipSphere.Intersects(sphere))
		{
			ApplyPowerup(player);
			if (MainGame.NetMan.IsNetworkGame)
			{
				MainGame.NetMan.SendPowerupCollectedPacket(m_PowerupID, player.PlayerID);
			}
			ScheduleNextAppearance();
		}
	}

	public void ApplyPowerup(Player player)
	{
		Sound sound = Sound.PowerUpCollected;
		LocalPlayer localPlayer = null;
		if (player is LocalPlayer)
		{
			localPlayer = (LocalPlayer)player;
		}
		switch (m_Type)
		{
		case PowerUpType.Acceleration:
			if (localPlayer != null)
			{
				localPlayer.TheShip.Acceleration++;
			}
			MainGame.DebugMsg = "Acceleration Powerup";
			break;
		case PowerUpType.TopSpeed:
			if (localPlayer != null)
			{
				localPlayer.TheShip.MaxSpeed += 50f;
			}
			MainGame.DebugMsg = "Max Speed Powerup";
			break;
		case PowerUpType.Refuel:
			if (localPlayer != null)
			{
				localPlayer.TheShip.Fuel += 150f;
			}
			MainGame.DebugMsg = "Refuel";
			break;
		case PowerUpType.Repair:
			if (localPlayer != null)
			{
				localPlayer.TheShip.Strength = MathHelper.Clamp(localPlayer.TheShip.Strength + 50f, 0f, 100f);
			}
			MainGame.DebugMsg = "Repair Powerup";
			break;
		case PowerUpType.ShieldBoost:
			localPlayer?.TheShip.ShieldBoost(50f);
			MainGame.DebugMsg = "Shieldboost Powerup";
			break;
		case PowerUpType.ShieldRegenRate:
			if (localPlayer != null)
			{
				localPlayer.TheShip.ShieldRegenRate = MathHelper.Min(localPlayer.TheShip.ShieldRegenRate + 1f, 8f);
			}
			MainGame.DebugMsg = "Boost Shield Regen Rate Powerup";
			break;
		case PowerUpType.FrontAmmo:
			localPlayer?.TheShip.Weapons.ApplyAmmoPack();
			MainGame.DebugMsg = "Front Ammo Power Up";
			break;
		case PowerUpType.FrontGun:
			localPlayer?.TheShip.Weapons.WeaponPickedUp(WeaponType.Gun);
			MainGame.DebugMsg = "Add Front Gun Powerup";
			break;
		case PowerUpType.RearGun:
			localPlayer?.TheShip.Weapons.RearWeaponPickedUp();
			MainGame.DebugMsg = "Add Rear Gun Powerup";
			break;
		case PowerUpType.FrontBlaster:
			localPlayer?.TheShip.Weapons.WeaponPickedUp(WeaponType.Blaster);
			MainGame.DebugMsg = "Add Front Blaster Powerup";
			break;
		case PowerUpType.FrontVBlaster:
			localPlayer?.TheShip.Weapons.WeaponPickedUp(WeaponType.VBlaster);
			MainGame.DebugMsg = "Add Front VBlaster Powerup";
			break;
		case PowerUpType.IncreaseFireRate:
			localPlayer?.TheShip.Weapons.IncreaseFrontFireRate();
			MainGame.DebugMsg = "Increase Fire Rate";
			break;
		case PowerUpType.MegaDamage:
			player.ApplyMegaDamage(45f);
			MainGame.DebugMsg = "Mega Damage Power up";
			sound = Sound.PUMegaDamage;
			break;
		case PowerUpType.Starburst:
			localPlayer?.TheShip.Weapons.SpecialWeaponPickedUp(SpecialWeaponType.Starburst);
			break;
		case PowerUpType.Shockwave:
			localPlayer?.TheShip.Weapons.SpecialWeaponPickedUp(SpecialWeaponType.ShockWave);
			break;
		case PowerUpType.EMP:
			localPlayer?.TheShip.Weapons.SpecialWeaponPickedUp(SpecialWeaponType.EMP);
			break;
		case PowerUpType.Cloak:
			player.ApplyCloak(30f);
			MainGame.DebugMsg = "Cloak Powerup";
			sound = Sound.Cloak;
			break;
		case PowerUpType.Invincible:
			player.MakeInvincible(30f);
			MainGame.DebugMsg = "Invincible Powerup";
			break;
		default:
			MainGame.DebugMsg = "ERROR: Unknown Powerup!";
			break;
		}
		if (MainGame.Instance.IsWithinAudibleRange(m_Position))
		{
			MainGame.AudioMan.Play(sound, m_Position);
		}
		if (player is HumanPlayer)
		{
			MainGame.Instance.AddToMessageWindow("You collected a" + ScreenNameFromType(m_Type) + " powerup");
		}
		switch (m_Category)
		{
		case PowerupCategory.Defensive:
			MainGame.ParticleMan.CreateBluePowerUpPlasma(m_Position);
			break;
		case PowerupCategory.Offensive:
			MainGame.ParticleMan.CreateRedPowerUpPlasma(m_Position);
			break;
		default:
			MainGame.ParticleMan.CreateGreenPowerUpPlasma(m_Position);
			break;
		}
		m_IsActive = false;
		m_IsGrowing = false;
		m_IsShrinking = false;
		m_NextAppearance = 2147483647.0;
	}

	public static PowerupCategory CategoryFromType(PowerUpType type)
	{
		switch (type)
		{
		case PowerUpType.Repair:
		case PowerUpType.ShieldBoost:
		case PowerUpType.ShieldRegenRate:
		case PowerUpType.Invincible:
			return PowerupCategory.Defensive;
		case PowerUpType.FrontAmmo:
		case PowerUpType.FrontGun:
		case PowerUpType.RearGun:
		case PowerUpType.FrontBlaster:
		case PowerUpType.FrontVBlaster:
		case PowerUpType.IncreaseFireRate:
		case PowerUpType.MegaDamage:
		case PowerUpType.Starburst:
		case PowerUpType.Shockwave:
		case PowerUpType.EMP:
			return PowerupCategory.Offensive;
		default:
			return PowerupCategory.Other;
		}
	}
}
