using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Data;
using Game.Entities;
using Game.Particles;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Environment;

public class Sky : Entity3D
{
	private const string PATH_MODEL = "Content/Models/Universe/Sky/Model";

	private const int MAP_SIZE = 1024;

	private const string MAP_LIB_NAME = "TextureEnv";

	private const float TIME_TOTAL = 60000f;

	private static int[] TYPE_STRIPS_A = new int[7] { 0, 2, 6, 3, 8, 4, 9 };

	private static int[] TYPE_STRIPS_B = new int[7] { 1, 3, 14, 10, 13, 14, 10 };

	private static Color COLOR_BLANK = new Color(1, 0, 0, 1);

	private static CubeMapFace[] FACES = new CubeMapFace[6]
	{
		CubeMapFace.NegativeX,
		CubeMapFace.NegativeY,
		CubeMapFace.NegativeZ,
		CubeMapFace.PositiveX,
		CubeMapFace.PositiveY,
		CubeMapFace.PositiveZ
	};

	public static string[] PARTICLES = new string[5] { "Floating Stars", "Floating Jellies", "Falling Snow", "Star Implosion", "Sun Fluff" };

	public MaxModelPart part;

	private MaxModel model;

	private EffectParameter effectStripA;

	private EffectParameter effectStripB;

	private EffectParameter effectRatio;

	private int type;

	private List<SkyItem> items;

	private List<SkyRing> rings;

	private Object3D lightPrimary;

	private Object3D lightSecondary;

	private Camera[] cameras;

	private RenderTargetCube target;

	private float time;

	public ParticleEmitter emitter;

	public ParticleEmitterSchema emitterSchema;

	public Sky(Scene oScene)
	{
		scene = oScene;
		scale = new Vector3(0.9f);
		items = new List<SkyItem>();
		rings = new List<SkyRing>();
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/Universe/Sky/Model").Clone();
		model.Build(this);
		part = model.modelParts[0];
		effectStripA = part.material.effect.Parameters["stripA"];
		effectStripB = part.material.effect.Parameters["stripB"];
		effectRatio = part.material.effect.Parameters["ratio"];
		effectStripA.SetValue(TYPE_STRIPS_A[type]);
		effectStripB.SetValue(TYPE_STRIPS_B[type]);
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Add(guid.value, this);
		lightPrimary = new Object3D(scene, "Content/Models/Universe/Sun/Model", scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD));
		lightSecondary = new Object3D(scene, "Content/Models/Universe/Sun/Model", scene.RenderStacks_FromName(GameMain.RENDERSTACK_ADD));
		base.Load();
	}

	public override void Dispose()
	{
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID).Remove(guid.value, this);
		if (scene is PlayScene)
		{
			emitter.Dispose();
		}
		base.Dispose();
		target.Dispose();
		target = null;
		scene.library.textureCubes["TextureEnv"] = null;
		part.material.Dispose();
		Items_Clear();
		items = null;
		Rings_Clear();
		rings = null;
		lightPrimary.Dispose();
		lightSecondary.Dispose();
	}

	public void Update(GameTime oGameTime)
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].Update(oGameTime);
		}
		for (int i = 0; i < rings.Count; i++)
		{
			rings[i].Update(oGameTime);
		}
		lightPrimary.Update(oGameTime.ElapsedGameTime);
		lightSecondary.Update(oGameTime.ElapsedGameTime);
		time += oGameTime.ElapsedGameTime.Milliseconds;
		time %= 60000f;
		effectRatio.SetValue(time / 60000f);
	}

	public void FromName(string xSkyDataName, int pParticleIndex)
	{
		DataSky dataSky = null;
		for (int i = 0; i < DataManager.global.skys.Count; i++)
		{
			if (DataManager.global.skys[i].name == xSkyDataName)
			{
				dataSky = DataManager.global.skys[i];
				break;
			}
		}
		if (dataSky != null)
		{
			type = dataSky.type;
			scene.lights.primary.position = dataSky.primaryLightPosition;
			scene.lights.primary.SetColor((byte)dataSky.primaryLightColor.X, (byte)dataSky.primaryLightColor.Y, (byte)dataSky.primaryLightColor.Z);
			scene.lights.secondary.position = dataSky.secondaryLightPosition;
			scene.lights.secondary.SetColor((byte)dataSky.secondaryLightColor.X, (byte)dataSky.secondaryLightColor.Y, (byte)dataSky.secondaryLightColor.Z);
			scene.lights.SetAmbientColor((byte)dataSky.ambientLightColor.X, (byte)dataSky.ambientLightColor.Y, (byte)dataSky.ambientLightColor.Z);
			lightPrimary.position = scene.lights.primary.position;
			lightPrimary.model.modelParts[0].material.effect.Parameters["Vector"].SetValue(scene.lights.primary.position);
			lightPrimary.model.modelParts[0].material.effect.Parameters["Tint"].SetValue(scene.lights.primary.color);
			lightPrimary.model.modelParts[0].material.effect.Parameters["Scale"].SetValue(30);
			lightSecondary.position = scene.lights.secondary.position;
			lightSecondary.model.modelParts[0].material.effect.Parameters["Vector"].SetValue(scene.lights.secondary.position);
			lightSecondary.model.modelParts[0].material.effect.Parameters["Tint"].SetValue(scene.lights.secondary.color);
			lightSecondary.model.modelParts[0].material.effect.Parameters["Scale"].SetValue(10);
			effectStripA.SetValue(TYPE_STRIPS_A[type]);
			effectStripB.SetValue(TYPE_STRIPS_B[type]);
			Items_FromData(dataSky, pParticleIndex);
			Rings_FromData(dataSky);
			Map_Render();
		}
	}

	private void Items_Clear()
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].Dispose();
		}
		items.Clear();
		items = new List<SkyItem>();
	}

	private void Items_FromData(DataSky oData, int pParticles)
	{
		Items_Clear();
		for (int i = 0; i < oData.items.Count; i++)
		{
			SkyItem item = new SkyItem(this, (SkyItem.Type)oData.items[i].type, oData.items[i].position, oData.items[i].scale, oData.items[i].rotation, oData.items[i].renderStack);
			items.Add(item);
		}
		Particles_Set(pParticles);
	}

	private void Rings_Clear()
	{
		for (int i = 0; i < rings.Count; i++)
		{
			rings[i].Dispose();
		}
		rings.Clear();
		rings = new List<SkyRing>();
	}

	private void Rings_FromData(DataSky oData)
	{
		Rings_Clear();
		for (int i = 0; i < oData.rings.Count; i++)
		{
			SkyRing item = new SkyRing(this, (SkyRing.Type)oData.rings[i].type, oData.rings[i].axis, oData.rings[i].position, oData.rings[i].rotation, oData.rings[i].radius, oData.rings[i].height, oData.rings[i].speed, oData.rings[i].renderStack);
			rings.Add(item);
		}
	}

	public void Map_Render()
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		int size = 1024;
		if (graphicsDevice.Viewport.Width < 1024 || graphicsDevice.Viewport.Height < 1024)
		{
			size = 512;
		}
		scene.library.textureCubes["TextureEnv"] = new TextureCube(graphicsDevice, size, mipMap: true, SurfaceFormat.Color);
		target = new RenderTargetCube(graphicsDevice, size, mipMap: true, SurfaceFormat.Color, DepthFormat.Depth24);
		Map_InitCameras();
		for (int i = 0; i < 6; i++)
		{
			graphicsDevice.SetRenderTarget(target, FACES[i]);
			graphicsDevice.Clear(COLOR_BLANK);
			RenderMap(cameras[i]);
			for (int j = 0; j < items.Count; j++)
			{
				items[j].RenderMap(cameras[i]);
			}
			for (int j = 0; j < rings.Count; j++)
			{
				rings[j].RenderMap(cameras[i]);
			}
		}
		graphicsDevice.SetRenderTarget(null);
		scene.library.textureCubes["TextureEnv"] = target;
		Material.RenderStates_Reset();
	}

	private void Map_InitCameras()
	{
		cameras = new Camera[6]
		{
			new Camera("Sky Map 0", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras),
			new Camera("Sky Map 1", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras),
			new Camera("Sky Map 2", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras),
			new Camera("Sky Map 3", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras),
			new Camera("Sky Map 4", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras),
			new Camera("Sky Map 5", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras)
		};
		cameras[0].rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, Vector3.Left, Vector3.Up));
		cameras[3].rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, Vector3.Right, Vector3.Up));
		cameras[1].rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, Vector3.Down, Vector3.Backward));
		cameras[4].rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Forward));
		cameras[2].rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up));
		cameras[5].rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Vector3.Zero, Vector3.Backward, Vector3.Up));
		for (int i = 0; i < 6; i++)
		{
			cameras[i].fov = 90f;
			cameras[i].Update_Projection(1f);
			cameras[i].view = Matrix.CreateFromQuaternion(cameras[i].rotation) * Matrix.CreateTranslation(cameras[i].position);
			cameras[i].projection = Matrix.CreateScale(new Vector3(-1f, 1f, 1f)) * cameras[i].projection;
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (!part.material.effect.IsDisposed)
		{
			model.Render(scene.cameras.camera);
		}
	}

	public void RenderMap(Camera oCam)
	{
		if (!part.material.effect.IsDisposed)
		{
			Material.RenderStates_Set(Material.State.Solid);
			model.Render(oCam);
		}
	}

	public void Particles_Set(int pParticles)
	{
		if (scene is PlayScene)
		{
			int num = 10000;
			emitter = new ParticleEmitter(scene, ParticleEmitter.EffectType.Default, GameMain.RENDERSTACK_ADD);
			switch (pParticles)
			{
			case 0:
				emitterSchema = new ParticleEmitterSchema(100);
				emitterSchema.mode = ParticleEmitter.Mode.Loop;
				emitterSchema.rotationStart = 0f;
				emitterSchema.rotationEnd = (float)Math.PI * 2f;
				emitterSchema.rotationTween = 0;
				emitterSchema.scaleStart = 50f;
				emitterSchema.scaleEnd = 80f;
				emitterSchema.scaleTween = 3;
				emitterSchema.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Dust_0"];
				emitterSchema.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Dust_1"];
				emitterSchema.textureTween = 3;
				emitterSchema.tintStart = new Color(255, 255, 255, 255);
				emitterSchema.tintEnd = new Color(255, 255, 255, 255);
				emitterSchema.tintTween = -1;
				emitterSchema.tween = 3;
				emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 1f);
				emitterSchema.Float_Random(ref emitterSchema.data, 1u, 0.5f, 2f);
				emitterSchema.Vector_Random(ref emitterSchema.positions, new Vector3(0f, 0f, 0f), 1000f, 2000f);
				emitterSchema.Vector_Random(ref emitterSchema.deltas, Vector3.Zero, 50f, 100f);
				num = 10000;
				break;
			case 1:
				emitterSchema = new ParticleEmitterSchema(50);
				emitterSchema.mode = ParticleEmitter.Mode.Loop;
				emitterSchema.rotationStart = 0f;
				emitterSchema.rotationEnd = (float)Math.PI * 2f;
				emitterSchema.rotationTween = 0;
				emitterSchema.scaleStart = 80f;
				emitterSchema.scaleEnd = 80f;
				emitterSchema.scaleTween = -1;
				emitterSchema.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Jelly_0"];
				emitterSchema.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Jelly_0"];
				emitterSchema.textureTween = -1;
				emitterSchema.tintStart = new Color(255, 255, 255, 255);
				emitterSchema.tintEnd = new Color(255, 255, 255, 255);
				emitterSchema.tintTween = -1;
				emitterSchema.tween = 3;
				emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 1f);
				emitterSchema.Float_Random(ref emitterSchema.data, 1u, 0.5f, 2f);
				emitterSchema.Vector_Random(ref emitterSchema.positions, new Vector3(0f, 0f, 0f), 2000f, 3000f);
				emitterSchema.Vector_Random(ref emitterSchema.deltas, Vector3.Zero, 50f, 100f);
				num = 30000;
				break;
			case 2:
				emitterSchema = new ParticleEmitterSchema(100);
				emitterSchema.mode = ParticleEmitter.Mode.Loop;
				emitterSchema.rotationStart = 0f;
				emitterSchema.rotationEnd = (float)Math.PI * 2f;
				emitterSchema.rotationTween = -1;
				emitterSchema.scaleStart = 10f;
				emitterSchema.scaleEnd = 10f;
				emitterSchema.scaleTween = -1;
				emitterSchema.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Ember_0"];
				emitterSchema.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Ember_1"];
				emitterSchema.textureTween = 2;
				emitterSchema.tintStart = new Color(0, 0, 0, 0);
				emitterSchema.tintEnd = new Color(255, 255, 255, 255);
				emitterSchema.tintTween = 2;
				emitterSchema.tween = 0;
				emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 1f);
				emitterSchema.Float_Random(ref emitterSchema.data, 1u, 0.5f, 2f);
				emitterSchema.Vector_Random_XZ(ref emitterSchema.positions, new Vector3(0f, 500f, 0f), new Range(-700f, 700f), new Range(-700f, 700f));
				emitterSchema.Vector_RandomRay(ref emitterSchema.deltas, Vector3.Zero, Vector3.Down, 2000f);
				break;
			case 3:
				emitterSchema = new ParticleEmitterSchema(100);
				emitterSchema.mode = ParticleEmitter.Mode.Loop;
				emitterSchema.rotationStart = 0f;
				emitterSchema.rotationEnd = (float)Math.PI * 2f;
				emitterSchema.rotationTween = -1;
				emitterSchema.scaleStart = 20f;
				emitterSchema.scaleEnd = 100f;
				emitterSchema.scaleTween = 2;
				emitterSchema.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Ember_0"];
				emitterSchema.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Stars_3"];
				emitterSchema.textureTween = 2;
				emitterSchema.tintStart = new Color(0, 0, 0, 0);
				emitterSchema.tintEnd = new Color(20, 200, 180, 255);
				emitterSchema.tintTween = 2;
				emitterSchema.tween = 0;
				emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 1f);
				emitterSchema.Float_Random(ref emitterSchema.data, 1u, 0.5f, 2f);
				emitterSchema.Vector_Random(ref emitterSchema.positions, new Vector3(0f, 0f, 0f), 2000f, 2000f);
				emitterSchema.Vector_Focus(ref emitterSchema.positions, ref emitterSchema.deltas, Vector3.Zero, -500f, -1000f);
				num = 8000;
				break;
			case 4:
				emitterSchema = new ParticleEmitterSchema(100);
				emitterSchema.mode = ParticleEmitter.Mode.Loop;
				emitterSchema.rotationStart = 0f;
				emitterSchema.rotationEnd = (float)Math.PI * 2f;
				emitterSchema.rotationTween = -1;
				emitterSchema.scaleStart = 10f;
				emitterSchema.scaleEnd = 20f;
				emitterSchema.scaleTween = 0;
				emitterSchema.textureStart = scene.library.texture2Ds["TextureDiffuse_Particles_Ember_1"];
				emitterSchema.textureEnd = scene.library.texture2Ds["TextureDiffuse_Particles_Stripe_1"];
				emitterSchema.textureTween = 2;
				emitterSchema.tintStart = new Color(0, 0, 0, 0);
				emitterSchema.tintEnd = new Color(255, 200, 0, 255);
				emitterSchema.tintTween = 2;
				emitterSchema.tween = 0;
				emitterSchema.Float_Random(ref emitterSchema.data, 0u, 0f, 1f);
				emitterSchema.Float_Random(ref emitterSchema.data, 1u, 0.5f, 2f);
				emitterSchema.Vector_Random_XZ(ref emitterSchema.positions, new Vector3(0f, -500f, 0f), new Range(-700f, 700f), new Range(-700f, 700f));
				emitterSchema.Vector_RandomRay(ref emitterSchema.deltas, Vector3.Zero, Vector3.Up, 2000f);
				break;
			}
			emitter.Start(num, emitterSchema);
		}
	}
}
