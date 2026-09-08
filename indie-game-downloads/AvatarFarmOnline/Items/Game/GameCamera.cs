using System;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar.Elements;
using Quasar.Global;

namespace AvatarFarmOnline.Items.Game;

internal class GameCamera : Camera
{
	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Vector3 yawPitchRoll;

	private Vector2 offset;

	public GameCamera(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Engine.AspectRatio, 0.1f, 10000f);
		yawPitchRoll = new Vector3(stage.LocalPlayer.Orientation, 0f);
	}

	protected override void DoUpdate()
	{
		switch (stage.LocalPlayer.CameraState)
		{
		case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Normal:
		{
			float num = 3.75f;
			yawPitchRoll = GameMath.Damping(yawPitchRoll, new Vector3(stage.LocalPlayer.Orientation, 0f), 0.03f, Timer.DefaultTimer.LastIntervalSeconds);
			offset = GameMath.Damping(offset, GameMath.VectorFromAngle(stage.LocalPlayer.Orientation.X, 0.6f), 0.03f, Timer.DefaultTimer.LastIntervalSeconds);
			Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f - yawPitchRoll.X, yawPitchRoll.Y, 0f);
			rotation.Normalize();
			Transform.Rotation = rotation;
			Vector3 vector = stage.LocalPlayer.WorldPosition + new Vector3(0f, 1.25f, 0f) + Transform.ZVector * num;
			Transform.Translation = vector + new Vector3(offset.X, 0f, offset.Y);
			Transform.LookAt(transform.Translation, transform.Translation - Transform.ZVector * 10f, Transform.YVector);
			projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Engine.AspectRatio, 0.1f, 10000f);
			break;
		}
		case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Cenital:
		{
			Vector3 worldPosition2 = stage.LocalPlayer.WorldPosition;
			Transform.LookAt(worldPosition2 + new Vector3(0f, 10f, 8f), worldPosition2 + new Vector3(0f, 0.5f, 0f), new Vector3(0f, 0f, -1f));
			projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Engine.AspectRatio, 0.1f, 10000f);
			break;
		}
		case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World:
		{
			Vector2 farmExtents = stage.FarmData.FarmExtents;
			float totalTimeSeconds = Timer.DefaultTimer.TotalTimeSeconds;
			Vector3 position = new Vector3(farmExtents.X * 0.5f + farmExtents.X * 0.825f * (float)Math.Sin(totalTimeSeconds * 0.035f), farmExtents.Length() * 0.175f, farmExtents.Y * 0.5f + farmExtents.Y * 0.825f * (float)Math.Cos(totalTimeSeconds * 0.035f));
			Transform.LookAt(position, new Vector3(farmExtents.X * 0.5f, (0f - position.Y) * 0.5f, farmExtents.Y * 0.5f), new Vector3(0f, 1f, 0f));
			projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Engine.AspectRatio, 0.1f, 10000f);
			break;
		}
		case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.Isometric:
		{
			Vector3 worldPosition = stage.LocalPlayer.WorldPosition;
			Transform.LookAt(worldPosition + new Vector3(8f, 10f, 8f), worldPosition + new Vector3(0f, 0.5f, 0f), new Vector3(0f, 1f, 0f));
			projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, Engine.AspectRatio, 0.1f, 10000f);
			break;
		}
		}
		base.DoUpdate();
	}
}
