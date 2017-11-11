using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFlanschPoint : MonoBehaviour,IFlanschPoint
{
	public BeginFlanschPoint ConnectedPoint;
	public Flanschable ParentFlanschable { get; set; }

	public void Disconnect()
	{
		if (ConnectedPoint == null) return;
		ConnectedPoint.ConnectedPoint = null;
		ConnectedPoint = null;
	}
}