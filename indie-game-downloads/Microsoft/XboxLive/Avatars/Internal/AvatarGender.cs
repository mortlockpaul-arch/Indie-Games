using System;

namespace Microsoft.XboxLive.Avatars.Internal;

[Flags]
public enum AvatarGender
{
	Unknown = 0,
	Male = 1,
	Female = 2,
	Both = Male | Female
}
