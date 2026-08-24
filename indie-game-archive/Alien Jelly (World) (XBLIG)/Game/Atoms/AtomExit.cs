using System;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Grids;
using Game.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomExit : AtomSingle, IGridable, IRenderable
{
	private const float PARTICLE_ROTATION_SPEED = 0.001f;

	private const float TIME_RAY_TOTAL = 5000f;

	public static string TITLE = "Exit Point";

	public static string DESCRIPTION = "This is the exit point for your Alien Jellies. This item has properties.";

	public static string PROPERTIES_DESCRIPTION = "This exit point has the following properties that allow control over what type of Alien Jelly can exit through it.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[1]
	{
		new AtomProperty("Alien Jelly Types", "This option allows you to set the type of Alein Jellies allowed through this exit.", new string[5] { "All", "Red", "Green", "Blue", "Yellow" })
	};

	public static int[] PROPERTIES_DEFAULT;

	public static Color[] COLORS;

	private static Range TIME_BOB_TOTAL;

	private static Range DELTA_BOB_TOTAL;

	private Matrix _render_matrix;

	public ParticleEmitter emitter;

	public ParticleEmitterSchema emitterSchema;

	public int type;

	private MaxModelPart partShip;

	private MaxModelPart partBrain;

	private MaxModelPartRenderable partRay;

	private MaxModelPartRenderable partGlass;

	private EffectParameter effectTint;

	private EffectParameter effectTime;

	private float timeRay;

	private float timeBob;

	private float timeBobTotal;

	private Vector3 positionBase;

	private float bobDelta;

	public override int[] properties
	{
		get
		{
			return base.properties;
		}
		set
		{
			base.properties = value;
			if (value.Length > 0)
			{
				type = value[0];
				SetEffects();
			}
		}
	}

	public AtomExit(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void Load()
	{
		useMaterials = false;
		base.Load();
		partShip = model.PartFromName("Ship");
		partBrain = model.PartFromName("Brain");
		partGlass = new MaxModelPartRenderable(manager.scene, this, model.PartFromName("Glass"));
		partRay = new MaxModelPartRenderable(manager.scene, this, model.PartFromName("Ray"));
		effectTint = partRay.part.material.effect.Parameters["tint"];
		effectTime = partRay.part.material.effect.Parameters["time"];
		model.modelParts.Remove(partRay.part);
		model.modelPartsCount = model.modelParts.Count;
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(partGlass.guid.value, partGlass);
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(partRay.guid.value, partRay);
	}

	public override void Dispose()
	{
		if (emitter != null)
		{
			emitter.Dispose();
		}
		base.Dispose();
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(partGlass.guid.value, partGlass);
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(partRay.guid.value, partRay);
		partRay.Dispose();
		partGlass.Dispose();
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		timeRay += oGameTime.ElapsedGameTime.Milliseconds;
		timeBob += oGameTime.ElapsedGameTime.Milliseconds;
		timeRay %= 5000f;
		if (timeBob >= timeBobTotal)
		{
			timeBob %= timeBobTotal;
			timeBobTotal = TIME_BOB_TOTAL.random;
		}
		effectTime.SetValue(timeRay / 5000f);
		position = positionBase + matrix.Up * (float)(Math.Sin(timeBob / timeBobTotal * (float)Math.PI * 2f) * (double)bobDelta);
		emitter.matrix = matrix;
	}

	public override void InitPlay()
	{
		base.InitPlay();
		positionBase = position;
		timeBobTotal = TIME_BOB_TOTAL.random;
		bobDelta = DELTA_BOB_TOTAL.random;
		Particles_Set();
		emitter.position = position;
		emitter.Start(2000f, emitterSchema);
	}

	public void SetEffects()
	{
		if (effectTint != null)
		{
			effectTint.SetValue(COLORS[type].ToVector4());
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			partShip.material.effect.Parameters["data"].SetValue(data);
			_render_matrix = matrix;
			Camera camera = manager.scene.cameras.camera;
			partShip.Render(ref _render_matrix, camera);
			partBrain.Render(ref _render_matrix, camera);
		}
	}

	public void Particles_Set()
	{
		emitter = new ParticleEmitter(manager.scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD);
		emitterSchema = new ParticleEmitterSchema(50);
		emitterSchema.mode = ParticleEmitter.Mode.Loop;
		emitterSchema.rotationStart = 0f;
		emitterSchema.rotationEnd = 0f;
		emitterSchema.rotationTween = -1;
		emitterSchema.scaleStart = 1f;
		emitterSchema.scaleEnd = 6f;
		emitterSchema.scaleTween = 2;
		emitterSchema.textureStart = manager.scene.library.texture2Ds["TextureDiffuse_Particles_Stars_2"];
		emitterSchema.textureEnd = manager.scene.library.texture2Ds["TextureDiffuse_Particles_Stars_3"];
		emitterSchema.textureTween = 1;
		emitterSchema.tintStart = COLORS[type];
		emitterSchema.tintStart.A = 0;
		emitterSchema.tintEnd = COLORS[type];
		emitterSchema.tintTween = 2;
		emitterSchema.tween = 0;
		emitterSchema.Float_Spread(ref emitterSchema.data, 0u, 1f, 0f);
		emitterSchema.Vector_Random(ref emitterSchema.positions, new Vector3(0f, 0f, 0f), 10f, 15f);
		emitterSchema.Vector_Constant(ref emitterSchema.deltas, new Vector3(0f, 37f, -34f));
	}

	public override void Event_Flip_Start()
	{
		if (play)
		{
			position = positionBase;
		}
		base.Event_Flip_Start();
	}

	public override void Event_Flip_End()
	{
		if (play)
		{
			positionBase = position;
		}
		base.Event_Flip_End();
	}

	public override void Event_Flip_Update()
	{
		base.Event_Flip_Update();
		if (play)
		{
			emitter.position = position;
		}
	}

	static AtomExit()
	{
		int[] pROPERTIES_DEFAULT = new int[1];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
		COLORS = new Color[5]
		{
			new Color(255, 255, 255, 200),
			new Color(255, 0, 32, 200),
			new Color(64, 255, 0, 200),
			new Color(0, 102, 255, 200),
			new Color(255, 212, 0, 200)
		};
		TIME_BOB_TOTAL = new Range(500f, 3000f);
		DELTA_BOB_TOTAL = new Range(1f, 5f);
	}
}
