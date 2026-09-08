using System;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.XboxLive.Avatars.Internal.Animations;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.Xna.Framework.GamerServices;

public class AvatarPropAnimation : AvatarAnimation
{
	protected Pose[] carryableJointPoses;

	protected Matrix[] carryableBoneTransforms;

	protected ReadOnlyCollection<Matrix> carryableBoneTransformCollection;

	internal ReadOnlyCollection<Matrix> CarryableBoneTransforms => carryableBoneTransformCollection;

	public static IAsyncResult BeginGetAvatarPropAnimation(AvatarDescription avatarDescription, AsyncCallback callback, object asyncState)
	{
		if (avatarDescription == null)
		{
			throw new ArgumentNullException("avatarDescription");
		}
		AvatarPropLoadContext avatarPropLoadContext = new AvatarPropLoadContext(avatarDescription, callback, asyncState);
		avatarPropLoadContext.Load();
		return avatarPropLoadContext;
	}

	public static AvatarPropAnimation EndGetAvatarPropAnimation(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new ArgumentNullException("asyncResult");
		}
		if (!(asyncResult is AvatarPropLoadContext avatarPropLoadContext))
		{
			throw new ArgumentException("Invalid parameter", "asyncResult");
		}
		if (AvatarResources.UIThreadId == Thread.CurrentThread.ManagedThreadId)
		{
			if (!asyncResult.IsCompleted)
			{
				throw new NotSupportedException("Blocking load issued from UI thread not supported.");
			}
		}
		else
		{
			asyncResult.AsyncWaitHandle.WaitOne();
		}
		if (avatarPropLoadContext.ResultUsed)
		{
			throw new InvalidOperationException("EndGetAvatarPropAnimation already called");
		}
		avatarPropLoadContext.ResultUsed = true;
		if (avatarPropLoadContext.Animation == null)
		{
			return null;
		}
		return new AvatarPropAnimation(avatarPropLoadContext);
	}

	internal AvatarPropAnimation(AvatarPropLoadContext loadContext)
	{
		Initialize(loadContext.Animation);
	}

	private new void Initialize(Microsoft.XboxLive.Avatars.Internal.Animations.AvatarAnimation animation)
	{
		base.Initialize(animation);
		if (animation != null && animation.HasCarryableKeyframes)
		{
			int carryableJointsCount = animation.CarryableJointsCount;
			carryableJointPoses = new Pose[carryableJointsCount];
			carryableBoneTransforms = new Matrix[carryableJointsCount];
			carryableBoneTransformCollection = new ReadOnlyCollection<Matrix>(carryableBoneTransforms);
		}
		else
		{
			carryableBoneTransformCollection = null;
			carryableJointPoses = null;
			carryableBoneTransforms = null;
		}
	}

	internal override void LoadPoseMatrices()
	{
		base.LoadPoseMatrices();
		if (animation != null && animation.HasCarryableKeyframes)
		{
			animation.GetCarryablePose(cursor, 1f, carryableJointPoses);
			for (int i = 0; i < carryableBoneTransforms.Length; i++)
			{
				Matrix matrix = Matrix.CreateFromQuaternion(new Quaternion(carryableJointPoses[i].rotation.X, carryableJointPoses[i].rotation.Y, carryableJointPoses[i].rotation.Z, carryableJointPoses[i].rotation.W));
				matrix = Matrix.Multiply(matrix, Matrix.CreateScale(carryableJointPoses[i].scale.X, carryableJointPoses[i].scale.Y, carryableJointPoses[i].scale.Z));
				matrix.M41 = carryableJointPoses[i].position.X;
				matrix.M42 = carryableJointPoses[i].position.Y;
				matrix.M43 = carryableJointPoses[i].position.Z;
				carryableBoneTransforms[i] = matrix;
			}
		}
	}
}
