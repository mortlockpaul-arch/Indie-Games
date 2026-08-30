using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public static class SpriteManager
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
		sm_iTextIndex = 0;
		sm_textPool = new List<RenderText>(2000);
		for (int i = 0; i < 2000; i++)
		{
			sm_textPool.Add(new RenderText());
		}
	}

	public static void ClearThemedSprites()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, SpritePage> page in pages)
		{
			if (page.Value.IsThemePage)
			{
				list.Add(page.Key);
			}
		}
		foreach (string item in list)
		{
			pages.Remove(item);
		}
	}

	public static RenderSprite GetSprite(string pageName, Vector2 pos, float depth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetSprite(pageName, pos, depth, isTheme: false);
	}

	public static RenderSprite GetSprite(string pageName, Rectangle pageCoords, Vector2 pos, float depth)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetSprite(pageName, pageCoords, pos, depth, isTheme: false);
	}

	public static RenderSprite GetSprite(string pageName, Vector2 pos, float depth, bool isTheme)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new RenderSprite(GetImage(pageName, isTheme), pos, depth);
	}

	public static RenderSprite GetSprite(string pageName, Rectangle pageCoords, Vector2 pos, float depth, bool isTheme)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return new RenderSprite(GetImage(pageName, pageCoords, isTheme), pos, depth);
	}

	public static SpriteImage GetImage(string pageName)
	{
		return GetImage(pageName, isTheme: false);
	}

	public static SpriteImage GetImage(string pageName, Rectangle pageCoords)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetImage(pageName, pageCoords, isTheme: false);
	}

	public static SpriteImage GetImage(string pageName, Rectangle pageCoords, bool isTheme)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new SpriteImage(GetPage(pageName, isTheme), pageCoords);
	}

	public static SpriteImage GetImage(string pageName, bool isTheme)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SpritePage page = GetPage(pageName, isTheme);
		Rectangle coords = default(Rectangle);
		((Rectangle)(ref coords))._002Ector(0, 0, page.Texture.Width, page.Texture.Height);
		return new SpriteImage(page, coords);
	}

	public static void SetSpecialPage(Texture2D tex, string pageName)
	{
		pages[pageName] = new SpritePage(tex, pageName, b: false);
	}

	public static SpritePage GetPage(string pageName, bool isTheme)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (pageName.Equals(SPECIAL_PAGE_NAME_GAMERPIC))
		{
			PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
			SignedInGamer val = null;
			GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SignedInGamer current = enumerator.Current;
					if (current.PlayerIndex == playerIndex)
					{
						val = current;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			if (val != null)
			{
				GamerProfile profile = ((Gamer)val).GetProfile();
				pages[pageName] = new SpritePage(profile.GamerPicture, pageName, isTheme);
			}
			else
			{
				pages[pageName] = new SpritePage(content.Load<Texture2D>("blackzone"), pageName, isTheme);
			}
			return pages[pageName];
		}
		if (pages.ContainsKey(pageName))
		{
			return pages[pageName];
		}
		pages[pageName] = new SpritePage(content.Load<Texture2D>(pageName), pageName, isTheme);
		return pages[pageName];
	}

	public static void Update(GameTime gameTime)
	{
	}

	public static void AddSpriteRefForDraw(RenderSprite s)
	{
		sm_drawList.Add(s);
	}

	public static void AddTextRefForDraw(SceneRenderer.textData t, float depth)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cameraPosition = SceneRenderer.GetCameraPosition() - SceneRenderer.GetViewportDim() / (SceneRenderer.GetCameraZoom() * 2f);
		SceneRenderer.IniFrame(b: true);
		sm_drawList.Sort(spriteComparer);
		SceneRenderer.SetTexturedRectData();
		AvatarHandler avatar = SceneRenderer.Avatar;
		bool flag = false;
		for (int i = 0; i < sm_drawList.Count; i++)
		{
			if (sm_drawList[i] is RenderSprite)
			{
				RenderSprite renderSprite = (RenderSprite)sm_drawList[i];
				float alpha = renderSprite.Alpha;
				Color color = renderSprite.Color;
				bool blendAdditive = renderSprite.BlendAdditive;
				if (i > 0)
				{
					SceneRenderer.SetActiveAlpha(alpha);
					SceneRenderer.SetActiveColor(color);
					if (!(sm_drawList[i - 1] is RenderSprite) || ((RenderSprite)sm_drawList[i - 1]).BlendAdditive != blendAdditive)
					{
						SceneRenderer.SetBlendMode(blendAdditive);
					}
				}
				else
				{
					SceneRenderer.SetActiveAlpha(alpha);
					SceneRenderer.SetActiveColor(color);
					SceneRenderer.SetBlendMode(blendAdditive);
				}
				if (!flag && avatar != null && avatar.ShouldDraw && renderSprite.Depth > avatar.Depth)
				{
					flag = true;
					avatar.Draw();
					SceneRenderer.ResetBatch();
				}
				SceneRenderer.DrawRenderSpriteFromManager(renderSprite, inPass: false);
			}
			else
			{
				SceneRenderer.DrawFinalString((RenderText)sm_drawList[i], cameraPosition);
			}
		}
		sm_drawList.Clear();
		sm_iTextIndex = 0;
	}
}
