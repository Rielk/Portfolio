using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Shared;

public class MainLayoutSetter : ComponentBase
{
	[CascadingParameter(Name = "Layout")]
	public MainLayout Layout { get; set; } = null!;

	[Parameter, EditorRequired]
	public RenderFragment? Hero { get; set; }

	protected override void OnInitialized()
	{
		Layout.SetHero(Hero);
		base.OnInitialized();
	}
}
