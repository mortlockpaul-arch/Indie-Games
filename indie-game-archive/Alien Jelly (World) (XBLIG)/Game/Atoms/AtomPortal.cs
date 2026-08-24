using System;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Grids;
using Game.Particles;
using Game.QBits;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomPortal : AtomSingle, IGridable, IRenderable
{
	private const float TIME_TOTAL = 5000f;

	public static string TITLE = "Portal";

	public static string DESCRIPTION = "This is a portal that can beam your Alien Jellies from one place to another. This item has properties.";

	public static string PROPERTIES_DESCRIPTION = "This portal has the following properties that allow control over what type of Alien Jelly can use it and where it goes to.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[2]
	{
		new AtomProperty("Alien Jelly Types", "This option allows you to set the type of Alein Jellies allowed through this portal.", new string[5] { "All", "Red", "Green", "Blue", "Yellow" }),
		new AtomProperty("Destination", "This option allows you to set the detination this portal.", new string[1] { "#MARKERS" })
	};

	public static int[] PROPERTIES_DEFAULT;

	public static Color[] COLORS_PARTICLES;

	public static Color[] COLORS;

	public static Color[] COLORS_LOW;

	private MaxModelPart partModel;

	private MaxModelPartRenderable partRays;

	private EffectParameter effectTint;

	private EffectParameter effectTintLow;

	private EffectParameter effectTime;

	public ParticleEmitter emitter;

	public ParticleEmitterSchema emitterSchema;

	public int type;

	public int marker;

	private float time;

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
				marker = value[1];
				SetEffects();
			}
		}
	}

	public AtomPortal(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void Load()
	{
		useMaterials = false;
		base.Load();
		partModel = model.PartFromName("Model");
		partRays = new MaxModelPartRenderable(manager.scene, this, model.PartFromName("Rays"));
		effectTint = partRays.part.material.effect.Parameters["tint"];
		effectTintLow = partRays.part.material.effect.Parameters["low"];
		effectTime = partRays.part.material.effect.Parameters["time"];
		model.modelParts.Remove(partRays.part);
		model.modelPartsCount = model.modelParts.Count;
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Add(partRays.guid.value, partRays);
		trigger = new AtomTrigger(this, Triggered);
		SetEffects();
	}

	public override void Dispose()
	{
		if (emitter != null)
		{
			emitter.Dispose();
		}
		if (emitterSchema != null)
		{
			emitterSchema.Dispose();
		}
		effectTint = null;
		effectTintLow = null;
		effectTime = null;
		emitter = null;
		emitterSchema = null;
		base.Dispose();
		manager.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD).Remove(partRays.guid.value, partRays);
		partModel = null;
		partRays.Dispose();
		partRays = null;
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		time += oGameTime.ElapsedGameTime.Milliseconds;
		time %= 5000f;
		effectTime.SetValue(time / 5000f);
	}

	public override void InitPlay()
	{
		base.InitPlay();
		Particles_Set();
		emitter.position = position;
		emitter.Start(500f, emitterSchema);
		SetEffects();
	}

	public bool Triggered(object oTriggerer)
	{
		bool flag = false;
		QBit qBit = oTriggerer as QBit;
		return oTriggerer != null && (type == 0 || (type > 0 && qBit.type == (QBit.QBitType)(type - 1)));
	}

	public void SetEffects()
	{
		if (effectTint != null)
		{
			effectTint.SetValue(COLORS[type].ToVector4());
			effectTintLow.SetValue(COLORS_LOW[type].ToVector4());
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

	public void Particles_Set()
	{
		emitter = new ParticleEmitter(manager.scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD);
		emitterSchema = new ParticleEmitterSchema(50);
		emitterSchema.mode = ParticleEmitter.Mode.Loop;
		emitterSchema.rotationStart = 0f;
		emitterSchema.rotationEnd = (float)Math.PI * 2f;
		emitterSchema.rotationTween = -1;
		emitterSchema.scaleStart = 5f;
		emitterSchema.scaleEnd = 1f;
		emitterSchema.scaleTween = 4;
		emitterSchema.textureStart = manager.scene.library.texture2Ds["TextureDiffuse_Particles_Ember_1"];
		emitterSchema.textureEnd = manager.scene.library.texture2Ds["TextureDiffuse_Particles_Stripe_1"];
		emitterSchema.textureTween = 4;
		emitterSchema.tintStart = COLORS_PARTICLES[type];
		emitterSchema.tintStart.A = 0;
		emitterSchema.tintEnd = COLORS_PARTICLES[type];
		emitterSchema.tintTween = 2;
		emitterSchema.tween = 4;
		emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 1f);
		emitterSchema.Vector_Random(ref emitterSchema.positions, new Vector3(0f, 25f, 0f), 5f, 15f);
		emitterSchema.Vector_Focus(ref emitterSchema.positions, ref emitterSchema.deltas, Vector3.Zero, -10f, -20f);
	}

	public override void Event_Triggered_Start(object oTriggerer)
	{
		QBit qBit = oTriggerer as QBit;
		Vector3 vector = (manager as PlayAtomManager).Marker_FromIndex(marker);
		if (qBit != null && vector.Y != (float)AtomMarker.OUTOFBOUNDS)
		{
			qBit.Move_Portal((int)vector.X, (int)vector.Y, (int)vector.Z);
		}
	}

	public override void Event_Flip_Update()
	{
		base.Event_Flip_Update();
		if (emitter != null)
		{
			emitter.position = position;
		}
	}

	static AtomPortal()
	{
		int[] pROPERTIES_DEFAULT = new int[2];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
		COLORS_PARTICLES = new Color[5]
		{
			new Color(255, 255, 255, 255),
			new Color(255, 64, 64, 255),
			new Color(64, 255, 64, 255),
			new Color(64, 64, 255, 255),
			new Color(255, 255, 64, 255)
		};
		COLORS = new Color[5]
		{
			new Color(255, 255, 255, 200),
			new Color(255, 0, 32, 255),
			new Color(64, 255, 0, 255),
			new Color(0, 102, 255, 255),
			new Color(255, 212, 0, 255)
		};
		COLORS_LOW = new Color[5]
		{
			new Color(128, 0, 128, 0),
			new Color(32, 32, 0, 0),
			new Color(0, 32, 32, 0),
			new Color(16, 0, 32, 0),
			new Color(128, 64, 0, 0)
		};
	}
}
