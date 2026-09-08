using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Input;

public interface BaseInput<T> where T : IInputState
{
	void Update(List<PlayerIndex> playerIndices, ref T result);

	void Update(PlayerIndex playerIndex, ref T result);
}
