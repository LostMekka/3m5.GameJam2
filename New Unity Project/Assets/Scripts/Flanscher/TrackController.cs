using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class TrackController : MonoBehaviour
{
	public List<Flanschable> TrackPrefabs = new List<Flanschable>();
	public Flanschable StartingTrackPrefab;
	public MovementPlane MovementPlanePrefab;

	public MovementPlane MovementPlaneInstance { get; set; }

	private Flanschable currentTrackElement = null;
	private static int counter = 0;

	private Flanschable CreateTrack(Flanschable prefab, Flanschable previousTrackElement = null)
	{
		Flanschable nextElement = Instantiate(prefab);
		Debug.Log("instantiated");
		nextElement.InitializeFlanschPoints();
		nextElement.gameObject.name = "Track " + counter++;
		if (previousTrackElement != null)
		{
			EndFlanschPoint source = previousTrackElement.EndFlanschPoints[0];
			BeginFlanschPoint target = nextElement.BeginFlanschPoints[0];

			Transform previousPos = source.transform;
			Transform nextPos = target.transform;

			Vector3 angleDiff = previousPos.eulerAngles - nextPos.eulerAngles;
			nextElement.transform.Rotate(angleDiff);

			Vector3 posDiff = previousPos.position - nextPos.position;
			nextElement.transform.position = nextElement.transform.position + posDiff;

			source.ConnectedPoint = target;
			target.ConnectedPoint = source;
		}
		return nextElement;
	}

	// Use this for initialization
	void Start()
	{
		currentTrackElement = CreateTrack(StartingTrackPrefab);
		Flanschable head = currentTrackElement;
		for (int i = 0; i < 5; i++) head = CreateTrack(TrackPrefabs[0], head);

		MovementPlaneInstance = Instantiate(MovementPlanePrefab);
		MovementPlaneInstance.transform.position = currentTrackElement.BeginFlanschPoints[0].transform.position;
		MovementPlaneInstance.transform.rotation = currentTrackElement.BeginFlanschPoints[0].transform.rotation;
	}

	// Update is called once per frame
	void Update()
	{
	}
}