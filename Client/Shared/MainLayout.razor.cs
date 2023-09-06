using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Shared;

public class MainLayoutSetter : ComponentBase
{
    [CascadingParameter(Name = "Layout")]
    public MainLayout Layout { get; set; } = null!;

    [Parameter]
    public RenderFragment? Hero { get; set; }

    [Parameter]
    public List<LinkDetails>? Links { get; set; }

    protected override void OnInitialized()
    {
        Layout.SetHero(Hero);
        Layout.SetLinks(Links);
        base.OnInitialized();
    }
}
