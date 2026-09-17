using GKEngine.Cameras;
using GKEngine.Edit.Gysmos;
using GKEngine.Entities;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace GKEngine.Edit;

public class Editable
{
	public delegate GUID GuidGet();

	public delegate void GuidSet(GUID oGUID);

	public delegate float? Pick(Vector2 vMouse, Camera oCamera);

	public static string SELECT_PATH_DELIMITER = ":";

	public string path = "";

	public string name = "";

	public bool selected;

	public object parent;

	public Gysmo.Translation translation;

	public Base3D translationOrbitParent;

	public GuidGet __guidGet = () => new GUID();

	public GuidSet __guidSet = delegate
	{
	};

	public bool pickable;

	public Pick __pick = (Vector2 vMouse, Camera oCamera) => (float?)null;

	public GUID guid
	{
		get
		{
			return __guidGet();
		}
		set
		{
			__guidSet(value);
		}
	}

	public Editable(object oParent)
	{
		parent = oParent;
	}

	public void Focus(Camera oCamera)
	{
		if (parent is Base3D base3D)
		{
			Vector3 vector = Vector3.Normalize(base3D.position - oCamera.position);
			if (vector == Vector3.Up || vector == Vector3.Down)
			{
				oCamera.X += 0.0001f;
			}
			Quaternion rotation = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(oCamera.position, base3D.position, Vector3.Up, null)));
			oCamera.rotation = rotation;
		}
	}
}
