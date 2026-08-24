using Microsoft.Xna.Framework;

namespace Game.Data;

public class DataSettings
{
	public int level;

	public bool seenHelp;

	public int volumeMusic;

	public int volumeFX;

	public bool moveInvertX;

	public bool moveInvertY;

	public bool cameraInvertX;

	public bool cameraInvertY;

	public bool cameraSnapping;

	public bool showBuildHelpBar;

	public int gamma;

	public Rectangle screen;

	public Point resolution;

	public DataSettings()
	{
		showBuildHelpBar = true;
	}

	public DataSettings(int xLevel, bool xSeenHelp, int xVolumeMusic, int xVolumeFX, bool xMoveInvertX, bool xMoveInvertY, bool xCameraInvertX, bool xCameraInvertY, bool xCameraSnapping, bool xShowBuildHelpBar, int xGamma, Rectangle pScreen, Point pResolution)
	{
		level = xLevel;
		seenHelp = xSeenHelp;
		volumeMusic = xVolumeMusic;
		volumeFX = xVolumeFX;
		moveInvertX = xMoveInvertX;
		moveInvertY = xMoveInvertY;
		cameraInvertX = xCameraInvertX;
		cameraInvertY = xCameraInvertY;
		cameraSnapping = xCameraSnapping;
		showBuildHelpBar = xShowBuildHelpBar;
		gamma = xGamma;
		screen = pScreen;
		resolution = pResolution;
	}

	public void InversionMove(ref Vector2 vInput)
	{
		vInput.X = (moveInvertX ? (vInput.X * -1f) : vInput.X);
		vInput.Y = (moveInvertY ? (vInput.Y * -1f) : vInput.Y);
	}

	public void InversionCam(ref Vector2 vInput)
	{
		vInput.X = (cameraInvertX ? (vInput.X * -1f) : vInput.X);
		vInput.Y = (cameraInvertY ? (vInput.Y * -1f) : vInput.Y);
	}
}
