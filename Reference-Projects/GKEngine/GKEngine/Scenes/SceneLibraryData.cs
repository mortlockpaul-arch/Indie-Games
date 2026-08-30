using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GKEngine.Scenes;

public class SceneLibraryData
{
	public List<AssetEntity> Assets = new List<AssetEntity>();

	public List<AssetSequence> AssetSequences = new List<AssetSequence>();

	public List<TextureSheet> TextureSheets = new List<TextureSheet>();

	public List<TextureSheetSequence> TextureSheetSequences = new List<TextureSheetSequence>();

	public static SceneLibraryData Load(string xFileName)
	{
		Stream stream = File.OpenRead(xFileName);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SceneLibraryData));
		return (SceneLibraryData)xmlSerializer.Deserialize(stream);
	}
}
