using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.ContentPipeline;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GUI;

public class Layout : IDisposable
{
	public const string LAYOUT_DIR = "Layouts/";

	private bool enabled = true;

	private string title;

	private string template;

	private string id;

	private Dictionary<string, Control> controls = new Dictionary<string, Control>(2);

	private List<Control> controlList = new List<Control>(2);

	private InteractiveControl activeControl;

	private Dialog activeDialog;

	private TextInputDialog activeTextInput;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			title = value;
		}
	}

	public string Template => template;

	public string Id => id;

	public List<Control> Controls => controlList;

	public InteractiveControl ActiveControl
	{
		get
		{
			return activeControl;
		}
		set
		{
			activeControl = value;
		}
	}

	public Dialog ActiveDialog => activeDialog;

	public TextInputDialog ActiveTextInput => activeTextInput;

	public bool IsDialogShown
	{
		get
		{
			if (activeDialog == null)
			{
				return activeTextInput != null;
			}
			return true;
		}
	}

	public event Action<Layout, Dialog> DialogCreated;

	public event Action<Layout> DialogRemoved;

	public event Action<Layout, TextInputDialog> TextInputCreated;

	public event Action<Layout> TextInputRemoved;

	public event Func<Layout, PlayerIndex, bool> OnCancel;

	public event Action<Layout, PlayerIndex> OnCancelled;

	public Layout()
	{
	}

	public Layout(string id, string title, string template)
	{
		this.id = id;
		this.title = title;
		this.template = template;
	}

	public void AddControl(Control c)
	{
		if (c.Id != null && c.Id.Length > 0)
		{
			controls.Add(c.Id, c);
		}
		controlList.Add(c);
		InteractiveControl interactiveControl = c as InteractiveControl;
		if (activeControl == null && interactiveControl != null)
		{
			activeControl = interactiveControl;
		}
	}

	public void Remove(string id)
	{
		if (!id.Contains('/'))
		{
			Control value = null;
			controls.TryGetValue(id, out value);
			Remove(value);
			return;
		}
		string[] array = id.Split('/');
		if (array.Length <= 2 && GetControl(array[0]) is Group obj)
		{
			obj.removeControl(array[1]);
		}
	}

	public void Remove(Control c)
	{
		if (c != null)
		{
			if (c.Id != null)
			{
				controls.Remove(c.Id);
			}
			controlList.Remove(c);
			c.Removed();
		}
	}

	public Control GetControl(string id)
	{
		int num = id.IndexOf('/');
		if (num == -1)
		{
			Control value = null;
			controls.TryGetValue(id, out value);
			return value;
		}
		if (!(GetControl(id.Substring(0, num)) is Group obj))
		{
			return null;
		}
		return obj.GetControl(id.Substring(num + 1));
	}

	private bool CheckClick(ClickType type, Vector2 position)
	{
		if (type == ClickType.Press)
		{
			foreach (Control control in controlList)
			{
				if (control is InteractiveControl interactiveControl && interactiveControl.CheckClick(type, position))
				{
					activeControl = interactiveControl;
					return true;
				}
			}
		}
		else if (activeControl != null && activeControl.CheckClick(type, position))
		{
			activeControl = null;
		}
		return false;
	}

	public void Update()
	{
		if (!Enabled)
		{
			return;
		}
		if (activeDialog != null)
		{
			activeDialog.Update();
			return;
		}
		if (activeTextInput != null)
		{
			activeTextInput.Update();
			return;
		}
		if (activeControl != null)
		{
			activeControl.Update();
		}
		if (OnCancel != null)
		{
			PlayerIndex whoPressed = PlayerIndex.One;
			if (InputManager.MenuBack(ref whoPressed) && OnCancel(this, whoPressed) && OnCancelled != null)
			{
				OnCancelled(this, whoPressed);
			}
		}
	}

	public void ShowDialog(string title, string message, DialogOptions options, int delay, DialogHandler handler)
	{
		if (IsDialogShown)
		{
			throw new Exception("There's a dialog still active");
		}
		activeDialog = new Dialog(title, message, options, handler, delay, this);
		if (DialogCreated != null)
		{
			DialogCreated(this, activeDialog);
		}
	}

	public void ShowTextInput(string title, string message, string defaultText, TextInputHandler handler)
	{
		if (IsDialogShown)
		{
			throw new Exception("There's a dialog still active");
		}
		activeTextInput = new TextInputDialog(title, message, defaultText, handler, this);
		if (TextInputCreated != null)
		{
			TextInputCreated(this, activeTextInput);
		}
	}

	public void ShowDialog(string title, string message, DialogOptions options, DialogHandler handler)
	{
		ShowDialog(title, message, options, 0, handler);
	}

	public void ShowMessage(string title, string message)
	{
		if (IsDialogShown)
		{
			throw new Exception("There's a dialog still active");
		}
		activeDialog = new Dialog(title, message, DialogOptions.Ok, null, this);
		if (DialogCreated != null)
		{
			DialogCreated(this, activeDialog);
		}
	}

	public void HideDialog()
	{
		if (activeDialog != null)
		{
			activeDialog = null;
			if (DialogRemoved != null)
			{
				DialogRemoved(this);
			}
		}
		if (activeTextInput != null)
		{
			activeTextInput = null;
			if (TextInputRemoved != null)
			{
				TextInputRemoved(this);
			}
		}
	}

	public static Layout Load(string name)
	{
		XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Layouts/" + name);
		XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
		Layout layout = new Layout();
		layout.id = XDocHelper.GetAttribute(xDocument.Root, "id");
		layout.title = LanguageManager.Texts[XDocHelper.GetAttribute(xDocument.Root, "title")];
		layout.template = XDocHelper.GetAttribute(xDocument.Root, "template");
		foreach (XElement item in xDocument.Root.Elements())
		{
			Control control = ControlCreatorManager.Instance.ParseControl(item, layout);
			if (control != null)
			{
				layout.AddControl(control);
			}
		}
		return layout;
	}

	public Button AddButton(Group group, string name, string text, Action<Button, PlayerIndex> handler, Vector2 size)
	{
		Button button = new Button(this, group);
		button.SetId(name);
		button.Size = size;
		button.Text = text;
		button.OnInteraction += handler;
		group.addControl(button);
		return button;
	}

	public Button AddButton(Group group, string name, string text, Action<Button, PlayerIndex> handler)
	{
		Button button = new Button(this, group);
		button.SetId(name);
		button.Text = text;
		button.OnInteraction += handler;
		group.addControl(button);
		return button;
	}

	public Selector AddSelector(Group group, string name, string text, bool loop)
	{
		Selector selector = new Selector(this, group);
		selector.SetId(name);
		selector.Text = text;
		selector.Loop = loop;
		group.addControl(selector);
		return selector;
	}

	public Selector AddSelector(Group group, string name, string text, bool loop, Vector2 size)
	{
		Selector selector = new Selector(this, group);
		selector.SetId(name);
		selector.Text = text;
		selector.Size = size;
		selector.Loop = loop;
		group.addControl(selector);
		return selector;
	}

	public virtual void Dispose()
	{
		if (activeDialog != null)
		{
			activeDialog.Dispose();
			activeDialog = null;
		}
		OnCancelled = null;
		OnCancel = null;
		DialogCreated = null;
		DialogRemoved = null;
		activeControl = null;
		controls = null;
		if (controlList == null)
		{
			return;
		}
		foreach (Control control in controlList)
		{
			control.Dispose();
		}
		controlList = null;
	}
}
