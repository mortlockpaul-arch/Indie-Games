using System;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GUI;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GUI.Elements;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Template;

public class MenuButtonItem : GroupControlElement
{
	private Button button;

	protected TextMesh text;

	private RenderItem textItem;

	private BorderedRectangle bgMesh;

	private float defaultSize;

	public Element Element => this;

	public MenuButtonItem(Button button)
		: base(button)
	{
		bgMesh = new BorderedRectangle(TextureManager.Textures["GUI/WoodTileTex"], new Vector2(button.Size.X, button.Size.Y), new float[4] { 32f, 0f, 32f, 0f });
		defaultSize = button.Size.X;
		bgMesh.FirstMaterial.Textures.Add(TextureManager.Textures["GUI/WoodTileTex"]);
		bgMesh.Offset = new Vector2(0.5f);
		bgMesh.Shader = ShaderManager.Shaders["GUI"];
		addMesh(bgMesh);
		text = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.MenuFont, new TextDrawProperties(HorizontalAlignment.Right), 30, useStringBuilder: true);
		text.Offset = new Vector2(button.Size.X * 0.5f - 30f, 9f);
		text.Text = button.Text;
		text.Scale = 1f;
		textItem = new RenderItem();
		addChild(textItem);
		textItem.addMesh(text);
		this.button = button;
		button.OnInteraction += OnInteraction;
		button.OnRemoved += OnRemoved;
		updateBg(AvatarFarmOnline.Template.ExtendedGameTemplate.MenuFont.MeasureString(button.Text));
		text.Text = button.Text;
	}

	private void OnRemoved(Control c)
	{
		if (Parent != null)
		{
			Parent.removeChild(this);
		}
	}

	protected virtual void OnInteraction(Button buttonPressed, PlayerIndex whoPressed)
	{
		GameTemplate.InteractionAudio.Start();
	}

	protected override void DoUpdate()
	{
		if (button.Focused)
		{
			text.FirstMaterial.Diffuse = Vector3.Lerp(text.FirstMaterial.Diffuse, GameMath.RGBToVector(byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.15f);
			bgMesh.FirstMaterial.Textures[1] = TextureManager.Textures["GUI/WoodTileTex"];
			Transform.Scale = new Vector3(1.2f + 0.025f * (float)Math.Sin(3f * Timer.DefaultTimer.TotalTimeSeconds));
		}
		else
		{
			text.FirstMaterial.Diffuse = Vector3.Lerp(text.FirstMaterial.Diffuse, GameMath.RGBToVector(150, 220, 60), 0.15f);
			bgMesh.FirstMaterial.Textures[1] = TextureManager.Textures["GUI/WoodLightTileTex"];
			Transform.Scale = Vector3.Lerp(Transform.Scale, Vector3.One, 0.15f);
		}
		base.DoUpdate();
	}

	protected void updateBg(float textWidth)
	{
		textWidth += 80f;
		if (textWidth < button.Size.X)
		{
			textWidth = button.Size.X;
		}
		bgMesh.Size = new Vector2(textWidth, bgMesh.Size.Y);
		bgMesh.Offset = new Vector2(button.Size.X * 0.5f + 0.5f - textWidth * 0.5f, 0.5f);
	}

	public override void Dispose()
	{
		if (button != null)
		{
			button.OnInteraction -= OnInteraction;
			button.OnRemoved -= OnRemoved;
		}
		button = null;
		base.Dispose();
	}
}
