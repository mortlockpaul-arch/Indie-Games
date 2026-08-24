using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Scenes.Build;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Environment;

public class BuildGrid : Entity3D
{
	private const string PATH_MODEL = "Content/Models/Universe/Grid/Model";

	private const string PATH_MODEL_CENTER = "Content/Models/Universe/Center/Model";

	private const string PATH_MODEL_PLANE = "Content/Models/Universe/Plane/Model";

	private BuildUniverse universe;

	private MaxModel model;

	private MaxModel modelPlane;

	private EffectParameter effectPlaneCursor;

	public Base3D plane = new Base3D();

	public BuildGrid(BuildUniverse oUniverse)
	{
		universe = oUniverse;
		scene = universe.scene;
		position = new Vector3((float)universe.grid.width * 0.5f + (float)universe.grid.fromX, (float)universe.grid.height * 0.5f + (float)universe.grid.fromY, (float)universe.grid.depth * 0.5f + (float)universe.grid.fromZ);
		scale = new Vector3(universe.grid.width, universe.grid.height, universe.grid.depth);
		plane.scale = new Vector3(universe.grid.width, universe.grid.height, universe.grid.depth);
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/Universe/Grid/Model").Clone();
		model.Build(this);
		modelPlane = GameEngine.SceneContent.Load<MaxModel>("Content/Models/Universe/Plane/Model").Clone();
		modelPlane.Build(plane);
		effectPlaneCursor = modelPlane.modelParts[0].material.effect.Parameters["Cursor"];
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_ALPHA_LAST).Add(guid.value, this);
		base.Load();
	}

	public override void Dispose()
	{
		base.Dispose();
		model.Dispose();
		modelPlane.Dispose();
	}

	public override void Render(GameTime oGameTime)
	{
		Camera camera = scene.cameras.camera;
		if (model != null && visible)
		{
			effectPlaneCursor.SetValue(universe.player._position);
			model.Render(camera);
			modelPlane.Render(camera);
		}
	}
}
