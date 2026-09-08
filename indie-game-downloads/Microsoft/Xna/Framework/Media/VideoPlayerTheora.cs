using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Media;

internal sealed class VideoPlayerTheora : BaseYUVPlayer, IVideoPlayerCodec, IDisposable
{
	private bool backing_ismuted;

	private float backing_volume;

	private Stopwatch timer;

	private nint theora;

	private double fps;

	private nint yuvData;

	private int yuvDataLen;

	private int currentFrame;

	private const int AUDIO_BUFFER_SIZE = 8192;

	private static readonly float[] audioData = new float[8192];

	private static GCHandle audioHandle = GCHandle.Alloc(audioData, GCHandleType.Pinned);

	private nint audioDataPtr = audioHandle.AddrOfPinnedObject();

	private DynamicSoundEffectInstance audioStream;

	public bool IsLooped { get; set; }

	public bool IsMuted
	{
		get
		{
			return backing_ismuted;
		}
		set
		{
			backing_ismuted = value;
			UpdateVolume();
		}
	}

	public TimeSpan PlayPosition => timer.Elapsed;

	public MediaState State { get; private set; }

	public Video Video { get; private set; }

	public float Volume
	{
		get
		{
			return backing_volume;
		}
		set
		{
			if (value > 1f)
			{
				backing_volume = 1f;
			}
			else if (value < 0f)
			{
				backing_volume = 0f;
			}
			else
			{
				backing_volume = value;
			}
			UpdateVolume();
		}
	}

	private void UpdateVolume()
	{
		if (audioStream != null)
		{
			if (IsMuted)
			{
				audioStream.Volume = 0f;
			}
			else
			{
				audioStream.Volume = Volume * (1f / SoundEffect.MasterVolume);
			}
		}
	}

	public VideoPlayerTheora()
	{
		IsLooped = false;
		IsMuted = false;
		State = MediaState.Stopped;
		Volume = 1f;
		timer = new Stopwatch();
	}

	public override void Dispose()
	{
		if (!base.IsDisposed)
		{
			Stop();
			base.Dispose();
			if (audioStream != null)
			{
				audioStream.Dispose();
				audioStream = null;
			}
			if (yuvData != IntPtr.Zero)
			{
				FNAPlatform.Free(yuvData);
				yuvData = IntPtr.Zero;
			}
			if (theora != IntPtr.Zero)
			{
				Theorafile.tf_close(ref theora);
			}
		}
	}

	public Texture2D GetTexture()
	{
		checkDisposed();
		if (Video == null)
		{
			throw new InvalidOperationException();
		}
		if (State == MediaState.Stopped || theora == IntPtr.Zero || Theorafile.tf_hasvideo(theora) == 0)
		{
			return videoTexture[0].RenderTarget as Texture2D;
		}
		int num = (int)(timer.Elapsed.TotalMilliseconds / (1000.0 / fps));
		if (num > currentFrame)
		{
			if (Theorafile.tf_readvideo(theora, yuvData, num - currentFrame) == 1 || currentFrame == -1)
			{
				UpdateTexture();
			}
			currentFrame = num;
		}
		bool flag = Theorafile.tf_eos(theora) == 1;
		if (audioStream != null)
		{
			flag &= audioStream.PendingBufferCount == 0;
		}
		if (flag)
		{
			if (Video.needsDurationHack)
			{
				Video.Duration = timer.Elapsed;
			}
			timer.Stop();
			timer.Reset();
			if (audioStream != null)
			{
				audioStream.Stop();
				audioStream.Dispose();
				audioStream = null;
			}
			Theorafile.tf_reset(theora);
			if (IsLooped)
			{
				InitializeTheoraStream();
				timer.Start();
				if (audioStream != null)
				{
					audioStream.Play();
				}
			}
			else
			{
				State = MediaState.Stopped;
			}
		}
		return videoTexture[0].RenderTarget as Texture2D;
	}

	public void Play(Video video)
	{
		checkDisposed();
		Video = video;
		Video.parent = this;
		if (theora != IntPtr.Zero)
		{
			Theorafile.tf_close(ref theora);
			theora = IntPtr.Zero;
		}
		if (Video.needsDurationHack)
		{
			Video.Duration = TimeSpan.MaxValue;
		}
		Theorafile.tf_fopen(Video.handle, out theora);
		if (theora == IntPtr.Zero)
		{
			throw new FileNotFoundException(Video.handle);
		}
		Theorafile.tf_videoinfo(theora, out var width, out var height, out fps, out var fmt);
		int num;
		int num2;
		switch (fmt)
		{
		case Theorafile.th_pixel_fmt.TH_PF_420:
			num = width / 2;
			num2 = height / 2;
			break;
		case Theorafile.th_pixel_fmt.TH_PF_422:
			num = width / 2;
			num2 = height;
			break;
		case Theorafile.th_pixel_fmt.TH_PF_444:
			num = width;
			num2 = height;
			break;
		default:
			throw new NotSupportedException("Unrecognized YUV format!");
		}
		if (Video.Width != width || Video.Height != height)
		{
			throw new InvalidOperationException("XNB/OGV width/height mismatch! Width: " + Video.Width + " Height: " + Video.Height);
		}
		if (Math.Abs((double)Video.FramesPerSecond - fps) >= 1.0)
		{
			throw new InvalidOperationException("XNB/OGV framesPerSecond mismatch! FPS: " + Video.FramesPerSecond);
		}
		if (Video.audioTrack >= 0)
		{
			SetAudioTrackEXT(Video.audioTrack);
		}
		if (Video.videoTrack >= 0)
		{
			SetVideoTrackEXT(Video.videoTrack);
		}
		if (State != MediaState.Stopped)
		{
			return;
		}
		State = MediaState.Playing;
		if (yuvData != IntPtr.Zero)
		{
			FNAPlatform.Free(yuvData);
		}
		yuvDataLen = width * height + num * num2 * 2;
		yuvData = FNAPlatform.Malloc(yuvDataLen);
		InitializeTheoraStream();
		if (Theorafile.tf_hasvideo(theora) == 1)
		{
			if (currentDevice != Video.GraphicsDevice)
			{
				GL_dispose();
				currentDevice = Video.GraphicsDevice;
				GL_initialize(Resources.YUVToRGBAEffect);
			}
			RenderTargetBinding renderTargetBinding = videoTexture[0];
			videoTexture[0] = new RenderTargetBinding(new RenderTarget2D(currentDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents));
			if (renderTargetBinding.RenderTarget != null)
			{
				renderTargetBinding.RenderTarget.Dispose();
			}
			GL_setupTextures(width, height, num, num2, SurfaceFormat.Alpha8);
		}
		timer.Start();
		if (audioStream != null)
		{
			audioStream.Play();
		}
	}

	public void Stop()
	{
		checkDisposed();
		if (State != MediaState.Stopped)
		{
			State = MediaState.Stopped;
			timer.Stop();
			timer.Reset();
			if (audioStream != null)
			{
				audioStream.Stop();
				audioStream.Dispose();
				audioStream = null;
			}
			Theorafile.tf_reset(theora);
		}
	}

	public void Pause()
	{
		checkDisposed();
		if (State == MediaState.Playing)
		{
			State = MediaState.Paused;
			timer.Stop();
			if (audioStream != null)
			{
				audioStream.Pause();
			}
		}
	}

	public void Resume()
	{
		checkDisposed();
		if (State == MediaState.Paused)
		{
			State = MediaState.Playing;
			timer.Start();
			if (audioStream != null)
			{
				audioStream.Resume();
			}
		}
	}

	public void SetAudioTrackEXT(int track)
	{
		if (theora != IntPtr.Zero)
		{
			Theorafile.tf_setaudiotrack(theora, track);
		}
	}

	public void SetVideoTrackEXT(int track)
	{
		if (theora != IntPtr.Zero)
		{
			Theorafile.tf_setvideotrack(theora, track);
		}
	}

	private void OnBufferRequest(object sender, EventArgs args)
	{
		int num = Theorafile.tf_readaudio(theora, audioDataPtr, 8192);
		if (num > 0)
		{
			audioStream.SubmitFloatBufferEXT(audioData, 0, num);
		}
		else if (Theorafile.tf_eos(theora) == 1)
		{
			audioStream.BufferNeeded -= OnBufferRequest;
		}
	}

	private void UpdateTexture()
	{
		FNA3D.FNA3D_SetTextureDataYUV(currentDevice.GLDevice, yuvTextures[0].texture, yuvTextures[1].texture, yuvTextures[2].texture, yuvTextures[0].Width, yuvTextures[0].Height, yuvTextures[1].Width, yuvTextures[1].Height, yuvData, yuvDataLen);
		GL_pushState();
		currentDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		GL_popState();
	}

	private void InitializeTheoraStream()
	{
		while (Theorafile.tf_readvideo(theora, yuvData, 1) == 0)
		{
		}
		if (Theorafile.tf_hasaudio(theora) == 1)
		{
			Theorafile.tf_audioinfo(theora, out var channels, out var samplerate);
			audioStream = new DynamicSoundEffectInstance(samplerate, (AudioChannels)channels);
			audioStream.BufferNeeded += OnBufferRequest;
			UpdateVolume();
			for (int i = 0; i < 4; i++)
			{
				OnBufferRequest(audioStream, EventArgs.Empty);
				if (audioStream.PendingBufferCount == i)
				{
					break;
				}
			}
		}
		currentFrame = -1;
	}

	internal static VideoPlayer.VideoInfo ReadInfo(string fileName)
	{
		Theorafile.tf_fopen(fileName, out var file);
		Theorafile.tf_videoinfo(file, out var width, out var height, out var num, out var _);
		Theorafile.tf_close(ref file);
		return new VideoPlayer.VideoInfo
		{
			fps = num,
			width = width,
			height = height
		};
	}
}
