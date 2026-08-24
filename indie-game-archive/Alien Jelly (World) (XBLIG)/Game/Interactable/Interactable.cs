using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Atoms;
using Game.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Interactable;

public class Interactable : PhysicsItem
{
	public InteractableManager manager;

	protected EntityStack renderStack;

	protected MaxModel model;

	public AtomDefinition definition;

	public Interactable(InteractableManager oManager, AtomDefinition oDef)
	{
		manager = oManager;
		scene = manager.universe.scene;
		definition = oDef;
		manager.universe.physics.Add(this);
		Load();
	}

	public override void Load()
	{
		base.Load();
		for (int i = 0; i < model.modelParts.Count; i++)
		{
			if (model.modelParts[i].material.effect.Parameters["CamCull"] != null)
			{
				model.modelParts[i].material.effect.Parameters["CamCull"].SetValue(value: false);
			}
		}
		renderStack.Add(guid.value, this);
	}

	public virtual void Update(GameTime oGameTime)
	{
	}

	public override void Dispose()
	{
		base.Dispose();
		manager.universe.physics.Remove(this);
		renderStack.Remove(guid.value, this);
		model.Dispose();
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			model.Render(scene.cameras.camera);
		}
	}

	public void RenderEffect(ref Effect oEffect)
	{
		if (visible)
		{
			oEffect.Parameters["World"].SetValue(matrix);
			model.RenderEffect(ref oEffect);
		}
	}
}
