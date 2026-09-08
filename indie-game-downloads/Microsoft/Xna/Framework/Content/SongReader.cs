using System.IO;
using Microsoft.Xna.Framework.Media;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Content;

internal class SongReader : ContentTypeReader<Song>
{
	internal static readonly string[] supportedExtensions = new string[3] { ".ogg", ".oga", ".qoa" };

	protected internal override Song Read(ContentReader input, Song existingInstance)
	{
		string text = FileHelpers.ResolveRelativePath(Path.Combine(input.ContentManager.RootDirectoryFullPath, input.AssetName), input.ReadString());
		string text2 = Normalize(text.Substring(0, text.Length - 4));
		if (!string.IsNullOrEmpty(text2))
		{
			text = text2;
		}
		int durationMS = input.ReadInt32();
		return new Song(text, input.AssetName, durationMS);
	}

	private static string Normalize(string fileName)
	{
		if (File.Exists(fileName))
		{
			return fileName;
		}
		string[] array = supportedExtensions;
		foreach (string text in array)
		{
			string text2 = fileName + text;
			if (File.Exists(text2))
			{
				return text2;
			}
		}
		return null;
	}
}
