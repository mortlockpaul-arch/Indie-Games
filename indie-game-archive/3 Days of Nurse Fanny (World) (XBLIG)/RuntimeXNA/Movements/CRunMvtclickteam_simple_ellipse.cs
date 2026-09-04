using System;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

internal class CRunMvtclickteam_simple_ellipse : CRunMvtExtension
{
	private const int MFLAG1_MOVEATSTART = 1;

	private int m_dwCX;

	private int m_dwCY;

	private int m_dwRadiusX;

	private int m_dwRadiusY;

	private int m_dwStartAngle;

	private int m_dwFlags;

	private int m_dwAngVel;

	private int m_dwOffset;

	private bool r_Stopped;

	private int r_CX;

	private int r_CY;

	private int r_radiusX;

	private int r_radiusY;

	private double r_AngVel;

	private double r_Offset;

	private double r_CurrentAngle;

	public override void initialize(CFile file)
	{
		file.skipBytes(1);
		m_dwCX = file.readAInt();
		m_dwCY = file.readAInt();
		m_dwRadiusX = file.readAInt();
		m_dwRadiusY = file.readAInt();
		m_dwStartAngle = file.readAInt();
		m_dwFlags = file.readAInt();
		m_dwAngVel = file.readAInt();
		m_dwOffset = file.readAInt();
		r_Stopped = (m_dwFlags & 1) == 0;
		r_CX = m_dwCX;
		r_CY = m_dwCY;
		r_AngVel = (double)m_dwAngVel / 50.0 * (Math.PI / 180.0);
		r_Offset = (double)m_dwOffset * (Math.PI / 180.0);
		r_CurrentAngle = (double)m_dwStartAngle * (Math.PI / 180.0);
		r_radiusX = m_dwRadiusX;
		r_radiusY = m_dwRadiusY;
		ho.roc.rcSpeed = m_dwAngVel;
	}

	public override bool move()
	{
		if (!r_Stopped)
		{
			double num = (double)r_radiusX * Math.Cos(r_CurrentAngle);
			double num2 = (double)r_radiusY * Math.Sin(r_CurrentAngle);
			if (Math.Abs(r_Offset) > 0.0001)
			{
				double num3 = Math.Cos(r_Offset) * num - num2 * Math.Sin(r_Offset);
				double num4 = Math.Sin(r_Offset) * num + num2 * Math.Cos(r_Offset);
				num = num3;
				num2 = num4;
			}
			double num5 = r_AngVel;
			if ((ho.hoAdRunHeader.rhFrame.leFlags & 0x8000) != 0)
			{
				num5 *= ho.hoAdRunHeader.rh4MvtTimerCoef;
			}
			r_CurrentAngle += num5;
			if (r_CurrentAngle < 0.0)
			{
				r_CurrentAngle += Math.PI * 2.0;
			}
			else if (r_CurrentAngle > Math.PI * 2.0)
			{
				r_CurrentAngle -= Math.PI * 2.0;
			}
			animations(1);
			ho.hoX = (int)((double)r_CX + num);
			ho.hoY = (int)((double)r_CY - num2);
			collisions();
			return true;
		}
		animations(0);
		collisions();
		return ho.roc.rcChanged;
	}

	private void reset()
	{
		r_CX = m_dwCX;
		r_CY = m_dwCY;
		r_AngVel = (double)m_dwAngVel / 50.0 * (Math.PI / 180.0);
		r_Offset = (double)m_dwOffset * (Math.PI / 180.0);
		r_CurrentAngle = (double)m_dwStartAngle * (Math.PI / 180.0);
		r_radiusX = m_dwRadiusX;
		r_radiusY = m_dwRadiusY;
	}

	public override void setPosition(int x, int y)
	{
		r_CX -= ho.hoX - x;
		r_CY -= ho.hoY - y;
		ho.hoX = x;
		ho.hoY = y;
	}

	public override void setXPosition(int x)
	{
		r_CX -= ho.hoX - x;
		ho.hoX = x;
	}

	public override void setYPosition(int y)
	{
		r_CY -= ho.hoY - y;
		ho.hoY = y;
	}

	public override void stop(bool bCurrent)
	{
		r_Stopped = true;
	}

	public override void reverse()
	{
		r_AngVel *= -1.0;
	}

	public override void start()
	{
		r_Stopped = false;
	}

	public override void setSpeed(int speed)
	{
		r_AngVel = (double)speed / 50.0 * (Math.PI / 180.0);
		ho.roc.rcSpeed = speed;
	}

	public override double actionEntry(int action)
	{
		switch (action)
		{
		case 3645:
		{
			int num = (int)getParamDouble();
			r_CX = num;
			break;
		}
		case 3646:
		{
			int num = (int)getParamDouble();
			r_CY = num;
			break;
		}
		case 3647:
		{
			int num = (int)getParamDouble();
			r_radiusX = num;
			break;
		}
		case 3648:
		{
			int num = (int)getParamDouble();
			r_radiusY = num;
			break;
		}
		case 3649:
		{
			int num = (int)getParamDouble();
			r_AngVel = (double)num / 50.0 * (Math.PI / 180.0);
			ho.roc.rcSpeed = num;
			break;
		}
		case 3650:
		{
			int num = (int)getParamDouble();
			r_CurrentAngle = (double)num * (Math.PI / 180.0);
			break;
		}
		case 3651:
		{
			int num = (int)getParamDouble();
			r_Offset = (double)num * (Math.PI / 180.0);
			break;
		}
		case 3652:
			return r_CX;
		case 3653:
			return r_CY;
		case 3654:
			return r_radiusX;
		case 3655:
			return r_radiusY;
		case 3656:
			return r_AngVel * 50.0 * (180.0 / Math.PI);
		case 3657:
			return r_CurrentAngle * (180.0 / Math.PI);
		case 3658:
			return r_Offset * (180.0 / Math.PI);
		}
		return 0.0;
	}

	public override int getSpeed()
	{
		return ho.roc.rcSpeed;
	}
}
