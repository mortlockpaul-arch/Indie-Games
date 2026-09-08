using System;

namespace Microsoft.XboxLive.Avatars.Internal;

[Flags]
public enum ComponentCategories
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
	Eyes = 0x2000,
	Eyebrows = 0x4000,
	Mouth = 0x8000,
	FacialHair = 0x10000,
	FacialOther = 0x20000,
	EyeShadow = 0x40000,
	Nose = 0x80000,
	Chin = 0x100000,
	Ears = 0x200000,
	Shape = 0x1000000,
	Animation = 0x400000,
	Costume = 0x800000,
	Models = Head | Body | Hair | Shirt | Trousers | Shoes | Hat | Gloves | Glasses | Wristwear | Earrings | Ring | Carryable,
	Textures = Eyes | Eyebrows | Mouth | FacialHair | FacialOther | EyeShadow,
	BlendShapes = Nose | Chin | Ears | Shape,
	Animations = Animation,
	Costumes = Costume,
	Clothing = Shirt | Trousers | Shoes | Hat | Gloves | Glasses | Wristwear | Earrings | Ring | Carryable | Costume,
	Valid = Models | Textures | BlendShapes | Animation | Costume
}
