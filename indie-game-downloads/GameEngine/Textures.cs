using EGEngine;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine;

public class Textures : TextureBase
{
	private const int numTextures = 2;

	private string[] textureNames = new string[2] { "button01", "button02" };

	public override void LoadContent()
	{
		for (int i = 0; i < 2; i++)
		{
			TextureDataElement item = default(TextureDataElement);
			item.id = i + 1;
			item.name = textureNames[i];
			item.map2D = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\menus\\" + item.name);
			TextureBase.textureList.Add(item);
		}
	}

	public virtual void GetMaterialsTextureByName(string name, out Texture2D diffuse, out Texture2D normalmap)
	{
		diffuse = null;
		normalmap = null;
		foreach (TextureDataElement texture in TextureBase.textureList)
		{
			if (texture.name == name)
			{
				diffuse = texture.map2D;
				break;
			}
		}
		foreach (TextureDataElement texture2 in TextureBase.textureList)
		{
			if (texture2.name == name + "_norm")
			{
				normalmap = texture2.map2D;
				break;
			}
		}
	}

	public virtual void GetTexture2DByName(string name, out Texture2D texture)
	{
		texture = null;
		foreach (TextureDataElement texture2 in TextureBase.textureList)
		{
			if (texture2.name == name)
			{
				texture = texture2.map2D;
				break;
			}
		}
	}

	public virtual void GetTextureCubeByName(string name, out TextureCube texture)
	{
		texture = null;
		foreach (TextureDataElement texture2 in TextureBase.textureList)
		{
			if (texture2.name == name)
			{
				texture = texture2.mapCube;
				break;
			}
		}
	}
}
