using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.CharacterData, DataContent")]
public class CharacterData
{
	public string Name;

	public Vector3 Scale;

	public string CoOpIdle;

	public string CoOpCrouch;

	public string CoOpCrouchWalk;

	public string CoOpCrouchWalkBack;

	public string CoOpWalk;

	public string CoOpWalkBack;

	public string CoOpSideStepLeft;

	public string CoOpSideStepRight;

	public string CoOpRun;

	public string CoOpReload;

	public string CoOpKnife;

	public string CoOpSwap;

	public string CoOpJump;

	public string CoOpClimb;

	public string CoOpClimbUp;

	public string CoOpDeath00;

	public string CoOpDeath01;

	public string CoOpDeath02;

	public string CoOpDeath03;
}
