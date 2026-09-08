using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SgMotion.Controllers;

public class AnimationController : IAnimationController, IBlendable
{
	private SkinnedModelBoneCollection skeleton;

	private Pose[] localBonePoses;

	private Matrix[] skinnedBoneTransforms;

	private Quaternion[] boneControllers;

	private bool[] boneControllersUsed;

	private AnimationClip animationClip;

	private TimeSpan time;

	private float speed;

	private bool loopEnabled;

	private PlaybackMode playbackMode;

	private float blendWeight;

	private InterpolationMode translationInterpolation;

	private InterpolationMode orientationInterpolation;

	private InterpolationMode scaleInterpolation;

	private bool crossFadeEnabled;

	private AnimationClip crossFadeAnimationClip;

	private float crossFadeInterpolationAmount;

	private TimeSpan crossFadeTime;

	private TimeSpan crossFadeElapsedTime;

	private InterpolationMode crossFadeTranslationInterpolation;

	private InterpolationMode crossFadeOrientationInterpolation;

	private InterpolationMode crossFadeScaleInterpolation;

	private bool hasFinished;

	private bool isPlaying;

	private int keyframeIndex;

	private List<VertexPositionColor> boundingBoxLines = new List<VertexPositionColor>();

	private List<VertexPositionColor> skeletonLines = new List<VertexPositionColor>();

	private Dictionary<string, Vector3> boneNamePosition = new Dictionary<string, Vector3>();

	private SkinnedModelBone selectedBone;

	private BasicEffect effect;

	private SpriteBatch spriteBatch;

	public int KeyframeIndex => keyframeIndex;

	public AnimationClip AnimationClip => animationClip;

	public TimeSpan Time
	{
		get
		{
			return time;
		}
		set
		{
			time = value;
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			if (speed < 0f)
			{
				throw new ArgumentException("Speed must be a positive value");
			}
			speed = value;
		}
	}

	public bool LoopEnabled
	{
		get
		{
			return loopEnabled;
		}
		set
		{
			loopEnabled = value;
			if (hasFinished && loopEnabled)
			{
				hasFinished = false;
			}
		}
	}

	public PlaybackMode PlaybackMode
	{
		get
		{
			return playbackMode;
		}
		set
		{
			playbackMode = value;
		}
	}

	public InterpolationMode TranslationInterpolation
	{
		get
		{
			return translationInterpolation;
		}
		set
		{
			translationInterpolation = value;
		}
	}

	public InterpolationMode OrientationInterpolation
	{
		get
		{
			return orientationInterpolation;
		}
		set
		{
			orientationInterpolation = value;
		}
	}

	public InterpolationMode ScaleInterpolation
	{
		get
		{
			return scaleInterpolation;
		}
		set
		{
			scaleInterpolation = value;
		}
	}

	public bool HasFinished => hasFinished;

	public bool IsPlaying => isPlaying;

	public Pose[] LocalBonePoses => localBonePoses;

	public Matrix[] SkinnedBoneTransforms => skinnedBoneTransforms;

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

	public AnimationController(SkinnedModelBoneCollection skeleton)
	{
		this.skeleton = skeleton;
		localBonePoses = new Pose[skeleton.Count];
		skinnedBoneTransforms = new Matrix[skeleton.Count];
		skeleton[0].CopyBindPoseTo(localBonePoses);
		boneControllers = new Quaternion[skeleton.Count];
		boneControllersUsed = new bool[skeleton.Count];
		time = TimeSpan.Zero;
		speed = 1f;
		loopEnabled = true;
		playbackMode = PlaybackMode.Forward;
		blendWeight = 1f;
		translationInterpolation = InterpolationMode.None;
		orientationInterpolation = InterpolationMode.None;
		scaleInterpolation = InterpolationMode.None;
		crossFadeEnabled = false;
		crossFadeInterpolationAmount = 0f;
		crossFadeTime = TimeSpan.Zero;
		crossFadeElapsedTime = TimeSpan.Zero;
		hasFinished = false;
		isPlaying = false;
	}

	public void StartClip(AnimationClip animationClip)
	{
		this.animationClip = animationClip;
		hasFinished = false;
		isPlaying = true;
		time = ((playbackMode == PlaybackMode.Forward) ? TimeSpan.Zero : animationClip.Duration);
		skeleton[0].CopyBindPoseTo(localBonePoses);
	}

	[Obsolete("PlayClip has been replaced with SwitchToClip - please update your code")]
	public void PlayClip(AnimationClip animationClip)
	{
		SwitchToClip(animationClip);
	}

	public void SwitchToClip(AnimationClip animationClip)
	{
		this.animationClip = animationClip;
		if (time < animationClip.Duration)
		{
			hasFinished = false;
			isPlaying = true;
		}
	}

	public void CrossFade(AnimationClip animationClip, TimeSpan fadeTime)
	{
		CrossFade(animationClip, fadeTime, InterpolationMode.Linear, InterpolationMode.Linear, InterpolationMode.Linear);
	}

	public void CrossFade(AnimationClip animationClip, TimeSpan fadeTime, InterpolationMode translationInterpolation, InterpolationMode orientationInterpolation, InterpolationMode scaleInterpolation)
	{
		if (!isPlaying)
		{
			StartClip(animationClip);
			return;
		}
		if (crossFadeEnabled)
		{
			StartClip(crossFadeAnimationClip);
		}
		crossFadeAnimationClip = animationClip;
		crossFadeTime = fadeTime;
		crossFadeElapsedTime = TimeSpan.Zero;
		crossFadeTranslationInterpolation = translationInterpolation;
		crossFadeOrientationInterpolation = orientationInterpolation;
		crossFadeScaleInterpolation = scaleInterpolation;
		crossFadeEnabled = true;
	}

	public Matrix GetBoneAbsoluteTransform(int boneId)
	{
		return Matrix.Invert(skeleton[boneId].InverseBindPoseTransform) * SkinnedBoneTransforms[boneId];
	}

	public Matrix GetBoneAbsoluteTransform(string boneName)
	{
		int num = 0;
		for (int i = 0; i < skeleton.Count; i++)
		{
			if (skeleton[i].Name == boneName)
			{
				num = i;
				break;
			}
		}
		return Matrix.Invert(skeleton[num].InverseBindPoseTransform) * SkinnedBoneTransforms[num];
	}

	public void SetBoneController(int bone, Quaternion angles)
	{
		boneControllers[bone] = angles;
		boneControllersUsed[bone] = true;
	}

	public void SetBoneController(string boneName, Quaternion angles)
	{
		SetBoneController(skeleton.GetBoneId(boneName), angles);
	}

	public void Update(TimeSpan elapsedTime, Matrix parent)
	{
		if (hasFinished && !crossFadeEnabled)
		{
			return;
		}
		TimeSpan elapsedTime2 = TimeSpan.FromTicks((long)((float)elapsedTime.Ticks * speed));
		if (animationClip != null)
		{
			UpdateAnimationTime(elapsedTime2);
			if (crossFadeEnabled)
			{
				UpdateCrossFadeTime(elapsedTime2);
			}
			UpdateChannelPoses();
		}
		UpdateAbsoluteBoneTransforms(ref parent);
	}

	private void UpdateCrossFadeTime(TimeSpan elapsedTime)
	{
		crossFadeElapsedTime += elapsedTime;
		if (crossFadeElapsedTime > crossFadeTime)
		{
			crossFadeEnabled = false;
			crossFadeInterpolationAmount = 0f;
			crossFadeTime = TimeSpan.Zero;
			crossFadeElapsedTime = TimeSpan.Zero;
			StartClip(crossFadeAnimationClip);
		}
		else
		{
			crossFadeInterpolationAmount = (float)crossFadeElapsedTime.Ticks / (float)crossFadeTime.Ticks;
		}
	}

	private void UpdateAnimationTime(TimeSpan elapsedTime)
	{
		if (playbackMode == PlaybackMode.Forward)
		{
			time += elapsedTime;
		}
		else
		{
			time -= elapsedTime;
		}
		if (!(time < TimeSpan.Zero) && !(time > animationClip.Duration))
		{
			return;
		}
		if (loopEnabled)
		{
			if (time > animationClip.Duration)
			{
				while (time > animationClip.Duration)
				{
					time -= animationClip.Duration;
				}
			}
			else
			{
				while (time < TimeSpan.Zero)
				{
					time += animationClip.Duration;
				}
			}
			skeleton[0].CopyBindPoseTo(localBonePoses);
		}
		else
		{
			time = ((time > animationClip.Duration) ? animationClip.Duration : TimeSpan.Zero);
			isPlaying = false;
			hasFinished = true;
		}
	}

	private void UpdateChannelPoses()
	{
		for (int i = 0; i < localBonePoses.Length; i++)
		{
			string name = skeleton[i].Name;
			if (animationClip.Channels.TryGetValue(name, out var value))
			{
				InterpolateChannelPose(value, time, out localBonePoses[i]);
			}
			if (crossFadeEnabled)
			{
				Pose outPose;
				if (crossFadeAnimationClip.Channels.TryGetValue(name, out value))
				{
					InterpolateChannelPose(value, TimeSpan.Zero, out outPose);
				}
				else
				{
					outPose = skeleton[i].BindPose;
				}
				ref Pose reference = ref localBonePoses[i];
				reference = Pose.Interpolate(localBonePoses[i], outPose, crossFadeInterpolationAmount, crossFadeTranslationInterpolation, crossFadeOrientationInterpolation, crossFadeScaleInterpolation);
			}
		}
	}

	private void InterpolateChannelPose(AnimationChannel animationChannel, TimeSpan animationTime, out Pose outPose)
	{
		if (translationInterpolation == InterpolationMode.None && orientationInterpolation == InterpolationMode.None && scaleInterpolation == InterpolationMode.None)
		{
			keyframeIndex = animationChannel.GetKeyframeIndexByTime(animationTime);
			outPose = animationChannel[keyframeIndex].Pose;
			return;
		}
		keyframeIndex = animationChannel.GetKeyframeIndexByTime(animationTime);
		int index = ((!loopEnabled) ? Math.Min(keyframeIndex + 1, animationChannel.Count - 1) : ((keyframeIndex + 1) % animationChannel.Count));
		AnimationChannelKeyframe animationChannelKeyframe = animationChannel[keyframeIndex];
		AnimationChannelKeyframe animationChannelKeyframe2 = animationChannel[index];
		long num = ((keyframeIndex != animationChannel.Count - 1) ? (animationChannelKeyframe2.Time.Ticks - animationChannelKeyframe.Time.Ticks) : (animationClip.Duration.Ticks - animationChannelKeyframe.Time.Ticks));
		if (num > 0)
		{
			long num2 = animationTime.Ticks - animationChannelKeyframe.Time.Ticks;
			float amount = (float)num2 / (float)num;
			outPose = Pose.Interpolate(animationChannelKeyframe.Pose, animationChannelKeyframe2.Pose, amount, translationInterpolation, orientationInterpolation, scaleInterpolation);
		}
		else
		{
			outPose = animationChannelKeyframe.Pose;
		}
	}

	private void UpdateAbsoluteBoneTransforms(ref Matrix parent)
	{
		Matrix matrix = Matrix.CreateFromQuaternion(localBonePoses[0].Orientation);
		if (boneControllersUsed[0])
		{
			matrix *= Matrix.CreateFromQuaternion(boneControllers[0]);
		}
		matrix.Translation = localBonePoses[0].Translation;
		matrix.M11 *= localBonePoses[0].Scale.X;
		matrix.M21 *= localBonePoses[0].Scale.X;
		matrix.M31 *= localBonePoses[0].Scale.X;
		matrix.M12 *= localBonePoses[0].Scale.Y;
		matrix.M22 *= localBonePoses[0].Scale.Y;
		matrix.M32 *= localBonePoses[0].Scale.Y;
		matrix.M13 *= localBonePoses[0].Scale.Z;
		matrix.M23 *= localBonePoses[0].Scale.Z;
		matrix.M33 *= localBonePoses[0].Scale.Z;
		ref Matrix reference = ref skinnedBoneTransforms[0];
		reference = matrix * parent;
		for (int i = 1; i < skinnedBoneTransforms.Length; i++)
		{
			matrix = Matrix.CreateFromQuaternion(localBonePoses[i].Orientation);
			if (boneControllersUsed[i])
			{
				matrix *= Matrix.CreateFromQuaternion(boneControllers[i]);
			}
			matrix.Translation = localBonePoses[i].Translation;
			matrix.M11 *= localBonePoses[i].Scale.X;
			matrix.M21 *= localBonePoses[i].Scale.X;
			matrix.M31 *= localBonePoses[i].Scale.X;
			matrix.M12 *= localBonePoses[i].Scale.Y;
			matrix.M22 *= localBonePoses[i].Scale.Y;
			matrix.M32 *= localBonePoses[i].Scale.Y;
			matrix.M13 *= localBonePoses[i].Scale.Z;
			matrix.M23 *= localBonePoses[i].Scale.Z;
			matrix.M33 *= localBonePoses[i].Scale.Z;
			int index = skeleton[i].Parent.Index;
			ref Matrix reference2 = ref skinnedBoneTransforms[i];
			reference2 = matrix * skinnedBoneTransforms[index];
		}
		for (int j = 0; j < skinnedBoneTransforms.Length; j++)
		{
			ref Matrix reference3 = ref skinnedBoneTransforms[j];
			reference3 = skeleton[j].InverseBindPoseTransform * skinnedBoneTransforms[j];
		}
	}

	public void SetSelectedBone(int index)
	{
		try
		{
			selectedBone = skeleton[index];
		}
		catch (Exception)
		{
			selectedBone = null;
		}
	}

	public void DrawDebugSkeleton(Matrix world, Matrix view, Matrix projection, GraphicsDevice graphics, SpriteFont font)
	{
		DrawDebugSkeleton(world, view, projection, graphics, font, DebugDisplay.Always, DebugDisplay.Never, Color.SkyBlue);
	}

	public void DrawDebugSkeleton(Matrix world, Matrix view, Matrix projection, GraphicsDevice graphics, SpriteFont font, DebugDisplay displaynames, DebugDisplay displayindices)
	{
		DrawDebugSkeleton(world, view, projection, graphics, font, displaynames, displayindices, Color.SkyBlue);
	}

	public void DrawDebugSkeleton(Matrix world, Matrix view, Matrix projection, GraphicsDevice graphics, SpriteFont font, DebugDisplay displaynames, DebugDisplay displayindices, Color debugcolor, float jointScale = 1f)
	{
		float value = 0.2f * jointScale;
		if (effect == null || effect.IsDisposed)
		{
			effect = new BasicEffect(graphics)
			{
				VertexColorEnabled = true
			};
		}
		if (spriteBatch == null || spriteBatch.IsDisposed)
		{
			spriteBatch = new SpriteBatch(graphics);
		}
		effect.World = world;
		effect.View = view;
		effect.Projection = projection;
		boundingBoxLines.Clear();
		skeletonLines.Clear();
		boneNamePosition.Clear();
		foreach (SkinnedModelBone item in skeleton)
		{
			Vector3 translation = GetBoneAbsoluteTransform(item.Index).Translation;
			boneNamePosition.Add(item.Name, translation);
		}
		foreach (SkinnedModelBone item2 in skeleton)
		{
			Vector3 vector = boneNamePosition[item2.Name];
			BoundingBox boundingBox = new BoundingBox(vector - new Vector3(value), vector + new Vector3(value));
			Color color = debugcolor;
			if (selectedBone != null)
			{
				if (item2.Index == selectedBone.Index)
				{
					color = Color.Red;
				}
				else if (item2.Parent == selectedBone)
				{
					color = Color.Yellow;
				}
			}
			Vector3[] corners = boundingBox.GetCorners();
			boundingBoxLines.Add(new VertexPositionColor(corners[0], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[1], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[0], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[3], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[0], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[4], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[1], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[2], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[1], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[5], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[2], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[3], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[2], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[6], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[3], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[7], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[4], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[5], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[4], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[7], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[5], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[6], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[6], color));
			boundingBoxLines.Add(new VertexPositionColor(corners[7], color));
			if (item2.Index == 0)
			{
				AddBoneChildren(item2, vector, debugcolor);
			}
		}
		effect.CurrentTechnique.Passes[0].Apply();
		graphics.DrawUserPrimitives(PrimitiveType.LineList, boundingBoxLines.ToArray(), 0, boundingBoxLines.Count / 2);
		graphics.DrawUserPrimitives(PrimitiveType.LineList, skeletonLines.ToArray(), 0, skeletonLines.Count / 2);
		if (displaynames == DebugDisplay.Never && displayindices == DebugDisplay.Never)
		{
			return;
		}
		spriteBatch.Begin();
		foreach (string key in boneNamePosition.Keys)
		{
			Vector3 vector2 = graphics.Viewport.Project(boneNamePosition[key], projection, view, world);
			string text = "";
			if (displaynames == DebugDisplay.Always || (selectedBone != null && displaynames == DebugDisplay.Selected && skeleton[key].Index == selectedBone.Index))
			{
				text = key;
			}
			if (displayindices == DebugDisplay.Always || (selectedBone != null && displayindices == DebugDisplay.Selected && skeleton[key].Index == selectedBone.Index))
			{
				object obj = text;
				text = string.Concat(obj, "[", skeleton.GetBoneId(key), "]");
			}
			Color color2 = debugcolor;
			if (selectedBone != null)
			{
				if (key == selectedBone.Name)
				{
					color2 = Color.Red;
				}
				else
				{
					foreach (SkinnedModelBone child in selectedBone.Children)
					{
						if (key == child.Name)
						{
							color2 = Color.Yellow;
							break;
						}
					}
				}
			}
			spriteBatch.DrawString(font, text, new Vector2(vector2.X + 10f, vector2.Y), color2);
		}
		spriteBatch.End();
	}

	private void AddBoneChildren(SkinnedModelBone parent, Vector3 parentTransform, Color color)
	{
		Color color2 = color;
		Color color3 = color;
		if (parent == selectedBone)
		{
			color2 = Color.Red;
			color3 = Color.Yellow;
		}
		foreach (SkinnedModelBone child in parent.Children)
		{
			Vector3 vector = boneNamePosition[child.Name];
			skeletonLines.Add(new VertexPositionColor(parentTransform, color2));
			skeletonLines.Add(new VertexPositionColor(vector, color3));
			AddBoneChildren(child, vector, color);
		}
	}
}
