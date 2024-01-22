using System.Windows;
using System.Windows.Controls;

using FIFAScripts.MVVM.ViewModels;

namespace FIFAScripts.MVVM;

public class TabItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate Normal { get; set; } = new DataTemplate();
    public DataTemplate Github { get; set; } = new DataTemplate();

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is GitHubViewModel)
        {
            return Github;
        }

        else
        {
            return Normal;
        }
    }
}