using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class for effects with automatic support for, and binding of, FX Standard Annotations and Semantics (SAS).
/// </summary>
public abstract class BaseSasBindEffect : ParameteredEffect
{
	protected new class _0001CB
	{
		private Dictionary<string, List<EffectParameter>> HCB = new Dictionary<string, List<EffectParameter>>(128);

		public void Add(string bindaddress, EffectParameter parameter)
		{
			if (!HCB.ContainsKey(bindaddress))
			{
				List<EffectParameter> list = new List<EffectParameter>(8);
				list.Add(parameter);
				HCB.Add(bindaddress, list);
			}
			else
			{
				HCB[bindaddress].Add(parameter);
			}
		}

		public List<EffectParameter> Find(string bindaddress)
		{
			if (!HCB.ContainsKey(bindaddress))
			{
				return null;
			}
			return HCB[bindaddress];
		}

		public void Clear()
		{
			HCB.Clear();
		}
	}

	/// <summary />
	public const string SASAddress_World_Matrix = "Sas.Camera.World";

	/// <summary />
	public const string SASAddress_WorldInverse_Matrix = "Sas.Camera.WorldInverse";

	/// <summary />
	public const string SASAddress_WorldTranspose_Matrix = "Sas.Camera.WorldTranspose";

	/// <summary />
	public const string SASAddress_WorldInverseTranspose_Matrix = "Sas.Camera.WorldInverseTranspose";

	/// <summary />
	public const string SASAddress_WorldView_Matrix = "Sas.Camera.ObjectToView";

	/// <summary />
	public const string SASAddress_WorldViewInverse_Matrix = "Sas.Camera.ObjectToViewInverse";

	/// <summary />
	public const string SASAddress_WorldViewTranspose_Matrix = "Sas.Camera.ObjectToViewTranspose";

	/// <summary />
	public const string SASAddress_WorldViewInverseTranspose_Matrix = "Sas.Camera.ObjectToViewInverseTranspose";

	/// <summary />
	public const string SASAddress_WorldViewProjection_Matrix = "Sas.Camera.ObjectToProjection";

	/// <summary />
	public const string SASAddress_WorldViewProjectionInverse_Matrix = "Sas.Camera.ObjectToProjectionInverse";

	/// <summary />
	public const string SASAddress_WorldViewProjectionTranspose_Matrix = "Sas.Camera.ObjectToProjectionTranspose";

	/// <summary />
	public const string SASAddress_WorldViewProjectionInverseTranspose_Matrix = "Sas.Camera.ObjectToProjectionInverseTranspose";

	/// <summary />
	public const string SASAddress_View_Matrix = "Sas.Camera.WorldToView";

	/// <summary />
	public const string SASAddress_ViewInverse_Matrix = "Sas.Camera.WorldToViewInverse";

	/// <summary />
	public const string SASAddress_ViewTranspose_Matrix = "Sas.Camera.WorldToViewTranspose";

	/// <summary />
	public const string SASAddress_ViewInverseTranspose_Matrix = "Sas.Camera.WorldToViewInverseTranspose";

	/// <summary />
	public const string SASAddress_Projection_Matrix = "Sas.Camera.Projection";

	/// <summary />
	public const string SASAddress_ProjectionInverse_Matrix = "Sas.Camera.ProjectionInverse";

	/// <summary />
	public const string SASAddress_ProjectionTranspose_Matrix = "Sas.Camera.ProjectionTranspose";

	/// <summary />
	public const string SASAddress_ProjectionInverseTranspose_Matrix = "Sas.Camera.ProjectionInverseTranspose";

	/// <summary />
	public const string SASAddress_NumAmbientLights = "Sas.NumAmbientLights";

	/// <summary />
	public const string SASAddress_NumDirectionalLights = "Sas.NumDirectionalLights";

	/// <summary />
	public const string SASAddress_NumPointLights = "Sas.NumPointLights";

	/// <summary />
	public const string SASAddress_Camera_Position = "Sas.Camera.Position";

	/// <summary />
	public const string SASAddress_SkeletonBones_Matrix = "Sas.Skeleton.MeshToJointToWorld[*]";

	/// <summary />
	public const string SASAddress_Time_Now = "Sas.Time.Now";

	/// <summary />
	public const string SASAddress_Time_Last = "Sas.Time.Last";

	/// <summary />
	public const string SASAddress_Time_FrameNumber = "Sas.Time.FrameNumber";

	/// <summary />
	public static readonly string[] SASAddress_AmbientLight_Color = new string[4] { "Sas.AmbientLight[0].Color", "Sas.AmbientLight[1].Color", "Sas.AmbientLight[2].Color", "Sas.AmbientLight[3].Color" };

	/// <summary />
	public static readonly string[] SASAddress_DirectionalLight_Color = new string[4] { "Sas.DirectionalLight[0].Color", "Sas.DirectionalLight[1].Color", "Sas.DirectionalLight[2].Color", "Sas.DirectionalLight[3].Color" };

	/// <summary />
	public static readonly string[] SASAddress_DirectionalLight_Direction = new string[4] { "Sas.DirectionalLight[0].Direction", "Sas.DirectionalLight[1].Direction", "Sas.DirectionalLight[2].Direction", "Sas.DirectionalLight[3].Direction" };

	/// <summary />
	public static readonly string[] SASAddress_PointLight_Color = new string[4] { "Sas.PointLight[0].Color", "Sas.PointLight[1].Color", "Sas.PointLight[2].Color", "Sas.PointLight[3].Color" };

	/// <summary />
	public static readonly string[] SASAddress_PointLight_Position = new string[4] { "Sas.PointLight[0].Position", "Sas.PointLight[1].Position", "Sas.PointLight[2].Position", "Sas.PointLight[3].Position" };

	/// <summary />
	public static readonly string[] SASAddress_PointLight_Range = new string[4] { "Sas.PointLight[0].Range", "Sas.PointLight[1].Range", "Sas.PointLight[2].Range", "Sas.PointLight[3].Range" };

	private int HCB;

	private GameTime HC_0002 = new GameTime();

	private GameTime HC_0012 = new GameTime();

	private _0001CB HCH = new _0001CB();

	/// <summary>
	/// The current game time used by animated materials.
	/// </summary>
	public GameTime GameTime
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0012 = HC_0002;
			HC_0002 = value;
			HCB++;
			SyncTimeEffectData();
		}
	}

	/// <summary>
	/// Maintains a table of string addresses and their bound effect parameter lists.
	/// Used to tie any number of similar parameters using different names, semantics,
	/// and bind addresses to the same single address.
	/// </summary>
	protected _0001CB SasAutoBindTable => HCH;

	/// <summary>
	/// Finds parameter by shader variable name.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	protected EffectParameter FindByName(string name)
	{
		return base.Parameters[name];
	}

	/// <summary>
	/// Finds parameter by shader variable semantic.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	protected EffectParameter FindBySemantic(string name)
	{
		for (int i = 0; i < base.Parameters.Count; i++)
		{
			EffectParameter effectParameter = base.Parameters[i];
			string semantic = effectParameter.Semantic;
			if (!string.IsNullOrEmpty(semantic) && string.Compare(semantic, name, StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return effectParameter;
			}
		}
		return null;
	}

	/// <summary>
	/// Finds parameter by shader variable bind address.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	protected EffectParameter FindBySasAddress(string name)
	{
		for (int i = 0; i < base.Parameters.Count; i++)
		{
			EffectParameter effectParameter = base.Parameters[i];
			EffectAnnotation effectAnnotation = effectParameter.Annotations["SasBindAddress"];
			if (effectAnnotation != null)
			{
				string valueString = effectAnnotation.GetValueString();
				if (!string.IsNullOrEmpty(valueString) && valueString == name)
				{
					return effectParameter;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Binds parameter to a specific string address. Generally used to remap
	/// non standard semantics to standard bind addresses.
	/// </summary>
	/// <param name="parameter"></param>
	/// <param name="address"></param>
	protected void BindBySasAddress(EffectParameter parameter, string address)
	{
		if (parameter != null)
		{
			SasAutoBindTable.Add(address, parameter);
		}
	}

	/// <summary>
	/// Binds all parameters containing a bind address that starts
	/// with the partial address, to the partial address.
	/// </summary>
	/// <param name="partialaddress"></param>
	protected void BindAllByPartialSasAddress(string partialaddress)
	{
		for (int i = 0; i < base.Parameters.Count; i++)
		{
			EffectParameter effectParameter = base.Parameters[i];
			EffectAnnotation effectAnnotation = effectParameter.Annotations["SasBindAddress"];
			if (effectAnnotation == null)
			{
				continue;
			}
			string valueString = effectAnnotation.GetValueString();
			if (!string.IsNullOrEmpty(valueString) && valueString.Length >= partialaddress.Length && !(partialaddress != valueString.Substring(0, partialaddress.Length)))
			{
				if (effectParameter.Elements.Count > 0)
				{
					_0012_0006(effectParameter, valueString);
				}
				else if (effectParameter.StructureMembers.Count > 0)
				{
					_0012o(effectParameter, valueString);
				}
				else
				{
					BindBySasAddress(effectParameter, valueString);
				}
			}
		}
	}

	private void _0012_0006(EffectParameter P_0, string P_1)
	{
		for (int i = 0; i < P_0.Elements.Count; i++)
		{
			EffectParameter effectParameter = P_0.Elements[i];
			string text = P_1.Replace("*", i.ToString());
			if (effectParameter.StructureMembers.Count > 0)
			{
				_0012o(effectParameter, text);
			}
			else
			{
				BindBySasAddress(effectParameter, text);
			}
		}
	}

	private void _0012o(EffectParameter P_0, string P_1)
	{
		for (int i = 0; i < P_0.StructureMembers.Count; i++)
		{
			BindBySasAddress(P_0.StructureMembers[i], P_1 + "." + P_0.StructureMembers[i].Name);
		}
	}

	/// <summary>
	/// Applies the current game time information to the bound effect time parameters.
	/// </summary>
	protected void SyncTimeEffectData()
	{
		EffectHelper._0012v(SasAutoBindTable.Find("Sas.Time.Now"), new Vector4((float)HC_0002.TotalGameTime.TotalMilliseconds));
		EffectHelper._0012v(SasAutoBindTable.Find("Sas.Time.Last"), new Vector4((float)HC_0012.TotalGameTime.TotalMilliseconds));
		EffectHelper._0012v(SasAutoBindTable.Find("Sas.Time.FrameNumber"), new Vector4(HCB));
	}

	internal BaseSasBindEffect(GraphicsDevice P_0, byte[] P_1)
		: base(P_0, P_1)
	{
		BindAllByPartialSasAddress("Sas.");
		BindBySasAddress(FindBySemantic("MODEL"), "Sas.Camera.World");
		BindBySasAddress(FindBySemantic("MODELI"), "Sas.Camera.WorldInverse");
		BindBySasAddress(FindBySemantic("MODELINVERSE"), "Sas.Camera.WorldInverse");
		BindBySasAddress(FindBySemantic("MODELT"), "Sas.Camera.WorldTranspose");
		BindBySasAddress(FindBySemantic("MODELTRANSPOSE"), "Sas.Camera.WorldTranspose");
		BindBySasAddress(FindBySemantic("MODELIT"), "Sas.Camera.WorldInverseTranspose");
		BindBySasAddress(FindBySemantic("MODELINVERSETRANSPOSE"), "Sas.Camera.WorldInverseTranspose");
		BindBySasAddress(FindBySemantic("WORLD"), "Sas.Camera.World");
		BindBySasAddress(FindBySemantic("WORLDI"), "Sas.Camera.WorldInverse");
		BindBySasAddress(FindBySemantic("WORLDINVERSE"), "Sas.Camera.WorldInverse");
		BindBySasAddress(FindBySemantic("WORLDT"), "Sas.Camera.WorldTranspose");
		BindBySasAddress(FindBySemantic("WORLDTRANSPOSE"), "Sas.Camera.WorldTranspose");
		BindBySasAddress(FindBySemantic("WORLDIT"), "Sas.Camera.WorldInverseTranspose");
		BindBySasAddress(FindBySemantic("WORLDINVERSETRANSPOSE"), "Sas.Camera.WorldInverseTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEW"), "Sas.Camera.ObjectToView");
		BindBySasAddress(FindBySemantic("MODELVIEWI"), "Sas.Camera.ObjectToViewInverse");
		BindBySasAddress(FindBySemantic("MODELVIEWINVERSE"), "Sas.Camera.ObjectToViewInverse");
		BindBySasAddress(FindBySemantic("MODELVIEWT"), "Sas.Camera.ObjectToViewTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWTRANSPOSE"), "Sas.Camera.ObjectToViewTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWIT"), "Sas.Camera.ObjectToViewInverseTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWINVERSETRANSPOSE"), "Sas.Camera.ObjectToViewInverseTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEW"), "Sas.Camera.ObjectToView");
		BindBySasAddress(FindBySemantic("WORLDVIEWI"), "Sas.Camera.ObjectToViewInverse");
		BindBySasAddress(FindBySemantic("WORLDVIEWINVERSE"), "Sas.Camera.ObjectToViewInverse");
		BindBySasAddress(FindBySemantic("WORLDVIEWT"), "Sas.Camera.ObjectToViewTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWTRANSPOSE"), "Sas.Camera.ObjectToViewTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWIT"), "Sas.Camera.ObjectToViewInverseTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWINVERSETRANSPOSE"), "Sas.Camera.ObjectToViewInverseTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTION"), "Sas.Camera.ObjectToProjection");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTIONI"), "Sas.Camera.ObjectToProjectionInverse");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTIONINVERSE"), "Sas.Camera.ObjectToProjectionInverse");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTIONT"), "Sas.Camera.ObjectToProjectionTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTIONTRANSPOSE"), "Sas.Camera.ObjectToProjectionTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTIONIT"), "Sas.Camera.ObjectToProjectionInverseTranspose");
		BindBySasAddress(FindBySemantic("MODELVIEWPROJECTIONINVERSETRANSPOSE"), "Sas.Camera.ObjectToProjectionInverseTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTION"), "Sas.Camera.ObjectToProjection");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTIONI"), "Sas.Camera.ObjectToProjectionInverse");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTIONINVERSE"), "Sas.Camera.ObjectToProjectionInverse");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTIONT"), "Sas.Camera.ObjectToProjectionTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTIONTRANSPOSE"), "Sas.Camera.ObjectToProjectionTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTIONIT"), "Sas.Camera.ObjectToProjectionInverseTranspose");
		BindBySasAddress(FindBySemantic("WORLDVIEWPROJECTIONINVERSETRANSPOSE"), "Sas.Camera.ObjectToProjectionInverseTranspose");
		BindBySasAddress(FindBySemantic("VIEW"), "Sas.Camera.WorldToView");
		BindBySasAddress(FindBySemantic("VIEWI"), "Sas.Camera.WorldToViewInverse");
		BindBySasAddress(FindBySemantic("VIEWINVERSE"), "Sas.Camera.WorldToViewInverse");
		BindBySasAddress(FindBySemantic("VIEWT"), "Sas.Camera.WorldToViewTranspose");
		BindBySasAddress(FindBySemantic("VIEWTRANSPOSE"), "Sas.Camera.WorldToViewTranspose");
		BindBySasAddress(FindBySemantic("VIEWIT"), "Sas.Camera.WorldToViewInverseTranspose");
		BindBySasAddress(FindBySemantic("VIEWINVERSETRANSPOSE"), "Sas.Camera.WorldToViewInverseTranspose");
		BindBySasAddress(FindBySemantic("PROJECTION"), "Sas.Camera.Projection");
		BindBySasAddress(FindBySemantic("PROJECTIONI"), "Sas.Camera.ProjectionInverse");
		BindBySasAddress(FindBySemantic("PROJECTIONINVERSE"), "Sas.Camera.ProjectionInverse");
		BindBySasAddress(FindBySemantic("PROJECTIONT"), "Sas.Camera.ProjectionTranspose");
		BindBySasAddress(FindBySemantic("PROJECTIONTRANSPOSE"), "Sas.Camera.ProjectionTranspose");
		BindBySasAddress(FindBySemantic("PROJECTIONIT"), "Sas.Camera.ProjectionInverseTranspose");
		BindBySasAddress(FindBySemantic("PROJECTIONINVERSETRANSPOSE"), "Sas.Camera.ProjectionInverseTranspose");
	}
}
