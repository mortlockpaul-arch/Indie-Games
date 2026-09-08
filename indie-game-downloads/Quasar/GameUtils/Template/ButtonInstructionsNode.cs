using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GUI.Controls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class ButtonInstructionsNode : RenderItem
{
	private const float ROW_HEIGHT = 25f;

	private const float TOTAL_WIDTH = 780f;

	private ButtonInstructions instructions;

	public ButtonInstructionsNode(ButtonInstructions instructions)
	{
		transform.Translation = new Vector3(0f, -0.4f * Engine.GUIHeight, 0f);
		this.instructions = instructions;
		SetInstructions(instructions.Instructions);
		instructions.OnChange += OnChange;
	}

	private void OnChange(ButtonInstructions instructions)
	{
		SetInstructions(instructions.Instructions);
	}

	public ButtonInstructionsNode()
	{
		transform.Translation = new Vector3(0f, -0.4f * Engine.GUIHeight, 0f);
	}

	public void SetInstructions(KeyValuePair<ButtonInstructions.ButtonInstructionsItem, string> button)
	{
		clearMeshes();
		AddInstructions(button, 0, 1);
	}

	public void SetInstructions(KeyValuePair<InputManager.MenuInputCodes, string> button)
	{
		clearMeshes();
		AddInstructions(new KeyValuePair<ButtonInstructions.ButtonInstructionsItem, string>(new ButtonInstructions.ButtonInstructionsItem(button.Key), button.Value), 0, 1);
	}

	public void SetInstructions(List<KeyValuePair<ButtonInstructions.ButtonInstructionsItem, string>> buttons)
	{
		clearMeshes();
		for (int i = 0; i < buttons.Count; i++)
		{
			AddInstructions(buttons[i], i, buttons.Count);
		}
	}

	public void SetInstructions(List<KeyValuePair<InputManager.MenuInputCodes, string>> buttons)
	{
		clearMeshes();
		for (int i = 0; i < buttons.Count; i++)
		{
			AddInstructions(new KeyValuePair<ButtonInstructions.ButtonInstructionsItem, string>(new ButtonInstructions.ButtonInstructionsItem(buttons[i].Key), buttons[i].Value), i, buttons.Count);
		}
	}

	private void AddInstructions(KeyValuePair<ButtonInstructions.ButtonInstructionsItem, string> button, int index, int totalCount)
	{
		int num = (totalCount - 1) / 4 + 1;
		if (num > 2)
		{
			return;
		}
		int num2 = index / 4;
		float num3 = ((num != 2) ? 0f : ((num2 == 0) ? 12.5f : (-25f)));
		int num4 = 0;
		int num5 = 0;
		if (num2 == num - 1)
		{
			num5 = totalCount % 4;
			if (num5 == 0)
			{
				num5 = 4;
			}
		}
		else
		{
			num5 = 4;
		}
		num4 = index % 4;
		float num6 = ((num5 != 1) ? GameMath.Interpolate(-780f * ((float)num5 / 4f) * 0.5f, 780f * ((float)num5 / 4f) * 0.5f, (float)num4 / (float)(num5 - 1)) : 0f);
		num6 = (float)Math.Floor(num6) + 0.5f;
		num3 = (float)Math.Floor(num3) + 0.5f;
		TextMesh textMesh = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(num6, num3), HorizontalAlignment.Center), 40, useStringBuilder: true);
		textMesh.StringBuilder.Append(InputManager.GetInputGlyph(button.Key.button));
		if (button.Key.isPaired)
		{
			textMesh.StringBuilder.Append(InputManager.GetInputGlyph(button.Key.button2));
		}
		textMesh.StringBuilder.Append(' ');
		textMesh.StringBuilder.Append(button.Value);
		addMesh(textMesh);
	}

	protected override void DoUpdate()
	{
		if (instructions != null)
		{
			Visible = instructions.Visible;
		}
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (instructions != null)
		{
			instructions.OnChange -= OnChange;
		}
		base.Dispose();
	}
}
