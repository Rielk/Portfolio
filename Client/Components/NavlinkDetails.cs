public class NavLinkDetails
{
	public NavLinkDetails(string href, string content, bool matchAll = false)
	{
		HRef = href;
		Content = content;
		MatchAll = matchAll;
	}

	public string HRef { get; set; }
	public string Content { get; set; }
	public bool MatchAll { get; set; }
}