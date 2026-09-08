using System.Collections.Generic;
using Quasar.Audios;

namespace Quasar.Behaviors;

public class SpatializedAudioBehavior : Behavior
{
	private List<SpatializedAudio> audioList;

	public SpatializedAudioBehavior()
	{
		audioList = new List<SpatializedAudio>();
	}

	public SpatializedAudioBehavior(SpatializedAudio audio)
	{
		audioList = new List<SpatializedAudio>();
		AddAudio(audio);
	}

	public void AddAudio(SpatializedAudio audio)
	{
		audioList.Add(audio);
	}

	public override void DoUpdate(Element element)
	{
		if (audioList.Count <= 0)
		{
			return;
		}
		foreach (SpatializedAudio audio in audioList)
		{
			if (audio.Playing && audio.Volume > 0f)
			{
				audio.Update(element);
			}
		}
	}
}
