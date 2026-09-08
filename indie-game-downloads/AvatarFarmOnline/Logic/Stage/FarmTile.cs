using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal class FarmTile
{
	private Int2 tile;

	private AvatarFarmOnline.Logic.Stage.TileContents contents;

	private AvatarFarmOnline.Logic.Stage.FarmData farmData;

	public Int2 Tile => tile;

	public AvatarFarmOnline.Logic.Stage.TileContents Contents => contents;

	public bool IsEmpty => contents == null;

	public AvatarFarmOnline.Logic.Stage.FarmData FarmData => farmData;

	public int HackCheck
	{
		get
		{
			int num = tile.X * 7777 + tile.Y * 3333;
			if (contents != null)
			{
				num += contents.HackCheck;
			}
			return num;
		}
	}

	public event Action<AvatarFarmOnline.Logic.Stage.FarmTile, AvatarFarmOnline.Logic.Stage.TileContents> OnChangeContents;

	public FarmTile(AvatarFarmOnline.Logic.Stage.FarmData farmData, Int2 tile)
	{
		this.farmData = farmData;
		this.tile = tile;
	}

	public void Tick(int ticksPassed)
	{
		if (contents != null)
		{
			contents.Tick(ticksPassed);
		}
	}

	public bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, int rotation, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		lock (farmData)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
			if (contents == null)
			{
				switch (workType)
				{
				default:
					return false;
				case AvatarFarmOnline.Logic.WorkType.Plow:
					return farmData.TryPlow(player, this, out failReason);
				case AvatarFarmOnline.Logic.WorkType.PlantTree:
					return farmData.TryPlantTree(player, workItemDefinition as AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition, this, out failReason);
				case AvatarFarmOnline.Logic.WorkType.PlantAnimal:
					return farmData.TryPlantAnimal(player, workItemDefinition as AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition, this, out failReason);
				case AvatarFarmOnline.Logic.WorkType.Build:
					return farmData.TryPlaceBuilding(player, workItemDefinition as AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition, Tile, rotation, out failReason);
				case AvatarFarmOnline.Logic.WorkType.Plant:
					failReason = AvatarFarmOnline.Logic.FailedActionReason.LandNotPlowed;
					return false;
				}
			}
			return contents.Work(player, workType, workItemDefinition, out failReason);
		}
	}

	public bool Plant(AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition plant, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanPlant(plant, out failReason))
		{
			ClearContents();
			SetContent(new AvatarFarmOnline.Logic.Stage.Contents.Plant(plant, this));
			return true;
		}
		return false;
	}

	public bool CanPlant(AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition plant, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (contents != null && contents.Type == TileContentsType.Land && ((AvatarFarmOnline.Logic.Stage.Contents.Land)contents).State == AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
			return true;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.LandNotPlowed;
		return false;
	}

	public bool CanPlantTree(AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition tree, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		if (contents != null)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.LandNotEmpty;
			return false;
		}
		return true;
	}

	public bool CanPlantAnimal(AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition tree, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		if (contents != null)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.LandNotEmpty;
			return false;
		}
		return true;
	}

	public bool PlantTree(AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition tree, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanPlantTree(tree, out failReason))
		{
			SetContent(new AvatarFarmOnline.Logic.Stage.Contents.Tree(tree, this));
			return true;
		}
		return false;
	}

	public bool PlantAnimal(AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition animal, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanPlantAnimal(animal, out failReason))
		{
			SetContent(new AvatarFarmOnline.Logic.Stage.Contents.Animal(animal, this));
			return true;
		}
		return false;
	}

	public void SetContent(AvatarFarmOnline.Logic.Stage.TileContents content)
	{
		contents = content;
		if (OnChangeContents != null)
		{
			OnChangeContents(this, contents);
		}
		farmData.TileChangedContents(this, contents);
	}

	public void ClearContents()
	{
		contents = null;
		if (OnChangeContents != null)
		{
			OnChangeContents(this, contents);
		}
		farmData.TileChangedContents(this, contents);
	}

	public void ToXml(XElement xe)
	{
		xe.SetInt2Attribute("tile", tile);
		if (contents != null)
		{
			XElement xElement = new XElement("Contents");
			contents.ToXml(xElement);
			xe.Add(xElement);
		}
	}

	public static AvatarFarmOnline.Logic.Stage.FarmTile FromXml(AvatarFarmOnline.Logic.Stage.FarmData farmData, XElement xe)
	{
		AvatarFarmOnline.Logic.Stage.FarmTile farmTile = new AvatarFarmOnline.Logic.Stage.FarmTile(farmData, xe.ParseInt2Attribute("tile"));
		XElement xElement = xe.Element("Contents");
		if (xElement != null)
		{
			AvatarFarmOnline.Logic.Stage.TileContents tileContents = AvatarFarmOnline.Logic.Stage.TileContents.FromXml(farmTile, xElement);
			if (tileContents != null)
			{
				farmTile.contents = tileContents;
			}
		}
		return farmTile;
	}
}
