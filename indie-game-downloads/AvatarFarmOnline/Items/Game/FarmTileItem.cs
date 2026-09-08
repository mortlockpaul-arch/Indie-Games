using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;
using Quasar.Xml;

namespace AvatarFarmOnline.Items.Game;

internal class FarmTileItem : RenderItem
{
	private AvatarFarmOnline.Logic.Stage.FarmTile farmTile;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Vector3 tilePos;

	private Int2 tile;

	private float viewDistance;

	private long plantTime;

	private float currentScale;

	private float progress;

	private float wobble;

	private Sized2DRectangleMesh shadow;

	private float shadowRotation;

	private static Vector2 lightSource;

	private static Dictionary<string, List<Mesh>> loadedMeshes = new Dictionary<string, List<Mesh>>();

	private static readonly Vector3 WitheredPlantColor = GameMath.RGBToVector(208, 126, 52);

	private static readonly Vector3 GrowingPlantColor = GameMath.RGBToVector(190, byte.MaxValue, 180);

	public Vector3 TilePosition => tilePos;

	public float ViewDistance => viewDistance;

	public static void SetLightSource(Vector2 position)
	{
		lightSource = position;
	}

	public static void LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory category, string id, RenderItem model)
	{
		string text = category switch
		{
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree => "Trees/", 
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration => "Decorations/", 
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal => "Animals/", 
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building => "Buildings/", 
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool => "Tools/", 
			_ => "Crops/", 
		} + id;
		if (!loadedMeshes.TryGetValue(text, out var value))
		{
			RenderItem renderItem = ModelLoader.LoadModelDefinition(text + "Model");
			value = new List<Mesh>(renderItem.Meshes);
			loadedMeshes.Add(text, value);
		}
		foreach (Mesh item in value)
		{
			Mesh mesh = item.Clone();
			if (mesh != null)
			{
				model.addMesh(mesh);
			}
		}
	}

	public FarmTileItem(AvatarFarmOnline.Logic.Stage.Stage stage, Int2 tilePos)
	{
		this.stage = stage;
		tile = tilePos;
		this.tilePos = AvatarFarmOnline.Logic.GameGlobals.TileWorldPosition(tilePos);
		Transform.Translation = new Vector3(((float)tilePos.X + 0.5f) * 2f, 0f, ((float)tilePos.Y + 0.5f) * 2f);
		tile_OnChangeContents(null, null);
		DebugColor = Color.Blue;
	}

	public FarmTileItem(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.FarmTile tile)
	{
		this.stage = stage;
		farmTile = tile;
		this.tile = tile.Tile;
		tilePos = AvatarFarmOnline.Logic.GameGlobals.TileWorldPosition(tile.Tile);
		Transform.Translation = new Vector3(((float)tile.Tile.X + 0.5f) * 2f, 0f, ((float)tile.Tile.Y + 0.5f) * 2f);
		tile.OnChangeContents += tile_OnChangeContents;
		tile_OnChangeContents(tile, tile.Contents);
		DebugColor = Color.Blue;
	}

	public void SetTile(AvatarFarmOnline.Logic.Stage.FarmTile tile)
	{
		if (tile == null)
		{
			farmTile = tile;
			if (tile != null)
			{
				tile.OnChangeContents += tile_OnChangeContents;
				tile_OnChangeContents(tile, tile.Contents);
			}
			else
			{
				tile_OnChangeContents(null, null);
			}
		}
	}

	private void tile_OnChangeContents(AvatarFarmOnline.Logic.Stage.FarmTile arg1, AvatarFarmOnline.Logic.Stage.TileContents arg2)
	{
		wobble = (progress = 0f);
		clearMeshes();
		Transform.Scale = new Vector3(1f);
		Vector3 ambient = (Diffuse = Vector3.One);
		Ambient = ambient;
		viewDistance = 0f;
		Transform.Rotation = Quaternion.Identity;
		if (arg2 != null)
		{
			switch (arg2.Type)
			{
			case TileContentsType.Tree:
				viewDistance = 15f;
				LoadMesh(((AvatarFarmOnline.Logic.Stage.Contents.Tree)arg2).Definition.Category, ((AvatarFarmOnline.Logic.Stage.Contents.Tree)arg2).Definition.Id, this);
				if (meshes.Count > 0 && Mesh.FirstMaterial.FloatParameterCount <= 0)
				{
					Mesh.FirstMaterial.AddFloatParameter(1f);
				}
				AddShadow();
				wobble = 1f;
				shadowRotation = GameMath.RandomAngle();
				break;
			case TileContentsType.Animal:
				viewDistance = 8f;
				LoadMesh(((AvatarFarmOnline.Logic.Stage.Contents.Animal)arg2).Definition.Category, ((AvatarFarmOnline.Logic.Stage.Contents.Animal)arg2).Definition.Id, this);
				if (Mesh is SkinnedMesh)
				{
					((SkinnedMesh)Mesh).SetTimeScale(GameMath.Random.NextFloat(0.5f, 0.85f));
				}
				if (meshes.Count > 0 && Mesh.FirstMaterial.FloatParameterCount <= 0)
				{
					Mesh.FirstMaterial.AddFloatParameter(1f);
				}
				wobble = 1f;
				shadowRotation = GameMath.RandomAngle();
				break;
			case TileContentsType.Plant:
				viewDistance = 7f;
				LoadMesh(((AvatarFarmOnline.Logic.Stage.Contents.Plant)arg2).Definition.Category, ((AvatarFarmOnline.Logic.Stage.Contents.Plant)arg2).Definition.Id, this);
				if (meshes.Count > 0 && Mesh.FirstMaterial.FloatParameterCount <= 0)
				{
					Mesh.FirstMaterial.AddFloatParameter(1f);
					Mesh.FirstMaterial.AddFloatParameter(0f);
				}
				wobble = 1f;
				shadowRotation = (float)GameMath.Random.Next(4) * ((float)Math.PI / 2f);
				break;
			}
			clearLayer("NoShadow");
		}
		else
		{
			viewDistance = 10f;
			if (tile.X > stage.FarmData.FarmSize.X || tile.Y > stage.FarmData.FarmSize.Y || tile.X < -1 || tile.Y < -1)
			{
				switch (GameMath.Random.Next(90))
				{
				case 0:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "CherryTree", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 1:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "BananaTree", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 2:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "LemonTree", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 3:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "AppleTree", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 4:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "ApricotTree", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 5:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "Cactus", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 6:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree, "LaurelTree", this);
					viewDistance = 15f;
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 7:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration, "BarrelStack", this);
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 8:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration, "DeckChair", this);
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 9:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration, "Haystack", this);
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 10:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration, "TreeCut", this);
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 11:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration, "WaterTower", this);
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				case 12:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration, "Well", this);
					Mesh.FirstMaterial.SetFloatParameter(0, 1f);
					break;
				default:
					LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant, "Grass", this);
					viewDistance = 5f;
					break;
				}
			}
			else if (tile.X < stage.FarmData.FarmSize.X && tile.Y < stage.FarmData.FarmSize.Y && tile.X >= 0 && tile.Y >= 0)
			{
				LoadMesh(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant, "Grass", this);
				viewDistance = 5f;
			}
			shadowRotation = (float)GameMath.Random.Next(4) * ((float)Math.PI / 2f);
		}
		Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, 0f - shadowRotation);
		plantTime = Timer.DefaultTimer.TotalTime;
		currentScale = 0f;
	}

	private void AddShadow()
	{
		shadow = new Sized2DRectangleMesh(new Vector2(3f, 1.5f), TextureManager.Textures["TreeShadow"]);
		shadow.Shader = ShaderManager.Shaders["SimpleNoZWrite"];
		shadow.UseXZCoords = true;
		shadow.ZValue = 0.02f;
		addMesh(shadow);
	}

	protected override void DoUpdate()
	{
		if (farmTile != null && !farmTile.IsEmpty)
		{
			switch (farmTile.Contents.Type)
			{
			case TileContentsType.Plant:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Plant plant = (AvatarFarmOnline.Logic.Stage.Contents.Plant)farmTile.Contents;
				progress = plant.GrowthProgress;
				if (progress >= 1f && progress > Mesh.FirstMaterial.GetFloatParameter(0))
				{
					wobble = 1f;
				}
				Mesh.FirstMaterial.SetFloatParameter(0, progress);
				switch (plant.State)
				{
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Growing:
					Mesh.FirstMaterial.Specular = GrowingPlantColor;
					Mesh.FirstMaterial.SetFloatParameter(1, 0.75f - plant.GrowthProgress * 0.5f);
					break;
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather:
					Transform.Scale = Vector3.One;
					Mesh.FirstMaterial.Specular = Vector3.One;
					Mesh.FirstMaterial.SetFloatParameter(1, 0f);
					break;
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered:
					Mesh.FirstMaterial.SetFloatParameter(1, 1f);
					Mesh.FirstMaterial.Specular = WitheredPlantColor;
					break;
				}
				break;
			}
			case TileContentsType.Tree:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Tree tree = (AvatarFarmOnline.Logic.Stage.Contents.Tree)farmTile.Contents;
				progress = ((tree.State == AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather) ? 1 : 0);
				if (progress >= 1f && progress > Mesh.FirstMaterial.GetFloatParameter(0))
				{
					wobble = 1f;
				}
				Mesh.FirstMaterial.SetFloatParameter(0, progress);
				break;
			}
			case TileContentsType.Animal:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Animal animal = (AvatarFarmOnline.Logic.Stage.Contents.Animal)farmTile.Contents;
				progress = ((animal.State == AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.ReadyToGather) ? 1 : 0);
				Mesh.FirstMaterial.SetFloatParameter(0, progress);
				if (progress >= 1f && progress > Mesh.FirstMaterial.GetFloatParameter(0))
				{
					wobble = 1f;
				}
				break;
			}
			}
		}
		else
		{
			Diffuse = Vector3.Lerp(Diffuse, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.01f);
		}
		if (shadow != null)
		{
			Vector2 v = new Vector2(tilePos.Z - lightSource.Y, tilePos.X - lightSource.X);
			shadow.Rotation = (float)Math.PI / 2f - shadowRotation - GameMath.VectorAngle(v);
			shadow.Offset = GameMath.VectorFromAngle(shadow.Rotation, shadow.Size.X * 0.4f);
		}
		if (Meshes.Count > 0)
		{
			if (farmTile == null || farmTile.Contents == null || stage.LocalPlayer.CameraState != AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World)
			{
				Vector3 value = stage.LocalPlayer.WorldPosition;
				Vector3.Distance(ref value, ref tilePos, out var result);
				float amount = GameMath.Saturate((result / 2f - (viewDistance - 2f)) / 1f);
				Alpha = GameMath.Interpolate(1f, 0f, amount);
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
			Visible = Alpha > 0f;
		}
		if (currentScale != 1f)
		{
			long num = Timer.DefaultTimer.TimeSince(plantTime);
			if (num < 800)
			{
				currentScale = GameMath.Damping(currentScale, 1f, 0.85f);
			}
			else
			{
				currentScale = 1f;
			}
		}
		float num2 = currentScale;
		if (wobble > 0.001f)
		{
			float num3 = 1f + wobble * (float)Math.Sin(stage.Timer.TotalTimeSeconds * 8f) * 0.75f;
			float num4 = 1f + wobble * (float)Math.Cos(stage.Timer.TotalTimeSeconds * 8f) * 0.4f;
			Transform.Scale = new Vector3(num4 * currentScale, num3 * currentScale, num4 * currentScale);
			wobble = GameMath.Interpolate(wobble, 0f, 0.075f);
		}
		else if (Transform.Scale.X != num2)
		{
			Transform.Scale = new Vector3(currentScale);
		}
		base.DoUpdate();
	}

	protected override void DoRender()
	{
		base.DoRender();
	}
}
