using BEPUphysics.Collidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace AircraftRC;

public class TerrainP
{
	public SceneState sceneStateScene;

	private SunBurnCoreSystem sunBurnCoreSystemScene;

	private FrameBuffers frameBuffersScene;

	public SceneInterface sceneInterfaceScene;

	private SceneEnvironment environmentScene;

	private Scene scenejeu;

	private StaticMesh staticMeshTerrain;

	private StaticMesh staticMeshTerObjets;

	private StaticMesh staticMeshArbres;

	private Model Piste;

	private Model terrain;

	private Model terrainObjets;

	private Model arbres;

	private Model arbresh;

	private Model fuelMzone;

	private SceneObject objterrain;

	private SceneObject objterrainObj;

	private SceneObject objterrainpiste;

	private SceneObject objarbres;

	public Skybox ciel;

	private DetectorVolume FuelZone;

	public bool FuelZActi;

	public TerrainP(CustomPhysicsGame game)
	{
		sunBurnCoreSystemScene = new SunBurnCoreSystem(game.Services, game.Content);
		sceneStateScene = new SceneState();
		sceneInterfaceScene = new SceneInterface();
		sceneInterfaceScene.CreateDefaultManagers(RenderingSystemType.Forward, includeautoloadedplugins: false);
		frameBuffersScene = new FrameBuffers(DetailPreference.High, DetailPreference.High);
		sceneInterfaceScene.ResourceManager.AssignOwnership(frameBuffersScene);
		ciel = new Skybox();
	}

	public void Load(CustomPhysicsGame game)
	{
		scenejeu = game.Content.Load<Scene>("Scenes/Scenejeu");
		environmentScene = game.Content.Load<SceneEnvironment>("Environment/Environmentjeu");
		sceneInterfaceScene.Submit(scenejeu);
		sceneInterfaceScene.ApplyPreferences(game.preferences);
		MeshData meshdata = game.Content.Load<MeshData>("Models/terrain-forward");
		objterrain = new SceneObject(meshdata);
		objterrain.World = Matrix.CreateScale(22000f) * Matrix.CreateTranslation(0f, 0f, 0f);
		ciel.Load(game);
		Piste = game.Content.Load<Model>("Models/piste");
		terrain = game.Content.Load<Model>("Models/terrain");
		terrainObjets = game.Content.Load<Model>("Models/terraina");
		arbres = game.Content.Load<Model>("Models/arbres");
		arbresh = game.Content.Load<Model>("Models/arbresh");
		fuelMzone = game.Content.Load<Model>("Models/fuelzone");
		objterrainObj = new SceneObject(terrainObjets);
		objterrainpiste = new SceneObject(Piste);
		objarbres = new SceneObject(arbres);
		TriangleMesh.GetVerticesAndIndicesFromModel(fuelMzone, out var vertices, out var indices);
		FuelZone = new DetectorVolume(new TriangleMesh(new StaticMeshData(vertices, indices)));
		TriangleMesh.GetVerticesAndIndicesFromModel(terrain, out vertices, out indices);
		staticMeshTerrain = new StaticMesh(vertices, indices);
		staticMeshTerrain.Sidedness = TriangleSidedness.Counterclockwise;
		TriangleMesh.GetVerticesAndIndicesFromModel(terrainObjets, out vertices, out indices);
		staticMeshTerObjets = new StaticMesh(vertices, indices);
		staticMeshTerObjets.Sidedness = TriangleSidedness.Counterclockwise;
		TriangleMesh.GetVerticesAndIndicesFromModel(arbresh, out vertices, out indices);
		staticMeshArbres = new StaticMesh(vertices, indices);
		sceneInterfaceScene.ObjectManager.Submit(objterrain);
		sceneInterfaceScene.ObjectManager.Submit(objterrainObj);
		sceneInterfaceScene.ObjectManager.Submit(objterrainpiste);
		sceneInterfaceScene.ObjectManager.Submit(objarbres);
		game.space.Add(staticMeshTerrain);
		game.space.Add(staticMeshTerObjets);
		game.space.Add(staticMeshArbres);
		game.space.Add(FuelZone);
		FuelZone.EntityBeganTouching += Toucher1;
		FuelZone.EntityStoppedTouching += Toucher2;
	}

	private void Toucher1(DetectorVolume volume, Entity toucher)
	{
		FuelZActi = true;
	}

	private void Toucher2(DetectorVolume volume, Entity toucher)
	{
		FuelZActi = false;
	}

	public void Draw(CustomPhysicsGame game, GameTime gameTime)
	{
		sceneStateScene.BeginFrameRendering(game.camera.View, game.camera.Projection, gameTime, environmentScene, frameBuffersScene, renderingtoscreen: true);
		sceneInterfaceScene.BeginFrameRendering(sceneStateScene);
		sceneInterfaceScene.RenderManager.Render();
		Matrix world = Matrix.CreateScale(1000f) * Matrix.CreateTranslation(0f, 90000f, 0f);
		ciel.Draw(world, game, this);
		sceneInterfaceScene.EndFrameRendering();
		sceneStateScene.EndFrameRendering();
	}
}
