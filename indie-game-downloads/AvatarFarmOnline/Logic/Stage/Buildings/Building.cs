using System.Collections.Generic;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Buildings;

internal class Building : AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding
{
	public enum BuildingGatherState
	{
		NotGatherable,
		ReadyToGather,
		Waiting
	}

	public enum BuildingAccumulationState
	{
		NotAccumulable,
		ReadyToGather,
		Accumulating
	}

	private BuildingGatherState gatherState;

	private int nextGather;

	private BuildingAccumulationState accumulationState;

	private int accumulatedItems;

	private FastRandom gambleRandom;

	public new AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition Definition => (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)definition;

	public BuildingGatherState GatherState => gatherState;

	public int NextGather => nextGather;

	public float GrowthProgress
	{
		get
		{
			if (gatherState != BuildingGatherState.Waiting)
			{
				return 1f;
			}
			return 1f - (float)(nextGather - farmData.WorldTicks) / (float)Definition.GatherPeriod;
		}
	}

	public BuildingAccumulationState AccumulationState => accumulationState;

	public int AccumulatedItems => accumulatedItems;

	public int RemainingTime
	{
		get
		{
			if (gatherState != BuildingGatherState.Waiting)
			{
				return 0;
			}
			return nextGather - farmData.WorldTicks;
		}
	}

	public bool IsGamble => Definition.CanGamble;

	public int HackCheck => (int)gatherState * 10 + nextGather * 5 + (int)accumulationState * 10 + accumulatedItems * 5 + Definition.Id.GetHashCode() / 3;

	public bool CanBeGathered(out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (Definition.CanGamble)
		{
			if (!farmData.PlayerData.CanBuy(Definition.GamblePrice))
			{
				failReason = AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney;
				return false;
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
			return true;
		}
		if (gatherState != BuildingGatherState.ReadyToGather && accumulationState != BuildingAccumulationState.ReadyToGather)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.BuildingNotReady;
			return false;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return true;
	}

	public Building(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition definition, Int2 topLeftTile, int rotation, int id)
		: base(stage, definition, topLeftTile, rotation, id)
	{
		if (definition.CanBeGathered)
		{
			if (farmData != null)
			{
				nextGather = farmData.WorldTicks + definition.GatherPeriod;
			}
			else
			{
				nextGather = definition.GatherPeriod;
			}
			gatherState = BuildingGatherState.Waiting;
		}
		if (definition.CanAccumulateItems)
		{
			accumulationState = BuildingAccumulationState.Accumulating;
		}
		if (definition.CanGamble)
		{
			gambleRandom = new FastRandom(farmData.WorldTicks);
		}
	}

	public Building(AvatarFarmOnline.Logic.Stage.FarmData farmData)
		: base(farmData)
	{
	}

	public void Gather(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (gatherState == BuildingGatherState.ReadyToGather)
		{
			farmData.EarnMoney(Definition.GatherAmount);
			farmData.EarnXp(Definition.GatherXp);
			player.EarnXp(Definition.GatherXp);
			gatherState = BuildingGatherState.Waiting;
			nextGather = farmData.WorldTicks + Definition.GatherPeriod;
			farmData.ActionPerformed(base.CenterPosition, Definition.GatherXp, Definition.GatherAmount);
		}
		if (accumulationState == BuildingAccumulationState.ReadyToGather)
		{
			farmData.EarnMoney(Definition.ItemAccumulationAmount);
			farmData.EarnXp(Definition.ItemAccumulationXp);
			player.EarnXp(Definition.ItemAccumulationXp);
			accumulationState = BuildingAccumulationState.Accumulating;
			accumulatedItems = 0;
			farmData.ActionPerformed(base.CenterPosition, Definition.ItemAccumulationXp, Definition.ItemAccumulationAmount);
		}
		if (!Definition.CanGamble || !farmData.PlayerData.TryBuy(Definition.GamblePrice))
		{
			return;
		}
		int num = gambleRandom.Next(100);
		int num2 = 0;
		List<KeyValuePair<int, AvatarFarmOnline.Logic.Money>> gamblePrizes = Definition.GamblePrizes;
		bool flag = false;
		for (int i = 0; i < gamblePrizes.Count; i++)
		{
			num2 += gamblePrizes[i].Key;
			if (num < num2)
			{
				AvatarFarmOnline.Logic.Money money = gamblePrizes[i].Value;
				farmData.EarnMoney(money);
				if (money.Type == Definition.GamblePrice.Type)
				{
					money = new AvatarFarmOnline.Logic.Money(money.Type, money.Amount - Definition.GamblePrice.Amount);
				}
				farmData.ActionPerformed(base.CenterPosition, 0, money);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			farmData.ActionPerformed(base.CenterPosition, 0, -Definition.GamblePrice);
		}
	}

	public bool ItemGathered(string category)
	{
		if (!Definition.CanAccumulateItems)
		{
			return false;
		}
		if (Definition.ItemAccumulationCategory != category)
		{
			return false;
		}
		if (accumulatedItems >= Definition.ItemAccumulationCount)
		{
			return false;
		}
		accumulatedItems++;
		if (accumulatedItems >= Definition.ItemAccumulationCount)
		{
			accumulationState = BuildingAccumulationState.ReadyToGather;
		}
		return true;
	}

	public void Tick(int ticksPassed)
	{
		if (!Definition.CanBeGathered)
		{
			return;
		}
		switch (gatherState)
		{
		case BuildingGatherState.Waiting:
			if (farmData.WorldTicks + ticksPassed >= nextGather)
			{
				gatherState = BuildingGatherState.ReadyToGather;
			}
			break;
		case BuildingGatherState.ReadyToGather:
			break;
		}
	}

	public override void Recycle(AvatarFarmOnline.Logic.Stage.Player player)
	{
		farmData.EarnMoney(Definition.RecycleMoney);
		farmData.ActionPerformed(base.CenterPosition, 0, definition.RecycleMoney);
		farmData.RemoveBuilding(player, this);
		base.Recycle(player);
	}

	public override void ToXml(XElement xe)
	{
		xe.SetIntAttribute("next_gather", nextGather);
		xe.SetIntAttribute("gather_state", (int)gatherState);
		xe.SetIntAttribute("accumulation_state", (int)accumulationState);
		xe.SetIntAttribute("accumulated_items", accumulatedItems);
		if (Definition.CanGamble)
		{
			xe.SetLongAttribute("gamble_seed", gambleRandom.CurrentSeed);
		}
		base.ToXml(xe);
	}

	public override void FromXml(XElement xe)
	{
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetBuilding(xe.GetAttribute("id"));
		nextGather = xe.ParseIntAttribute("next_gather");
		gatherState = (BuildingGatherState)xe.ParseIntAttribute("gather_state");
		accumulationState = (BuildingAccumulationState)xe.ParseIntAttribute("accumulation_state");
		accumulatedItems = xe.ParseIntAttribute("accumulated_items");
		if (Definition.CanGamble)
		{
			long seed = xe.ParseLongAttribute("gamble_seed");
			gambleRandom = new FastRandom(seed);
		}
		base.FromXml(xe);
	}

	public static AvatarFarmOnline.Logic.Stage.Buildings.Building FromXml(AvatarFarmOnline.Logic.Stage.FarmData farmData, XElement xe)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Building building = new AvatarFarmOnline.Logic.Stage.Buildings.Building(farmData);
		building.FromXml(xe);
		return building;
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write(definition.Id);
		base.SendData(writer);
		writer.Write((short)gatherState);
		writer.Write(nextGather);
		writer.Write(accumulatedItems);
		if (Definition.CanGamble)
		{
			writer.Write(gambleRandom.CurrentSeed);
		}
	}

	public static AvatarFarmOnline.Logic.Stage.Buildings.Building ReceiveData(AvatarFarmOnline.Logic.Stage.FarmData farmData, IPacketReader reader)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Building building = new AvatarFarmOnline.Logic.Stage.Buildings.Building(farmData);
		building.ReadData(reader);
		return building;
	}

	protected override void ReadData(IPacketReader reader)
	{
		string name = reader.ReadString();
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetBuilding(name);
		base.ReadData(reader);
		gatherState = (BuildingGatherState)reader.ReadInt16();
		nextGather = reader.ReadInt32();
		accumulatedItems = reader.ReadInt32();
		if (Definition.CanGamble)
		{
			gambleRandom = new FastRandom(reader.ReadInt64());
		}
	}
}
