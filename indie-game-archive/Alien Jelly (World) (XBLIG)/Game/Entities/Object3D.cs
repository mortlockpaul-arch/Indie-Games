using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;

namespace Game.Entities;

public class Object3D : Entity3D
{
	public MaxModel model;

	public string path;

	public EntityStack renderStack;

	public Object3D(Scene xScene, string xPath, EntityStack oRenderStack)
	{
		scene = xScene;
		path = xPath;
		renderStack = oRenderStack;
		Init();
	}

	public virtual void Init()
	{
		Load();
		renderStack.Add(guid.value, this);
	}

	public override void Dispose()
	{
		renderStack.Remove(guid.value, this);
		model.Dispose();
		base.Dispose();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>(path).Clone();
		model.Build(this);
		base.Load();
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible && model != null)
		{
			model.Render(matrix, scene.cameras.camera);
		}
	}

	public void Update(TimeSpan oElapsedTime)
	{
	}
}
