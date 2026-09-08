using System.Collections.Generic;

namespace Quasar.GameUtils.Network;

public interface IAvailableSessionCollection
{
	IEnumerable<IAvailableSession> Sessions { get; }

	IAvailableSession this[int index] { get; }

	int Count { get; }
}
