using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Textures;

public class CubeTextureManager : Manager<TextureCube>
{
	private static CubeTextureManager instance;

	public static CubeTextureManager Textures
	{
		get
		{
			if (instance == null)
			{
				instance = new CubeTextureManager();
			}
			return instance;
		}
	}

	private CubeTextureManager()
	{
	}

	protected override TextureCube createDefaultItem()
	{
		return Engine.ContentManager.Load<TextureCube>("Textures/default_cube");
	}

	protected override TextureCube LoadItem(string name)
	{
		return Engine.ContentManager.Load<TextureCube>("Textures/" + name);
	}

	public static TextureCube LoadTexture(string path)
	{
		return LoadTexture(path, addToManager: false);
	}

	public static TextureCube LoadTexture(string path, bool addToManager)
	{
		if (addToManager && Textures.TryGetValue(path, out var item))
		{
			return item;
		}
		try
		{
			item = Engine.ContentManager.Load<TextureCube>(path);
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
