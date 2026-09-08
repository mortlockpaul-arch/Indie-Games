using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.Xml;

internal class ModelElementLoader : BaseElementLoader
{
	public override Element Load(XElement xe, string currentPath)
	{
		RenderItem renderItem = ModelLoader.LoadModelDefinition(Engine.ProcessPath(currentPath, XDocHelper.GetAttribute(xe, "source")));
		LoadCommonData(renderItem, xe, currentPath, loadChildren: true);
		return renderItem;
	}
}
