using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Game;

public class MenuScreen : GameScreen
{
	[CompilerGenerated]
	private static Color _003CEntryColor_003Ek__BackingField;

	[CompilerGenerated]
	private static Color _003CSelectedEntryColor_003Ek__BackingField;

	[CompilerGenerated]
	private static Color _003CDisabledEntryColor_003Ek__BackingField;

	[CompilerGenerated]
	private static Color _003CSelectedDisabledEntryColor_003Ek__BackingField;

	public string Title { get; set; }

	public List<MenuEntry> Entries { get; private set; }

	public int SelectedIndex { get; set; }

	public MenuEntry SelectedEntry => Entries[SelectedIndex];

	public static Color EntryColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return _003CEntryColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_003CEntryColor_003Ek__BackingField = value;
		}
	}

	public static Color SelectedEntryColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return _003CSelectedEntryColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_003CSelectedEntryColor_003Ek__BackingField = value;
		}
	}

	public static Color DisabledEntryColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return _003CDisabledEntryColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_003CDisabledEntryColor_003Ek__BackingField = value;
		}
	}

	public static Color SelectedDisabledEntryColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return _003CSelectedDisabledEntryColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_003CSelectedDisabledEntryColor_003Ek__BackingField = value;
		}
	}

	public static event EventHandler DrawingMenu;

	public static event EventHandler MenuChanged;

	public static event EventHandler MenuSelect;

	public static event EventHandler MenuClose;

	static MenuScreen()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntryColor = Color.White;
		SelectedEntryColor = Color.Yellow;
		DisabledEntryColor = Color.Gray;
		SelectedDisabledEntryColor = Color.DimGray;
	}

	public MenuScreen(ScreenManager screenManager, string title)
		: base(screenManager)
	{
		Entries = new List<MenuEntry>();
		Title = title;
	}

	public override void HandleInput()
	{
		if (ScreenInput.Select && SelectedEntry != null && SelectedEntry.Enabled)
		{
			SelectedEntry.Next();
			SelectedEntry.Select();
			OnMenuSelect();
		}
		if (ScreenInput.Right && SelectedEntry != null && SelectedEntry.Enabled)
		{
			SelectedEntry.Next();
		}
		if (ScreenInput.Left && SelectedEntry != null && SelectedEntry.Enabled)
		{
			SelectedEntry.Previous();
		}
		if (ScreenInput.Back)
		{
			Close();
		}
		if (ScreenInput.Up)
		{
			SelectedIndex = (int)MathHelper.Max(0f, (float)(--SelectedIndex));
			OnMenuChanged();
		}
		if (ScreenInput.Down)
		{
			SelectedIndex = (int)MathHelper.Min((float)(Entries.Count - 1), (float)(++SelectedIndex));
			OnMenuChanged();
		}
		base.HandleInput();
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		if (DrawingMenu != null)
		{
			DrawingMenu(this, EventArgs.Empty);
		}
		base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
		for (int i = 0; i < Entries.Count; i++)
		{
			OnDrawEntry(Entries[i], i, transition);
		}
		base.ScreenManager.SpriteBatch.End();
		base.Draw(gameTime, transition);
	}

	protected virtual void OnDrawEntry(MenuEntry entry, int index, float transition)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Font == null)
		{
			return;
		}
		Color color = default(Color);
		((Color)(ref color))._002Ector(EntryColor, transition);
		if (entry.Enabled)
		{
			if (entry == SelectedEntry)
			{
				((Color)(ref color))._002Ector(SelectedEntryColor, transition);
			}
		}
		else
		{
			((Color)(ref color))._002Ector(DisabledEntryColor, transition);
			if (entry == SelectedEntry)
			{
				((Color)(ref color))._002Ector(SelectedDisabledEntryColor, transition);
			}
		}
		Vector2 screenCenter = base.ScreenManager.ScreenCenter;
		float num = screenCenter.Y - (float)((Entries.Count - 1) * GameManager.Font.LineSpacing / 2);
		base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, entry.ToString(), new Vector2(screenCenter.X, num + (float)(index * GameManager.Font.LineSpacing)), Align.Center, color);
	}

	protected virtual void OnMenuChanged()
	{
		if (MenuChanged != null)
		{
			MenuChanged(this, EventArgs.Empty);
		}
	}

	protected virtual void OnMenuSelect()
	{
		if (MenuSelect != null)
		{
			MenuSelect(this, EventArgs.Empty);
		}
	}

	protected virtual void OnMenuClose()
	{
		if (MenuClose != null)
		{
			MenuClose(this, EventArgs.Empty);
		}
	}

	public override void Show()
	{
		SelectedIndex = 0;
		base.Show();
	}

	public override void Close()
	{
		OnMenuClose();
		base.Close();
	}
}
