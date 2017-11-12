using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementPlane : MonoBehaviour
{
	public float Speed = 15;

	private Action onPathReachedCallback;
	private List<Vector3> waypoints;
	private string currPathName;

	public void StartMovementPath(Flanschable trackElement, Action callback)
	{
		currPathName = trackElement.name;
		onPathReachedCallback = callback;
		Waypoint p = trackElement.BeginFlanschPoints[0];
		waypoints = new List<Vector3>();
		while (p.Targets.Count > 0)
		{
			p = p.Targets[0];
			waypoints.Add(p.transform.position);
		}
		if (waypoints.Count == 1) waypoints.Add(waypoints[0]);
		MoveToNextWaypoint();
	}

	public void MoveToNextWaypoint()
	{
		if (waypoints.Count > 1)
		{
			Vector3 next = waypoints[0];
			waypoints.RemoveAt(0);
			iTween.MoveTo(gameObject, new Hashtable
			{
				{"name", currPathName},
				{"path", new[] {gameObject.transform.position, next}},
				{"speed", Speed},
				{"easetype", iTween.EaseType.linear},
				{"oncomplete", "MoveToNextWaypoint"},
				{"oncompletetarget", gameObject},
				{"onupdate", "OnItweenUpdate"},
				{"onupdatetarget", gameObject},
				{"orienttopath", true},
			});
		}
		else
		{
			if (onPathReachedCallback != null)
			{
				Action tmp = onPathReachedCallback;
				onPathReachedCallback = null;
				tmp();
			}
		}
	}

	public void OnItweenUpdate()
	{
		Hashtable currTween = iTween.tweens.ToList().Find(ht => ht["name"] is string && (string) ht["name"] == currPathName);
		string str = currTween.Keys.Cast<string>().Aggregate("", (current, key) => current + key + ": " + currTween[key] + ", ");
		Debug.Log("update with param: " + str);
//		Vector3 lt = (Vector3) currTween["looktarget"];
		Debug.Log(gameObject.transform.eulerAngles);
	}

	private void Start()
	{
		iTween.Init(gameObject);
		GetComponentsInChildren<MeshRenderer>().ToList().ForEach(mr => mr.material.color = Color.blue);
	}
}