using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackController : MonoBehaviour
{
	public List<Flanschable> TrackPrefabs = new List<Flanschable>();
	public Flanschable StartingTrackPrefab;
	public MovementPlane MovementPlanePrefab;

	public MovementPlane MovementPlaneInstance { get; set; }

	private Flanschable currentTrackElement;
	private static int counter;

	private Flanschable CreateTrack(Flanschable prefab, Flanschable previousTrackElement = null)
	{
		Flanschable nextElement = Instantiate(prefab);
		nextElement.InitializeTrackElement("Track " + counter++);

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
		for (int i = 0; i < 5; i++)
		{
			head = CreateTrack(TrackPrefabs[0], head);
			head.GetComponentInChildren<MeshRenderer>().material.color = Color.gray;
		}

		MovementPlaneInstance = Instantiate(MovementPlanePrefab);
		MovementPlaneInstance.transform.position = currentTrackElement.BeginFlanschPoints[0].transform.position;
		MovementPlaneInstance.transform.rotation = currentTrackElement.BeginFlanschPoints[0].transform.rotation;

		MovementPlaneInstance.StartMovementPath(currentTrackElement, NextTrackElement);
	}

	private void NextTrackElement()
	{
		currentTrackElement.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(mr => mr.material.color = Color.green);
		currentTrackElement = currentTrackElement.EndFlanschPoints[0].ConnectedPoint.ParentFlanschable;
		MovementPlaneInstance.StartMovementPath(currentTrackElement, NextTrackElement);
	}
}