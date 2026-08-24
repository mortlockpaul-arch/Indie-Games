using System;
using GKEngine.Cameras;
using GKEngine.Input;
using Game.Data;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.Players;

public class PlayerCamera
{
	public enum Mode
	{
		Rotate,
		Zoom,
		Free
	}

	public static string CAMERA_NAME = "BuildCam";

	public static float YAW_DEFAULT = 0.78853977f;

	public static float PITCH_DEFAULT = 0.78853977f;

	public static float YAW_SPEED = 0.005f;

	public static float PITCH_SPEED = 0.005f;

	public static float PITCH_LIMIT_MIN = -1.5607964f;

	public static float PITCH_LIMIT_MAX = 1.5607964f;

	public static float RADUIS_SPEED = 5f;

	public static float RADIUS_MAX = 1000f;

	public static float RADIUS_MIN = 200f;

	public static float AXIS_Y_THRESHOLD = 0.85f;

	public static float AXIS_XZ_THRESHOLD = 0.5f;

	public static float FREE_SPEED_X = 1f;

	public static float FREE_SPEED_Z = -2f;

	public static float FREE_YAW_SPEED = -0.002f;

	public static float FREE_PITCH_SPEED = -0.002f;

	private Vector3 _transform = default(Vector3);

	public Player player;

	public Camera camera;

	public BuildScene scene;

	public Mode mode;

	public float radius = RADIUS_MIN;

	private float radiusTo = RADIUS_MIN;

	public float pitch = PITCH_DEFAULT;

	public float yaw = YAW_DEFAULT;

	private Vector3 position = Vector3.Zero;

	private float pitchTo = PITCH_DEFAULT;

	private float yawTo = YAW_DEFAULT;

	private Vector3 positionTo = Vector3.Zero;

	public Point axis = new Point(0, 1);

	public Vector3 dot = default(Vector3);

	public PlayerCamera(Player oPlayer)
	{
		player = oPlayer;
		scene = oPlayer.universe.scene;
		camera = scene.cameras.cameras[CAMERA_NAME];
		camera.position = Vector3.Zero;
		camera.rotation = Quaternion.Identity;
		scene.cameras.SetActive(CAMERA_NAME);
		SetAxis();
	}

	public void Update(GameTime oGameTime)
	{
		if (mode == Mode.Rotate || mode == Mode.Zoom)
		{
			Update_Rotate_Zoom(oGameTime);
		}
		else if (mode == Mode.Free)
		{
			Update_Free(oGameTime);
		}
	}

	public void Update_Rotate_Zoom(GameTime oGameTime)
	{
		bool flag = false;
		float num = oGameTime.ElapsedGameTime.Milliseconds;
		float amount = Math.Min(num / 100f, 1f);
		if (yaw != yawTo)
		{
			yaw = MathHelper.Lerp(yaw, yawTo, amount);
			flag = true;
		}
		if (pitch != pitchTo)
		{
			pitch = MathHelper.Lerp(pitch, pitchTo, amount);
			flag = true;
		}
		camera.rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw) * Quaternion.CreateFromAxisAngle(Vector3.Left, pitch);
		if (mode == Mode.Zoom && radius != radiusTo)
		{
			radius = MathHelper.Lerp(radius, radiusTo, amount);
			flag = true;
		}
		positionTo = player.position;
		position = Vector3.Lerp(position, positionTo, amount);
		camera.position = position + camera.unit * radius * -1f;
		camera.Update_View();
		camera.focalLength = radius;
		if (flag)
		{
			SetAxis();
		}
	}

	public void Update_Free(GameTime oGameTime)
	{
		float num = oGameTime.ElapsedGameTime.Milliseconds;
		float amount = Math.Min(num / 100f, 1f);
		if (yaw != yawTo)
		{
			yaw = MathHelper.Lerp(yaw, yawTo, amount);
		}
		if (pitch != pitchTo)
		{
			pitch = MathHelper.Lerp(pitch, pitchTo, amount);
		}
		camera.rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw) * Quaternion.CreateFromAxisAngle(Vector3.Left, pitch);
		camera.Update_View();
	}

	public void SetAxis()
	{
		dot.X = Vector3.Dot(Vector3.Left, camera.matrix.Left);
		dot.Y = Vector3.Dot(Vector3.Up, camera.matrix.Up);
		dot.Z = Vector3.Dot(Vector3.Forward, camera.matrix.Left);
		if (player.moveMode == Player.MoveMode.Y || (Math.Abs(dot.Y) > AXIS_Y_THRESHOLD && player.moveMode == Player.MoveMode.CAMERA))
		{
			axis.Y = 1;
			if (Math.Abs(dot.X) > AXIS_XZ_THRESHOLD)
			{
				axis.X = 0;
			}
			else
			{
				axis.X = 2;
			}
		}
		else if (Math.Abs(dot.X) > AXIS_XZ_THRESHOLD)
		{
			axis.X = 0;
			axis.Y = 2;
		}
		else
		{
			axis.X = 2;
			axis.Y = 0;
		}
	}

	public void SetFromPos(Quaternion qRot)
	{
		Vector3 vector = Vector3.Transform(Vector3.Backward, qRot);
		if (vector.Length() == 0f)
		{
			vector.X = 1f;
			vector.Y = 1f;
			vector.Z = 1f;
		}
		vector.Normalize();
		yaw = (yawTo = (float)Math.Atan2(vector.X, vector.Z));
		pitch = (pitchTo = (float)Math.Atan2(vector.Y, Math.Sqrt(Math.Pow(vector.X, 2.0) + Math.Pow(vector.Z, 2.0))));
	}

	public void Input_Update(int elapsed)
	{
		float num = elapsed;
		switch (mode)
		{
		case Mode.Rotate:
			if (UniversalInput.inputEntities["PlayerCamToggle"].pressed)
			{
				mode = Mode.Zoom;
			}
			if (UniversalInput.inputEntities["PlayerCam"].value2D.X != 0f)
			{
				yawTo += UniversalInput.inputEntities["PlayerCam"].value2D.X * YAW_SPEED * num * (float)((!DataManager.local.settings.cameraInvertX) ? 1 : (-1));
			}
			if (UniversalInput.inputEntities["PlayerCam"].value2D.Y != 0f)
			{
				pitchTo += UniversalInput.inputEntities["PlayerCam"].value2D.Y * PITCH_SPEED * num * (float)((!DataManager.local.settings.cameraInvertY) ? 1 : (-1));
				pitchTo = MathHelper.Clamp(pitchTo, PITCH_LIMIT_MIN, PITCH_LIMIT_MAX);
			}
			break;
		case Mode.Zoom:
			if (UniversalInput.inputEntities["PlayerCamToggle"].pressed)
			{
				mode = Mode.Rotate;
			}
			if (UniversalInput.inputEntities["PlayerCam"].value2D.Y != 0f)
			{
				radiusTo += UniversalInput.inputEntities["PlayerCam"].value2D.Y * RADUIS_SPEED * num * -1f * (float)((!DataManager.local.settings.cameraInvertY) ? 1 : (-1));
				radiusTo = MathHelper.Clamp(radiusTo, RADIUS_MIN, RADIUS_MAX);
			}
			break;
		case Mode.Free:
			if (UniversalInput.inputEntities["ButtonA"].downed)
			{
				if (player.universe.mode == BuildUniverse.Modes.Camera)
				{
					scene.universe.intro.Recording_Record();
				}
				else if (player.universe.mode == BuildUniverse.Modes.Focus)
				{
					scene.universe.Modes_SetFocus_End();
				}
			}
			else if (UniversalInput.inputEntities["ButtonB"].downed && player.universe.mode == BuildUniverse.Modes.Camera)
			{
				scene.dialogs.Show("Build_Environment_Intro_Save");
			}
			if (UniversalInput.inputEntities["PlayerCam"].value2D.X != 0f)
			{
				yawTo += UniversalInput.inputEntities["PlayerCam"].value2D.X * FREE_YAW_SPEED * num * (float)((!DataManager.local.settings.cameraInvertX) ? 1 : (-1));
			}
			if (UniversalInput.inputEntities["PlayerCam"].value2D.Y != 0f)
			{
				pitchTo += UniversalInput.inputEntities["PlayerCam"].value2D.Y * FREE_PITCH_SPEED * num * (float)((!DataManager.local.settings.cameraInvertY) ? 1 : (-1));
				pitchTo = MathHelper.Clamp(pitchTo, PITCH_LIMIT_MIN, PITCH_LIMIT_MAX);
			}
			if (UniversalInput.inputEntities["PlayerMove"].value2D.X != 0f || UniversalInput.inputEntities["PlayerMove"].value2D.Y != 0f)
			{
				_transform.X = UniversalInput.inputEntities["PlayerMove"].value2D.X * FREE_SPEED_X * (float)((!DataManager.local.settings.moveInvertX) ? 1 : (-1));
				_transform.Z = UniversalInput.inputEntities["PlayerMove"].value2D.Y * FREE_SPEED_Z * (float)((!DataManager.local.settings.moveInvertY) ? 1 : (-1));
				camera.position += Vector3.Transform(_transform, camera.rotation);
			}
			break;
		}
	}
}
