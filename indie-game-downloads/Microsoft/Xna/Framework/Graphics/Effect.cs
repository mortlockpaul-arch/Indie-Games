using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Xna.Framework.Graphics;

public class Effect : GraphicsResource
{
	private class IntPtrBoxlessComparer : IEqualityComparer<nint>
	{
		public bool Equals(nint x, nint y)
		{
			return x == y;
		}

		public int GetHashCode(nint obj)
		{
			return ((IntPtr)obj).GetHashCode();
		}
	}

	private enum MOJOSHADER_symbolClass
	{
		MOJOSHADER_SYMCLASS_SCALAR,
		MOJOSHADER_SYMCLASS_VECTOR,
		MOJOSHADER_SYMCLASS_MATRIX_ROWS,
		MOJOSHADER_SYMCLASS_MATRIX_COLUMNS,
		MOJOSHADER_SYMCLASS_OBJECT,
		MOJOSHADER_SYMCLASS_STRUCT,
		MOJOSHADER_SYMCLASS_TOTAL
	}

	private enum MOJOSHADER_symbolType
	{
		MOJOSHADER_SYMTYPE_VOID,
		MOJOSHADER_SYMTYPE_BOOL,
		MOJOSHADER_SYMTYPE_INT,
		MOJOSHADER_SYMTYPE_FLOAT,
		MOJOSHADER_SYMTYPE_STRING,
		MOJOSHADER_SYMTYPE_TEXTURE,
		MOJOSHADER_SYMTYPE_TEXTURE1D,
		MOJOSHADER_SYMTYPE_TEXTURE2D,
		MOJOSHADER_SYMTYPE_TEXTURE3D,
		MOJOSHADER_SYMTYPE_TEXTURECUBE,
		MOJOSHADER_SYMTYPE_SAMPLER,
		MOJOSHADER_SYMTYPE_SAMPLER1D,
		MOJOSHADER_SYMTYPE_SAMPLER2D,
		MOJOSHADER_SYMTYPE_SAMPLER3D,
		MOJOSHADER_SYMTYPE_SAMPLERCUBE,
		MOJOSHADER_SYMTYPE_PIXELSHADER,
		MOJOSHADER_SYMTYPE_VERTEXSHADER,
		MOJOSHADER_SYMTYPE_PIXELFRAGMENT,
		MOJOSHADER_SYMTYPE_VERTEXFRAGMENT,
		MOJOSHADER_SYMTYPE_UNSUPPORTED,
		MOJOSHADER_SYMTYPE_TOTAL
	}

	private struct MOJOSHADER_symbolTypeInfo
	{
		public MOJOSHADER_symbolClass parameter_class;

		public MOJOSHADER_symbolType parameter_type;

		public uint rows;

		public uint columns;

		public uint elements;

		public uint member_count;

		public nint members;
	}

	private struct MOJOSHADER_symbolStructMember
	{
		public nint name;

		public MOJOSHADER_symbolTypeInfo info;
	}

	private enum MOJOSHADER_renderStateType
	{
		MOJOSHADER_RS_ZENABLE = 0,
		MOJOSHADER_RS_FILLMODE = 1,
		MOJOSHADER_RS_SHADEMODE = 2,
		MOJOSHADER_RS_ZWRITEENABLE = 3,
		MOJOSHADER_RS_ALPHATESTENABLE = 4,
		MOJOSHADER_RS_LASTPIXEL = 5,
		MOJOSHADER_RS_SRCBLEND = 6,
		MOJOSHADER_RS_DESTBLEND = 7,
		MOJOSHADER_RS_CULLMODE = 8,
		MOJOSHADER_RS_ZFUNC = 9,
		MOJOSHADER_RS_ALPHAREF = 10,
		MOJOSHADER_RS_ALPHAFUNC = 11,
		MOJOSHADER_RS_DITHERENABLE = 12,
		MOJOSHADER_RS_ALPHABLENDENABLE = 13,
		MOJOSHADER_RS_FOGENABLE = 14,
		MOJOSHADER_RS_SPECULARENABLE = 15,
		MOJOSHADER_RS_FOGCOLOR = 16,
		MOJOSHADER_RS_FOGTABLEMODE = 17,
		MOJOSHADER_RS_FOGSTART = 18,
		MOJOSHADER_RS_FOGEND = 19,
		MOJOSHADER_RS_FOGDENSITY = 20,
		MOJOSHADER_RS_RANGEFOGENABLE = 21,
		MOJOSHADER_RS_STENCILENABLE = 22,
		MOJOSHADER_RS_STENCILFAIL = 23,
		MOJOSHADER_RS_STENCILZFAIL = 24,
		MOJOSHADER_RS_STENCILPASS = 25,
		MOJOSHADER_RS_STENCILFUNC = 26,
		MOJOSHADER_RS_STENCILREF = 27,
		MOJOSHADER_RS_STENCILMASK = 28,
		MOJOSHADER_RS_STENCILWRITEMASK = 29,
		MOJOSHADER_RS_TEXTUREFACTOR = 30,
		MOJOSHADER_RS_WRAP0 = 31,
		MOJOSHADER_RS_WRAP1 = 32,
		MOJOSHADER_RS_WRAP2 = 33,
		MOJOSHADER_RS_WRAP3 = 34,
		MOJOSHADER_RS_WRAP4 = 35,
		MOJOSHADER_RS_WRAP5 = 36,
		MOJOSHADER_RS_WRAP6 = 37,
		MOJOSHADER_RS_WRAP7 = 38,
		MOJOSHADER_RS_WRAP8 = 39,
		MOJOSHADER_RS_WRAP9 = 40,
		MOJOSHADER_RS_WRAP10 = 41,
		MOJOSHADER_RS_WRAP11 = 42,
		MOJOSHADER_RS_WRAP12 = 43,
		MOJOSHADER_RS_WRAP13 = 44,
		MOJOSHADER_RS_WRAP14 = 45,
		MOJOSHADER_RS_WRAP15 = 46,
		MOJOSHADER_RS_CLIPPING = 47,
		MOJOSHADER_RS_LIGHTING = 48,
		MOJOSHADER_RS_AMBIENT = 49,
		MOJOSHADER_RS_FOGVERTEXMODE = 50,
		MOJOSHADER_RS_COLORVERTEX = 51,
		MOJOSHADER_RS_LOCALVIEWER = 52,
		MOJOSHADER_RS_NORMALIZENORMALS = 53,
		MOJOSHADER_RS_DIFFUSEMATERIALSOURCE = 54,
		MOJOSHADER_RS_SPECULARMATERIALSOURCE = 55,
		MOJOSHADER_RS_AMBIENTMATERIALSOURCE = 56,
		MOJOSHADER_RS_EMISSIVEMATERIALSOURCE = 57,
		MOJOSHADER_RS_VERTEXBLEND = 58,
		MOJOSHADER_RS_CLIPPLANEENABLE = 59,
		MOJOSHADER_RS_POINTSIZE = 60,
		MOJOSHADER_RS_POINTSIZE_MIN = 61,
		MOJOSHADER_RS_POINTSPRITEENABLE = 62,
		MOJOSHADER_RS_POINTSCALEENABLE = 63,
		MOJOSHADER_RS_POINTSCALE_A = 64,
		MOJOSHADER_RS_POINTSCALE_B = 65,
		MOJOSHADER_RS_POINTSCALE_C = 66,
		MOJOSHADER_RS_MULTISAMPLEANTIALIAS = 67,
		MOJOSHADER_RS_MULTISAMPLEMASK = 68,
		MOJOSHADER_RS_PATCHEDGESTYLE = 69,
		MOJOSHADER_RS_DEBUGMONITORTOKEN = 70,
		MOJOSHADER_RS_POINTSIZE_MAX = 71,
		MOJOSHADER_RS_INDEXEDVERTEXBLENDENABLE = 72,
		MOJOSHADER_RS_COLORWRITEENABLE = 73,
		MOJOSHADER_RS_TWEENFACTOR = 74,
		MOJOSHADER_RS_BLENDOP = 75,
		MOJOSHADER_RS_POSITIONDEGREE = 76,
		MOJOSHADER_RS_NORMALDEGREE = 77,
		MOJOSHADER_RS_SCISSORTESTENABLE = 78,
		MOJOSHADER_RS_SLOPESCALEDEPTHBIAS = 79,
		MOJOSHADER_RS_ANTIALIASEDLINEENABLE = 80,
		MOJOSHADER_RS_MINTESSELLATIONLEVEL = 81,
		MOJOSHADER_RS_MAXTESSELLATIONLEVEL = 82,
		MOJOSHADER_RS_ADAPTIVETESS_X = 83,
		MOJOSHADER_RS_ADAPTIVETESS_Y = 84,
		MOJOSHADER_RS_ADAPTIVETESS_Z = 85,
		MOJOSHADER_RS_ADAPTIVETESS_W = 86,
		MOJOSHADER_RS_ENABLEADAPTIVETESSELLATION = 87,
		MOJOSHADER_RS_TWOSIDEDSTENCILMODE = 88,
		MOJOSHADER_RS_CCW_STENCILFAIL = 89,
		MOJOSHADER_RS_CCW_STENCILZFAIL = 90,
		MOJOSHADER_RS_CCW_STENCILPASS = 91,
		MOJOSHADER_RS_CCW_STENCILFUNC = 92,
		MOJOSHADER_RS_COLORWRITEENABLE1 = 93,
		MOJOSHADER_RS_COLORWRITEENABLE2 = 94,
		MOJOSHADER_RS_COLORWRITEENABLE3 = 95,
		MOJOSHADER_RS_BLENDFACTOR = 96,
		MOJOSHADER_RS_SRGBWRITEENABLE = 97,
		MOJOSHADER_RS_DEPTHBIAS = 98,
		MOJOSHADER_RS_SEPARATEALPHABLENDENABLE = 99,
		MOJOSHADER_RS_SRCBLENDALPHA = 100,
		MOJOSHADER_RS_DESTBLENDALPHA = 101,
		MOJOSHADER_RS_BLENDOPALPHA = 102,
		MOJOSHADER_RS_VERTEXSHADER = 146,
		MOJOSHADER_RS_PIXELSHADER = 147
	}

	private enum MOJOSHADER_zBufferType
	{
		MOJOSHADER_ZB_FALSE,
		MOJOSHADER_ZB_TRUE,
		MOJOSHADER_ZB_USEW
	}

	private enum MOJOSHADER_fillMode
	{
		MOJOSHADER_FILL_POINT = 1,
		MOJOSHADER_FILL_WIREFRAME,
		MOJOSHADER_FILL_SOLID
	}

	private enum MOJOSHADER_blendMode
	{
		MOJOSHADER_BLEND_ZERO = 1,
		MOJOSHADER_BLEND_ONE,
		MOJOSHADER_BLEND_SRCCOLOR,
		MOJOSHADER_BLEND_INVSRCCOLOR,
		MOJOSHADER_BLEND_SRCALPHA,
		MOJOSHADER_BLEND_INVSRCALPHA,
		MOJOSHADER_BLEND_DESTALPHA,
		MOJOSHADER_BLEND_INVDESTALPHA,
		MOJOSHADER_BLEND_DESTCOLOR,
		MOJOSHADER_BLEND_INVDESTCOLOR,
		MOJOSHADER_BLEND_SRCALPHASAT,
		MOJOSHADER_BLEND_BOTHSRCALPHA,
		MOJOSHADER_BLEND_BOTHINVSRCALPHA,
		MOJOSHADER_BLEND_BLENDFACTOR,
		MOJOSHADER_BLEND_INVBLENDFACTOR,
		MOJOSHADER_BLEND_SRCCOLOR2,
		MOJOSHADER_BLEND_INVSRCCOLOR2
	}

	private enum MOJOSHADER_cullMode
	{
		MOJOSHADER_CULL_NONE = 1,
		MOJOSHADER_CULL_CW,
		MOJOSHADER_CULL_CCW
	}

	private enum MOJOSHADER_compareFunc
	{
		MOJOSHADER_CMP_NEVER = 1,
		MOJOSHADER_CMP_LESS,
		MOJOSHADER_CMP_EQUAL,
		MOJOSHADER_CMP_LESSEQUAL,
		MOJOSHADER_CMP_GREATER,
		MOJOSHADER_CMP_NOTEQUAL,
		MOJOSHADER_CMP_GREATEREQUAL,
		MOJOSHADER_CMP_ALWAYS
	}

	private enum MOJOSHADER_stencilOp
	{
		MOJOSHADER_STENCILOP_KEEP = 1,
		MOJOSHADER_STENCILOP_ZERO,
		MOJOSHADER_STENCILOP_REPLACE,
		MOJOSHADER_STENCILOP_INCRSAT,
		MOJOSHADER_STENCILOP_DECRSAT,
		MOJOSHADER_STENCILOP_INVERT,
		MOJOSHADER_STENCILOP_INCR,
		MOJOSHADER_STENCILOP_DECR
	}

	private enum MOJOSHADER_blendOp
	{
		MOJOSHADER_BLENDOP_ADD = 1,
		MOJOSHADER_BLENDOP_SUBTRACT,
		MOJOSHADER_BLENDOP_REVSUBTRACT,
		MOJOSHADER_BLENDOP_MIN,
		MOJOSHADER_BLENDOP_MAX
	}

	private enum MOJOSHADER_samplerStateType
	{
		MOJOSHADER_SAMP_UNKNOWN0,
		MOJOSHADER_SAMP_UNKNOWN1,
		MOJOSHADER_SAMP_UNKNOWN2,
		MOJOSHADER_SAMP_UNKNOWN3,
		MOJOSHADER_SAMP_TEXTURE,
		MOJOSHADER_SAMP_ADDRESSU,
		MOJOSHADER_SAMP_ADDRESSV,
		MOJOSHADER_SAMP_ADDRESSW,
		MOJOSHADER_SAMP_BORDERCOLOR,
		MOJOSHADER_SAMP_MAGFILTER,
		MOJOSHADER_SAMP_MINFILTER,
		MOJOSHADER_SAMP_MIPFILTER,
		MOJOSHADER_SAMP_MIPMAPLODBIAS,
		MOJOSHADER_SAMP_MAXMIPLEVEL,
		MOJOSHADER_SAMP_MAXANISOTROPY,
		MOJOSHADER_SAMP_SRGBTEXTURE,
		MOJOSHADER_SAMP_ELEMENTINDEX,
		MOJOSHADER_SAMP_DMAPOFFSET
	}

	private enum MOJOSHADER_textureAddress
	{
		MOJOSHADER_TADDRESS_WRAP = 1,
		MOJOSHADER_TADDRESS_MIRROR,
		MOJOSHADER_TADDRESS_CLAMP,
		MOJOSHADER_TADDRESS_BORDER,
		MOJOSHADER_TADDRESS_MIRRORONCE
	}

	private enum MOJOSHADER_textureFilterType
	{
		MOJOSHADER_TEXTUREFILTER_NONE,
		MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_TEXTUREFILTER_ANISOTROPIC,
		MOJOSHADER_TEXTUREFILTER_PYRAMIDALQUAD,
		MOJOSHADER_TEXTUREFILTER_GAUSSIANQUAD,
		MOJOSHADER_TEXTUREFILTER_CONVOLUTIONMONO
	}

	private struct MOJOSHADER_effectValue
	{
		public nint name;

		public nint semantic;

		public MOJOSHADER_symbolTypeInfo type;

		public uint value_count;

		public nint values;
	}

	private struct MOJOSHADER_effectState
	{
		public MOJOSHADER_renderStateType type;

		public MOJOSHADER_effectValue value;
	}

	private struct MOJOSHADER_effectSamplerState
	{
		public MOJOSHADER_samplerStateType type;

		public MOJOSHADER_effectValue value;
	}

	private struct MOJOSHADER_effectAnnotation
	{
		public nint name;

		public nint semantic;

		public MOJOSHADER_symbolTypeInfo type;

		public uint value_count;

		public nint values;
	}

	private struct MOJOSHADER_effectParam
	{
		public MOJOSHADER_effectValue value;

		public uint annotation_count;

		public nint annotations;
	}

	private struct MOJOSHADER_effectPass
	{
		public nint name;

		public uint state_count;

		public nint states;

		public uint annotation_count;

		public nint annotations;
	}

	private struct MOJOSHADER_effectTechnique
	{
		public nint name;

		public uint pass_count;

		public nint passes;

		public uint annotation_count;

		public nint annotations;
	}

	private struct MOJOSHADER_effectShader
	{
		public MOJOSHADER_symbolType type;

		public uint technique;

		public uint pass;

		public uint is_preshader;

		public uint preshader_param_count;

		public nint preshader_params;

		public uint param_count;

		public nint parameters;

		public uint sampler_count;

		public nint samplers;

		public nint shader;
	}

	private struct MOJOSHADER_effectSamplerMap
	{
		public MOJOSHADER_symbolType type;

		public nint name;
	}

	private struct MOJOSHADER_effectString
	{
		public MOJOSHADER_symbolType type;

		public nint stringvalue;
	}

	private struct MOJOSHADER_effectTexture
	{
		public MOJOSHADER_symbolType type;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct MOJOSHADER_effectObject
	{
		[FieldOffset(0)]
		public MOJOSHADER_symbolType type;

		[FieldOffset(0)]
		public MOJOSHADER_effectShader shader;

		[FieldOffset(0)]
		public MOJOSHADER_effectSamplerMap mapping;

		[FieldOffset(0)]
		public MOJOSHADER_effectString stringvalue;

		[FieldOffset(0)]
		public MOJOSHADER_effectTexture texture;
	}

	private struct MOJOSHADER_samplerStateRegister
	{
		public nint sampler_name;

		public uint sampler_register;

		public uint sampler_state_count;

		public nint sampler_states;
	}

	internal struct MOJOSHADER_effectStateChanges
	{
		public uint render_state_change_count;

		public nint render_state_changes;

		public uint sampler_state_change_count;

		public nint sampler_state_changes;

		public uint vertex_sampler_state_change_count;

		public nint vertex_sampler_state_changes;
	}

	private struct MOJOSHADER_effect
	{
		public int error_count;

		public nint errors;

		public int param_count;

		public nint parameters;

		public int technique_count;

		public nint techniques;

		public int object_count;

		public nint objects;
	}

	private EffectTechnique INTERNAL_currentTechnique;

	internal nint glEffect;

	private Dictionary<nint, EffectParameter> samplerMap = new Dictionary<nint, EffectParameter>(new IntPtrBoxlessComparer());

	private nint effectData;

	private static readonly EffectParameterType[] XNAType = new EffectParameterType[10]
	{
		EffectParameterType.Void,
		EffectParameterType.Bool,
		EffectParameterType.Int32,
		EffectParameterType.Single,
		EffectParameterType.String,
		EffectParameterType.Texture,
		EffectParameterType.Texture1D,
		EffectParameterType.Texture2D,
		EffectParameterType.Texture3D,
		EffectParameterType.TextureCube
	};

	private static readonly EffectParameterClass[] XNAClass = new EffectParameterClass[6]
	{
		EffectParameterClass.Scalar,
		EffectParameterClass.Vector,
		EffectParameterClass.Matrix,
		EffectParameterClass.Matrix,
		EffectParameterClass.Object,
		EffectParameterClass.Struct
	};

	private static readonly Blend[] XNABlend = new Blend[16]
	{
		(Blend)(-1),
		Blend.Zero,
		Blend.One,
		Blend.SourceColor,
		Blend.InverseSourceColor,
		Blend.SourceAlpha,
		Blend.InverseSourceAlpha,
		Blend.DestinationAlpha,
		Blend.InverseDestinationAlpha,
		Blend.DestinationColor,
		Blend.InverseDestinationColor,
		Blend.SourceAlphaSaturation,
		(Blend)(-1),
		(Blend)(-1),
		Blend.BlendFactor,
		Blend.InverseBlendFactor
	};

	private static readonly BlendFunction[] XNABlendOp = new BlendFunction[6]
	{
		(BlendFunction)(-1),
		BlendFunction.Add,
		BlendFunction.Subtract,
		BlendFunction.ReverseSubtract,
		BlendFunction.Min,
		BlendFunction.Max
	};

	private static readonly CompareFunction[] XNACompare = new CompareFunction[9]
	{
		(CompareFunction)(-1),
		CompareFunction.Never,
		CompareFunction.Less,
		CompareFunction.Equal,
		CompareFunction.LessEqual,
		CompareFunction.Greater,
		CompareFunction.NotEqual,
		CompareFunction.GreaterEqual,
		CompareFunction.Always
	};

	private static readonly StencilOperation[] XNAStencilOp = new StencilOperation[9]
	{
		(StencilOperation)(-1),
		StencilOperation.Keep,
		StencilOperation.Zero,
		StencilOperation.Replace,
		StencilOperation.IncrementSaturation,
		StencilOperation.DecrementSaturation,
		StencilOperation.Invert,
		StencilOperation.Increment,
		StencilOperation.Decrement
	};

	private static readonly TextureAddressMode[] XNAAddress = new TextureAddressMode[4]
	{
		(TextureAddressMode)(-1),
		TextureAddressMode.Wrap,
		TextureAddressMode.Mirror,
		TextureAddressMode.Clamp
	};

	private static readonly MOJOSHADER_textureFilterType[] XNAMag = new MOJOSHADER_textureFilterType[9]
	{
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR
	};

	private static readonly MOJOSHADER_textureFilterType[] XNAMin = new MOJOSHADER_textureFilterType[9]
	{
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT
	};

	private static readonly MOJOSHADER_textureFilterType[] XNAMip = new MOJOSHADER_textureFilterType[9]
	{
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR,
		MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT
	};

	public EffectTechnique CurrentTechnique
	{
		get
		{
			return INTERNAL_currentTechnique;
		}
		set
		{
			FNA3D.FNA3D_SetEffectTechnique(base.GraphicsDevice.GLDevice, glEffect, value.TechniquePointer);
			INTERNAL_currentTechnique = value;
		}
	}

	public EffectParameterCollection Parameters { get; private set; }

	public EffectTechniqueCollection Techniques { get; private set; }

	public Effect(GraphicsDevice graphicsDevice, byte[] effectCode)
	{
		base.GraphicsDevice = graphicsDevice;
		FNA3D.FNA3D_CreateEffect(graphicsDevice.GLDevice, effectCode, effectCode.Length, out glEffect, out var num);
		effectData = num;
		INTERNAL_parseEffectStruct(num);
		CurrentTechnique = Techniques[0];
	}

	protected Effect(Effect cloneSource)
	{
		base.GraphicsDevice = cloneSource.GraphicsDevice;
		FNA3D.FNA3D_CloneEffect(base.GraphicsDevice.GLDevice, cloneSource.glEffect, out glEffect, out var num);
		effectData = num;
		INTERNAL_parseEffectStruct(num);
		for (int i = 0; i < cloneSource.Parameters.Count; i++)
		{
			Parameters[i].texture = cloneSource.Parameters[i].texture;
		}
		for (int j = 0; j < cloneSource.Techniques.Count; j++)
		{
			if (cloneSource.Techniques[j] == cloneSource.CurrentTechnique)
			{
				CurrentTechnique = Techniques[j];
			}
		}
	}

	public virtual Effect Clone()
	{
		return new Effect(this);
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.IsDisposed)
		{
			nint num = Interlocked.Exchange(ref glEffect, IntPtr.Zero);
			if (num != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeEffect(base.GraphicsDevice.GLDevice, num);
			}
		}
		base.Dispose(disposing);
	}

	protected internal virtual void OnApply()
	{
	}

	internal unsafe void INTERNAL_applyEffect(uint pass)
	{
		FNA3D.FNA3D_ApplyEffect(base.GraphicsDevice.GLDevice, glEffect, pass, base.GraphicsDevice.effectStateChangesPtr);
		MOJOSHADER_effectStateChanges* effectStateChangesPtr = (MOJOSHADER_effectStateChanges*)base.GraphicsDevice.effectStateChangesPtr;
		if (effectStateChangesPtr->render_state_change_count != 0)
		{
			PipelineCache pipelineCache = base.GraphicsDevice.PipelineCache;
			pipelineCache.BeginApplyBlend();
			pipelineCache.BeginApplyDepthStencil();
			pipelineCache.BeginApplyRasterizer();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			MOJOSHADER_effectState* render_state_changes = (MOJOSHADER_effectState*)effectStateChangesPtr->render_state_changes;
			for (int i = 0; i < effectStateChangesPtr->render_state_change_count; i++)
			{
				MOJOSHADER_renderStateType type = render_state_changes[i].type;
				if (type == MOJOSHADER_renderStateType.MOJOSHADER_RS_VERTEXSHADER || type == MOJOSHADER_renderStateType.MOJOSHADER_RS_PIXELSHADER)
				{
					continue;
				}
				switch (type)
				{
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_ZENABLE:
				{
					MOJOSHADER_zBufferType* values30 = (MOJOSHADER_zBufferType*)render_state_changes[i].value.values;
					pipelineCache.DepthBufferEnable = *values30 == MOJOSHADER_zBufferType.MOJOSHADER_ZB_TRUE;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_FILLMODE:
				{
					MOJOSHADER_fillMode* values35 = (MOJOSHADER_fillMode*)render_state_changes[i].value.values;
					if (*values35 == MOJOSHADER_fillMode.MOJOSHADER_FILL_SOLID)
					{
						pipelineCache.FillMode = FillMode.Solid;
					}
					else if (*values35 == MOJOSHADER_fillMode.MOJOSHADER_FILL_WIREFRAME)
					{
						pipelineCache.FillMode = FillMode.WireFrame;
					}
					flag3 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_ZWRITEENABLE:
				{
					int* values34 = (int*)render_state_changes[i].value.values;
					pipelineCache.DepthBufferWriteEnable = *values34 == 1;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_SRCBLEND:
				{
					MOJOSHADER_blendMode* values36 = (MOJOSHADER_blendMode*)render_state_changes[i].value.values;
					pipelineCache.ColorSourceBlend = XNABlend[(int)(*values36)];
					if (!pipelineCache.SeparateAlphaBlend)
					{
						pipelineCache.AlphaSourceBlend = XNABlend[(int)(*values36)];
					}
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_DESTBLEND:
				{
					MOJOSHADER_blendMode* values33 = (MOJOSHADER_blendMode*)render_state_changes[i].value.values;
					pipelineCache.ColorDestinationBlend = XNABlend[(int)(*values33)];
					if (!pipelineCache.SeparateAlphaBlend)
					{
						pipelineCache.AlphaDestinationBlend = XNABlend[(int)(*values33)];
					}
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_CULLMODE:
				{
					MOJOSHADER_cullMode* values32 = (MOJOSHADER_cullMode*)render_state_changes[i].value.values;
					if (*values32 == MOJOSHADER_cullMode.MOJOSHADER_CULL_NONE)
					{
						pipelineCache.CullMode = CullMode.None;
					}
					else if (*values32 == MOJOSHADER_cullMode.MOJOSHADER_CULL_CW)
					{
						pipelineCache.CullMode = CullMode.CullClockwiseFace;
					}
					else if (*values32 == MOJOSHADER_cullMode.MOJOSHADER_CULL_CCW)
					{
						pipelineCache.CullMode = CullMode.CullCounterClockwiseFace;
					}
					flag3 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_ZFUNC:
				{
					MOJOSHADER_compareFunc* values31 = (MOJOSHADER_compareFunc*)render_state_changes[i].value.values;
					pipelineCache.DepthBufferFunction = XNACompare[(int)(*values31)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_ALPHABLENDENABLE:
				{
					int* values29 = (int*)render_state_changes[i].value.values;
					if (*values29 == 0)
					{
						pipelineCache.ColorSourceBlend = Blend.One;
						pipelineCache.ColorDestinationBlend = Blend.Zero;
						pipelineCache.AlphaSourceBlend = Blend.One;
						pipelineCache.AlphaDestinationBlend = Blend.Zero;
						flag = true;
					}
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILENABLE:
				{
					int* values28 = (int*)render_state_changes[i].value.values;
					pipelineCache.StencilEnable = *values28 == 1;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILFAIL:
				{
					MOJOSHADER_stencilOp* values27 = (MOJOSHADER_stencilOp*)render_state_changes[i].value.values;
					pipelineCache.StencilFail = XNAStencilOp[(int)(*values27)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILZFAIL:
				{
					MOJOSHADER_stencilOp* values26 = (MOJOSHADER_stencilOp*)render_state_changes[i].value.values;
					pipelineCache.StencilDepthBufferFail = XNAStencilOp[(int)(*values26)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILPASS:
				{
					MOJOSHADER_stencilOp* values25 = (MOJOSHADER_stencilOp*)render_state_changes[i].value.values;
					pipelineCache.StencilPass = XNAStencilOp[(int)(*values25)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILFUNC:
				{
					MOJOSHADER_compareFunc* values24 = (MOJOSHADER_compareFunc*)render_state_changes[i].value.values;
					pipelineCache.StencilFunction = XNACompare[(int)(*values24)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILREF:
				{
					int* values23 = (int*)render_state_changes[i].value.values;
					pipelineCache.ReferenceStencil = *values23;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILMASK:
				{
					int* values22 = (int*)render_state_changes[i].value.values;
					pipelineCache.StencilMask = *values22;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_STENCILWRITEMASK:
				{
					int* values21 = (int*)render_state_changes[i].value.values;
					pipelineCache.StencilWriteMask = *values21;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_MULTISAMPLEANTIALIAS:
				{
					int* values20 = (int*)render_state_changes[i].value.values;
					pipelineCache.MultiSampleAntiAlias = *values20 == 1;
					flag3 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_MULTISAMPLEMASK:
				{
					int* values19 = (int*)render_state_changes[i].value.values;
					pipelineCache.MultiSampleMask = *values19;
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_COLORWRITEENABLE:
				{
					int* values18 = (int*)render_state_changes[i].value.values;
					pipelineCache.ColorWriteChannels = (ColorWriteChannels)(*values18);
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_BLENDOP:
				{
					MOJOSHADER_blendOp* values17 = (MOJOSHADER_blendOp*)render_state_changes[i].value.values;
					pipelineCache.ColorBlendFunction = XNABlendOp[(int)(*values17)];
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_SCISSORTESTENABLE:
				{
					int* values16 = (int*)render_state_changes[i].value.values;
					pipelineCache.ScissorTestEnable = *values16 == 1;
					flag3 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_SLOPESCALEDEPTHBIAS:
				{
					float* values15 = (float*)render_state_changes[i].value.values;
					pipelineCache.SlopeScaleDepthBias = *values15;
					flag3 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_TWOSIDEDSTENCILMODE:
				{
					int* values14 = (int*)render_state_changes[i].value.values;
					pipelineCache.TwoSidedStencilMode = *values14 == 1;
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_CCW_STENCILFAIL:
				{
					MOJOSHADER_stencilOp* values13 = (MOJOSHADER_stencilOp*)render_state_changes[i].value.values;
					pipelineCache.CCWStencilFail = XNAStencilOp[(int)(*values13)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_CCW_STENCILZFAIL:
				{
					MOJOSHADER_stencilOp* values12 = (MOJOSHADER_stencilOp*)render_state_changes[i].value.values;
					pipelineCache.CCWStencilDepthBufferFail = XNAStencilOp[(int)(*values12)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_CCW_STENCILPASS:
				{
					MOJOSHADER_stencilOp* values11 = (MOJOSHADER_stencilOp*)render_state_changes[i].value.values;
					pipelineCache.CCWStencilPass = XNAStencilOp[(int)(*values11)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_CCW_STENCILFUNC:
				{
					MOJOSHADER_compareFunc* values10 = (MOJOSHADER_compareFunc*)render_state_changes[i].value.values;
					pipelineCache.CCWStencilFunction = XNACompare[(int)(*values10)];
					flag2 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_COLORWRITEENABLE1:
				{
					int* values9 = (int*)render_state_changes[i].value.values;
					pipelineCache.ColorWriteChannels1 = (ColorWriteChannels)(*values9);
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_COLORWRITEENABLE2:
				{
					int* values8 = (int*)render_state_changes[i].value.values;
					pipelineCache.ColorWriteChannels2 = (ColorWriteChannels)(*values8);
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_COLORWRITEENABLE3:
				{
					int* values7 = (int*)render_state_changes[i].value.values;
					pipelineCache.ColorWriteChannels3 = (ColorWriteChannels)(*values7);
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_BLENDFACTOR:
				{
					int* values6 = (int*)render_state_changes[i].value.values;
					pipelineCache.BlendFactor = new Color((*values6 >> 24) & 0xFF, (*values6 >> 16) & 0xFF, (*values6 >> 8) & 0xFF, *values6 & 0xFF);
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_DEPTHBIAS:
				{
					float* values5 = (float*)render_state_changes[i].value.values;
					pipelineCache.DepthBias = *values5;
					flag3 = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_SEPARATEALPHABLENDENABLE:
				{
					int* values4 = (int*)render_state_changes[i].value.values;
					pipelineCache.SeparateAlphaBlend = *values4 == 1;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_SRCBLENDALPHA:
				{
					MOJOSHADER_blendMode* values3 = (MOJOSHADER_blendMode*)render_state_changes[i].value.values;
					pipelineCache.AlphaSourceBlend = XNABlend[(int)(*values3)];
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_DESTBLENDALPHA:
				{
					MOJOSHADER_blendMode* values2 = (MOJOSHADER_blendMode*)render_state_changes[i].value.values;
					pipelineCache.AlphaDestinationBlend = XNABlend[(int)(*values2)];
					flag = true;
					break;
				}
				case MOJOSHADER_renderStateType.MOJOSHADER_RS_BLENDOPALPHA:
				{
					MOJOSHADER_blendOp* values = (MOJOSHADER_blendOp*)render_state_changes[i].value.values;
					pipelineCache.AlphaBlendFunction = XNABlendOp[(int)(*values)];
					flag = true;
					break;
				}
				default:
					throw new NotImplementedException("Unhandled render state! " + type);
				case (MOJOSHADER_renderStateType)178:
					break;
				}
			}
			if (flag)
			{
				pipelineCache.EndApplyBlend();
			}
			if (flag2)
			{
				pipelineCache.EndApplyDepthStencil();
			}
			if (flag3)
			{
				pipelineCache.EndApplyRasterizer();
			}
		}
		if (effectStateChangesPtr->sampler_state_change_count != 0)
		{
			INTERNAL_updateSamplers(effectStateChangesPtr->sampler_state_change_count, (MOJOSHADER_samplerStateRegister*)effectStateChangesPtr->sampler_state_changes, base.GraphicsDevice.Textures, base.GraphicsDevice.SamplerStates);
		}
		if (effectStateChangesPtr->vertex_sampler_state_change_count != 0)
		{
			INTERNAL_updateSamplers(effectStateChangesPtr->vertex_sampler_state_change_count, (MOJOSHADER_samplerStateRegister*)effectStateChangesPtr->vertex_sampler_state_changes, base.GraphicsDevice.VertexTextures, base.GraphicsDevice.VertexSamplerStates);
		}
	}

	private unsafe void INTERNAL_updateSamplers(uint changeCount, MOJOSHADER_samplerStateRegister* registers, TextureCollection textures, SamplerStateCollection samplers)
	{
		for (int i = 0; i < changeCount; i++)
		{
			if (registers[i].sampler_state_count == 0)
			{
				continue;
			}
			int sampler_register = (int)registers[i].sampler_register;
			PipelineCache pipelineCache = base.GraphicsDevice.PipelineCache;
			pipelineCache.BeginApplySampler(samplers, sampler_register);
			bool flag = false;
			bool flag2 = false;
			TextureFilter filter = pipelineCache.Filter;
			MOJOSHADER_textureFilterType mOJOSHADER_textureFilterType = XNAMag[(int)filter];
			MOJOSHADER_textureFilterType mOJOSHADER_textureFilterType2 = XNAMin[(int)filter];
			MOJOSHADER_textureFilterType mOJOSHADER_textureFilterType3 = XNAMip[(int)filter];
			MOJOSHADER_effectSamplerState* sampler_states = (MOJOSHADER_effectSamplerState*)registers[i].sampler_states;
			for (int j = 0; j < registers[i].sampler_state_count; j++)
			{
				MOJOSHADER_samplerStateType type = sampler_states[j].type;
				switch (type)
				{
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_TEXTURE:
				{
					if (samplerMap.TryGetValue(registers[i].sampler_name, out var value))
					{
						Texture texture = value.texture;
						if (texture != null)
						{
							textures[sampler_register] = texture;
						}
					}
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_ADDRESSU:
				{
					MOJOSHADER_textureAddress* values9 = (MOJOSHADER_textureAddress*)sampler_states[j].value.values;
					pipelineCache.AddressU = XNAAddress[(int)(*values9)];
					flag = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_ADDRESSV:
				{
					MOJOSHADER_textureAddress* values8 = (MOJOSHADER_textureAddress*)sampler_states[j].value.values;
					pipelineCache.AddressV = XNAAddress[(int)(*values8)];
					flag = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_ADDRESSW:
				{
					MOJOSHADER_textureAddress* values7 = (MOJOSHADER_textureAddress*)sampler_states[j].value.values;
					pipelineCache.AddressW = XNAAddress[(int)(*values7)];
					flag = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_MAGFILTER:
				{
					MOJOSHADER_textureFilterType* values6 = (MOJOSHADER_textureFilterType*)sampler_states[j].value.values;
					mOJOSHADER_textureFilterType = *values6;
					flag2 = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_MINFILTER:
				{
					MOJOSHADER_textureFilterType* values5 = (MOJOSHADER_textureFilterType*)sampler_states[j].value.values;
					mOJOSHADER_textureFilterType2 = *values5;
					flag2 = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_MIPFILTER:
				{
					MOJOSHADER_textureFilterType* values4 = (MOJOSHADER_textureFilterType*)sampler_states[j].value.values;
					mOJOSHADER_textureFilterType3 = *values4;
					flag2 = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_MIPMAPLODBIAS:
				{
					float* values3 = (float*)sampler_states[j].value.values;
					pipelineCache.MipMapLODBias = *values3;
					flag = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_MAXMIPLEVEL:
				{
					int* values2 = (int*)sampler_states[j].value.values;
					pipelineCache.MaxMipLevel = *values2;
					flag = true;
					break;
				}
				case MOJOSHADER_samplerStateType.MOJOSHADER_SAMP_MAXANISOTROPY:
				{
					int* values = (int*)sampler_states[j].value.values;
					pipelineCache.MaxAnisotropy = *values;
					flag = true;
					break;
				}
				default:
					throw new NotImplementedException("Unhandled sampler state! " + type);
				}
			}
			if (flag2)
			{
				if (mOJOSHADER_textureFilterType == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
				{
					if (mOJOSHADER_textureFilterType2 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
					{
						if (mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_NONE || mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
						{
							pipelineCache.Filter = TextureFilter.Point;
						}
						else
						{
							if (mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
							{
								throw new NotImplementedException("Unhandled mipfilter type! " + mOJOSHADER_textureFilterType3);
							}
							pipelineCache.Filter = TextureFilter.PointMipLinear;
						}
					}
					else
					{
						if (mOJOSHADER_textureFilterType2 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType2 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
						{
							throw new NotImplementedException("Unhandled minfilter type! " + mOJOSHADER_textureFilterType2);
						}
						if (mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_NONE || mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
						{
							pipelineCache.Filter = TextureFilter.MinLinearMagPointMipPoint;
						}
						else
						{
							if (mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
							{
								throw new NotImplementedException("Unhandled mipfilter type! " + mOJOSHADER_textureFilterType3);
							}
							pipelineCache.Filter = TextureFilter.MinLinearMagPointMipLinear;
						}
					}
				}
				else
				{
					if (mOJOSHADER_textureFilterType != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
					{
						throw new NotImplementedException("Unhandled magfilter type! " + mOJOSHADER_textureFilterType);
					}
					if (mOJOSHADER_textureFilterType2 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
					{
						if (mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_NONE || mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
						{
							pipelineCache.Filter = TextureFilter.MinPointMagLinearMipPoint;
						}
						else
						{
							if (mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
							{
								throw new NotImplementedException("Unhandled mipfilter type! " + mOJOSHADER_textureFilterType3);
							}
							pipelineCache.Filter = TextureFilter.MinPointMagLinearMipLinear;
						}
					}
					else
					{
						if (mOJOSHADER_textureFilterType2 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType2 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
						{
							throw new NotImplementedException("Unhandled minfilter type! " + mOJOSHADER_textureFilterType2);
						}
						if (mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_NONE || mOJOSHADER_textureFilterType3 == MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_POINT)
						{
							pipelineCache.Filter = TextureFilter.LinearMipPoint;
						}
						else
						{
							if (mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_LINEAR && mOJOSHADER_textureFilterType3 != MOJOSHADER_textureFilterType.MOJOSHADER_TEXTUREFILTER_ANISOTROPIC)
							{
								throw new NotImplementedException("Unhandled mipfilter type! " + mOJOSHADER_textureFilterType3);
							}
							pipelineCache.Filter = TextureFilter.Linear;
						}
					}
				}
				flag = true;
			}
			if (flag)
			{
				pipelineCache.EndApplySampler(samplers, sampler_register);
			}
		}
	}

	private unsafe void INTERNAL_parseEffectStruct(nint effectData)
	{
		MOJOSHADER_effectParam* parameters = (MOJOSHADER_effectParam*)((MOJOSHADER_effect*)effectData)->parameters;
		List<EffectParameter> list = new List<EffectParameter>();
		for (int i = 0; i < ((MOJOSHADER_effect*)effectData)->param_count; i++)
		{
			MOJOSHADER_effectParam mOJOSHADER_effectParam = parameters[i];
			if (mOJOSHADER_effectParam.value.type.parameter_type == MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_VERTEXSHADER || mOJOSHADER_effectParam.value.type.parameter_type == MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_PIXELSHADER)
			{
				continue;
			}
			if (mOJOSHADER_effectParam.value.type.parameter_type >= MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_SAMPLER && mOJOSHADER_effectParam.value.type.parameter_type <= MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_SAMPLERCUBE)
			{
				string text = string.Empty;
				MOJOSHADER_effectSamplerState* values = (MOJOSHADER_effectSamplerState*)mOJOSHADER_effectParam.value.values;
				for (int j = 0; j < mOJOSHADER_effectParam.value.value_count; j++)
				{
					if (values[j].value.type.parameter_type >= MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_TEXTURE && values[j].value.type.parameter_type <= MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_TEXTURECUBE)
					{
						MOJOSHADER_effectObject* objects = (MOJOSHADER_effectObject*)((MOJOSHADER_effect*)effectData)->objects;
						int* values2 = (int*)values[j].value.values;
						text = Marshal.PtrToStringAnsi(objects[*values2].mapping.name);
						break;
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					if (text.Equals(list[k].Name))
					{
						samplerMap[mOJOSHADER_effectParam.value.name] = list[k];
						break;
					}
				}
			}
			else
			{
				EffectParameter effectParameter = new EffectParameter(MarshalHelper.PtrToInternedStringAnsi(mOJOSHADER_effectParam.value.name), MarshalHelper.PtrToInternedStringAnsi(mOJOSHADER_effectParam.value.semantic), (int)mOJOSHADER_effectParam.value.type.rows, (int)mOJOSHADER_effectParam.value.type.columns, (int)mOJOSHADER_effectParam.value.type.elements, XNAClass[(int)mOJOSHADER_effectParam.value.type.parameter_class], XNAType[(int)mOJOSHADER_effectParam.value.type.parameter_type], new IntPtr(&parameters[i].value.type), INTERNAL_readAnnotations(mOJOSHADER_effectParam.annotations, mOJOSHADER_effectParam.annotation_count), mOJOSHADER_effectParam.value.values, mOJOSHADER_effectParam.value.value_count * 4, this);
				if (mOJOSHADER_effectParam.value.type.parameter_type == MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_STRING)
				{
					int* values3 = (int*)mOJOSHADER_effectParam.value.values;
					effectParameter.cachedString = INTERNAL_GetStringFromObjectTable(*values3);
				}
				list.Add(effectParameter);
			}
		}
		Parameters = new EffectParameterCollection(list);
		MOJOSHADER_effectTechnique* ptr = (MOJOSHADER_effectTechnique*)((MOJOSHADER_effect*)effectData)->techniques;
		List<EffectTechnique> list2 = new List<EffectTechnique>(((MOJOSHADER_effect*)effectData)->technique_count);
		int num = 0;
		while (num < list2.Capacity)
		{
			MOJOSHADER_effectPass* passes = (MOJOSHADER_effectPass*)ptr->passes;
			EffectPassCollection passes2;
			if (ptr->pass_count == 1)
			{
				passes2 = new EffectPassCollection(INTERNAL_readPass(ref *passes, (nint)ptr, 0u));
			}
			else
			{
				List<EffectPass> list3 = new List<EffectPass>((int)ptr->pass_count);
				for (int l = 0; l < list3.Capacity; l++)
				{
					list3.Add(INTERNAL_readPass(ref passes[l], (nint)ptr, (uint)l));
				}
				passes2 = new EffectPassCollection(list3);
			}
			list2.Add(new EffectTechnique(MarshalHelper.PtrToInternedStringAnsi(ptr->name), (nint)ptr, passes2, INTERNAL_readAnnotations(ptr->annotations, ptr->annotation_count)));
			num++;
			ptr++;
		}
		Techniques = new EffectTechniqueCollection(list2);
	}

	internal unsafe static EffectParameterCollection INTERNAL_readEffectParameterStructureMembers(EffectParameter parameter, nint _type, Effect outer)
	{
		if (_type == IntPtr.Zero)
		{
			return new EffectParameterCollection(new List<EffectParameter>(0));
		}
		MOJOSHADER_symbolTypeInfo mOJOSHADER_symbolTypeInfo = *(MOJOSHADER_symbolTypeInfo*)_type;
		EffectParameterCollection effectParameterCollection = null;
		List<EffectParameter> list = new List<EffectParameter>();
		MOJOSHADER_symbolStructMember* members = (MOJOSHADER_symbolStructMember*)mOJOSHADER_symbolTypeInfo.members;
		nint num = IntPtr.Zero;
		for (int i = 0; i < mOJOSHADER_symbolTypeInfo.member_count; i++)
		{
			uint num2 = members[i].info.rows * members[i].info.columns;
			if (members[i].info.elements != 0)
			{
				num2 *= members[i].info.elements;
			}
			EffectParameter effectParameter = new EffectParameter(MarshalHelper.PtrToInternedStringAnsi(members[i].name), null, (int)members[i].info.rows, (int)members[i].info.columns, (int)members[i].info.elements, XNAClass[(int)members[i].info.parameter_class], XNAType[(int)members[i].info.parameter_type], null, null, parameter.values + ((IntPtr)num).ToInt32(), num2 * 4, outer);
			if (members[i].info.parameter_type == MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_STRING)
			{
				int* ptr = (int*)(parameter.values + ((IntPtr)num).ToInt32());
				effectParameter.cachedString = outer.INTERNAL_GetStringFromObjectTable(*ptr);
			}
			list.Add(effectParameter);
			num += (int)(num2 * 4);
		}
		return new EffectParameterCollection(list);
	}

	private unsafe string INTERNAL_GetStringFromObjectTable(int index)
	{
		MOJOSHADER_effect* ptr = (MOJOSHADER_effect*)effectData;
		MOJOSHADER_effectObject* objects = (MOJOSHADER_effectObject*)ptr->objects;
		if (index < ptr->object_count)
		{
			return Marshal.PtrToStringAnsi(objects[index].stringvalue.stringvalue);
		}
		throw new InvalidOperationException("Invalid effect object index");
	}

	private EffectPass INTERNAL_readPass(ref MOJOSHADER_effectPass pass, nint techPtr, uint index)
	{
		return new EffectPass(MarshalHelper.PtrToInternedStringAnsi(pass.name), INTERNAL_readAnnotations(pass.annotations, pass.annotation_count), this, techPtr, index);
	}

	private unsafe EffectAnnotationCollection INTERNAL_readAnnotations(nint rawAnnotations, uint numAnnotations)
	{
		if (numAnnotations == 0)
		{
			return EffectAnnotationCollection.Empty;
		}
		List<EffectAnnotation> list = new List<EffectAnnotation>((int)numAnnotations);
		for (int i = 0; i < numAnnotations; i++)
		{
			MOJOSHADER_effectAnnotation mOJOSHADER_effectAnnotation = *(MOJOSHADER_effectAnnotation*)(rawAnnotations + (nint)i * (nint)sizeof(MOJOSHADER_effectAnnotation));
			EffectAnnotation effectAnnotation = new EffectAnnotation(MarshalHelper.PtrToInternedStringAnsi(mOJOSHADER_effectAnnotation.name), MarshalHelper.PtrToInternedStringAnsi(mOJOSHADER_effectAnnotation.semantic), (int)mOJOSHADER_effectAnnotation.type.rows, (int)mOJOSHADER_effectAnnotation.type.columns, XNAClass[(int)mOJOSHADER_effectAnnotation.type.parameter_class], XNAType[(int)mOJOSHADER_effectAnnotation.type.parameter_type], mOJOSHADER_effectAnnotation.values);
			if (mOJOSHADER_effectAnnotation.type.parameter_type == MOJOSHADER_symbolType.MOJOSHADER_SYMTYPE_STRING)
			{
				int* values = (int*)mOJOSHADER_effectAnnotation.values;
				effectAnnotation.cachedString = INTERNAL_GetStringFromObjectTable(*values);
			}
			list.Add(effectAnnotation);
		}
		return new EffectAnnotationCollection(list);
	}
}
