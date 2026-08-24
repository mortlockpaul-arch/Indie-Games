using GKEngine;
using GKEngine.Entities;
using Game.Atoms;
using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Interactable;

public class Crate : Interactable
{
	public static string MODEL_PATH = "Content/Models/Atoms/1x1x1 Crate/Model";

	public override bool physicsAlive
	{
		get
		{
			if (!dead && !dying)
			{
				return visible;
			}
			return false;
		}
	}

	public Crate(InteractableManager oManager, AtomDefinition oDef)
		: base(oManager, oDef)
	{
	}

	public override void Load()
	{
		renderStack = scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID);
		model = GameEngine.SceneContent.Load<MaxModel>(MODEL_PATH).Clone();
		model.Build(this);
		scene.lights.SetEffect(ref model.modelParts[0].material.effect);
		base.Load();
	}

	public override void Update(GameTime oGameTime)
	{
		if (!dead)
		{
			int milliseconds = oGameTime.ElapsedGameTime.Milliseconds;
			if (moving)
			{
				Move_Update(milliseconds);
			}
			else if (dying)
			{
				Death_Update(oGameTime);
			}
			else
			{
				Death_Check();
			}
		}
	}

	public void Move(int xX, int xY, int xZ, int xTime)
	{
		if (!physicsActive)
		{
			Event_Move_Start();
			moveTime = 0;
			moveTimeTotal = xTime;
			moveFrom = position;
			moveTo.X = (float)xX * Grid.SPACING.X;
			moveTo.Y = (float)xY * Grid.SPACING.Y;
			moveTo.Z = (float)xZ * Grid.SPACING.Z;
			moving = true;
		}
	}

	public void Move_Update(int elapsed)
	{
		moveTime += elapsed;
		if (moveTime >= moveTimeTotal)
		{
			Move_Done();
			return;
		}
		float ratio = (float)moveTime / (float)moveTimeTotal;
		Move_Lerp(ratio);
	}

	private void Move_Lerp(float ratio)
	{
		X = moveFrom.X + (moveTo.X - moveFrom.X) * ratio;
		Z = moveFrom.Z + (moveTo.Z - moveFrom.Z) * ratio;
		Y = moveFrom.Y + (moveTo.Y - moveFrom.Y) * ratio;
	}

	private void Move_Done()
	{
		Move_Lerp(1f);
		SnapToGrid();
		Event_Move_End();
		moving = false;
	}

	public void Death_Check()
	{
		if (Y < (float)manager.universe.atoms.grid.fromY * Grid.SPACING.Y)
		{
			Death();
		}
	}

	public override void Death()
	{
		base.Death();
		deathTime = 0f;
		visible = false;
	}

	private void Death_Update(GameTime oGameTime)
	{
		deathTime += oGameTime.ElapsedGameTime.Milliseconds;
		if (deathTime >= deathTimeTotal)
		{
			Dead();
		}
		else
		{
			Death_Lerp(deathTime / deathTimeTotal);
		}
	}

	private void Death_Lerp(float xRatio)
	{
	}

	public static Crate FromAtom(InteractableManager oManager, AtomDefinition oDef)
	{
		return new Crate(oManager, oDef);
	}

	protected override void Event_Physics_End()
	{
		base.Event_Physics_End();
		manager.universe.scene.audio.EventCues_Trigger("Sound_Crate");
	}

	public override void Event_Flip_Start()
	{
		base.Event_Flip_Start();
	}

	public override void Event_Flip_Update()
	{
		base.Event_Flip_Update();
	}

	public override void Event_Flip_End()
	{
		base.Event_Flip_End();
		SnapToGrid();
	}
}
