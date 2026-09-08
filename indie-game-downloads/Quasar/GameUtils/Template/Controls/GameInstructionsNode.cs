using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class GameInstructionsNode : Element
{
	private GameInstructions instructions;

	private List<Quasar.GameUtils.Template.Controls.GameInstructionsNodeItem> items = new List<Quasar.GameUtils.Template.Controls.GameInstructionsNodeItem>();

	private TextMesh currentPageText;

	public GameInstructionsNode(GameInstructions instructions)
	{
		this.instructions = instructions;
		instructions.OnChange += OnChange;
		RenderItem renderItem = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(690f, 500f));
		renderItem.addMesh(m);
		currentPageText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(0f, -200f), HorizontalAlignment.Center), 40, useStringBuilder: true);
		currentPageText.FirstMaterial.Diffuse = Vector3.One;
		renderItem.addMesh(currentPageText);
		addChild(renderItem);
		for (int i = 0; i < instructions.ItemsPerPage; i++)
		{
			Quasar.GameUtils.Template.Controls.GameInstructionsNodeItem gameInstructionsNodeItem = new Quasar.GameUtils.Template.Controls.GameInstructionsNodeItem(i % 2 == 0);
			gameInstructionsNodeItem.Transform.Translation = new Vector3(0f, 160 - 140 * i, 0f);
			items.Add(gameInstructionsNodeItem);
			addChild(gameInstructionsNodeItem);
		}
		Transform.Translation = new Vector3(0f, -25f, 0f);
	}

	private void OnChange()
	{
		GameTemplate.SelectorAudio.Start();
	}

	protected override void DoUpdate()
	{
		List<GameInstructionsItem> currentItems = instructions.GetCurrentItems();
		int i;
		for (i = 0; i < currentItems.Count; i++)
		{
			items[i].SetData(currentItems[i]);
		}
		for (; i < instructions.ItemsPerPage; i++)
		{
			items[i].Reset();
		}
		currentPageText.StringBuilder.Length = 0;
		currentPageText.StringBuilder.AppendNumber(instructions.CurrentPage + 1);
		currentPageText.StringBuilder.Append('/');
		currentPageText.StringBuilder.AppendNumber(instructions.NumPages);
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (instructions != null)
		{
			instructions.OnChange -= OnChange;
			instructions = null;
		}
		base.Dispose();
	}
}
