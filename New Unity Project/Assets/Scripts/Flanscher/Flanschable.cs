using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flanschable : MonoBehaviour
{
	private List<BeginFlanschPoint> beginFlanschPoints;
	private List<EndFlanschPoint> endFlanschPoints;

	public List<BeginFlanschPoint> BeginFlanschPoints
	{
		get { return beginFlanschPoints; }
	}

	public List<EndFlanschPoint> EndFlanschPoints
	{
		get { return endFlanschPoints; }
	}

	public void InitializeFlanschPoints()
	{
		beginFlanschPoints = GetComponentsInChildren<BeginFlanschPoint>().ToList();
		foreach (var point in beginFlanschPoints) point.ParentFlanschable = this;
		endFlanschPoints = GetComponentsInChildren<EndFlanschPoint>().ToList();
		Debug.Log("test");
		foreach (var point in endFlanschPoints) point.ParentFlanschable = this;
	}
}