using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BureauNewPDA.VideoData;

public class VideoControl
{
	public enum VideoStatus
	{
		Playing,
		Stopped,
		Waiting
	}

	private Texture2D videoTexture;

	public VideoPlayer videoPlayer;

	private Vector2 videoPosition = Vector2.Zero;

	private Rectangle videoRect = new Rectangle(0, 0, 1280, 720);

	private bool isVideoPlaying;

	private int pendingVideoId = -1;

	private int currentVideoId = -1;

	private bool videoIsLooping;

	private Video pendingVideo;

	private Video currentVideo;

	private bool videoPlaying;

	public VideoStatus currentVideoStatus = VideoStatus.Waiting;

	public string lastPlayedVideoName = "";

	private string pendingVideoName = "";

	private bool hasPlayed;

	public void addPendingVideo(Video p, string videoName)
	{
		pendingVideo = p;
		currentVideoStatus = VideoStatus.Waiting;
		pendingVideoName = videoName;
	}

	public void update(bool isPaused)
	{
		if (pendingVideo != null && checkVideoCondition())
		{
			currentVideo = pendingVideo;
			videoPlayer.Play(currentVideo);
			videoPlaying = true;
			currentVideoStatus = VideoStatus.Playing;
			lastPlayedVideoName = pendingVideoName;
			pendingVideo = null;
			pendingVideoName = "";
			hasPlayed = false;
		}
		if ((videoPlaying & hasPlayed) && videoPlayer.State == MediaState.Stopped)
		{
			currentVideoStatus = VideoStatus.Stopped;
		}
		if (isPaused & (videoPlayer.State == MediaState.Playing))
		{
			videoPlayer.Pause();
		}
		else if (!isPaused & (videoPlayer.State == MediaState.Paused))
		{
			videoPlayer.Resume();
		}
	}

	private bool checkVideoCondition()
	{
		if (pendingVideo != currentVideo)
		{
			return true;
		}
		if (pendingVideoName == "PuzzleChangeOrder")
		{
			Console.WriteLine("Puzzle Video Found");
			return true;
		}
		return false;
	}

	public void draw(SpriteBatch spriteBatch)
	{
		if (videoPlaying)
		{
			hasPlayed = true;
			if (videoPlayer.State == MediaState.Playing)
			{
				videoTexture = videoPlayer.GetTexture();
			}
			if (videoTexture != null)
			{
				spriteBatch.Draw(videoTexture, videoPosition, videoRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
			}
		}
	}
}
