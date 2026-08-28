namespace Elite.Core.Shared;

public class GameFile
{
	public GameVersion[] GameVersions { get; set; }

	public string Name { get; set; }

	public string GameSaveContainerName { get; set; }

	public InfoPage[] InfoPages { get; set; }

	public string StartInstructions { get; set; }

	public float HeaderX { get; set; }

	public float HeaderY { get; set; }

	public string[] HeaderTextures { get; set; }

	public string CopyrightText { get; set; }

	public int HeaderTextureSpeed { get; set; }

	public string ButtonY { get; set; }
}
