using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class ModelOluAnimator : ModelAnimator
{
	private Matrix world;

	private readonly ModelWrapper model;

	private Effect modelEffect;

	public BonePoseCollection bonePoses;

	private AnimationInfoCollection animations;

	private IList<IAttachable> attachedObjects;

	private readonly int numMeshes;

	private static Matrix skinTransform;

	private Matrix[] pose;

	private Matrix[][] palette;

	private SkinInfoCollection[] skinInfo;

	public new AnimationInfoCollection Animations => animations;

	public new ModelWrapper Model => model;

	public new IList<IAttachable> AttachedObjects => attachedObjects;

	public new BonePoseCollection BonePoses => bonePoses;

	public ModelOluAnimator(Game game, ModelWrapper model, Effect _modelEffect)
		: this(game, model, _modelEffect, ref model.transforms, ref model.palette, ref model.boneNames)
	{
	}

	public unsafe ModelOluAnimator(Game game, ModelWrapper model, Effect _modelEffect, ref Matrix[] _aniMatrix, ref Matrix[][] _paletteTown, ref Dictionary<string, int[]> bNames)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		world = Matrix.Identity;
		attachedObjects = new List<IAttachable>();
		base._002Ector(game);
		this.model = model;
		bNames = new Dictionary<string, int[]>();
		animations = AnimationInfoCollection.FromModel(model.model);
		bonePoses = BonePoseCollection.FromModelBoneCollection(model.model.Bones);
		numMeshes = ((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes).Count;
		modelEffect = _modelEffect;
		pose = _aniMatrix;
		model.model.CopyAbsoluteBoneTransformsTo(pose);
		Dictionary<string, object> dictionary = (Dictionary<string, object>)model.model.Tag;
		if (dictionary == null)
		{
			throw new Exception("Model Processor must subclass AnimatedModelProcessor.");
		}
		skinInfo = (SkinInfoCollection[])dictionary["SkinInfo"];
		if (skinInfo == null)
		{
			throw new Exception("Model processor must pass skinning info through the tag.");
		}
		bool flag = false;
		Enumerator enumerator = model.model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				if (Util.IsSkinned(current))
				{
					flag = true;
					break;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		if (!flag)
		{
			return;
		}
		_paletteTown = new Matrix[((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes).Count][];
		palette = _paletteTown;
		for (int i = 0; i < skinInfo.Length; i++)
		{
			if (Util.IsSkinned(((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes)[i]))
			{
				palette[i] = (Matrix[])(object)new Matrix[skinInfo[i].Count];
			}
			else
			{
				palette[i] = null;
			}
			for (int j = 0; j < skinInfo[i].Count; j++)
			{
				bNames.Add(skinInfo[i][j].BoneName, new int[2] { i, j });
			}
		}
	}

	protected unsafe override IList<Effect> CreateEffectList()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<Effect> list = new List<Effect>();
		Enumerator enumerator = model.model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						list.Add(current2.Effect);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public override void Update(GameTime gameTime)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.Get().paused)
		{
			return;
		}
		bonePoses.CopyAbsoluteTransformsTo(pose);
		if (palette != null)
		{
			for (int i = 0; i < skinInfo.Length; i++)
			{
				if (palette[i] == null)
				{
					continue;
				}
				SkinInfoCollection skinInfoCollection = skinInfo[i];
				foreach (SkinInfo item in skinInfoCollection)
				{
					skinTransform = item.InverseBindPoseTransform;
					Matrix.Multiply(ref skinTransform, ref pose[item.BoneIndex], ref palette[i][item.PaletteIndex]);
				}
			}
		}
		foreach (IAttachable attachedObject in attachedObjects)
		{
			attachedObject.CombinedTransform = attachedObject.LocalTransform * Matrix.Invert(pose[((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes)[0].ParentBone.Index]) * pose[attachedObject.AttachedBone.Index] * world;
		}
	}

	public new void CopyAbsoluteTransformsTo(Matrix[] transforms)
	{
		pose.CopyTo(transforms, 0);
	}

	public new Matrix GetAbsoluteTransform(int boneIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return pose[boneIndex];
	}
}
