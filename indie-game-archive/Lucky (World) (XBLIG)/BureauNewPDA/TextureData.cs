using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class TextureData
{
	public string textureName = "";

	public Texture2D texture;

	public bool isCompressed;

	public bool textureLoaded;

	public bool shouldLoadArray;

	public bool hasArrayBeenLoaded;

	public bool[,] containsAlpha;
}
