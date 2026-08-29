namespace GKEngine.Edit;

public interface IEditable
{
	Editable editable { get; set; }

	void Edit_Event_Activate();

	void Edit_Event_Deactivate();
}
