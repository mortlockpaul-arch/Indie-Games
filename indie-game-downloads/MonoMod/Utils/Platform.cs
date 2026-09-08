using System;

namespace MonoMod.Utils;

[Flags]
[MonoMod__OldName__("MonoMod.Helpers.Platform")]
public enum Platform
{
	OS = 1,
	X86 = 0,
	X64 = 2,
	NT = 4,
	Unix = 8,
	Unknown = 0x11,
	Windows = 0x25,
	MacOS = 0x49,
	Linux = 0x89,
	Android = 0x189,
	iOS = 0x249,
	Unknown64 = Unknown | X64,
	Windows64 = Windows | X64,
	MacOS64 = MacOS | X64,
	Linux64 = Linux | X64,
	Android64 = Android | X64,
	iOS64 = iOS | X64
}
