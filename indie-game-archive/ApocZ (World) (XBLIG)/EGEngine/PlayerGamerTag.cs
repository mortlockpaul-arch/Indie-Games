using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class PlayerGamerTag
{
	public string gamerTag = "";

	public Vector2 textOffset;

	public Texture2D gamerLabel;

	public Texture2D gamerPicture;

	public GamerProfile gamerProfile;

	private static bool Initialized = false;

	public static Texture2D defaultGamerPicture = null;

	private static Color gtColor = Color.White;

	private static Vector2 newPos = Vector2.Zero;

	private static Rectangle gtRec = default(Rectangle);

	public void SetRemote(string gt)
	{
		if (!Initialized)
		{
			Initialized = true;
			gamerTag = gt;
			textOffset = Menu.defaultFont.MeasureString(gamerTag) / 1.8f;
		}
	}

	public void Set(string gt)
	{
		if (!Initialized)
		{
			Initialized = true;
			defaultGamerPicture = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\defaultPicture");
		}
		gamerTag = gt;
		textOffset = Menu.defaultFont.MeasureString(gamerTag) / 1.8f;
	}

	public void Draw(ref Vector2 pos2D, byte alpha, bool offsetTag, bool drawPicture)
	{
		Draw(ref pos2D, alpha, offsetTag, drawPicture, 1f, drawAsSelected: false, drawIsHost: false);
	}

	public void Draw(ref Vector2 pos2D, byte alpha, bool offsetTag, bool drawPicture, float scale, bool drawAsSelected, bool drawIsHost)
	{
	}
}
