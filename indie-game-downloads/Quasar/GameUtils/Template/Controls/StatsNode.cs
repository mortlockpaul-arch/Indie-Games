using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.GameUtils.Stats;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class StatsNode : Element
{
	private Stats stats;

	private List<Quasar.GameUtils.Template.Controls.StatsNodeItem> items = new List<Quasar.GameUtils.Template.Controls.StatsNodeItem>();

	private TextMesh currentPageText;

	public StatsNode(Stats stats)
	{
		this.stats = stats;
		stats.OnChange += OnChange;
		float value = (500f + (Engine.GUIHeight - 720f) * 0.5f) / 500f;
		RenderItem renderItem = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(690f, 500f));
		renderItem.addMesh(m);
		renderItem.addMesh(new HUDDetailRectangle(new Vector2(650f, 420f))
		{
			Offset = new Vector2(0f, 20f)
		});
		currentPageText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(0f, -200f), HorizontalAlignment.Center), 40, useStringBuilder: true);
		currentPageText.FirstMaterial.Diffuse = Vector3.One;
		renderItem.addMesh(currentPageText);
		addChild(renderItem);
		for (int i = 0; i < stats.ItemsPerPage; i++)
		{
			Quasar.GameUtils.Template.Controls.StatsNodeItem statsNodeItem = new Quasar.GameUtils.Template.Controls.StatsNodeItem();
			statsNodeItem.Transform.Translation = new Vector3(0f, 210 - 40 * i, 0f);
			items.Add(statsNodeItem);
			addChild(statsNodeItem);
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
		List<StatProgress> currentItems = stats.GetCurrentItems();
		int i;
		for (i = 0; i < currentItems.Count; i++)
		{
			items[i].SetData(currentItems[i]);
		}
		for (; i < stats.ItemsPerPage; i++)
		{
			items[i].Reset();
		}
		currentPageText.StringBuilder.Length = 0;
		currentPageText.StringBuilder.AppendNumber(stats.CurrentPage + 1);
		currentPageText.StringBuilder.Append('/');
		currentPageText.StringBuilder.AppendNumber(stats.NumPages);
		base.DoUpdate();
	}

	public override void Dispose()
	{
		if (stats != null)
		{
			stats.OnChange -= OnChange;
			stats = null;
		}
		base.Dispose();
	}
}
