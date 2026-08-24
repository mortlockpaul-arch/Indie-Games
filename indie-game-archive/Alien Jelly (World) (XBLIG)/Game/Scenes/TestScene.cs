using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using Game.Particles;
using Microsoft.Xna.Framework;

namespace Game.Scenes;

public class TestScene : Scene
{
	public static TestScene instance;

	private MaxModelRenderable model;

	private Base3D modelBase;

	public ParticleEmitter emitter;

	public ParticleEmitterSchema emitterSchema;

	public TestScene()
		: base("Test")
	{
		instance = this;
		renderStacks.Add(new EntityStack(this, Material.State.Solid, GameMain.RENDERSTACK_SOLID, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Alpha, GameMain.RENDERSTACK_ALPHA_HARD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Alpha, GameMain.RENDERSTACK_ALPHA_SORTED, xSort: true));
		renderStacks.Add(new EntityStack(this, Material.State.AlphaNoDepthWrite, GameMain.RENDERSTACK_ALPHA_UNSORTED, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_MANUAL, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Add, GameMain.RENDERSTACK_ADD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_UI, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_DIALOGS, xSort: false));
	}

	public override void Load()
	{
		library.FileLoad("Content/Data/Library_Play.xml");
		base.Load();
		Init();
	}

	public override void Init()
	{
		base.Init();
		ParticleEmitter.Initialize();
		cameras.camera.Z = 100f;
		modelBase = new Base3D();
		modelBase.position = new Vector3(0f, 0f, 0f);
		modelBase.scale = new Vector3(100f, 100f, 100f);
		model = new MaxModelRenderable(this, GameEngine.SceneContent.Load<MaxModel>("Content/Models/Universe/Grid/Model").Clone());
		model.model.Build(modelBase);
		RenderStacks_FromName(GameMain.RENDERSTACK_ALPHA_HARD).Add(model.guid.value, model);
		Particles_Set();
		emitter.position = Vector3.Zero;
		emitter.Start(500f, emitterSchema);
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
	}

	public override void Exit()
	{
		base.Exit();
		GameMain.instance.Exit();
	}

	public override void Unload()
	{
		UniversalInput.InputEntity_Flush(InputEntity.Scope.Scene);
		base.Unload();
	}

	public void Particles_Set()
	{
		emitter = new ParticleEmitter(this, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ALPHA_UNSORTED);
		emitterSchema = new ParticleEmitterSchema(1000);
		emitterSchema.mode = ParticleEmitter.Mode.Loop;
		emitterSchema.rotationStart = 0f;
		emitterSchema.rotationEnd = (float)Math.PI * 2f;
		emitterSchema.rotationTween = 0;
		emitterSchema.scaleStart = 5f;
		emitterSchema.scaleEnd = 1f;
		emitterSchema.scaleTween = 4;
		emitterSchema.textureStart = library.texture2Ds["TextureDiffuse_Particles_Stars_0"];
		emitterSchema.textureEnd = library.texture2Ds["TextureDiffuse_Particles_Stripe_1"];
		emitterSchema.textureTween = 4;
		emitterSchema.tintStart = new Color(255, 255, 255, 255);
		emitterSchema.tintEnd = new Color(255, 0, 255, 255);
		emitterSchema.tintTween = 0;
		emitterSchema.tween = 4;
		emitterSchema.Float_Constant(ref emitterSchema.data, 0u, 0f);
		emitterSchema.Float_Random(ref emitterSchema.data, 1u, 0.1f, 1.5f);
		emitterSchema.Vector_Random(ref emitterSchema.positions, new Vector3(0f, 25f, 0f), 5f, 15f);
		emitterSchema.Vector_Focus(ref emitterSchema.positions, ref emitterSchema.deltas, Vector3.Zero, -10f, -20f);
	}

	public override void Input_Update(GameTime oGameTime)
	{
		base.Input_Update(oGameTime);
	}
}
