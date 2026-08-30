using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Defenders;

public class AssetManager
{
	public List<Asset> asset;

	public AssetManager(bool b)
	{
		asset = new List<Asset>(999);
	}

	public AssetManager()
	{
		asset = new List<Asset>(999);
		asset.Add(new Asset());
		asset.Add(new Asset(LevelData.playerSpawn, 1));
		asset.Add(new Asset(LevelData.playerSpawn, 2));
		asset.Add(new Asset(LevelData.playerSpawn, 3));
		asset.Add(new Asset(LevelData.playerSpawn, 4));
	}

	public void Add(Asset a)
	{
		asset.Add(a);
	}

	public void Add(LevelData levelData, Vector2 position, string name, string desc, float angle, float size, int type, uint frame, string text, Color color, Color color2, Color color3, int numSec, int numPri)
	{
		asset.Add(new Asset(levelData, position, name, desc, angle, size, type, frame, text, color, color2, color3, numSec, numPri));
	}

	public bool isMouseOver(int element, Vector2 mouse)
	{
		int num = 40;
		Rectangle rectangle = new Rectangle((int)(asset[element].position.X - (float)(num / 2)), (int)(asset[element].position.Y - (float)(num / 2)), num, num);
		Rectangle value = new Rectangle((int)(mouse.X - 2f), (int)(mouse.Y - 2f), 4, 4);
		return rectangle.Intersects(value);
	}

	public int Selection(Vector2 mouse)
	{
		int result = -1;
		for (int i = 0; i < asset.Count; i++)
		{
			if (isMouseOver(i, mouse))
			{
				result = i;
			}
		}
		return result;
	}
}
