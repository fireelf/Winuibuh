﻿

#pragma checksum "C:\Users\sirius\Documents\Microsoft Research\Project Hawaii SDK 2.1 for Windows 8\Source\Ocr\sampleapps\WinRT\Controls\PhotoSelector.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "457B90AB7981A288530279D41C115AAE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.Controls
{
    partial class PhotoSelector : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 14 "..\..\Controls\PhotoSelector.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.OpenPicture_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


