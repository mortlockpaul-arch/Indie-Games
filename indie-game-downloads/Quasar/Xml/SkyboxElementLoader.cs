using System.Xml.Linq;
using Quasar.Global;
using Quasar.Items;

namespace Quasar.Xml;

internal class SkyboxElementLoader : BaseElementLoader
{
	public override Element Load(XElement xe, string currentPath)
	{
		Skybox skybox = new Skybox(XDocHelper.GetAttribute(xe, "texture"));
		LoadCommonData(skybox, xe, currentPath, loadChildren: true);
		return skybox;
	}
}
