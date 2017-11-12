using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementPlane : MonoBehaviour
{
	public float MoveSpeed = 15;
	public float TurnSpeed = 20;
	public float VehicleAngularPosition;

	public SpringJoint OuterJoint, InnerJoint;

	private Action onPathReachedCallback;
	private string currPathName;
	private Waypoint currentSourceWaypoint;

	public void StartMovementPath(Flanschable trackElement, Action callback)
	{
		currPathName = trackElement.name;
		onPathReachedCallback = callback;
		currentSourceWaypoint = trackElement.BeginFlanschPoints[0];
		MoveToNextWaypoint();
	}

	public void MoveToNextWaypoint()
	{
		if (currentSourceWaypoint.Targets.Count == 0)
		{
			if (onPathReachedCallback != null)
			{
				Action tmp = onPathReachedCallback;
				onPathReachedCallback = null;
				tmp();
			}
		}
		else
		{
			Vector3 vehiclePos = GetComponentInChildren<Vehicle>().transform.position;
			Waypoint nextWaypoint = currentSourceWaypoint.Targets
				.Select(wp => new KeyValuePair<Waypoint, float>(wp, (wp.transform.position - vehiclePos).magnitude))
				.Aggregate((wp1, wp2) => wp1.Value > wp2.Value ? wp2 : wp1)
				.Key;
			currentSourceWaypoint = nextWaypoint;
			Vector3 targetPos = nextWaypoint.gameObject.transform.position;
			
			iTween.MoveTo(gameObject, new Hashtable
			{
				{"name", currPathName},
				{"path", new[] {gameObject.transform.position, targetPos}},
				{"speed", MoveSpeed},
				{"easetype", iTween.EaseType.linear},
				{"oncomplete", "MoveToNextWaypoint"},
				{"oncompletetarget", gameObject},
				{"onupdate", "OnItweenUpdate"},
				{"onupdatetarget", gameObject},
			});
			iTween.LookTo(gameObject, new Hashtable
			{
				{"looktarget", targetPos},
				{"time", (gameObject.transform.position - targetPos).magnitude / MoveSpeed * 2.5f},
				{"easetype", iTween.EaseType.easeOutSine},
			});
		}
	}

	private void Start()
	{
		iTween.Init(gameObject);
		this.SetColorOfAllMeshRenderers(Color.blue);
	}

	private void Update()
	{
		VehicleAngularPosition -= Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
	}

	private void LateUpdate()
	{
		CorrectVehiclePosition();
	}

	private void CorrectVehiclePosition()
	{
		Vector3 angles = transform.eulerAngles;
		angles.z = VehicleAngularPosition;
		transform.eulerAngles = angles;
	}
}