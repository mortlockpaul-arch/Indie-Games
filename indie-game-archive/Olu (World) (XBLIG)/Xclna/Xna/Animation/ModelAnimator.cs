using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public class ModelAnimator : DrawableGameComponent
{
	private Matrix world;

	private readonly Model model;

	private readonly EffectParameter[] worldParams;

	private readonly EffectParameter[] matrixPaletteParams;

	private Effect[] modelEffects;

	private ReadOnlyCollection<Effect> effectCollection;

	private BonePoseCollection bonePoses;

	private AnimationInfoCollection animations;

	private IList<IAttachable> attachedObjects;

	private readonly int numMeshes;

	private readonly int numEffects;

	private static Matrix skinTransform;

	private Matrix[] pose;

	private Matrix[][] palette;

	private SkinInfoCollection[] skinInfo;

	public Matrix World
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return world;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			world = value;
		}
	}

	protected int EffectCount => numEffects;

	public Model Model => model;

	public AnimationInfoCollection Animations => animations;

	public ReadOnlyCollection<Effect> Effects => effectCollection;

	public IList<IAttachable> AttachedObjects => attachedObjects;

	public BonePoseCollection BonePoses => bonePoses;

	public ModelAnimator(Game game)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		world = Matrix.Identity;
		attachedObjects = new List<IAttachable>();
		((DrawableGameComponent)this)._002Ector(game);
	}

	public unsafe ModelAnimator(Game game, Model model)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		world = Matrix.Identity;
		attachedObjects = new List<IAttachable>();
		((DrawableGameComponent)this)._002Ector(game);
		this.model = model;
		animations = AnimationInfoCollection.FromModel(model);
		bonePoses = BonePoseCollection.FromModelBoneCollection(model.Bones);
		numMeshes = ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count;
		numEffects = 0;
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						Effect current2 = ((Enumerator)(ref enumerator2)).Current;
						numEffects++;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		modelEffects = (Effect[])(object)new Effect[numEffects];
		worldParams = (EffectParameter[])(object)new EffectParameter[numEffects];
		matrixPaletteParams = (EffectParameter[])(object)new EffectParameter[numEffects];
		InitializeEffectParams();
		pose = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count];
		model.CopyAbsoluteBoneTransformsTo(pose);
		Dictionary<string, object> dictionary = (Dictionary<string, object>)model.Tag;
		if (dictionary == null)
		{
			throw new Exception("Model Processor must subclass AnimatedModelProcessor.");
		}
		skinInfo = (SkinInfoCollection[])dictionary["SkinInfo"];
		if (skinInfo == null)
		{
			throw new Exception("Model processor must pass skinning info through the tag.");
		}
		palette = new Matrix[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count][];
		for (int i = 0; i < skinInfo.Length; i++)
		{
			if (Util.IsSkinned(((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i]))
			{
				palette[i] = (Matrix[])(object)new Matrix[skinInfo[i].Count];
			}
			else
			{
				palette[i] = null;
			}
		}
		((GameComponent)this).UpdateOrder = 1;
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)this);
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; i++)
		{
			if (palette[i] != null && matrixPaletteParams[i] != null)
			{
				Matrix[] value = palette[i];
				try
				{
					matrixPaletteParams[i].SetValue(value);
				}
				catch
				{
					throw new Exception("Model has too many skinned bones for the matrix palette.");
				}
			}
		}
	}

	public SkinInfoCollection GetMeshSkinInfo(int index)
	{
		return skinInfo[index];
	}

	protected unsafe virtual IList<Effect> CreateEffectList()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		List<Effect> list = new List<Effect>();
		Enumerator enumerator = model.Meshes.GetEnumerator();
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
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return list;
	}

	public void InitializeEffectParams()
	{
		IList<Effect> list = CreateEffectList();
		if (list.Count != numEffects)
		{
			throw new Exception("The number of effects in the list returned by CreateEffectList must be equal to the number of ModelMeshParts.");
		}
		list.CopyTo(modelEffects, 0);
		effectCollection = new ReadOnlyCollection<Effect>(modelEffects);
		for (int i = 0; i < numEffects; i++)
		{
			worldParams[i] = modelEffects[i].Parameters["World"];
			matrixPaletteParams[i] = modelEffects[i].Parameters["MatrixPalette"];
		}
	}

	public override void Update(GameTime gameTime)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		bonePoses.CopyAbsoluteTransformsTo(pose);
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
		foreach (IAttachable attachedObject in attachedObjects)
		{
			attachedObject.CombinedTransform = attachedObject.LocalTransform * Matrix.Invert(pose[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[0].ParentBone.Index]) * pose[attachedObject.AttachedBone.Index] * world;
		}
	}

	public void CopyAbsoluteTransformsTo(Matrix[] transforms)
	{
		pose.CopyTo(transforms, 0);
	}

	public Matrix GetAbsoluteTransform(int boneIndex)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return pose[boneIndex];
	}

	public unsafe override void Draw(GameTime gameTime)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			int num = 0;
			for (int i = 0; i < numMeshes; i++)
			{
				ModelMesh val = ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i];
				int num2 = num;
				Enumerator enumerator;
				if (matrixPaletteParams[num] != null)
				{
					enumerator = val.Effects.GetEnumerator();
					try
					{
						while (((Enumerator)(ref enumerator)).MoveNext())
						{
							Effect current = ((Enumerator)(ref enumerator)).Current;
							worldParams[num].SetValue(world);
							matrixPaletteParams[num].SetValue(palette[i]);
							num++;
						}
					}
					finally
					{
						((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
					}
				}
				else
				{
					enumerator = val.Effects.GetEnumerator();
					try
					{
						while (((Enumerator)(ref enumerator)).MoveNext())
						{
							Effect current = ((Enumerator)(ref enumerator)).Current;
							worldParams[num].SetValue(pose[val.ParentBone.Index] * world);
							num++;
						}
					}
					finally
					{
						((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
					}
				}
				int count = ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts).Count;
				GraphicsDevice graphicsDevice = ((GraphicsResource)val.VertexBuffer).GraphicsDevice;
				graphicsDevice.Indices = val.IndexBuffer;
				for (int j = 0; j < count; j++)
				{
					ModelMeshPart val2 = ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts)[j];
					if (val2.NumVertices != 0 && val2.PrimitiveCount != 0)
					{
						Effect val3 = modelEffects[num2 + j];
						graphicsDevice.VertexDeclaration = val2.VertexDeclaration;
						graphicsDevice.Vertices[0].SetSource(val.VertexBuffer, val2.StreamOffset, val2.VertexStride);
						val3.Begin();
						EffectPassCollection passes = val3.CurrentTechnique.Passes;
						int count2 = passes.Count;
						for (int k = 0; k < count2; k++)
						{
							EffectPass val4 = passes[k];
							val4.Begin();
							graphicsDevice.DrawIndexedPrimitives((PrimitiveType)4, val2.BaseVertex, 0, val2.NumVertices, val2.StartIndex, val2.PrimitiveCount);
							val4.End();
						}
						val3.End();
					}
				}
			}
		}
		catch (NullReferenceException)
		{
			throw new InvalidOperationException("The effects on the model for a ModelAnimator were changed without calling ModelAnimator.InitializeEffectParams().");
		}
		catch (InvalidCastException)
		{
			throw new InvalidCastException("ModelAnimator has thrown an InvalidCastException.  This is likely because the model uses too many bones for the matrix palette.  The default palette size is 56 for windows and 40 for Xbox.");
		}
	}
}
