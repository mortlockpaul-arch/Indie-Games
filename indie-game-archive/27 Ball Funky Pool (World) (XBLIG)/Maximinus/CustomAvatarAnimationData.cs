using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Maximinus;

public class CustomAvatarAnimationData
{
	[ContentSerializer]
	public string Name { get; private set; }

	[ContentSerializer]
	public TimeSpan Length { get; private set; }

	[ContentSerializer]
	public List<AvatarKeyFrame> Keyframes { get; private set; }

	private CustomAvatarAnimationData()
	{
	}

	public CustomAvatarAnimationData(string name, TimeSpan length, List<AvatarKeyFrame> keyframes)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (length <= TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("length", "The length of the animation cannot be zero.");
		}
		if (keyframes == null || keyframes.Count <= 0)
		{
			throw new ArgumentNullException("keyframes");
		}
		Name = name;
		Length = length;
		Keyframes = keyframes;
	}
}
