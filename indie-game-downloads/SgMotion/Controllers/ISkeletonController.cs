namespace SgMotion.Controllers;

public interface ISkeletonController
{
	Pose[] LocalBonePoses { get; }

	void SetBonePose(string channelName, ref Pose pose);

	void SetBonePose(string channelName, Pose pose);
}
