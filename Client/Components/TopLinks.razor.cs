public class LinkDetails
{
	private bool? external;

	public LinkDetails(string href, string content, bool? external = null)
	{
		HRef = href;
		Content = content;
		this.external = external;
	}

	public string HRef { get; set; }
	public string Content { get; set; }
	public bool External
	{
		get
		{
			return external ?? !HRef.StartsWith('.');
		}
		set
		{
			external = value;
		}
	}
}