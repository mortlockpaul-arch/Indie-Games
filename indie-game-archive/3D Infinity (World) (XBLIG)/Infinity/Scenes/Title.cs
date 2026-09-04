using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Infinity.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using XnaLibrary;
using XnaLibrary.Blade;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class Title : AnaglyphScene
{
	public enum Phase
	{
		None,
		DeviceSelect,
		DeviceSelectOperation,
		ShowMessageBox,
		SelectMessageBox,
		SelectMenu
	}

	private readonly TimeSpan DefaultRetryWait = new TimeSpan(0, 0, 2);

	private Phase phase;

	private bool isBgmFade;

	private TimeSpan retryWait;

	private XSIModel screenModel;

	private XSIModel startModel;

	private XSIModel[,] itemModels;

	private int selectIndex;

	private string copyrightText;

	private string versionText;

	private MessageBoxComponent messageBox;

	public Title(Game game)
		: this(game, Phase.None)
	{
	}

	public Title(Game game, Phase phase)
		: base(game)
	{
		base.update += SceneUpdate;
		base.draw += SceneDraw;
		this.phase = phase;
	}

	public override void Initialize()
	{
		screenModel = new XSIModel("Models/Models/screen/screen_title", base.Content);
		screenModel.Play(isLoop: true);
		startModel = new XSIModel("Models/Models/screen/screen_press_start", base.Content);
		startModel.Play(isLoop: true);
		itemModels = new XSIModel[3, 2]
		{
			{
				new XSIModel("Models/Models/screen/title_start", base.Content),
				new XSIModel("Models/Models/screen/title_start_sel", base.Content)
			},
			{
				new XSIModel("Models/Models/screen/title_option", base.Content),
				new XSIModel("Models/Models/screen/title_option_sel", base.Content)
			},
			{
				new XSIModel("Models/Models/screen/title_quit", base.Content),
				new XSIModel("Models/Models/screen/title_quit_sel", base.Content)
			}
		};
		XSIModel[,] array = itemModels;
		foreach (XSIModel xSIModel in array)
		{
			xSIModel.Play(isLoop: true);
		}
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		copyrightText = GetCopyright();
		AssemblyName name = executingAssembly.GetName();
		versionText = ((name != null) ? name.Version.ToString() : string.Empty);
		messageBox = new MessageBoxComponent(base.Game);
		((Collection<IGameComponent>)(object)base.Game.Components).Add((IGameComponent)(object)messageBox);
		if (Global.bgm == null)
		{
			Global.bgm = base.Sound.PlayBGM("StreamBGM", "BGM_Title");
			base.Sound.SetVolume(Global.bgm, 1f);
		}
		base.Initialize();
	}

	public override void Dispose()
	{
		base.Content.Unload();
		((GameComponent)messageBox).Dispose();
		if (isBgmFade)
		{
			Global.bgm = null;
		}
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
			else if (fadePhase == FadePhase.Out && isBgmFade)
			{
				base.Sound.SetVolume(Global.bgm, 1f - base.Fade.GetAmount());
			}
		}
		UpdateModels(gameTime);
		if (retryWait.TotalSeconds > 0.0)
		{
			retryWait -= gameTime.ElapsedGameTime;
		}
	}

	private void UpdateMain(GameTime gameTime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (phase == Phase.None && retryWait.TotalSeconds <= 0.0)
		{
			for (PlayerIndex val = (PlayerIndex)0; (int)val <= 3; val = (PlayerIndex)(val + 1))
			{
				VirtualPadState virtualPadState = base.Input[val];
				VirtualPadButtons buttons = virtualPadState.Buttons;
				if (InputState.IsPush(buttons.A) || InputState.IsPush(buttons.Start))
				{
					Global.CurrentPlayer = val;
					SetPhase(Phase.DeviceSelect);
					base.Sound.PlaySE("SE10");
				}
			}
		}
		else if (phase == Phase.DeviceSelect)
		{
			SetPhase(Phase.DeviceSelectOperation);
			base.Storage.ClearAllEvents();
			base.Storage.DeviceSelected += Storage_DeviceSelected;
			base.Storage.ShowStorageDeviceSelector("3D Infinity", null, 0, 0);
		}
		else if (phase == Phase.ShowMessageBox && !messageBox.IsVisible)
		{
			SetPhase(Phase.SelectMessageBox);
			messageBox.ShowMessageBox(Global.CurrentPlayer, " ", UIMessage.StorageCancel, new string[2]
			{
				UIMessage.Yes,
				UIMessage.No
			}, 1, (MessageBoxIcon)2);
			messageBox.RemoveSelectedEvents();
			messageBox.Selected += messageBox_Selected;
		}
		else if (phase != Phase.SelectMessageBox && phase == Phase.SelectMenu)
		{
			HandleInputMenu();
		}
	}

	private void HandleInputMenu()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		VirtualPadButtons buttons = virtualPadState.Buttons;
		VirtualPadDPad left = virtualPadState.ThumbSticks.Left;
		VirtualPadDPad dPad = virtualPadState.DPad;
		int length = itemModels.GetLength(0);
		if (InputState.IsPush(left.Up) || InputState.IsPush(dPad.Up))
		{
			selectIndex = (selectIndex + (length - 1)) % length;
			base.Sound.PlaySE("SE02");
		}
		if (InputState.IsPush(left.Down) || InputState.IsPush(dPad.Down))
		{
			selectIndex = (selectIndex + 1) % length;
			base.Sound.PlaySE("SE02");
		}
		if (InputState.IsPush(buttons.A) || InputState.IsPush(buttons.Start))
		{
			base.Sound.PlaySE("SE10");
			if (selectIndex == 0)
			{
				isBgmFade = true;
				base.SceneManager.AddScene(new MainGameLoader(base.Game));
				FadeOut();
			}
			else if (selectIndex == 1)
			{
				base.SceneManager.AddScene(new Option(base.Game));
				FadeOut();
			}
			else if (selectIndex == 2)
			{
				FadeOut();
			}
		}
		else if (InputState.IsPush(buttons.B))
		{
			SetPhase(Phase.None);
			base.Storage.DisposeStorage();
		}
	}

	private void UpdateModels(GameTime gameTime)
	{
		screenModel.Update(gameTime);
		startModel.Update(gameTime);
		XSIModel[,] array = itemModels;
		foreach (XSIModel xSIModel in array)
		{
			xSIModel.Update(gameTime);
		}
	}

	private void Storage_DeviceSelected(bool isCancel)
	{
		if (isCancel)
		{
			SetPhase(Phase.ShowMessageBox);
			return;
		}
		Global.Load(base.Storage);
		SetDrawMode(Global.SaveData.DrawModeIndex);
		SetPhase(Phase.SelectMenu);
	}

	private void messageBox_Selected(object sender, MessageBoxComponent.EventResult e)
	{
		if (e.Result.HasValue && e.Result.Value == 0)
		{
			SetPhase(Phase.SelectMenu);
		}
		else
		{
			SetPhase(Phase.None);
		}
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		anaglyphRender.Draw(gameTime, base.SASData);
	}

	protected override void DrawScene(GameTime gameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		screenModel.Draw(base.SASData, Matrix.Identity);
		if (phase == Phase.SelectMenu)
		{
			for (int i = 0; i < itemModels.GetLength(0); i++)
			{
				int num = ((i == selectIndex) ? 1 : 0);
				XSIModel xSIModel = itemModels[i, num];
				xSIModel.Draw(base.SASData, Matrix.Identity);
			}
		}
		else if (phase == Phase.None && retryWait.TotalSeconds <= 0.0)
		{
			startModel.Draw(base.SASData, Matrix.Identity);
		}
		base.DrawScene(gameTime);
	}

	private string GetCopyright()
	{
		string result = string.Empty;
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		object[] customAttributes = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), inherit: false);
		if (customAttributes != null && customAttributes.Length > 0)
		{
			result = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
		}
		return result;
	}

	private string GetVersion()
	{
		string empty = string.Empty;
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		AssemblyName name = executingAssembly.GetName();
		return (name != null) ? name.Version.ToString() : string.Empty;
	}

	private void SetPhase(Phase nextPhase)
	{
		phase = nextPhase;
		if (phase == Phase.None)
		{
			retryWait = DefaultRetryWait;
		}
	}
}
