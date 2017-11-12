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

			// rotate to fit flansch points
			Vector3 angleDiff = previousPos.eulerAngles - nextPos.eulerAngles;
			nextElement.transform.eulerAngles = angleDiff;

			// translate to fit flansch points
			Vector3 posDiff = previousPos.position - nextPos.position;
			nextElement.transform.position = nextElement.transform.position + posDiff;

			// rotate randomly around flansch point axis
			nextElement.transform.RotateAround(previousPos.position, nextPos.forward, Random.Range(0f, 360f));

			source.ConnectedPoint = target;
			target.ConnectedPoint = source;
		}
		return nextElement;
	}

	private Flanschable FlanschRandom(Flanschable target)
	{
		var cols = new[] {Color.gray, Color.red, Color.magenta,};

		float sum = TrackPrefabs.Sum(prefab => prefab.FlanschProbability);
		float ran = Random.Range(0, sum);
		int prefabIndex = 0;
		foreach (Flanschable prefab in TrackPrefabs)
		{
			ran -= prefab.FlanschProbability;
			if (ran <= 0) break;
			prefabIndex++;
		}
		Flanschable created = CreateTrack(TrackPrefabs[prefabIndex], target);
		created.SetColorOfAllMeshRenderers(cols[prefabIndex]);
		return created;
	}
	
	// Use this for initialization
	void Start()
	{
		currentTrackElement = CreateTrack(StartingTrackPrefab);
		Flanschable head = currentTrackElement;
		for (int i = 0; i < 40; i++)
		{
			head = FlanschRandom(head);
		}

		MovementPlaneInstance = Instantiate(MovementPlanePrefab);
		MovementPlaneInstance.transform.position = currentTrackElement.BeginFlanschPoints[0].transform.position;
		MovementPlaneInstance.transform.rotation = currentTrackElement.BeginFlanschPoints[0].transform.rotation;

		MovementPlaneInstance.StartMovementPath(currentTrackElement, NextTrackElement);
	}

	private void NextTrackElement()
	{
		currentTrackElement.SetColorOfAllMeshRenderers(Color.green);
		currentTrackElement = currentTrackElement.EndFlanschPoints[0].ConnectedPoint.ParentFlanschable;
		MovementPlaneInstance.StartMovementPath(currentTrackElement, NextTrackElement);
	}
}