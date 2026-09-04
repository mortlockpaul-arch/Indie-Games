using RuntimeXNA.Services;

namespace RuntimeXNA.Frame;

public class CLayer
{
	public const int FLOPT_XCOEF = 1;

	public const int FLOPT_YCOEF = 2;

	public const int FLOPT_NOSAVEBKD = 4;

	public const int FLOPT_VISIBLE = 16;

	public const int FLOPT_WRAP_HORZ = 32;

	public const int FLOPT_WRAP_VERT = 64;

	public const int FLOPT_REDRAW = 65536;

	public const int FLOPT_TOHIDE = 131072;

	public const int FLOPT_TOSHOW = 262144;

	public string pName;

	public int x;

	public int y;

	public int dx;

	public int dy;

	public CArrayList pBkd2;

	public CArrayList pLadders;

	public int nZOrderMax;

	public int dwOptions;

	public float xCoef;

	public float yCoef;

	public int nBkdLOs;

	public int nFirstLOIndex;

	public int backUp_dwOptions;

	public float backUp_xCoef;

	public float backUp_yCoef;

	public int backUp_nBkdLOs;

	public int backUp_nFirstLOIndex;

	public void load(CFile file)
	{
		dwOptions = file.readAInt();
		xCoef = file.readAFloat();
		yCoef = file.readAFloat();
		nBkdLOs = file.readAInt();
		nFirstLOIndex = file.readAInt();
		pName = file.readAString();
		backUp_dwOptions = dwOptions;
		backUp_xCoef = xCoef;
		backUp_yCoef = yCoef;
		backUp_nBkdLOs = nBkdLOs;
		backUp_nFirstLOIndex = nFirstLOIndex;
	}
}
