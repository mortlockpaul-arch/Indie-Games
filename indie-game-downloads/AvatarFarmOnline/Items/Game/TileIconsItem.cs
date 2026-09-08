using System;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Contents;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game;

internal class TileIconsItem : RenderItem
{
	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private ParticleMesh pm;

	public TileIconsItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		pm = new ParticleMesh(TextureManager.Textures["TileStateIcons"], 256, ShaderManager.Shaders["TileIcons"]);
		pm.TileCols = 4;
		pm.TileRows = 2;
		addMesh(pm);
	}

	protected override void DoUpdate()
	{
		Visible = stage.LocalPlayer.CameraState != AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World && !stage.IsOnShop;
		if (Visible && stage.LocalPlayer.HighlightedTile != null)
		{
			int num = 0;
			AvatarFarmOnline.Logic.Stage.FarmTile highlightedTile = stage.LocalPlayer.HighlightedTile;
			int num2 = Math.Max(0, highlightedTile.Tile.X - 7);
			int num3 = Math.Min(stage.FarmData.FarmSize.X, highlightedTile.Tile.X + 7);
			int num4 = Math.Max(0, highlightedTile.Tile.Y - 7);
			int num5 = Math.Min(stage.FarmData.FarmSize.Y, highlightedTile.Tile.Y + 7);
			Vector2 position = stage.LocalPlayer.Position;
			ParticleMesh.Particle particle = new ParticleMesh.Particle(Vector3.Zero, Vector4.One, 0.5f, 0f);
			switch (stage.LocalPlayer.CameraState)
			{
			case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Cenital:
				particle.Size = 0.75f;
				break;
			case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Isometric:
				particle.Size = 0.8f;
				break;
			case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Normal:
				particle.Size = 0.55f;
				break;
			}
			for (int i = num2; i < num3; i++)
			{
				for (int j = num4; j < num5; j++)
				{
					if (num >= pm.ParticleData.Length)
					{
						break;
					}
					AvatarFarmOnline.Logic.Stage.FarmTile farmTile = stage.FarmData.Tiles[i, j];
					if (farmTile.IsEmpty)
					{
						continue;
					}
					AvatarFarmOnline.Logic.Stage.TileContents contents = farmTile.Contents;
					int num6 = -1;
					switch (contents.Type)
					{
					case TileContentsType.Tree:
						if (((AvatarFarmOnline.Logic.Stage.Contents.Tree)contents).State == AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather)
						{
							num6 = 1;
						}
						break;
					case TileContentsType.Animal:
						switch (((AvatarFarmOnline.Logic.Stage.Contents.Animal)contents).State)
						{
						case AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.ReadyToGather:
							num6 = 1;
							break;
						case AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.NeedsFood:
							num6 = 3;
							break;
						}
						break;
					case TileContentsType.Plant:
						switch (((AvatarFarmOnline.Logic.Stage.Contents.Plant)contents).State)
						{
						case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather:
							num6 = 0;
							break;
						case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered:
							num6 = 4;
							break;
						case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Growing:
							if (!((AvatarFarmOnline.Logic.Stage.Contents.Plant)contents).Watered)
							{
								num6 = 2;
							}
							break;
						}
						break;
					}
					if (num6 != -1)
					{
						float num7 = Vector2.Distance(AvatarFarmOnline.Logic.GameGlobals.TilePosition(new Int2(i, j)), position);
						float alpha = 1f - GameMath.Clamp(0f, 1f, num7 - 12f);
						Vector3 position2 = AvatarFarmOnline.Logic.GameGlobals.WorldPosition(AvatarFarmOnline.Logic.GameGlobals.TilePosition(farmTile.Tile) + new Vector2(1f)) + new Vector3(0f, 1.25f, 0f);
						particle.Position = position2;
						particle.Alpha = alpha;
						particle.Tile = num6;
						pm.ParticleData[num++] = particle;
					}
				}
			}
			foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in stage.FarmData.Buildings)
			{
				if (num >= pm.ParticleData.Length)
				{
					break;
				}
				if (!building.IsGamble && building.CanBeGathered(out var _))
				{
					float num8 = 1f - GameMath.Clamp(0f, 1f, Vector2.Distance(building.CenterPosition, position) - 12f);
					if (num8 > 0f)
					{
						particle.Position = AvatarFarmOnline.Logic.GameGlobals.WorldPosition(building.CenterPosition) + new Vector3(0f, 1.25f, 0f);
						particle.Tile = 5;
						particle.Alpha = num8;
						pm.ParticleData[num++] = particle;
					}
				}
			}
			foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in stage.FarmData.Tools)
			{
				if (num >= pm.ParticleData.Length)
				{
					break;
				}
				if (tool.RemainingFuel == 0)
				{
					float num9 = 1f - GameMath.Clamp(0f, 1f, Vector2.Distance(tool.CenterPosition, position) - 12f);
					if (num9 > 0f)
					{
						particle.Position = AvatarFarmOnline.Logic.GameGlobals.WorldPosition(tool.CenterPosition) + new Vector3(0f, 1.25f, 0f);
						particle.Tile = 6;
						particle.Alpha = num9;
						pm.ParticleData[num++] = particle;
					}
				}
			}
			pm.ParticleNumber = num;
		}
		base.DoUpdate();
	}
}
