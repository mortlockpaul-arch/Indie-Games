namespace SgMotion.Controllers;

public class SkeletonController : ISkeletonController, IBlendable
{
	private SkinnedModelBoneDictionary skeletonDictionary;

	private Pose[] localBonePoses;

	private float blendWeight;

	public Pose[] LocalBonePoses => localBonePoses;

	public float BlendWeight
	{
		get
		{
			return blendWeight;
		}
		set
		{
			blendWeight = value;
		}
	}

	public SkeletonController(SkinnedModelBoneDictionary skeletonDictionary)
	{
		this.skeletonDictionary = skeletonDictionary;
		localBonePoses = new Pose[skeletonDictionary.Count];
		blendWeight = 1f;
	}

	public void SetBonePose(string channelName, ref Pose pose)
	{
		ref Pose reference = ref localBonePoses[skeletonDictionary[channelName].Index];
		reference = pose;
	}

	public void SetBonePose(string channelName, Pose pose)
	{
		localBonePoses[skeletonDictionary[channelName].Index] = pose;
	}
}
