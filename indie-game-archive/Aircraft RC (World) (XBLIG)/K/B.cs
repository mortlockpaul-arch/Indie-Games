using System;
using System.Runtime.CompilerServices;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;

namespace K
{
	internal static class B
	{
		public const string Diffuse1 = "";

		public const string Diffuse2 = "";

		public const string Diffuse3 = "";

		public const string Diffuse4 = "";

		public const string Normal1 = "";

		public const string Normal2 = "";

		public const string Normal3 = "";

		public const string Normal4 = "";

		public const string HeightMap = "";

		public const string BlendMap = "";

		public const string NormalStrength = "";

		public const string DiffuseScale = "";

		public const string HeightScale = "";

		public const string TileAmount = "";

		public const string MeshRepeatCount = "";

		public const string SpecularAmount = "";

		public const string SpecularPower = "";

		public const string SpecularColor = "";

		public const string MeshSegments = "";
	}
}
namespace k
{
	internal class B : BaseSkinnedEffect, ITransparentEffect, IAddressableEffect, _0003._0012, IShadowGenerateEffect, ITerrainEffect
	{
		private _0003.H HCB;

		private TransparencyMode HC_0002;

		private float HC_0012 = -1f;

		private Texture2D HCH;

		private TextureCube HC7;

		private TextureCube HC_0001;

		private Texture2D HCw;

		private float HCZ;

		private float HC_000F;

		private float HCy;

		private Vector4 HC6;

		private Vector4 HCD;

		private TextureAddressMode HC_0011;

		private TextureAddressMode HCK;

		private TextureAddressMode HC_0003;

		private Matrix HCk;

		private int HCs;

		private int HC_0013;

		private float HCX;

		private float HCz;

		private Texture2D HCA;

		private EffectParameter HCc;

		private EffectParameter HCY;

		private new EffectParameter HCV;

		private EffectParameter HCu;

		private EffectParameter HCq;

		private EffectParameter HCR;

		private EffectParameter HCN;

		private EffectParameter HCF;

		private EffectParameter HCf;

		private EffectParameter HCG;

		private EffectParameter HC_0010;

		private EffectParameter HC_0014;

		private EffectParameter HCL;

		private EffectParameter HCh;

		private EffectParameter HCT;

		private EffectParameter HCa;

		private EffectParameter HCS;

		private EffectParameter HCr;

		private EffectParameter HCJ;

		private EffectParameter HC_0006;

		private static Matrix[] HCo = new Matrix[3];

		private static Vector4[] HCe = new Vector4[3];

		[CompilerGenerated]
		private bool HC_0015;

		public TextureCube ShadowFaceMap
		{
			get
			{
				return HC7;
			}
			set
			{
				if (HCq != null && value != HC7)
				{
					HCq.SetValue(value);
					HC7 = value;
				}
			}
		}

		public TextureCube ShadowCoordMap
		{
			get
			{
				return HC_0001;
			}
			set
			{
				if (HCR != null && value != HC_0001)
				{
					HCR.SetValue(value);
					HC_0001 = value;
				}
			}
		}

		public Texture2D ShadowMap => HCH;

		public float ShadowPrimaryBias
		{
			get
			{
				return HCy;
			}
			set
			{
				if (HCf != null && value != HCy)
				{
					HCf.SetValue(value);
					HCy = value;
				}
			}
		}

		public float ShadowSecondaryBias
		{
			get
			{
				return HC_000F;
			}
			set
			{
				if (HCF != null && value != HC_000F)
				{
					HCF.SetValue(value);
					HC_000F = value;
				}
			}
		}

		public Vector4 ShadowViewDistance
		{
			get
			{
				return HCD;
			}
			set
			{
				value.W = Math.Min(value.Z * 0.99f, value.W);
				EffectHelper._0012v(value, ref HCD, ref HC_0010);
			}
		}

		public Vector4[] ShadowMapLocationAndSpan
		{
			set
			{
				Matrix value2 = new Matrix
				{
					M11 = (value[0].X + value[1].X) * 0.5f,
					M12 = (value[0].Y + value[1].Y) * 0.5f,
					M13 = (value[0].Z + value[1].Z) * 0.5f,
					M14 = (value[0].W + value[1].W) * 0.5f,
					M21 = (value[2].X + value[3].X) * 0.5f,
					M22 = (value[2].Y + value[3].Y) * 0.5f,
					M23 = (value[2].Z + value[3].Z) * 0.5f,
					M24 = (value[2].W + value[3].W) * 0.5f,
					M31 = (value[4].X + value[5].X) * 0.5f,
					M32 = (value[4].Y + value[5].Y) * 0.5f,
					M33 = (value[4].Z + value[5].Z) * 0.5f,
					M34 = (value[4].W + value[5].W) * 0.5f
				};
				Matrix value3 = new Matrix
				{
					M11 = (value[0].X - value[1].X) * 0.5f,
					M12 = (value[0].Y - value[1].Y) * 0.5f,
					M13 = (value[0].Z - value[1].Z) * 0.5f,
					M14 = (value[0].W - value[1].W) * 0.5f,
					M21 = (value[2].X - value[3].X) * 0.5f,
					M22 = (value[2].Y - value[3].Y) * 0.5f,
					M23 = (value[2].Z - value[3].Z) * 0.5f,
					M24 = (value[2].W - value[3].W) * 0.5f,
					M31 = (value[4].X - value[5].X) * 0.5f,
					M32 = (value[4].Y - value[5].Y) * 0.5f,
					M33 = (value[4].Z - value[5].Z) * 0.5f,
					M34 = (value[4].W - value[5].W) * 0.5f
				};
				if (HC_0014 != null)
				{
					int num = Math.Min(value.Length, HCe.Length);
					for (int i = 0; i < num; i++)
					{
						ref Vector4 reference = ref HCe[i];
						reference = value[i];
					}
					HC_0014.SetValue(HCe);
				}
				if (HCL != null && HCh != null)
				{
					HCL.SetValue(value2);
					HCh.SetValue(value3);
				}
			}
		}

		public BoundingSphere ShadowArea
		{
			set
			{
				Vector4 vector = new Vector4(value.Center, value.Radius);
				EffectHelper._0012v(vector, ref HC6, ref HCY);
			}
		}

		public Matrix[] ShadowViewProjection
		{
			set
			{
				if (HCG != null)
				{
					int num = Math.Min(value.Length, HCo.Length);
					for (int i = 0; i < num; i++)
					{
						ref Matrix reference = ref HCo[i];
						reference = value[i];
					}
					HCG.SetValue(HCo);
				}
			}
		}

		public _0003.H ShadowSourceType => HCB;

		public bool SupportsShadowGeneration => true;

		public TransparencyMode TransparencyMode => HC_0002;

		public float TransparencyThreshold => HC_0012;

		public Texture TransparencyMap => HCw;

		public TextureAddressMode AddressModeU
		{
			get
			{
				return HC_0011;
			}
			set
			{
				HC_0011 = value;
			}
		}

		public TextureAddressMode AddressModeV
		{
			get
			{
				return HCK;
			}
			set
			{
				HCK = value;
			}
		}

		public TextureAddressMode AddressModeW
		{
			get
			{
				return HC_0003;
			}
			set
			{
				HC_0003 = value;
			}
		}

		public Texture2D HeightMapTexture
		{
			get
			{
				return HCA;
			}
			set
			{
				if (value != HCA)
				{
					EffectHelper._00120(value, ref HCA, ref HC_0006);
					SetTechnique();
				}
			}
		}

		public float HeightScale
		{
			get
			{
				return HCX;
			}
			set
			{
				EffectHelper._00124(value, ref HCX, ref HCr);
			}
		}

		public float Tiling
		{
			get
			{
				return HCz;
			}
			set
			{
				EffectHelper._00124(value, ref HCz, ref HCJ);
			}
		}

		public int TileRepeatCount
		{
			get
			{
				return HC_0013;
			}
			set
			{
				EffectHelper._0012_0005(value, ref HC_0013, ref HCS);
			}
		}

		public int MeshSegments
		{
			get
			{
				return HCs;
			}
			set
			{
				EffectHelper._0012_0005(value, ref HCs, ref HCa);
			}
		}

		public override bool DoubleSided
		{
			[CompilerGenerated]
			get
			{
				return HC_0015;
			}
			[CompilerGenerated]
			set
			{
				HC_0015 = value;
			}
		}

		public void SetCameraView(Matrix view, Matrix viewtoworld)
		{
			EffectHelper._0012I(viewtoworld, ref HCk, ref HCT);
		}

		public void SetTransparencyModeAndMap(TransparencyMode mode, float threshold, Texture map)
		{
			bool flag = false;
			if (mode != HC_0002)
			{
				HC_0002 = mode;
				flag = true;
			}
			if (HCc != null && threshold != HC_0012)
			{
				HC_0012 = threshold;
				HCc.SetValue(HC_0012);
				flag = true;
			}
			Texture2D texture2D = map as Texture2D;
			if (HCN != null && texture2D != HCw)
			{
				HCw = texture2D;
				HCN.SetValue(HCw);
				flag = true;
			}
			if (flag)
			{
				SetTechnique();
			}
		}

		protected override void SetTechnique()
		{
			base.HCV.HCB.AccumulationValue++;
			bool flag = false;
			bool flag2 = HCA != null;
			_0003._0002._0001C_0002 obj = _0003._0002._0001C_0002.Point;
			_0003._0002._0001CB obj2 = _0003._0002._0001CB.ShadowGen;
			if (HCH == null)
			{
				flag = HCw != null && HC_0002 != TransparencyMode.None;
				obj2 = _0003._0002._0001CB.ShadowGen;
				obj = ((HCB != _0003.H.Directional) ? _0003._0002._0001C_0002.Point : _0003._0002._0001C_0002.Directional);
			}
			else if (base.EffectDetail == DetailPreference.High)
			{
				obj2 = _0003._0002._0001CB.Shadow;
				obj = ((HCB != _0003.H.Directional) ? _0003._0002._0001C_0002.Point4 : _0003._0002._0001C_0002.Directional4);
			}
			else if (base.EffectDetail == DetailPreference.Medium)
			{
				obj2 = _0003._0002._0001CB.Shadow;
				obj = ((HCB != _0003.H.Directional) ? _0003._0002._0001C_0002.Point3 : _0003._0002._0001C_0002.Directional3);
			}
			else
			{
				obj2 = _0003._0002._0001CB.Shadow;
				obj = ((HCB != _0003.H.Directional) ? _0003._0002._0001C_0002.Point : _0003._0002._0001C_0002.Directional);
			}
			base.CurrentTechnique = base.Techniques[_0003._0002.H_0012(obj2, obj, 0, false, flag && !flag2, base.Skinned && !flag2, flag2)];
		}

		public void SetShadowMapAndType(Texture2D shadowmap, _0003.H type)
		{
			bool flag = false;
			if (type != HCB)
			{
				HCB = type;
				flag = true;
			}
			if (HCV != null && shadowmap != null && (float)shadowmap.Width != HCZ)
			{
				HCZ = shadowmap.Width;
				HCV.SetValue(HCZ);
				flag = true;
			}
			if (HCu != null && shadowmap != HCH)
			{
				HCH = shadowmap;
				HCu.SetValue(HCH);
				flag = true;
			}
			if (flag)
			{
				SetTechnique();
			}
		}

		public B(GraphicsDevice graphicsdevice)
			: base(graphicsdevice, "ShadowEffect")
		{
			HCc = base.Parameters["_TransparencyClipReference"];
			HCY = base.Parameters["_Direction_Or_Position_And_Radius"];
			HCV = base.Parameters["_ShadowBufferPageSize"];
			HCu = base.Parameters["_ShadowMap"];
			HCq = base.Parameters["_FaceMap"];
			HCR = base.Parameters["_CoordMap"];
			HCN = base.Parameters["_TransparencyMap"];
			HCF = base.Parameters["_DepthBias"];
			HCf = base.Parameters["_OffsetBias"];
			HCG = base.Parameters["_ShadowViewProjection"];
			HC_0010 = base.Parameters["_ShadowViewDistance"];
			HC_0014 = base.Parameters["_RenderTargetLocation_And_Span"];
			HCL = base.Parameters["_RenderTargetLocation_Offset"];
			HCh = base.Parameters["_RenderTargetLocation_Difference"];
			HCT = base.Parameters["_CameraViewToWorld"];
			HC_0006 = base.Parameters["HeightMapTexture"];
			HCa = base.Parameters["MeshSegments"];
			HCS = base.Parameters["MeshRepeatCount"];
			HCr = base.Parameters["HeightScale"];
			HCJ = base.Parameters["Tiling"];
			ShadowFaceMap = SunBurnCoreSystem.Instance._0002W();
			ShadowCoordMap = SunBurnCoreSystem.Instance._0002_0018();
			ShadowPrimaryBias = 1f;
			ShadowSecondaryBias = 0.2f;
			TileRepeatCount = 1;
			SetTechnique();
		}

		protected override Effect Create()
		{
			return new B(base.GraphicsDevice);
		}
	}
}
