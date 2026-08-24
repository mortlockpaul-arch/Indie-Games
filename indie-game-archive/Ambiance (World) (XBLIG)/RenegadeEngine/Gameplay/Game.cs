using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RenegadeEngine.Cyclone;
using RenegadeEngine.Graphics;
using RenegadeEngine.MenuSystem;

namespace RenegadeEngine.Gameplay;

public class Game
{
	protected SpriteBatch spriteBatch;

	protected GraphicsDevice device;

	protected Camera camera;

	private BloomSettings bloomSettings = new BloomSettings(0.1f, 3f, 3f, 1f, 2f, 2f);

	private Texture2D background;

	private Rectangle screenBoundary = new Rectangle(0, 0, Global.ScreenWidth, Global.ScreenHeight);

	private BoundingBox boundary = new BoundingBox(new Vector3(-40f, -40f, -40f), new Vector3(40f, 40f, 40f));

	private RigidBody cameraBody = new RigidBody();

	private InstancedModel smallStars;

	private InstancedModel giantStars;

	private TinyUpdateAsync1 updateAsync1;

	private TinyUpdateAsync2 updateAsync2;

	private TinyUpdateAsync3 updateAsync3;

	private TinyUpdateAsync4 updateAsync4;

	private string trialMsg = "Please buy me!";

	private Color trialCol = new Color(150, 150, 150, 255);

	private SpriteFont trialFnt;

	protected GameplayState gameState = GameplayState.IsStarting;

	protected ScreenState screenState = ScreenState.TransitionIn;

	protected bool isExiting;

	protected FadeTransition tracker;

	public Rectangle Boundary = new Rectangle(0, 0, Global.ScreenWidth, Global.ScreenHeight);

	public PlayerIndex ControllingPlayer;

	private float giantMassSq = DataMgr.giantMass * DataMgr.giantMass;

	private Matrix view = Matrix.Identity;

	private Matrix projection = Matrix.Identity;

	public bool IsExiting => isExiting;

	public TransitionTracker GetTracker => tracker;

	public GameplayState GameState => gameState;

	public ScreenState ScreenState
	{
		get
		{
			return screenState;
		}
		set
		{
			screenState = value;
			switch (screenState)
			{
			case ScreenState.TransitionOut:
				tracker.State = TransitionState.Out;
				break;
			case ScreenState.TransitionToBackground:
				tracker.State = TransitionState.PartialOut;
				break;
			case ScreenState.TransitionIn:
				tracker.State = TransitionState.In;
				break;
			case ScreenState.Active:
				tracker.State = TransitionState.Idle;
				break;
			}
		}
	}

	public float Transition => tracker.Transition;

	public event EventHandler Deactivated;

	public event EventHandler Disposed;

	public void Initialize()
	{
		spriteBatch = EngineManager.GetSpriteBatch;
		device = EngineManager.GetGraphicsDevice;
		BloomEffect.ApplySettings(bloomSettings);
		camera = new Camera(device.DisplayMode.AspectRatio);
		camera.Position = new Vector3(0f, 20f, 50f);
		camera.LookAt = Vector3.Zero;
		smallStars = new InstancedModel(AssetManager.GetAsset(ModelKeys.icoSphere));
		giantStars = new InstancedModel(AssetManager.GetAsset(ModelKeys.icoSphere));
		updateAsync1 = new TinyUpdateAsync1();
		updateAsync2 = new TinyUpdateAsync2();
		updateAsync3 = new TinyUpdateAsync3();
		updateAsync4 = new TinyUpdateAsync4();
		updateAsync1.Initialize(1);
		updateAsync2.Initialize(3);
		updateAsync3.Initialize(4);
		updateAsync4.Initialize(5);
		gameState = GameplayState.IsStarting;
		screenState = ScreenState.TransitionIn;
		isExiting = false;
		tracker = new FadeTransition();
		tracker.State = TransitionState.In;
		tracker.InCompleted += On_TrackerInCompleted;
		tracker.PartialCompleted += On_TrackerPartialCompleted;
		tracker.OutCompleted += On_TrackerOutCompleted;
		Boundary.X = 0;
		Boundary.Y = 0;
		Boundary.Width = Global.ScreenWidth;
		Boundary.Height = Global.ScreenHeight;
	}

	public void Dispose()
	{
		isExiting = true;
		gameState = GameplayState.IsExiting;
		screenState = ScreenState.TransitionOut;
		tracker.State = TransitionState.Out;
		updateAsync1.Dispose();
		updateAsync2.Dispose();
		updateAsync3.Dispose();
		updateAsync4.Dispose();
	}

	public void LoadContent()
	{
		AssetManager.GetAsset(ImageKeys.background, ref background);
		for (int i = 0; i < DataMgr.numGiants; i++)
		{
			DataMgr.giantBodies[i] = new RigidBody();
			DataMgr.giantBodies[i].Mass = DataMgr.giantMass;
		}
		ref VertexColorInstanceWorld reference = ref DataMgr.giantTransforms[0];
		reference = new VertexColorInstanceWorld(new Vector4(1f, 0f, 0f, 1f), Matrix.Identity);
		ref VertexColorInstanceWorld reference2 = ref DataMgr.giantTransforms[1];
		reference2 = new VertexColorInstanceWorld(new Vector4(0f, 1f, 0f, 1f), Matrix.Identity);
		ref VertexColorInstanceWorld reference3 = ref DataMgr.giantTransforms[2];
		reference3 = new VertexColorInstanceWorld(new Vector4(0f, 0f, 1f, 1f), Matrix.Identity);
		DataMgr.giantColors[0].Diffuse = new Vector3(1f, 0f, 0f);
		DataMgr.giantColors[0].Emissive = new Vector3(1f, 0.2f, 0.2f);
		DataMgr.giantBodies[0].Position = new Vector3(-10f, -5f, -5f);
		if (DataMgr.numGiants > 1)
		{
			DataMgr.giantColors[1].Diffuse = new Vector3(0f, 1f, 0f);
			DataMgr.giantColors[1].Emissive = new Vector3(0.2f, 1f, 0.2f);
			DataMgr.giantBodies[1].Position = new Vector3(10f, 0f, 5f);
		}
		if (DataMgr.numGiants > 2)
		{
			DataMgr.giantColors[2].Diffuse = new Vector3(0f, 0f, 1f);
			DataMgr.giantColors[2].Emissive = new Vector3(0.2f, 0.2f, 1f);
			DataMgr.giantBodies[2].Position = new Vector3(0f, 10f, 10f);
		}
		DataMgr.smallStar = new Sphere(device, 5, DataMgr.smallSize);
		DataMgr.smallStar.effect.LightingEnabled = false;
		DataMgr.smallStar.effect.EmissiveColor = new Vector3(0.2f, 0.2f, 0.2f);
		DataMgr.smallStar.effect.SpecularColor = Vector3.One;
		DataMgr.smallStar.Scale = Vector3.One * DataMgr.smallSize;
		for (int j = 0; j < DataMgr.numTinies; j++)
		{
			DataMgr.smallBodies[j] = new RigidBody();
			DataMgr.smallBodies[j].Position = new Vector3((float)Rand.Next(-2000, 2000) * 0.01f, (float)Rand.Next(-2000, 2000) * 0.01f, (float)Rand.Next(-2000, 2000) * 0.01f);
			DataMgr.smallBodies[j].linearDamping = 0.9800000190734863;
			DataMgr.smallBodies[j].Mass = DataMgr.tinyMass;
			DataMgr.smallColors[j].Diffuse = Vector3.One;
			ref VertexColorInstanceWorld reference4 = ref DataMgr.starTransforms[j];
			reference4 = new VertexColorInstanceWorld(Vector4.One, Matrix.Identity);
		}
		AssetManager.GetAsset(FontKeys.TitleFont, ref trialFnt);
	}

	public void UnloadContent()
	{
	}

	protected internal virtual void On_Deactivated(EventArgs e)
	{
		if (Deactivated != null)
		{
			Deactivated(this, e);
		}
	}

	protected internal virtual void On_Disposed(EventArgs e)
	{
		EngineManager.EndGameplay(ControllingPlayer);
		if (Disposed != null)
		{
			Disposed(this, e);
		}
	}

	protected internal virtual void On_TrackerInCompleted(object sender, EventArgs e)
	{
		screenState = ScreenState.Active;
	}

	protected internal virtual void On_TrackerPartialCompleted(object sender, EventArgs e)
	{
		screenState = ScreenState.Inactive;
		On_Deactivated(e);
	}

	protected internal virtual void On_TrackerOutCompleted(object sender, EventArgs e)
	{
		screenState = ScreenState.Hidden;
		if (isExiting)
		{
			On_Disposed(e);
		}
		else
		{
			On_Deactivated(e);
		}
	}

	public void Update(GameTime gameTime)
	{
		if (screenState == ScreenState.Active)
		{
			updateInput(gameTime);
		}
		screenBoundary.Width = Global.ScreenWidth;
		screenBoundary.Height = Global.ScreenHeight;
		camera.Update();
		DataMgr.frust = new BoundingFrustum(camera.View * camera.Projection);
		float[] array = new float[DataMgr.numGiants];
		float[] array2 = new float[DataMgr.numGiants];
		_ = DataMgr.numGiants;
		Vector3 zero = Vector3.Zero;
		if (DataMgr.numGiants > 1)
		{
			for (int i = 0; i < DataMgr.numGiants; i++)
			{
				Vector3.Distance(ref DataMgr.giantBodies[i].Position, ref DataMgr.giantBodies[(i + 1) % DataMgr.numGiants].Position, out array2[i]);
				array[i] = giantMassSq / array2[i] * DataMgr.movementRate;
				for (int j = 0; j < DataMgr.numGiants; j++)
				{
					int num = (i + 1) % DataMgr.numGiants;
					zero += Vector3.Normalize(DataMgr.giantBodies[num].Position - DataMgr.giantBodies[i].Position) * array[i];
					BoundingSphere boundingSphere = new BoundingSphere(DataMgr.giantBodies[i].Position, DataMgr.giantSize);
					BoundingSphere sphere = new BoundingSphere(DataMgr.giantBodies[num].Position, DataMgr.giantSize);
					if (boundingSphere.Intersects(sphere))
					{
						float num2 = DataMgr.giantSize + DataMgr.giantSize - array2[i] + 0.0001f;
						Vector3 vector = Vector3.Normalize(DataMgr.giantBodies[i].Velocity) * num2;
						DataMgr.giantBodies[i].Position -= vector;
						Vector3 value = DataMgr.giantBodies[i].Position - DataMgr.giantBodies[num].Position;
						value = Vector3.Normalize(value);
						DataMgr.giantBodies[i].AddForce(value * DataMgr.giantBodies[num].Velocity.Length());
						value = DataMgr.giantBodies[num].Position - DataMgr.giantBodies[i].Position;
						value = Vector3.Normalize(value);
						DataMgr.giantBodies[num].AddForce(value * DataMgr.giantBodies[i].Velocity.Length());
					}
				}
				DataMgr.giantBodies[i].AddForce(zero / DataMgr.numGiants);
				BoundingSphere sphere2 = new BoundingSphere(DataMgr.giantBodies[i].Position, DataMgr.giantSize);
				if (DataMgr.frust.Left.Intersects(sphere2) == PlaneIntersectionType.Intersecting)
				{
					if (DataMgr.giantBodies[i].Velocity.X < 0f)
					{
						DataMgr.giantBodies[i].Velocity.X = 0f - DataMgr.giantBodies[i].Velocity.X;
					}
				}
				else if (DataMgr.frust.Right.Intersects(sphere2) == PlaneIntersectionType.Intersecting && DataMgr.giantBodies[i].Velocity.X > 0f)
				{
					DataMgr.giantBodies[i].Velocity.X = 0f - DataMgr.giantBodies[i].Velocity.X;
				}
				if (DataMgr.frust.Bottom.Intersects(sphere2) == PlaneIntersectionType.Intersecting)
				{
					if (DataMgr.giantBodies[i].Velocity.Y < 0f)
					{
						DataMgr.giantBodies[i].Velocity.Y = 0f - DataMgr.giantBodies[i].Velocity.Y;
					}
				}
				else if (DataMgr.frust.Top.Intersects(sphere2) == PlaneIntersectionType.Intersecting && DataMgr.giantBodies[i].Velocity.Y > 0f)
				{
					DataMgr.giantBodies[i].Velocity.Y = 0f - DataMgr.giantBodies[i].Velocity.Y;
				}
				if (DataMgr.giantBodies[i].Position.Z >= 25f)
				{
					if (DataMgr.giantBodies[i].Velocity.Z > 0f)
					{
						DataMgr.giantBodies[i].Velocity.Z = 0f - DataMgr.giantBodies[i].Velocity.Z;
					}
				}
				else if (DataMgr.giantBodies[i].Position.Z <= -20f && DataMgr.giantBodies[i].Velocity.Z < 0f)
				{
					DataMgr.giantBodies[i].Velocity.Z = 0f - DataMgr.giantBodies[i].Velocity.Z;
				}
				DataMgr.giantBodies[i].Integrate((float)gameTime.ElapsedGameTime.TotalSeconds);
				DataMgr.giantTransforms[i].World = Matrix.CreateScale(DataMgr.giantSize) * Matrix.CreateTranslation(DataMgr.giantBodies[i].Position);
			}
		}
		updateAsync1.BeginUpdateAsync(gameTime);
		updateAsync2.BeginUpdateAsync(gameTime);
		updateAsync3.BeginUpdateAsync(gameTime);
		updateAsync4.BeginUpdateAsync(gameTime);
		updateAsync1.EndUpdateAsync();
		updateAsync2.EndUpdateAsync();
		updateAsync3.EndUpdateAsync();
		updateAsync4.EndUpdateAsync();
		SoundMgr.Check();
		if (screenState == ScreenState.TransitionIn || screenState == ScreenState.TransitionOut || screenState == ScreenState.TransitionToBackground)
		{
			tracker.Update(gameTime);
		}
	}

	private void updateInput(GameTime gameTime)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Input.GameplayQuit((PlayerIndex)i) || Input.ButtonPR((PlayerIndex)i, Buttons.Start))
			{
				ControllingPlayer = (PlayerIndex)i;
				EngineManager.AddMenuScreen(ControllingPlayer, new MenuPopup());
				break;
			}
		}
	}

	public void Draw(GameTime gameTime)
	{
		device.Clear(Color.Blue);
		view = camera.View;
		projection = camera.Projection;
		BloomEffect.BeginNonBloom();
		spriteBatch.Begin();
		spriteBatch.Draw(background, screenBoundary, Color.White);
		spriteBatch.End();
		BloomEffect.BeginBloom();
		smallStars.DrawInstances(DrawTechnique.NoShading, ref DataMgr.starTransforms, camera);
		giantStars.DrawInstances(DrawTechnique.NoShading, ref DataMgr.giantTransforms, camera);
		BloomEffect.EndBloom();
		spriteBatch.Begin();
		if (Guide.IsTrialMode)
		{
			float x = (float)(Global.ScreenWidth / 2) - trialFnt.MeasureString(trialMsg).X / 2f;
			spriteBatch.DrawString(trialFnt, trialMsg, new Vector2(x, Global.ScreenHeight / 2), trialCol);
		}
		if (screenState != ScreenState.Active && screenState != ScreenState.Hidden)
		{
			tracker.Draw();
		}
		spriteBatch.End();
	}
}
