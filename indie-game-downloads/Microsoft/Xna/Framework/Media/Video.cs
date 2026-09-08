using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Media;

public sealed class Video
{
	internal string handle;

	internal bool needsDurationHack;

	internal int audioTrack = -1;

	internal int videoTrack = -1;

	internal IVideoPlayerCodec parent;

	public int Width { get; private set; }

	public int Height { get; private set; }

	public float FramesPerSecond { get; private set; }

	public VideoSoundtrackType VideoSoundtrackType { get; private set; }

	public TimeSpan Duration { get; internal set; }

	internal GraphicsDevice GraphicsDevice { get; private set; }

	internal string Codec { get; private set; }

	internal Video(string fileName, GraphicsDevice device)
	{
		handle = fileName;
		GraphicsDevice = device;
		if (!File.Exists(fileName))
		{
			throw new FileNotFoundException(fileName);
		}
		Codec = GuessCodec(fileName);
		VideoPlayer.VideoInfo videoInfo = VideoPlayer.codecInfoReaders[Codec](fileName);
		Width = videoInfo.width;
		Height = videoInfo.height;
		FramesPerSecond = (float)videoInfo.fps;
		Duration = TimeSpan.MaxValue;
		needsDurationHack = true;
	}

	internal Video(string fileName, GraphicsDevice device, int durationMS, int width, int height, float framesPerSecond, VideoSoundtrackType soundtrackType)
		: this(fileName, device, durationMS, width, height, framesPerSecond, soundtrackType, GuessCodec(fileName))
	{
	}

	internal Video(string fileName, GraphicsDevice device, int durationMS, int width, int height, float framesPerSecond, VideoSoundtrackType soundtrackType, string codec)
	{
		handle = fileName;
		GraphicsDevice = device;
		Width = width;
		Height = height;
		FramesPerSecond = framesPerSecond;
		Codec = codec;
		Duration = TimeSpan.FromMilliseconds(durationMS);
		needsDurationHack = false;
		VideoSoundtrackType = soundtrackType;
	}

	public static Video FromUriEXT(Uri uri, GraphicsDevice graphicsDevice)
	{
		string fileName;
		if (uri.IsAbsoluteUri)
		{
			if (!uri.IsFile)
			{
				throw new InvalidOperationException("Only local file URIs are supported for now");
			}
			fileName = uri.LocalPath;
		}
		else
		{
			fileName = Path.Combine(TitleLocation.Path, uri.ToString());
		}
		return new Video(fileName, graphicsDevice);
	}

	public void SetAudioTrackEXT(int track)
	{
		audioTrack = track;
		if (parent != null)
		{
			parent.SetAudioTrackEXT(track);
		}
	}

	public void SetVideoTrackEXT(int track)
	{
		videoTrack = track;
		if (parent != null)
		{
			parent.SetVideoTrackEXT(track);
		}
	}

	private static string GuessCodec(string filename)
	{
		filename = filename.ToLower();
		foreach (KeyValuePair<string, string> codecExtension in VideoPlayer.codecExtensions)
		{
			if (filename.EndsWith(codecExtension.Key))
			{
				return codecExtension.Value;
			}
		}
		return "Theora";
	}
}
