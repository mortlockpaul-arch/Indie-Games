namespace Microsoft.XboxLive.Avatars.Internal.Animations;

public class Animation
{
	public float length;

	public int frameCount;

	public float fps;

	public AnimationPlayMode ClipPlayMode { get; set; }

	public float Length => length;

	public float Fps => fps;

	public int FrameCount => frameCount;

	public Animation(int frameCount, float framerate)
	{
		this.frameCount = frameCount;
		fps = framerate;
		ClipPlayMode = AnimationPlayMode.Once;
		length = (float)frameCount / fps;
	}
}
