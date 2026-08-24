using System;
using GKEngine;
using GKEngine.Entities;
using Game.Atoms;
using Game.Grids;
using Game.History;
using Microsoft.Xna.Framework;

namespace Game.Interactable;

public class MovingSpikes : Interactable
{
	public static string MODEL_PATH = "Content/Models/Atoms/1x1x1 Spikes/Model";

	public static float[] SPEED = new float[3] { 0.05f, 0.1f, 0.2f };

	public static string TITLE = "Moving Spikes";

	public static string DESCRIPTION = "Beware of this moving spiky death! This object moves until it hits another object. It destroys Jellies and crates on contact. This item has properties.";

	public static string PROPERTIES_DESCRIPTION = "Moving Spikes has properties that allows you to define how it moves and how fast it moves.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[1]
	{
		new AtomProperty("Speed", "This option allows you to set how fast the platform will move.", new string[3] { "Slow", "Normal", "Fast" })
	};

	public static int[] PROPERTIES_DEFAULT;

	public static Vector3 VECTOR_ZERO;

	private Vector3 _collision_error = default(Vector3);

	private float speed;

	public override int[] properties
	{
		set
		{
			if (value.Length > 0)
			{
				speed = SPEED[value[0]];
			}
		}
	}

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

	public MovingSpikes(InteractableManager oManager, AtomDefinition oDef)
		: base(oManager, oDef)
	{
	}

	public override void Load()
	{
		renderStack = scene.RenderStacks_FromName(GameMain.RENDERSTACK_SOLID);
		model = GameEngine.SceneContent.Load<MaxModel>(MODEL_PATH).Clone();
		for (int i = 0; i < model.modelParts.Count; i++)
		{
			model.modelParts[i].materialData = definition.surface;
		}
		model.Build(this);
		scene.lights.SetEffect(ref model.modelParts[0].material.effect);
		base.Load();
	}

	public override void Update(GameTime oGameTime)
	{
		for (int i = 0; i < physics.lengthStack; i++)
		{
			if (physics.stack[i] != this && physics.stack[i].physicsAlive && physics.stack[i].visible && !physics.stack[i].historyLocked && !physics.stack[i].dead && !physics.stack[i].dying && !(physics.stack[i] is MovingSpikes) && Math.Abs(X - physics.stack[i].X) < Grid.SPACING.X * 0.75f && Math.Abs(Y - physics.stack[i].Y) <= Grid.SPACING.Y * 1f && Math.Abs(Z - physics.stack[i].Z) < Grid.SPACING.Z * 0.75f)
			{
				physics.stack[i].Death();
			}
		}
	}

	public void Start()
	{
		Velocity_Set();
		PhysicsStart();
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void PhysicsUpdate(float elapsed)
	{
		if (!physicsActive || manager.universe.flipping || manager.universe.history.reversing)
		{
			return;
		}
		bool flag = false;
		position += velocity * elapsed;
		_collision_error.X = (float)Math.Sign(Math.Abs(velocity.X)) * 0.1f;
		_collision_error.Y = (float)Math.Sign(Math.Abs(velocity.Y)) * 0.1f;
		_collision_error.Z = (float)Math.Sign(Math.Abs(velocity.Z)) * 0.1f;
		for (int i = 0; i < physics.atoms.lengthAtoms; i++)
		{
			if (!physics.atoms.atoms[i].visible)
			{
				continue;
			}
			for (int j = 0; j < physics.atoms.atoms[i].area.Length; j++)
			{
				physics.atoms.atoms[i].area[j].ToPosition(ref _temp_vector);
				_physicsAtomArea.X = physics.atoms.atoms[i].position.X + _temp_vector.X;
				_physicsAtomArea.Y = physics.atoms.atoms[i].position.Y + _temp_vector.Y;
				_physicsAtomArea.Z = physics.atoms.atoms[i].position.Z + _temp_vector.Z;
				if (PhysicsCollide(ref _collision_error, ref _physicsAtomArea, ref physics.atoms.atoms[i].velocity, ref _temp_out_position, ref _temp_out_velocity))
				{
					position += _temp_out_position;
					flag = true;
				}
			}
		}
		for (int i = 0; i < physics.lengthStack; i++)
		{
			if (physics.stack[i] != this && physics.stack[i].physicsAlive && physics.stack[i] is MovingSpikes && PhysicsCollide(ref _collision_error, ref physics.stack[i]._position, ref VECTOR_ZERO, ref _temp_out_position, ref _temp_out_velocity) && physics.stack[i] is MovingSpikes)
			{
				position += _temp_out_position;
				(physics.stack[i] as MovingSpikes).Event_Collide_Platform();
				Event_Collide_Platform();
			}
		}
		if (flag)
		{
			PhysicsStop();
		}
		Event_Physics_Update(elapsed);
	}

	public static MovingSpikes FromAtom(InteractableManager oManager, AtomDefinition oDef)
	{
		return new MovingSpikes(oManager, oDef);
	}

	public void Velocity_Reverse()
	{
		velocity *= -1f;
	}

	public void Velocity_Set()
	{
		velocity = Vector3.Transform(Vector3.Backward * speed, rotation);
	}

	public override void Event_Flip_Start()
	{
		_base.matrix = matrix;
		physics.universe.history.Close(this, HistoryItem.Action.Physics);
	}

	public override void Event_Flip_End()
	{
		velocity = Vector3.Transform(velocity, Quaternion.CreateFromAxisAngle(manager.universe.flippingAxis, (float)Math.PI / 2f * (float)manager.universe.flippingAmount));
		Console.Write("Flip End");
		physics.universe.history.Open(this, HistoryItem.Action.Physics);
	}

	protected override void Event_Physics_End()
	{
		physics.universe.history.Close(this, HistoryItem.Action.Physics);
		SnapToGrid();
		Velocity_Reverse();
		PhysicsStart();
	}

	public void Event_Collide_Platform()
	{
		if (physicsActive)
		{
			physics.universe.history.Close(this, HistoryItem.Action.Physics);
			Velocity_Reverse();
			physics.universe.history.Open(this, HistoryItem.Action.Physics);
		}
	}

	static MovingSpikes()
	{
		int[] pROPERTIES_DEFAULT = new int[1];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
		VECTOR_ZERO = Vector3.Zero;
	}
}
