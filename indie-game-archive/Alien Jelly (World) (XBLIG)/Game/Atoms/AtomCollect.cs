using System;
using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Grids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomCollect : AtomSingle, IGridable, IRenderable, ICollectable
{
	private const float SPIN_SPEED = 0.0005f;

	private const float TIME_TOTAL = 20000f;

	private const float BOB_DELTA = 1.3f;

	public static Color[] COLORS = new Color[4]
	{
		new Color(1f, 0.9f, 0.5f, 1f),
		new Color(0f, 1f, 0f, 1f),
		new Color(1f, 0.2f, 0f, 1f),
		new Color(46, 208, 255, 255)
	};

	private bool spinning = true;

	private float time;

	private MaxModelPart partModel;

	private MaxModelPartRenderable partPlane;

	private EffectParameter effectRatio;

	private bool _collected;

	public bool collected => _collected;

	public int type => (definition as AtomCollectDefinition).value switch
	{
		100 => 0, 
		250 => 1, 
		1000 => 2, 
		_ => 0, 
	};

	public Atom atom => this;

	public AtomCollect(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
		time = (float)GameEngine.random.NextDouble() * 20000f;
	}

	public override void Load()
	{
		useMaterials = false;
		base.Load();
		partModel = model.PartFromName("Model");
		partPlane = new MaxModelPartRenderable(manager.scene, this, model.PartFromName("Plane"));
		partPlane.part.material.effect.Parameters["scaleStart"].SetValue(1.2f);
		partPlane.part.material.effect.Parameters["scaleEnd"].SetValue(1.4f);
		partPlane.part.material.effect.Parameters["scaleTween"].SetValue(2);
		partPlane.part.material.effect.Parameters["tintTween"].SetValue(2);
		partPlane.part.material.effect.Parameters["rotationStart"].SetValue(0);
		partPlane.part.material.effect.Parameters["rotationEnd"].SetValue((float)Math.PI * 2f);
		partPlane.part.material.effect.Parameters["rotationTween"].SetValue(0);
		partPlane.part.material.effect.Parameters["TextureDiffuse_0"].SetValue(GameEngine.SceneContent.Load<Texture2D>("Content/Materials/Common/Rays/TextureDiffuse_0"));
		partPlane.part.material.effect.Parameters["TextureDiffuse_1"].SetValue(GameEngine.SceneContent.Load<Texture2D>("Content/Materials/Common/Rays/TextureDiffuse_1"));
		partPlane.part.material.effect.Parameters["textureTween"].SetValue(2);
		effectRatio = partPlane.part.material.effect.Parameters["ratio"];
		model.modelParts.Remove(partPlane.part);
		model.modelPartsCount = model.modelParts.Count;
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(partPlane.guid.value, partPlane);
	}

	public override void Dispose()
	{
		base.Dispose();
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(partPlane.guid.value, partPlane);
		partPlane.Dispose();
		partPlane = null;
		partModel = null;
	}

	public override void InitPlay()
	{
		base.InitPlay();
		rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, (float)(GameEngine.random.NextDouble() * Math.PI * 2.0));
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		time += oGameTime.ElapsedGameTime.Milliseconds;
		time %= 20000f;
		float num = time / 20000f;
		effectRatio.SetValue(time / 20000f);
		if (spinning)
		{
			rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, (float)oGameTime.ElapsedGameTime.Milliseconds * 0.0005f);
			Y = (float)point.Y * Grid.SPACING.Y + (float)(Math.Sin((double)(num * 4f) * Math.PI * 2.0) * 1.2999999523162842);
		}
	}

	public void Collect()
	{
		if (!collected)
		{
			_collected = true;
			visible = false;
			partPlane.part.visible = false;
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			partModel.material.effect.Parameters["data"].SetValue(data);
			Camera camera = manager.scene.cameras.camera;
			partModel.Render(ref _matrix, camera);
		}
	}
}
