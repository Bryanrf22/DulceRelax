using Microsoft.Maui.Controls;

namespace DulceRelax.UI.Behaviors;

/// <summary>
/// A behavior that applies Safe Area insets to ContentPage controls.
/// </summary>
public sealed class SafeAreaBehavior : Behavior<ContentPage>
{
    public static readonly BindableProperty EdgesProperty =
        BindableProperty.Create(
            nameof(Edges),
            typeof(SafeAreaEdges),
            typeof(SafeAreaBehavior),
            SafeAreaEdges.All,
            propertyChanged: OnEdgesChanged);

    public static readonly BindableProperty SafeAreaProperty =
        BindableProperty.Create(
            nameof(SafeArea),
            typeof(Thickness),
            typeof(SafeAreaBehavior),
            new Thickness(0),
            propertyChanged: OnSafeAreaChanged);

    public SafeAreaEdges Edges { get => (SafeAreaEdges)GetValue(EdgesProperty); set => SetValue(EdgesProperty, value); }
    public Thickness SafeArea { get => (Thickness)GetValue(SafeAreaProperty); set => SetValue(SafeAreaProperty, value); }

    private static void OnEdgesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SafeAreaBehavior behavior)
        {
            // Apply initial safe area
            behavior.ApplySafeArea();

            // Hook up to page events
            if (behavior.Parent is ContentPage page)
            {
                page.Loaded += behavior.OnPageLoaded;
                page.Appearing += behavior.OnPageAppearing;
                page.Disappearing += behavior.OnPageDisappearing;
            }
        }
    }

    private static void OnSafeAreaChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SafeAreaBehavior behavior)
        {
            behavior.ApplySafeArea();
        }
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        ApplySafeArea();
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        ApplySafeArea();
    }

    private void OnPageDisappearing(object? sender, EventArgs e)
    {
        // Remove safe area when page disappears
        if (Parent is ContentPage page)
        {
            // Remove padding that was added
            var currentPadding = page.Padding;
            page.Padding = new Thickness(
                currentPadding.Left,
                0, // Top padding removed
                currentPadding.Right,
                currentPadding.Bottom
            );
        }
    }

    private void ApplySafeArea()
    {
        if (Parent is not ContentPage page)
            return;

        // Get current padding
        var currentPadding = page.Padding;

        // Calculate new padding based on SafeArea property and edges
        var newPadding = new Thickness(currentPadding.Left, currentPadding.Top, currentPadding.Right, currentPadding.Bottom);

        if (Edges.HasFlag(SafeAreaEdges.Top))
        {
            // Apply safe area padding for the top (status bar, navigation, etc.)
            newPadding.Top = SafeArea.Top + 20; // Standard status bar offset
        }

        if (Edges.HasFlag(SafeAreaEdges.Bottom))
        {
            // Apply safe area padding for the bottom (navigation bar, home indicator, etc.)
            newPadding.Bottom = SafeArea.Bottom + 20; // Standard bottom safe area
        }

        page.Padding = newPadding;
    }
}

/// <summary>
/// Enumeration of safe area edges that can be applied.
/// </summary>
[Flags]
public enum SafeAreaEdges
{
    None = 0,
    Top = 1,
    Bottom = 2,
    All = Top | Bottom
}
