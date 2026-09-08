using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Media;

internal interface IVideoPlayerCodec : IDisposable
{
	bool IsLooped { get; set; }

	bool IsMuted { get; set; }

	TimeSpan PlayPosition { get; }

	MediaState State { get; }

	Video Video { get; }

	float Volume { get; set; }

	Texture2D GetTexture();

	void Play(Video video);

	void Stop();

	void Pause();

	void Resume();

	void SetAudioTrackEXT(int track);

	void SetVideoTrackEXT(int track);
}
