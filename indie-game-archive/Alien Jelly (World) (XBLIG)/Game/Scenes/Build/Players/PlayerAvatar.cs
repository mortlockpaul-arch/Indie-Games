using System;
using GKEngine;
using GKEngine.Entities;
using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.Players;

public class PlayerAvatar : Entity3D
{
	private const string PATH_MODEL = "Content/Models/Player/Model";

	public Player player;

	public new BuildScene scene;

	private GridPoint pointLast;

	public GridPoint point;

	private MaxModel model;

	private MaxModelPart modelPartX;

	private MaxModelPart modelPartZ;

	private MaxModelPart modelPartY;

	public PlayerAvatar(Player oPlayer)
	{
		player = oPlayer;
		scene = player.universe.scene;
		point = new GridPoint(this);
		pointLast = new GridPoint();
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/Player/Model").Clone();
		model.Build(this);
		modelPartX = model.PartFromName("Model_Axis_X");
		modelPartY = model.PartFromName("Model_Axis_Y");
		modelPartZ = model.PartFromName("Model_Axis_Z");
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Add(guid.value, this);
		player.universe.scene.lights.SetEffect(ref model);
		base.Load();
	}

	public override void Dispose()
	{
		base.Dispose();
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Remove(guid.value, this);
		model.Dispose();
	}

	public void Update(GameTime oGameTime)
	{
		if (visible)
		{
			point.FromPosition(player.position);
			if (pointLast.X != point.X || pointLast.Y != point.Y || pointLast.Z != point.Z)
			{
				player.universe.atoms.Cursor_Change(point);
			}
			pointLast.X = point.X;
			pointLast.Y = point.Y;
			pointLast.Z = point.Z;
			modelPartX.visible = player.camera.axis.X == 0 || player.camera.axis.Y == 0;
			modelPartY.visible = player.camera.axis.X == 1 || player.camera.axis.Y == 1;
			modelPartZ.visible = player.camera.axis.X == 2 || player.camera.axis.Y == 2;
			if ((player.camera.axis.X == 0 || player.camera.axis.Y == 0) && (player.camera.axis.X == 2 || player.camera.axis.Y == 2))
			{
				player.universe.buildGrid.plane.X = 0f;
				player.universe.buildGrid.plane.Y = Y;
				player.universe.buildGrid.plane.Z = 0f;
				player.universe.buildGrid.plane.rotation = Quaternion.Identity;
			}
			else if ((player.camera.axis.X == 0 || player.camera.axis.Y == 0) && (player.camera.axis.X == 1 || player.camera.axis.Y == 1))
			{
				player.universe.buildGrid.plane.X = 0f;
				player.universe.buildGrid.plane.Y = 0f;
				player.universe.buildGrid.plane.Z = Z;
				player.universe.buildGrid.plane.rotation = Quaternion.CreateFromYawPitchRoll(0f, (float)Math.PI / 2f, 0f);
			}
			else if ((player.camera.axis.X == 2 || player.camera.axis.Y == 2) && (player.camera.axis.X == 1 || player.camera.axis.Y == 1))
			{
				player.universe.buildGrid.plane.X = X;
				player.universe.buildGrid.plane.Y = 0f;
				player.universe.buildGrid.plane.Z = 0f;
				player.universe.buildGrid.plane.rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, (float)Math.PI / 2f);
			}
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			model.Render(scene.cameras.camera);
		}
	}
}
