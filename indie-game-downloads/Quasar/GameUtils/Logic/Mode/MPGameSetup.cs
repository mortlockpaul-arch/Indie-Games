using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.GameUtils.Logic.Mode;

public abstract class MPGameSetup<T> : BaseGameSetup<T> where T : struct, IConvertible
{
	private List<PlayerIndex> indices = new List<PlayerIndex>();

	public override List<PlayerIndex> PlayerIndices => indices;

	public MPGameSetup(T gameMode)
		: base(gameMode)
	{
	}

	public void AddPlayer(PlayerIndex index)
	{
		if (!indices.Contains(index))
		{
			indices.Add(index);
		}
	}

	public void SetPlayers(List<PlayerIndex> indices)
	{
		this.indices.Clear();
		this.indices.AddRange(indices);
	}
}
