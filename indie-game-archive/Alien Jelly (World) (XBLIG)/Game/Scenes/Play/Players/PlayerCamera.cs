using System;
using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Input;
using Game.Data;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Play.Players;

public class PlayerCamera
{
	public enum Mode
	{
		Rotate,
		Focus
	}

	public static string CAMERA_NAME = "PlayCam";

	public static float YAW_SPEED = 0.005f;

	public static float YAW_FLOOR = 0.001f;

	public static float PITCH_SPEED = 0.005f;

	public static float PITCH_LIMIT_MIN = -1.5607964f;

	public static float PITCH_LIMIT_MAX = 1.5607964f;

	public static float PITCH_FLOOR = 0.001f;

	public static float RADUIS_SPEED = 5f;

	public static float RADIUS_MAX = 750f;

	public static float RADIUS_MIN = 200f;

	public static float MOVING_TIME = 500f;

	public PlayerManager manager;

	public Camera camera;

	public PlayScene scene;

	public Mode mode;

	public bool active = true;

	public float radius = RADIUS_MIN;

	public bool zoomed;

	public float pitch;

	public float yaw;

	public Vector3 position = Vector3.Zero;

	private float pitchTo;

	private float yawTo;

	private Vector3 positionTo = Vector3.Zero;

	private List<InputEntity> inputStick;

	private List<int> inputStickIndex;

	private InputEntity inputClick;

	private int inputCount;

	private bool inputSet;

	private bool inputMoving;

	private bool inputMovingPrevious;

	public PlayerCamera(PlayerManager oManager)
	{
		manager = oManager;
		scene = manager.scene;
		camera = scene.cameras.cameras[CAMERA_NAME];
		camera.position = Vector3.Zero;
		camera.rotation = Quaternion.Identity;
		scene.cameras.SetActive(CAMERA_NAME);
		Input_Set();
	}

	public void Update(GameTime oGameTime)
	{
		if (!active)
		{
			return;
		}
		float num = oGameTime.ElapsedGameTime.Milliseconds;
		if (!inputMoving && inputMovingPrevious && DataManager.local.settings.cameraSnapping)
		{
			yawTo = (float)Math.Round(yawTo / ((float)Math.PI / 2f)) * (float)Math.PI * 0.5f;
		}
		float amount = Math.Min(num / 100f, 1f);
		if (yaw != yawTo)
		{
			yaw = MathHelper.Lerp(yaw, yawTo, amount);
			if (Math.Abs(yawTo - yaw) < YAW_FLOOR)
			{
				yaw = yawTo;
			}
		}
		if (pitch != pitchTo)
		{
			pitch = MathHelper.Lerp(pitch, pitchTo, amount);
			if (Math.Abs(pitchTo - pitch) < PITCH_FLOOR)
			{
				pitch = pitchTo;
			}
		}
		camera.rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw) * Quaternion.CreateFromAxisAngle(Vector3.Left, pitch);
		positionTo = manager.position;
		position = Vector3.Lerp(position, positionTo, amount);
		camera.position = position + camera.unit * radius * -1f;
		camera.Update_View();
		camera.focalLength = radius;
	}

	public void Refresh()
	{
		camera.rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw) * Quaternion.CreateFromAxisAngle(Vector3.Left, pitch);
		positionTo = manager.position;
		position = positionTo;
		camera.position = position + camera.unit * radius * -1f;
		camera.Update_View();
	}

	public void Set(float xYaw, float xPitch, float xRadius)
	{
		yaw = xYaw;
		yawTo = xYaw;
		pitch = xPitch;
		pitchTo = xPitch;
		radius = xRadius;
		Update(new GameTime());
	}

	public void Set()
	{
		Set(yaw, pitch, radius);
	}

	public void Dispose()
	{
	}

	public void Input_Set()
	{
		inputStick = new List<InputEntity>();
		inputStickIndex = new List<int>();
		inputClick = new InputEntity(InputEntity.Type.Button, "PlayerCamToggle", InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputClick);
		inputClick.Add(new InputButton(GamePadButton.ShoulderLeft));
		inputClick.active = true;
		inputSet = true;
	}

	public void Input_Activate(int xIndex)
	{
		InputEntity inputEntity = new InputEntity(InputEntity.Type.Analog2D, "PlayerCam_" + xIndex, InputEntity.Scope.Scene);
		inputEntity.Add(new InputAnalog2D(GamePadAnalog2D.Right, xIndex));
		UniversalInput.InputEntity_Add(inputEntity);
		inputEntity.active = true;
		inputStick.Add(inputEntity);
		inputStickIndex.Add(xIndex);
		inputCount = inputStick.Count;
	}

	public void Input_Deactivate(int xIndex)
	{
		InputEntity inputEntity = inputStick[inputStickIndex.IndexOf(xIndex)];
		UniversalInput.InputEntity_Remove(inputEntity);
		inputStick.Remove(inputEntity);
		inputStickIndex.RemoveAt(inputStickIndex.IndexOf(xIndex));
		inputEntity.active = false;
		inputEntity = null;
		inputCount = inputStick.Count;
	}

	public void Input_Update(GameTime oGameTime)
	{
		float num = oGameTime.ElapsedGameTime.Milliseconds;
		if (!inputSet || !active || manager.universe.history.reversing)
		{
			return;
		}
		if (inputClick.downed)
		{
			zoomed = true;
			radius = RADIUS_MAX;
		}
		else if (inputClick.pressed)
		{
			zoomed = false;
			radius = RADIUS_MIN;
		}
		bool flag = false;
		for (int i = 0; i < inputCount; i++)
		{
			if (inputStick[i].value2D.X != 0f)
			{
				yawTo += inputStick[i].value2D.X * YAW_SPEED * num * (float)((!DataManager.local.settings.cameraInvertX) ? 1 : (-1));
			}
			if (inputStick[i].value2D.Y != 0f)
			{
				pitchTo += inputStick[i].value2D.Y * PITCH_SPEED * num * (float)((!DataManager.local.settings.cameraInvertY) ? 1 : (-1));
				pitchTo = MathHelper.Clamp(pitchTo, PITCH_LIMIT_MIN, PITCH_LIMIT_MAX);
			}
			inputMovingPrevious = inputMoving;
			if (inputStick[i].value2D.X != 0f || inputStick[i].value2D.Y != 0f)
			{
				flag = true;
			}
		}
		inputMoving = flag;
	}
}
