using System;
using Microsoft.Xna.Framework.Media;
using Quasar.GameUtils.Tasks;

namespace Quasar.GameUtils.Audio;

public class JukeboxManager
{
	private static JukeboxManager instance;

	public static JukeboxManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new JukeboxManager();
			}
			return instance;
		}
	}

	public void ChangeSong(Song song)
	{
		TaskManager.Post(changeSong, song);
	}

	private void changeSong(object parameters)
	{
		try
		{
			Song song = (Song)parameters;
			if (MediaPlayer.GameHasControl)
			{
				MediaPlayer.IsVisualizationEnabled = false;
				MediaPlayer.Play(song);
				if (Jukebox.ActiveJukebox != null)
				{
					MediaPlayer.Volume = Jukebox.ActiveJukebox.Volume * Quasar.Audio.MusicVolume;
				}
				else
				{
					MediaPlayer.Volume = Quasar.Audio.MusicVolume;
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
