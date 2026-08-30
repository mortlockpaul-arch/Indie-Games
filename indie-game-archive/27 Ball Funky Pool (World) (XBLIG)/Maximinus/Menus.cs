using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Menus
{
	public enum AlignX
	{
		Left,
		Center,
		Right
	}

	public enum BoolEntryType
	{
		TrueFalse,
		OnOff,
		YesNo
	}

	public class Manager
	{
		protected Drawing2D draw2D;

		protected List<Screen> screensMenus;

		protected List<ScreenInfo> screensInfo;

		protected int wantedScreenId = -1;

		public int WantedScreenId
		{
			set
			{
				wantedScreenId = value;
			}
		}

		public virtual Rectangle CurrentOverlay
		{
			get
			{
				Screen activeScreen = ActiveScreen;
				Rectangle overlay = activeScreen.Overlay;
				Rectangle overlayTitle = activeScreen.OverlayTitle;
				overlay.Width = Math.Max(overlay.Width, overlayTitle.Width);
				overlay.Height += overlayTitle.Height;
				overlay.X = Math.Min(overlay.X, overlayTitle.X);
				overlay.Y = overlayTitle.Y;
				return overlay;
			}
		}

		protected Screen ActiveScreen
		{
			get
			{
				foreach (Screen screensMenu in screensMenus)
				{
					if (screensMenu.state == Screen.State.Active || screensMenu.state == Screen.State.TransitionOn)
					{
						return screensMenu;
					}
				}
				throw new Exception("no active screen");
			}
		}

		public bool AnyActivatedScreen
		{
			get
			{
				foreach (Screen screensMenu in screensMenus)
				{
					if (screensMenu.state == Screen.State.Active)
					{
						return true;
					}
				}
				return false;
			}
		}

		protected bool AnyActiveScreen
		{
			get
			{
				foreach (Screen screensMenu in screensMenus)
				{
					if (screensMenu.state == Screen.State.Active || screensMenu.state == Screen.State.TransitionOn)
					{
						return true;
					}
				}
				return false;
			}
		}

		public int ActiveScreenId => ActiveScreen.Id;

		public Manager(Drawing2D draw2D)
		{
			this.draw2D = draw2D;
			screensMenus = new List<Screen>();
			screensInfo = new List<ScreenInfo>();
		}

		public virtual void AddScreen(Screen s)
		{
			s.SetBlankTex(draw2D.BlankTex);
			screensMenus.Add(s);
		}

		public virtual void render(GameTime gameTime)
		{
			foreach (Screen screensMenu in screensMenus)
			{
				if (screensMenu.HasToBeDrawn)
				{
					screensMenu.render(draw2D, gameTime);
				}
			}
			foreach (ScreenInfo item in screensInfo)
			{
				if (item.HasToBeDrawn)
				{
					item.render(draw2D, gameTime);
				}
			}
		}

		public virtual void Update(GameTime gameTime)
		{
			foreach (Screen screensMenu in screensMenus)
			{
				if (screensMenu.HasToBeDrawn)
				{
					screensMenu.Update(gameTime);
				}
			}
			foreach (ScreenInfo item in screensInfo)
			{
				if (item.HasToBeDrawn)
				{
					item.Update(gameTime);
				}
			}
		}

		protected bool AnyActiveScreenNotMenu(List<ScreenInfo> outScreenNotMenu)
		{
			outScreenNotMenu.Clear();
			foreach (ScreenInfo item in screensInfo)
			{
				if (item.HasToBeDrawn)
				{
					outScreenNotMenu.Add(item);
				}
			}
			return outScreenNotMenu.Count > 0;
		}

		protected Screen FindScreen(int id)
		{
			foreach (Screen screensMenu in screensMenus)
			{
				if (screensMenu.Id == id)
				{
					return screensMenu;
				}
			}
			throw new Exception("screen id " + id + " not found");
		}

		protected ScreenInfo FindScreenInfo(int id)
		{
			foreach (ScreenInfo item in screensInfo)
			{
				if (item.Id == id)
				{
					return item;
				}
			}
			throw new Exception("screen id " + id + " not found");
		}

		protected virtual void ChangeFocus(int id)
		{
			foreach (Screen screensMenu in screensMenus)
			{
				screensMenu.DisableIfNecessary();
			}
			FindScreen(id).Enable();
		}

		public void Enable()
		{
			if (wantedScreenId != -1)
			{
				ChangeFocus(wantedScreenId);
				wantedScreenId = -1;
			}
			else
			{
				ChangeFocus(0);
			}
		}

		public void Disable()
		{
			ActiveScreen.Disable();
		}

		public virtual void HandleInput(Utils.Input.ActionMenu action)
		{
			ActiveScreen.HandleInput(action);
		}
	}

	public class ManagerV2 : Manager
	{
		public enum Style
		{
			RoundedRect,
			Texture
		}

		protected const float NotMenuTransitionTime = 0.4f;

		private Style style;

		private RoundedRectangle outline;

		public Color OverlayColor = Utils.ColorWithAlpha(Color.White, 0.665f);

		private Color colorOutline;

		public static int DefaultBorder = -1;

		private int decoWidth;

		protected Texture2D styleTextureBG;

		public int DecoWidth => decoWidth;

		public ManagerV2(Style style, Drawing2D draw2D, int decoWidth, int screenBorderDefaultWidth, Color decoColor)
			: base(draw2D)
		{
			this.style = style;
			this.decoWidth = decoWidth;
			outline = new RoundedRectangle(Rectangle.Empty);
			outline.TexWidth = decoWidth;
			outline.Color = decoColor;
			colorOutline = decoColor;
			DefaultBorder = screenBorderDefaultWidth;
		}

		public override void AddScreen(Screen s)
		{
			s.useDefaultDeco = false;
			base.AddScreen(s);
		}

		public void AddScreenInfo(int id, Vector2 posAsRatio, double maxTime, bool killAfterTimer, float depth)
		{
			ScreenInfo screenInfo = new ScreenInfo(id, "", AlignX.Center, useDefaultDeco: false, maxTime, killAfterTimer, -1, depth);
			screenInfo.RatioXOverride = posAsRatio.X;
			screenInfo.RatioYOverride = posAsRatio.Y;
			screenInfo.TransitionTimeSeconds = 0.4f;
			screensInfo.Add(screenInfo);
		}

		public void AddScreenInfo(ScreenInfo s)
		{
			s.TransitionTimeSeconds = 0.4f;
			screensInfo.Add(s);
		}

		public void SwitchAllScreenInfoWithID(GameTime gameTime, int id, bool value)
		{
			foreach (ScreenInfo item in screensInfo)
			{
				if (item.Id == id)
				{
					if (value)
					{
						item.Enable(gameTime);
					}
					else
					{
						item.Disable();
					}
				}
			}
		}

		public void EnableScreenInfo(GameTime gameTime, int id, List<string> strEntries)
		{
			EnableScreenInfo(gameTime, id, strEntries, new List<Color>());
		}

		public void EnableScreenInfo(GameTime gameTime, int id, List<string> strEntries, List<Color> colorOverrides)
		{
			ScreenInfo screenInfo = FindScreenInfo(id);
			screenInfo.entries.Clear();
			for (int i = 0; i < strEntries.Count; i++)
			{
				string text = strEntries[i];
				if (i < colorOverrides.Count)
				{
					screenInfo.AddNonSelectableEntry(text, overrideSelectionTransition: true, new List<Color> { colorOverrides[i] });
				}
				else
				{
					screenInfo.AddNonSelectableEntry(text, overrideSelectionTransition: true);
				}
			}
			screenInfo.DefaultSelection = -2;
			screenInfo.Enable(gameTime);
		}

		public override void render(GameTime gameTime)
		{
			if (base.AnyActiveScreen)
			{
				DrawDeco(base.ActiveScreen.Overlay, colorOutline, base.ActiveScreen.TransitionPosition, 1f);
				if (base.ActiveScreen.TitleIsSet)
				{
					DrawDeco(base.ActiveScreen.OverlayTitle, colorOutline, base.ActiveScreen.TransitionPosition, 1f);
				}
			}
			List<ScreenInfo> list = new List<ScreenInfo>();
			if (AnyActiveScreenNotMenu(list))
			{
				foreach (ScreenInfo item in list)
				{
					if (!item.HasColorOverlayOverride(out var col))
					{
						col = colorOutline;
					}
					DrawDeco(item.Overlay, col, item.TransitionPosition, item.Depth);
					if (item.TitleIsSet)
					{
						DrawDeco(item.OverlayTitle, colorOutline, item.TransitionPosition, item.Depth);
					}
				}
			}
			base.render(gameTime);
		}

		protected void DrawDeco(Rectangle overlay, Color colorOutline, float transition, float depth)
		{
			outline.Rect = overlay;
			outline.Color = Utils.ColorWithAlpha(colorOutline, transition);
			outline.Draw(draw2D.SpriteBatch);
			if (style == Style.Texture)
			{
				draw2D.SpriteBatch.Draw(styleTextureBG, overlay, null, Utils.ColorWithAlpha(OverlayColor, transition * (float)(int)OverlayColor.A / 255f), 0f, Vector2.Zero, SpriteEffects.None, depth);
			}
		}

		public static Rectangle OverlayWithOffset(Rectangle overlay)
		{
			int num = ((MaximinusGame.BackBufferSize == MaximinusGame.BackBufferSizeValue.HD_1080) ? 5 : 3);
			Rectangle result = overlay;
			result.X -= num;
			result.Y -= num;
			result.Width += 2 * num;
			result.Height += 2 * num;
			return result;
		}
	}

	public class Screen
	{
		public enum State
		{
			Active,
			TransitionOn,
			TransitionOff,
			Hidden
		}

		private const int offsetTitleY = 20;

		protected TimeSpan transitionTime = TimeSpan.FromSeconds(0.5);

		protected static float alphaMax = 0.8f;

		protected Rectangle overlay;

		public bool useDefaultDeco;

		protected Drawing2D draw2D;

		public int border = -1;

		private Texture2D blankTex;

		private float transitionPosition;

		private string title;

		private Vector2 titlePos;

		private Vector2 titleSize = Vector2.Zero;

		private Rectangle overlayTitle;

		public static Color colorStringSelected = Color.Red;

		public static Color colorStringNotSelected = Color.Black;

		private MenuEntry entry;

		public State state;

		private int id;

		protected AlignX alignX;

		protected float overrideRatioY;

		protected float overrideRatioX;

		private bool dontDisable;

		public float TransitionTimeSeconds
		{
			set
			{
				transitionTime = TimeSpan.FromSeconds(value);
			}
		}

		public Rectangle Overlay => overlay;

		public float TransitionPosition => transitionPosition;

		public Rectangle OverlayTitle => overlayTitle;

		public bool TitleIsSet => titleSize.X != 0f;

		private Color colorOverlay => Utils.ColorWithAlpha(Color.White, transitionPosition * alphaMax);

		public Color ColorStringNormal => ColorString(selected: false);

		public Color ColorTex => colorOverlay;

		public float RatioYOverride
		{
			set
			{
				overrideRatioY = value;
			}
		}

		public float RatioXOverride
		{
			set
			{
				overrideRatioX = value;
			}
		}

		public int Id => id;

		public bool DontDisable
		{
			set
			{
				dontDisable = value;
			}
		}

		public bool HasToBeDrawn => state != State.Hidden;

		public event EventHandler<Utils.EventArgsInteger> Activated;

		public event EventHandler<EventArgs> Cancelled;

		protected Color ColorString(bool selected)
		{
			return Utils.ColorWithAlpha(selected ? colorStringSelected : colorStringNotSelected, (state == State.TransitionOff) ? 0f : MathHelper.SmoothStep(0f, 1f, transitionPosition));
		}

		public void SetBlankTex(Texture2D b)
		{
			blankTex = b;
		}

		public Screen(int id, string title, Drawing2D draw2D, AlignX alignX)
			: this(id, title, draw2D, alignX, useDefaultDeco: true)
		{
		}

		public Screen(int id, string title, Drawing2D draw2D, AlignX alignX, bool useDefaultDeco)
			: this(id, title, draw2D, alignX, useDefaultDeco, -1f, -1)
		{
		}

		public Screen(int id, string title, Drawing2D draw2D, AlignX alignX, bool useDefaultDeco, float overrideRatioY, int overrideBorder)
		{
			if (border == -1)
			{
				border = 0;
			}
			if (ManagerV2.DefaultBorder != -1)
			{
				border = ManagerV2.DefaultBorder;
			}
			if (overrideBorder != -1)
			{
				border = overrideBorder;
			}
			this.overrideRatioY = overrideRatioY;
			overrideRatioX = -1f;
			this.title = title;
			titleSize = draw2D.FontTitle.MeasureString(title);
			if (TitleIsSet)
			{
				overlayTitle = new Rectangle(0, 0, (int)(titleSize.X + (float)(2 * border)), (int)(titleSize.Y + (float)(2 * border)));
			}
			else
			{
				overlayTitle = Rectangle.Empty;
			}
			this.useDefaultDeco = useDefaultDeco;
			this.draw2D = draw2D;
			this.alignX = alignX;
			this.id = id;
			state = State.Hidden;
			transitionPosition = 0f;
			overlay = new Rectangle((int)(draw2D.ScreenSize.X / 2f), (int)(draw2D.ScreenSize.Y / 2f), 0, 0);
			entry = null;
			Activated = null;
			Cancelled = null;
		}

		protected void UpdateTitle()
		{
			if (TitleIsSet)
			{
				if (overlayTitle.Width < Overlay.Width)
				{
					overlayTitle.Width = Overlay.Width;
				}
				else
				{
					int num = overlayTitle.Width - Overlay.Width;
					overlay.X -= num / 2;
					overlay.Width += num;
				}
				overlayTitle.X = Overlay.Center.X - overlayTitle.Width / 2;
				overlayTitle.Y = Overlay.Top - overlayTitle.Height - 20;
				titlePos = new Vector2((float)overlayTitle.Center.X - titleSize.X * 0.5f, (float)overlayTitle.Y + (float)border * 1f);
			}
		}

		public virtual void Update(GameTime gameTime)
		{
			switch (state)
			{
			case State.TransitionOn:
				if (UpdateTransition(gameTime, transitionTime, 1))
				{
					state = State.TransitionOn;
				}
				else
				{
					state = State.Active;
				}
				break;
			case State.TransitionOff:
				if (UpdateTransition(gameTime, transitionTime, -1))
				{
					state = State.TransitionOff;
				}
				else
				{
					state = State.Hidden;
				}
				break;
			case State.Active:
			case State.Hidden:
				break;
			}
		}

		private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
		{
			float num = ((!(time == TimeSpan.Zero)) ? ((float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds)) : 1f);
			transitionPosition += num * (float)direction;
			if ((direction < 0 && transitionPosition <= 0f) || (direction > 0 && transitionPosition >= 1f))
			{
				transitionPosition = MathHelper.Clamp(transitionPosition, 0f, 1f);
				return false;
			}
			return true;
		}

		public virtual void Enable()
		{
			state = State.TransitionOn;
		}

		public virtual void Disable()
		{
			state = State.TransitionOff;
		}

		public virtual void DisableIfNecessary()
		{
			if ((state == State.Active || state == State.TransitionOn) && !dontDisable)
			{
				state = State.TransitionOff;
			}
		}

		public virtual void DisableNow()
		{
			state = State.Hidden;
		}

		public virtual void render(Drawing2D draw2D, GameTime gameTime)
		{
			if (TitleIsSet)
			{
				draw2D.DrawString(title, titlePos, ColorString(selected: false), draw2D.FontTitle);
			}
			if (entry != null)
			{
				entry.render(draw2D, gameTime, (byte)((state == State.TransitionOff) ? 0f : (255f * transitionPosition)));
			}
			if (useDefaultDeco)
			{
				draw2D.SpriteBatch.Draw(blankTex, overlay, null, colorOverlay, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				if (TitleIsSet)
				{
					draw2D.SpriteBatch.Draw(blankTex, overlayTitle, null, colorOverlay, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}
			}
		}

		public virtual void HandleInput(Utils.Input.ActionMenu action)
		{
			switch (action)
			{
			case Utils.Input.ActionMenu.MENU_ACTIVATE:
				if (Activated != null)
				{
					Activated(this, new Utils.EventArgsInteger(-1));
				}
				break;
			case Utils.Input.ActionMenu.MENU_BACK:
				if (Cancelled != null)
				{
					Cancelled(this, new EventArgs());
				}
				break;
			}
		}

		public virtual void SetEntry(MenuEntry e)
		{
			SetEntry(e, updatePos: true);
		}

		public virtual void SetEntry(MenuEntry e, bool updatePos)
		{
			Vector2 pos = Vector2.Zero;
			if (!updatePos)
			{
				pos = entry.Pos;
			}
			entry = e;
			if (updatePos)
			{
				Vector2 size = e.size;
				size.X += 2 * border;
				size.Y += 2 * border;
				int y = (int)(draw2D.ScreenSize.Y * ((overrideRatioY != -1f) ? overrideRatioY : 0.5f) - size.Y / 2f + (float)(OverlayTitle.Height / 2));
				int x = 0;
				if (overrideRatioX != -1f)
				{
					x = (int)(draw2D.ScreenSize.X * overrideRatioX - size.X / 2f);
				}
				else
				{
					switch (alignX)
					{
					case AlignX.Center:
						x = (int)(draw2D.ScreenSize.X / 2f - size.X / 2f);
						break;
					case AlignX.Left:
						x = (int)(draw2D.ScreenSize.X * 0.1f);
						break;
					case AlignX.Right:
						x = (int)(draw2D.ScreenSize.X * 0.95f - size.X);
						break;
					}
				}
				overlay = new Rectangle(x, y, (int)size.X, (int)size.Y);
				entry.SetPos(new Vector2((float)overlay.Center.X - e.size.X / 2f, (float)overlay.Center.Y - e.size.Y / 2f));
				UpdateTitle();
			}
			else
			{
				entry.SetPos(pos);
			}
		}

		protected void LaunchActivateCB(Utils.EventArgsInteger e)
		{
			if (Activated != null)
			{
				Activated(this, e);
			}
		}

		protected void LaunchCancelCB()
		{
			if (Cancelled != null)
			{
				Cancelled(this, new EventArgs());
			}
		}
	}

	public class MenuScreen : Screen
	{
		public List<MenuEntry> entries;

		private MenuEntryValue<string> Separator;

		private int selection;

		private int defaultSelection;

		public int DefaultSelection
		{
			set
			{
				defaultSelection = value;
			}
		}

		public int Selection => selection;

		public event EventHandler<Utils.EventArgs2Integers> ChangedValue;

		public MenuScreen(int id, string title, Drawing2D draw2D, AlignX alignX, bool useDefaultDeco, int defaultSelection)
			: this(id, title, draw2D, alignX, useDefaultDeco, defaultSelection, -1)
		{
		}

		public MenuScreen(int id, string title, Drawing2D draw2D, AlignX alignX, bool useDefaultDeco)
			: this(id, title, draw2D, alignX, useDefaultDeco, -1)
		{
		}

		public MenuScreen(int id, string title, Drawing2D draw2D, AlignX alignX, bool useDefaultDeco, int defaultSelection, int borderOverride)
			: base(id, title, draw2D, alignX, useDefaultDeco, -1f, borderOverride)
		{
			ChangedValue = null;
			entries = new List<MenuEntry>();
			selection = 0;
			this.defaultSelection = defaultSelection;
			Separator = new MenuEntryValue<string>(-1, "", draw2D.Font, -1);
			Separator.Selectable = false;
		}

		public void AddSeparator(int nbSep)
		{
			for (int i = 0; i < nbSep; i++)
			{
				AddSeparator();
			}
		}

		public void AddSeparator()
		{
			AddEntryValue(Separator);
		}

		public void AddNonSelectableEntry(string text, bool overrideSelectionTransition, List<Color> colorOverride)
		{
			MenuEntryValue<string> menuEntryValue = new MenuEntryValue<string>(entries.Count, text, draw2D.Font, -1);
			menuEntryValue.Selectable = false;
			if (overrideSelectionTransition)
			{
				menuEntryValue.SelectionTransitionOverride(1f);
			}
			if (colorOverride.Count > 0)
			{
				if (overrideSelectionTransition)
				{
					menuEntryValue.OverrideStringColorFront(colorOverride[0]);
				}
				else
				{
					menuEntryValue.OverrideStringColorBack(colorOverride[0]);
				}
			}
			AddEntryValue(menuEntryValue);
		}

		public void AddNonSelectableEntry(string text, bool overrideSelectionTransition)
		{
			AddNonSelectableEntry(text, overrideSelectionTransition, new List<Color>());
		}

		public void ChangeEntry(int index, MenuEntry e)
		{
			entries[index] = e;
			UpdatePositions();
		}

		public void AddEntry(string text, int actionId)
		{
			entries.Add(new MenuEntryValue<string>(entries.Count, text, draw2D.Font, actionId));
			UpdatePositions();
		}

		public void AddMultipleChoiceEntry(string text, int actionId, List<MenuEntryValue<string>> values, int defaultValue)
		{
			entries.Add(new MenuEntryMultipleChoice<List<MenuEntryValue<string>>>(entries.Count, text, draw2D.Font, actionId, values, defaultValue));
			UpdatePositions();
		}

		public void SetMultipleChoiceEntry(string text, int actionId, List<MenuEntryValue<string>> values, int defaultValue)
		{
			entries.Clear();
			AddMultipleChoiceEntry(text, actionId, values, defaultValue);
		}

		public void AddBooleanEntry(string text, int actionId, bool defaultValue)
		{
			AddBooleanEntry(text, actionId, defaultValue, BoolEntryType.OnOff);
		}

		public void AddBooleanEntry(string text, int actionId, bool defaultValue, BoolEntryType type)
		{
			List<MenuEntryValue<bool>> list = new List<MenuEntryValue<bool>>();
			list.Add(new MenuEntryValue<bool>(Utils.BoolToInt(b: false), value: false, draw2D.Font, 0));
			list.Add(new MenuEntryValue<bool>(Utils.BoolToInt(b: true), value: true, draw2D.Font, 0));
			entries.Add(new MenuEntryMultipleChoice<List<MenuEntryValue<bool>>>(entries.Count, text, draw2D.Font, actionId, list, Utils.BoolToInt(defaultValue), type));
			UpdatePositions();
		}

		public void AddMultipleChoiceImageEntry(string text, int actionId, List<MenuEntryValue<TextureWithName>> values, int defaultValue)
		{
			entries.Add(new MenuEntryMultipleChoice<List<MenuEntryValue<TextureWithName>>>(entries.Count, text, draw2D.Font, actionId, values, defaultValue));
			UpdatePositions();
		}

		public void AddEntryValue(MenuEntry e)
		{
			entries.Add(e);
			UpdatePositions();
		}

		public void UpdatePositions()
		{
			UpdatePositions(Point.Zero, useOverridePosition: false, Point.Zero);
		}

		public void OverridePositions(Point newPos)
		{
			UpdatePositions(Point.Zero, useOverridePosition: true, newPos);
		}

		public void UpdatePositions(Point offset)
		{
			UpdatePositions(offset, useOverridePosition: false, Point.Zero);
		}

		private void UpdatePositions(Point offset, bool useOverridePosition, Point overridePosition)
		{
			float num = (float)border * 0.33f;
			overlay.Width = MenuEntry.ListWidth(entries) + 2 * border;
			overlay.Height = MenuEntry.ListHeight(entries) + 2 * border + (int)((float)(entries.Count - 1) * num);
			_ = overlay.Height;
			_ = base.OverlayTitle.Height;
			if (overrideRatioX != -1f)
			{
				overlay.X = (int)(draw2D.ScreenSize.X * overrideRatioX - (float)(overlay.Width / 2));
			}
			else
			{
				switch (alignX)
				{
				case AlignX.Center:
					overlay.X = (int)(draw2D.ScreenSize.X / 2f - (float)(overlay.Width / 2));
					break;
				case AlignX.Left:
					overlay.X = (int)(draw2D.ScreenSize.X * 0.12f);
					break;
				case AlignX.Right:
					overlay.X = (int)(draw2D.ScreenSize.X * 0.95f - (float)overlay.Width);
					break;
				}
			}
			if (overrideRatioY != -1f)
			{
				overlay.Y = (int)(draw2D.ScreenSize.Y * overrideRatioY - (float)(overlay.Height / 2) + (float)(base.OverlayTitle.Height / 2));
			}
			else
			{
				overlay.Y = (int)(draw2D.ScreenSize.Y / 2f - (float)(overlay.Height / 2) + (float)(base.OverlayTitle.Height / 2));
			}
			overlay.X += offset.X;
			overlay.Y += offset.Y;
			if (useOverridePosition)
			{
				overlay.X = overridePosition.X - overlay.Width / 2;
				overlay.Y = overridePosition.Y - overlay.Height / 2;
			}
			entries[0].SetPos(new Vector2((float)overlay.Center.X - entries[0].size.X / 2f, overlay.Y + border));
			for (int i = 1; i < entries.Count; i++)
			{
				entries[i].SetPos(new Vector2((float)overlay.Center.X - entries[i].size.X / 2f, entries[i - 1].Bottom + num));
			}
			UpdateTitle();
		}

		public override void Enable()
		{
			base.Enable();
			if (defaultSelection != -1)
			{
				selection = defaultSelection;
			}
		}

		public override void render(Drawing2D draw2D, GameTime gameTime)
		{
			base.render(draw2D, gameTime);
			foreach (MenuEntry entry in entries)
			{
				entry.render(draw2D, gameTime, ColorString(selected: false).A);
			}
		}

		public override void HandleInput(Utils.Input.ActionMenu action)
		{
			switch (action)
			{
			case Utils.Input.ActionMenu.MENU_UP:
			case Utils.Input.ActionMenu.MENU_DOWN:
				ChangeSelection(action);
				break;
			case Utils.Input.ActionMenu.MENU_LEFT:
			case Utils.Input.ActionMenu.MENU_RIGHT:
				entries[selection].HandleInput(action);
				if (ChangedValue != null)
				{
					ChangedValue(this, new Utils.EventArgs2Integers(entries[selection].actionId, entries[selection].choiceValue));
				}
				break;
			case Utils.Input.ActionMenu.MENU_ACTIVATE:
				LaunchActivateCB(new Utils.EventArgsInteger(entries[selection].actionId));
				break;
			case Utils.Input.ActionMenu.MENU_BACK:
				LaunchCancelCB();
				break;
			}
		}

		private void ChangeSelection(Utils.Input.ActionMenu action)
		{
			do
			{
				selection += ((action == Utils.Input.ActionMenu.MENU_DOWN) ? 1 : (-1));
				if (selection < 0)
				{
					selection += entries.Count;
				}
				else if (selection > entries.Count - 1)
				{
					selection -= entries.Count;
				}
			}
			while (!entries[selection].Selectable);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			foreach (MenuEntry entry in entries)
			{
				entry.Update(gameTime, selection == entry.number);
			}
		}

		public override void SetEntry(MenuEntry e)
		{
			throw new Exception("SetEntry must not be called by a MenuScreen, only by a Screen");
		}

		public void UpdateCustomColorForEntry(int entryNum, List<Color> customColors)
		{
			entries[entryNum].UpdateCustomColor(customColors);
		}
	}

	public class ScreenInfo : MenuScreen
	{
		private double maxTime;

		private double startTime;

		private bool killAfterTimer;

		private bool finished;

		private float depth;

		private bool colorOverlayOverride;

		private Color colorOverlay;

		public float Depth => depth;

		public bool Finished => finished;

		public bool HasColorOverlayOverride(out Color col)
		{
			col = colorOverlay;
			return colorOverlayOverride;
		}

		public void SetColorOverlayOverride(Color col)
		{
			colorOverlayOverride = true;
			colorOverlay = col;
		}

		public ScreenInfo(int id, string title, Vector2 posAsRatio, float depth)
			: this(id, title, posAsRatio, -1, depth)
		{
		}

		public ScreenInfo(int id, string title, Vector2 posAsRatio, int borderOverride, float depth)
			: this(id, title, AlignX.Center, useDefaultDeco: false, -1.0, killAfterTimer: false, borderOverride, depth)
		{
			overrideRatioX = posAsRatio.X;
			overrideRatioY = posAsRatio.Y;
		}

		public ScreenInfo(int id, string title, AlignX alignX, bool useDefaultDeco, double maxTime, bool killAfterTimer, int borderOverride, float depth)
			: base(id, title, Menus.draw2D, alignX, useDefaultDeco, -1, borderOverride)
		{
			this.depth = depth;
			this.maxTime = maxTime;
			this.killAfterTimer = killAfterTimer;
		}

		public void Enable(GameTime gameTime)
		{
			startTime = gameTime.TotalGameTime.TotalSeconds;
			Enable();
		}

		public override void Update(GameTime gameTime)
		{
			finished = gameTime.TotalGameTime.TotalSeconds > startTime + maxTime;
			if (finished && killAfterTimer && state == State.Active)
			{
				Disable();
			}
			base.Update(gameTime);
		}
	}

	public class MenuEntry
	{
		private enum SelectionState
		{
			TransitionOn,
			On,
			TransitionOff,
			Off
		}

		public Vector2 size;

		public int actionId;

		public int number;

		protected Vector2 pos;

		public int choiceValue;

		protected float selectionTransition;

		protected TimeSpan transitionTime = TimeSpan.FromSeconds(0.10000000149011612);

		private SelectionState selectionState;

		protected bool customColor;

		protected List<Color> customColors;

		private bool selectable;

		public float scaleMax = 1.2f;

		public float scaleMin = 5f / 6f;

		public Vector2 Pos => pos;

		public float SelectionTransition => selectionTransition;

		public bool Selectable
		{
			get
			{
				return selectable;
			}
			set
			{
				selectable = value;
			}
		}

		public float Bottom => pos.Y + size.Y;

		protected float ScaleString => MathHelper.SmoothStep(1f, scaleMax, selectionTransition);

		public void SelectionTransitionOverride(float value)
		{
			selectionTransition = value;
		}

		public MenuEntry()
		{
			selectionTransition = 0f;
			selectionState = SelectionState.Off;
			customColor = false;
			customColors = null;
			selectable = true;
		}

		public static int ListWidth(List<MenuEntry> entries)
		{
			int num = 0;
			foreach (MenuEntry entry in entries)
			{
				num = (int)Math.Max(num, entry.size.X);
			}
			return num;
		}

		public static int ListHeight(List<MenuEntry> entries)
		{
			int num = 0;
			foreach (MenuEntry entry in entries)
			{
				num += (int)entry.size.Y;
			}
			return num;
		}

		protected Color ColorString(byte alpha)
		{
			return Utils.ColorWithAlpha(Utils.SmoothStepColor(Screen.colorStringNotSelected, Screen.colorStringSelected, selectionTransition), alpha);
		}

		protected Color ColorTexture(byte alpha)
		{
			return Utils.ColorWithAlpha(Color.White, alpha);
		}

		private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
		{
			float num = ((!(time == TimeSpan.Zero)) ? ((float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds)) : 1f);
			selectionTransition += num * (float)direction;
			if ((direction < 0 && selectionTransition <= 0f) || (direction > 0 && selectionTransition >= 1f))
			{
				selectionTransition = MathHelper.Clamp(selectionTransition, 0f, 1f);
				return false;
			}
			return true;
		}

		public virtual void Update(GameTime gameTime, bool isSelected)
		{
			switch (selectionState)
			{
			case SelectionState.On:
				if (!isSelected)
				{
					selectionState = SelectionState.TransitionOff;
				}
				break;
			case SelectionState.Off:
				if (isSelected)
				{
					selectionState = SelectionState.TransitionOn;
				}
				break;
			case SelectionState.TransitionOn:
				if (UpdateTransition(gameTime, transitionTime, 1))
				{
					selectionState = SelectionState.TransitionOn;
				}
				else
				{
					selectionState = SelectionState.On;
				}
				break;
			case SelectionState.TransitionOff:
				if (UpdateTransition(gameTime, transitionTime, -1))
				{
					selectionState = SelectionState.TransitionOff;
				}
				else
				{
					selectionState = SelectionState.Off;
				}
				break;
			}
		}

		public virtual void HandleInput(Utils.Input.ActionMenu action)
		{
		}

		public virtual void SetPos(Vector2 pos)
		{
			this.pos = pos;
		}

		public virtual void render(Drawing2D draw2D, GameTime gameTime, byte alpha)
		{
		}

		public void UpdateCustomColor(List<Color> customColors)
		{
			customColor = true;
			this.customColors = customColors;
		}
	}

	public class MenuEntryValue<T> : MenuEntry
	{
		public T value;

		private object tmpForUglyCast;

		private SpriteFont font;

		public Color stringColorBack;

		public Color stringColorFront;

		public bool overrideStringColorBack;

		public bool overrideStringColorFront;

		private BoolEntryType boolType;

		public void OverrideStringColorBack(Color c)
		{
			stringColorBack = c;
			overrideStringColorBack = true;
		}

		public void OverrideStringColorFront(Color c)
		{
			stringColorFront = c;
			overrideStringColorFront = true;
		}

		public MenuEntryValue(int number, T value, SpriteFont font, int actionId, float scaleMin, float scaleMax)
			: this(number, value, font, actionId)
		{
			base.scaleMin = scaleMin;
			base.scaleMax = scaleMax;
		}

		public MenuEntryValue(int number, T value, SpriteFont font, int actionId)
			: this(number, value, font, actionId, BoolEntryType.OnOff)
		{
		}

		public MenuEntryValue(int number, T value, SpriteFont font, int actionId, BoolEntryType boolType)
		{
			base.number = number;
			base.actionId = actionId;
			this.value = value;
			tmpForUglyCast = value;
			this.font = font;
			this.boolType = boolType;
			ComputeSize(font);
		}

		private void ComputeSize(SpriteFont font)
		{
			if ((object)typeof(T) == typeof(string))
			{
				size = font.MeasureString((string)tmpForUglyCast);
				return;
			}
			if ((object)typeof(T) == typeof(Texture2D))
			{
				size = new Vector2(((Texture2D)tmpForUglyCast).Width, ((Texture2D)tmpForUglyCast).Height);
				return;
			}
			if ((object)typeof(T) == typeof(TextureWithName))
			{
				size = ((TextureWithName)tmpForUglyCast).Size(font);
				return;
			}
			if ((object)typeof(T) == typeof(bool))
			{
				size = BoolColor.Size(boolType, (bool)tmpForUglyCast, font);
				return;
			}
			throw new Exception("type not supported");
		}

		public override void render(Drawing2D draw2D, GameTime gameTime, byte alpha)
		{
			render(draw2D, pos, gameTime, alpha);
		}

		public void render(Drawing2D draw2D, Vector2 overridePos, GameTime gameTime, byte alpha)
		{
			if ((object)typeof(T) == typeof(string))
			{
				draw2D.DrawStringWithSelectEffect((string)tmpForUglyCast, font, overridePos, overrideStringColorFront ? Utils.ColorWithAlpha(stringColorFront, alpha) : ColorString(alpha), Utils.ColorWithAlpha(overrideStringColorBack ? stringColorBack : Screen.colorStringNotSelected, alpha), scaleMax, selectionTransition);
				return;
			}
			if ((object)typeof(T) == typeof(Texture2D))
			{
				draw2D.SpriteBatch.Draw((Texture2D)tmpForUglyCast, overridePos, ColorTexture(alpha));
				return;
			}
			if ((object)typeof(T) == typeof(TextureWithName))
			{
				((TextureWithName)tmpForUglyCast).DrawWithSelectEffect(draw2D, overridePos, overrideStringColorFront ? Utils.ColorWithAlpha(stringColorFront, alpha) : ColorString(alpha), Utils.ColorWithAlpha(overrideStringColorBack ? stringColorBack : Screen.colorStringNotSelected, alpha), scaleMin, scaleMax, selectionTransition, null, "", isLocked: false);
				return;
			}
			throw new Exception("type not supported");
		}
	}

	public class MenuEntryMultipleChoice<T> : MenuEntry
	{
		private string name;

		private T choices;

		private int choicesCount;

		private object tmpForUglyCast;

		private List<Vector2> choicesSizes;

		private List<Vector2> choicesPos;

		private Vector2 nameSize;

		private float offset;

		private BoolEntryType boolType;

		private float outlineOffset => offset / 2f;

		public MenuEntryMultipleChoice(int number, string name, SpriteFont font, int actionId, T choices, int defaultValue)
			: this(number, name, font, actionId, choices, defaultValue, BoolEntryType.OnOff)
		{
		}

		public MenuEntryMultipleChoice(int number, string name, SpriteFont font, int actionId, T choices, int defaultValue, BoolEntryType boolType)
		{
			base.number = number;
			base.actionId = actionId;
			this.name = name;
			this.choices = choices;
			this.boolType = boolType;
			tmpForUglyCast = this.choices;
			nameSize = font.MeasureString(name);
			choicesSizes = new List<Vector2>();
			choicesPos = new List<Vector2>();
			offset = font.LineSpacing;
			if ((object)typeof(T) == typeof(List<MenuEntryValue<string>>))
			{
				choicesCount = ((List<MenuEntryValue<string>>)tmpForUglyCast).Count;
				size = nameSize * scaleMax;
				size.X += 2f * offset;
				size.Y += outlineOffset;
				foreach (MenuEntryValue<string> item4 in (List<MenuEntryValue<string>>)tmpForUglyCast)
				{
					Vector2 item = font.MeasureString(item4.value);
					size.X += item.X + offset;
					choicesSizes.Add(item);
				}
				size.X -= offset;
			}
			else if ((object)typeof(T) == typeof(List<MenuEntryValue<bool>>))
			{
				size = nameSize * scaleMax;
				size.X += 2f * offset;
				size.Y += outlineOffset;
				choicesCount = 2;
				Vector2 item2 = font.MeasureString(BoolColor.FalseString(boolType));
				size.X += item2.X + offset;
				size.Y = Math.Max(size.Y, item2.Y + outlineOffset);
				choicesSizes.Add(item2);
				item2 = font.MeasureString(BoolColor.TrueString(boolType));
				size.X += item2.X;
				size.Y = Math.Max(size.Y, item2.Y + outlineOffset);
				choicesSizes.Add(item2);
			}
			else
			{
				if ((object)typeof(T) != typeof(List<MenuEntryValue<TextureWithName>>))
				{
					throw new Exception("type not supported");
				}
				size = nameSize * scaleMax;
				size.X += offset;
				size.Y += 1f * outlineOffset;
				choicesCount = ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast).Count;
				foreach (MenuEntryValue<TextureWithName> item5 in (List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)
				{
					Vector2 item3 = item5.size;
					size.Y = Math.Max(size.Y, item3.Y + outlineOffset);
					size.X += item3.X + 1.5f * offset;
					choicesSizes.Add(item3);
				}
				size.X += offset;
			}
			choiceValue = defaultValue;
		}

		public override void SetPos(Vector2 newPos)
		{
			Vector2 vector = newPos;
			if (number == 0)
			{
				vector.Y += outlineOffset / 2f;
			}
			base.SetPos(vector);
			Vector2 item = vector;
			if (nameSize.X != 0f)
			{
				item.X += nameSize.X * scaleMax + 2f * offset;
			}
			else
			{
				item.X += 1f * offset;
			}
			choicesPos.Clear();
			for (int i = 0; i < choicesCount; i++)
			{
				choicesPos.Add(item);
				item.X += choicesSizes[i].X + (((object)typeof(T) == typeof(List<MenuEntryValue<TextureWithName>>)) ? 1.5f : 1f) * offset;
			}
		}

		public override void render(Drawing2D draw2D, GameTime gameTime, byte alpha)
		{
			Color c = ColorString(alpha);
			Color colorStringNotSelected = Screen.colorStringNotSelected;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = pos;
			if ((object)typeof(T) == typeof(List<string>))
			{
				vector2 = pos + vector;
			}
			else if ((object)typeof(T) == typeof(List<TextureWithName>))
			{
				vector2 = new Vector2(pos.X, pos.Y + size.Y / 2f - draw2D.Font.MeasureString("AAA").Y / 1f);
			}
			draw2D.DrawStringWithSelectEffect(name, vector2, ColorString(alpha), Utils.ColorWithAlpha(Screen.colorStringNotSelected, alpha), scaleMax, selectionTransition);
			for (int i = 0; i < choicesCount; i++)
			{
				if ((object)typeof(T) == typeof(List<MenuEntryValue<string>>))
				{
					string value = ((List<MenuEntryValue<string>>)tmpForUglyCast)[i].value;
					draw2D.DrawStringWithSelectEffect(value, choicesPos[i] - Vector2.UnitY * draw2D.Font.MeasureString(value).Y / 2f, Utils.ColorWithAlpha(c, alpha), Utils.ColorWithAlpha(colorStringNotSelected, alpha), ((List<MenuEntryValue<string>>)tmpForUglyCast)[i].scaleMax, ((List<MenuEntryValue<string>>)tmpForUglyCast)[i].SelectionTransition);
					continue;
				}
				if ((object)typeof(T) == typeof(List<MenuEntryValue<bool>>))
				{
					bool value2 = ((List<MenuEntryValue<bool>>)tmpForUglyCast)[i].value;
					Color color = Utils.ColorWithAlpha(BoolColor.ToColor(value2), alpha);
					draw2D.DrawStringWithSelectEffect(BoolColor.ToString(boolType, value2), choicesPos[i], color, Utils.ColorWithAlpha(color, (byte)(alpha * 100 / 255)), ((List<MenuEntryValue<bool>>)tmpForUglyCast)[i].scaleMax, ((List<MenuEntryValue<bool>>)tmpForUglyCast)[i].SelectionTransition);
					continue;
				}
				if ((object)typeof(T) == typeof(List<MenuEntryValue<TextureWithName>>))
				{
					Color? customTexColor = null;
					string customString = "";
					if (customColor)
					{
						customTexColor = customColors[i];
					}
					((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].value.DrawWithSelectEffect(draw2D, choicesPos[i], ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].overrideStringColorFront ? Utils.ColorWithAlpha(((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].stringColorFront, alpha) : ColorString(alpha), Utils.ColorWithAlpha(((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].overrideStringColorBack ? ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].stringColorBack : Screen.colorStringNotSelected, alpha), ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].scaleMin, ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].scaleMax, ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].SelectionTransition, customTexColor, customString, !((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[i].Selectable);
					continue;
				}
				throw new Exception("type not supported");
			}
		}

		public override void HandleInput(Utils.Input.ActionMenu action)
		{
			if (action == Utils.Input.ActionMenu.MENU_RIGHT)
			{
				do
				{
					choiceValue++;
					if (choiceValue > choicesCount - 1)
					{
						choiceValue = 0;
					}
				}
				while (!IsSelectable(choiceValue));
			}
			else
			{
				if (action != Utils.Input.ActionMenu.MENU_LEFT)
				{
					return;
				}
				do
				{
					choiceValue--;
					if (choiceValue < 0)
					{
						choiceValue = choicesCount - 1;
					}
				}
				while (!IsSelectable(choiceValue));
			}
		}

		public override void Update(GameTime gameTime, bool isSelected)
		{
			base.Update(gameTime, isSelected);
			if ((object)typeof(T) == typeof(List<MenuEntryValue<string>>))
			{
				foreach (MenuEntryValue<string> item in (List<MenuEntryValue<string>>)tmpForUglyCast)
				{
					item.Update(gameTime, choiceValue == item.number);
				}
				return;
			}
			if ((object)typeof(T) == typeof(List<MenuEntryValue<bool>>))
			{
				foreach (MenuEntryValue<bool> item2 in (List<MenuEntryValue<bool>>)tmpForUglyCast)
				{
					item2.Update(gameTime, choiceValue == item2.number);
				}
				return;
			}
			if ((object)typeof(T) == typeof(List<MenuEntryValue<TextureWithName>>))
			{
				foreach (MenuEntryValue<TextureWithName> item3 in (List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)
				{
					item3.Update(gameTime, choiceValue == item3.number);
				}
				return;
			}
			throw new Exception("type not supported");
		}

		private bool IsSelectable(int entryNum)
		{
			if ((object)typeof(T) == typeof(List<MenuEntryValue<string>>))
			{
				return ((List<MenuEntryValue<string>>)tmpForUglyCast)[entryNum].Selectable;
			}
			if ((object)typeof(T) == typeof(List<MenuEntryValue<bool>>))
			{
				return ((List<MenuEntryValue<bool>>)tmpForUglyCast)[entryNum].Selectable;
			}
			if ((object)typeof(T) == typeof(List<MenuEntryValue<TextureWithName>>))
			{
				return ((List<MenuEntryValue<TextureWithName>>)tmpForUglyCast)[entryNum].Selectable;
			}
			throw new Exception("type not supported");
		}
	}

	public static Drawing2D draw2D;

	public static void Initialize(Drawing2D draw2DValue)
	{
		draw2D = draw2DValue;
	}
}
