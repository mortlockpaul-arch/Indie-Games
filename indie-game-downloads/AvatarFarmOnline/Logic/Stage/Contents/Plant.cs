using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class Plant : AvatarFarmOnline.Logic.Stage.TileContents
{
	public enum PlantState
	{
		Growing,
		ReadyToGather,
		Withered
	}

	private PlantState state;

	private AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition definition;

	private int growthPoints;

	private int gatherEnd;

	private int waterLevel;

	public PlantState State => state;

	public AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition Definition => definition;

	public int GrowthPoints => growthPoints;

	public float GrowthProgress
	{
		get
		{
			if (state != PlantState.Growing)
			{
				return 1f;
			}
			return (float)growthPoints / (float)definition.GrowTime;
		}
	}

	public int RemainingTime
	{
		get
		{
			if (state != PlantState.Growing)
			{
				return 0;
			}
			int num = definition.GrowTime - growthPoints;
			return num - Math.Min(waterLevel, num / 2);
		}
	}

	public int DeathRemainingTime => state switch
	{
		PlantState.Growing => RemainingTime + Math.Max((int)((float)definition.GatherTime * 2f), 86400), 
		PlantState.ReadyToGather => gatherEnd - farmTile.FarmData.WorldTicks, 
		_ => 0, 
	};

	public int GatherEnd => gatherEnd;

	public float GatherProgress
	{
		get
		{
			if (state != PlantState.ReadyToGather)
			{
				return 1f;
			}
			return 1f - (float)(gatherEnd - farmTile.FarmData.WorldTicks) / (float)definition.GatherTime;
		}
	}

	public int WaterLevel => waterLevel;

	public float WaterProgress
	{
		get
		{
			if (state != PlantState.Growing)
			{
				return 1f;
			}
			return (float)waterLevel / (float)definition.MaxWaterLevel;
		}
	}

	public bool Watered => waterLevel > 0;

	public override int HackCheck => 20 + (int)state * 5 + definition.Id.GetHashCode() / 3 + growthPoints * 20 + gatherEnd * 5 + waterLevel * 3;

	public Plant(AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Plant)
	{
	}

	public Plant(AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition definition, AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Plant)
	{
		this.definition = definition;
		state = PlantState.Growing;
		growthPoints = 0;
		waterLevel = 0;
	}

	public override void Tick(int ticksPassed)
	{
		switch (state)
		{
		case PlantState.Growing:
		{
			int num = GameMath.Clamp(0, ticksPassed, waterLevel);
			waterLevel = Math.Max(0, waterLevel - ticksPassed);
			int val = ticksPassed - num;
			int num2 = definition.GrowTime - growthPoints;
			num = Math.Min(num, (num2 + 1) / 2);
			num2 -= num * 2;
			val = Math.Min(val, num2);
			int num3 = farmTile.FarmData.WorldTicks + num + val;
			growthPoints += num * 2 + val;
			if (growthPoints >= definition.GrowTime)
			{
				state = PlantState.ReadyToGather;
				gatherEnd = num3 + Math.Max(86400, (int)((float)definition.GatherTime * 2f));
				goto case PlantState.ReadyToGather;
			}
			break;
		}
		case PlantState.ReadyToGather:
			if (farmTile.FarmData.WorldTicks + ticksPassed >= gatherEnd)
			{
				state = PlantState.Withered;
			}
			break;
		}
	}

	public void Water()
	{
		waterLevel = definition.MaxWaterLevel;
	}

	public void Randomize()
	{
		waterLevel = GameMath.Random.Next(0, definition.MaxWaterLevel / 2);
		growthPoints = GameMath.Random.Next(definition.GrowTime / 2, definition.GrowTime * 3 / 4);
	}

	public void Gather(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (state == PlantState.ReadyToGather)
		{
			farmTile.FarmData.EarnXp(definition.GatherXp);
			player.EarnXp(definition.GatherXp);
			farmTile.FarmData.EarnMoney(definition.GatherMoney);
			farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, definition.GatherXp, definition.GatherMoney);
			farmTile.FarmData.ItemGathered(definition.PlantCategory);
		}
		else if (state == PlantState.Withered)
		{
			farmTile.FarmData.EarnMoney(definition.GatherWitheredMoney);
			farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, 0, definition.GatherWitheredMoney);
		}
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.Gather:
			if (state == PlantState.ReadyToGather || state == PlantState.Withered)
			{
				Gather(player);
				farmTile.ClearContents();
				farmTile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.Land(farmTile, AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered));
				return true;
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.PlantNotReady;
			break;
		case AvatarFarmOnline.Logic.WorkType.Plow:
			if (state == PlantState.Withered)
			{
				return farmTile.FarmData.TryPlow(player, farmTile, out failReason);
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.CantPlow;
			break;
		case AvatarFarmOnline.Logic.WorkType.Water:
			if (state == PlantState.Growing)
			{
				Water();
				return true;
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.CantWater;
			break;
		case AvatarFarmOnline.Logic.WorkType.Recycle:
			if (state == PlantState.Withered)
			{
				return farmTile.FarmData.TryPlow(player, farmTile, out failReason);
			}
			farmTile.FarmData.EarnMoney(definition.RecycleMoney);
			farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, 0, definition.RecycleMoney);
			farmTile.ClearContents();
			farmTile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.Land(farmTile, AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered));
			return true;
		}
		return false;
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
		xe.SetIntAttribute("state", (int)state);
		xe.SetIntAttribute("gatherEnd", gatherEnd);
		xe.SetIntAttribute("growthPoints", growthPoints);
		xe.SetIntAttribute("waterLevel", waterLevel);
		xe.SetAttribute("definition", definition.Id);
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		state = (PlantState)xe.ParseIntAttribute("state");
		growthPoints = xe.ParseIntAttribute("growthPoints");
		gatherEnd = xe.ParseIntAttribute("gatherEnd");
		waterLevel = xe.ParseIntAttribute("waterLevel");
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetPlant(xe.GetAttribute("definition"));
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write(Definition.Id);
		writer.Write((short)State);
		writer.Write(GrowthPoints);
		writer.Write(GatherEnd);
		writer.Write(WaterLevel);
	}

	public override void ReceiveData(IPacketReader packetReader)
	{
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetPlant(packetReader.ReadString());
		state = (PlantState)packetReader.ReadInt16();
		growthPoints = packetReader.ReadInt32();
		gatherEnd = packetReader.ReadInt32();
		waterLevel = packetReader.ReadInt32();
	}
}
