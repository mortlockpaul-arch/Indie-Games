using System;
using GKEngine.Entities;
using Game.Atoms;
using Game.Grids;
using Game.History;
using Game.QBits;
using Microsoft.Xna.Framework;

namespace Game.Physics;

public class PhysicsItem : Entity3D, IReversible
{
	public static Vector3 FARAWAY = new Vector3(0f, Grid.SPACING.Y * 100f, 0f);

	public static Vector3 GRID_ERROR = new Vector3(0.1f, 0f, 0.1f);

	protected Vector3 _physicsAtomArea = default(Vector3);

	public Base3D _base = new Base3D();

	protected Vector3 _temp_vector = default(Vector3);

	protected Vector3 _temp_vector_at = default(Vector3);

	protected IGridable _temp_gridable;

	protected PhysicsItem _temp_item;

	protected Vector3 _temp_collision_delta = default(Vector3);

	protected BoundingBox _temp_collision_box = default(BoundingBox);

	protected Vector3 _temp_collision_velocity = default(Vector3);

	protected Vector3 _temp_collision_unit = default(Vector3);

	protected Vector3 _temp_collision_boxCenter = default(Vector3);

	protected Vector3 _temp_collision_volume = default(Vector3);

	protected Ray _temp_collision_ray = default(Ray);

	protected Vector3 _temp_out_position = default(Vector3);

	protected Vector3 _temp_out_velocity = default(Vector3);

	protected PhysicsItem[] _temp_out_items = new PhysicsItem[100];

	public PhysicsManager physics;

	public bool physicsActive;

	public bool physicsFalling;

	public bool physicsFlipNoRotate;

	public Vector3 physicsPreviousPosition = default(Vector3);

	public bool physicsCheckActive = true;

	public bool moving;

	public bool dead;

	public bool dying;

	public Vector3 moveFrom = default(Vector3);

	public Vector3 moveTo = default(Vector3);

	protected Quaternion moveRotationFrom;

	protected Quaternion moveRotationTo;

	protected int moveTime;

	protected int moveTimeTotal = 200;

	protected float deathTime;

	protected float deathTimeTotal = 1000f;

	public Vector3 velocity = default(Vector3);

	public Vector3 velocityCollide = default(Vector3);

	public bool historyLocked;

	public virtual bool physicsAlive => true;

	public int gridX => (int)(Math.Round(Math.Abs(_position.X / Grid.SPACING.X)) * (double)Math.Sign(_position.X));

	public int gridY => (int)(Math.Round(Math.Abs(_position.Y / Grid.SPACING.Y)) * (double)Math.Sign(_position.Y));

	public int gridZ => (int)(Math.Round(Math.Abs(_position.Z / Grid.SPACING.Z)) * (double)Math.Sign(_position.Z));

	public virtual int[] properties
	{
		set
		{
		}
	}

	public virtual void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction)
	{
		if (oAction == HistoryItem.Action.Move)
		{
			oItem.position = _position;
			oItem.rotation = _rotation;
		}
		if (oAction == HistoryItem.Action.Physics)
		{
			oItem.position = _position;
			oItem.rotation = _rotation;
			oItem.velocity = velocity;
		}
		if (oAction == HistoryItem.Action.Death)
		{
			oItem.position = _position;
			oItem.rotation = _rotation;
			oItem.value = deathTime;
		}
	}

	public virtual void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		if (oItem.action == HistoryItem.Action.Move)
		{
			History_Reverse_Move_Lerp(ref oItem, xRatio, oGameTime);
		}
		if (oItem.action == HistoryItem.Action.Physics)
		{
			History_Reverse_Physics_Lerp(ref oItem, xRatio, oGameTime);
		}
		if (oItem.action == HistoryItem.Action.Death)
		{
			History_Reverse_Death_Lerp(ref oItem, xRatio, oGameTime);
		}
	}

	public virtual bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		bool result = false;
		if (oAction == HistoryItem.Action.Move)
		{
			result = true;
		}
		if (oAction == HistoryItem.Action.Death)
		{
			result = true;
		}
		return result;
	}

	protected virtual void History_Reverse_Move_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		if (xRatio == 0f || xRatio == 1f)
		{
			moving = false;
		}
		else
		{
			moving = true;
		}
		position = Vector3.Lerp(oItem.end.position, oItem.start.position, xRatio);
		rotation = Quaternion.Lerp(oItem.end.rotation, oItem.start.rotation, xRatio);
		if (float.IsNaN(rotation.W))
		{
			rotation = (((double)xRatio >= 0.5) ? oItem.end.rotation : oItem.start.rotation);
		}
	}

	protected virtual void History_Reverse_Physics_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		velocity = Vector3.Lerp(oItem.end.velocity, oItem.start.velocity, xRatio);
		position = Vector3.Lerp(oItem.end.position, oItem.start.position, xRatio);
		rotation = Quaternion.Lerp(oItem.end.rotation, oItem.start.rotation, xRatio);
	}

	protected virtual void History_Reverse_Death_Lerp(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
		position = oItem.start.position;
		rotation = oItem.start.rotation;
		deathTime = MathHelper.Lerp(oItem.end.value, oItem.start.value, xRatio);
		visible = true;
		scaleX = xRatio;
		scaleY = xRatio;
		scaleZ = xRatio;
	}

	public virtual void PhysicsStart()
	{
		physicsActive = true;
		Event_Physics_Start();
	}

	public virtual void PhysicsStop()
	{
		physicsActive = false;
		Event_Physics_End();
	}

	public virtual void PhysicsUpdate(float elapsed)
	{
		if (physicsActive)
		{
			velocity.Y += PhysicsManager.GRAVITY * elapsed;
			velocity.Y = MathHelper.Clamp(velocity.Y, 0f - PhysicsManager.VELOCITY_MAX, PhysicsManager.VELOCITY_MAX);
			position += velocity * elapsed;
			if (PhysicsCollideCheck())
			{
				PhysicsStop();
			}
			Event_Physics_Update(elapsed);
		}
		else if (physicsCheckActive && !PhysicsGroundCheck())
		{
			PhysicsStart();
		}
	}

	public virtual bool PhysicsCollideCheck()
	{
		bool result = false;
		for (int i = 0; i < physics.atoms.lengthAtoms; i++)
		{
			if (!physics.atoms.atoms[i].visible || !physics.atoms.atoms[i].definition.playGrid)
			{
				continue;
			}
			for (int j = 0; j < physics.atoms.atoms[i].area.Length; j++)
			{
				physics.atoms.atoms[i].area[j].ToPosition(ref _temp_vector);
				_physicsAtomArea.X = physics.atoms.atoms[i].position.X + _temp_vector.X;
				_physicsAtomArea.Y = physics.atoms.atoms[i].position.Y + _temp_vector.Y;
				_physicsAtomArea.Z = physics.atoms.atoms[i].position.Z + _temp_vector.Z;
				if (PhysicsCollide(ref GRID_ERROR, ref _physicsAtomArea, ref physics.atoms.atoms[i].velocity, ref _temp_out_position, ref _temp_out_velocity))
				{
					position += _temp_out_position;
					velocity -= _temp_out_velocity;
					result = true;
				}
			}
		}
		for (int i = 0; i < physics.lengthStack; i++)
		{
			if (physics.stack[i] != this && physics.stack[i].physicsAlive && PhysicsCollide(ref GRID_ERROR, ref physics.stack[i]._position, ref physics.stack[i].velocity, ref _temp_out_position, ref _temp_out_velocity))
			{
				position += _temp_out_position;
				velocity -= _temp_out_velocity;
				result = true;
			}
		}
		return result;
	}

	protected bool PhysicsGroundCheck()
	{
		_temp_gridable = physics.universe.grid.At(X / Grid.SPACING.X, Y / Grid.SPACING.Y - 1f, Z / Grid.SPACING.Z);
		_temp_item = physics.At(X, Y - Grid.SPACING.Y, Z);
		if (_temp_gridable == null)
		{
			return _temp_item != null;
		}
		return true;
	}

	protected bool PhysicsCollide(ref Vector3 vError, ref Vector3 vItemArea, ref Vector3 vItemVelocity, ref Vector3 vOutPosition, ref Vector3 vOutVelocity)
	{
		bool result = false;
		if (Grid.BoxCollide(position, vItemArea, ref vError, ref _temp_collision_delta, ref _temp_collision_box))
		{
			_temp_collision_velocity = vItemVelocity * -1f + velocity;
			Vector3.Normalize(ref _temp_collision_velocity, out _temp_collision_unit);
			_temp_collision_boxCenter.X = (_temp_collision_box.Min.X + _temp_collision_box.Max.X) * 0.5f;
			_temp_collision_boxCenter.Y = (_temp_collision_box.Min.Y + _temp_collision_box.Max.Y) * 0.5f;
			_temp_collision_boxCenter.Z = (_temp_collision_box.Min.Z + _temp_collision_box.Max.Z) * 0.5f;
			_temp_collision_volume.X = _temp_collision_box.Max.X - _temp_collision_box.Min.X;
			_temp_collision_volume.Y = _temp_collision_box.Max.Y - _temp_collision_box.Min.Y;
			_temp_collision_volume.Z = _temp_collision_box.Max.Z - _temp_collision_box.Min.Z;
			_temp_collision_ray.Position = _temp_collision_boxCenter + _temp_collision_volume.Length() * _temp_collision_unit * -1f;
			_temp_collision_ray.Direction = _temp_collision_unit;
			float? num = _temp_collision_ray.Intersects(_temp_collision_box);
			if (num.HasValue && num.Value >= 0f)
			{
				vOutPosition = _temp_collision_unit * -1f * ((_temp_collision_volume.Length() - num.Value) * 2f);
				velocityCollide = velocity;
				vOutVelocity = _temp_collision_velocity;
				result = true;
			}
		}
		return result;
	}

	public virtual object PhysicsRayCast(PhysicsItem oCaster, ref Ray oRay, ref float xDistance)
	{
		float? num = null;
		object result = null;
		for (int i = 0; i < physics.atoms.lengthAtoms; i++)
		{
			if (!physics.atoms.atoms[i].visible || !physics.atoms.atoms[i].definition.playGrid)
			{
				continue;
			}
			for (int j = 0; j < physics.atoms.atoms[i].area.Length; j++)
			{
				physics.atoms.atoms[i].area[j].ToPosition(ref _temp_vector);
				_physicsAtomArea.X = physics.atoms.atoms[i].position.X + _temp_vector.X;
				_physicsAtomArea.Y = physics.atoms.atoms[i].position.Y + _temp_vector.Y;
				_physicsAtomArea.Z = physics.atoms.atoms[i].position.Z + _temp_vector.Z;
				_temp_collision_box.Min.X = _physicsAtomArea.X - Grid.SPACING.X * 0.5f;
				_temp_collision_box.Min.Y = _physicsAtomArea.Y - Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Min.Z = _physicsAtomArea.Z - Grid.SPACING.Z * 0.5f;
				_temp_collision_box.Max.X = _physicsAtomArea.X + Grid.SPACING.X * 0.5f;
				_temp_collision_box.Max.Y = _physicsAtomArea.Y + Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Max.Z = _physicsAtomArea.Z + Grid.SPACING.Z * 0.5f;
				num = oRay.Intersects(_temp_collision_box);
				if (num.HasValue && num.Value < xDistance)
				{
					xDistance = num.Value;
					result = physics.atoms.atoms[i];
				}
			}
		}
		for (int i = 0; i < physics.lengthStack; i++)
		{
			if (physics.stack[i] != this && physics.stack[i].physicsAlive)
			{
				_temp_collision_box.Min.X = physics.stack[i]._position.X - Grid.SPACING.X * 0.5f;
				_temp_collision_box.Min.Y = physics.stack[i]._position.Y - Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Min.Z = physics.stack[i]._position.Z - Grid.SPACING.Z * 0.5f;
				_temp_collision_box.Max.X = physics.stack[i]._position.X + Grid.SPACING.X * 0.5f;
				_temp_collision_box.Max.Y = physics.stack[i]._position.Y + Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Max.Z = physics.stack[i]._position.Z + Grid.SPACING.Z * 0.5f;
				num = oRay.Intersects(_temp_collision_box);
				if (num.HasValue && num.Value < xDistance)
				{
					xDistance = num.Value;
					result = physics.stack[i];
				}
			}
		}
		return result;
	}

	public virtual bool PhysicsRayCheckNoQbits(PhysicsItem oCaster, ref Ray oRay, ref float xDistance)
	{
		float? num = null;
		for (int i = 0; i < physics.atoms.lengthAtoms; i++)
		{
			if (physics.atoms.atoms[i].visible && physics.atoms.atoms[i].definition.playGrid)
			{
				for (int j = 0; j < physics.atoms.atoms[i].area.Length; j++)
				{
					physics.atoms.atoms[i].area[j].ToPosition(ref _temp_vector);
					_physicsAtomArea.X = physics.atoms.atoms[i].position.X + _temp_vector.X;
					_physicsAtomArea.Y = physics.atoms.atoms[i].position.Y + _temp_vector.Y;
					_physicsAtomArea.Z = physics.atoms.atoms[i].position.Z + _temp_vector.Z;
					_temp_collision_box.Min.X = _physicsAtomArea.X - Grid.SPACING.X * 0.5f;
					_temp_collision_box.Min.Y = _physicsAtomArea.Y - Grid.SPACING.Y * 0.5f;
					_temp_collision_box.Min.Z = _physicsAtomArea.Z - Grid.SPACING.Z * 0.5f;
					_temp_collision_box.Max.X = _physicsAtomArea.X + Grid.SPACING.X * 0.5f;
					_temp_collision_box.Max.Y = _physicsAtomArea.Y + Grid.SPACING.Y * 0.5f;
					_temp_collision_box.Max.Z = _physicsAtomArea.Z + Grid.SPACING.Z * 0.5f;
					num = oRay.Intersects(_temp_collision_box);
					if (num.HasValue)
					{
						if (num.Value <= xDistance)
						{
							break;
						}
						num = null;
					}
				}
			}
			if (num.HasValue)
			{
				break;
			}
		}
		if (!num.HasValue)
		{
			for (int i = 0; i < physics.lengthStack; i++)
			{
				if (physics.stack[i] == this || !physics.stack[i].physicsAlive || physics.stack[i] is QBit)
				{
					continue;
				}
				_temp_collision_box.Min.X = physics.stack[i]._position.X - Grid.SPACING.X * 0.5f;
				_temp_collision_box.Min.Y = physics.stack[i]._position.Y - Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Min.Z = physics.stack[i]._position.Z - Grid.SPACING.Z * 0.5f;
				_temp_collision_box.Max.X = physics.stack[i]._position.X + Grid.SPACING.X * 0.5f;
				_temp_collision_box.Max.Y = physics.stack[i]._position.Y + Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Max.Z = physics.stack[i]._position.Z + Grid.SPACING.Z * 0.5f;
				num = oRay.Intersects(_temp_collision_box);
				if (num.HasValue)
				{
					if (num.Value <= xDistance)
					{
						break;
					}
					num = null;
				}
			}
		}
		return num.HasValue;
	}

	public virtual QBit PhysicsRayCheckOnlyQbits(ref Ray oRay, ref float xDistance)
	{
		float? num = null;
		QBit result = null;
		for (int i = 0; i < physics.universe.qbits.qbits.Count; i++)
		{
			if (physics.universe.qbits.qbits[i].physicsAlive)
			{
				_temp_collision_box.Min.X = physics.universe.qbits.qbits[i]._position.X - Grid.SPACING.X * 0.5f;
				_temp_collision_box.Min.Y = physics.universe.qbits.qbits[i]._position.Y - Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Min.Z = physics.universe.qbits.qbits[i]._position.Z - Grid.SPACING.Z * 0.5f;
				_temp_collision_box.Max.X = physics.universe.qbits.qbits[i]._position.X + Grid.SPACING.X * 0.5f;
				_temp_collision_box.Max.Y = physics.universe.qbits.qbits[i]._position.Y + Grid.SPACING.Y * 0.5f;
				_temp_collision_box.Max.Z = physics.universe.qbits.qbits[i]._position.Z + Grid.SPACING.Z * 0.5f;
				num = oRay.Intersects(_temp_collision_box);
				if (num.HasValue && num.Value <= xDistance)
				{
					result = physics.universe.qbits.qbits[i];
					break;
				}
			}
		}
		return result;
	}

	public virtual void Death()
	{
		physics.universe.history.Close(this, HistoryItem.Action.Physics);
		if (moving)
		{
			moving = false;
			physics.universe.history.Close(this, HistoryItem.Action.Move);
		}
		physicsCheckActive = false;
		physicsActive = false;
		physics.universe.history.Open(this, HistoryItem.Action.Death);
		dying = true;
	}

	protected virtual void Dead()
	{
		if (!physics.universe.history.reversing)
		{
			physics.universe.history.Close(this, HistoryItem.Action.Death);
		}
		dying = false;
		dead = true;
		position = FARAWAY;
		visible = false;
		physicsCheckActive = false;
		physicsActive = false;
	}

	public void SnapToGrid()
	{
		_position.X = (float)(Math.Round(Math.Abs(_position.X / Grid.SPACING.X)) * (double)Math.Sign(_position.X) * (double)Grid.SPACING.X);
		_position.Y = (float)(Math.Round(Math.Abs(_position.Y / Grid.SPACING.Y)) * (double)Math.Sign(_position.Y) * (double)Grid.SPACING.Y);
		_position.Z = (float)(Math.Round(Math.Abs(_position.Z / Grid.SPACING.Z)) * (double)Math.Sign(_position.Z) * (double)Grid.SPACING.Z);
		_change_position = true;
	}

	public Atom AtomBelow()
	{
		Atom atom = null;
		_temp_vector_at.X = X;
		_temp_vector_at.Y = Y - Grid.SPACING.Y;
		_temp_vector_at.Z = Z;
		return physics.universe.grid.At(_temp_vector_at) as Atom;
	}

	public void PositionFromPoint(GridPoint oPoint)
	{
		X = (float)oPoint.X * Grid.SPACING.X;
		Y = (float)oPoint.Y * Grid.SPACING.Y;
		Z = (float)oPoint.Z * Grid.SPACING.Z;
	}

	public void PositionFromPoint(Vector3 vPoint)
	{
		X = vPoint.X * Grid.SPACING.X;
		Y = vPoint.Y * Grid.SPACING.Y;
		Z = vPoint.Z * Grid.SPACING.Z;
	}

	protected virtual void Event_Physics_Start()
	{
		physicsPreviousPosition = _position;
		physicsCheckActive = false;
		physics.universe.history.Open(this, HistoryItem.Action.Physics);
	}

	protected virtual void Event_Physics_End()
	{
		physics.universe.history.Close(this, HistoryItem.Action.Physics);
		SnapToGrid();
		if (physicsAlive)
		{
			physicsCheckActive = true;
		}
	}

	public virtual void Event_Physics_Update(float elapsed)
	{
	}

	protected virtual void Event_Move_Start()
	{
		physicsCheckActive = false;
		physicsPreviousPosition = _position;
		physics.universe.history.Open(this, HistoryItem.Action.Move);
	}

	protected virtual void Event_Move_End()
	{
		physics.universe.history.Close(this, HistoryItem.Action.Move);
		if (physicsAlive)
		{
			physicsCheckActive = true;
		}
	}

	public virtual void Event_Flip_Start()
	{
		physicsCheckActive = false;
		PhysicsStop();
		_base.matrix = matrix;
	}

	public virtual void Event_Flip_End()
	{
		if (physicsAlive)
		{
			physicsCheckActive = true;
		}
	}

	public virtual void Event_Flip_Update()
	{
	}

	public virtual void History_Event_Resume(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Physics)
		{
			PhysicsStart();
		}
		if (oItem.action == HistoryItem.Action.Move)
		{
			Console.WriteLine("Move Resume in PhysicsItem, this should never trigger");
		}
		if (oItem.action == HistoryItem.Action.Death)
		{
			Console.WriteLine("Death Resume in PhysicsItem, this should never trigger");
		}
	}

	public virtual void History_Event_Reverse_Start(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Move)
		{
			moving = true;
		}
		if (oItem.action == HistoryItem.Action.Death)
		{
			deathTime = deathTimeTotal - 1f;
			dead = false;
			dying = true;
			visible = true;
		}
	}

	public virtual void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Move)
		{
			moving = false;
		}
		if (oItem.action == HistoryItem.Action.Physics)
		{
			physicsActive = false;
			physicsCheckActive = true;
		}
		if (oItem.action == HistoryItem.Action.Death)
		{
			dying = false;
			dead = false;
			visible = true;
			deathTime = 0f;
			scaleX = 1f;
			scaleY = 1f;
			scaleZ = 1f;
		}
	}

	public virtual void History_Event_Lock()
	{
		historyLocked = true;
	}

	public virtual void History_Event_Unlock()
	{
		historyLocked = false;
	}

	public virtual void History_Event_ForceClose(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.Move && moving)
		{
			oItem.end.time = physics.universe.history.time - moveTime + moveTimeTotal;
			oItem.end.position = moveTo;
			oItem.end.rotation = moveRotationTo;
			moving = false;
		}
	}
}
