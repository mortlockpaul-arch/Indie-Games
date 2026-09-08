using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Quasar.GUI.Controls;

public class Label : Control
{
	public delegate void LabelHandler(Label label);

	public class LabelCreator : IControlCreator
	{
		private static LabelCreator instance;

		public static LabelCreator Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new LabelCreator();
				}
				return instance;
			}
		}

		public Control ParseXml(XElement xe, Layout layout)
		{
			Label label = new Label(layout);
			label.parseXml(xe);
			return label;
		}
	}

	public const string Type = "Label";

	private string text = "";

	private Vector2 position;

	private Vector2 size;

	public override string ControlType => "Label";

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			if (OnTextChange != null)
			{
				OnTextChange(this);
			}
		}
	}

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public event LabelHandler OnTextChange;

	public Label(Layout layout)
		: base(layout)
	{
	}

	public override void parseXml(XElement xe)
	{
		base.parseXml(xe);
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
