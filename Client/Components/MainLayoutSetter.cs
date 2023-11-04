using BlazorApp.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components;

public class MainLayoutSetter : ComponentBase
{
	protected override bool ShouldRender() => false;

	[CascadingParameter(Name = "Layout")]
	public MainLayout Layout { get; set; } = null!;

	[Parameter]
	public RenderFragment? DisplayHero { get; set; }

	[Parameter]
	public List<LinkDetails>? Links { get; set; }

	[Parameter]
	public bool EndBGLighter { get; set; } = true;

	protected override void OnInitialized()
	{
		Layout.SetHero(DisplayHero);
		Layout.SetLinks(Links);
		Layout.SetEndBGLighter(EndBGLighter);
		base.OnInitialized();
	}

	protected override void OnParametersSet()
	{
		Layout.SetEndBGLighter(EndBGLighter);
		base.OnParametersSet();
	}
}
