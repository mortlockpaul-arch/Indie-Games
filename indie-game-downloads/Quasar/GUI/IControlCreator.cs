using System.Xml.Linq;

namespace Quasar.GUI;

public interface IControlCreator
{
	Control ParseXml(XElement xe, Layout layout);
}
