using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GUI.Controls.GroupControls;

public class Selector : Button
{
	public delegate void SelectorHandler(Selector selectorChanged, int selection, string value);

	public new const string Type = "Selector";

	protected SortedList<int, string> options = new SortedList<int, string>(1);

	protected int currentOption = -1;

	protected int pageSize = 5;

	protected bool loop = true;

	public override string ControlType => "Selector";

	public SortedList<int, string> Options => options;

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
		}
	}

	public string CurrentOptionValue
	{
		get
		{
			if (options.TryGetValue(currentOption, out var value))
			{
				return value;
			}
			return "";
		}
	}

	public int CurrentOption
	{
		get
		{
			return currentOption;
		}
		set
		{
			currentOption = value;
		}
	}

	public virtual bool CanSelectNext
	{
		get
		{
			if (options.Count <= 1)
			{
				return false;
			}
			if (Loop)
			{
				return true;
			}
			int num = options.IndexOfKey(currentOption);
			return num < options.Count - 1;
		}
	}

	public virtual bool CanSelectPrevious
	{
		get
		{
			if (options.Count <= 1)
			{
				return false;
			}
			if (Loop)
			{
				return true;
			}
			int num = options.IndexOfKey(currentOption);
			return num > 0;
		}
	}

	public event SelectorHandler OnChange;

	public Selector(Layout layout, Group group)
		: base(layout, group)
	{
	}

	public void addOption(int id, string value)
	{
		options.Add(id, value);
		if (currentOption == -1)
		{
			currentOption = id;
		}
	}

	public void clearOptions()
	{
		options.Clear();
		currentOption = -1;
	}

	protected void SelectNext()
	{
		if (CanSelectNext)
		{
			int num = options.IndexOfKey(currentOption);
			if (Loop)
			{
				currentOption = options.Keys[(num + 1) % options.Count];
			}
			else
			{
				currentOption = options.Keys[Math.Min(options.Count - 1, num + 1)];
			}
			SendEvent();
		}
	}

	protected void SelectPrevious()
	{
		if (CanSelectPrevious)
		{
			int num = options.IndexOfKey(currentOption);
			if (Loop)
			{
				currentOption = options.Keys[(options.Count + num - 1) % options.Count];
			}
			else
			{
				currentOption = options.Keys[Math.Max(0, num - 1)];
			}
			SendEvent();
		}
	}

	protected void SelectPreviousPage()
	{
		int num = options.IndexOfKey(currentOption);
		if (num == 0)
		{
			return;
		}
		int num2 = Math.Min(pageSize, num);
		bool flag = false;
		for (int i = 1; i <= num2; i++)
		{
			if (!CanSelectPrevious)
			{
				break;
			}
			currentOption = options.Keys[num - i];
			flag = true;
		}
		if (flag)
		{
			SendEvent();
		}
	}

	protected void SelectFirst()
	{
		int num = options.IndexOfKey(currentOption);
		if (num != 0)
		{
			bool flag = false;
			int num2 = num;
			while (num2 > 0 && CanSelectPrevious)
			{
				currentOption = options.Keys[num2 - 1];
				flag = true;
				num2--;
			}
			if (flag)
			{
				SendEvent();
			}
		}
	}

	protected void SelectNextPage()
	{
		int num = options.IndexOfKey(currentOption);
		if (num == options.Count - 1)
		{
			return;
		}
		int num2 = Math.Min(pageSize, options.Count - num - 1);
		bool flag = false;
		for (int i = 1; i <= num2; i++)
		{
			if (!CanSelectNext)
			{
				break;
			}
			currentOption = options.Keys[num + i];
			flag = true;
		}
		if (flag)
		{
			SendEvent();
		}
	}

	protected void SelectLast()
	{
		int num = options.IndexOfKey(currentOption);
		if (num == options.Count - 1)
		{
			return;
		}
		bool flag = false;
		for (int i = num; i < options.Count - 1; i++)
		{
			if (!CanSelectNext)
			{
				break;
			}
			currentOption = options.Keys[i + 1];
			flag = true;
		}
		if (flag)
		{
			SendEvent();
		}
	}

	protected virtual void SendEvent()
	{
		if (OnChange != null)
		{
			OnChange(this, currentOption, CurrentOptionValue);
		}
	}

	protected override void Click()
	{
		SelectNext();
	}

	public override void Update()
	{
		if (InputManager.MenuRightRepeat())
		{
			SelectNext();
		}
		if (InputManager.MenuLeftRepeat())
		{
			SelectPrevious();
		}
		if (InputManager.MenuGoToFirst())
		{
			SelectFirst();
		}
		if (InputManager.MenuGoToLast())
		{
			SelectLast();
		}
		if (InputManager.MenuNextPage())
		{
			SelectNextPage();
		}
		if (InputManager.MenuPrevPage())
		{
			SelectPreviousPage();
		}
		base.Update();
	}

	public override void parseXml(XElement xe)
	{
		base.parseXml(xe);
		loop = XDocHelper.ParseBoolAttribute(xe, "loop", defaultValue: true);
		foreach (XElement item in xe.Elements("Option"))
		{
			int num = XDocHelper.ParseIntAttribute(item, "id");
			if (XDocHelper.ParseBoolAttribute(item, "selected"))
			{
				CurrentOption = num;
			}
			addOption(num, LanguageManager.Texts[XDocHelper.GetAttribute(item, "text")]);
		}
		if (CurrentOption == -1 && Options.Count > 0)
		{
			CurrentOption = Options.Keys[0];
		}
	}

	public override void Dispose()
	{
		OnChange = null;
		base.Dispose();
	}
}
