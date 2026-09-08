using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class DLCNode : Element
{
	private DLC dlc;

	private List<Quasar.GameUtils.Template.Controls.DLCNodeItem> items = new List<Quasar.GameUtils.Template.Controls.DLCNodeItem>();

	private TextMesh currentPageText;

	private TextBoxMesh introText;

	public DLCNode(DLC dlc)
	{
		this.dlc = dlc;
		dlc.OnChange += OnChange;
		RenderItem renderItem = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(890f, 500f));
		renderItem.addMesh(m);
		introText = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, 148f), new Vector2(780f, 192f), 1f, HorizontalAlignment.Center, VerticalAlignment.Center), 256, useStringBuilder: false);
		introText.Text = "DLC_INTRO_TEXT".Translate();
		renderItem.addMesh(introText);
		currentPageText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(0f, -190f), HorizontalAlignment.Center), 40, useStringBuilder: true);
		currentPageText.FirstMaterial.Diffuse = Vector3.One;
		renderItem.addMesh(currentPageText);
		addChild(renderItem);
		for (int i = 0; i < dlc.ItemsPerPage; i++)
		{
			Quasar.GameUtils.Template.Controls.DLCNodeItem dLCNodeItem = new Quasar.GameUtils.Template.Controls.DLCNodeItem();
			dLCNodeItem.Transform.Translation = new Vector3(0f, 55 - 95 * i, 0f);
			items.Add(dLCNodeItem);
			addChild(dLCNodeItem);
		}
		Transform.Translation = new Vector3(0f, -25f, 0f);
	}

	private void OnChange()
	{
		GameTemplate.SelectorAudio.Start();
	}

	protected override void DoUpdate()
	{
		List<DLC.DLCItem> currentItems = dlc.GetCurrentItems();
		int i;
		for (i = 0; i < currentItems.Count; i++)
		{
			items[i].SetData(currentItems[i]);
		}
		for (; i < dlc.ItemsPerPage; i++)
		{
			items[i].Reset();
		}
		currentPageText.StringBuilder.Length = 0;
		currentPageText.StringBuilder.AppendNumber(dlc.CurrentPage + 1);
		currentPageText.StringBuilder.Append('/');
		currentPageText.StringBuilder.AppendNumber(dlc.NumPages);
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (dlc != null)
		{
			dlc.OnChange -= OnChange;
			dlc = null;
		}
		base.Dispose();
	}
}
