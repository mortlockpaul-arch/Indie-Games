using System;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;
using Quasar.Xml;

namespace AvatarFarmOnline.Items.Game;

internal class BuildingItem : RenderItem
{
	private AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding baseBuilding;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Vector3 buildingPos;

	private BorderedRectangle shadow;

	private float shadowDistance;

	private float currentScale;

	private float progress;

	private float wobble;

	private long buildTime;

	private static Vector2 lightSource;

	public AvatarFarmOnline.Logic.Stage.Buildings.Building Building => baseBuilding as AvatarFarmOnline.Logic.Stage.Buildings.Building;

	public AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding BaseBuilding => baseBuilding;

	public Vector3 CenterPosition => buildingPos;

	public static void SetLightSource(Vector2 position)
	{
		lightSource = position;
	}

	public BuildingItem(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding b)
	{
		DebugColor = Color.MediumTurquoise;
		this.stage = stage;
		int rotation = b.Rotation;
		if (rotation % 2 != 0)
		{
			_ = b.Definition.Size.Y;
		}
		else
		{
			_ = b.Definition.Size.X;
		}
		if (rotation % 2 != 0)
		{
			_ = b.Definition.Size.X;
		}
		else
		{
			_ = b.Definition.Size.Y;
		}
		Vector3 vector = AvatarFarmOnline.Logic.GameGlobals.WorldPosition(b.CenterPosition);
		buildingPos = vector;
		baseBuilding = b;
		switch (b.Definition.Category)
		{
		default:
			ModelLoader.LoadModelDefinition("Buildings/" + b.Definition.Id + "Model", this);
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
			ModelLoader.LoadModelDefinition("Decorations/" + b.Definition.Id + "Model", this);
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
			ModelLoader.LoadModelDefinition("Tools/" + b.Definition.Id + "Model", this);
			break;
		}
		Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI - (float)rotation * ((float)Math.PI / 2f));
		Transform.Translation = buildingPos;
		buildTime = Timer.DefaultTimer.TotalTime;
		wobble = 1f;
		if (baseBuilding.Definition.HasShadow)
		{
			shadow = new BorderedRectangle(TextureManager.Textures["BuildingShadow"], b.Definition.Size * 1f * 2f, new float[4] { 1f, 1f, 1f, 1f }, new float[4] { 32f, 32f, 32f, 32f });
			shadow.Shader = ShaderManager.Shaders["SimpleNoZWrite"];
			shadow.UseXZCoords = true;
			Vector2 vector2 = GameMath.RotateVector(new Vector2(0.4f, 0.4f), -(float)Math.PI + (float)rotation * ((float)Math.PI / 2f));
			shadow.Offset = new Vector2(vector2.Y, vector2.X);
			shadow.ZValue = 0.01f;
			addMesh(shadow);
			shadowDistance = 0.4f * (float)(b.Definition.Size.X + b.Definition.Size.Y) * 0.5f;
		}
	}

	protected override void DoUpdate()
	{
		if (shadow != null)
		{
			Vector2 v = new Vector2(buildingPos.Z - lightSource.Y, buildingPos.X - lightSource.X);
			Vector2 vector = GameMath.RotateVector(GameMath.VectorSetLength(v, shadowDistance), (float)baseBuilding.Rotation * ((float)Math.PI / 2f) - (float)Math.PI);
			shadow.Offset = new Vector2(vector.Y, vector.X);
		}
		if (stage.LocalPlayer.CameraState != AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World)
		{
			Vector3 value = stage.LocalPlayer.WorldPosition;
			Vector3.Distance(ref value, ref buildingPos, out var result);
			float num = result / 30f;
			Alpha = GameMath.Interpolate(1f, 0f, GameMath.Clamp(0f, 1f, (num - 0.9f) * 10f));
			if (shadow != null)
			{
				shadow.Alpha = Alpha;
			}
		}
		else
		{
			Alpha = 1f;
			if (shadow != null)
			{
				shadow.Alpha = 1f;
			}
		}
		if (baseBuilding is AvatarFarmOnline.Logic.Stage.Buildings.Building)
		{
			AvatarFarmOnline.Logic.Stage.Buildings.Building building = Building;
			float num2 = 0f;
			if (building.Definition.CanAccumulateItems)
			{
				num2 = (float)building.AccumulatedItems / (float)building.Definition.ItemAccumulationCount;
			}
			else if (building.Definition.CanBeGathered)
			{
				num2 = building.GrowthProgress;
			}
			if (num2 >= 1f && num2 > progress)
			{
				wobble = 1f;
			}
			progress = num2;
		}
		if (currentScale != 1f)
		{
			long num3 = Timer.DefaultTimer.TimeSince(buildTime);
			if (num3 < 800)
			{
				currentScale = GameMath.Damping(currentScale, 1f, 0.85f);
			}
			else
			{
				currentScale = 1f;
			}
		}
		float num4 = currentScale;
		if (wobble > 0.001f)
		{
			float num5 = 1f + wobble * (float)Math.Sin(stage.Timer.TotalTimeSeconds * 8f) * 0.75f;
			float num6 = 1f + wobble * (float)Math.Cos(stage.Timer.TotalTimeSeconds * 8f) * 0.4f;
			Transform.Scale = new Vector3(num6 * currentScale, num5 * currentScale, num6 * currentScale);
			wobble = GameMath.Interpolate(wobble, 0f, 0.075f);
		}
		else if (Transform.Scale.X != num4)
		{
			Transform.Scale = new Vector3(currentScale);
		}
		base.DoUpdate();
	}
}
