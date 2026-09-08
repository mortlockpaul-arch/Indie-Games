using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class Tree : AvatarFarmOnline.Logic.Stage.TileContents
{
	public enum TreeState
	{
		ReadyToGather,
		WaitingSeason
	}

	private TreeState state = TreeState.WaitingSeason;

	private AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition definition;

	public TreeState State => state;

	public AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition Definition => definition;

	public override int HackCheck => 10 + (int)state * 5 + definition.Id.GetHashCode() / 3;

	public int TimeUntilNextFruit
	{
		get
		{
			int num = farmTile.FarmData.NextSeasonChange - farmTile.FarmData.WorldTicks;
			AvatarFarmOnline.Logic.Seasons seasons = AvatarFarmOnline.Logic.Parsing.NextSeason(farmTile.FarmData.CurrentSeason);
			while ((seasons & definition.GatherSeasons) == 0 && seasons != farmTile.FarmData.CurrentSeason)
			{
				num += 900;
				seasons = AvatarFarmOnline.Logic.Parsing.NextSeason(seasons);
			}
			return num;
		}
	}

	public Tree(AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Tree)
	{
	}

	public Tree(AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition definition, AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Tree)
	{
		this.definition = definition;
		state = TreeState.WaitingSeason;
	}

	public override void Tick(int ticksPassed)
	{
		if (farmTile.FarmData.StartedSeasons.Count > 0)
		{
			if ((farmTile.FarmData.StartedSeasons[farmTile.FarmData.StartedSeasons.Count - 1].Key & definition.GatherSeasons) != AvatarFarmOnline.Logic.Seasons.None)
			{
				state = TreeState.ReadyToGather;
			}
			else
			{
				state = TreeState.WaitingSeason;
			}
		}
	}

	public void Gather(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (state == TreeState.ReadyToGather)
		{
			farmTile.FarmData.EarnXp(definition.GatherXp);
			player.EarnXp(definition.GatherXp);
			farmTile.FarmData.EarnMoney(definition.GatherMoney);
			farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, definition.GatherXp, definition.GatherMoney);
			farmTile.FarmData.ItemGathered(definition.PlantCategory);
			state = TreeState.WaitingSeason;
		}
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.GatherTree:
			if (state == TreeState.ReadyToGather)
			{
				Gather(player);
				return true;
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.TreeNotReady;
			break;
		case AvatarFarmOnline.Logic.WorkType.Recycle:
			farmTile.ClearContents();
			farmTile.FarmData.EarnMoney(definition.RecycleMoney);
			farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, 0, definition.RecycleMoney);
			return true;
		}
		return false;
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
		xe.SetIntAttribute("state", (int)state);
		xe.SetAttribute("definition", definition.Id);
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		state = (TreeState)xe.ParseIntAttribute("state");
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetTree(xe.GetAttribute("definition"));
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write(Definition.Id);
		writer.Write((short)State);
	}

	public override void ReceiveData(IPacketReader reader)
	{
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetTree(reader.ReadString());
		state = (TreeState)reader.ReadInt16();
	}
}
