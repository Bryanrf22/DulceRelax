#if IOS || MACCATALYST
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace DulceRelax.UI.Behaviors;

[Flags]
public enum SafeAreaEdges
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
    Horizontal = Left | Right,
    Vertical = Top | Bottom,
    All = Left | Top | Right | Bottom
}

public class SafeAreaBehavior : Behavior<ContentPage>
{
    ContentPage? _page;
    Thickness _basePadding;

    public SafeAreaEdges Edges { get; set; } = SafeAreaEdges.All;

    protected override void OnAttachedTo(ContentPage page)
    {
        base.OnAttachedTo(page);
        _page = page;
        _basePadding = page.Padding;
        page.Loaded += OnPageEvent;
        page.SizeChanged += OnPageEvent;
        Apply();
    }

    protected override void OnDetachingFrom(ContentPage page)
    {
        page.Loaded -= OnPageEvent;
        page.SizeChanged -= OnPageEvent;
        _page = null;
        base.OnDetachingFrom(page);
    }

    void OnPageEvent(object? sender, EventArgs e) => Apply();

    void Apply()
    {
        if (_page is null) return;

#if IOS || MACCATALYST
        _page.On<iOS>().SetUseSafeArea(Edges != SafeAreaEdges.None);
#elif ANDROID
        var i = GetAndroidInsets(_page);
        var target = new Thickness(
            _basePadding.Left + (Edges.HasFlag(SafeAreaEdges.Left) ? i.Left : 0),
            _basePadding.Top + (Edges.HasFlag(SafeAreaEdges.Top) ? i.Top : 0),
            _basePadding.Right + (Edges.HasFlag(SafeAreaEdges.Right) ? i.Right : 0),
            _basePadding.Bottom + (Edges.HasFlag(SafeAreaEdges.Bottom) ? i.Bottom : 0));

        if (_page.Padding != target)
            _page.Padding = target;
#endif
    }

#if ANDROID
    static Thickness GetAndroidInsets(ContentPage page)
    {
        if (page.Handler?.PlatformView is not Android.Views.View view)
            return default;

        var insets = AndroidX.Core.View.ViewCompat.GetRootWindowInsets(view);
        if (insets is null) return default;

        var bars = insets.GetInsets(AndroidX.Core.View.WindowInsetsCompat.Type.SystemBars());
        var density = view.Context?.Resources?.DisplayMetrics?.Density ?? 1f;

        return new Thickness(
            bars.Left / density,
            bars.Top / density,
            bars.Right / density,
            bars.Bottom / density);
    }
#endif
}