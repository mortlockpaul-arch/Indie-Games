using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Kobingo.Xna.Library.Common;
using Kobingo.Xna.Library.Data;
using Kobingo.Xna.Library.Game;
using Kobingo.Xna.Library.Graphics;
using Kobingo.Xna.Library.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Games.Painter;

internal class PainterPlayScreen : PlayScreen
{
	public const NetworkSessionType SESSION_TYPE = (NetworkSessionType)2;

	public const int MAX_GAMERS = 4;

	public const int MAX_HISTORY_PICTURES = 20;

	public Texture2D Picture { get; private set; }

	public TextureRenderer Renderer { get; private set; }

	public PacketWriter Writer { get; private set; }

	public PacketReader Reader { get; private set; }

	public List<Texture2D> History { get; private set; }

	public int HistoryIndex { get; private set; }

	public bool Changed { get; private set; }

	public TickTimer SendDataTimer { get; set; }

	public PainterGameMenu PainterGameMenu { get; private set; }

	public UnlockScreen UnlockScreen { get; private set; }

	public Transition<PainterState> Transition { get; set; }

	public Transition<ColorPalette> PaletteTransition { get; set; }

	public Transition<int> DisplayPaletteTransition { get; set; }

	public List<ColorPalette> Palettes { get; set; }

	public float Progress { get; set; }

	public TickTimer TutorialTimer { get; set; }

	public Transition<int> TutorialTransition { get; set; }

	public int TutorialIndex { get; set; }

	public Transition<int> DisplayTutorialTransition { get; set; }

	public PainterSessionType SessionType { get; private set; }

	public PainterPlayScreen(ScreenManager screenManager)
		: base(screenManager)
	{
		History = new List<Texture2D>();
		EventHandler value = delegate
		{
			NetworkManager.Session.AllowHostMigration = true;
			NetworkManager.Session.AllowJoinInProgress = true;
			NetworkManager.Session.GamerJoined += OnGamerJoined;
			NetworkManager.Session.GamerLeft += OnGamerLeft;
		};
		NetworkManager.Created += value;
		NetworkManager.Closed += delegate
		{
			NetworkManager.Session.GamerJoined -= OnGamerJoined;
		};
		SendDataTimer = new TickTimer(TimeSpan.FromSeconds(0.02500000037252903));
		SendDataTimer.Tick += delegate
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (NetworkManager.Session != null)
			{
				GamerCollectionEnumerator<LocalNetworkGamer> enumerator = NetworkManager.Session.LocalGamers.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						LocalNetworkGamer current = enumerator.Current;
						SendPacketDataMoving(current);
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
		};
		PainterGameMenu = new PainterGameMenu(screenManager, this);
		base.GameMenu = PainterGameMenu;
		UnlockScreen = new UnlockScreen(screenManager);
		UnlockScreen.IsPopup = true;
		Transition = new Transition<PainterState>();
		PaletteTransition = new Transition<ColorPalette>();
		DisplayPaletteTransition = new Transition<int>();
		TutorialTimer = new TickTimer(TimeSpan.FromSeconds(8.0));
		TutorialTimer.Tick += delegate
		{
			if (++TutorialIndex > 5)
			{
				TutorialTimer.Enabled = false;
				TutorialIndex = 1;
				TutorialTransition.Change(0, TimeSpan.FromMilliseconds(1.0), wait: true);
			}
			else
			{
				TutorialTransition.Change(TutorialIndex, TimeSpan.FromSeconds(1.0), wait: true);
			}
		};
		TutorialTimer.Enabled = false;
		TutorialTransition = new Transition<int>();
		TutorialIndex = 1;
		DisplayTutorialTransition = new Transition<int>();
	}

	public override void HandleInput()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (GamepadManager.IsButtonPressed(GameManager.ActiveGamer.PlayerIndex, (Buttons)8388608))
		{
			Undo();
		}
		if (GamepadManager.IsButtonPressed(GameManager.ActiveGamer.PlayerIndex, (Buttons)4194304))
		{
			Redo();
		}
		if (NetworkManager.Session != null)
		{
			GamerCollectionEnumerator<LocalNetworkGamer> enumerator = NetworkManager.Session.LocalGamers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					LocalNetworkGamer current = enumerator.Current;
					if (((Gamer)current).Tag != null)
					{
						(((Gamer)current).Tag as PainterPlayer).ProcessInput(base.ScreenManager.TitleSafeArea);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if (!TutorialTimer.Enabled && GamepadManager.IsButtonPressed(GameManager.ActiveGamer.PlayerIndex, (Buttons)32))
		{
			TutorialTimer.Enabled = true;
			TutorialTimer.Reset();
			TutorialTransition.Change(1, TimeSpan.FromSeconds(1.0), wait: true);
		}
		base.HandleInput();
	}

	public override void Update(GameTime gameTime, bool active)
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if (active)
		{
			TutorialTimer.Update(gameTime);
		}
		foreach (ColorPalette palette in Palettes)
		{
			palette.Animation = 0f;
		}
		foreach (TransitionState<ColorPalette> state in PaletteTransition.States)
		{
			state.Value.Animation = state.Transition * 20f;
		}
		TutorialTransition.Update(gameTime);
		DisplayTutorialTransition.Update(gameTime);
		Transition.Update(gameTime);
		PaletteTransition.Update(gameTime);
		DisplayPaletteTransition.Update(gameTime);
		if (NetworkManager.Session != null)
		{
			GamerCollectionEnumerator<LocalNetworkGamer> enumerator3 = NetworkManager.Session.LocalGamers.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					LocalNetworkGamer current3 = enumerator3.Current;
					while (current3.IsDataAvailable)
					{
						NetworkGamer val = null;
						current3.ReceiveData(Reader, ref val);
						if (!val.IsLocal)
						{
							ReadPacketData(val);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
			}
			GamerCollectionEnumerator<NetworkGamer> enumerator4 = NetworkManager.Session.AllGamers.GetEnumerator();
			try
			{
				while (enumerator4.MoveNext())
				{
					NetworkGamer current4 = enumerator4.Current;
					if (((Gamer)current4).Tag is PainterPlayer painterPlayer)
					{
						painterPlayer.Update(gameTime);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
			}
			NetworkManager.Session.Update();
		}
		SendDataTimer.Update(gameTime);
		Progress += 0.03f;
		base.Update(gameTime, active);
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)0);
		base.ScreenManager.SpriteBatch.Draw(Picture, base.ScreenManager.TitleSafeArea, Color.White);
		base.ScreenManager.SpriteBatch.End();
		base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
		foreach (TransitionState<int> state in DisplayTutorialTransition.States)
		{
			if (state.Value == 1)
			{
				Vector2 position = base.ScreenManager.ScreenCenter + new Vector2(0f, 260f - 25f * state.Transition);
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, "You can watch the tutorial at any time by pressing 'BACK' button", position, Align.Center, new Color(Color.DimGray, state.Transition));
			}
		}
		foreach (TransitionState<int> state2 in DisplayPaletteTransition.States)
		{
			if (state2.Value == 1)
			{
				for (int i = 0; i < Palettes.Count; i++)
				{
					Palettes[i].Draw(base.ScreenManager.SpriteBatch, state2.Transition);
				}
			}
		}
		base.ScreenManager.SpriteBatch.Draw(Graphics.Border, Vector2.Zero, Color.SteelBlue);
		if (NetworkManager.Session != null)
		{
			GamerCollectionEnumerator<NetworkGamer> enumerator3 = NetworkManager.Session.AllGamers.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					NetworkGamer current3 = enumerator3.Current;
					if (((Gamer)current3).Tag != null)
					{
						PainterPlayer painterPlayer = ((Gamer)current3).Tag as PainterPlayer;
						painterPlayer.Draw(base.ScreenManager.SpriteBatch);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
			}
		}
		foreach (TransitionState<int> state3 in TutorialTransition.States)
		{
			Texture2D val = null;
			switch (state3.Value)
			{
			case 1:
				val = Graphics.Controls1;
				break;
			case 2:
				val = Graphics.Controls2;
				break;
			case 3:
				val = Graphics.Controls3;
				break;
			case 4:
				val = Graphics.Controls4;
				break;
			case 5:
				val = Graphics.Controls5;
				break;
			}
			if (val != null)
			{
				base.ScreenManager.SpriteBatch.Draw(val, Vector2.Zero, new Color(Color.White, state3.Transition));
			}
		}
		Vector2 val2 = default(Vector2);
		foreach (TransitionState<PainterState> state4 in Transition.States)
		{
			string text = string.Empty;
			switch (state4.Value)
			{
			case PainterState.Saving:
				text = "Saving picture to gallery...";
				break;
			case PainterState.Connecting:
				text = "Searching for someone to draw with...";
				break;
			case PainterState.Waiting:
				text = "Waiting for a friend...";
				break;
			case PainterState.Joining:
				text = "Joining a friend...";
				break;
			case PainterState.OutOfSync:
				text = "Everyone is not synchronized (press 'START' then 'Create New' to get it right)";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Rectangle titleSafeArea = base.ScreenManager.TitleSafeArea;
				float num = ((Rectangle)(ref titleSafeArea)).Left + 55;
				Rectangle titleSafeArea2 = base.ScreenManager.TitleSafeArea;
				((Vector2)(ref val2))._002Ector(num, (float)(((Rectangle)(ref titleSafeArea2)).Top + 47));
				base.ScreenManager.SpriteBatch.DrawAligned(Graphics.Progress, val2, Progress, 1f, Align.Center, new Color(Color.DimGray, state4.Transition));
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, text, val2 + new Vector2(37f, -20f), Align.Left, new Color(Color.DimGray, state4.Transition));
			}
		}
		base.ScreenManager.SpriteBatch.End();
		base.Draw(gameTime, transition);
	}

	public void Show(PainterSessionType type, Texture2D picture)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		SessionType = type;
		New();
		if (picture != null)
		{
			Picture = picture;
			SaveHistoryPicture();
		}
		Transition.Clear();
		TutorialTransition.Clear();
		DisplayTutorialTransition.Clear();
		DisplayPaletteTransition.Clear();
		Renderer = new TextureRenderer(((DrawableGameComponent)base.ScreenManager).GraphicsDevice, 1024, 576);
		Writer = new PacketWriter();
		Reader = new PacketReader();
		switch (type)
		{
		case PainterSessionType.Local:
			NetworkManager.Create((NetworkSessionType)0, null, new List<SignedInGamer> { GameManager.ActiveGamer }, 4, 0);
			break;
		case PainterSessionType.Public:
			NetworkManager.FindCreate((NetworkSessionType)2, null, new List<SignedInGamer> { GameManager.ActiveGamer }, 4, 0);
			Transition.Change(PainterState.Connecting, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
			break;
		case PainterSessionType.Private:
			NetworkManager.Create((NetworkSessionType)2, null, new List<SignedInGamer> { GameManager.ActiveGamer }, 4, 3);
			Transition.Change(PainterState.Waiting, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
			break;
		case PainterSessionType.Invited:
		{
			List<SignedInGamer> list = new List<SignedInGamer>();
			list.Add(GameManager.ActiveGamer);
			NetworkManager.JoinInvited(list);
			Transition.Change(PainterState.Joining, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
			break;
		}
		}
		Palettes = new List<ColorPalette>();
		Rectangle bounds = default(Rectangle);
		for (int i = 0; i < 15; i++)
		{
			int num = base.ScreenManager.TitleSafeArea.X + 45 + i * 64;
			Rectangle titleSafeArea = base.ScreenManager.TitleSafeArea;
			((Rectangle)(ref bounds))._002Ector(num, ((Rectangle)(ref titleSafeArea)).Bottom - 75, 45, 45);
			Palettes.Add(new ColorPalette(bounds, (PainterColor)i));
		}
		TutorialIndex = 1;
		TutorialTimer.Reset();
		TutorialTimer.Enabled = false;
		if (PainterGame.FirstRun)
		{
			TutorialTimer.Enabled = true;
			TutorialTransition.Change(1, TimeSpan.FromSeconds(1.0), wait: true);
			PainterGame.FirstRun = false;
		}
		else
		{
			DisplayTutorialTransition.Change(1, TimeSpan.FromSeconds(0.5), wait: true, TimeSpan.FromSeconds(7.0));
		}
		Show();
	}

	public override void Close()
	{
		NetworkManager.Close();
		base.Close();
	}

	public void Save()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (StorageManager.IsBusy)
		{
			return;
		}
		if (Guide.IsTrialMode)
		{
			UnlockScreen.Show();
			return;
		}
		if (Transition.Current == PainterState.None)
		{
			Transition.Change(PainterState.Saving, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
		}
		StorageManager.PerformOperation(GameManager.ActiveGamer.PlayerIndex, delegate(StorageContainer container)
		{
			if (container != null)
			{
				string path = Path.ChangeExtension(Guid.NewGuid().ToString(), "pic");
				string filepath = Path.Combine(container.Path, path);
				try
				{
					PainterHelper.SavePictureToFile(Picture, filepath);
				}
				catch
				{
				}
			}
			if (Transition.Current == PainterState.Saving)
			{
				Transition.Change(PainterState.None, TimeSpan.FromMilliseconds(1.0), wait: true, TimeSpan.Zero);
			}
		});
	}

	public void New()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		History.Clear();
		Picture = new Texture2D(((DrawableGameComponent)base.ScreenManager).GraphicsDevice, 1024, 576);
		Color[] array = (Color[])(object)new Color[Picture.Width * Picture.Height];
		for (int i = 0; i < array.Length; i++)
		{
			ref Color reference = ref array[i];
			reference = Color.White;
		}
		Picture.SetData<Color>(array);
		SaveHistoryPicture();
		if (Transition.Current == PainterState.OutOfSync)
		{
			Transition.Change(PainterState.None, TimeSpan.FromMilliseconds(1.0), wait: true, TimeSpan.FromMilliseconds(1.0));
		}
		Changed = false;
	}

	private void Undo()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (NetworkManager.Session != null && (int)NetworkManager.Session.SessionType == 0 && !Guide.IsTrialMode && HistoryIndex > 0)
		{
			HistoryIndex--;
			Picture = History[HistoryIndex];
		}
	}

	private void Redo()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (NetworkManager.Session != null && (int)NetworkManager.Session.SessionType == 0 && !Guide.IsTrialMode && HistoryIndex < History.Count - 1)
		{
			HistoryIndex++;
			Picture = History[HistoryIndex];
		}
	}

	private void SaveHistoryPicture()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		Color[] array = (Color[])(object)new Color[Picture.Width * Picture.Height];
		Picture.GetData<Color>(array);
		Texture2D val = new Texture2D(((DrawableGameComponent)base.ScreenManager).GraphicsDevice, Picture.Width, Picture.Height);
		val.SetData<Color>(array);
		while (HistoryIndex < History.Count - 1)
		{
			History.RemoveAt(History.Count - 1);
		}
		History.Add(val);
		if (History.Count > 20)
		{
			History.RemoveAt(0);
		}
		HistoryIndex = History.Count - 1;
	}

	private void OnGamerJoined(object sender, GamerJoinedEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (e.Gamer.IsLocal)
		{
			PainterPlayer painterPlayer = new PainterPlayer((LocalNetworkGamer)e.Gamer, this);
			painterPlayer.Painting += Paint;
			painterPlayer.StoppedPainting += delegate
			{
				SaveHistoryPicture();
			};
			painterPlayer.Painting += delegate(PainterPlayer p, PainterType type, Vector2 location, float size, PainterColor color)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				SendPacketDataPainting(p.LocalNetworkGamer, type, location, size, color);
			};
			painterPlayer.Cursor = base.ScreenManager.ScreenCenter;
			((Gamer)e.Gamer).Tag = painterPlayer;
		}
		else
		{
			((Gamer)e.Gamer).Tag = new PainterPlayer(e.Gamer, this);
			if (Changed && NetworkManager.Session.IsHost)
			{
				Transition.Change(PainterState.OutOfSync, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
			}
			else
			{
				Transition.Change(PainterState.None, TimeSpan.FromMilliseconds(1.0), wait: true, TimeSpan.FromMilliseconds(1.0));
			}
		}
	}

	private void OnGamerLeft(object sender, GamerLeftEventArgs e)
	{
		if (((ReadOnlyCollection<NetworkGamer>)(object)NetworkManager.Session.AllGamers).Count == 1)
		{
			switch (SessionType)
			{
			case PainterSessionType.Public:
				Transition.Change(PainterState.Connecting, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
				break;
			case PainterSessionType.Private:
			case PainterSessionType.Invited:
				Transition.Change(PainterState.Waiting, TimeSpan.FromSeconds(1.0), wait: true, TimeSpan.Zero);
				break;
			}
		}
	}

	private void Paint(PainterPlayer player, PainterType type, Vector2 location, float size, PainterColor color)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		float x = location.X;
		Rectangle titleSafeArea = base.ScreenManager.TitleSafeArea;
		float num = x - (float)((Rectangle)(ref titleSafeArea)).Left;
		float y = location.Y;
		Rectangle titleSafeArea2 = base.ScreenManager.TitleSafeArea;
		Vector2 position = default(Vector2);
		((Vector2)(ref position))._002Ector(num, y - (float)((Rectangle)(ref titleSafeArea2)).Top);
		switch (type)
		{
		case PainterType.Pencil:
		case PainterType.Brush:
		{
			Renderer.Begin(Color.White);
			base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)0, (SpriteSortMode)0, (SaveStateMode)0);
			((DrawableGameComponent)base.ScreenManager).GraphicsDevice.SamplerStates[0].MagFilter = (TextureFilter)1;
			((DrawableGameComponent)base.ScreenManager).GraphicsDevice.SamplerStates[0].MipFilter = (TextureFilter)1;
			((DrawableGameComponent)base.ScreenManager).GraphicsDevice.SamplerStates[0].MinFilter = (TextureFilter)1;
			((DrawableGameComponent)base.ScreenManager).GraphicsDevice.RenderState.MultiSampleAntiAlias = false;
			base.ScreenManager.SpriteBatch.Draw(Picture, Vector2.Zero, Color.White);
			base.ScreenManager.SpriteBatch.End();
			Texture2D texture = null;
			switch (type)
			{
			case PainterType.Pencil:
				texture = Graphics.Pencil1;
				break;
			case PainterType.Brush:
				texture = Graphics.Brush1;
				break;
			}
			base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
			base.ScreenManager.SpriteBatch.DrawAligned(texture, position, 0f, size, Align.Center, GetColor(color));
			base.ScreenManager.SpriteBatch.End();
			Renderer.End();
			Picture = Renderer.GetTexture();
			break;
		}
		case PainterType.Bucket:
		{
			Color[] array = (Color[])(object)new Color[Picture.Width * Picture.Height];
			Picture.GetData<Color>(array);
			int num2 = (int)player.Cursor.X;
			Rectangle titleSafeArea3 = base.ScreenManager.TitleSafeArea;
			int num3 = num2 - ((Rectangle)(ref titleSafeArea3)).Left;
			int num4 = (int)player.Cursor.Y;
			Rectangle titleSafeArea4 = base.ScreenManager.TitleSafeArea;
			int num5 = num4 - ((Rectangle)(ref titleSafeArea4)).Top;
			Color targetColor = array[num3 + num5 * Picture.Width];
			FloodFill.Fill(array, new Point(num3, num5), GetColor(color), targetColor, Picture.Width, Picture.Height);
			Picture = new Texture2D(((DrawableGameComponent)base.ScreenManager).GraphicsDevice, Picture.Width, Picture.Height);
			Picture.SetData<Color>(array);
			break;
		}
		}
		Changed = true;
	}

	private void ReadPacketData(NetworkGamer sender)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (!(((Gamer)sender).Tag is PainterPlayer painterPlayer))
		{
			return;
		}
		switch ((NetworkMessageType)((BinaryReader)(object)Reader).ReadByte())
		{
		case NetworkMessageType.Painting:
		{
			Vector2 location = Reader.ReadVector2();
			PainterColor color = (PainterColor)((BinaryReader)(object)Reader).ReadByte();
			PainterType type = (PainterType)((BinaryReader)(object)Reader).ReadByte();
			Paint(painterPlayer, type, location, 1f, color);
			break;
		}
		case NetworkMessageType.Moving:
		{
			Vector2 val = Reader.ReadVector2();
			PainterType painterType = (PainterType)((BinaryReader)(object)Reader).ReadByte();
			Vector2 cursor = painterPlayer.Cursor;
			float num = Math.Abs(((Vector2)(ref cursor)).Length() - ((Vector2)(ref val)).Length());
			if (num > 20f)
			{
				painterPlayer.Cursor = val;
				painterPlayer.MovingTo = val;
			}
			else
			{
				painterPlayer.MovingTo = val;
			}
			if (painterPlayer.TypeTransition.Current != painterType)
			{
				painterPlayer.TypeTransition.Change(painterType, TimeSpan.FromSeconds(0.20000000298023224), wait: true);
			}
			break;
		}
		case NetworkMessageType.CreateNew:
			New();
			break;
		}
	}

	private void SendPacketDataPainting(LocalNetworkGamer gamer, PainterType type, Vector2 location, float size, PainterColor color)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((BinaryWriter)(object)Writer).Write((byte)0);
		Writer.Write(location);
		((BinaryWriter)(object)Writer).Write((byte)color);
		((BinaryWriter)(object)Writer).Write((byte)type);
		gamer.SendData(Writer, (SendDataOptions)3);
	}

	private void SendPacketDataMoving(LocalNetworkGamer gamer)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((Gamer)gamer).Tag != null)
		{
			PainterPlayer painterPlayer = ((Gamer)gamer).Tag as PainterPlayer;
			((BinaryWriter)(object)Writer).Write((byte)1);
			Writer.Write(painterPlayer.Cursor);
			((BinaryWriter)(object)Writer).Write((byte)painterPlayer.TypeTransition.Current);
			gamer.SendData(Writer, (SendDataOptions)2);
		}
	}

	public void SendPacketDataCreateNew(LocalNetworkGamer gamer)
	{
		((BinaryWriter)(object)Writer).Write((byte)2);
		gamer.SendData(Writer, (SendDataOptions)3);
	}

	public static Color GetColor(PainterColor color)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(color switch
		{
			PainterColor.Black => Color.Black, 
			PainterColor.Blue => Color.Blue, 
			PainterColor.Green => Color.Green, 
			PainterColor.Yellow => Color.Yellow, 
			PainterColor.Red => Color.Red, 
			PainterColor.Orange => Color.Orange, 
			PainterColor.Pink => Color.Pink, 
			PainterColor.Brown => Color.Brown, 
			PainterColor.Purple => Color.Violet, 
			PainterColor.Lime => Color.Lime, 
			PainterColor.SkyBlue => Color.SkyBlue, 
			PainterColor.Silver => Color.Silver, 
			PainterColor.Gray => Color.Gray, 
			PainterColor.Beige => Color.Bisque, 
			PainterColor.White => Color.White, 
			_ => Color.White, 
		});
	}
}
