using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Buildings;

internal abstract class BaseBuilding
{
	protected AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition definition;

	protected Int2 topLeftTile;

	protected int rotation;

	private Vector2 centerPosition;

	protected int id;

	protected AvatarFarmOnline.Logic.Stage.FarmData farmData;

	private Body buildingBody;

	private Fixture buildingFixture;

	public AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition Definition => definition;

	public Int2 TopLeftTile => topLeftTile;

	public int Rotation => rotation;

	public Vector2 CenterPosition => centerPosition;

	public int Id => id;

	public BaseBuilding(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition definition, Int2 topLeftTile, int rotation, int id)
	{
		this.definition = definition;
		this.rotation = rotation;
		this.topLeftTile = topLeftTile;
		UpdateCenter();
		this.id = id;
		if (stage != null)
		{
			farmData = stage.FarmData;
			InitPhysics(stage.World, collide: false);
		}
	}

	public void SetStage(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		InitPhysics(stage.World, collide: true);
		if (farmData == null)
		{
			farmData = stage.FarmData;
		}
	}

	public BaseBuilding(AvatarFarmOnline.Logic.Stage.FarmData farmData)
	{
		this.farmData = farmData;
	}

	private void InitPhysics(World world, bool collide)
	{
		if (definition.HasCollision)
		{
			buildingBody = BodyFactory.CreateBody(world, Vector2.Zero);
			buildingBody.IgnoreGravity = true;
			buildingBody.BodyType = BodyType.Static;
			buildingBody.SleepingAllowed = false;
			int num = ((rotation % 2 == 0) ? definition.Size.X : definition.Size.Y);
			int num2 = ((rotation % 2 == 0) ? definition.Size.Y : definition.Size.X);
			buildingFixture = FixtureFactory.CreateRectangle((float)num * 2f - 0.44f, (float)num2 * 2f - 0.44f, 1f, Vector2.Zero, buildingBody);
			buildingFixture.UserData = this;
			if (!collide)
			{
				buildingFixture.IsSensor = true;
				Fixture fixture = buildingFixture;
				fixture.OnSeparation = (OnSeparationEventHandler)Delegate.Combine(fixture.OnSeparation, new OnSeparationEventHandler(OnSeparation));
			}
			buildingBody.Position = centerPosition;
		}
	}

	private void OnSeparation(Fixture f1, Fixture f2)
	{
		buildingFixture.IsSensor = false;
		Fixture fixture = buildingFixture;
		fixture.OnSeparation = (OnSeparationEventHandler)Delegate.Remove(fixture.OnSeparation, new OnSeparationEventHandler(OnSeparation));
	}

	private void UpdateCenter()
	{
		int num = ((rotation % 2 == 0) ? definition.Size.X : definition.Size.Y);
		int num2 = ((rotation % 2 == 0) ? definition.Size.Y : definition.Size.X);
		int num3 = num;
		int num4 = num2;
		if (rotation == 1 || rotation == 2)
		{
			num3 = 1 - (num3 - 1);
		}
		if (rotation == 2 || rotation == 3)
		{
			num4 = 1 - (num4 - 1);
		}
		centerPosition = new Vector2(((float)topLeftTile.X + (float)num3 * 0.5f) * 2f, ((float)topLeftTile.Y + (float)num4 * 0.5f) * 2f);
	}

	public virtual void ToXml(XElement xe)
	{
		xe.SetIntAttribute("item_id", id);
		xe.SetInt2Attribute("tile", topLeftTile);
		xe.SetIntAttribute("rotation", rotation);
		xe.SetAttribute("id", definition.Id);
	}

	public virtual void Recycle(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (buildingBody != null)
		{
			buildingBody.Dispose();
			buildingBody = null;
		}
	}

	public void Dispose()
	{
		if (buildingBody != null)
		{
			buildingBody.Dispose();
			buildingBody = null;
		}
	}

	public virtual void FromXml(XElement xe)
	{
		topLeftTile = xe.ParseInt2Attribute("tile");
		id = xe.ParseIntAttribute("item_id");
		rotation = xe.ParseIntAttribute("rotation", 0);
		UpdateCenter();
	}

	public virtual void SendData(IPacketWriter writer)
	{
		writer.Write(TopLeftTile);
		writer.Write(Rotation);
		writer.Write(Id);
	}

	protected virtual void ReadData(IPacketReader reader)
	{
		topLeftTile = reader.ReadInt2();
		rotation = reader.ReadInt32();
		id = reader.ReadInt32();
		UpdateCenter();
	}
}
