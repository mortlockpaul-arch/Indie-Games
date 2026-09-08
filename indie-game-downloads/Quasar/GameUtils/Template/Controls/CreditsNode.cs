using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class CreditsNode : Element
{
	private Credits credits;

	private List<Quasar.GameUtils.Template.Controls.CreditsNodeItem> items = new List<Quasar.GameUtils.Template.Controls.CreditsNodeItem>();

	private TextMesh currentPageText;

	public CreditsNode(Credits credits)
	{
		this.credits = credits;
		credits.OnChange += OnChange;
		float value = (500f + (Engine.GUIHeight - 720f) * 0.5f) / 500f;
		RenderItem renderItem = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(690f, 500f));
		renderItem.addMesh(m);
		currentPageText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(0f, -200f), HorizontalAlignment.Center), 40, useStringBuilder: true);
		currentPageText.FirstMaterial.Diffuse = Vector3.One;
		renderItem.addMesh(currentPageText);
		addChild(renderItem);
		for (int i = 0; i < credits.ItemsPerPage; i++)
		{
			Quasar.GameUtils.Template.Controls.CreditsNodeItem creditsNodeItem = new Quasar.GameUtils.Template.Controls.CreditsNodeItem();
			creditsNodeItem.Transform.Translation = new Vector3(0f, 160 - 140 * i, 0f);
			items.Add(creditsNodeItem);
			addChild(creditsNodeItem);
		}
		Transform.Translation = new Vector3(0.5f, -24.5f, 0f);
		Transform.Scale = new Vector3(value);
	}

	private void OnChange()
	{
		GameTemplate.SelectorAudio.Start();
	}

	protected override void DoUpdate()
	{
		List<CreditsItem> currentItems = credits.GetCurrentItems();
		int i;
		for (i = 0; i < currentItems.Count; i++)
		{
			items[i].SetData(currentItems[i]);
		}
		for (; i < credits.ItemsPerPage; i++)
		{
			items[i].Reset();
		}
		currentPageText.StringBuilder.Length = 0;
		if (credits.NumPages > 1)
		{
			currentPageText.StringBuilder.AppendNumber(credits.CurrentPage + 1);
			currentPageText.StringBuilder.Append('/');
			currentPageText.StringBuilder.AppendNumber(credits.NumPages);
		}
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (credits != null)
		{
			credits.OnChange -= OnChange;
			credits = null;
		}
		base.Dispose();
	}
}
