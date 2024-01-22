using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace FIFAScripts.MVVM.ViewModels;


public partial class GitHubViewModel : ObservableObject
{

    [ObservableProperty]
    private string? _header;

    public readonly static string GitHubLink = "https://github.com/alisharey/FIFAScripts.MVVM";

    [ObservableProperty]
    private string? _URL = GitHubLink;
    
    

    
}
