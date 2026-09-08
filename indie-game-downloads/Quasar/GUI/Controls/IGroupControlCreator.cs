using System.Xml.Linq;

namespace Quasar.GUI.Controls;

public interface IGroupControlCreator
{
	GroupControl ParseXml(XElement xe, Group group, Layout layout);
}
