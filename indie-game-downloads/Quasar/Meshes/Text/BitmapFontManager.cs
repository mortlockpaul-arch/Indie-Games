using Quasar.Global;

namespace Quasar.Meshes.Text;

public class BitmapFontManager : Manager<BitmapFont>
{
	private static BitmapFontManager instance;

	public static BitmapFontManager Fonts
	{
		get
		{
			if (instance == null)
			{
				instance = new BitmapFontManager();
			}
			return instance;
		}
	}

	private BitmapFontManager()
	{
	}

	protected override BitmapFont createDefaultItem()
	{
		return null;
	}

	protected override BitmapFont LoadItem(string name)
	{
		return new BitmapFont("Fonts/" + name);
	}
}
