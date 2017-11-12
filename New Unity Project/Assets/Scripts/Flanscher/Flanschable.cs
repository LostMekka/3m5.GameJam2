using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flanschable : MonoBehaviour
{
	public float FlanschProbability = 1f;
	
	public List<BeginFlanschPoint> BeginFlanschPoints {get; private set;}
	public List<EndFlanschPoint> EndFlanschPoints {get; private set;}

	public void InitializeTrackElement(string uniqueName)
	{
		BeginFlanschPoints = GetComponentsInChildren<BeginFlanschPoint>().ToList();
		foreach (var point in BeginFlanschPoints) point.ParentFlanschable = this;
		EndFlanschPoints = GetComponentsInChildren<EndFlanschPoint>().ToList();
		foreach (var point in EndFlanschPoints) point.ParentFlanschable = this;
	}
}