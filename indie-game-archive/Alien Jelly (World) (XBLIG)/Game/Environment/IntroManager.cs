using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Animation;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Data;
using Game.Dialogs;
using Game.Scenes;
using Game.Scenes.Play.Players;
using Microsoft.Xna.Framework;

namespace Game.Environment;

public class IntroManager
{
	public enum Modes
	{
		Intro,
		OneShot
	}

	public delegate void IntroDelegate();

	public const float SPEED = 0.1f;

	public const uint SLOTS = 20u;

	public const string CAMERA_NAME = "IntroCamera";

	public const float FOCUS_LENGHT = 250f;

	public List<Base3D> _stack;

	private Matrix _flipRotationMatrix = default(Matrix);

	private Base3D _flipInverse = new Base3D();

	private Base3D _flipBase = new Base3D();

	public Scene scene;

	public List<Base3D> stack;

	private Modes mode;

	public bool playing;

	public int index;

	private float time;

	private float timeTotal;

	private IntroDelegate completed;

	private Camera camera;

	private Camera cameraFrom;

	private Vector3 oneShotFromPosition;

	private Quaternion oneShotFromRotation;

	private Vector3 oneShotToPosition;

	private Quaternion oneShotToRotation;

	private IntroDelegate oneShotCompleted;

	public Base3D current
	{
		get
		{
			if (index >= 0 && index < stack.Count)
			{
				return stack[index];
			}
			return null;
		}
	}

	public IntroManager(Scene oScene)
	{
		scene = oScene;
		Init();
	}

	private void Init()
	{
		_stack = new List<Base3D>();
		stack = new List<Base3D>();
		Clear();
		camera = new Camera("IntroCamera", GameEngine.Graphics.GraphicsDevice.Viewport, scene.cameras);
		scene.cameras.Add(camera);
	}

	public void Clear()
	{
		_stack.Clear();
		stack.Clear();
	}

	public void Dispose()
	{
		Clear();
		stack = null;
		_stack = null;
		camera.manager.Remove(camera);
	}

	public void Update(GameTime oGameTime)
	{
		if (!playing)
		{
			return;
		}
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (mode == Modes.Intro)
		{
			if (time >= timeTotal)
			{
				Lerp(1f);
				index++;
				if (index >= stack.Count - 1)
				{
					playing = false;
					scene.cameras.SetActive(cameraFrom.name);
					completed();
				}
				else
				{
					time = 0f;
					timeTotal = GetTimeTotal();
				}
			}
			else
			{
				Lerp(time / timeTotal);
			}
		}
		else if (mode == Modes.OneShot)
		{
			if (time >= timeTotal)
			{
				OneShot_Lerp(1f);
				playing = false;
				oneShotCompleted();
			}
			else
			{
				OneShot_Lerp(time / timeTotal);
			}
		}
	}

	public void Start(IntroDelegate oCompleted)
	{
		completed = oCompleted;
		mode = Modes.Intro;
		cameraFrom = scene.cameras.camera;
		scene.cameras.SetActive("IntroCamera");
		time = 0f;
		index = 0;
		if (stack.Count >= 2)
		{
			timeTotal = GetTimeTotal();
			Lerp(0f);
			playing = true;
		}
		else if (stack.Count == 1)
		{
			camera.position = stack[0].position;
			camera.rotation = stack[0].rotation;
			playing = false;
			scene.cameras.SetActive(cameraFrom.name);
			completed();
		}
		else
		{
			camera.position = Vector3.Zero;
			camera.rotation = Quaternion.Identity;
			playing = false;
			scene.cameras.SetActive(cameraFrom.name);
			completed();
		}
	}

	private void Lerp(float xRatio)
	{
		float amount = Tween.EaseInOut(xRatio);
		Vector3.Lerp(ref stack[index]._position, ref stack[index + 1]._position, amount, out camera._position);
		camera.rotation = Quaternion.Lerp(stack[index].rotation, stack[index + 1].rotation, amount);
		camera.Update_View();
	}

	public void Stop()
	{
		playing = false;
		index = 0;
		scene.cameras.SetActive(cameraFrom.name);
	}

	public void SetStartCamera(PlayerCamera oCam)
	{
		if (stack.Count > 0)
		{
			scene.cameras.SetActive("IntroCamera");
			Vector3 vector = stack[stack.Count - 1].position - oCam.manager.position;
			if (vector.Length() == 0f)
			{
				vector.X = 1f;
				vector.Y = 1f;
				vector.Z = 1f;
			}
			vector.Normalize();
			oCam.Set((float)Math.Atan2(vector.X, vector.Z), (float)Math.Atan2(vector.Y, Math.Sqrt(Math.Pow(vector.X, 2.0) + Math.Pow(vector.Z, 2.0))), oCam.radius);
			stack[stack.Count - 1] = new Base3D(oCam.camera.position, oCam.camera.rotation, Vector3.One);
			camera.Set(stack[0]);
		}
	}

	private float GetTimeTotal()
	{
		Vector3 value = stack[index].position + Vector3.Transform(Vector3.Forward * 250f, stack[index].rotation);
		Vector3 value2 = stack[index + 1].position + Vector3.Transform(Vector3.Forward * 250f, stack[index + 1].rotation);
		return Math.Max(Vector3.Distance(value, value2), Vector3.Distance(stack[index].position, stack[index + 1].position)) / 0.1f;
	}

	public void OneShot_Start(float pTime, IntroDelegate pCompleted, Vector3 pFromPos, Quaternion pFromRot, Vector3 pToPos, Quaternion pToRot)
	{
		timeTotal = pTime;
		oneShotCompleted = pCompleted;
		oneShotFromPosition = pFromPos;
		oneShotFromRotation = pFromRot;
		oneShotToPosition = pToPos;
		oneShotToRotation = pToRot;
		mode = Modes.OneShot;
		time = 0f;
		OneShot_Lerp(0f);
		scene.cameras.SetActive("IntroCamera");
		playing = true;
	}

	private void OneShot_Lerp(float xRatio)
	{
		float amount = Tween.EaseInOut(xRatio);
		camera.position = Vector3.Lerp(oneShotFromPosition, oneShotToPosition, amount);
		camera.rotation = Quaternion.Lerp(oneShotFromRotation, oneShotToRotation, amount);
		camera.Update_View();
	}

	private void Recording_Start()
	{
		_stack.Clear();
	}

	public void Recording_Record()
	{
		if ((long)_stack.Count < 20L)
		{
			(scene as BuildScene).postWhiteOut.active = true;
			(scene as BuildScene).postWhiteOut.Anim_Out();
			_stack.Add(new Base3D(scene.cameras.camera.position, scene.cameras.camera.rotation, Vector3.One));
		}
	}

	public void Recording_End(bool xCommit)
	{
		if (xCommit)
		{
			stack.Clear();
			for (int i = 0; i < _stack.Count; i++)
			{
				stack.Add(_stack[i]);
			}
		}
	}

	public void FromData(DataLevel oData)
	{
		stack.Clear();
		index = 0;
		for (int i = 0; i < oData.intro.Count; i++)
		{
			stack.Add(new Base3D(oData.intro[i].position, oData.intro[i].rotation, Vector3.One));
		}
	}

	public void ToData(DataLevel oData)
	{
		oData.intro.Clear();
		for (int i = 0; i < stack.Count; i++)
		{
			oData.intro.Add(new DataKeyFrame(stack[i].position, stack[i].rotation));
		}
	}

	public void Flip(Vector3 vAxis, float xAmount)
	{
		Matrix.CreateFromAxisAngle(ref vAxis, (float)Math.PI / 2f * xAmount, out _flipRotationMatrix);
		_flipBase.matrix = Matrix.Multiply(_flipInverse.matrix, _flipRotationMatrix);
		camera.position = _flipBase.position;
		camera.rotation = _flipBase.rotation;
	}

	public void Flip_Start(Matrix pInverse)
	{
		_flipInverse.matrix = Matrix.Multiply(camera.matrix, pInverse);
	}

	public void Flip_End()
	{
	}

	public void Menu_PopulateOptions(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		list.Add(new DialogMenuOption("Make Cinimatic", delegate
		{
			Recording_Start();
			(scene as BuildScene).universe.Modes_SetCamera_Start();
		}));
		list.Add(new DialogMenuOption("Clear Cinimatic", delegate(Dialog dialog)
		{
			dialog.manager.Show("Build_Environment_Intro_Clear");
		}));
		if (stack.Count > 1)
		{
			list.Add(new DialogMenuOption("View Cinimatic", delegate
			{
				(scene as BuildScene).universe.Modes_SetCinimatic();
			}));
		}
		oMenu.Options_Set(list);
	}
}
