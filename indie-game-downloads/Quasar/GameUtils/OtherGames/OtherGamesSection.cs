using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.ContentPipeline;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Render;
using Quasar.Scenes;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.GameUtils.OtherGames;

public class OtherGamesSection : GameSection
{
	private int previousSectionId;

	private Quasar.GameUtils.OtherGames.OtherGamesScene otherGamesScene;

	protected OtherGames otherGames;

	private Quasar.GameUtils.OtherGames.OtherGamesHUDScene hudScene;

	private RenderPass2D postprocessRenderPass;

	public OtherGamesSection(int previousSectionId, RenderPass2D postprocessRenderPass, string currentGame)
		: base(6)
	{
		this.previousSectionId = previousSectionId;
		this.postprocessRenderPass = postprocessRenderPass;
		otherGames = new OtherGames();
		otherGames.OnCancel += otherGames_OnCancel;
		if (!DirectoryManager.FileExists("OtherGames"))
		{
			return;
		}
		XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("OtherGames");
		XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
		foreach (XElement item in xDocument.Root.Elements())
		{
			OtherGames.OtherGamesItem otherGamesItem = ParseOtherGamesItem(item);
			if (otherGamesItem.Name != currentGame)
			{
				otherGames.AddItem(otherGamesItem);
			}
		}
	}

	private OtherGames.OtherGamesItem ParseOtherGamesItem(XElement xe)
	{
		string attribute = xe.GetAttribute("name");
		string description = xe.GetAttribute("description").Translate();
		int month = xe.ParseIntAttribute("month");
		int year = xe.ParseIntAttribute("year");
		int price = xe.ParseIntAttribute("price");
		string attribute2 = xe.GetAttribute("id");
		string boxart = attribute2 + "_BA";
		string qr = attribute2 + "_QR";
		Vector3 color = xe.ParseColor3Attribute("color");
		List<string> list = new List<string>(4);
		for (int i = 0; i < 4; i++)
		{
			list.Add(attribute2 + "_" + i);
		}
		return new OtherGames.OtherGamesItem(attribute, description, month, year, price, boxart, qr, list, color);
	}

	private void otherGames_OnCancel()
	{
		Back();
	}

	protected override void initScenes()
	{
		otherGamesScene = new Quasar.GameUtils.OtherGames.OtherGamesScene(otherGames);
		AddScene(otherGamesScene, isDefault: true);
		hudScene = new Quasar.GameUtils.OtherGames.OtherGamesHUDScene(otherGames);
		AddScene(hudScene, isDefault: false);
	}

	protected override void initRenderProcesses()
	{
		postprocessRenderPass.Clear();
		postprocessRenderPass.MustClearColor = true;
		postprocessRenderPass.MustClearDepth = true;
		postprocessRenderPass.BackgroundColor = Color.Black;
		PostprocessScene postprocessScene = new PostprocessScene(TextureManager.Textures["OtherGames/BGPattern"], ShaderManager.Shaders["OtherGamesBG"]);
		postprocessScene.PostProcess.Diffuse = GameMath.RGBToVector(byte.MaxValue, 174, 0);
		postprocessRenderPass.addSource(postprocessScene);
		postprocessRenderPass.addSource(otherGamesScene);
		AddExtraRenderProcess(new RenderProcess(postprocessRenderPass));
		mainRenderPass = new RenderPass2D(createRenderTarget: false);
		base.MainRenderPass.addSource(new PostprocessScene(postprocessRenderPass.RenderTarget, ShaderManager.Shaders["XboxGamma"]));
		base.MainRenderPass.addSource(hudScene);
	}

	private void Back()
	{
		GameTemplate.LayoutCancelAudio.Start();
		((BaseGame)Engine.Game).NextGameSectionId = previousSectionId;
	}

	public override void MainLoop()
	{
		Quasar.GameUtils.Player.Player.SetPresence(GamerPresenceMode.AtMenu);
		if (InputManager.MenuBack())
		{
			Back();
		}
		otherGames.Update();
		base.MainLoop();
	}
}
