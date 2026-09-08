using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Textures;

namespace Quasar.GameUtils.Template;

public class ClickButtonNode : RenderItem
{
	private ClickButton button;

	private TextMesh text;

	private BorderedRectangle bRectangle;

	public ClickButtonNode(ClickButton button)
	{
		bRectangle = new BorderedRectangle(TextureManager.Textures["GUI/ClickButton"], button.Size, new float[4] { 17f, 17f, 17f, 22f });
		addMesh(bRectangle);
		text = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Center), 50, useStringBuilder: false);
		addMesh(text);
		transform.Translation = new Vector3(button.Position, 0f);
		this.button = button;
		button.OnClick += OnInteraction;
	}

	private void OnInteraction(ClickButton buttonPressed)
	{
		GameTemplate.InteractionAudio.Start();
	}

	protected override void DoUpdate()
	{
		text.Text = button.Text;
		base.DoUpdate();
	}

	protected override void DoRender()
	{
		base.DoRender();
	}

	public override void Dispose()
	{
		if (button != null)
		{
			button.OnClick -= OnInteraction;
		}
		button = null;
		base.Dispose();
	}
}
