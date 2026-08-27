namespace DataContent;

public class GameSettingClass
{
	public string GameName = "null";

	public bool PlayLogoVideo;

	public bool SimulateTrialMode;

	public bool FixedTimeStep;

	public int TimeStepMilliseconds;

	public bool VerticalSync;

	public bool EnablePost;

	public bool EditorInvertPitch;

	public bool GameInvertPitch;

	public int GBufferSizeX;

	public int GBufferSizeY;

	public int BackBufferSizeX;

	public int BackBufferSizeY;

	public int RenderTargetSizeX;

	public int RenderTargetSizeY;

	public int BloomTextureSize;

	public int ShadowTextureSize;

	public int ReflectionTextureSize;

	public string LevelOutsideName = "null";

	public string LevelEmitterName = "null";

	public bool EnableCollision = true;

	public bool EnableGravity = true;
}
