using GKEngine.Edit;
using GKEngine.Scenes;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Entity3D : Base3D, IRenderable, IEditable
{
	public bool visible;

	public Scene scene;

	public float sort;

	public GUID guid;

	public Editable _editable;

	public Editable editable
	{
		get
		{
			return _editable;
		}
		set
		{
			_editable = value;
		}
	}

	public Entity3D()
		: base(Matrix.Identity)
	{
		visible = true;
		guid = new GUID();
	}

	public virtual void Load()
	{
	}

	public virtual void Dispose()
	{
	}

	public virtual void Render(GameTime oGameTime)
	{
	}

	public virtual void Edit_Init()
	{
		editable.__guidGet = () => guid;
		editable.__guidSet = delegate(GUID oGUID)
		{
			guid = oGUID;
		};
	}

	public virtual void Edit_Event_Activate()
	{
	}

	public virtual void Edit_Event_Deactivate()
	{
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
