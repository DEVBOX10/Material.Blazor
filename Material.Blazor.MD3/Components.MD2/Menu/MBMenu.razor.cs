﻿using Material.Blazor.Internal.MD2;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Material.Blazor.MD2;

/// <summary>
/// This is a general purpose Material Theme menu.
/// </summary>
public partial class MBMenu : ComponentFoundationMD2
{
    /// <summary>
    /// A render fragement as a set of <see cref="MBListItem"/>s.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }


    /// <summary>
    /// Regular, fullwidth or fixed positioning/width.
    /// </summary>
    [Parameter] public MBMenuSurfacePositioning MenuSurfacePositioning { get; set; } = MBMenuSurfacePositioning.Regular;


    /// <summary>
    /// Called when the menu is closed.
    /// </summary>
    [Parameter] public Action OnMenuClosed { get; set; }


    private DotNetObjectReference<MBMenu> ObjectReference { get; set; }
    private ElementReference ElementReference { get; set; }
    private bool IsOpen { get; set; } = false;


    // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside Material.Blazor
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ConditionalCssClasses
            .AddIf(GetMenuSurfacePositioningClass(MenuSurfacePositioning), () => MenuSurfacePositioning != MBMenuSurfacePositioning.Regular);

        ObjectReference = DotNetObjectReference.Create(this);
    }


    private bool _disposed = false;
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            ObjectReference?.Dispose();
        }

        _disposed = true;

        base.Dispose(disposing);
    }


    /// <summary>
    /// For Material Theme to notify of menu closure via JS Interop.
    /// </summary>
    [JSInvokable]
    public void NotifyClosed()
    {
        IsOpen = false;

        if (OnMenuClosed != null)
        {
            _ = InvokeAsync(OnMenuClosed);
        }
    }


    /// <summary>
    /// Toggles the menu open and closed. NEED TO RETURN <code>Task</code> RATHER THAN <code>Task&lt;string&gt;</code> IN VERSION 2.0.0
    /// </summary>
    /// <returns></returns>
    public async Task ToggleAsync()
    {
        if (IsOpen)
        {
            await InvokeJsVoidAsync("MaterialBlazor.MBMenu.hide", ElementReference);
            IsOpen = false;
        }
        else
        {
            await InvokeJsVoidAsync("MaterialBlazor.MBMenu.show", ElementReference);
            IsOpen = true;
        }
    }


    /// <inheritdoc/>
    internal override async Task InstantiateMcwComponent()
    {
        if (!_disposed)
        {
            await InvokeJsVoidAsync("MaterialBlazor.MBMenu.init", ElementReference, ObjectReference).ConfigureAwait(false);
        }
    }


    /// <summary>
    /// Returns a menu surface class determined by the parameter.
    /// </summary>
    /// <param name="surfacePositioning"></param>
    /// <returns></returns>
    internal static string GetMenuSurfacePositioningClass(MBMenuSurfacePositioning surfacePositioning) =>
        surfacePositioning switch
        {
            MBMenuSurfacePositioning.FullWidth => "mdc-menu-surface--fullwidth",
            MBMenuSurfacePositioning.Fixed => "mdc-menu-surface--fixed",
            _ => ""
        };
}
