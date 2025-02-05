﻿using Material.Blazor.Internal;

namespace Material.Blazor;

/// <summary>
/// A Material.Blazor outlined text field.
/// </summary>
public sealed class MBOutlinedTextField : InternalTextFieldBase
{
    private protected override string WebComponentName()
    {
        return "md-outlined-text-field";
    }
}
