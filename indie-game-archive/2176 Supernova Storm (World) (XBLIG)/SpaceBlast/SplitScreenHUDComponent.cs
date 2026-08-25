using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class SplitScreenHUDComponent : DrawableGameComponent
{
	private Texture2D m_TexShieldBar;

	private Texture2D m_TexStrengthBar;

	private Texture2D m_TexGun1Box;

	private Texture2D m_TexGun2Box;

	private Texture2D m_TexGun3Box;

	private Texture2D m_TexBlaster1Box;

	private Texture2D m_TexBlaster2Box;

	private Texture2D m_TexBlaster3Box;

	private Texture2D m_TexVBlaster2Box;

	private Texture2D m_TexVBlaster3Box;

	private Texture2D m_TexVBlaster4Box;

	private Texture2D m_TexVBlaster5Box;

	private Texture2D m_TexEMPBox;

	private Texture2D m_TexStarBurstBox;

	private SpriteFont m_HUDFont;

	private SpriteFont m_HUDSmallFont;

	private SpriteBatch m_SpriteBatch;

	private Vector2 m_PosShieldBarLeft;

	private Vector2 m_PosStrengthBarLeft;

	private Vector2 m_PosKillsLeft;

	private Vector2 m_PosAmmoLeft;

	private Vector2 m_PosMegaDamageLeft;

	private Vector2 m_PosCloakLeft;

	private Vector2 m_PosInvincibleLeft;

	private Vector2 m_PosPowerCutLeft;

	private LocalPlayer m_LeftPlayer;

	private Ship m_LeftShip;

	private Vector2 m_PosShieldBarRight;

	private Vector2 m_PosStrengthBarRight;

	private Vector2 m_PosKillsRight;

	private Vector2 m_PosAmmoRight;

	private Vector2 m_PosMegaDamageRight;

	private Vector2 m_PosCloakRight;

	private Vector2 m_PosInvincibleRight;

	private Vector2 m_PosPowerCutRight;

	private float m_BarScale;

	private LocalPlayer m_RightPlayer;

	private Ship m_RightShip;

	public SplitScreenHUDComponent(Game game, bool loadContentNow)
		: base(game)
	{
		if (loadContentNow)
		{
			LoadContent();
		}
	}

	public void ShowPlayerHUD(LocalPlayer leftPlayer, LocalPlayer rightPlayer)
	{
		m_LeftPlayer = leftPlayer;
		m_LeftShip = leftPlayer.TheShip;
		m_RightPlayer = rightPlayer;
		m_RightShip = rightPlayer.TheShip;
		base.Visible = true;
	}

	protected override void LoadContent()
	{
		m_SpriteBatch = new SpriteBatch(MainGame.Instance.GraphicsDevice);
		m_TexShieldBar = MainGame.ContentMan.Load<Texture2D>("Textures/HUD_GreenBar");
		m_TexStrengthBar = MainGame.ContentMan.Load<Texture2D>("Textures/HUD_BlueBar");
		m_TexGun1Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_Gun1");
		m_TexGun2Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_Gun2");
		m_TexGun3Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_Gun3");
		m_TexBlaster1Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_Blaster1");
		m_TexBlaster2Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_Blaster2");
		m_TexBlaster3Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_Blaster3");
		m_TexVBlaster2Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_VBlaster2");
		m_TexVBlaster3Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_VBlaster3");
		m_TexVBlaster4Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_VBlaster4");
		m_TexVBlaster5Box = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_VBlaster5");
		m_TexEMPBox = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_EMP");
		m_TexStarBurstBox = MainGame.ContentMan.Load<Texture2D>("Textures/WeaponBox_StarBurst");
		m_HUDFont = MainGame.ContentMan.Load<SpriteFont>("Fonts/HUDFont");
		m_HUDSmallFont = MainGame.ContentMan.Load<SpriteFont>("Fonts/HUDSmallFont");
		if (MainGame.Is1080HD)
		{
			m_PosShieldBarLeft = new Vector2(128f, 110f);
			m_PosStrengthBarLeft = new Vector2(584f, 110f);
			m_PosKillsLeft = new Vector2(459f, 110f);
			m_PosAmmoLeft = new Vector2(504f, 110f);
			m_PosMegaDamageLeft = new Vector2(128f, 135f);
			m_PosCloakLeft = new Vector2(454f, 135f);
			m_PosInvincibleLeft = new Vector2(744f, 135f);
			m_PosPowerCutLeft = new Vector2(128f, 160f);
			Vector2 vector = new Vector2(908f, 0f);
			m_PosShieldBarRight = m_PosShieldBarLeft + vector;
			m_PosStrengthBarRight = m_PosStrengthBarLeft + vector;
			m_PosKillsRight = m_PosKillsLeft + vector;
			m_PosAmmoRight = m_PosAmmoLeft + vector;
			m_PosMegaDamageRight = m_PosMegaDamageLeft + vector;
			m_PosCloakRight = m_PosCloakLeft + vector;
			m_PosInvincibleRight = m_PosInvincibleLeft + vector;
			m_PosPowerCutRight = m_PosPowerCutLeft + vector;
			m_BarScale = 3f;
		}
		else
		{
			m_PosShieldBarLeft = new Vector2(128f, 80f);
			m_PosStrengthBarLeft = new Vector2(430f, 80f);
			m_PosKillsLeft = new Vector2(320f, 80f);
			m_PosAmmoLeft = new Vector2(360f, 80f);
			m_PosMegaDamageLeft = new Vector2(128f, 105f);
			m_PosCloakLeft = new Vector2(315f, 105f);
			m_PosInvincibleLeft = new Vector2(470f, 105f);
			m_PosPowerCutLeft = new Vector2(128f, 130f);
			Vector2 vector2 = new Vector2(552f, 0f);
			m_PosShieldBarRight = m_PosShieldBarLeft + vector2;
			m_PosStrengthBarRight = m_PosStrengthBarLeft + vector2;
			m_PosKillsRight = m_PosKillsLeft + vector2;
			m_PosAmmoRight = m_PosAmmoLeft + vector2;
			m_PosMegaDamageRight = m_PosMegaDamageLeft + vector2;
			m_PosCloakRight = m_PosCloakLeft + vector2;
			m_PosInvincibleRight = m_PosInvincibleLeft + vector2;
			m_PosPowerCutRight = m_PosPowerCutLeft + vector2;
			m_BarScale = 1.75f;
		}
		base.LoadContent();
	}

	public override void Draw(GameTime gameTime)
	{
		m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		float num = MathHelper.Min(100f, m_LeftShip.Shields);
		m_SpriteBatch.Draw(m_TexShieldBar, new Rectangle((int)m_PosShieldBarLeft.X, (int)m_PosShieldBarLeft.Y, (int)(num * m_BarScale), 16), Color.White);
		m_SpriteBatch.Draw(m_TexStrengthBar, new Rectangle((int)m_PosStrengthBarLeft.X, (int)m_PosStrengthBarLeft.Y, (int)(m_LeftShip.Strength * m_BarScale), 16), Color.White);
		m_SpriteBatch.DrawString(m_HUDFont, m_LeftPlayer.TheShip.Weapons.ActiveFrontWeapon.Ammo.ToString(), m_PosAmmoLeft, Color.White);
		int num2 = ((m_LeftPlayer.Team == ETeam.None) ? m_LeftPlayer.Kills : MainGame.Players.GetTeamScore(m_LeftPlayer.Team));
		m_SpriteBatch.DrawString(m_HUDFont, num2.ToString(), m_PosKillsLeft, Color.White);
		if (m_LeftPlayer.IsMegaDamageActive)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Mega Damage " + (int)m_LeftPlayer.MegaDamageRemaining, m_PosMegaDamageLeft, Color.White);
		}
		if (m_LeftPlayer.IsInvincibile)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Invincibility " + (int)m_LeftPlayer.InvincibilityRemaining, m_PosInvincibleLeft, Color.White);
		}
		if (m_LeftPlayer.IsCloakActive)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Cloaked " + (int)m_LeftPlayer.CloakRemaining, m_PosCloakLeft, Color.White);
		}
		if (m_LeftPlayer.IsPowerCut)
		{
			m_SpriteBatch.DrawString(m_HUDFont, "Critical Power Failure! Power Online in " + (int)m_LeftPlayer.PowerCutRemaining, m_PosPowerCutLeft, Color.Yellow);
		}
		int xpos = base.GraphicsDevice.Viewport.TitleSafeArea.Left;
		DrawWeaponBox(m_LeftPlayer.TheShip, WeaponType.Gun, ref xpos);
		DrawWeaponBox(m_LeftPlayer.TheShip, WeaponType.Blaster, ref xpos);
		DrawWeaponBox(m_LeftPlayer.TheShip, WeaponType.VBlaster, ref xpos);
		Texture2D texture2D = null;
		switch (m_LeftPlayer.TheShip.Weapons.ActiveSpecialWeaponType)
		{
		case SpecialWeaponType.EMP:
			texture2D = m_TexEMPBox;
			break;
		case SpecialWeaponType.Starburst:
			texture2D = m_TexStarBurstBox;
			break;
		}
		if (texture2D != null)
		{
			Vector2 position = new Vector2
			{
				X = base.GraphicsDevice.Viewport.Width / 2 - 84,
				Y = base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 64
			};
			m_SpriteBatch.Draw(texture2D, position, Color.White);
		}
		num = MathHelper.Min(100f, m_RightShip.Shields);
		m_SpriteBatch.Draw(m_TexShieldBar, new Rectangle((int)m_PosShieldBarRight.X, (int)m_PosShieldBarRight.Y, (int)(num * m_BarScale), 16), Color.White);
		m_SpriteBatch.Draw(m_TexStrengthBar, new Rectangle((int)m_PosStrengthBarRight.X, (int)m_PosStrengthBarRight.Y, (int)(m_RightShip.Strength * m_BarScale), 16), Color.White);
		m_SpriteBatch.DrawString(m_HUDFont, m_RightPlayer.TheShip.Weapons.ActiveFrontWeapon.Ammo.ToString(), m_PosAmmoRight, Color.White);
		int num3 = ((m_RightPlayer.Team == ETeam.None) ? m_RightPlayer.Kills : MainGame.Players.GetTeamScore(m_RightPlayer.Team));
		m_SpriteBatch.DrawString(m_HUDFont, num3.ToString(), m_PosKillsRight, Color.White);
		if (m_RightPlayer.IsMegaDamageActive)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Mega Damage " + (int)m_RightPlayer.MegaDamageRemaining, m_PosMegaDamageRight, Color.White);
		}
		if (m_RightPlayer.IsInvincibile)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Invincibility " + (int)m_RightPlayer.InvincibilityRemaining, m_PosInvincibleRight, Color.White);
		}
		if (m_RightPlayer.IsCloakActive)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Cloaked " + (int)m_RightPlayer.CloakRemaining, m_PosCloakRight, Color.White);
		}
		if (m_RightPlayer.IsPowerCut)
		{
			m_SpriteBatch.DrawString(m_HUDFont, "Critical Power Failure! Power Online in " + (int)m_RightPlayer.PowerCutRemaining, m_PosPowerCutRight, Color.Yellow);
		}
		xpos = base.GraphicsDevice.Viewport.Width / 2 + 20;
		DrawWeaponBox(m_RightPlayer.TheShip, WeaponType.Gun, ref xpos);
		DrawWeaponBox(m_RightPlayer.TheShip, WeaponType.Blaster, ref xpos);
		DrawWeaponBox(m_RightPlayer.TheShip, WeaponType.VBlaster, ref xpos);
		texture2D = null;
		switch (m_RightPlayer.TheShip.Weapons.ActiveSpecialWeaponType)
		{
		case SpecialWeaponType.EMP:
			texture2D = m_TexEMPBox;
			break;
		case SpecialWeaponType.Starburst:
			texture2D = m_TexStarBurstBox;
			break;
		}
		if (texture2D != null)
		{
			Vector2 position2 = new Vector2
			{
				X = base.GraphicsDevice.Viewport.TitleSafeArea.Right - 70,
				Y = base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 64
			};
			m_SpriteBatch.Draw(texture2D, position2, Color.White);
		}
		m_SpriteBatch.End();
		base.Draw(gameTime);
	}

	private void DrawWeaponBox(Ship ship, WeaponType type, ref int xpos)
	{
		if (ship.Weapons.IsWeaponFittedEx(type, out var guncount, out var ammo, out var currentweapon))
		{
			Texture2D weaponBoxTexture = GetWeaponBoxTexture(type, guncount);
			Color color = ((ammo > 50) ? Color.White : ((ammo <= 0) ? Color.Red : Color.Orange));
			color.A = (byte)(currentweapon ? byte.MaxValue : 92);
			Vector2 position = new Vector2
			{
				X = xpos,
				Y = base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 64
			};
			m_SpriteBatch.Draw(weaponBoxTexture, position, color);
			xpos += 70;
		}
	}

	private Texture2D GetWeaponBoxTexture(WeaponType type, int guncount)
	{
		switch (type)
		{
		case WeaponType.Gun:
			switch (guncount)
			{
			case 1:
				return m_TexGun1Box;
			case 2:
				return m_TexGun2Box;
			case 3:
				return m_TexGun3Box;
			}
			break;
		case WeaponType.Blaster:
			switch (guncount)
			{
			case 1:
				return m_TexBlaster1Box;
			case 2:
				return m_TexBlaster2Box;
			case 3:
				return m_TexBlaster3Box;
			}
			break;
		case WeaponType.VBlaster:
			switch (guncount)
			{
			case 2:
				return m_TexVBlaster2Box;
			case 3:
				return m_TexVBlaster3Box;
			case 4:
				return m_TexVBlaster4Box;
			case 5:
				return m_TexVBlaster5Box;
			}
			break;
		}
		return null;
	}
}
