﻿namespace Material.Blazor.MD2;

/// <summary>
/// Material Icons foundry details.
/// </summary>
internal class IconFoundryMI : IMBIconFoundry
{
    /// <inheritdoc/>
    MBIconFoundryName IMBIconFoundry.FoundryName => MBIconFoundryName.MaterialIcons;


    /// <summary>
    /// The Material Icons theme.
    /// </summary>
    public MBIconMITheme? Theme { get; }


    public IconFoundryMI(MBIconMITheme? theme = null)
    {
        Theme = theme;
    }
}
