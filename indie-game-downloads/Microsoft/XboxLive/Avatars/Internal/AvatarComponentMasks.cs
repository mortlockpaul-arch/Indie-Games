using System;

namespace Microsoft.XboxLive.Avatars.Internal;

[Flags]
public enum AvatarComponentMasks : short
{
	None = 0,
	Head = 1,
	Body = 2,
	Hair = 4,
	Shirt = 8,
	Trousers = 0x10,
	Shoes = 0x20,
	Hat = 0x40,
	Gloves = 0x80,
	Glasses = 0x100,
	Wristwear = 0x200,
	Earrings = 0x400,
	Ring = 0x800,
	Carryable = 0x1000,
	All = Head | Body | Hair | Shirt | Trousers | Shoes | Hat | Gloves | Glasses | Wristwear | Earrings | Ring | Carryable
}
