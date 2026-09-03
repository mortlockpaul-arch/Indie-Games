using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xclna.Xna.Animation;

public class AnimationChannelCollection : ReadOnlyCollection<BoneKeyframeCollection>
{
	private Dictionary<string, BoneKeyframeCollection> dict = new Dictionary<string, BoneKeyframeCollection>();

	private ReadOnlyCollection<string> affectedBones;

	public BoneKeyframeCollection this[string boneName] => dict[boneName];

	internal ReadOnlyCollection<string> AffectedBones => affectedBones;

	internal AnimationChannelCollection(IList<BoneKeyframeCollection> channels)
		: base(channels)
	{
		List<string> list = new List<string>();
		foreach (BoneKeyframeCollection channel in channels)
		{
			dict.Add(channel.BoneName, channel);
			list.Add(channel.BoneName);
		}
		affectedBones = new ReadOnlyCollection<string>(list);
	}

	internal bool AffectsBone(string boneName)
	{
		return dict.ContainsKey(boneName);
	}
}
