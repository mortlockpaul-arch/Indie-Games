using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.GameUtils.Logic.Mode.Campaign;

public abstract class BaseSinglePlayerCampaignGameSetup : BaseCampaignGameSetup
{
	private List<PlayerIndex> indices = new List<PlayerIndex>();

	public override List<PlayerIndex> PlayerIndices => indices;

	public PlayerIndex PlayerIndex
	{
		get
		{
			if (indices.Count == 0)
			{
				return PlayerIndex.One;
			}
			return indices[0];
		}
	}

	public BaseSinglePlayerCampaignGameSetup(PlayerIndex index, int gameMode)
		: base(gameMode)
	{
		SetPlayer(index);
	}

	public void SetPlayer(PlayerIndex index)
	{
		if (indices.Count == 0)
		{
			indices.Add(index);
		}
		else
		{
			indices[0] = index;
		}
	}
}
