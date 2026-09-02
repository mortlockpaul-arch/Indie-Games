using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace RacingGame.Helpers;

internal class Directories
{
	public static readonly string GameBaseDirectory = StorageContainer.TitleLocation;

	public static string ContentDirectory => "Content";

	public static string SoundsDirectory => Path.Combine(GameBaseDirectory, "Content\\Audio");

	public static string ScreenshotsDirectory => Path.Combine(GameBaseDirectory, "Screenshots");

	private Directories()
	{
	}
}
