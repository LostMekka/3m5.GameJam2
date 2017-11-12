using System.Linq;
using UnityEngine;

public static class Util
{
	public static void SetColorOfAllMeshRenderers(this GameObject o, Color color)
	{
		o.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(mr => mr.material.color = color);
	} 
	public static void SetColorOfAllMeshRenderers(this MonoBehaviour o, Color color)
	{
		o.gameObject.SetColorOfAllMeshRenderers(color);
	} 
}