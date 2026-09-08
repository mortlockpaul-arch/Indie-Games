using System.IO;
using Microsoft.Xna.Framework.Media;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Content;

internal class VideoReader : ContentTypeReader<Video>
{
	internal static readonly string[] supportedExtensions = new string[2] { ".ogv", ".ogg" };

	protected internal override Video Read(ContentReader input, Video existingInstance)
	{
		string text = FileHelpers.ResolveRelativePath(Path.Combine(input.ContentManager.RootDirectoryFullPath, input.AssetName), input.ReadObject<string>());
		string text2 = Normalize(text.Substring(0, text.Length - 4));
		if (!string.IsNullOrEmpty(text2))
		{
			text = text2;
		}
		int durationMS = input.ReadObject<int>();
		int width = input.ReadObject<int>();
		int height = input.ReadObject<int>();
		float framesPerSecond = input.ReadObject<float>();
		VideoSoundtrackType soundtrackType = (VideoSoundtrackType)input.ReadObject<int>();
		return new Video(text, input.ContentManager.GetGraphicsDevice(), durationMS, width, height, framesPerSecond, soundtrackType);
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
