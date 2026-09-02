using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Properties;
using RacingGame.Shaders;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class SplashScreen : IGameScreen
{
	public bool Render()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Invalid comparison between Unknown and I4
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.UpdateCarInMenu();
		ShadowMapShader.PrepareGameShadows();
		BaseGame.UI.RenderGameBackground();
		BaseGame.UI.RenderMenuTrackBackground();
		BaseGame.UI.RenderBlackBar(518, 61);
		if (BaseGame.AllowShadowMapping)
		{
			ShaderEffect.shadowMapping.ShowShadows();
		}
		if ((int)(BaseGame.TotalTime / 0.375f) % 3 != 0)
		{
			BaseGame.UI.Headers.RenderOnScreen(BaseGame.CalcRectangleCenteredWithGivenHeight(512, 548, 26, UIRenderer.PressStartGfxRect), UIRenderer.PressStartGfxRect);
		}
		Input.controllingPlayer = (PlayerIndex)0;
		for (PlayerIndex val = (PlayerIndex)0; (int)val <= 3; val = (PlayerIndex)(val + 1))
		{
			GamePadState state = GamePad.GetState(val);
			GamePadButtons buttons = ((GamePadState)(ref state)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				Input.controllingPlayer = val;
				SignedInGamer val2 = Gamer.SignedInGamers[Input.controllingPlayer];
				if (val2 != null)
				{
					GameSettings.playerName = ((Gamer)val2).Gamertag;
				}
				else
				{
					Guide.ShowSignIn(1, false);
				}
				val2 = Gamer.SignedInGamers[Input.controllingPlayer];
				if (val2 != null)
				{
					GameSettings.Initialize();
					Sound.SetVolumes(GameSettings.Default.SoundVolume, GameSettings.Default.MusicVolume);
					Highscores.Initialize();
					GameSettings.playerName = ((Gamer)val2).Gamertag;
					return true;
				}
				return false;
			}
		}
		return false;
	}
}
