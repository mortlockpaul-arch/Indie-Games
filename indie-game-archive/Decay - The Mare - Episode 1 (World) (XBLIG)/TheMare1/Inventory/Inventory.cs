using System;
using Core;
using Core.Inventory;
using TheMare1.Inventory.Items;

namespace TheMare1.Inventory;

public class Inventory : Core.Inventory.Inventory
{
	public Inventory(Game game)
		: base(game)
	{
		try
		{
			m_preloaded_items.Add(new PaperTear(m_game, m_game.Content));
			m_preloaded_items.Add(new PaperArrow(m_game, m_game.Content));
			m_preloaded_items.Add(new PaperEye(m_game, m_game.Content));
			m_preloaded_items.Add(new PaperFlame(m_game, m_game.Content));
			m_preloaded_items.Add(new Room2Envelope(m_game));
			m_preloaded_items.Add(new Room2BloodyKey(m_game));
			m_preloaded_items.Add(new DreamRoom3SledgeTop(m_game));
			m_preloaded_items.Add(new DreamRoom3Document(m_game, m_game.Content));
			m_preloaded_items.Add(new DreamRoom4Document(m_game, m_game.Content));
			m_preloaded_items.Add(new Room5Document(m_game, m_game.Content));
			m_preloaded_items.Add(new DreamRoom6Photo(m_game));
			m_preloaded_items.Add(new DreamRoom6Paper(m_game, m_game.Content));
			m_preloaded_items.Add(new DreamRoom7Document(m_game, m_game.Content));
			m_preloaded_items.Add(new DreamRoom9SledgeHandle(m_game));
			m_preloaded_items.Add(new DreamRoom10PuppyKey(m_game));
			m_preloaded_items.Add(new DreamRoom13Drawings(m_game, m_game.Content));
			m_preloaded_items.Add(new Sledge(m_game));
			m_preloaded_items.Add(new Coin(m_game, m_game.Content));
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void AddItem(string id, bool loading)
	{
		if (id == "Coin")
		{
			HandleAddCoin(loading);
		}
		else
		{
			base.AddItem(id, loading);
		}
	}

	private void HandleAddCoin(bool loading)
	{
		try
		{
			Item item = GetItem("Coin");
			if (item == null)
			{
				return;
			}
			if (!FindItem("Coin"))
			{
				m_items.Add(item);
			}
			if (m_game.m_game_data.GetState("Coins") != "")
			{
				int num = int.Parse(m_game.m_game_data.GetState("Coins"));
				if (!loading)
				{
					num++;
				}
				m_game.m_game_data.SetState("Coins", num.ToString());
			}
			else
			{
				m_game.m_game_data.SetState("Coins", "1");
			}
			if (m_game.m_game_data.GetState("Coins") != "1")
			{
				item.m_name = m_game.m_language.GetString("Coins") + " (" + m_game.m_game_data.GetState("Coins") + "/5)";
			}
			else
			{
				item.m_name = m_game.m_language.GetString("Coin") + " (" + m_game.m_game_data.GetState("Coins") + "/5)";
			}
			item.m_desc = item.m_name;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
