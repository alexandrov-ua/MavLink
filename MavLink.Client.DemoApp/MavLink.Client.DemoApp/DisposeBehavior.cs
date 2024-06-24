using System;
using Avalonia;
using Avalonia.Xaml.Interactivity;

namespace MavLink.Client.DemoApp;

public class DisposeBehavior : Behavior<Visual>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is not null)
        {
            AssociatedObject.DetachedFromVisualTree += AssociatedObjectOnDetachedFromVisualTree;
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.DetachedFromVisualTree -= AssociatedObjectOnDetachedFromVisualTree;
        }

        if (AssociatedObject?.DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnDetaching();
    }

    private void AssociatedObjectOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (AssociatedObject?.DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}