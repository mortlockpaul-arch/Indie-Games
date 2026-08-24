using GKEngine.Input;
using Microsoft.Xna.Framework;

namespace Game.Dialogs;

public class Dialog
{
	public delegate void DialogDelegate(Dialog dialog);

	public float timeIn = 200f;

	public float timeOut = 200f;

	public DialogManager manager;

	public bool active;

	public bool usePostShader = true;

	public bool paused;

	public object data;

	public int postIndex;

	public DialogDelegate eventA;

	public DialogDelegate eventB;

	public DialogDelegate eventX;

	public DialogDelegate eventY;

	public Dialog(DialogManager oManager, DialogDelegate xEventA, DialogDelegate xEventB, DialogDelegate xEventX, DialogDelegate xEventY)
	{
		manager = oManager;
		eventA = xEventA;
		eventB = xEventB;
		eventX = xEventX;
		eventY = xEventY;
	}

	public virtual void Load()
	{
	}

	public virtual void Init()
	{
		Hide();
	}

	public virtual void Dispose()
	{
		manager = null;
		eventA = null;
		eventB = null;
		eventX = null;
		eventY = null;
	}

	public virtual void Show()
	{
	}

	public virtual void Hide()
	{
	}

	public virtual void Update(GameTime oGameTime)
	{
	}

	public virtual void Input_Update(GameTime oGameTime)
	{
		if (!paused)
		{
			if (eventA != null && UniversalInput.inputEntities["DialogA"].active && UniversalInput.inputEntities["DialogA"].pressed)
			{
				manager.Dialog_Out(DialogManager.ExitEvent.A, eventA);
			}
			if (eventB != null && UniversalInput.inputEntities["DialogB"].active && UniversalInput.inputEntities["DialogB"].pressed)
			{
				manager.Dialog_Out(DialogManager.ExitEvent.B, eventB);
			}
			if (eventX != null && UniversalInput.inputEntities["DialogX"].active && UniversalInput.inputEntities["DialogX"].pressed)
			{
				manager.Dialog_Out(DialogManager.ExitEvent.X, eventX);
			}
			if (eventY != null && UniversalInput.inputEntities["DialogY"].active && UniversalInput.inputEntities["DialogY"].pressed)
			{
				manager.Dialog_Out(DialogManager.ExitEvent.Y, eventY);
			}
		}
	}

	public virtual void Event_In_Start()
	{
	}

	public virtual void Event_In_Lerp(float xRatio)
	{
	}

	public virtual void Event_In_Done()
	{
	}

	public virtual void Event_Out_Start()
	{
	}

	public virtual void Event_Out_Lerp(float xRatio)
	{
	}

	public virtual void Event_Out_Done()
	{
	}
}
