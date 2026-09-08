using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.Global;
using Quasar.Render;
using Quasar.Render.Sorters;

namespace Quasar.GameUtils.Sections;

public abstract class GUISection : GameSection
{
	private Layout layout;

	private static RenderTarget2D gammaRenderTarget;

	public Layout Layout => layout;

	private static RenderTarget2D GammaRenderTarget
	{
		get
		{
			if (gammaRenderTarget == null)
			{
				gammaRenderTarget = new RenderTarget2D(Engine.Device, Engine.BackBufferWidth, Engine.BackBufferHeight, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.DiscardContents);
				Engine.RegisterDisposeHandler(DisposeGamma);
			}
			return gammaRenderTarget;
		}
	}

	public GUISection(int id, string layout)
		: base(id)
	{
		this.layout = Layout.Load(layout);
	}

	public GUISection(int id, Layout layout)
		: base(id)
	{
		this.layout = layout;
	}

	protected override void initScenes()
	{
		AddScene(new LayoutScene(layout), isDefault: true);
	}

	private static void DisposeGamma()
	{
		gammaRenderTarget.Dispose();
		gammaRenderTarget = null;
	}

	protected override void initRenderProcesses()
	{
		_ = layout;
		SceneRenderData sceneRenderData = new SceneRenderData(base.DefaultScene);
		sceneRenderData.Sorter = SimpleSorter.Instance;
		RenderPass2D renderPass2D = new RenderPass2D(createRenderTarget: false);
		renderPass2D.addSource(sceneRenderData);
		renderPass2D.MustClearColor = true;
		renderPass2D.MustClearDepth = true;
		mainRenderPass = renderPass2D;
	}

	protected void RegisterButtonEvent(string id, Action<Button, PlayerIndex> handler)
	{
		if (layout.GetControl(id) is Button button)
		{
			button.OnInteraction += handler;
		}
	}

	protected void RegisterGroupCancelEvent(string id, Func<Group, PlayerIndex, bool> handler)
	{
		if (layout.GetControl(id) is Group obj)
		{
			obj.OnCancel += handler;
		}
	}

	protected void UnRegisterButtonEvent(string id, Action<Button, PlayerIndex> handler)
	{
		if (layout.GetControl(id) is Button button)
		{
			button.OnInteraction -= handler;
		}
	}

	public override void MainLoop()
	{
		layout.Update();
	}

	public override void Dispose()
	{
		layout.Dispose();
		layout = null;
		base.Dispose();
	}
}
