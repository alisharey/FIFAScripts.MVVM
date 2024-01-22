using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace FIFAScripts.MVVM.ViewModels;

public partial class CareerViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _header;
    
}
