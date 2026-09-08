using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class Land : AvatarFarmOnline.Logic.Stage.TileContents
{
	public enum LandState
	{
		Plowed,
		Gathered
	}

	private LandState state;

	public override int HackCheck => (int)state * 5;

	public LandState State => state;

	public Land(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, LandState state)
		: base(farmTile, TileContentsType.Land)
	{
		this.state = state;
	}

	public Land(AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Land)
	{
	}

	public override void Tick(int ticksPassed)
	{
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.Plow:
			if (state == LandState.Gathered)
			{
				return farmTile.FarmData.TryPlow(player, farmTile, out failReason);
			}
			break;
		case AvatarFarmOnline.Logic.WorkType.Recycle:
			farmTile.ClearContents();
			if (state == LandState.Plowed)
			{
				farmTile.FarmData.EarnMoney(AvatarFarmOnline.Logic.GameGlobals.RecyclePlowedMoney);
				farmTile.FarmData.ActionPerformed((farmTile.Tile + new Vector2(0.5f)) * 2f, 0, AvatarFarmOnline.Logic.GameGlobals.RecyclePlowedMoney);
			}
			return true;
		case AvatarFarmOnline.Logic.WorkType.Plant:
			if (state == LandState.Plowed)
			{
				return farmTile.FarmData.TryPlant(player, workItemDefinition as AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition, farmTile, out failReason);
			}
			failReason = AvatarFarmOnline.Logic.FailedActionReason.LandNotPlowed;
			return false;
		}
		return false;
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
		xe.SetIntAttribute("state", (int)state);
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		state = (LandState)xe.ParseIntAttribute("state");
	}

	public override void SendData(IPacketWriter writer)
	{
		writer.Write((short)State);
	}

	public override void ReceiveData(IPacketReader reader)
	{
		state = (LandState)reader.ReadInt16();
	}
}
