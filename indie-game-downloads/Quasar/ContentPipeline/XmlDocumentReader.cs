using Microsoft.Xna.Framework.Content;

namespace Quasar.ContentPipeline;

public class XmlDocumentReader : ContentTypeReader<XmlSource>
{
	protected override XmlSource Read(ContentReader input, XmlSource existingInstance)
	{
		string xmlCode = input.ReadString();
		return new XmlSource(xmlCode);
	}
}
