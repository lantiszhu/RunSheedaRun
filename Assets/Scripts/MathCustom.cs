using UnityEngine;
using System.Collections;

public class MathCustom
{
	public static float LerpLinear(float start, float end, float factor)
	{
		if (Mathf.Abs(start-end) <= factor)
			return end;
		else if ( start-end < 0 )
			start += factor;
		else if (start-end > 0)
			start -= factor;
		
		return start;
	}
	
	public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
	
	public static float VectorDistanceXYZ(Vector3 a, Vector3 b)
	{
		return Mathf.Sqrt( Mathf.Pow((b.x - a.x),2) + Mathf.Pow((b.y - a.y),2) + Mathf.Pow((b.z - a.z),2) );
	}
	
	public static float VectorDistanceXZ(Vector3 a, Vector3 b)
	{
		return Mathf.Sqrt( Mathf.Pow((b.x - a.x),2) + Mathf.Pow((b.z - a.z),2) );
	}
}
