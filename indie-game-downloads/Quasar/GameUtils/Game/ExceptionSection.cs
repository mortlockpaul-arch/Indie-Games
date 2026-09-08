using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes.Text;
using Quasar.Scenes;

namespace Quasar.GameUtils.Game;

public class ExceptionSection : GameSection
{
	private string message;

	private string stackTrace;

	public ExceptionSection(Exception e)
		: base(9)
	{
		message = e.Message;
		stackTrace = e.StackTrace;
	}

	public ExceptionSection(string message, StackTrace stackTrace)
		: base(9)
	{
		this.message = message;
		if (stackTrace != null)
		{
			this.stackTrace = stackTrace.ToString();
		}
		else
		{
			this.stackTrace = "No more data available";
		}
	}

	protected override void initScenes()
	{
		Scene2D scene2D = new Scene2D();
		TextMesh textMesh = new TextMesh(GameTemplate.TitleFont, new TextDrawProperties(HorizontalAlignment.Center));
		textMesh.Offset = new Vector2(0f, Engine.GUIHeight * 0.4f);
		textMesh.Text = "CRITICAL ERROR";
		scene2D.Add(new RenderItem(textMesh));
		TextBoxMesh textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, Engine.GUIHeight * 0.25f), new Vector2(Engine.GUIWidth * 0.85f, Engine.GUIHeight * 0.15f), 1f, HorizontalAlignment.Center, VerticalAlignment.Center), 1024, useStringBuilder: false);
		textBoxMesh.Text = "An unexpected crash has been detected!";
		textBoxMesh.Text += '\n';
		textBoxMesh.Text += message;
		scene2D.Add(new RenderItem(textBoxMesh));
		TextBoxMesh textBoxMesh2 = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, (0f - Engine.GUIHeight) * 0.1f), new Vector2(Engine.GUIWidth * 0.85f, Engine.GUIHeight * 0.4f), 0.8f, HorizontalAlignment.Center, VerticalAlignment.Center), 1024, useStringBuilder: false);
		textBoxMesh2.Text = stackTrace;
		scene2D.Add(new RenderItem(textBoxMesh2));
		TextMesh textMesh2 = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Center));
		textMesh2.Text = "Please report this data to waaghman@milkstonestudios.com to help us solve the issue";
		textMesh2.Offset = new Vector2(0f, (0f - Engine.GUIHeight) * 0.38f);
		scene2D.Add(new RenderItem(textMesh2));
		AddScene(scene2D, isDefault: true);
	}

	protected override void initRenderProcesses()
	{
		base.initRenderProcesses();
		mainRenderPass.MustClearColor = true;
		mainRenderPass.MustClearDepth = true;
		mainRenderPass.BackgroundColor = Color.Black;
	}
}
