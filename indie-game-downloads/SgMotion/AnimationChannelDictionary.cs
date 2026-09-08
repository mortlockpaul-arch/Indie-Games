using System.Collections.Generic;

namespace SgMotion;

public class AnimationChannelDictionary : ReadOnlyDictionary<string, AnimationChannel>
{
	public AnimationChannelDictionary(IDictionary<string, AnimationChannel> dictionary)
		: base(dictionary)
	{
	}
}
