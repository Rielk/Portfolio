using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApp.Client.Components;

public class ResizeWatcher : ComponentBase, IDisposable
{
	protected override bool ShouldRender() => false;

	public static event Func<Task>? OnResize;

	[Inject] private IJSRuntime JSRuntime { get; set; } = null!;

	[JSInvokable]
	public static async Task OnBrowserResize()
	{
		await (OnResize?.Invoke() ?? System.Threading.Tasks.Task.CompletedTask);
	}

	private static bool eventRegistered = false;
	protected override async Task OnInitializedAsync()
	{
		if (!eventRegistered)
		{
			eventRegistered = true;
			await JSRuntime.InvokeAsync<object>("browserResize.registerResizeCallback");
		}
		await base.OnInitializedAsync();
	}

	private Func<Task>? OldTask;
	[Parameter]
	public Func<Task>? Task { get; set; }
	protected override void OnParametersSet()
	{
		if (OldTask != Task)
		{
			OnResize -= OldTask;
			OnResize += Task;
			OldTask = Task;
		}
		base.OnParametersSet();
	}

	private bool disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				OnResize -= Task;
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
