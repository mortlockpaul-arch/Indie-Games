using AvatarFarmOnline.Scenes;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Render;
using Quasar.GameUtils.Sections;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Render;
using Quasar.Scenes;

namespace AvatarFarmOnline.Sections;

internal class BGSection : ExtraSection
{
	private static AvatarFarmOnline.Sections.BGSection instance;

	private Sized2DRectangleMesh fadeMesh;

	public static AvatarFarmOnline.Sections.BGSection Instance => instance;

	public static bool HasInstance => instance != null;

	static BGSection()
	{
		instance = new AvatarFarmOnline.Sections.BGSection();
		Engine.RegisterDisposeHandler(OnDispose);
	}

	private static void OnDispose()
	{
		instance.Dispose();
		instance = null;
	}

	public BGSection()
		: base(Priority.Background)
	{
	}

	public override void InitSection()
	{
		base.InitSection();
		RenderPass2D antialiasPass = AvatarFarmOnline.AvatarFarmOnlineGame.AntialiasPass;
		antialiasPass.Clear();
		antialiasPass.MustClearColor = true;
		antialiasPass.MustClearDepth = true;
		antialiasPass.addSource(AvatarFarmOnline.Scenes.BGScene.Instance);
	}

	protected override void initScenes()
	{
		AddScene(AvatarFarmOnline.Scenes.BGScene.Instance, isDefault: true);
	}

	protected override void initRenderProcesses()
	{
		RenderProcess renderProcess = new RenderProcess();
		renderProcess.addIntermediatePass(AvatarFarmOnline.AvatarFarmOnlineGame.AntialiasPass);
		AddExtraRenderProcess(renderProcess);
		FakeDOFRenderProcess fakeDOFRenderProcess = new FakeDOFRenderProcess(AvatarFarmOnline.AvatarFarmOnlineGame.AntialiasPass.RenderTarget);
		fakeDOFRenderProcess.Deviation = 3f;
		fakeDOFRenderProcess.BlurAmount = 2f;
		fakeDOFRenderProcess.BaseDistance = new Vector2(0.5f, 0.3f);
		fakeDOFRenderProcess.Aperture = new Vector2(3f);
		fakeDOFRenderProcess.SetRenderTarget(RenderPass2D.CreateRenderTarget(), setOwner: true);
		AddExtraRenderProcess(fakeDOFRenderProcess);
		BloomRenderProcess bloomRenderProcess = new BloomRenderProcess(fakeDOFRenderProcess.RenderTarget);
		bloomRenderProcess.BloomThreshold = 0.6f;
		bloomRenderProcess.BloomMultiplier = 1.7f;
		bloomRenderProcess.Deviation = 3f;
		bloomRenderProcess.BlurAmount = 4f;
		bloomRenderProcess.SetRenderTarget(RenderPass2D.CreateRenderTarget(), setOwner: true);
		AddExtraRenderProcess(bloomRenderProcess);
		AvatarFarmOnline.Scenes.ANFinalScene s = new AvatarFarmOnline.Scenes.ANFinalScene(AvatarFarmOnline.Scenes.BGScene.Instance.Camera, bloomRenderProcess.RenderTarget, AvatarFarmOnline.Scenes.BGScene.Instance.Season);
		AddScene(s, isDefault: false);
		mainRenderPass = new RenderPass2D(createRenderTarget: false);
		mainRenderPass.BackgroundColor = Color.Red;
		mainRenderPass.addSource(new PostprocessScene(bloomRenderProcess.RenderTarget));
		Scene2D scene2D = new Scene2D();
		AddScene(scene2D, isDefault: false);
		fadeMesh = new Sized2DRectangleMesh(Engine.GUISize, Vector3.Zero);
		fadeMesh.Alpha = 1f;
		scene2D.Add(new RenderItem(fadeMesh));
		base.MainRenderPass.addSource(scene2D);
	}

	public override void UpdateScenes()
	{
		base.UpdateScenes();
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void MainLoop()
	{
		if (AvatarFarmOnline.Scenes.BGScene.Instance.IsLoaded)
		{
			fadeMesh.Alpha = GameMath.Approximate(fadeMesh.Alpha, 0f, (float)Timer.DefaultTimer.LastInterval / 1125f);
		}
		base.MainLoop();
	}
}
