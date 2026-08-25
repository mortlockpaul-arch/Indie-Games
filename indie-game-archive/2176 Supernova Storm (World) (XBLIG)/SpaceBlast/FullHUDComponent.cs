using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class FullHUDComponent : DrawableGameComponent
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

	private Vector2 m_PosShieldBar;

	private Vector2 m_PosStrengthBar;

	private Vector2 m_PosKills;

	private Vector2 m_PosAmmo;

	private Vector2 m_PosMegaDamage;

	private Vector2 m_PosCloak;

	private Vector2 m_PosInvincible;

	private Vector2 m_PosPowerCut;

	private int m_BarScale;

	public FullHUDComponent(Game game, bool loadContentNow)
		: base(game)
	{
		if (loadContentNow)
		{
			LoadContent();
		}
	}

	public void ShowPlayerHUD()
	{
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
			m_PosShieldBar = new Vector2(128f, 110f);
			m_PosStrengthBar = new Vector2(1192f, 110f);
			m_PosKills = new Vector2(895f, 110f);
			m_PosAmmo = new Vector2(980f, 110f);
			m_PosMegaDamage = new Vector2(128f, 135f);
			m_PosCloak = new Vector2(910f, 135f);
			m_PosInvincible = new Vector2(1652f, 135f);
			m_PosPowerCut = new Vector2(100f, 150f);
			m_BarScale = 6;
		}
		else
		{
			m_PosShieldBar = new Vector2(128f, 80f);
			m_PosStrengthBar = new Vector2(852f, 80f);
			m_PosKills = new Vector2(590f, 80f);
			m_PosAmmo = new Vector2(650f, 80f);
			m_PosMegaDamage = new Vector2(128f, 105f);
			m_PosCloak = new Vector2(595f, 105f);
			m_PosInvincible = new Vector2(1012f, 105f);
			m_PosPowerCut = new Vector2(100f, 130f);
			m_BarScale = 3;
		}
		base.LoadContent();
	}

	public override void Draw(GameTime gameTime)
	{
		m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		LocalPlayer leftPlayer = MainGame.Instance.LeftPlayer;
		Ship theShip = leftPlayer.TheShip;
		float num = MathHelper.Min(100f, theShip.Shields);
		m_SpriteBatch.Draw(m_TexShieldBar, new Rectangle((int)m_PosShieldBar.X, (int)m_PosShieldBar.Y, (int)num * m_BarScale, 16), Color.White);
		m_SpriteBatch.Draw(m_TexStrengthBar, new Rectangle((int)m_PosStrengthBar.X, (int)m_PosStrengthBar.Y, (int)theShip.Strength * m_BarScale, 16), Color.White);
		m_SpriteBatch.DrawString(m_HUDFont, leftPlayer.TheShip.Weapons.ActiveFrontWeapon.Ammo.ToString(), m_PosAmmo, Color.White);
		int num2 = ((leftPlayer.Team == ETeam.None) ? leftPlayer.Kills : MainGame.Players.GetTeamScore(leftPlayer.Team));
		m_SpriteBatch.DrawString(m_HUDFont, num2.ToString(), m_PosKills, Color.White);
		if (leftPlayer.IsMegaDamageActive)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Mega Damage " + (int)leftPlayer.MegaDamageRemaining, m_PosMegaDamage, Color.White);
		}
		if (leftPlayer.IsInvincibile)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Invincibility " + (int)leftPlayer.InvincibilityRemaining, m_PosInvincible, Color.White);
		}
		if (leftPlayer.IsCloakActive)
		{
			m_SpriteBatch.DrawString(m_HUDSmallFont, "Cloaked " + (int)leftPlayer.CloakRemaining, m_PosCloak, Color.White);
		}
		if (leftPlayer.IsPowerCut)
		{
			m_SpriteBatch.DrawString(m_HUDFont, "Warning: Critical Power Failure! All Auxilary Power Routed to Shields. Full Power Back Online in " + (int)leftPlayer.PowerCutRemaining, m_PosPowerCut, Color.Yellow);
		}
		int boxslot = 0;
		DrawWeaponBox(theShip, WeaponType.Gun, ref boxslot);
		DrawWeaponBox(theShip, WeaponType.Blaster, ref boxslot);
		DrawWeaponBox(theShip, WeaponType.VBlaster, ref boxslot);
		Texture2D texture2D = null;
		switch (theShip.Weapons.ActiveSpecialWeaponType)
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
				X = base.GraphicsDevice.Viewport.TitleSafeArea.Right - 64,
				Y = base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 64
			};
			m_SpriteBatch.Draw(texture2D, position, Color.White);
		}
		m_SpriteBatch.End();
		base.Draw(gameTime);
	}

	private void DrawWeaponBox(Ship ship, WeaponType type, ref int boxslot)
	{
		if (ship.Weapons.IsWeaponFittedEx(type, out var guncount, out var ammo, out var currentweapon))
		{
			Texture2D weaponBoxTexture = GetWeaponBoxTexture(type, guncount);
			Color color = ((ammo > 50) ? Color.White : ((ammo <= 0) ? Color.Red : Color.Orange));
			color.A = (byte)(currentweapon ? byte.MaxValue : 92);
			Vector2 position = new Vector2
			{
				X = base.GraphicsDevice.Viewport.TitleSafeArea.Left + boxslot * 70,
				Y = base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 64
			};
			m_SpriteBatch.Draw(weaponBoxTexture, position, color);
			boxslot++;
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
