using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Buildings;

internal class Tool : AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding
{
	private int remainingFuel;

	public new AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition Definition => (AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)definition;

	public int RemainingFuel => remainingFuel;

	public float FuelProgress => (float)remainingFuel / (float)Definition.MaxFuel;

	public int HackCheck => remainingFuel * 4 + Definition.Id.GetHashCode() / 3;

	public void UseFuel()
	{
		remainingFuel = Math.Max(0, remainingFuel - 1);
	}

	public bool CanBeRefilled(out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (remainingFuel >= Definition.MaxFuel)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.ToolIsFull;
			return false;
		}
		if (!farmData.PlayerData.CanBuy(Definition.RefillPrice))
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney;
			return false;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return true;
	}

	public Tool(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition definition, Int2 topLeftTile, int rotation, int id)
		: base(stage, definition, topLeftTile, rotation, id)
	{
		remainingFuel = definition.MaxFuel;
	}

	public Tool(AvatarFarmOnline.Logic.Stage.FarmData farmData)
		: base(farmData)
	{
	}

	public void Refill(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (CanBeRefilled(out var _) && farmData.PlayerData.TryBuy(Definition.RefillPrice))
		{
			farmData.EarnXp(Definition.RefillXp);
			player.EarnXp(Definition.RefillXp);
			remainingFuel = Definition.MaxFuel;
			farmData.ActionPerformed(base.CenterPosition, Definition.RefillXp, -Definition.RefillPrice);
		}
	}

	public void Tick(int ticksPassed)
	{
	}

	public override void Recycle(AvatarFarmOnline.Logic.Stage.Player player)
	{
		farmData.EarnMoney(Definition.RecycleMoney);
		farmData.ActionPerformed(base.CenterPosition, 0, definition.RecycleMoney);
		farmData.RemoveTool(player, this);
		base.Recycle(player);
	}

	public override void ToXml(XElement xe)
	{
		xe.SetIntAttribute("remaining_fuel", remainingFuel);
		base.ToXml(xe);
	}

	public override void FromXml(XElement xe)
	{
		string attribute = xe.GetAttribute("id");
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetTool(attribute);
		remainingFuel = xe.ParseIntAttribute("remaining_fuel");
		base.FromXml(xe);
	}

	public static AvatarFarmOnline.Logic.Stage.Buildings.Tool FromXml(AvatarFarmOnline.Logic.Stage.FarmData farmData, XElement xe)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = new AvatarFarmOnline.Logic.Stage.Buildings.Tool(farmData);
		tool.FromXml(xe);
		return tool;
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write(definition.Id);
		base.SendData(writer);
		writer.Write(remainingFuel);
	}

	public static AvatarFarmOnline.Logic.Stage.Buildings.Tool ReceiveData(AvatarFarmOnline.Logic.Stage.FarmData farmData, IPacketReader reader)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = new AvatarFarmOnline.Logic.Stage.Buildings.Tool(farmData);
		tool.ReadData(reader);
		return tool;
	}

	protected override void ReadData(IPacketReader reader)
	{
		string name = reader.ReadString();
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetTool(name);
		base.ReadData(reader);
		remainingFuel = reader.ReadInt32();
	}
}
