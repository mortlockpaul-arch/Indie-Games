using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class PostMenuUI
{
	private static string msgStr = "";

	private static Color colorGeneric = Color.White;

	private static Color colorNades = new Color(180, 180, 180, 180);

	private static Color colorHitMarker = new Color(255, 0, 0, 255);

	private static Color markerColor = new Color(0, 180, 0, 180);

	private static Rectangle recGeneric = new Rectangle(0, 0, 0, 0);

	private static Rectangle hitMarkerRec = new Rectangle(631, 352, 19, 17);

	private static Rectangle nadeRec = new Rectangle(0, 520, 22, 36);

	private static Rectangle playerMarker = new Rectangle(0, 0, 16, 2);

	private static Vector2 msgPos = Vector2.Zero;

	private static Vector2 msgPosOffset = Vector2.Zero;

	private static Vector2 tmpProjectionDirection = Vector2.Zero;

	private static Vector3 tmpVec3 = Vector3.Zero;

	private static Vector4 tmpVec4 = Vector4.Zero;

	private static Vector4 projectedPosition = Vector4.Zero;

	private static Matrix tmpUI = Matrix.CreateScale(0.8f, 0.08888f, 1f);

	private static Matrix tmpUIBullet = Matrix.Identity;

	private static PlayerBaseState otherPlayerRef;

	private static Texture2D healthBar = null;

	private static byte[] healthColor = new byte[4];

	private static float healthColorBlink = 1f;

	public static void Update(float eTime, int qIndex, PlayerBase playerRef)
	{
		healthColor[0] = 0;
		healthColor[1] = (byte)(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Health * 0.8f);
		healthColor[2] = 0;
		healthColor[3] = 80;
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Health <= 80f)
		{
			healthColor[0] = (byte)(114f - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Health * 1.4f);
		}
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].Health <= 40f)
		{
			healthColorBlink += eTime * 2f;
			if (healthColorBlink >= 1f)
			{
				healthColorBlink = 0f;
			}
			healthColor[0] = (byte)((float)(int)healthColor[0] * healthColorBlink);
			healthColor[1] = (byte)((float)(int)healthColor[1] * healthColorBlink);
			healthColor[2] = (byte)((float)(int)healthColor[2] * healthColorBlink);
			healthColor[3] = (byte)(healthColorBlink * 80f);
		}
	}

	public static void Draw(int qIndex, PlayerBase playerRef)
	{
		float num = (float)playerRef.vpViewPort.TitleSafeArea.Width / (float)EndGameEngine.GameSettings.BackBufferSizeX;
		MediaEmitterClass.Draw(qIndex);
		if (playerRef.MatchCoolDownTimer > 0f)
		{
			EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = playerRef.vpViewPort;
			_ = Vector2.Zero;
			_ = playerRef.vpViewPort.TitleSafeArea.Center.X;
			_ = playerRef.vpViewPort.X;
			_ = playerRef.vpViewPort.TitleSafeArea.Top;
			Menu.spriteBatch.Begin();
			msgPos.X = EndGameEngine.DefualtViewport.TitleSafeArea.Center.X - 180;
			msgPos.Y = EndGameEngine.DefualtViewport.TitleSafeArea.Top + 48;
			Menu.spriteBatch.End();
			if (playerRef.MenuState == PlayerMenuState.InGame)
			{
			}
		}
		else if (playerRef.DeathTimer <= 0f && playerRef.RespawnTimer < playerRef.RESPAWN_TIME)
		{
			EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = playerRef.vpViewPort;
			Menu.spriteBatch.Begin();
			if (playerRef.ToggledRespawn)
			{
				if (playerRef.TargetPraticeMessage)
				{
					Vector2 zero = Vector2.Zero;
					string text = "SURVIVE WAVES OF ZOMBIES";
					zero.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text).X * 0.5f * 2f;
					zero.Y = playerRef.vpViewPort.TitleSafeArea.Top + 32;
					Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.Black, 0f, new Vector2(-3f, -2f), 2f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
					Rectangle a = default(Rectangle);
					a.X = playerRef.vpViewPort.TitleSafeArea.Left + 128;
					a.Y = playerRef.vpViewPort.TitleSafeArea.Top + 128;
					a.Width = playerRef.vpViewPort.TitleSafeArea.Right - 128 - a.X;
					a.Height = playerRef.vpViewPort.TitleSafeArea.Bottom - 128 - a.Y;
					Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, a, new Color(100, 100, 100, 160));
					text = "Kill All Zombies!";
					zero.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text).X * 0.5f * 1.1f;
					zero.Y = playerRef.vpViewPort.TitleSafeArea.Top + 140;
					Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
					text = "Press";
					zero.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text).X * 0.5f * 1.25f;
					zero.Y = playerRef.vpViewPort.TitleSafeArea.Bottom - 164;
					Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
					zero.X += 116f;
					zero.Y -= 2f;
					Menu.spriteBatch.Draw(Menu.aButton, new Rectangle((int)zero.X, (int)zero.Y, 36, 36), Color.White);
				}
				else if (playerRef.AvRStartMessage)
				{
					Vector2 zero2 = Vector2.Zero;
					string text2 = "COMBAT WAVES OF ALIENS";
					zero2.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text2).X * 0.5f * 2f;
					zero2.Y = playerRef.vpViewPort.TitleSafeArea.Top + 32;
					Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero2, Color.Black, 0f, new Vector2(-3f, -2f), 2f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero2, Color.LightGray, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
					Rectangle a2 = default(Rectangle);
					a2.X = playerRef.vpViewPort.TitleSafeArea.Left + 128;
					a2.Y = playerRef.vpViewPort.TitleSafeArea.Top + 128;
					a2.Width = playerRef.vpViewPort.TitleSafeArea.Right - 128 - a2.X;
					a2.Height = playerRef.vpViewPort.TitleSafeArea.Bottom - 128 - a2.Y;
					Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, a2, new Color(100, 100, 100, 160));
					text2 = "Kill All Aliens!";
					zero2.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text2).X * 0.5f * 1.1f;
					zero2.Y = playerRef.vpViewPort.TitleSafeArea.Top + 140;
					Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero2, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
					text2 = "Press";
					zero2.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text2).X * 0.5f * 1.25f;
					zero2.Y = playerRef.vpViewPort.TitleSafeArea.Bottom - 164;
					Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero2, Color.LightGray, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
					zero2.X += 116f;
					zero2.Y -= 2f;
					Menu.spriteBatch.Draw(Menu.aButton, new Rectangle((int)zero2.X, (int)zero2.Y, 36, 36), Color.White);
				}
				else
				{
					Vector2 zero3 = Vector2.Zero;
					zero3.X = (float)playerRef.vpViewPort.TitleSafeArea.Center.X - 112f - (float)playerRef.vpViewPort.X;
					zero3.Y = playerRef.vpViewPort.TitleSafeArea.Center.Y - playerRef.vpViewPort.Y;
					string b = "Spawn In..." + (int)(playerRef.RESPAWN_TIME + 1f - playerRef.RespawnTimer);
					Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero3, Color.Black, 0f, new Vector2(-3f, -3f), 1.5f * num, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero3, Color.White, 0f, Vector2.Zero, 1.5f * num, SpriteEffects.None, 0);
					if (EndGameEngine.GameSettings.GameName.Contains("_AvR_") || EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor"))
					{
						Menu.DrawEnterLoadoutButton(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, "TO LOADOUT");
					}
				}
			}
			Menu.spriteBatch.End();
			if (playerRef.MenuState == PlayerMenuState.InGame)
			{
			}
		}
		else if (playerRef.MenuState == PlayerMenuState.InGame)
		{
			DrawInGameUI(qIndex, playerRef);
		}
	}

	private static void DrawInGameUI(int qIndex, PlayerBase playerRef)
	{
		if (FPSGameMenu.isVisable)
		{
			return;
		}
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = playerRef.vpViewPort;
		if (EndGameEngine.GameSettings.GameName == "_AvR_")
		{
			if (!playerRef.OverrideCamera)
			{
				msgPos.X = 132f;
				msgPos.Y = 560f;
				playerRef.playerTag.Draw(ref msgPos, byte.MaxValue, offsetTag: true, drawPicture: true);
			}
		}
		else if (EndGameEngine.GameSettings.GameName == "ToyPlane")
		{
			if (playerRef.OverrideCamera)
			{
			}
		}
		else if (EndGameEngine.GameSettings.GameName == "TowerDefense")
		{
			_ = playerRef.OverrideCamera;
		}
		else if (!(EndGameEngine.GameSettings.GameName == "ApocalypseZ"))
		{
			if (!playerRef.OverridePosition)
			{
				DrawBulletUI(qIndex, playerRef);
			}
			msgPos.X = 132f;
			msgPos.Y = 560f;
			playerRef.playerTag.Draw(ref msgPos, byte.MaxValue, offsetTag: true, drawPicture: true);
			if (!playerRef.Sighted && playerRef.Stance != PlayerStance.Run)
			{
				if (playerRef.fpsWeapon.CurrentWeapon.NaderToggled)
				{
					Menu.spriteBatch.Begin();
					recGeneric.X = 590;
					recGeneric.Y = 305;
					recGeneric.Width = 100;
					recGeneric.Height = 200;
					Menu.spriteBatch.Draw(PlayerBase.NadeReticleUI, recGeneric, Color.White);
					Menu.spriteBatch.End();
				}
				else
				{
					playerRef.DrawReticle(playerRef.vpViewPort.AspectRatio, playerRef);
				}
			}
			Menu.spriteBatch.Begin();
			if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
			{
				DrawCombatTraining(qIndex, playerRef);
			}
			else if (LevelBaseMenu.gameMode == GameMode.SurvivorLocal)
			{
				DrawSurvivalLocal(qIndex, playerRef);
			}
			else
			{
				_ = LevelBaseMenu.gameMode;
				_ = 1;
			}
			if (playerRef.NumberThrowingKnife > 0)
			{
				recGeneric.X = 1030;
				recGeneric.Y = 538;
				recGeneric.Width = 22;
				recGeneric.Height = 22;
				Menu.spriteBatch.Draw(PlayerBase.DPadLeftIconUI, recGeneric, colorNades);
				recGeneric.X = 1008;
				recGeneric.Y = 524;
				recGeneric.Width = 26;
				recGeneric.Height = 38;
				Menu.spriteBatch.Draw(PlayerBase.ThrowKnifeIconUI, recGeneric, colorNades);
			}
			if (!EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor"))
			{
				recGeneric.X = 1100;
				recGeneric.Y = 538;
				recGeneric.Width = 22;
				recGeneric.Height = 22;
				Menu.spriteBatch.Draw(PlayerBase.DPadRightIconUI, recGeneric, colorNades);
			}
			if (playerRef.NumberSmokeGrenades > 0)
			{
				recGeneric.X = 1082;
				recGeneric.Y = 536;
				recGeneric.Width = 16;
				recGeneric.Height = 24;
				Menu.spriteBatch.Draw(PlayerBase.SmookeIconUI, recGeneric, colorNades);
				if (playerRef.NumberFragGrenades > 1)
				{
					recGeneric.X = 1064;
					recGeneric.Y = 536;
					recGeneric.Width = 16;
					recGeneric.Height = 24;
					Menu.spriteBatch.Draw(PlayerBase.FragIconUI, recGeneric, colorNades);
				}
			}
			else if (playerRef.NumberFragGrenades > 1)
			{
				recGeneric.X = 1082;
				recGeneric.Y = 536;
				recGeneric.Width = 16;
				recGeneric.Height = 24;
				Menu.spriteBatch.Draw(PlayerBase.FragIconUI, recGeneric, colorNades);
			}
			if (playerRef.NumberFragGrenades > 0)
			{
				recGeneric.X = 1124;
				recGeneric.Y = 524;
				recGeneric.Width = 22;
				recGeneric.Height = 36;
				Menu.spriteBatch.Draw(PlayerBase.FragIconUI, recGeneric, colorNades);
			}
			if (playerRef.fpsWeapon.CurrentWeapon.AttachmentTwo == WeaponAttachment.NadeLauncher)
			{
				int num = 130;
				for (int i = 0; i < playerRef.NumberNaderGrenades; i++)
				{
					nadeRec.X = num;
					Menu.spriteBatch.Draw(PlayerBase.NaderIconUI, nadeRec, colorNades);
					num += 24;
				}
			}
			if (playerRef.fHitIndicatorTimer > 0f)
			{
				playerRef.fHitIndicatorTimer -= 0.009f;
				Menu.spriteBatch.Draw(LevelBaseMenu.texHitMarker, hitMarkerRec, colorHitMarker);
			}
			msgPos.X = 1090f;
			msgPos.Y = 562f;
			msgPosOffset.X = 0f;
			msgPosOffset.Y = 0f;
			byte b = (colorGeneric.B = 0);
			byte r = (colorGeneric.G = b);
			colorGeneric.R = r;
			colorGeneric.A = byte.MaxValue;
			msgStr = playerRef.fpsWeapon.CurrentWeapon.BulletsInMag + " / " + playerRef.fpsWeapon.CurrentWeapon.BulletsTotal;
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, colorGeneric, 0f, msgPosOffset, 0.5f, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
		}
		_ = playerRef.ModeratorDrawAllGamerTags;
	}

	private static void DrawTDMUI(int qIndex, PlayerBase playerRef)
	{
	}

	private static void DrawCombatTraining(int qIndex, PlayerBase playerRef)
	{
		tmpProjectionDirection.X = FPSGameMenu.DestinationPos.X - playerRef.vecPosition.X;
		tmpProjectionDirection.Y = FPSGameMenu.DestinationPos.Z - playerRef.vecPosition.Z;
		float num = tmpProjectionDirection.X * playerRef.vecDirection.X + tmpProjectionDirection.Y * playerRef.vecDirection.Z;
		if (num > 0f)
		{
			float num2 = 1f - tmpProjectionDirection.LengthSquared() / 36000000f;
			float num3 = ((num2 > 1f) ? 1f : num2);
			num3 = ((num3 < 0.75f) ? 0.75f : num3);
			Vector3 vector = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Project(FPSGameMenu.DestinationPos + new Vector3(0f, 60f, 0f), playerRef.mDataQueue[qIndex].projection, playerRef.mDataQueue[qIndex].view, Matrix.Identity);
			Vector2 c = new Vector2(vector.X - (float)playerRef.vpViewPort.X + 60f * (0.5f - num3), vector.Y - (float)playerRef.vpViewPort.Y);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "GOAL", c, Color.Black, 0f, new Vector2(-2f, -2f), num3 * 0.9f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "GOAL", c, Color.Green, 0f, Vector2.Zero, num3 * 0.9f, SpriteEffects.None, 0);
		}
		msgPos.X = 560f;
		msgPos.Y = 604f;
		if (FPSGameMenu.TrialTime > 0f)
		{
			float num4 = FPSGameMenu.TrialTime * (1f / 60f);
			msgStr = (int)num4 + " : ";
			int num5 = (int)(num4 * 60f) % 60;
			if (num5 < 10)
			{
				msgStr += "0";
			}
			msgStr = msgStr + num5 + " : ";
			int num6 = (int)(FPSGameMenu.TrialTime * 100f) % 100;
			if (num6 < 10)
			{
				msgStr += "0";
			}
			msgStr += num6;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Time " + msgStr, msgPos + new Vector2(1f, 1f), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Time " + msgStr, msgPos, Color.LightGray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		else
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, "No Bonus Time", msgPos + new Vector2(1f, 1f), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "No Bonus Time", msgPos, Color.LightGray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		msgPos.X = 500f;
		msgPos.Y = 80f;
		msgStr = "Best Trial Score " + LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].TrialScore;
		Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, Color.Black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
		Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, Color.LightGray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		if (Guide.IsTrialMode)
		{
			msgStr = "Unlock Weapons, Map, Characters, Play Modes And Level Up";
			msgPos.Y = 106f;
			msgPos.X = 640f - Menu.defaultFont.MeasureString(msgStr).X * 0.8f * 0.5f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, Color.Black, 0f, new Vector2(-2f, -2f), 0.8f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, Color.LightGray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			msgStr = "Skill Points In Online 8 vs 8 Battle! Unlock Full Version!";
			msgPos.Y = 128f;
			msgPos.X = 640f - Menu.defaultFont.MeasureString(msgStr).X * 0.8f * 0.5f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, Color.Black, 0f, new Vector2(-2f, -2f), 0.8f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgStr, msgPos, Color.LightGray, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
	}

	private static void DrawSurvivalLocal(int qIndex, PlayerBase playerRef)
	{
	}

	private static void DrawBulletUI(int qIndex, PlayerBase playerRef)
	{
		float num = playerRef.fpsWeapon.CurrentWeapon.BulletsInMag;
		float num2 = playerRef.fpsWeapon.CurrentWeapon.BulletsMagMax;
		if (num2 > 30f)
		{
			num2 = 30f;
			if (num > 30f)
			{
				num = 30f;
			}
		}
		float num3 = 1f / 60f;
		float x = 0.5f - num3 * (num2 - num);
		tmpVec3.X = 0f;
		tmpVec3.Y = -0.65f;
		tmpVec3.Z = 0f;
		tmpUI.Translation = tmpVec3;
		tmpUIBullet = Matrix.CreateScale(0.32727203f * (num2 / 30f), 0.064438f, 1f);
		tmpVec3.X = 0.466f + 0.010905f * (30f - num2);
		tmpVec3.Y = -0.667f;
		tmpVec3.Z = 0f;
		tmpUIBullet.Translation = tmpVec3;
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.AlphaBlend;
		graphicsDevice.SetVertexBuffer(LevelBaseMenu.postVertexBuffer);
		materialEffect.CurrentTechnique = EndGameEngine.MaterialParams.T_UI;
		EndGameEngine.MaterialParams.matWorld.SetValue(tmpUI);
		EndGameEngine.MaterialParams.Texture4.SetValue(PlayerBase.BottomUI);
		materialEffect.CurrentTechnique.Passes[0].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		tmpVec4.X = x;
		tmpVec4.Y = 0f;
		tmpVec4.Z = 0.5f * (num2 / 30f);
		tmpVec4.W = 1f;
		EndGameEngine.MaterialParams.uvDisplacement.SetValue(tmpVec4);
		EndGameEngine.MaterialParams.matWorld.SetValue(tmpUIBullet);
		EndGameEngine.MaterialParams.Texture4.SetValue(PlayerBase.BulletUI);
		materialEffect.CurrentTechnique.Passes[1].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		graphicsDevice.BlendState = BlendState.Opaque;
	}
}
