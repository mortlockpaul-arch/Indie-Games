using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Containers;
using Microsoft.Xna.Framework;
using Platformer.Asset_Classes;
using PlatformerFromHell.Asset_Classes;

namespace PlatformerFromHell;

internal class LevelLoader
{
	public PlatformerGame platformerGame;

	public readonly int levelWidth;

	public readonly int levelHeight;

	public readonly int levelTime;

	public Vector2 startLocation;

	public List<Asset> groundAssets = new List<Asset>();

	public List<Asset> wallAssets = new List<Asset>();

	public List<Asset> platformAssets = new List<Asset>();

	public List<Asset> switchAssets = new List<Asset>();

	public List<Asset> gravityAssets = new List<Asset>();

	public List<Asset> hellevatorAssets = new List<Asset>();

	public readonly List<Asset> levelAssets = new List<Asset>();

	public XHashSet<Asset>[,] zonePanels;

	private Level level;

	private Dictionary<int, AssetDefinition> definitions = new Dictionary<int, AssetDefinition>();

	public LevelLoader(Level level, string fileName)
	{
		this.level = level;
		StreamReader streamReader = new StreamReader(fileName);
		string text = "";
		while ((text = streamReader.ReadLine()) != null)
		{
			if (text == "")
			{
				continue;
			}
			char c = text.ElementAt(0);
			text = text.Substring(1);
			switch (c)
			{
			case 'W':
				levelWidth = int.Parse(text);
				break;
			case 'H':
				levelHeight = int.Parse(text);
				break;
			case 'T':
				levelTime = int.Parse(text);
				break;
			case 'D':
			{
				AssetDefinition assetDefinition = loadDefinition(text);
				definitions.Add(assetDefinition.defID, assetDefinition);
				break;
			}
			case 'A':
			{
				Asset asset = loadAsset(text);
				if (asset != null)
				{
					if (asset.texturename.Contains("ground"))
					{
						groundAssets.Add(asset);
					}
					else if (asset is Wall)
					{
						wallAssets.Add(asset);
					}
					else if (asset is Platform)
					{
						platformAssets.Add(asset);
					}
					else if (asset is Switch || asset is Background)
					{
						switchAssets.Add(asset);
					}
					else if (asset is Gravity)
					{
						gravityAssets.Add(asset);
					}
					else if (asset is ExitAsset || asset is StartAsset)
					{
						hellevatorAssets.Add(asset);
					}
				}
				break;
			}
			}
		}
		GenerateFullAssetList();
		SortZoneAssets();
	}

	private void GenerateFullAssetList()
	{
		foreach (Asset hellevatorAsset in hellevatorAssets)
		{
			levelAssets.Add(hellevatorAsset);
		}
		foreach (Asset gravityAsset in gravityAssets)
		{
			levelAssets.Add(gravityAsset);
		}
		foreach (Asset platformAsset in platformAssets)
		{
			levelAssets.Add(platformAsset);
		}
		foreach (Asset wallAsset in wallAssets)
		{
			levelAssets.Add(wallAsset);
		}
		foreach (Asset groundAsset in groundAssets)
		{
			levelAssets.Add(groundAsset);
		}
		foreach (Asset switchAsset in switchAssets)
		{
			levelAssets.Add(switchAsset);
		}
	}

	private void SortZoneAssets()
	{
		int num = levelWidth / 100 + 1;
		int num2 = levelHeight / 100 + 1;
		zonePanels = new XHashSet<Asset>[num, num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				zonePanels[i, j] = new XHashSet<Asset>();
			}
		}
		for (int k = 0; k < levelAssets.Count; k++)
		{
			Asset asset = levelAssets[k];
			Rectangle rectangle = new Rectangle((int)asset.Position.X, (int)asset.Position.Y, asset.frameWidth, asset.frameHeight);
			int num3 = Math.Max(rectangle.Left / 100, 0);
			int num4 = Math.Min(rectangle.Right / 100, num - 1);
			int num5 = Math.Max(rectangle.Top / 100, 0);
			int num6 = Math.Min(rectangle.Bottom / 100, num2 - 1);
			for (int i = num4; i >= num3; i--)
			{
				for (int j = num6; j >= num5; j--)
				{
					zonePanels[i, j].Add(asset);
				}
			}
		}
		level.finishedLoading = true;
	}

	private AssetDefinition loadDefinition(string description)
	{
		string[] array = description.Split(',');
		int defID = int.Parse(array[0]);
		string type = array[1];
		string textureName = array[2].Replace(".png", "");
		int fc = 1;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.StartsWith("Fc:"))
			{
				fc = int.Parse(text.Substring(3));
				break;
			}
		}
		return new AssetDefinition(defID, type, textureName, fc);
	}

	private Asset loadAsset(string description)
	{
		string[] array = description.Split(',');
		int key = int.Parse(array[0]);
		int num = int.Parse(array[1]);
		int num2 = int.Parse(array[2]);
		Vector2 position = new Vector2(num, num2);
		AssetDefinition assetDefinition = null;
		assetDefinition = definitions[key];
		if (assetDefinition == null)
		{
			Console.WriteLine("Failed to load an Asset: No matching AssetDefinition found.  Asset Definitions must be loaded first.");
			return null;
		}
		string text = assetDefinition.typeSymbol;
		string textureName = assetDefinition.textureName;
		int frameCount = assetDefinition.frameCount;
		Asset.Dir dir = parseDir(description);
		int num3 = (int)dir;
		if (text.Equals("GV"))
		{
			text = ((num3 <= 1) ? "GU" : "GD");
		}
		else if (text.Equals("GH"))
		{
			text = ((num3 % 2 != 0) ? "GR" : "GL");
		}
		switch (text)
		{
		case "PL":
		{
			char newSwitchID = 'A';
			if (array.Length == 6)
			{
				newSwitchID = array[5].ElementAt(0);
			}
			return new Platform(level, position, textureName, frameCount, dir, newSwitchID);
		}
		case "SW":
			return new Switch(switchID: (array.Length >= 6) ? array[5].ElementAt(0) : 'A', level: level, position: position, texturename: textureName, frameCount: frameCount, flip: dir);
		case "WA":
			return new Wall(level, position, textureName, frameCount, dir);
		case "GU":
			return new GravityUp(level, position, textureName, frameCount, dir);
		case "GD":
			return new GravityDown(level, position, textureName, frameCount, dir);
		case "GL":
			return new GravityLeft(level, position, textureName, frameCount, dir);
		case "GR":
			return new GravityRight(level, position, textureName, frameCount, dir);
		case "ST":
			startLocation = position;
			return new StartAsset(level, position, textureName, frameCount, dir);
		case "EX":
			return new ExitAsset(level, position, textureName, frameCount, dir);
		default:
			Console.Out.WriteLine("WARNING: Default case in loadAsset !");
			Console.Out.WriteLine(description + " tex=" + textureName + " type=" + text);
			return null;
		}
	}

	public Asset.Dir parseDir(string description)
	{
		if (description.Contains("FlippedDown"))
		{
			return Asset.Dir.DownLeft;
		}
		if (description.Contains("D:UpLeft"))
		{
			return Asset.Dir.UpLeft;
		}
		if (description.Contains("D:UpRight"))
		{
			return Asset.Dir.UpRight;
		}
		if (description.Contains("D:DownLeft"))
		{
			return Asset.Dir.DownLeft;
		}
		if (description.Contains("D:DownRight"))
		{
			return Asset.Dir.DownRight;
		}
		return Asset.Dir.UpLeft;
	}

	public int parseFC(string description)
	{
		int num = description.IndexOf("FC:");
		if (num == -1)
		{
			return 1;
		}
		int num2 = 3 + num;
		return int.Parse(description.Substring(3 + num, Math.Min(description.IndexOf(',', num), description.Length - 1)));
	}

	internal void Dispose()
	{
		groundAssets.Clear();
		wallAssets.Clear();
		platformAssets.Clear();
		switchAssets.Clear();
		gravityAssets.Clear();
		hellevatorAssets.Clear();
		levelAssets.Clear();
		XHashSet<Asset>[,] array = zonePanels;
		foreach (XHashSet<Asset> xHashSet in array)
		{
			xHashSet.Clear();
		}
	}
}
