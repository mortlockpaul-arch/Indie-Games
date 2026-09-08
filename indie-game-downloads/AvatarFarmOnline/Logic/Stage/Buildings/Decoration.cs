using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Buildings;

internal class Decoration : AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding
{
	public new AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition Definition => (AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)definition;

	public int HackCheck => Definition.Id.GetHashCode() / 3;

	public Decoration(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition definition, Int2 topLeftTile, int rotation, int id)
		: base(stage, definition, topLeftTile, rotation, id)
	{
	}

	public Decoration(AvatarFarmOnline.Logic.Stage.FarmData farmData)
		: base(farmData)
	{
	}

	public void Tick()
	{
	}

	public override void Recycle(AvatarFarmOnline.Logic.Stage.Player player)
	{
		farmData.EarnMoney(Definition.RecycleMoney);
		farmData.ActionPerformed(base.CenterPosition, 0, definition.RecycleMoney);
		farmData.RemoveDecoration(player, this);
		base.Recycle(player);
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
	}

	public static AvatarFarmOnline.Logic.Stage.Buildings.Decoration FromXml(AvatarFarmOnline.Logic.Stage.FarmData farmData, XElement xe)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration = new AvatarFarmOnline.Logic.Stage.Buildings.Decoration(farmData);
		string attribute = xe.GetAttribute("id");
		decoration.definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetDecoration(attribute);
		decoration.FromXml(xe);
		return decoration;
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write(definition.Id);
		base.SendData(writer);
	}

	public static AvatarFarmOnline.Logic.Stage.Buildings.Decoration ReceiveData(AvatarFarmOnline.Logic.Stage.FarmData farmData, IPacketReader reader)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration = new AvatarFarmOnline.Logic.Stage.Buildings.Decoration(farmData);
		decoration.ReadData(reader);
		return decoration;
	}

	protected override void ReadData(IPacketReader reader)
	{
		string name = reader.ReadString();
		definition = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetDecoration(name);
		base.ReadData(reader);
	}
}
