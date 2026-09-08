namespace SgMotion.Controllers;

public interface IBlendable
{
	Pose[] LocalBonePoses { get; }

	float BlendWeight { get; set; }
}
