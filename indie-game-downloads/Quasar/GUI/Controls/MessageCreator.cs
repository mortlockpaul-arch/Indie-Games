using System.Xml.Linq;

namespace Quasar.GUI.Controls;

public class MessageCreator : IControlCreator
{
	private static MessageCreator instance;

	public static MessageCreator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new MessageCreator();
			}
			return instance;
		}
	}

	public Control ParseXml(XElement xe, Layout layout)
	{
		Message message = new Message(layout);
		message.parseXml(xe);
		return message;
	}
}
