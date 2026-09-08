using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class Animal : AvatarFarmOnline.Logic.Stage.TileContents
{
	public enum AnimalState
	{
		NeedsFood,
		Growing,
		ReadyToGather
	}

	private AnimalState state = AnimalState.Growing;

	private AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition definition;

	private int timesFed;

	private int foodLevel;

	public AnimalState State => state;

	public AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition Definition => definition;

	public int TimesFed => timesFed;

	public float GrowthProgress
	{
		get
		{
			if (state == AnimalState.ReadyToGather)
			{
				return 1f;
			}
			if (state == AnimalState.NeedsFood)
			{
				return (float)(timesFed + 1) / ((float)definition.FeedAmount + 1f);
			}
			return (float)(definition.FeedInterval - foodLevel + timesFed * definition.FeedInterval) / (float)(definition.FeedInterval * (definition.FeedAmount + 1));
		}
	}

	public int RemainingTime
	{
		get
		{
			if (state != AnimalState.Growing)
			{
				return 0;
			}
			return foodLevel;
		}
	}

	public int FoodLevel => foodLevel;

	public float FoodProgress => (float)foodLevel / (float)definition.FeedInterval;

	public bool NeedsFood => state == AnimalState.NeedsFood;

	public override int HackCheck => 60 + (int)state * 5 + definition.Id.GetHashCode() / 3 + timesFed * 20 + foodLevel * 3;

	public Animal(AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Animal)
	{
	}

	public Animal(AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition definition, AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Animal)
	{
		this.definition = definition;
		state = AnimalState.Growing;
		timesFed = 0;
		foodLevel = definition.FeedInterval;
	}

	public override void Tick(int ticksPassed)
	{
		switch (state)
		{
		case AnimalState.Growing:
		{
			int num = GameMath.Clamp(0, ticksPassed, foodLevel);
			foodLevel = Math.Max(0, foodLevel - num);
			if (foodLevel == 0)
			{
				if (timesFed == definition.FeedAmount)
				{
					state = AnimalState.ReadyToGather;
				}
				else
				{
					state = AnimalState.NeedsFood;
				}
			}
			break;
		}
		case AnimalState.NeedsFood:
		case AnimalState.ReadyToGather:
			break;
		}
	}

	public bool CanBeFeed(out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (!NeedsFood)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.AnimalHasEnoughFood;
			return false;
		}
		if (!farmTile.FarmData.PlayerData.CanBuy(Definition.FeedMoney))
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney;
			return false;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return true;
	}

	public void Feed(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (CanBeFeed(out var _) && farmTile.FarmData.PlayerData.TryBuy(Definition.FeedMoney))
		{
			farmTile.FarmData.EarnXp(Definition.FeedXp);
			player.EarnXp(Definition.FeedXp);
			foodLevel = definition.FeedInterval;
			timesFed++;
			state = AnimalState.Growing;
			farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, definition.FeedXp, -Definition.FeedMoney);
		}
	}

	public void Gather(AvatarFarmOnline.Logic.Stage.Player player)
	{
		farmTile.FarmData.EarnXp(definition.GatherXp);
		player.EarnXp(definition.GatherXp);
		farmTile.FarmData.EarnMoney(definition.GatherMoney);
		farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, definition.GatherXp, definition.GatherMoney);
		state = AnimalState.Growing;
		foodLevel = definition.FeedInterval;
		timesFed = 0;
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.GatherAnimal:
			if (state == AnimalState.ReadyToGather)
			{
				Gather(player);
				return true;
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.AnimalNotReady;
			break;
		case AvatarFarmOnline.Logic.WorkType.FeedAnimal:
			if (CanBeFeed(out failReason))
			{
				Feed(player);
				return true;
			}
			return false;
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
		xe.SetIntAttribute("foodLevel", foodLevel);
		xe.SetIntAttribute("timesFed", timesFed);
		xe.SetAttribute("definition", definition.Id);
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		state = (AnimalState)xe.ParseIntAttribute("state");
		timesFed = xe.ParseIntAttribute("timesFed");
		foodLevel = xe.ParseIntAttribute("foodLevel");
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetAnimal(xe.GetAttribute("definition"));
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write(Definition.Id);
		writer.Write((short)State);
		writer.Write(timesFed);
		writer.Write(FoodLevel);
	}

	public override void ReceiveData(IPacketReader packetReader)
	{
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetAnimal(packetReader.ReadString());
		state = (AnimalState)packetReader.ReadInt16();
		timesFed = packetReader.ReadInt32();
		foodLevel = packetReader.ReadInt32();
	}
}
