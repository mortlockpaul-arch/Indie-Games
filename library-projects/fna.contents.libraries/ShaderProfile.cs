// Microsoft.Xna.Framework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=6d5c3888ef60e27d
// Microsoft.Xna.Framework.Graphics.ShaderProfile
namespace Microsoft.Xna.Framework.Graphics
{

    /// <summary>Defines vertex and pixel shader versions. Reference page contains links to related code samples.</summary>
    public enum ShaderProfile
    {
        /// <summary>Pixel shader version ps_1_1.</summary>
        PS_1_1,
        /// <summary>Pixel shader version ps_1_2.</summary>
        PS_1_2,
        /// <summary>Pixel shader version ps_1_3.</summary>
        PS_1_3,
        /// <summary>Pixel shader version ps_1_4.</summary>
        PS_1_4,
        /// <summary>Pixel shader version ps_2_0.</summary>
        PS_2_0,
        /// <summary>Pixel shader version ps_2_a.</summary>
        PS_2_A,
        /// <summary>Pixel shader version ps_2_b.</summary>
        PS_2_B,
        /// <summary>Pixel software shader version ps_2_sw.</summary>
        PS_2_SW,
        /// <summary>Pixel shader version ps_3_0.</summary>
        PS_3_0,
        /// <summary>Xbox microcode assembly pixel shader version xps_3_0.  Microcode assembly language supports a superset of the ps_3_0 and vs_3_0 specifications defined by Direct3D 9.0 for Windows. It does not support earlier Direct3D vertex and pixel shader specifications.</summary>
        XPS_3_0,
        /// <summary>Vertex shader version v_1_1.</summary>
        VS_1_1,
        /// <summary>Vertex shader version v_2_0.</summary>
        VS_2_0,
        /// <summary>Vertex shader version v_2_a.</summary>
        VS_2_A,
        /// <summary>Vertex software shader version v_2_sw.</summary>
        VS_2_SW,
        /// <summary>Vertex shader version vs_3_0.</summary>
        VS_3_0,
        /// <summary>Xbox microcode assembly vertex shader version xvs_3_0.  Microcode assembly language supports a superset of the ps_3_0 and vs_3_0 specifications defined by Direct3D 9.0 for Windows. It does not support earlier Direct3D vertex and pixel shader specifications.</summary>
        XVS_3_0,
        /// <summary>Unknown pixel shader version.</summary>
        Unknown
    }
}