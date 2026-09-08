namespace System.Text;

[Flags]
internal enum TrimType
{
	Head = 1,
	Tail = 2,
	Both = Head | Tail
}
