using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace BlazorApp.Client.Components;

public class HashRoutingManager : ComponentBase, IDisposable
{
	protected override bool ShouldRender() => false;

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

			if (newUri.Fragment.Length <= 1)
				return JSRuntime.InvokeVoidAsync("scrollToId", "pageTop");
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

	private bool disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				NavManager.LocationChanged -= OnLocationChanged;
				ChangingRegistration?.Dispose();
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
