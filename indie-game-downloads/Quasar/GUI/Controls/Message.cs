using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.GUI.Controls;

public class Message : Control
{
	public const string Type = "Message";

	private string text;

	public override string ControlType => "Message";

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public Message(Layout layout)
		: base(layout)
	{
	}

	public override void parseXml(XElement xe)
	{
		text = XDocHelper.GetAttribute(xe, "text");
		base.parseXml(xe);
	}
}
