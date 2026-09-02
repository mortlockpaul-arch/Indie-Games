using System;
using Microsoft.Xna.Framework;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Shaders;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class GameScreen : IGameScreen
{
	public GameScreen()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		RacingGameManager.LoadLevel(TrackSelection.SelectedTrack);
		RacingGameManager.Player.Reset();
		BaseGame.LightDirection = LensFlare.DefaultLightPos;
		Sound.StartGearSound();
		Sound.Play(Sound.Sounds.GameMusic);
	}

	public bool Render()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		ShadowMapShader.PrepareGameShadows();
		BaseGame.UI.PostScreenGlowShader.Start();
		BaseGame.UI.RenderGameBackground();
		RacingGameManager.Landscape.Render();
		RacingGameManager.CarModel.RenderCar(RacingGameManager.currentCarNumber, RacingGameManager.CarColor, shadowCarMode: false, RacingGameManager.Player.CarRenderMatrix);
		BaseGame.MeshRenderManager.Render();
		Matrix carMatrixAtTime = RacingGameManager.Landscape.BestReplay.GetCarMatrixAtTime(RacingGameManager.Player.GameTimeMilliseconds / 1000f);
		carMatrixAtTime = Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateRotationZ((float)Math.PI) * carMatrixAtTime;
		if (RacingGameManager.Player.GameTimeMilliseconds > 0f)
		{
			RacingGameManager.CarModel.RenderCar(0, RacingGameManager.CarColor, shadowCarMode: true, carMatrixAtTime);
		}
		if (BaseGame.AllowShadowMapping)
		{
			ShaderEffect.shadowMapping.ShowShadows();
		}
		BaseGame.UI.PostScreenGlowShader.Show();
		Sound.UpdateGearSound(RacingGameManager.Player.Speed, RacingGameManager.Player.Acceleration);
		BaseGame.UI.RenderGameUI((int)RacingGameManager.Player.GameTimeMilliseconds, (int)RacingGameManager.Player.BestTimeMilliseconds, RacingGameManager.Player.CurrentLap + 1, RacingGameManager.Player.Speed * 5.793638f, 1 + (int)(5f * RacingGameManager.Player.Speed / 50.0549f), 0.5f * RacingGameManager.Player.Speed / 50.0549f + 0.5f * RacingGameManager.Player.Acceleration, RacingGameManager.Landscape.CurrentTrackName, Highscores.GetTop5LapTimes(TrackSelection.SelectedTrackNumber));
		if (RacingGameManager.Player.game_paused)
		{
			BaseGame.UI.RenderBlackBar(518, 61);
			if ((int)(BaseGame.TotalTime / 0.375f) % 3 != 0)
			{
				BaseGame.UI.Headers.RenderOnScreen(BaseGame.CalcRectangleCenteredWithGivenHeight(512, 548, 26, UIRenderer.PressStartGfxRect), UIRenderer.PressStartGfxRect);
			}
		}
		else if (Input.KeyboardEscapeJustPressed || Input.GamePadBackJustPressed || (RacingGameManager.Player.GameOver && (Input.KeyboardSpaceJustPressed || Input.GamePadAJustPressed || Input.GamePadBJustPressed || Input.GamePadXJustPressed || Input.GamePadXJustPressed || Input.MouseLeftButtonJustPressed)))
		{
			Sound.StopGearSound();
			Sound.Play(Sound.Sounds.MenuMusic);
			return true;
		}
		return false;
	}
}
