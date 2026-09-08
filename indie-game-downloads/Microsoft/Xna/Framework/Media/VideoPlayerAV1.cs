using System;
using System.Diagnostics;
using System.IO;
using Dav1dfile;
using Microsoft.Xna.Framework.Graphics;
using SDL3;

namespace Microsoft.Xna.Framework.Media;

internal class VideoPlayerAV1 : BaseYUVPlayer, IVideoPlayerCodec, IDisposable
{
	private Stopwatch timer;

	private nint context;

	private int bitsPerPixel;

	private int currentFrame;

	private double fps;

	public bool IsLooped { get; set; }

	public bool IsMuted
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public TimeSpan PlayPosition => timer.Elapsed;

	public MediaState State { get; private set; }

	public Video Video { get; private set; }

	public float Volume
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public VideoPlayerAV1()
	{
		IsLooped = false;
		IsMuted = true;
		State = MediaState.Stopped;
		Volume = 0f;
		timer = new Stopwatch();
	}

	public override void Dispose()
	{
		if (!base.IsDisposed)
		{
			Stop();
			base.Dispose();
			if (context != IntPtr.Zero)
			{
				Bindings.df_close(context);
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
		if (State == MediaState.Stopped || context == IntPtr.Zero)
		{
			return videoTexture[0].RenderTarget as Texture2D;
		}
		int num = (int)(timer.Elapsed.TotalMilliseconds / (1000.0 / fps));
		if (num > currentFrame)
		{
			if (DecodeAndUpdateFrame(num - currentFrame) || currentFrame == -1)
			{
				float num2 = ((bitsPerPixel == 12) ? 16f : ((bitsPerPixel != 10) ? 1f : 64f));
				shaderProgram.Parameters["RescaleFactor"].SetValue(new Vector4(num2, num2, num2, 1f));
				GL_pushState();
				currentDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
				GL_popState();
			}
			currentFrame = num;
		}
		if (Bindings.df_eos(context) == 1)
		{
			if (Video.needsDurationHack)
			{
				Video.Duration = timer.Elapsed;
			}
			timer.Stop();
			timer.Reset();
			Bindings.df_reset(context);
			if (IsLooped)
			{
				currentFrame = -1;
				timer.Start();
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
		if (context != IntPtr.Zero)
		{
			Bindings.df_close(context);
			context = IntPtr.Zero;
		}
		if (Video.needsDurationHack)
		{
			Video.Duration = TimeSpan.MaxValue;
		}
		int num = Bindings.df_fopen(Video.handle, out context);
		if (context == IntPtr.Zero || num == 0)
		{
			throw new FileNotFoundException(Video.handle);
		}
		int width;
		int height;
		Bindings.PixelLayout pixelLayout;
		try
		{
			Bindings.df_videoinfo2(context, out width, out height, out pixelLayout, out var hbd);
			switch (hbd)
			{
			case 2:
				bitsPerPixel = 12;
				break;
			case 1:
				bitsPerPixel = 10;
				break;
			default:
				bitsPerPixel = 8;
				break;
			}
			if (Bindings.df_guessframerate(context, out fps) == 0)
			{
				fps = Video.FramesPerSecond;
			}
		}
		catch
		{
			Bindings.df_videoinfo(context, out width, out height, out pixelLayout);
			bitsPerPixel = 8;
			fps = Video.FramesPerSecond;
		}
		int uvWidth;
		int uvHeight;
		switch (pixelLayout)
		{
		case Bindings.PixelLayout.I420:
			uvWidth = width / 2;
			uvHeight = height / 2;
			break;
		case Bindings.PixelLayout.I422:
			uvWidth = width / 2;
			uvHeight = height;
			break;
		case Bindings.PixelLayout.I444:
			uvWidth = width;
			uvHeight = height;
			break;
		default:
			throw new NotSupportedException("Unsupported pixel layout in AV1 file");
		}
		if (Video.Width != width || Video.Height != height)
		{
			throw new InvalidOperationException("XNB/OGV width/height mismatch! Width: " + Video.Width + " Height: " + Video.Height);
		}
		if (fps == 0.0)
		{
			throw new InvalidOperationException("Framerate not present in header or manually specified");
		}
		if (Math.Abs((double)Video.FramesPerSecond - fps) >= 1.0)
		{
			throw new InvalidOperationException("XNB/OGV framesPerSecond mismatch! FPS: " + Video.FramesPerSecond);
		}
		if (State == MediaState.Stopped)
		{
			State = MediaState.Playing;
			currentFrame = -1;
			if (currentDevice != Video.GraphicsDevice)
			{
				GL_dispose();
				currentDevice = Video.GraphicsDevice;
				GL_initialize(Resources.YUVToRGBAEffectR);
			}
			RenderTargetBinding renderTargetBinding = videoTexture[0];
			videoTexture[0] = new RenderTargetBinding(new RenderTarget2D(currentDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents));
			if (renderTargetBinding.RenderTarget != null)
			{
				renderTargetBinding.RenderTarget.Dispose();
			}
			GL_setupTextures(width, height, uvWidth, uvHeight, (bitsPerPixel > 8) ? SurfaceFormat.UShortEXT : SurfaceFormat.ByteEXT);
			timer.Start();
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
			Bindings.df_reset(context);
		}
	}

	public void Pause()
	{
		checkDisposed();
		if (State == MediaState.Playing)
		{
			State = MediaState.Paused;
			timer.Stop();
		}
	}

	public void Resume()
	{
		checkDisposed();
		if (State == MediaState.Paused)
		{
			State = MediaState.Playing;
			timer.Start();
		}
	}

	public void SetAudioTrackEXT(int track)
	{
	}

	public void SetVideoTrackEXT(int track)
	{
	}

	private bool DecodeAndUpdateFrame(int frameCount = 1)
	{
		byte[] scratchBuffer = null;
		byte[] scratchBuffer2 = null;
		int num = Bindings.df_readvideo(context, frameCount, out var yDataPtr, out var uDataPtr, out var vDataPtr, out var yDataLength, out var uvDataLength, out var yStride, out var uvStride);
		if (num != 1)
		{
			return false;
		}
		UploadDataToTexture(yuvTextures[0], yDataPtr, yDataLength, yStride, ref scratchBuffer);
		UploadDataToTexture(yuvTextures[1], uDataPtr, uvDataLength, uvStride, ref scratchBuffer2);
		UploadDataToTexture(yuvTextures[2], vDataPtr, uvDataLength, uvStride, ref scratchBuffer2);
		return true;
	}

	private unsafe void UploadDataToTexture(Texture2D texture, nint data, uint length, uint stride, ref byte[] scratchBuffer)
	{
		int width = texture.Width;
		int height = texture.Height;
		int val = (int)(length / stride);
		int num = Math.Min(val, height);
		int num2 = ((bitsPerPixel <= 8) ? 1 : 2);
		int num3 = num2 * width;
		if (width == stride)
		{
			texture.SetDataPointerEXT(0, new Rectangle(0, 0, width, num), data, (int)length);
			return;
		}
		Array.Resize(ref scratchBuffer, width * num * num2);
		fixed (byte* ptr = scratchBuffer)
		{
			for (int i = 0; i < num; i++)
			{
				SDL.SDL_memcpy((nint)(ptr + num3 * i), data + (int)(stride * i), (nuint)num3);
			}
			texture.SetDataPointerEXT(0, null, (nint)ptr, scratchBuffer.Length);
		}
	}

	internal static VideoPlayer.VideoInfo ReadInfo(string fileName)
	{
		Bindings.df_fopen(fileName, out var num);
		Bindings.df_videoinfo2(num, out var width, out var height, out var _, out var _);
		if (Bindings.df_guessframerate(num, out var num2) == 0)
		{
			num2 = 0.0;
		}
		Bindings.df_close(num);
		return new VideoPlayer.VideoInfo
		{
			fps = num2,
			width = width,
			height = height
		};
	}
}
