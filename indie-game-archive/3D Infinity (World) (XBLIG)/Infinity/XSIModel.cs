using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XSIXNARuntime;

namespace Infinity;

public class XSIModel
{
	public Vector3 Position;

	public Vector3? AmbientLightColor;

	public float? Alpha;

	private Matrix[] transforms;

	private Vector3[] dirlight;

	private Vector3[] dircolor;

	public Model CrosswalkModel { get; private set; }

	public BoundingSphere[] Spheres { get; private set; }

	public List<XSIAnimationContent> Animations { get; private set; }

	public int AnimationIndex { get; set; }

	public string FilePath { get; private set; }

	public bool PlaybackStatus { get; private set; }

	public XSIAnimationContent Animation => Animations[AnimationIndex];

	public bool HasAnimation
	{
		get
		{
			if (Animations != null)
			{
				return Animations.Count > 0;
			}
			return false;
		}
	}

	public event EventHandler Finished;

	public XSIModel(string AssetPath, ContentManager content)
	{
		LoadContent(AssetPath, content);
		FilePath = AssetPath;
	}

	public unsafe void LoadContent(string AssetPath, ContentManager content)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		CrosswalkModel = content.Load<Model>(AssetPath);
		Spheres = GetBoundingSpheres(CrosswalkModel.Meshes);
		Animations = new List<XSIAnimationContent>();
		XSIAnimationData xSIAnimationData = CrosswalkModel.Tag as XSIAnimationData;
		transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)CrosswalkModel.Bones).Count];
		if (xSIAnimationData != null)
		{
			foreach (KeyValuePair<string, XSIAnimationContent> item in xSIAnimationData.RuntimeAnimationContentDictionary)
			{
				item.Value.BindModelBones(CrosswalkModel);
				item.Value.Finished += AnimationFinished;
				Animations.Add(item.Value);
			}
			xSIAnimationData.ResolveBones(CrosswalkModel);
		}
		dirlight = (Vector3[])(object)new Vector3[3]
		{
			Vector3.Zero,
			Vector3.Zero,
			Vector3.Zero
		};
		dircolor = (Vector3[])(object)new Vector3[3]
		{
			Vector3.Zero,
			Vector3.Zero,
			Vector3.Zero
		};
		Enumerator enumerator2 = CrosswalkModel.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator2)).MoveNext())
			{
				ModelMesh current2 = ((Enumerator)(ref enumerator2)).Current;
				Enumerator enumerator3 = current2.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator3)).MoveNext())
					{
						Effect current3 = ((Enumerator)(ref enumerator3)).Current;
						BasicEffect val = (BasicEffect)(object)((current3 is BasicEffect) ? current3 : null);
						if (val != null)
						{
							val.EnableDefaultLighting();
							val.PreferPerPixelLighting = true;
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator3))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void Update(GameTime gameTime)
	{
		Update(gameTime.ElapsedGameTime);
	}

	public void Update(TimeSpan elapsedTime)
	{
		float blend = GetBlend(Animation.CurrentTime);
		Update(elapsedTime, blend);
	}

	public void Update(TimeSpan elapsedTime, float blend)
	{
		if (Animations != null && Animations.Count != 0)
		{
			if (PlaybackStatus)
			{
				Animations[AnimationIndex].PlayBack(elapsedTime, blend);
			}
			else
			{
				Animations[AnimationIndex].PlayBack(TimeSpan.Zero, blend);
			}
		}
	}

	public void FixedUpdate(TimeSpan time)
	{
		if (Animations == null || Animations.Count == 0)
		{
			return;
		}
		if (time < Animation.Duration)
		{
			Animation.CurrentTime = time;
		}
		else if (Animation.Duration.TotalSeconds > 0.0)
		{
			if (Animation.Loop)
			{
				while (time >= Animation.Duration)
				{
					time -= Animation.Duration;
				}
				Animation.CurrentTime = time;
			}
			else
			{
				Animation.CurrentTime = Animation.Duration;
			}
		}
		Update(TimeSpan.Zero, GetBlend(Animation.CurrentTime));
	}

	private void AnimationFinished(object sender, EventArgs e)
	{
		if (Finished != null)
		{
			Finished(this, EventArgs.Empty);
		}
		if (HasAnimation && !Animation.Loop)
		{
			Stop();
		}
	}

	public void UpdateBoundingSphere()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateBoundingSphere(Matrix.Identity);
	}

	public void UpdateBoundingSphere(Matrix world)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateBoundingSphere(Spheres, world);
	}

	public void UpdateBoundingSphere(BoundingSphere[] spheres, Matrix world)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Model crosswalkModel = CrosswalkModel;
		_ = crosswalkModel.Tag;
		crosswalkModel.CopyAbsoluteBoneTransformsTo(transforms);
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)crosswalkModel.Meshes).Count; i++)
		{
			Matrix val = transforms[((ReadOnlyCollection<ModelMesh>)(object)crosswalkModel.Meshes)[i].ParentBone.Index] * world;
			spheres[i].Center = ((Matrix)(ref val)).Translation;
		}
	}

	public bool Play()
	{
		return Play(isLoop: false);
	}

	public bool Play(bool isLoop)
	{
		if (HasAnimation)
		{
			Animation.isFinished = false;
			Animation.Loop = isLoop;
			PlaybackStatus = true;
			FixedUpdate(TimeSpan.Zero);
			return true;
		}
		return false;
	}

	public void Stop()
	{
		PlaybackStatus = false;
	}

	public void Pause()
	{
		PlaybackStatus = !PlaybackStatus;
	}

	public void Draw(XSISASContainer SASData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Draw(SASData, Matrix.Identity);
	}

	public unsafe void Draw(XSISASContainer SASData, Matrix world)
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		Model crosswalkModel = CrosswalkModel;
		XSIAnimationData xSIAnimationData = crosswalkModel.Tag as XSIAnimationData;
		crosswalkModel.CopyAbsoluteBoneTransformsTo(transforms);
		bool flag = false;
		Matrix[] array = null;
		if (xSIAnimationData != null)
		{
			xSIAnimationData.ComputeBoneTransforms(transforms);
			array = xSIAnimationData.BoneTransforms;
			if (array.Length > 0)
			{
				flag = true;
			}
		}
		for (int i = 0; i < SASData.PointLights.Count; i++)
		{
			XSISASPointLight xSISASPointLight = SASData.PointLights[i];
			dirlight[i].X = 0f - xSISASPointLight.Position.X;
			dirlight[i].Y = 0f - xSISASPointLight.Position.Y;
			dirlight[i].Z = 0f - xSISASPointLight.Position.Z;
			((Vector3)(ref dirlight[i])).Normalize();
			dircolor[i].X = xSISASPointLight.Color.X;
			dircolor[i].Y = xSISASPointLight.Color.Y;
			dircolor[i].Z = xSISASPointLight.Color.Z;
		}
		Enumerator enumerator = crosswalkModel.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				SASData.Model = transforms[current.ParentBone.Index] * world;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						Effect current2 = ((Enumerator)(ref enumerator2)).Current;
						if ((object)((object)current2).GetType() == typeof(BasicEffect))
						{
							BasicEffect val = (BasicEffect)current2;
							val.View = SASData.View;
							val.Projection = SASData.Projection;
							val.World = SASData.Model;
							val.DirectionalLight0.Direction = dirlight[0];
							val.DirectionalLight1.Direction = dirlight[1];
							val.DirectionalLight2.Direction = dirlight[2];
							val.DirectionalLight0.DiffuseColor = dircolor[0];
							val.DirectionalLight1.DiffuseColor = dircolor[1];
							val.DirectionalLight2.DiffuseColor = dircolor[2];
							if (AmbientLightColor.HasValue)
							{
								val.AmbientLightColor = AmbientLightColor.Value;
							}
							if (Alpha.HasValue)
							{
								val.Alpha = Alpha.Value;
							}
							continue;
						}
						if (flag && current2.Techniques["Skinned"] != null)
						{
							current2.CurrentTechnique = current2.Techniques["Skinned"];
						}
						else if (current2.Techniques["Static"] != null)
						{
							current2.CurrentTechnique = current2.Techniques["Static"];
						}
						else
						{
							current2.CurrentTechnique = current2.Techniques[0];
						}
						if (flag && current2.Parameters["Bones"] != null && flag)
						{
							current2.Parameters["Bones"].SetValue(array);
						}
						foreach (EffectParameter parameter in current2.Parameters)
						{
							SASData.SetEffectParameterValue(parameter);
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private BoundingSphere[] GetBoundingSpheres(ModelMeshCollection meshes)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		BoundingSphere[] array = (BoundingSphere[])(object)new BoundingSphere[((ReadOnlyCollection<ModelMesh>)(object)meshes).Count];
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)meshes).Count; i++)
		{
			ref BoundingSphere reference = ref array[i];
			reference = ((ReadOnlyCollection<ModelMesh>)(object)meshes)[i].BoundingSphere;
		}
		return array;
	}

	public void SetAnimationTime(TimeSpan time)
	{
	}

	public float GetBlend(TimeSpan time)
	{
		return 1f;
	}
}
