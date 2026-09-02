using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Shaders;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class CarSelection : IGameScreen
{
	private static float[] CarTypeMaxSpeed = new float[3] { 49.839146f, 47.465855f, 41.76995f };

	private static float[] CarTypeMass = new float[3] { 1015f, 1175f, 875f };

	private static float[] CarTypeMaxAcceleration = new float[3] { 2.125f, 3f, 2.5f };

	private float carSelectionRotationZ;

	private Rectangle gfxBarFromOptionsScreen;

	public bool Render()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.AllowShadowMapping)
		{
			BaseGame.ViewMatrix = Matrix.CreateLookAt(new Vector3(0f, 10.45f, 2.75f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, 1f));
			Vector3 lightDirection = -LensFlare.DefaultLightPos;
			((Vector3)(ref lightDirection))._002Ector(lightDirection.X, lightDirection.Y, 0f - lightDirection.Z);
			BaseGame.LightDirection = lightDirection;
			float num = (float)Math.PI * 2f / 3f;
			float targetRot = (float)RacingGameManager.currentCarNumber * num;
			carSelectionRotationZ = InterpolateRotation(carSelectionRotationZ, targetRot, BaseGame.MoveFactorPerSecond * 5f);
			Matrix[] array = (Matrix[])(object)new Matrix[3];
			for (int i = 0; i < 3; i++)
			{
				ref Matrix reference = ref array[i];
				reference = Matrix.CreateRotationZ(BaseGame.TotalTime / 3.9f) * Matrix.CreateTranslation(new Vector3(0f, 5f, 0f)) * Matrix.CreateRotationZ(0f - carSelectionRotationZ + (float)i * num) * Matrix.CreateTranslation(new Vector3(1.5f, 0f, 1f));
			}
			RacingGameManager.Player.SetCarPosition(Vector3.Zero, new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f));
		}
		BaseGame.UI.PostScreenMenuShader.Start();
		BaseGame.UI.RenderMenuBackground();
		BaseGame.UI.RenderBlackBar(170, 390);
		Texture.additiveSprite.End();
		Texture.alphaSprite.End();
		Texture.additiveSprite.Begin((SpriteBlendMode)2);
		Texture.alphaSprite.Begin((SpriteBlendMode)1);
		int num2 = 10;
		int num3 = 18;
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			num2 += 36;
			num3 += 26;
		}
		BaseGame.UI.Headers.RenderOnScreenRelative1600(num2, num3, UIRenderer.HeaderChooseCarGfxRect);
		TextureFont.WriteText(BaseGame.XToRes(85), BaseGame.YToRes(512), "Car Color: ");
		for (int j = 0; j < RacingGameManager.CarColors.Count; j++)
		{
			Rectangle rect = ((RacingGameManager.currentCarColor == j) ? BaseGame.CalcRectangle(250 + j * 50 - 6, 494, 58, 58) : BaseGame.CalcRectangle(250 + j * 50, 500, 46, 46));
			RacingGameManager.colorSelectionTexture.RenderOnScreen(rect, RacingGameManager.colorSelectionTexture.GfxRectangle, RacingGameManager.CarColors[j]);
			if (Input.MouseInBox(rect) && Input.MouseLeftButtonPressed)
			{
				if (RacingGameManager.currentCarColor != j)
				{
					Sound.Play(Sound.Sounds.Highlight);
				}
				RacingGameManager.currentCarColor = j;
			}
		}
		CarPhysics.SetCarVariablesForCarType(CarTypeMaxSpeed[RacingGameManager.currentCarNumber], CarTypeMass[RacingGameManager.currentCarNumber], CarTypeMaxAcceleration[RacingGameManager.currentCarNumber]);
		float num4 = -1.5f + 2.45f * (CarTypeMaxSpeed[RacingGameManager.currentCarNumber] / 47.465855f);
		float num5 = -1.25f + 1.85f * (CarTypeMaxAcceleration[RacingGameManager.currentCarNumber] / 2.5f);
		float num6 = -0.65f + 1.5f * (CarTypeMass[RacingGameManager.currentCarNumber] / 1000f);
		float value = -0.2f + num5 - num6 + num4;
		float value2 = -1f + (1f / num6 + num4 / 5f);
		float num7 = -0.2f + 0.5f * (num4 / num6 + num5 - num4 * 5f + 5f);
		if (num7 > 0.95f)
		{
			num7 = 0.95f;
		}
		ShowCarPropertyBar(BaseGame.XToRes(766), BaseGame.YToRes(190), "Max Speed: " + (int)(CarTypeMaxSpeed[RacingGameManager.currentCarNumber] / 0.17260312f) + "mph", num4);
		ShowCarPropertyBar(BaseGame.XToRes(766), BaseGame.YToRes(235), "Acceleration:", num5);
		ShowCarPropertyBar(BaseGame.XToRes(766), BaseGame.YToRes(280), "Car Mass:", num6);
		ShowCarPropertyBar(BaseGame.XToRes(766), BaseGame.YToRes(335), "Braking:", value);
		ShowCarPropertyBar(BaseGame.XToRes(766), BaseGame.YToRes(390), "Friction:", value2);
		ShowCarPropertyBar(BaseGame.XToRes(766), BaseGame.YToRes(445), "Engine:", num7);
		float num8 = (float)Math.Sin(BaseGame.TotalTime / 0.46f) * (float)Math.Cos(BaseGame.TotalTime / 0.285f);
		float num9 = 0.75f - 0.065f * num8;
		Rectangle val = BaseGame.CalcRectangle(512, 120, (int)Math.Round((float)UIRenderer.BigArrowGfxRect.Width * num9), (int)Math.Round((float)UIRenderer.BigArrowGfxRect.Width * num9));
		val.X -= val.Width / 2;
		Rectangle selectionArrowGfxRect = UIRenderer.SelectionArrowGfxRect;
		Rectangle rect2 = BaseGame.CalcRectangle(35, 250, selectionArrowGfxRect.Width, selectionArrowGfxRect.Height);
		rect2.Y = BaseGame.YToRes(360) + val.Y / 3;
		rect2.X += (int)Math.Round((float)BaseGame.XToRes(12) * num8);
		BaseGame.UI.Buttons.RenderOnScreen(rect2, new Rectangle(selectionArrowGfxRect.X + selectionArrowGfxRect.Width, selectionArrowGfxRect.Y, -selectionArrowGfxRect.Width, selectionArrowGfxRect.Height));
		Rectangle rect3 = BaseGame.CalcRectangle(689 - selectionArrowGfxRect.Width, 250, selectionArrowGfxRect.Width, selectionArrowGfxRect.Height);
		rect3.Y = BaseGame.YToRes(360) + val.Y / 3;
		rect3.X -= (int)Math.Round((float)BaseGame.XToRes(12) * num8);
		BaseGame.UI.Buttons.RenderOnScreen(rect3, UIRenderer.SelectionArrowGfxRect);
		if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed || (Input.MouseLeftButtonJustPressed && Input.MouseInBoxRelative(new Rectangle(562, 170, 362, 135))))
		{
			Sound.Play(Sound.Sounds.Highlight);
			RacingGameManager.currentCarNumber = (RacingGameManager.currentCarNumber + 1) % 3;
		}
		else if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed || (Input.MouseLeftButtonJustPressed && Input.MouseInBoxRelative(new Rectangle(100, 170, 312, 135))))
		{
			Sound.Play(Sound.Sounds.Highlight);
			RacingGameManager.currentCarNumber = (RacingGameManager.currentCarNumber + 2) % 3;
		}
		if (Input.GamePadUpJustPressed || Input.KeyboardUpJustPressed)
		{
			Sound.Play(Sound.Sounds.Highlight);
			RacingGameManager.currentCarColor = (RacingGameManager.currentCarColor + RacingGameManager.NumberOfCarColors - 1) % RacingGameManager.NumberOfCarColors;
		}
		else if (Input.GamePadDownJustPressed || Input.KeyboardDownJustPressed)
		{
			Sound.Play(Sound.Sounds.Highlight);
			RacingGameManager.currentCarColor = (RacingGameManager.currentCarColor + 1) % RacingGameManager.NumberOfCarColors;
		}
		bool flag = BaseGame.UI.RenderBottomButtons(onlyBack: false);
		if (Input.GamePadAJustPressed || Input.KeyboardSpaceJustPressed || flag)
		{
			RacingGameManager.AddGameScreen(new TrackSelection());
			return false;
		}
		if (Input.KeyboardEscapeJustPressed || Input.GamePadBJustPressed || Input.GamePadBackJustPressed || BaseGame.UI.backButtonPressed)
		{
			return true;
		}
		return false;
	}

	public static void AdjustRotRange(ref float desiredRot, float sourceRot)
	{
		if (desiredRot >= sourceRot + (float)Math.PI)
		{
			desiredRot -= (float)Math.PI * 2f;
		}
		if (desiredRot < sourceRot - (float)Math.PI)
		{
			desiredRot += (float)Math.PI * 2f;
		}
	}

	public static void AdjustRotToPIRange(ref float desiredRot)
	{
		if (desiredRot <= -(float)Math.PI)
		{
			desiredRot += (float)Math.PI * 2f;
		}
		if (desiredRot > (float)Math.PI)
		{
			desiredRot -= (float)Math.PI * 2f;
		}
	}

	public static float InterpolateRotation(float rot, float targetRot, float nearlyEqualRot)
	{
		AdjustRotRange(ref targetRot, rot);
		if (rot > targetRot)
		{
			rot = ((!(Math.Abs(rot - targetRot) < nearlyEqualRot)) ? (rot - nearlyEqualRot) : targetRot);
		}
		else if (rot < targetRot)
		{
			rot = ((!(Math.Abs(rot - targetRot) < nearlyEqualRot)) ? (rot + nearlyEqualRot) : targetRot);
		}
		AdjustRotToPIRange(ref rot);
		return rot;
	}

	private void ShowCarPropertyBar(int x, int y, string propertyName, float value)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		TextureFont.WriteText(x, y, propertyName);
		BaseGame.UI.OptionsScreen.RenderOnScreen(new Rectangle(x, y + BaseGame.YToRes(29), BaseGame.XToRes((int)(192f * value)), BaseGame.YToRes(6)), gfxBarFromOptionsScreen);
	}

	public void PostUIRender()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		Matrix viewMatrix = BaseGame.ViewMatrix;
		BaseGame.ViewMatrix = Matrix.CreateLookAt(new Vector3(0f, 10.45f, 2.75f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, 1f));
		Vector3 lightDirection = -LensFlare.DefaultLightPos;
		((Vector3)(ref lightDirection))._002Ector(lightDirection.X, lightDirection.Y, 0f - lightDirection.Z);
		BaseGame.LightDirection = lightDirection;
		float num = (float)Math.PI * 2f / 3f;
		float targetRot = (float)RacingGameManager.currentCarNumber * num;
		carSelectionRotationZ = InterpolateRotation(carSelectionRotationZ, targetRot, BaseGame.MoveFactorPerSecond * 5f);
		Matrix[] array = (Matrix[])(object)new Matrix[3];
		for (int i = 0; i < 3; i++)
		{
			ref Matrix reference = ref array[i];
			reference = Matrix.CreateRotationZ(BaseGame.TotalTime / 3.9f) * Matrix.CreateTranslation(new Vector3(0f, 5f, 0f)) * Matrix.CreateRotationZ(0f - carSelectionRotationZ + (float)i * num) * Matrix.CreateTranslation(new Vector3(1.5f, 0f, 1f));
		}
		RacingGameManager.Player.SetCarPosition(Vector3.Zero, new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f));
		for (int j = 0; j < 3; j++)
		{
			RacingGameManager.CarSelectionPlate.Render(array[j]);
			RacingGameManager.CarModel.RenderCar(j, RacingGameManager.CarColor, shadowCarMode: false, array[j]);
		}
		BaseGame.MeshRenderManager.Render();
		if (BaseGame.AllowShadowMapping)
		{
			ShaderEffect.shadowMapping.ShowShadows();
		}
		BaseGame.WorldMatrix = Matrix.Identity;
		BaseGame.ViewMatrix = viewMatrix;
	}

	public CarSelection()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		gfxBarFromOptionsScreen = new Rectangle(372, 297, 472, 6);
		base._002Ector();
	}
}
