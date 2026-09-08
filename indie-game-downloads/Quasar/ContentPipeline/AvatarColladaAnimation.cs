using System;
using Microsoft.Xna.Framework;
using XnaToFna.StubXDK.GamerServices;

namespace Quasar.ContentPipeline;

public class AvatarColladaAnimation
{
	public class SFrame
	{
		public Quaternion[] rotation;

		public Vector3[] translation;
	}

	public const int BoneCount = 71;

	private SFrame[] bones;

	private int nframes;

	private double current_frame;

	private Matrix[] bone_transforms = new Matrix[71];

	public Matrix[] BoneTransforms => bone_transforms;

	public AvatarExpression Expression { get; set; }

	public TimeSpan CurrentPosition
	{
		get
		{
			int milliseconds = (int)(current_frame / (double)nframes * (double)FramesPerSecond);
			return new TimeSpan(0, 0, 0, 0, milliseconds);
		}
		set
		{
			current_frame = (float)(FramesPerSecond * value.Milliseconds) * 0.001f;
		}
	}

	public TimeSpan Length
	{
		get
		{
			int milliseconds = (int)((float)nframes / (float)FramesPerSecond) * 1000;
			return new TimeSpan(0, 0, 0, 0, milliseconds);
		}
	}

	public int LengthMilliseconds => (int)((float)nframes / (float)FramesPerSecond) * 1000;

	public int TotalFrames => nframes;

	public int FramesPerSecond { get; set; }

	public AvatarColladaAnimation(SFrame[] abones, int count_frames)
	{
		bones = abones;
		nframes = count_frames;
		FramesPerSecond = 30;
		for (int i = 0; i < 71; i++)
		{
			ref Matrix reference = ref bone_transforms[i];
			reference = Matrix.Identity;
		}
	}

	public AvatarColladaAnimation(AvatarColladaAnimation source)
		: this(source.bones, source.nframes)
	{
	}

	public void Update(TimeSpan elapsedAnimationTime, bool loop)
	{
		double num = (float)elapsedAnimationTime.Milliseconds * 0.001f;
		if (num > 0.0)
		{
			current_frame += (double)FramesPerSecond * num;
		}
		if (nframes <= 0)
		{
			return;
		}
		int num2 = (int)current_frame;
		int next_frame = num2 + 1;
		if (num2 + 1 >= nframes)
		{
			if (loop)
			{
				current_frame -= num2;
				num2 = 0;
				next_frame = num2 + 1;
			}
			else
			{
				current_frame = nframes - 1;
				next_frame = (num2 = nframes - 1);
			}
		}
		double num3 = current_frame - (double)num2;
		updateBoneTransforms(num2, next_frame, (float)num3);
	}

	private void updateBoneTransforms(int begin_frame, int next_frame, float delta)
	{
		for (int i = 0; i < 71; i++)
		{
			Quaternion.Slerp(ref bones[i].rotation[begin_frame], ref bones[i].rotation[next_frame], delta, out var result);
			Vector3.Lerp(ref bones[i].translation[begin_frame], ref bones[i].translation[next_frame], delta, out var result2);
			Matrix.CreateFromQuaternion(ref result, out var result3);
			result3.Translation = result2;
			bone_transforms[i] = result3;
		}
	}
}
