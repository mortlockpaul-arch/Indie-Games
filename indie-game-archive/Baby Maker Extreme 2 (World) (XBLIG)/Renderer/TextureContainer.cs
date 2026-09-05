using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public static class TextureContainer
{
	public class DrawComponentComparer : IComparer<DrawableComponent>
	{
		public int Compare(DrawableComponent x, DrawableComponent y)
		{
			if (x.Depth > y.Depth)
			{
				return 1;
			}
			if (x.Depth == y.Depth)
			{
				return 0;
			}
			return -1;
		}
	}

	private const int MAX_TEXT = 2000;

	public static string SPECIAL_PAGE_NAME_GAMERPIC = "SPECIAL_PAGE_GAMERPIC";

	private static Dictionary<string, SpritePage> pages;

	private static Dictionary<string, Texture2D> texPages;

	private static GraphicsDevice device;

	private static ContentManager content;

	private static List<DrawableComponent> sm_drawList;

	private static DrawComponentComparer spriteComparer;

	private static List<RenderText> sm_textPool;

	private static int sm_iTextIndex;

	public static void Initialize(GraphicsDevice d, ContentManager c)
	{
		spriteComparer = new DrawComponentComparer();
		sm_drawList = new List<DrawableComponent>(10000);
		device = d;
		content = c;
		pages = new Dictionary<string, SpritePage>();
		texPages = new Dictionary<string, Texture2D>();
		sm_iTextIndex = 0;
		sm_textPool = new List<RenderText>(2000);
		for (int i = 0; i < 2000; i++)
		{
			sm_textPool.Add(new RenderText());
		}
	}

	public static SpriteInstance GetSprite(string pageName, Vector2 pos, float depth)
	{
		return new SpriteInstance(GetImage(pageName), pos, depth);
	}

	public static SpriteInstance GetSprite(string pageName, Rectangle pageCoords, Vector2 pos, float depth)
	{
		return new SpriteInstance(GetImage(pageName, pageCoords), pos, depth);
	}

	public static SpriteImage GetImage(string pageName, Rectangle pageCoords)
	{
		return new SpriteImage(GetPage(pageName), pageCoords);
	}

	public static SpriteImage GetImage(string pageName)
	{
		SpritePage page = GetPage(pageName);
		Rectangle coords = new Rectangle(0, 0, page.DiffuseTex.Width, page.DiffuseTex.Height);
		return new SpriteImage(page, coords);
	}

	public static void SetSpecialPage(Texture2D tex, string pageName)
	{
		pages[pageName] = new SpritePage(tex, pageName);
	}

	public static SpritePage GetPage(string pageName)
	{
		pageName = pageName.ToLower();
		if (pages.ContainsKey(pageName))
		{
			return pages[pageName];
		}
		pages[pageName] = new SpritePage(GetTexture(pageName), pageName);
		return pages[pageName];
	}

	public static Texture2D GetTexture(string texName)
	{
		texName = texName.ToLower();
		if (texPages.ContainsKey(texName))
		{
			return texPages[texName];
		}
		texPages[texName] = content.Load<Texture2D>(texName);
		return texPages[texName];
	}

	public static void Update(GameTime gameTime)
	{
	}

	public static void AddSpriteRefForDraw(SpriteInstance s)
	{
		sm_drawList.Add(s);
	}

	public static void AddTextRefForDraw(textData t, float depth)
	{
		RenderText textRef = GetTextRef();
		textRef.Initialize(t, depth);
		sm_drawList.Add(textRef);
	}

	private static RenderText GetTextRef()
	{
		sm_iTextIndex++;
		return sm_textPool[sm_iTextIndex - 1];
	}

	public static void Draw(GameTime gameTime)
	{
		sm_drawList.Clear();
		sm_iTextIndex = 0;
	}
}
