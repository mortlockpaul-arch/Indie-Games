using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Input;

namespace Quasar.GUI.Controls;

public class Group : InteractiveControl
{
	public class GroupCreator : IControlCreator
	{
		private static GroupCreator instance;

		public static GroupCreator Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GroupCreator();
				}
				return instance;
			}
		}

		public Control ParseXml(XElement xe, Layout layout)
		{
			Group obj = new Group(layout);
			obj.parseXml(xe);
			return obj;
		}
	}

	public const string Type = "Group";

	private GroupConfiguration configuration;

	private List<GroupControl> controls = new List<GroupControl>();

	private int activeControl = -1;

	private float childrenSize;

	public override string ControlType => "Group";

	public GroupConfiguration Configuration => configuration;

	public List<GroupControl> Controls => controls;

	public GroupControl ActiveControl
	{
		get
		{
			if (activeControl == -1)
			{
				return null;
			}
			return controls[activeControl];
		}
		set
		{
			for (int i = 0; i < controls.Count; i++)
			{
				if (controls[i] == value)
				{
					activeControl = i;
					break;
				}
			}
		}
	}

	private int PreviousControl
	{
		get
		{
			if (controls.Count <= 0)
			{
				return -1;
			}
			if (activeControl == -1)
			{
				return controls.Count - 1;
			}
			if (configuration.Loop)
			{
				return (activeControl - 1 + controls.Count) % controls.Count;
			}
			return Math.Max(0, activeControl - 1);
		}
	}

	private int NextControl
	{
		get
		{
			if (controls.Count <= 0)
			{
				return -1;
			}
			if (configuration.Loop)
			{
				return (activeControl + 1 + controls.Count) % controls.Count;
			}
			return Math.Min(controls.Count - 1, activeControl + 1);
		}
	}

	public event Func<Group, PlayerIndex, bool> OnCancel;

	public event Action<Group, PlayerIndex> OnCancelled;

	public event Action<Group> OnFocusChange;

	public event Action<Group, GroupControl> OnControlAdded;

	public Group(Layout layout)
		: base(layout)
	{
	}

	public override void SetId(string id)
	{
		base.SetId(id);
		configuration = GroupConfigurationManager.Config[base.Id];
	}

	public void addControl(GroupControl gc)
	{
		controls.Add(gc);
		if (activeControl == -1)
		{
			activeControl = 0;
		}
		if (configuration.NavigateMode == GroupNavigateMode.BottomUp)
		{
			UpdateChildPositions();
		}
		else
		{
			SetLastControlPosition(gc);
		}
		if (OnControlAdded != null)
		{
			OnControlAdded(this, gc);
		}
	}

	public void insertControl(GroupControl gc, int index)
	{
		controls.Insert(index, gc);
		if (activeControl == index)
		{
			activeControl++;
		}
		UpdateChildPositions();
		if (OnControlAdded != null)
		{
			OnControlAdded(this, gc);
		}
	}

	private void SetLastControlPosition(GroupControl gc)
	{
		switch (configuration.NavigateMode)
		{
		case GroupNavigateMode.LeftToRight:
			gc.Position = configuration.Position + new Vector2(childrenSize + configuration.Margin, 0f);
			childrenSize += gc.Size.X + configuration.Margin;
			break;
		case GroupNavigateMode.RightToLeft:
			gc.Position = configuration.Position - new Vector2(childrenSize + configuration.Margin, 0f);
			childrenSize += gc.Size.X + configuration.Margin;
			break;
		case GroupNavigateMode.TopDown:
			gc.Position = configuration.Position - new Vector2(0f, childrenSize + configuration.Margin);
			childrenSize += gc.Size.Y + configuration.Margin;
			break;
		case GroupNavigateMode.BottomUp:
			gc.Position = configuration.Position + new Vector2(0f, childrenSize + configuration.Margin);
			childrenSize += gc.Size.Y + configuration.Margin;
			break;
		}
		switch (configuration.HorizontalMode)
		{
		case GroupHorizontalLayoutMode.Left:
			gc.Position += new Vector2(gc.Size.X * 0.5f, 0f);
			break;
		case GroupHorizontalLayoutMode.Right:
			gc.Position -= new Vector2(gc.Size.X * 0.5f, 0f);
			break;
		case GroupHorizontalLayoutMode.Center:
			break;
		}
	}

	public void removeControl(GroupControl gc)
	{
		if (gc != null)
		{
			int num = controls.IndexOf(gc);
			if (num >= 0 && activeControl >= num)
			{
				activeControl--;
			}
			if (num >= 0)
			{
				controls.RemoveAt(num);
				gc.Focused = false;
				gc.Removed();
				UpdateChildPositions();
			}
		}
	}

	private void UpdateChildPositions()
	{
		childrenSize = 0f;
		if (configuration.NavigateMode == GroupNavigateMode.BottomUp)
		{
			for (int num = controls.Count - 1; num >= 0; num--)
			{
				SetLastControlPosition(controls[num]);
			}
			return;
		}
		foreach (GroupControl control in controls)
		{
			SetLastControlPosition(control);
		}
	}

	public void removeControl(string id)
	{
		removeControl(GetControl(id));
	}

	public GroupControl GetControl(string id)
	{
		foreach (GroupControl control in controls)
		{
			if (control.Id == id)
			{
				return control;
			}
		}
		return null;
	}

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		GroupControl groupControl = ActiveControl;
		for (int i = 0; i < controls.Count; i++)
		{
			GroupControl groupControl2 = controls[i];
			if (!groupControl2.CheckClick(type, position))
			{
				continue;
			}
			if (groupControl2 != groupControl)
			{
				activeControl = i;
				groupControl2.Focused = true;
				groupControl.Focused = false;
				if (OnFocusChange != null)
				{
					OnFocusChange(this);
				}
			}
			return true;
		}
		return false;
	}

	public override void Update()
	{
		GroupControl groupControl = ActiveControl;
		switch (configuration.NavigateMode)
		{
		case GroupNavigateMode.LeftToRight:
			if (InputManager.MenuLeftRepeat())
			{
				activeControl = PreviousControl;
			}
			if (InputManager.MenuRightRepeat())
			{
				activeControl = NextControl;
			}
			break;
		case GroupNavigateMode.TopDown:
			if (InputManager.MenuUpRepeat())
			{
				activeControl = PreviousControl;
			}
			if (InputManager.MenuDownRepeat())
			{
				activeControl = NextControl;
			}
			break;
		case GroupNavigateMode.BottomUp:
			if (InputManager.MenuUpRepeat())
			{
				activeControl = PreviousControl;
			}
			if (InputManager.MenuDownRepeat())
			{
				activeControl = NextControl;
			}
			break;
		}
		if (groupControl != null && groupControl != ActiveControl)
		{
			groupControl.Focused = false;
			if (OnFocusChange != null)
			{
				OnFocusChange(this);
			}
		}
		if (ActiveControl != null)
		{
			ActiveControl.Focused = true;
			ActiveControl.Update();
		}
		if (OnCancel != null)
		{
			PlayerIndex whoPressed = PlayerIndex.One;
			if (InputManager.MenuCancel(ref whoPressed) && OnCancel(this, whoPressed) && OnCancelled != null)
			{
				OnCancelled(this, whoPressed);
			}
		}
	}

	public override void parseXml(XElement xe)
	{
		base.parseXml(xe);
		configuration = GroupConfigurationManager.Config[base.Id];
		foreach (XElement item in xe.Elements())
		{
			GroupControl groupControl = GroupControlCreatorManager.Instance.ParseControl(item, this, base.Layout);
			if (groupControl != null)
			{
				addControl(groupControl);
			}
		}
	}

	public override void Dispose()
	{
		if (controls != null)
		{
			foreach (GroupControl control in controls)
			{
				control.Dispose();
			}
			controls = null;
		}
		OnCancelled = null;
		OnCancel = null;
		OnFocusChange = null;
		base.Dispose();
	}
}
