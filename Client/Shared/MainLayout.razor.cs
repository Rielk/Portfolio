using BlazorApp.Client.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace BlazorApp.Client.Shared;

public class MainLayoutSetter : ComponentBase
{
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

public class HashRoutingManager : ComponentBase, IDisposable
{
	[Inject] private NavigationManager NavManager { get; set; } = null!;
	[Inject] private IJSRuntime JSRuntime { get; set; } = null!;

	private IDisposable? ChangingRegistration;

	protected override void OnInitialized()
	{
		NavManager.LocationChanged += OnLocationChanged;
		ChangingRegistration = NavManager.RegisterLocationChangingHandler(OnLocationChanging);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			await NavigateToFragmentAsync();
	}

	private ValueTask OnLocationChanging(LocationChangingContext context)
	{
		Uri oldUri = NavManager.ToAbsoluteUri(NavManager.Uri);
		Uri newUri = NavManager.ToAbsoluteUri(context.TargetLocation);
		if (oldUri.AbsolutePath == newUri.AbsolutePath)
		{
			context.PreventNavigation();

			if (newUri.Fragment.Length == 1)
				return JSRuntime.InvokeVoidAsync("scrollToId", "page");
			else
				return JSRuntime.InvokeVoidAsync("scrollToId", newUri.Fragment[1..]);
		}
		return ValueTask.CompletedTask;
	}

	private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
	{
		await NavigateToFragmentAsync();
	}

	private ValueTask NavigateToFragmentAsync()
	{
		Uri uri = NavManager.ToAbsoluteUri(NavManager.Uri);

		if (uri.Fragment.Length == 0)
			return default;

		return JSRuntime.InvokeVoidAsync("scrollToId", uri.Fragment[1..]);
	}

	void IDisposable.Dispose()
	{
		NavManager.LocationChanged -= OnLocationChanged;
		ChangingRegistration?.Dispose();
		GC.SuppressFinalize(this);
	}
}
