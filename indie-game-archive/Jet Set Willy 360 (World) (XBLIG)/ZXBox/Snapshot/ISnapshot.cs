using Zilog;

namespace ZXBox.Snapshot;

public interface ISnapshot
{
	void LoadSnapshot(byte[] snapshotbytes, Z80 cpu);

	byte[] SaveSnapshot(Z80 cpu);
}
