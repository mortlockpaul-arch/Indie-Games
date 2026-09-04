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
			((DrawableGameComponent)this).LoadContent();
		}
	}

	public void ShowPlayerHUD(LocalPlayer leftPlayer, LocalPlayer rightPlayer)
	{
		m_LeftPlayer = leftPlayer;
		m_LeftShip = leftPlayer.TheShip;
		m_RightPlayer = rightPlayer;
		m_RightShip = rightPlayer.TheShip;
		((DrawableGameComponent)this).Visible = true;
	}

	protected override void LoadContent()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		m_SpriteBatch = new SpriteBatch(((Game)MainGame.Instance).GraphicsDevice);
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
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(908f, 0f);
			m_PosShieldBarRight = m_PosShieldBarLeft + val;
			m_PosStrengthBarRight = m_PosStrengthBarLeft + val;
			m_PosKillsRight = m_PosKillsLeft + val;
			m_PosAmmoRight = m_PosAmmoLeft + val;
			m_PosMegaDamageRight = m_PosMegaDamageLeft + val;
			m_PosCloakRight = m_PosCloakLeft + val;
			m_PosInvincibleRight = m_PosInvincibleLeft + val;
			m_PosPowerCutRight = m_PosPowerCutLeft + val;
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
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(552f, 0f);
			m_PosShieldBarRight = m_PosShieldBarLeft + val2;
			m_PosStrengthBarRight = m_PosStrengthBarLeft + val2;
			m_PosKillsRight = m_PosKillsLeft + val2;
			m_PosAmmoRight = m_PosAmmoLeft + val2;
			m_PosMegaDamageRight = m_PosMegaDamageLeft + val2;
			m_PosCloakRight = m_PosCloakLeft + val2;
			m_PosInvincibleRight = m_PosInvincibleLeft + val2;
			m_PosPowerCutRight = m_PosPowerCutLeft + val2;
			m_BarScale = 1.75f;
		}
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		m_SpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
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
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		Rectangle titleSafeArea = ((Viewport)(ref viewport)).TitleSafeArea;
		int xpos = ((Rectangle)(ref titleSafeArea)).Left;
		DrawWeaponBox(m_LeftPlayer.TheShip, WeaponType.Gun, ref xpos);
		DrawWeaponBox(m_LeftPlayer.TheShip, WeaponType.Blaster, ref xpos);
		DrawWeaponBox(m_LeftPlayer.TheShip, WeaponType.VBlaster, ref xpos);
		Texture2D val = null;
		switch (m_LeftPlayer.TheShip.Weapons.ActiveSpecialWeaponType)
		{
		case SpecialWeaponType.EMP:
			val = m_TexEMPBox;
			break;
		case SpecialWeaponType.Starburst:
			val = m_TexStarBurstBox;
			break;
		}
		if (val != null)
		{
			Vector2 val2 = default(Vector2);
			Viewport viewport2 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			val2.X = ((Viewport)(ref viewport2)).Width / 2 - 84;
			Viewport viewport3 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			Rectangle titleSafeArea2 = ((Viewport)(ref viewport3)).TitleSafeArea;
			val2.Y = ((Rectangle)(ref titleSafeArea2)).Bottom - 64;
			m_SpriteBatch.Draw(val, val2, Color.White);
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
		Viewport viewport4 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		xpos = ((Viewport)(ref viewport4)).Width / 2 + 20;
		DrawWeaponBox(m_RightPlayer.TheShip, WeaponType.Gun, ref xpos);
		DrawWeaponBox(m_RightPlayer.TheShip, WeaponType.Blaster, ref xpos);
		DrawWeaponBox(m_RightPlayer.TheShip, WeaponType.VBlaster, ref xpos);
		val = null;
		switch (m_RightPlayer.TheShip.Weapons.ActiveSpecialWeaponType)
		{
		case SpecialWeaponType.EMP:
			val = m_TexEMPBox;
			break;
		case SpecialWeaponType.Starburst:
			val = m_TexStarBurstBox;
			break;
		}
		if (val != null)
		{
			Vector2 val3 = default(Vector2);
			Viewport viewport5 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			Rectangle titleSafeArea3 = ((Viewport)(ref viewport5)).TitleSafeArea;
			val3.X = ((Rectangle)(ref titleSafeArea3)).Right - 70;
			Viewport viewport6 = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			Rectangle titleSafeArea4 = ((Viewport)(ref viewport6)).TitleSafeArea;
			val3.Y = ((Rectangle)(ref titleSafeArea4)).Bottom - 64;
			m_SpriteBatch.Draw(val, val3, Color.White);
		}
		m_SpriteBatch.End();
		((DrawableGameComponent)this).Draw(gameTime);
	}

	private void DrawWeaponBox(Ship ship, WeaponType type, ref int xpos)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (ship.Weapons.IsWeaponFittedEx(type, out var guncount, out var ammo, out var currentweapon))
		{
			Texture2D weaponBoxTexture = GetWeaponBoxTexture(type, guncount);
			Color val = ((ammo > 50) ? Color.White : ((ammo <= 0) ? Color.Red : Color.Orange));
			((Color)(ref val)).A = (byte)(currentweapon ? byte.MaxValue : 92);
			Vector2 val2 = new Vector2
			{
				X = xpos
			};
			Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			Rectangle titleSafeArea = ((Viewport)(ref viewport)).TitleSafeArea;
			val2.Y = ((Rectangle)(ref titleSafeArea)).Bottom - 64;
			m_SpriteBatch.Draw(weaponBoxTexture, val2, val);
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
