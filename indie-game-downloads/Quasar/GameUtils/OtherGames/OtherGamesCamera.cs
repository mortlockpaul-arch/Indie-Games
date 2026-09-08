using Microsoft.Xna.Framework;
using Quasar.Elements.Cameras;
using Quasar.Global;
using Quasar.Input;

namespace Quasar.GameUtils.OtherGames;

internal class OtherGamesCamera : OrbitCamera
{
	private Transform targetTransform = new Transform();

	private OtherGames otherGames;

	public OtherGamesCamera(OtherGames otherGames)
	{
		this.otherGames = otherGames;
		Target = targetTransform;
		radius = 2f;
	}

	protected override void DoUpdate()
	{
		float num = 0f;
		Vector3 to;
		float to2;
		switch (otherGames.CurrentCameraPosition)
		{
		default:
			to = new Vector3(0f, 0f, 0f);
			to2 = 1.9f;
			num = -0.3f;
			break;
		case OtherGames.CameraPositions.Back:
			to = new Vector3(0f, 0f, 0f);
			to2 = 1.9f;
			num = -0.3f;
			break;
		case OtherGames.CameraPositions.Description:
			to = new Vector3(0f, -0.25f, 0f);
			to2 = 0.85f;
			break;
		case OtherGames.CameraPositions.Screenshot1:
			to = new Vector3(-0.1925f, 0.34f, 0f);
			to2 = 0.35f;
			break;
		case OtherGames.CameraPositions.Screenshot2:
			to = new Vector3(0.1925f, 0.34f, 0f);
			to2 = 0.35f;
			break;
		case OtherGames.CameraPositions.Screenshot3:
			to = new Vector3(-0.1925f, 0.122f, 0f);
			to2 = 0.35f;
			break;
		case OtherGames.CameraPositions.Screenshot4:
			to = new Vector3(0.1925f, 0.122f, 0f);
			to2 = 0.35f;
			break;
		case OtherGames.CameraPositions.QR:
			to = new Vector3(0.325f, -0.4f, 0.035f);
			to2 = 0.15f;
			break;
		}
		targetTransform.Translation = GameMath.Damping(targetTransform.Translation, to, 0.04f, Timer.DefaultTimer.LastIntervalSeconds);
		Vector2 vector = InputManager.MenuAim();
		float to3 = vector.X * 0.3f;
		num += (0f - vector.Y) * 0.3f;
		heading = GameMath.Damping(heading, to3, 0.04f, Timer.DefaultTimer.LastIntervalSeconds);
		pitch = GameMath.Damping(pitch, num, 0.04f, Timer.DefaultTimer.LastIntervalSeconds);
		radius = GameMath.Damping(radius, to2, 0.04f, Timer.DefaultTimer.LastIntervalSeconds);
		base.DoUpdate();
	}
}
