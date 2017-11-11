﻿using UnityEngine;

public class BeginFlanschPoint : MonoBehaviour, IFlanschPoint
{
	public EndFlanschPoint ConnectedPoint;
	public Flanschable ParentFlanschable { get; set; }

	public void Disconnect()
	{
		if (ConnectedPoint == null) return;
		ConnectedPoint.ConnectedPoint = null;
		ConnectedPoint = null;
	}
}