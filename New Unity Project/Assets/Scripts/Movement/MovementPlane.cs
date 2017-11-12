using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlane : MonoBehaviour
{
	public float Speed = 15;

	private Action onPathReachedCallback;
	private List<Vector3> waypoints;

	public void StartMovementPath(Flanschable trackElement, Action callback)
	{
		onPathReachedCallback = callback;
		Waypoint p = trackElement.BeginFlanschPoints[0];
		waypoints = new List<Vector3>{p.transform.position};
		while (p.Targets.Count > 0)
		{
			p = p.Targets[0];
			waypoints.Add(p.transform.position);
		}
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
				{"path", new[] {next, waypoints[0]}},
				{"speed", Speed},
				{"easetype", iTween.EaseType.linear},
				{"oncomplete", "MoveToNextWaypoint"},
				{"oncompletetarget", gameObject},
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

	private void Start()
	{
		iTween.Init(gameObject);
	}
}