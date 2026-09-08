using System.Collections.Generic;

namespace SgMotion;

public class AnimationClipDictionary : ReadOnlyDictionary<string, AnimationClip>
{
	public AnimationClipDictionary(IDictionary<string, AnimationClip> dictionary)
		: base(dictionary)
	{
	}
}
