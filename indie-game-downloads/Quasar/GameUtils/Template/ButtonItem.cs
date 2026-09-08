using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GUI.Elements;
using Quasar.Meshes;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class ButtonItem : GroupControlElement
{
	private Button button;

	protected TextBoxMesh text;

	private Sized2DRectangleMesh bRectangle;

	public ButtonItem(Button button)
		: base(button)
	{
		bRectangle = new Sized2DRectangleMesh(button.LayoutData, new Vector3(0.2f));
		addMesh(bRectangle);
		text = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(button.LayoutData, 1f, HorizontalAlignment.Center, VerticalAlignment.Center), 50, useStringBuilder: true);
		addMesh(text);
		this.button = button;
		button.OnInteraction += OnInteraction;
		button.OnRemoved += OnRemoved;
	}

	private void OnRemoved(Control c)
	{
		if (Parent != null)
		{
			Parent.removeChild(this);
		}
	}

	private void OnInteraction(Button buttonPressed, PlayerIndex whoPressed)
	{
		GameTemplate.InteractionAudio.Start();
	}

	protected override void DoUpdate()
	{
		text.Text = button.Text;
		text.FirstMaterial.Diffuse = (button.Focused ? GameTemplate.MenuTextFocused : GameTemplate.MenuTextNormal);
		base.DoUpdate();
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
