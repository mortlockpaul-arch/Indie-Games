using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Utils;
using Game.Data;
using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class Atom : Base3D, IGridable
{
	public enum DataBit
	{
		Normal,
		Over,
		Selected,
		OverSelected
	}

	public Base3D _base = new Base3D();

	public AtomManager manager;

	public AtomDefinition definition;

	public Vector4 data;

	public GridPoint point;

	public GUID guid;

	public bool play;

	public DataAtom dataRef;

	protected bool _visible = true;

	public Vector3 velocity = default(Vector3);

	public bool selected;

	public GridPoint[] area;

	protected int _state = -1;

	private int[] _properties = new int[0];

	public AtomTrigger trigger;

	public virtual bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			_visible = value;
		}
	}

	public virtual int state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
		}
	}

	public virtual int[] properties
	{
		get
		{
			return _properties;
		}
		set
		{
			_properties = new int[definition.propertiesDefault.Length];
			definition.propertiesDefault.CopyTo(_properties, 0);
			if (_properties.Length >= value.Length)
			{
				value.CopyTo(_properties, 0);
			}
		}
	}

	public GridPoint gridPoint => point;

	public GridPoint[] gridArea => area;

	public GUID gridGUID => guid;

	public Type gridType => typeof(Atom);

	public Base3D gridBase3D => this;

	public Atom(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
	{
		manager = oManager;
		definition = oDefinition;
		if (xGUID == null)
		{
			guid = new GUID();
		}
		else
		{
			guid = GUID.FromString(xGUID);
		}
		data = new Vector4(0f, 0f, 0f, 0f);
		properties = definition.propertiesDefault;
		point = new GridPoint(this);
		AreaMake();
	}

	public virtual void InitPlay()
	{
		play = true;
	}

	public virtual void InitBuild()
	{
	}

	public virtual void Unover()
	{
		data.W = (selected ? 2 : 0);
	}

	public virtual void Over()
	{
		data.W = ((!selected) ? 1 : 3);
	}

	public virtual void Place(int xX, int xY, int xZ)
	{
		point.X = xX;
		point.Y = xY;
		point.Z = xZ;
		AreaRefresh();
	}

	public virtual void RotateAndUpdate(Quaternion xRotation)
	{
		rotation = xRotation;
		RoundMatrix();
		matrix = _matrix;
		AreaRefresh();
	}

	public virtual void Update(GameTime oGameTime)
	{
	}

	public virtual void StateSet(int xState)
	{
		state = xState;
	}

	public virtual void StateLerp(int xStateFrom, int xStateTo, float xRatio)
	{
	}

	public virtual void AreaMake()
	{
		if (manager.mode == AtomManager.Mode.Build || (manager.mode == AtomManager.Mode.Play && definition.playGrid))
		{
			area = new GridPoint[AtomCatalog.shapes[definition.shape].area.Length];
			for (int i = 0; i < area.Length; i++)
			{
				if (area[i] == null)
				{
					area[i] = new GridPoint();
				}
				Vector3 value = AtomCatalog.shapes[definition.shape].area[i].ToVector3();
				area[i].FromVector3(Vector3.Transform(value, rotation));
			}
		}
		else
		{
			area = new GridPoint[0];
		}
	}

	public virtual void AreaRefresh()
	{
		manager.grid.Remove(this);
		AreaRotate();
		manager.grid.Add(this);
	}

	public virtual void AreaRefreshSoft()
	{
		AreaRotate();
		manager.grid.Add(this);
	}

	public virtual void AreaRotate()
	{
		for (int i = 0; i < area.Length; i++)
		{
			Vector3 value = AtomCatalog.shapes[definition.shape].area[i].ToVector3();
			area[i].FromVector3(Vector3.Transform(value, rotation));
		}
	}

	public virtual void Event_Triggered_Start(object oTriggerer)
	{
	}

	public virtual void Event_Triggered_End()
	{
	}

	public virtual void Event_Flip_Start()
	{
		_base.matrix = Matrix.Multiply(matrix, manager.inverse);
	}

	public virtual void Event_Flip_End()
	{
		point.FromPosition(position);
		AreaRefreshSoft();
	}

	public virtual void Event_Flip_Update()
	{
	}

	public virtual void Event_Painted(AtomPainter oPainter)
	{
		if (definition.autoRotate)
		{
			SetRotation(Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI / 2f * (float)GameEngine.random.Next(3)));
		}
		else if (oPainter.selected.rotatable)
		{
			SetRotation(oPainter.rotation);
		}
		if (definition.hueable)
		{
			data.Z = (float)(GameEngine.random.NextDouble() * 1.0 - 0.5);
			if (definition.instanced)
			{
				(this as AtomInstanced).PopulateInstancer();
			}
		}
	}
}
