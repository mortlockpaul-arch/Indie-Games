using System;
using GKEngine;
using GKEngine.Entities;
using Game.Grids;
using Game.QBits;
using Game.Scenes.Play;
using Game.Scenes.Play.Players;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomSwitch : AtomSingle, IGridable, IRenderable
{
	public enum Types
	{
		Flip,
		Holograms
	}

	public static string[] COLORS = new string[4] { "0.9921|0.1523|0.0703|1", "0|0.6601|0.1992|1", "0.0078|0.2773|0.9921|1", "0.9921|0.9921|0.1992|1" };

	public static string[] COLORS_RIM = new string[4] { "1|0.3046875|0.140625", "0|1|0.3984375", "0.015625|0.5546875|1", "1|1|0.3984375" };

	public static Range BUTTON_DISPLACE = new Range(-8f, -2f);

	public static Range[] ARROW_TIME = new Range[3]
	{
		new Range(0f, 1000f),
		new Range(0f, 2000f),
		new Range(0f, 1000f)
	};

	public static float ARROW_WAITTIME = 500f;

	public static float ARROW_LERPTIME = 300f;

	private Vector3 _buttonPosition = default(Vector3);

	public Types type;

	private MaxModelPart housing;

	private MaxModelPart button;

	private MaxModelRenderable arrow;

	private MaxModel arrowModel;

	private Base3D arrowTransform;

	private Base3D arrowTransformLocal;

	private float arrowTime;

	private bool arrowActive = true;

	private byte arrowState;

	private Range arrowRotation;

	public Atom[] children;

	public Base3D focus;

	public Base3D focusBase = new Base3D();

	public AtomSwitch(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
		type = (definition as AtomSwitchDefinition).switchType;
	}

	public override void Load()
	{
		useMaterials = false;
		base.Load();
		trigger = new AtomTrigger(this, Triggered);
		button = model.modelParts[1];
		housing = model.modelParts[0];
		button.hasLocal = true;
		button.local = Matrix.Identity;
		button.material.effect.Parameters["Ks"].SetValue(1);
		button.material.effect.Parameters["SpecExpon"].SetValue(80);
		Arrow_Load();
		StateSet(0);
	}

	protected override void LoadManualSurfaces()
	{
		base.LoadManualSurfaces();
		model.modelParts[0].materialData = definition.surface;
		model.modelParts[1].materialData = "Atom_Single_ColorE:Path=:Kr=0.5:RimMix=1:RimAmount=0.1:Color=" + COLORS[(int)(definition as AtomSwitchDefinition).qbitType];
	}

	public override void Dispose()
	{
		base.Dispose();
		Arrow_Dispose();
		button.Dispose();
		housing.Dispose();
		button = null;
		housing = null;
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		Arrow_Update(oGameTime);
	}

	public override void InitPlay()
	{
		base.InitPlay();
		StateCheckIfOn();
	}

	public override void InitBuild()
	{
		base.InitBuild();
		StateSet(1);
	}

	public bool Triggered(object oTriggerer)
	{
		bool flag = false;
		QBit qBit = oTriggerer as QBit;
		flag = oTriggerer != null && qBit.type == (definition as AtomSwitchDefinition).qbitType && state == 1 && qBit.moveSwitchHit;
		if (oTriggerer != null && qBit.type == (definition as AtomSwitchDefinition).qbitType && state == 1)
		{
			StateSet(0);
		}
		return flag;
	}

	public void Button_Lerp(float xRatio)
	{
		_buttonPosition.Y = BUTTON_DISPLACE.Lerp(xRatio);
		button.local = Matrix.CreateTranslation(_buttonPosition);
	}

	public override void StateSet(int xState)
	{
		base.StateSet(xState);
		if (model != null)
		{
			switch (state)
			{
			case 0:
				Button_Lerp(0f);
				arrowModel.visible = false;
				break;
			case 1:
				Button_Lerp(1f);
				arrowModel.visible = true;
				break;
			}
		}
	}

	public void StateCheckIfOn()
	{
		if (Vector3.Dot(matrix.Up, Vector3.Up) > 0.9f)
		{
			StateSet(1);
		}
		else
		{
			StateSet(0);
		}
	}

	public void Arrow_Load()
	{
		arrowRotation = new Range(0f, (float)(Math.PI / 2.0 * (double)(float)(definition as AtomSwitchDefinition).value));
		arrowTransform = new Base3D();
		arrowTransformLocal = new Base3D(new Vector3(0f, 10f, 0f), Quaternion.Identity, Vector3.One);
		arrowModel = GameEngine.SceneContent.Load<MaxModel>("Content/Models/Atoms/" + definition.shape + "/Arrow_" + (definition as AtomSwitchDefinition).value).Clone();
		arrowModel.PartFromName("Model").materialData = "Hologram:Path=:Color=" + COLORS[(int)(definition as AtomSwitchDefinition).qbitType];
		arrow = new MaxModelRenderable(manager.scene, arrowModel);
		arrow.Init(arrowTransform, GameMain.RENDERSTACK_ADD);
	}

	public void Arrow_Update(GameTime oGameTime)
	{
		if (arrowActive)
		{
			arrowTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (arrowState == 0 && arrowTime > ARROW_TIME[(definition as AtomSwitchDefinition).value - 1].to)
			{
				arrowTime = 0f;
				arrowState = 1;
			}
			else if (arrowState == 1 && arrowTime > ARROW_WAITTIME)
			{
				arrowTime = 0f;
				arrowState = 2;
			}
			else if (arrowState == 2 && arrowTime > ARROW_LERPTIME)
			{
				arrowTime = 0f;
				arrowState = 0;
			}
			if (type == Types.Holograms)
			{
				if (arrowState == 1)
				{
					arrowTransformLocal.scaleAll = 1f - arrowTime / ARROW_WAITTIME;
				}
				else if (arrowState == 2)
				{
					arrowTransformLocal.scaleAll = arrowTime / ARROW_LERPTIME;
				}
				arrowTransformLocal.rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, -0.01f);
			}
			else if (arrowState == 0)
			{
				Arrow_Lerp(ARROW_TIME[(definition as AtomSwitchDefinition).value - 1].Ratio(arrowTime));
			}
			else if (arrowState == 2)
			{
				arrowTransformLocal.rotation = Quaternion.Lerp(arrowTransformLocal.rotation, Quaternion.CreateFromAxisAngle(Vector3.Left, arrowRotation.from), arrowTime / ARROW_WAITTIME);
			}
		}
		arrowTransform.matrix = Matrix.Multiply(arrowTransformLocal.matrix, matrix);
	}

	public void Arrow_Lerp(float xRatio)
	{
		arrowTransformLocal.rotation = Quaternion.CreateFromAxisAngle(Vector3.Left, arrowRotation.Lerp(xRatio));
	}

	public void Arrow_Dispose()
	{
		arrowActive = false;
		if (arrow != null)
		{
			arrow.Dispose();
		}
		if (arrowModel != null)
		{
			arrowModel.Dispose();
			arrowModel = null;
		}
	}

	private void Hologram_Trigger()
	{
		PlayUniverse oUniverse = (manager as PlayAtomManager).universe;
		if (focus != null)
		{
			oUniverse.paused = true;
			oUniverse.players.paused = true;
			oUniverse.intro.OneShot_Start(600f, delegate
			{
				oUniverse.paused = false;
				Hologram_Trigger_Continue();
				oUniverse.intro.OneShot_Start(900f, delegate
				{
					oUniverse.paused = true;
					oUniverse.intro.OneShot_Start(600f, delegate
					{
						oUniverse.paused = false;
						oUniverse.players.paused = false;
						oUniverse.scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
					}, oUniverse.scene.cameras.camera.position, oUniverse.scene.cameras.camera.rotation, oUniverse.players.camera.camera.position, oUniverse.players.camera.camera.rotation);
				}, focus.position, focus.rotation, focus.position, focus.rotation);
			}, oUniverse.scene.cameras.camera.position, oUniverse.scene.cameras.camera.rotation, focus.position, focus.rotation);
		}
		else
		{
			Hologram_Trigger_Continue();
		}
	}

	private void Hologram_Trigger_Continue()
	{
		PlayUniverse universe = (manager as PlayAtomManager).universe;
		for (int i = 0; i < children.Length; i++)
		{
			(children[i] as AtomInstancedHologram).Toggle();
		}
		for (int i = 0; i < universe.physics.stack.Count; i++)
		{
			if (!universe.physics.stack[i].dead && !universe.physics.stack[i].dying)
			{
				universe.physics.stack[i].physicsCheckActive = true;
			}
		}
	}

	public override void Event_Triggered_Start(object oTriggerer)
	{
		StateSet(0);
		if (type == Types.Flip)
		{
			(manager as PlayAtomManager).universe.Flip(matrix.Left, (definition as AtomSwitchDefinition).value, this);
		}
		else if (type == Types.Holograms)
		{
			Hologram_Trigger();
		}
	}

	public override void Event_Flip_End()
	{
		base.Event_Flip_End();
		StateCheckIfOn();
	}

	public override void Event_Flip_Start()
	{
		base.Event_Flip_Start();
		if (focus != null)
		{
			focusBase.matrix = Matrix.Multiply(focus.matrix, manager.inverse);
		}
	}

	public override void Event_Flip_Update()
	{
		base.Event_Flip_Update();
		arrowTransform.matrix = Matrix.Multiply(arrowTransformLocal.matrix, matrix);
		if (focus != null)
		{
			focus.matrix = Matrix.Multiply(focusBase.matrix, manager._flipRotationMatrix);
		}
	}
}
