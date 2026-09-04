using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public abstract class CParam
{
	public const short PARAM_EXPRESSION = 22;

	public short code;

	public CParam()
	{
	}

	public static CParam create(CRunApp app)
	{
		long num = app.file.getFilePointer();
		CParam cParam = null;
		short num2 = app.file.readAShort();
		short num3 = app.file.readAShort();
		switch (num3)
		{
		case 1:
			cParam = new PARAM_OBJECT();
			break;
		case 2:
			cParam = new PARAM_TIME();
			break;
		case 3:
			cParam = new PARAM_SHORT();
			break;
		case 4:
			cParam = new PARAM_SHORT();
			break;
		case 5:
			cParam = new PARAM_INT();
			break;
		case 6:
			cParam = new PARAM_SAMPLE();
			break;
		case 7:
			cParam = new PARAM_SAMPLE();
			break;
		case 9:
			cParam = new PARAM_CREATE();
			break;
		case 10:
			cParam = new PARAM_SHORT();
			break;
		case 11:
			cParam = new PARAM_SHORT();
			break;
		case 12:
			cParam = new PARAM_SHORT();
			break;
		case 13:
			cParam = new PARAM_EVERY();
			break;
		case 14:
			cParam = new PARAM_KEY();
			break;
		case 15:
			cParam = new PARAM_EXPRESSION();
			break;
		case 16:
			cParam = new PARAM_POSITION();
			break;
		case 17:
			cParam = new PARAM_SHORT();
			break;
		case 18:
			cParam = new PARAM_SHOOT();
			break;
		case 19:
			cParam = new PARAM_ZONE();
			break;
		case 21:
			cParam = new PARAM_CREATE();
			break;
		case 22:
			cParam = new PARAM_EXPRESSION();
			break;
		case 23:
			cParam = new PARAM_EXPRESSION();
			break;
		case 24:
			cParam = new PARAM_COLOUR();
			break;
		case 25:
			cParam = new PARAM_INT();
			break;
		case 26:
			cParam = new PARAM_SHORT();
			break;
		case 27:
			cParam = new PARAM_EXPRESSION();
			break;
		case 28:
			cParam = new PARAM_EXPRESSION();
			break;
		case 29:
			cParam = new PARAM_INT();
			break;
		case 31:
			cParam = new PARAM_SHORT();
			break;
		case 32:
			cParam = new PARAM_SHORT();
			break;
		case 33:
			cParam = new PARAM_PROGRAM();
			break;
		case 34:
			cParam = new PARAM_INT();
			break;
		case 35:
			cParam = new PARAM_SAMPLE();
			break;
		case 36:
			cParam = new PARAM_SAMPLE();
			break;
		case 37:
			cParam = new PARAM_SHORT();
			break;
		case 38:
			cParam = new PARAM_GROUP();
			break;
		case 39:
			cParam = new PARAM_GROUPOINTER();
			break;
		case 40:
			cParam = new PARAM_STRING();
			break;
		case 41:
			cParam = new PARAM_STRING();
			break;
		case 42:
			cParam = new PARAM_CMPTIME();
			break;
		case 43:
			cParam = new PARAM_SHORT();
			break;
		case 44:
			cParam = new PARAM_KEY();
			break;
		case 45:
			cParam = new PARAM_EXPRESSION();
			break;
		case 46:
			cParam = new PARAM_EXPRESSION();
			break;
		case 47:
			cParam = new PARAM_2SHORTS();
			break;
		case 48:
			cParam = new PARAM_INT();
			break;
		case 49:
			cParam = new PARAM_SHORT();
			break;
		case 50:
			cParam = new PARAM_SHORT();
			break;
		case 51:
			cParam = new PARAM_2SHORTS();
			break;
		case 52:
			cParam = new PARAM_EXPRESSION();
			break;
		case 53:
			cParam = new PARAM_EXPRESSION();
			break;
		case 54:
			cParam = new PARAM_EXPRESSION();
			break;
		case 55:
			cParam = new PARAM_EXTENSION();
			break;
		case 56:
			cParam = new PARAM_INT();
			break;
		case 57:
			cParam = new PARAM_SHORT();
			break;
		case 58:
			cParam = new PARAM_SHORT();
			break;
		case 59:
			cParam = new PARAM_EXPRESSION();
			break;
		case 60:
			cParam = new PARAM_SHORT();
			break;
		case 61:
			cParam = new PARAM_SHORT();
			break;
		case 62:
			cParam = new PARAM_EXPRESSION();
			break;
		case 63:
			cParam = new PARAM_STRING();
			break;
		case 64:
			cParam = new PARAM_EFFECT();
			break;
		}
		cParam.code = num3;
		cParam.load(app);
		app.file.seek((int)(num + num2));
		return cParam;
	}

	public abstract void load(CRunApp app);
}
