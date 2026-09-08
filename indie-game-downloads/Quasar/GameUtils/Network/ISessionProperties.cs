using System.Collections;
using System.Collections.Generic;

namespace Quasar.GameUtils.Network;

public interface ISessionProperties : IEnumerable<int?>, IEnumerable
{
	int Count { get; }

	int? this[int index] { get; set; }
}
