using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SgMotion;

public class AnimationClip
{
	public enum AnimationClipType
	{
		Root,
		Derived
	}

	private readonly string name;

	private TimeSpan duration;

	private readonly AnimationChannelDictionary channels;

	private readonly AnimationClipType clipType;

	private string parentAnimationName;

	private float startTimeSeconds;

	private float endTimeSeconds;

	public string ParentAnimationName
	{
		get
		{
			return parentAnimationName;
		}
		set
		{
			parentAnimationName = value;
		}
	}

	public float StartTimeSeconds
	{
		get
		{
			return startTimeSeconds;
		}
		set
		{
			startTimeSeconds = value;
		}
	}

	public float EndTimeSeconds
	{
		get
		{
			return endTimeSeconds;
		}
		set
		{
			endTimeSeconds = value;
		}
	}

	public AnimationClipType ClipType => clipType;

	public string Name => name;

	public TimeSpan Duration
	{
		get
		{
			return duration;
		}
		set
		{
			if (value.Ticks > 0)
			{
				duration = value;
			}
			else
			{
				duration = TimeSpan.FromTicks(1L);
			}
		}
	}

	public AnimationChannelDictionary Channels => channels;

	public AnimationClip(string name, AnimationClipType cliptype)
	{
		this.name = name;
		channels = new AnimationChannelDictionary(new Dictionary<string, AnimationChannel>());
		clipType = cliptype;
		parentAnimationName = string.Empty;
		Duration = default(TimeSpan);
	}

	internal AnimationClip(string name, TimeSpan duration, AnimationChannelDictionary channels, AnimationClipType cliptype, string parentanimationname, float starttimeseconds, float endtimeseconds)
	{
		this.name = name;
		this.channels = channels;
		clipType = cliptype;
		parentAnimationName = parentanimationname;
		startTimeSeconds = starttimeseconds;
		endTimeSeconds = endtimeseconds;
		Duration = duration;
	}

	internal static AnimationClip Read(ContentReader input)
	{
		string text = input.ReadString();
		TimeSpan timeSpan = input.ReadObject<TimeSpan>();
		AnimationClipType cliptype = (AnimationClipType)input.ReadInt32();
		string parentanimationname = input.ReadString();
		float starttimeseconds = input.ReadSingle();
		float endtimeseconds = input.ReadSingle();
		Dictionary<string, AnimationChannel> dictionary = new Dictionary<string, AnimationChannel>();
		int num = input.ReadInt32();
		Pose pose = default(Pose);
		for (int i = 0; i < num; i++)
		{
			string key = input.ReadString();
			int num2 = input.ReadInt32();
			List<AnimationChannelKeyframe> list = new List<AnimationChannelKeyframe>(num2);
			for (int j = 0; j < num2; j++)
			{
				TimeSpan time = input.ReadObject<TimeSpan>();
				pose.Translation = input.ReadVector3();
				pose.Orientation = input.ReadQuaternion();
				pose.Scale = input.ReadVector3();
				list.Add(new AnimationChannelKeyframe(time, pose));
			}
			AnimationChannel value = new AnimationChannel(list);
			dictionary.Add(key, value);
		}
		return new AnimationClip(text, timeSpan, new AnimationChannelDictionary(dictionary), cliptype, parentanimationname, starttimeseconds, endtimeseconds);
	}
}
