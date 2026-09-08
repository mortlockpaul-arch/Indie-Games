using System;
using System.Collections.Generic;
using AvatarFarmOnline.Items.Game;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Audios;
using Quasar.Elements;
using Quasar.Global;
using Quasar.Particles;
using Quasar.Xml;

namespace AvatarFarmOnline.Scenes;

internal class StageScene : Scene
{
	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Light light;

	private AvatarFarmOnline.Items.Game.GameCamera gameCamera;

	private AvatarFarmOnline.Items.Game.FarmItem farmItem;

	private ParticleSystem plowParticles;

	private ParticleSystem waterParticles;

	private PooledAudio failedActionSound;

	private SimpleAudio ambienceSound;

	private SimpleAudio changeSeasonSound;

	private SpatializedPooledAudio plantSound;

	private SpatializedPooledAudio wrapSound;

	private SpatializedPooledAudio gambleSound;

	private SpatializedPooledAudio plowSound;

	private SpatializedPooledAudio waterSound;

	private SpatializedPooledAudio harvestSound;

	private SpatializedPooledAudio buildSound;

	private SpatializedPooledAudio toolSound;

	private SpatializedPooledAudio appearSound;

	private SpatializedPooledAudio cashSound;

	private SpatializedPooledAudio feedSound;

	private SpatializedPooledAudio chickenSound;

	private SpatializedPooledAudio cowSound;

	private SpatializedPooledAudio donkeySound;

	private SpatializedPooledAudio duckSound;

	private SpatializedPooledAudio horseSound;

	private SpatializedPooledAudio pigSound;

	private SpatializedPooledAudio rabbitSound;

	private SpatializedPooledAudio sheepSound;

	private List<AvatarFarmOnline.Items.Game.PlayerItem> playerItems = new List<AvatarFarmOnline.Items.Game.PlayerItem>();

	public AvatarFarmOnline.Items.Game.GameCamera GameCamera => gameCamera;

	public StageScene(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		SpatializedPooledAudio.DefaultDistanceLimit = 20f;
		this.stage = stage;
		gameCamera = new AvatarFarmOnline.Items.Game.GameCamera(stage);
		Camera = gameCamera;
		Add(gameCamera);
		Element element = ElementLoader.Load("Scenes/StageScene");
		element.Transform.Translation = new Vector3((float)AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.X * 2f / 2f, 0f, (float)AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.Y * 2f / 2f);
		Add(element);
		Vector3 color = AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason);
		light = new Light(color);
		light.Specular = AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason);
		light.Diffuse = Vector3.Lerp(Vector3.One, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.6f) * 0.85f;
		light.Ambient = Vector3.Lerp(Vector3.One, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.9f) * 0.75f;
		light.Attenuation = new Vector3(1f, 0f, 0f);
		addLight(light);
		farmItem = new AvatarFarmOnline.Items.Game.FarmItem(stage);
		Add(farmItem);
		AvatarFarmOnline.Items.Game.FenceItem e = new AvatarFarmOnline.Items.Game.FenceItem(stage);
		Add(e);
		plowParticles = ParticleSystem.Load("Plow");
		Add(plowParticles);
		waterParticles = ParticleSystem.Load("Water");
		Add(waterParticles);
		failedActionSound = new PooledAudio(SoundEffectManager.SoundEffects["MenuBack"], 2);
		ambienceSound = new SimpleAudio(SoundEffectManager.SoundEffects["Ambience"]);
		ambienceSound.Loop = true;
		ambienceSound.Start();
		changeSeasonSound = new SimpleAudio(SoundEffectManager.SoundEffects["SeasonChange"]);
		plantSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Plant"], gameCamera);
		plowSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Plow"], gameCamera);
		harvestSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Harvest"], gameCamera);
		buildSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Build"], gameCamera);
		waterSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["WaterSprinkler"], gameCamera);
		toolSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Tool"], gameCamera);
		appearSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["ItemAppear"], gameCamera);
		cashSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Cash"], gameCamera);
		feedSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Feed"], gameCamera);
		chickenSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Chicken"], gameCamera);
		cowSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Cow"], gameCamera);
		donkeySound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Donkey"], gameCamera);
		duckSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Duck"], gameCamera);
		horseSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Horse"], gameCamera);
		pigSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Pig"], gameCamera);
		rabbitSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Rabbit"], gameCamera);
		sheepSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Animals/Sheep"], gameCamera);
		wrapSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Wrap"], gameCamera);
		gambleSound = new SpatializedPooledAudio(SoundEffectManager.SoundEffects["Jackpot"], gameCamera);
		foreach (AvatarFarmOnline.Logic.Stage.Player player in stage.Players)
		{
			stage_OnPlayerAdded(player);
		}
		stage.OnPlayerAdded += stage_OnPlayerAdded;
		stage.OnPlayerRemoved += stage_OnPlayerRemoved;
		stage.LocalPlayer.OnFailedAction += stage_OnFailedAction;
		stage.FarmData.OnSeasonChange += FarmData_OnSeasonChange;
	}

	private void stage_OnPlayerRemoved(AvatarFarmOnline.Logic.Stage.Player obj)
	{
		AvatarFarmOnline.Items.Game.PlayerItem playerItem = null;
		foreach (AvatarFarmOnline.Items.Game.PlayerItem playerItem2 in playerItems)
		{
			if (playerItem2.Player == obj)
			{
				playerItem = playerItem2;
				break;
			}
		}
		if (playerItem != null)
		{
			playerItems.Remove(playerItem);
			Remove(playerItem);
			playerItem.Dispose();
		}
	}

	private void stage_OnPlayerAdded(AvatarFarmOnline.Logic.Stage.Player obj)
	{
		if (stage != null && obj != null)
		{
			AvatarFarmOnline.Items.Game.PlayerItem playerItem = new AvatarFarmOnline.Items.Game.PlayerItem(stage, obj);
			Add(playerItem);
			playerItems.Add(playerItem);
			obj.OnStartWork += Player_OnStartWork;
			obj.OnEndWork += Player_OnEndWork;
		}
	}

	private void Player_OnEndWork(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> tiles, bool usingTools, int rotation)
	{
		float volume = ((player is AvatarFarmOnline.Logic.Stage.LocalPlayer) ? 1f : 0.5f);
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.Plow:
		case AvatarFarmOnline.Logic.WorkType.Build:
		case AvatarFarmOnline.Logic.WorkType.Plant:
		case AvatarFarmOnline.Logic.WorkType.PlantTree:
			appearSound.Volume = volume;
			appearSound.Start(player.WorldPosition, 0.15f);
			break;
		case AvatarFarmOnline.Logic.WorkType.GatherBuilding:
			cashSound.Volume = volume;
			cashSound.Start(player.WorldPosition, 0.15f);
			break;
		case AvatarFarmOnline.Logic.WorkType.GatherAnimal:
		case AvatarFarmOnline.Logic.WorkType.PlantAnimal:
		case AvatarFarmOnline.Logic.WorkType.FeedAnimal:
			if (tiles.Count > 0 && tiles[0].Contents is AvatarFarmOnline.Logic.Stage.Contents.Animal animal)
			{
				SpatializedPooledAudio spatializedPooledAudio = ((animal.Definition.Id == "Chicken") ? chickenSound : ((animal.Definition.Id == "Cow") ? cowSound : ((animal.Definition.Id == "Donkey") ? donkeySound : ((animal.Definition.Id == "Duck") ? duckSound : ((animal.Definition.Id == "Horse") ? horseSound : ((animal.Definition.Id == "Pig") ? pigSound : ((!(animal.Definition.Id == "Rabbit")) ? sheepSound : rabbitSound)))))));
				if (spatializedPooledAudio != null)
				{
					spatializedPooledAudio.Volume = volume;
					spatializedPooledAudio.Start(player.WorldPosition, 0.15f);
				}
			}
			break;
		}
	}

	private void Player_OnStartWork(AvatarFarmOnline.Logic.Stage.Player player)
	{
		ParticleSystem particleSystem = null;
		float volume = ((player is AvatarFarmOnline.Logic.Stage.LocalPlayer) ? 1f : 0.5f);
		switch (player.WorkType)
		{
		case AvatarFarmOnline.Logic.WorkType.Gather:
			particleSystem = plowParticles;
			harvestSound.Volume = volume;
			harvestSound.Start(player.WorldPosition, 0.15f);
			break;
		case AvatarFarmOnline.Logic.WorkType.GatherTree:
			harvestSound.Volume = volume;
			harvestSound.Start(player.WorldPosition, 0.15f);
			break;
		case AvatarFarmOnline.Logic.WorkType.GatherBuilding:
			if (player.WorkTiles.Count > 0)
			{
				if (player.WorkTiles[0].Contents is AvatarFarmOnline.Logic.Stage.Contents.BuildingTile buildingTile && buildingTile.Building.IsGamble)
				{
					gambleSound.Volume = volume;
					gambleSound.Start(player.WorldPosition, 0.07f);
				}
				else
				{
					wrapSound.Volume = volume;
					wrapSound.Start(player.WorldPosition, 0.15f);
					particleSystem = plowParticles;
				}
			}
			break;
		case AvatarFarmOnline.Logic.WorkType.GatherAnimal:
			wrapSound.Volume = volume;
			wrapSound.Start(player.WorldPosition, 0.15f);
			particleSystem = plowParticles;
			break;
		case AvatarFarmOnline.Logic.WorkType.FeedAnimal:
			feedSound.Volume = volume;
			feedSound.Start(player.WorldPosition, 0.15f);
			break;
		case AvatarFarmOnline.Logic.WorkType.Plow:
		case AvatarFarmOnline.Logic.WorkType.Recycle:
			plowSound.Volume = volume;
			plowSound.Start(player.WorldPosition, 0.15f);
			particleSystem = plowParticles;
			break;
		case AvatarFarmOnline.Logic.WorkType.Build:
			buildSound.Volume = volume;
			buildSound.Start(player.WorldPosition, 0.15f);
			particleSystem = plowParticles;
			break;
		case AvatarFarmOnline.Logic.WorkType.Plant:
		case AvatarFarmOnline.Logic.WorkType.PlantTree:
		case AvatarFarmOnline.Logic.WorkType.PlantAnimal:
			plantSound.Volume = volume;
			plantSound.Start(player.WorldPosition, 0.15f);
			particleSystem = plowParticles;
			break;
		case AvatarFarmOnline.Logic.WorkType.Refill:
		case AvatarFarmOnline.Logic.WorkType.Water:
			waterSound.Volume = volume;
			waterSound.Start(player.WorldPosition, 0.15f);
			particleSystem = waterParticles;
			break;
		}
		if (player.WorkUsesTools)
		{
			toolSound.Volume = volume;
			toolSound.Start(player.WorldPosition, 0.15f);
		}
		if (particleSystem == null)
		{
			return;
		}
		foreach (AvatarFarmOnline.Logic.Stage.FarmTile workTile in player.WorkTiles)
		{
			Vector3 position = AvatarFarmOnline.Logic.GameGlobals.TileWorldPosition(workTile.Tile) + new Vector3(1f, 0f, 1f);
			particleSystem.Burst(position);
		}
	}

	private void FarmData_OnSeasonChange(AvatarFarmOnline.Logic.Seasons obj)
	{
		changeSeasonSound.Start();
	}

	private void stage_OnFailedAction(AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (failReason != AvatarFarmOnline.Logic.FailedActionReason.None && failReason != AvatarFarmOnline.Logic.FailedActionReason.CantRecycle)
		{
			failedActionSound.Start();
		}
	}

	public override void Update()
	{
		if (stage.IsPaused)
		{
			ambienceSound.Volume = 0f;
		}
		else
		{
			ambienceSound.Volume = 0.4f;
		}
		light.Specular = Vector3.Lerp(light.Specular, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.01f);
		light.Diffuse = Vector3.Lerp(light.Diffuse, Vector3.Lerp(Vector3.One, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.6f) * 0.75f, 0.01f);
		light.Ambient = Vector3.Lerp(light.Ambient, Vector3.Lerp(Vector3.One, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.9f) * 0.65f, 0.01f);
		float num = 10f * AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.Length();
		Vector2 lightSource = GameMath.VectorFromAngle(Timer.DefaultTimer.TotalTimeSeconds * ((float)Math.PI * 2f) / 300f) * num;
		light.Transform.Translation = new Vector3(lightSource.X, 1.25f * num, lightSource.Y);
		light.UpdateRotationFromTranslation();
		AvatarFarmOnline.Items.Game.BuildingItem.SetLightSource(lightSource);
		AvatarFarmOnline.Items.Game.FarmTileItem.SetLightSource(lightSource);
		plantSound.Update();
		plowSound.Update();
		harvestSound.Update();
		buildSound.Update();
		waterSound.Update();
		toolSound.Update();
		appearSound.Update();
		cashSound.Update();
		feedSound.Update();
		chickenSound.Update();
		cowSound.Update();
		donkeySound.Update();
		duckSound.Update();
		horseSound.Update();
		pigSound.Update();
		rabbitSound.Update();
		sheepSound.Update();
		wrapSound.Update();
		base.Update();
	}

	public override void Render()
	{
		base.Render();
	}

	public override void Dispose()
	{
		stage = null;
		if (gambleSound != null)
		{
			gambleSound.Dispose();
			gambleSound = null;
		}
		if (wrapSound != null)
		{
			wrapSound.Dispose();
			wrapSound = null;
		}
		if (cashSound != null)
		{
			cashSound.Dispose();
			cashSound = null;
		}
		if (feedSound != null)
		{
			feedSound.Dispose();
			feedSound = null;
		}
		if (sheepSound != null)
		{
			sheepSound.Dispose();
			sheepSound = null;
		}
		if (rabbitSound != null)
		{
			rabbitSound.Dispose();
			rabbitSound = null;
		}
		if (pigSound != null)
		{
			pigSound.Dispose();
			pigSound = null;
		}
		if (horseSound != null)
		{
			horseSound.Dispose();
			horseSound = null;
		}
		if (duckSound != null)
		{
			duckSound.Dispose();
			duckSound = null;
		}
		if (donkeySound != null)
		{
			donkeySound.Dispose();
			donkeySound = null;
		}
		if (cowSound != null)
		{
			cowSound.Dispose();
			cowSound = null;
		}
		if (chickenSound != null)
		{
			chickenSound.Dispose();
			chickenSound = null;
		}
		if (plantSound != null)
		{
			plantSound.Dispose();
			plantSound = null;
		}
		if (plowSound != null)
		{
			plowSound.Dispose();
			plowSound = null;
		}
		if (waterSound != null)
		{
			waterSound.Dispose();
			waterSound = null;
		}
		if (harvestSound != null)
		{
			harvestSound.Dispose();
			harvestSound = null;
		}
		if (buildSound != null)
		{
			buildSound.Dispose();
			buildSound = null;
		}
		if (appearSound != null)
		{
			appearSound.Dispose();
			appearSound = null;
		}
		if (changeSeasonSound != null)
		{
			changeSeasonSound.Dispose();
			changeSeasonSound = null;
		}
		if (failedActionSound != null)
		{
			failedActionSound.Dispose();
			failedActionSound = null;
		}
		if (ambienceSound != null)
		{
			ambienceSound.Dispose();
			ambienceSound = null;
		}
		foreach (AvatarFarmOnline.Items.Game.PlayerItem playerItem in playerItems)
		{
			playerItem.Dispose();
		}
		playerItems.Clear();
		base.Dispose();
	}
}
