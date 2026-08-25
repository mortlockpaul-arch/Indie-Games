using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace RenegadeEngine;

public static class AssetManager
{
	private static ContentManager content;

	private static Dictionary<FontKeys, SpriteFont> Fonts = new Dictionary<FontKeys, SpriteFont>();

	private static Dictionary<ImageKeys, Texture2D> Images = new Dictionary<ImageKeys, Texture2D>();

	private static Dictionary<EffectKeys, Effect> Effects = new Dictionary<EffectKeys, Effect>();

	private static Dictionary<ModelKeys, Model> Models = new Dictionary<ModelKeys, Model>();

	private static Dictionary<MusicKeys, Song> Music = new Dictionary<MusicKeys, Song>();

	public static SpriteFont GetAsset(FontKeys key)
	{
		if (Fonts.ContainsKey(key))
		{
			return Fonts[key];
		}
		ErrorLogger.LogError("Font not found: " + key);
		return Fonts[FontKeys.MenuFont];
	}

	public static void GetAsset(FontKeys key, ref SpriteFont asset)
	{
		if (Fonts.ContainsKey(key))
		{
			asset = Fonts[key];
			return;
		}
		ErrorLogger.LogError("Font not found: " + key);
		asset = Fonts[FontKeys.MenuFont];
	}

	public static void GetAssets(FontKeys[] keys, ref SpriteFont[] assets)
	{
		List<SpriteFont> list = new List<SpriteFont>();
		foreach (FontKeys fontKeys in keys)
		{
			if (Fonts.ContainsKey(fontKeys))
			{
				list.Add(Fonts[fontKeys]);
				continue;
			}
			ErrorLogger.LogError("Font not found: " + fontKeys);
			list.Add(Fonts[FontKeys.MenuFont]);
		}
		assets = list.ToArray();
	}

	public static SpriteFont GetFont(string filename)
	{
		return content.Load<SpriteFont>(filename);
	}

	public static Effect GetAsset(EffectKeys key)
	{
		if (Effects.ContainsKey(key))
		{
			return Effects[key];
		}
		throw new Exception(key.ToString() + " not found");
	}

	public static void GetAsset(EffectKeys key, out Effect asset)
	{
		if (Effects.ContainsKey(key))
		{
			asset = Effects[key];
			return;
		}
		throw new Exception(key.ToString() + " not found");
	}

	public static Texture2D GetAsset(ImageKeys key)
	{
		if (Images.ContainsKey(key))
		{
			return Images[key];
		}
		throw new Exception(key.ToString() + " not found");
	}

	public static void GetAsset(ImageKeys key, ref Texture2D asset)
	{
		if (Images.ContainsKey(key))
		{
			asset = Images[key];
			return;
		}
		throw new Exception(key.ToString() + " not found");
	}

	public static void GetAssets(ImageKeys[] keys, ref Texture2D[] assets)
	{
		List<Texture2D> list = new List<Texture2D>();
		foreach (ImageKeys imageKeys in keys)
		{
			if (Images.ContainsKey(imageKeys))
			{
				list.Add(Images[imageKeys]);
				continue;
			}
			throw new Exception(imageKeys.ToString() + " not found");
		}
		assets = list.ToArray();
	}

	public static Texture2D GetImage(string fileName)
	{
		return content.Load<Texture2D>(fileName);
	}

	public static Model GetAsset(ModelKeys key)
	{
		if (Models.ContainsKey(key))
		{
			return Models[key];
		}
		throw new Exception(key.ToString() + " not found");
	}

	public static void GetAsset(ModelKeys key, out Model asset)
	{
		if (Models.ContainsKey(key))
		{
			asset = Models[key];
			return;
		}
		throw new Exception(key.ToString() + " not found");
	}

	public static void GetAssets(ModelKeys[] keys, out Model[] assets)
	{
		List<Model> list = new List<Model>();
		foreach (ModelKeys modelKeys in keys)
		{
			if (Models.ContainsKey(modelKeys))
			{
				list.Add(Models[modelKeys]);
				continue;
			}
			throw new Exception(modelKeys.ToString() + " not found");
		}
		assets = list.ToArray();
	}

	public static Model GetModel(string filename)
	{
		return content.Load<Model>(filename);
	}

	public static Song GetAsset(MusicKeys key)
	{
		if (Music.ContainsKey(key))
		{
			return Music[key];
		}
		throw new Exception(key.ToString() + " not found.");
	}

	public static void GetAsset(MusicKeys key, out Song song)
	{
		if (Music.ContainsKey(key))
		{
			song = Music[key];
			return;
		}
		throw new Exception(key.ToString() + " not found.");
	}

	public static void LoadContent(ContentManager Content)
	{
		content = Content;
		try
		{
			Fonts.Add(FontKeys.MenuFont, Content.Load<SpriteFont>("MenuFont"));
			Fonts.Add(FontKeys.TitleFont, Content.Load<SpriteFont>("TitleFont"));
		}
		catch (ContentLoadException ex)
		{
			throw ex;
		}
		try
		{
			Images.Add(ImageKeys.background, Content.Load<Texture2D>("background"));
			Images.Add(ImageKeys.pixel, Content.Load<Texture2D>("pixel"));
			Images.Add(ImageKeys.titleCredits, Content.Load<Texture2D>("titleCredits"));
		}
		catch (ContentLoadException ex2)
		{
			throw ex2;
		}
		try
		{
			Effects.Add(EffectKeys.BloomEffect, Content.Load<Effect>("BloomEffect"));
		}
		catch (ContentLoadException ex3)
		{
			throw ex3;
		}
		try
		{
			Models.Add(ModelKeys.icoSphere, Content.Load<Model>("icoSphere"));
		}
		catch (ContentLoadException ex4)
		{
			throw ex4;
		}
		try
		{
			Music.Add(MusicKeys.TimeToDream, content.Load<Song>("DST-TimeToDream"));
			Music.Add(MusicKeys.ThisMachine, content.Load<Song>("DST-ThisMachine"));
		}
		catch (ContentLoadException ex5)
		{
			throw ex5;
		}
	}
}
