﻿using Material.Blazor.MD2;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Material.Blazor.Internal.MD2;

/// <summary>
/// A DRY inspired abstract class providing <see cref="MBSelect{TItem}"/> and <see cref="MBRadioButtonGroup{TItem}"/> with validation.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingleSelectComponentMD2<T, TListElement> : InputComponentMD2<T> where TListElement : Material.Blazor.MD2.MBSelectElement<T>
{
    /// <summary>
    /// A function delegate to return the parameters for <c>@key</c> attributes. If unused
    /// "fake" keys set to GUIDs will be used.
    /// </summary>
    [Parameter] public Func<T, object> GetKeysFunc { get; set; }


    /// <summary>
    /// The item list to be represented as radio buttons
    /// </summary>
    [Parameter] public IEnumerable<TListElement> Items { get; set; }
    private IEnumerable<TListElement> _cachedItems;


    /// <summary>
    /// The form of validation to apply when Value is first set, deciding whether to accept
    /// a value outside the <see cref="Items"/> list, replace it with the first list item or
    /// to throw an exception (the default).
    /// <para>Overrides <see cref="MBCascadingDefaults.ItemValidation"/></para>
    /// </summary>
    [Parameter] public MBItemValidation? ItemValidation { get; set; }


    /// <summary>
    /// Generates keys for repeated elements in the single select list.
    /// </summary>
    private protected Func<T, object> KeyGenerator { get; set; }


    // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside Material.Blazor
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if ((Items == null && _cachedItems != null) || (Items != null && _cachedItems == null) || (Items != null && _cachedItems != null && !Items.SequenceEqual(_cachedItems)))
        {
            _cachedItems = Items;

            if (HasInstantiated)
            {
                var validatedValue = ValidateItemList(Items, Material.Blazor.MD2.MBItemValidation.DefaultToFirst).value;

                if (!validatedValue.Equals(Value))
                {
                    Value = validatedValue;
                }
            }

            AllowNextShouldRender();
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }


    // This method was added in the interest of DRY and is used by MBSelect & MBRadioButtonGroup
    /// <summary>
    /// Validates the item list against the validation specification.
    /// </summary>
    /// <param name="items">The item list</param>
    /// <param name="appliedItemValidation">Specification of the required validation <see cref="MBItemValidation"/></param>
    /// <returns>The an indicator of whether an item was found and the item in the list matching <see cref="InputComponent{T}._cachedValue"/> or default if not found.</returns>
    /// <exception cref="ArgumentException"/>
    public (bool hasValue, T value) ValidateItemList(IEnumerable<MBSelectElement<T>> items, Material.Blazor.MD2.MBItemValidation appliedItemValidation)
    {
        var componentName = Utilities.GetTypeName(GetType());

        if (items.GroupBy(i => i.SelectedValue).Any(g => g.Count() > 1))
        {
            throw new ArgumentException(componentName + " has multiple enties in the List with the same SelectedValue");
        }

        if (!items.Any(i => Equals(i.SelectedValue, Value)))
        {
            switch (appliedItemValidation)
            {
                case Material.Blazor.MD2.MBItemValidation.DefaultToFirst:
                    var defaultValue = items.FirstOrDefault().SelectedValue;
                    AllowNextShouldRender();
                    return (true, defaultValue);

                case Material.Blazor.MD2.MBItemValidation.Exception:
                    var itemList = "{ " + string.Join(", ", items.Select(item => $"'{item.SelectedValue}'")) + " }";
                    throw new ArgumentException(componentName + $" cannot select item with data value of '{Value?.ToString()}' from {itemList}");

                case Material.Blazor.MD2.MBItemValidation.NoSelection:
                    AllowNextShouldRender();
                    return (false, default);
            }
        }

        return (true, Value);
    }
}
