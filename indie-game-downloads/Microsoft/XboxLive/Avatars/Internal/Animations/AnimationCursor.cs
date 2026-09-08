namespace Microsoft.XboxLive.Avatars.Internal.Animations;

public class AnimationCursor
{
	public AnimationPlayMode playMode;

	public float replaySpeed;

	public float localTime;

	public AnimationPlayMode PlayMode => playMode;

	public float Speed => replaySpeed;

	public float Time
	{
		get
		{
			return localTime;
		}
		set
		{
			localTime = value;
		}
	}

	public AnimationCursor(float speed, AnimationPlayMode mode)
	{
		playMode = mode;
		replaySpeed = speed;
	}
}
