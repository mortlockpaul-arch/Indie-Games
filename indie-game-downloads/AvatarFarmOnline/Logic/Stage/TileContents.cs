using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal abstract class TileContents
{
	private TileContentsType type;

	protected AvatarFarmOnline.Logic.Stage.FarmTile farmTile;

	public TileContentsType Type => type;

	public abstract int HackCheck { get; }

	public abstract void Tick(int ticksPassed);

	public TileContents(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, TileContentsType type)
	{
		this.farmTile = farmTile;
		this.type = type;
	}

	public abstract bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason);

	public virtual void ToXml(XElement xe)
	{
		xe.SetIntAttribute("type", (int)type);
	}

	public virtual void FromXml(XElement xe)
	{
	}

	public static AvatarFarmOnline.Logic.Stage.TileContents FromXml(AvatarFarmOnline.Logic.Stage.FarmTile tile, XElement xe)
	{
		AvatarFarmOnline.Logic.Stage.TileContents tileContents = null;
		try
		{
			switch ((TileContentsType)xe.ParseIntAttribute("type"))
			{
			case TileContentsType.Land:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.Land(tile);
				break;
			case TileContentsType.Plant:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.Plant(tile);
				break;
			case TileContentsType.Building:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.BuildingTile(tile);
				break;
			case TileContentsType.Decorative:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.DecorationTile(tile);
				break;
			case TileContentsType.Tree:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.Tree(tile);
				break;
			case TileContentsType.Tool:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.ToolTile(tile);
				break;
			case TileContentsType.Animal:
				tileContents = new AvatarFarmOnline.Logic.Stage.Contents.Animal(tile);
				break;
			}
			tileContents?.FromXml(xe);
		}
		catch (Exception)
		{
			tileContents = null;
		}
		return tileContents;
	}

	public virtual void SendData(IPacketWriter writer)
	{
	}

	public virtual void ReceiveData(IPacketReader reader)
	{
	}
}
