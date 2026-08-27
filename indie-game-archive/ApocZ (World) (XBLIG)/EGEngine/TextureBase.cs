using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class TextureBase
{
	public struct TextureDataElement
	{
		public int id;

		public string name;

		public Texture2D map2D;

		public TextureCube mapCube;
	}

	private static int baseId = 2000;

	public static List<TextureDataElement> textureList = new List<TextureDataElement>();

	public virtual void LoadContent()
	{
	}

	public static void GetMaterialsTextureByName(ContentManager contMgr, string name, out Texture2D diffuse, out Texture2D normalmap)
	{
		diffuse = null;
		normalmap = null;
		if (name == null || name.Contains("null"))
		{
			return;
		}
		bool flag = false;
		foreach (TextureDataElement texture in textureList)
		{
			if (texture.name == name)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			TextureDataElement item = new TextureDataElement
			{
				id = baseId,
				name = name,
				map2D = contMgr.Load<Texture2D>("textures\\" + name)
			};
			textureList.Add(item);
			diffuse = item.map2D;
			item.id = baseId;
			item.name = name + "_norm";
			item.map2D = contMgr.Load<Texture2D>("textures\\" + name + "_norm");
			textureList.Add(item);
			normalmap = item.map2D;
			baseId++;
			return;
		}
		foreach (TextureDataElement texture2 in textureList)
		{
			if (texture2.name == name)
			{
				diffuse = texture2.map2D;
				break;
			}
		}
		foreach (TextureDataElement texture3 in textureList)
		{
			if (texture3.name == name + "_norm")
			{
				normalmap = texture3.map2D;
				break;
			}
		}
	}

	public static void GetTexture2DByName(ContentManager contMgr, string name, out Texture2D texture)
	{
		texture = null;
		bool flag = false;
		foreach (TextureDataElement texture2 in textureList)
		{
			if (texture2.name == name)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			TextureDataElement item = new TextureDataElement
			{
				id = baseId,
				name = name,
				map2D = contMgr.Load<Texture2D>("textures\\" + name)
			};
			textureList.Add(item);
			texture = item.map2D;
			baseId++;
			return;
		}
		foreach (TextureDataElement texture3 in textureList)
		{
			if (texture3.name == name)
			{
				texture = texture3.map2D;
				break;
			}
		}
	}

	public static void GetTextureCubeByName(ContentManager contMgr, string name, out TextureCube texture)
	{
		texture = null;
		foreach (TextureDataElement texture2 in textureList)
		{
			if (texture2.name == name)
			{
				texture = texture2.mapCube;
				break;
			}
		}
	}
}
