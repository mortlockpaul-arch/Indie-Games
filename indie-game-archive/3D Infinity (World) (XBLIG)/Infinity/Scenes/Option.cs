using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class Option : AnaglyphScene
{
	private enum ItemIndex
	{
		DrawMode,
		Difficult,
		MaxCount
	}

	private XSIModel screenModel;

	private XSIModel cursorModel;

	private XSIModel[,] drawModeModels;

	private XSIModel[,] difficultModels;

	private int selectIndex;

	private float cursorAmount;

	private readonly Vector3[] CursorPositions;

	public Option(Game game)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		CursorPositions = (Vector3[])(object)new Vector3[2]
		{
			new Vector3(-21f, 1.8f, -9f),
			new Vector3(5.6f, 1.8f, -9f)
		};
		base._002Ector(game);
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		screenModel = new XSIModel("Models/Models/screen/screen_option", base.Content);
		screenModel.Play(isLoop: true);
		cursorModel = new XSIModel("Models/Models/screen/option_cursor", base.Content);
		cursorModel.Play(isLoop: true);
		drawModeModels = new XSIModel[3, 2]
		{
			{
				new XSIModel("Models/Models/screen/option_normal", base.Content),
				new XSIModel("Models/Models/screen/option_normal_sel", base.Content)
			},
			{
				new XSIModel("Models/Models/screen/option_anaglyph", base.Content),
				new XSIModel("Models/Models/screen/option_anaglyph_sel", base.Content)
			},
			{
				new XSIModel("Models/Models/screen/option_sidebyside", base.Content),
				new XSIModel("Models/Models/screen/option_sidebyside_sel", base.Content)
			}
		};
		XSIModel[,] array = drawModeModels;
		foreach (XSIModel xSIModel in array)
		{
			xSIModel.Play(isLoop: true);
		}
		difficultModels = new XSIModel[3, 2]
		{
			{
				new XSIModel("Models/Models/screen/GL_easy", base.Content),
				new XSIModel("Models/Models/screen/GL_easy_sel", base.Content)
			},
			{
				new XSIModel("Models/Models/screen/GL_normal", base.Content),
				new XSIModel("Models/Models/screen/GL_normal_sel", base.Content)
			},
			{
				new XSIModel("Models/Models/screen/GL_hard", base.Content),
				new XSIModel("Models/Models/screen/GL_hard_sel", base.Content)
			}
		};
		XSIModel[,] array2 = difficultModels;
		foreach (XSIModel xSIModel2 in array2)
		{
			xSIModel2.Play(isLoop: true);
		}
		base.Initialize();
	}

	public override void Dispose()
	{
		Global.Save(base.Storage);
		base.Content.Unload();
		base.Dispose();
	}

	private void SceneUpdate(object sender, GameTime gameTime)
	{
		if (fadePhase != FadePhase.In)
		{
			if (fadePhase == FadePhase.Main)
			{
				UpdateMain(gameTime);
			}
			else
			{
				_ = fadePhase;
				_ = 2;
			}
		}
		UpdateModels(gameTime);
	}

	private void UpdateModels(GameTime gameTime)
	{
		screenModel.Update(gameTime);
		cursorModel.Update(gameTime);
		XSIModel[,] array = drawModeModels;
		foreach (XSIModel xSIModel in array)
		{
			xSIModel.Update(gameTime);
		}
		XSIModel[,] array2 = difficultModels;
		foreach (XSIModel xSIModel2 in array2)
		{
			xSIModel2.Update(gameTime);
		}
	}

	private void UpdateMain(GameTime gameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		UpdateItemIndex();
		UpdateItemValue();
		ChangeDrawMode(Global.SaveData.DrawModeIndex);
		if (InputState.IsPush(base.Input[Global.CurrentPlayer].Buttons.B))
		{
			base.Sound.PlaySE("SE10");
			base.SceneManager.AddScene(new Title(base.Game, Title.Phase.SelectMenu));
			FadeOut();
		}
		else
		{
			cursorAmount = MathHelper.Clamp(cursorAmount + ((selectIndex == 0) ? (-0.1f) : 0.1f), 0f, 1f);
			cursorModel.Position = Vector3.SmoothStep(CursorPositions[0], CursorPositions[1], cursorAmount);
		}
	}

	private void UpdateItemIndex()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		_ = virtualPadState.Buttons;
		VirtualPadDPad left = virtualPadState.ThumbSticks.Left;
		VirtualPadDPad dPad = virtualPadState.DPad;
		if (InputState.IsPush(left.Left) || InputState.IsPush(dPad.Left))
		{
			selectIndex = (selectIndex + 1) % 2;
			base.Sound.PlaySE("SE02");
		}
		else if (InputState.IsPush(left.Right) || InputState.IsPush(dPad.Right))
		{
			selectIndex = (selectIndex + 1) % 2;
			base.Sound.PlaySE("SE02");
		}
	}

	private void UpdateItemValue()
	{
		Action[] array = new Action[2] { UpdateItemValue_DrawMode, UpdateItemValue_Difficult };
		array[selectIndex]();
	}

	private void UpdateItemValue_DrawMode()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		_ = virtualPadState.Buttons;
		VirtualPadDPad left = virtualPadState.ThumbSticks.Left;
		VirtualPadDPad dPad = virtualPadState.DPad;
		if (InputState.IsPush(left.Up) || InputState.IsPush(dPad.Up))
		{
			Global.SaveData.DrawModeIndex = (Global.SaveData.DrawModeIndex + 2) % 3;
			base.Sound.PlaySE("SE02");
		}
		else if (InputState.IsPush(left.Down) || InputState.IsPush(dPad.Down))
		{
			Global.SaveData.DrawModeIndex = (Global.SaveData.DrawModeIndex + 1) % 3;
			base.Sound.PlaySE("SE02");
		}
	}

	private void UpdateItemValue_Difficult()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		_ = virtualPadState.Buttons;
		VirtualPadDPad left = virtualPadState.ThumbSticks.Left;
		VirtualPadDPad dPad = virtualPadState.DPad;
		if (InputState.IsPush(left.Up) || InputState.IsPush(dPad.Up))
		{
			Global.SaveData.DifficultIndex = (Global.SaveData.DifficultIndex + 2) % 3;
			base.Sound.PlaySE("SE02");
		}
		else if (InputState.IsPush(left.Down) || InputState.IsPush(dPad.Down))
		{
			Global.SaveData.DifficultIndex = (Global.SaveData.DifficultIndex + 1) % 3;
			base.Sound.PlaySE("SE02");
		}
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		anaglyphRender.Draw(gameTime, base.SASData);
	}

	protected override void DrawScene(GameTime gameTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		screenModel.Draw(base.SASData);
		cursorModel.Draw(base.SASData, Matrix.CreateTranslation(cursorModel.Position));
		for (int i = 0; i < 3; i++)
		{
			int num = ((i == Global.SaveData.DrawModeIndex) ? 1 : 0);
			XSIModel xSIModel = drawModeModels[i, num];
			xSIModel.Draw(base.SASData, Matrix.Identity);
		}
		for (int j = 0; j < 3; j++)
		{
			int num2 = ((j == Global.SaveData.DifficultIndex) ? 1 : 0);
			XSIModel xSIModel2 = difficultModels[j, num2];
			xSIModel2.Draw(base.SASData, Matrix.Identity);
		}
		base.DrawScene(gameTime);
	}

	private void ChangeDrawMode(int drawModeIndex)
	{
		Global.SaveData.DrawModeIndex = drawModeIndex;
		SetDrawMode(Global.SaveData.DrawModeIndex);
	}
}
