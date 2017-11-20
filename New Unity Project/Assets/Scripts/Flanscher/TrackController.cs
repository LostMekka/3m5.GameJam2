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

    public int _depth = 5;

    static Color[] cols = new Color[] { Color.gray, Color.red, Color.magenta, Color.blue, Color.gray, Color.yellow, Color.white, Color.cyan };


    private Flanschable CreateTrack(Flanschable prefab, EndFlanschPoint previousTrackElement = null)
	{
		Flanschable nextElement = Instantiate(prefab);
		nextElement.InitializeTrackElement("Track " + counter++);

		if (previousTrackElement != null)
		{
			EndFlanschPoint source = previousTrackElement;
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

	private Flanschable FlanschRandom(EndFlanschPoint target)
	{
		
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
		created.SetColorOfAllMeshRenderers(cols[prefabIndex % cols.Length]);
		return created;
	}

	private void AutoFlanschAll(Flanschable target, int depth)
	{
		foreach (EndFlanschPoint fp in target.EndFlanschPoints)
		{
			Flanschable next = fp.ConnectedPoint == null
				? FlanschRandom(fp)
				: fp.ConnectedPoint.ParentFlanschable;
			if (depth > 0) AutoFlanschAll(next, depth - 1);
		}
	}

	private void GarbageCollectBackwards(Flanschable flanschable, int ignoreDepth = 2)
	{
		EndFlanschPoint endFlanschPoint = flanschable.BeginFlanschPoints[0].ConnectedPoint;
		Flanschable previous = endFlanschPoint == null ? null : endFlanschPoint.ParentFlanschable;
		if (previous != null)
		{
			GarbageCollectBackwards(previous, ignoreDepth - 1);
			foreach (EndFlanschPoint efp in previous.EndFlanschPoints)
			{
				if (endFlanschPoint != efp && efp.ConnectedPoint != null) GarbageCollectForward(efp.ConnectedPoint.ParentFlanschable);
			}
		}
		if (ignoreDepth <= 0) Destroy(flanschable.gameObject);
	}

	private void GarbageCollectForward(Flanschable flanschable)
	{
		foreach (EndFlanschPoint endPoint in flanschable.EndFlanschPoints)
		{
			if (endPoint.ConnectedPoint != null)
			{
				GarbageCollectForward(endPoint.ConnectedPoint.ParentFlanschable);
			}
		}
		Destroy(flanschable.gameObject);
	}
	
	// Use this for initialization
	void Start()
	{
		currentTrackElement = CreateTrack(StartingTrackPrefab);
		AutoFlanschAll(currentTrackElement, _depth);

		MovementPlaneInstance = Instantiate(MovementPlanePrefab);
		MovementPlaneInstance.transform.position = currentTrackElement.BeginFlanschPoints[0].transform.position;
		MovementPlaneInstance.transform.rotation = currentTrackElement.BeginFlanschPoints[0].transform.rotation;

		MovementPlaneInstance.StartMovementPath(currentTrackElement, NextTrackElement);
	}

	private void NextTrackElement(EndFlanschPoint endFlanschPoint)
	{
		currentTrackElement.SetColorOfAllMeshRenderers(Color.green);
		currentTrackElement = endFlanschPoint.ConnectedPoint.ParentFlanschable;
		MovementPlaneInstance.StartMovementPath(currentTrackElement, NextTrackElement);
		AutoFlanschAll(currentTrackElement, 5);
		GarbageCollectBackwards(currentTrackElement);
	}
}