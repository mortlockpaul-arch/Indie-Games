using InfinityLibrary;
using Microsoft.Xna.Framework;

namespace Infinity.GameObjects;

public class Zako : EnemyData
{
	public EnemyType Type { get; set; }

	public Zako(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
	}
}
