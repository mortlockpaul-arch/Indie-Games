using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Scores;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Meshes;
using Quasar.GameUtils.Scores;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Template.Controls;

internal class ProHighScoresNode : RenderItem
{
	private ProHighscores<AvatarFarmOnline.Scores.GameHighscore> selector;

	private List<AvatarFarmOnline.Template.Controls.ProHighScoreEntry> entries = new List<AvatarFarmOnline.Template.Controls.ProHighScoreEntry>();

	private TextMesh filterText;

	private TextMesh periodText;

	public ProHighScoresNode(ProHighscores<AvatarFarmOnline.Scores.GameHighscore> selector)
	{
		Element element = new Element();
		element.Transform.Translation = new Vector3(0f, -235f, 0f);
		addChild(element);
		float value = (490f + (Engine.GUIHeight - 720f) * 0.5f) / 490f;
		RenderItem renderItem = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(850f, 490f));
		HUDDetailRectangle m2 = new HUDDetailRectangle(new Vector2(810f, 400f))
		{
			Offset = new Vector2(0f, 25f)
		};
		HUDDetailRectangle hUDDetailRectangle = new HUDDetailRectangle(new Vector2(810f, 58f))
		{
			Offset = new Vector2(0f, -201f)
		};
		renderItem.addMesh(m);
		renderItem.addMesh(m2);
		for (int i = 0; i < selector.ScoresPerPage; i++)
		{
			AvatarFarmOnline.Template.Controls.ProHighScoreEntry item = new AvatarFarmOnline.Template.Controls.ProHighScoreEntry
			{
				pos = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-395f, 185 - 24 * i), HorizontalAlignment.Left), 8, useStringBuilder: true),
				gamertag = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-340f, 185 - 24 * i), HorizontalAlignment.Left), 40, useStringBuilder: false),
				cash = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(300f, 185 - 24 * i), HorizontalAlignment.Right), 40, useStringBuilder: true),
				time = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-20f, 185 - 24 * i), HorizontalAlignment.Right), 40, useStringBuilder: true),
				level = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(390f, 185 - 24 * i), HorizontalAlignment.Right), 40, useStringBuilder: true),
				coins = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(150f, 185 - 24 * i), HorizontalAlignment.Right), 40, useStringBuilder: true)
			};
			TextMesh pos = item.pos;
			TextMesh time = item.time;
			TextMesh cash = item.cash;
			TextMesh coins = item.coins;
			TextMesh gamertag = item.gamertag;
			float num = (item.level.Scale = 0.625f);
			float num3 = (gamertag.Scale = num);
			float num5 = (coins.Scale = num3);
			float num7 = (cash.Scale = num5);
			float scale = (time.Scale = num7);
			pos.Scale = scale;
			entries.Add(item);
			renderItem.addMesh(item.pos);
			renderItem.addMesh(item.cash);
			renderItem.addMesh(item.coins);
			renderItem.addMesh(item.time);
			renderItem.addMesh(item.gamertag);
			renderItem.addMesh(item.level);
		}
		filterText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-400f, -180f), 0.75f, HorizontalAlignment.Left), 20, useStringBuilder: true);
		renderItem.addMesh(filterText);
		periodText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-400f, -212f), 0.75f, HorizontalAlignment.Left), 20, useStringBuilder: true);
		renderItem.addMesh(periodText);
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-150f, -180f), 0.75f, HorizontalAlignment.Left), 20, useStringBuilder: false)
		{
			Text = string.Format("{0}{1}", InputManager.GetInputGlyph(InputManager.MenuInputCodes.Interact), "MY_BEST_SCORE".Translate())
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-150f, -212f), 0.75f, HorizontalAlignment.Left), 20, useStringBuilder: false)
		{
			Text = string.Format("{0}{1}", InputManager.GetInputGlyph(InputManager.MenuInputCodes.Start), "CHALLENGE".Translate())
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(400f, -196f), 0.75f, HorizontalAlignment.Right), 40, useStringBuilder: false)
		{
			Text = "HIGH_SCORES_INSTRUCTIONS".Translate()
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(250f, 215f), HorizontalAlignment.Right), 40, useStringBuilder: false)
		{
			Diffuse = GameTemplate.DialogTitleColor,
			Text = LanguageManager.Texts["MONEY"]
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-20f, 215f), HorizontalAlignment.Right), 40, useStringBuilder: false)
		{
			Diffuse = GameTemplate.DialogTitleColor,
			Text = LanguageManager.Texts["TIME"]
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(390f, 215f), HorizontalAlignment.Right), 40, useStringBuilder: false)
		{
			Diffuse = GameTemplate.DialogTitleColor,
			Text = LanguageManager.Texts["LEVEL"]
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-395f, 215f), HorizontalAlignment.Left), 40, useStringBuilder: false)
		{
			Diffuse = GameTemplate.DialogTitleColor,
			Text = "#"
		});
		renderItem.addMesh(new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-340f, 215f), HorizontalAlignment.Left), 40, useStringBuilder: false)
		{
			Diffuse = GameTemplate.DialogTitleColor,
			Text = LanguageManager.Texts["NAME"]
		});
		renderItem.Transform.Translation = new Vector3(0f, -30f, 0f);
		renderItem.Transform.Scale = new Vector3(value);
		addChild(renderItem);
		this.selector = selector;
	}

	protected override void DoUpdate()
	{
		UpdateData();
		filterText.StringBuilder.Length = 0;
		filterText.StringBuilder.Append(InputManager.GetInputGlyph(InputManager.MenuInputCodes.Secondary));
		filterText.StringBuilder.Append("SCORE_LOCATION".Translate());
		filterText.StringBuilder.Append(' ');
		switch (selector.ScoreLocation)
		{
		case ProHighscores<AvatarFarmOnline.Scores.GameHighscore>.ScoreLocations.Local:
			filterText.StringBuilder.Append("SCORE_LOCATION_LOCAL".Translate());
			break;
		case ProHighscores<AvatarFarmOnline.Scores.GameHighscore>.ScoreLocations.Global:
			filterText.StringBuilder.Append("SCORE_LOCATION_GLOBAL".Translate());
			break;
		case ProHighscores<AvatarFarmOnline.Scores.GameHighscore>.ScoreLocations.Friends:
			filterText.StringBuilder.Append("SCORE_LOCATION_FRIENDS".Translate());
			break;
		}
		periodText.StringBuilder.Length = 0;
		periodText.StringBuilder.Append(InputManager.GetInputGlyph(InputManager.MenuInputCodes.Terciary));
		periodText.StringBuilder.Append("SCORE_PERIOD".Translate());
		periodText.StringBuilder.Append(' ');
		switch (selector.ScorePeriod)
		{
		case ScorePeriod.AllTime:
			periodText.StringBuilder.Append("SCORE_PERIOD_ALL_TIME".Translate());
			break;
		case ScorePeriod.Monthly:
			periodText.StringBuilder.Append("SCORE_PERIOD_MONTHLY".Translate());
			break;
		case ScorePeriod.Weekly:
			periodText.StringBuilder.Append("SCORE_PERIOD_WEEKLY".Translate());
			break;
		case ScorePeriod.Daily:
			periodText.StringBuilder.Append("SCORE_PERIOD_DAILY".Translate());
			break;
		}
		base.DoUpdate();
	}

	private void UpdateData()
	{
		List<AvatarFarmOnline.Scores.GameHighscore> list = selector.GetEntries();
		int i;
		for (i = 0; i < Math.Min(entries.Count, list.Count); i++)
		{
			AvatarFarmOnline.Template.Controls.ProHighScoreEntry proHighScoreEntry = entries[i];
			AvatarFarmOnline.Scores.GameHighscore gameHighscore = list[i];
			if (gameHighscore == null)
			{
				continue;
			}
			proHighScoreEntry.pos.StringBuilder.Length = 0;
			proHighScoreEntry.pos.StringBuilder.AppendNumber(selector.BaseIndex + i + 1);
			proHighScoreEntry.pos.StringBuilder.Append('.');
			proHighScoreEntry.gamertag.Text = gameHighscore.Gamer;
			proHighScoreEntry.level.StringBuilder.Length = 0;
			proHighScoreEntry.level.StringBuilder.AppendNumber(gameHighscore.Level);
			proHighScoreEntry.cash.StringBuilder.Length = 0;
			proHighScoreEntry.cash.StringBuilder.AppendNumber(gameHighscore.Cash, AppendNumberOptions.NumberGroup);
			proHighScoreEntry.cash.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CashChar);
			proHighScoreEntry.coins.StringBuilder.Length = 0;
			proHighScoreEntry.coins.StringBuilder.AppendNumber(gameHighscore.Coins, AppendNumberOptions.NumberGroup);
			proHighScoreEntry.coins.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CoinChar);
			proHighScoreEntry.time.StringBuilder.Length = 0;
			int ticks = gameHighscore.Ticks;
			int num = ticks / 86400;
			ticks -= num * 86400;
			int num2 = ticks / 3600;
			ticks -= num2 * 3600;
			int num3 = ticks / 60;
			bool flag = false;
			if (num > 0)
			{
				proHighScoreEntry.time.StringBuilder.AppendNumber(num);
				proHighScoreEntry.time.StringBuilder.Append("d");
				flag = true;
			}
			if (num2 > 0)
			{
				if (flag)
				{
					proHighScoreEntry.time.StringBuilder.Append(" ");
				}
				proHighScoreEntry.time.StringBuilder.AppendNumber(num2);
				proHighScoreEntry.time.StringBuilder.Append("h");
				flag = true;
			}
			if (num3 > 0)
			{
				if (flag)
				{
					proHighScoreEntry.time.StringBuilder.Append(" ");
				}
				proHighScoreEntry.time.StringBuilder.AppendNumber(num3);
				proHighScoreEntry.time.StringBuilder.Append("m");
				flag = true;
			}
			TextMesh pos = proHighScoreEntry.pos;
			TextMesh level = proHighScoreEntry.level;
			TextMesh cash = proHighScoreEntry.cash;
			TextMesh coins = proHighScoreEntry.coins;
			TextMesh time = proHighScoreEntry.time;
			Vector3 vector = (proHighScoreEntry.gamertag.Diffuse = ((selector.ScoreLocation != ProHighscores<AvatarFarmOnline.Scores.GameHighscore>.ScoreLocations.Local && gameHighscore.IsLocal) ? GameTemplate.DialogTitleColor : Vector3.One));
			Vector3 vector3 = (time.Diffuse = vector);
			Vector3 vector5 = (coins.Diffuse = vector3);
			Vector3 vector7 = (cash.Diffuse = vector5);
			Vector3 diffuse = (level.Diffuse = vector7);
			pos.Diffuse = diffuse;
		}
		for (; i < entries.Count; i++)
		{
			AvatarFarmOnline.Template.Controls.ProHighScoreEntry proHighScoreEntry2 = entries[i];
			proHighScoreEntry2.pos.Text = "";
			proHighScoreEntry2.gamertag.Text = "";
			proHighScoreEntry2.coins.Text = "";
			proHighScoreEntry2.cash.Text = "";
			proHighScoreEntry2.time.Text = "";
			proHighScoreEntry2.level.Text = "";
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
