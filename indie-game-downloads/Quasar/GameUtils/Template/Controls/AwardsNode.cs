using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Awards;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class AwardsNode : Element
{
	private Awards awards;

	private List<Quasar.GameUtils.Template.Controls.AwardsNodeItem> items = new List<Quasar.GameUtils.Template.Controls.AwardsNodeItem>();

	private TextMesh currentPageText;

	public AwardsNode(Awards awards)
	{
		this.awards = awards;
		awards.OnChange += OnChange;
		RenderItem renderItem = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(690f, 500f));
		renderItem.addMesh(m);
		currentPageText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(0f, -200f), HorizontalAlignment.Center), 40, useStringBuilder: true);
		currentPageText.FirstMaterial.Diffuse = Vector3.One;
		renderItem.addMesh(currentPageText);
		addChild(renderItem);
		for (int i = 0; i < awards.ItemsPerPage; i++)
		{
			Quasar.GameUtils.Template.Controls.AwardsNodeItem awardsNodeItem = new Quasar.GameUtils.Template.Controls.AwardsNodeItem();
			awardsNodeItem.Transform.Translation = new Vector3(0f, 160 - 140 * i, 0f);
			items.Add(awardsNodeItem);
			addChild(awardsNodeItem);
		}
		Transform.Translation = new Vector3(0f, -25f, 0f);
	}

	private void OnChange()
	{
		GameTemplate.SelectorAudio.Start();
	}

	protected override void DoUpdate()
	{
		List<AwardProgress> currentItems = awards.GetCurrentItems();
		int i;
		for (i = 0; i < currentItems.Count; i++)
		{
			items[i].SetData(currentItems[i]);
		}
		for (; i < awards.ItemsPerPage; i++)
		{
			items[i].Reset();
		}
		currentPageText.StringBuilder.Length = 0;
		currentPageText.StringBuilder.AppendNumber(awards.CurrentPage + 1);
		currentPageText.StringBuilder.Append('/');
		currentPageText.StringBuilder.AppendNumber(awards.NumPages);
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (awards != null)
		{
			awards.OnChange -= OnChange;
			awards = null;
		}
		base.Dispose();
	}
}
