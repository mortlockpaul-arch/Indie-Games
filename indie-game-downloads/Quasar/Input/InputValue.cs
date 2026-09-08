using Microsoft.Xna.Framework;

namespace Quasar.Input;

public struct InputValue<T>(T value, PlayerIndex player)
{
	public T Value = value;

	public PlayerIndex Player = player;
}
