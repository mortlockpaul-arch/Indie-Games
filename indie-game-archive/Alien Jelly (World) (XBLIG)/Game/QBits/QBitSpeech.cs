using GKEngine;
using GKEngine.Animation;
using GKEngine.Entities;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.QBits;

public class QBitSpeech : Entity3D
{
	public delegate void SpeechDelegate();

	private const string PATH_MODEL = "Content/Models/QBits/Speech/Model";

	private const float TIME_SHOW = 200f;

	private const float TIME_HIDE = 150f;

	private const float SCALE = 3f;

	public QBit qbit;

	private MaxModel model;

	private EffectParameter effectUVGrid;

	private EffectParameter effectPosition;

	private EffectParameter effectTexture;

	private EffectParameter effectBackground;

	private EffectParameter effectScale;

	private EffectParameter effectTime;

	private bool showing;

	private bool hiding;

	private bool waiting;

	private float time;

	private float timeWait;

	private SpeechDelegate __ready;

	private SpeechDelegate __done;

	public QBitSpeech(QBit pQBit)
	{
		qbit = pQBit;
		scene = qbit.scene;
		visible = false;
	}

	public override void Load()
	{
		base.Load();
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/QBits/Speech/Model").Clone();
		model.Build(this);
		effectUVGrid = model.PartFromName("Model").material.effect.Parameters["UVGrid"];
		effectPosition = model.PartFromName("Model").material.effect.Parameters["Vector"];
		effectTexture = model.PartFromName("Model").material.effect.Parameters["TextureDiffuse"];
		effectScale = model.PartFromName("Model").material.effect.Parameters["Scale"];
		effectBackground = model.PartFromName("Model").material.effect.Parameters["TextureBackground"];
		effectTime = model.PartFromName("Model").material.effect.Parameters["Time"];
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS).Add(guid.value, this);
	}

	public override void Dispose()
	{
		base.Dispose();
		scene.RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS).Add(guid.value, this);
		model.Dispose();
		effectBackground = null;
		effectUVGrid = null;
		model = null;
	}

	public override void Render(GameTime oGameTime)
	{
		base.Render(oGameTime);
		if (visible)
		{
			Material.RenderStates_Set(Material.State.AlphaNoDepth);
			model.Render(scene.cameras.camera);
			Material.RenderStates_Reset();
		}
	}

	public void Update(GameTime oGameTime)
	{
		if (visible)
		{
			effectPosition.SetValue(qbit._position);
			effectTime.SetValue((float)oGameTime.TotalGameTime.TotalMilliseconds);
		}
		Show_Update(oGameTime);
		Wait_Update(oGameTime);
		Hide_Update(oGameTime);
	}

	public void Show(Texture2D pSheet, Texture2D pBackground, int pX, int pY, int pWait, SpeechDelegate pCallbackReady, SpeechDelegate pCallbackDone)
	{
		if (qbit.manager.universe.players.primaryPlayer.qbit != qbit)
		{
			qbit.manager.universe.players.primaryPlayer.QBit_Set(qbit);
		}
		timeWait = pWait;
		__ready = pCallbackReady;
		__done = pCallbackDone;
		effectTexture.SetValue(pSheet);
		effectBackground.SetValue(pBackground);
		effectUVGrid.SetValue(new Vector2(pX, pY));
		effectPosition.SetValue(qbit._position);
		visible = true;
		showing = true;
		waiting = false;
		hiding = false;
		Show_Lerp(0f);
		time = 0f;
	}

	public void Hide()
	{
		hiding = true;
		time = 0f;
	}

	public void Wait()
	{
		waiting = true;
		time = 0f;
	}

	public void Halt()
	{
		showing = false;
		waiting = false;
		hiding = false;
		__ready = null;
		__done = null;
		visible = false;
		time = 0f;
	}

	private void Show_Update(GameTime oGameTime)
	{
		if (showing)
		{
			time += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (time >= 200f)
			{
				Show_Lerp(1f);
				showing = false;
				(scene as PlayScene).audio.EventCues_Trigger("Speech_Bubble");
				Wait();
			}
			else
			{
				Show_Lerp(time / 200f);
			}
		}
	}

	private void Show_Lerp(float xRatio)
	{
		float num = Tween.EaseIn(xRatio);
		effectScale.SetValue(num * 3f);
	}

	private void Wait_Update(GameTime oGameTime)
	{
		if (waiting)
		{
			time += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (time >= timeWait)
			{
				waiting = false;
				__ready();
			}
		}
	}

	private void Hide_Update(GameTime oGameTime)
	{
		if (!hiding)
		{
			return;
		}
		time += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
		if (time >= 150f)
		{
			Hide_Lerp(1f);
			hiding = false;
			visible = false;
			if (__done != null)
			{
				__done();
			}
		}
		else
		{
			Hide_Lerp(time / 200f);
		}
	}

	private void Hide_Lerp(float xRatio)
	{
		float num = Tween.EaseIn(1f - xRatio);
		effectScale.SetValue(num * 3f);
	}
}
