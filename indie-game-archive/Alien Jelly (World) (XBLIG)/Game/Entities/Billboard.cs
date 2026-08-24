using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Entities;

public class Billboard : Entity3D
{
	public const string PATH_MODEL = "Content/Models/SFX/Particles/Model";

	public const string NAME_EFFECT = "Billboard_Sequence";

	public MaxModel model;

	public MaxModelPart part;

	public string path;

	public EffectParameter effectPosition;

	public EffectParameter effectScale;

	public EffectParameter effectTint;

	public EffectParameter effectFrame;

	private int _frame;

	public int frameStart;

	public int frameEnd;

	public int frameCount;

	public int grid;

	public Texture2D texture;

	public float time;

	public float timeTotal;

	public bool loop;

	public bool hideOnComplete;

	public bool playing;

	public EntityStack renderStack;

	private Vector4 _tint;

	public int frame
	{
		get
		{
			return _frame;
		}
		set
		{
			_frame = value;
			effectFrame.SetValue(_frame);
		}
	}

	public Vector4 tint
	{
		get
		{
			return _tint;
		}
		set
		{
			_tint = value;
			part.material.effect.Parameters["Tint"].SetValue(_tint);
		}
	}

	public Billboard(Scene xScene, string xPath, int xGrid, int xFrameCount, EntityStack oRenderStack)
	{
		scene = xScene;
		path = xPath;
		grid = xGrid;
		frameCount = xFrameCount;
		renderStack = oRenderStack;
		Init();
	}

	public virtual void Init()
	{
		Load();
		frame = 0;
		renderStack.Add(guid.value, this);
		visible = false;
	}

	public override void Dispose()
	{
		texture = null;
		renderStack.Remove(guid.value, this);
		model.Dispose();
		base.Dispose();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/SFX/Particles/Model").Clone();
		part = model.modelParts[0];
		part.materialData = "Billboard_Sequence:Path=";
		model.Build(this);
		effectScale = part.material.effect.Parameters["Scale"];
		effectTint = part.material.effect.Parameters["Tint"];
		effectPosition = part.material.effect.Parameters["Vector"];
		effectFrame = part.material.effect.Parameters["Frame"];
		if (texture == null)
		{
			texture = GameEngine.SceneContent.Load<Texture2D>("Content/" + path);
		}
		part.material.effect.Parameters["TextureDiffuse"].SetValue(texture);
		part.material.effect.Parameters["Grid"].SetValue((float)grid);
		part.material.effect.Parameters["GridMulti"].SetValue(1f / (float)grid);
		base.Load();
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible && model != null)
		{
			model.Render(matrix, scene.cameras.camera);
		}
	}

	public void Update(TimeSpan oElapsedTime)
	{
		if (!playing)
		{
			return;
		}
		time += oElapsedTime.Milliseconds;
		if (time >= timeTotal)
		{
			if (loop)
			{
				time %= timeTotal;
			}
			else
			{
				time = timeTotal;
				playing = false;
				if (hideOnComplete)
				{
					visible = false;
				}
			}
		}
		Lerp(time / timeTotal);
	}

	public void Lerp(float ratio)
	{
		frame = frameStart + (int)Math.Round(ratio * (float)(frameEnd - frameStart));
	}

	public void Stop()
	{
		playing = false;
	}

	public void GotoAndStop(Vector3 oPos, float xScale, int xFrame)
	{
		effectPosition.SetValue(oPos);
		effectScale.SetValue(xScale);
		xFrame = Math.Abs(xFrame);
		xFrame %= frameCount;
		frame = xFrame;
		visible = true;
	}

	public void GotoAndPlay(Vector3 oPos, float xScale, int xFrameStart, int xFrameEnd, float xTime, bool xLoop, bool xHideOnComplete)
	{
		effectPosition.SetValue(oPos);
		effectScale.SetValue(xScale);
		frame = xFrameStart;
		frameStart = xFrameStart;
		frameEnd = xFrameEnd;
		loop = xLoop;
		hideOnComplete = xHideOnComplete;
		time = 0f;
		timeTotal = xTime;
		visible = true;
		playing = true;
	}
}
