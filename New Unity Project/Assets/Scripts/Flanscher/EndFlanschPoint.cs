public class EndFlanschPoint : Waypoint, IFlanschPoint
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