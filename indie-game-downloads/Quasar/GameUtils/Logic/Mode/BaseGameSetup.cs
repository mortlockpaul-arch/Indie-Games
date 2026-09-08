using System;

namespace Quasar.GameUtils.Logic.Mode;

public abstract class BaseGameSetup<T> : GameSetup where T : struct, IConvertible
{
	private T gameMode;

	public new T GameMode => gameMode;

	protected BaseGameSetup(T gameMode)
		: base(gameMode.ToInt32(null))
	{
		this.gameMode = gameMode;
	}
}
