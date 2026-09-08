using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Textures;

public class TextureManager : Manager<Texture2D>
{
	public const string TEXTURES_DIR = "Textures/";

	private static TextureManager instance;

	public static TextureManager Textures
	{
		get
		{
			if (instance == null)
			{
				instance = new TextureManager();
			}
			return instance;
		}
	}

	public Texture2D ErrorItem => base["Error"];

	private TextureManager()
	{
	}

	protected override Texture2D createDefaultItem()
	{
		return Engine.ContentManager.Load<Texture2D>("Textures/default");
	}

	public static Texture2D LoadTexture(string path)
	{
		return LoadTexture(path, addToManager: false);
	}

	public static void UnloadTexture(string path)
	{
		Engine.ContentTracker.Release(path);
	}

	protected override Texture2D LoadItem(string name)
	{
		return Engine.ContentManager.Load<Texture2D>("Textures/" + name);
	}

	public static Texture2D LoadTexture(string path, bool addToManager)
	{
		if (addToManager && Textures.TryGetValue(path, out var item))
		{
			return item;
		}
		try
		{
			item = Engine.ContentManager.Load<Texture2D>(path);
		}
		catch
		{
			item = Textures.DefaultItem;
		}
		if (addToManager)
		{
			Textures.Add(path, item);
		}
		return item;
	}
}
