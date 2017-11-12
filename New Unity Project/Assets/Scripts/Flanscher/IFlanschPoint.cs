public interface IFlanschPoint
{
	Flanschable ParentFlanschable { get; set; }
	void Disconnect();
}