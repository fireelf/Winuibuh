// -
// <copyright file="PhotoArea.xaml.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.Controls
{
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Data;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.ViewModels;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// PhotoArea class contains the code behind for the PhotoArea user control.
    /// The PhotoArea user control shows the photo that was taken with the camera 
    /// or opened from the camera roll.
    /// </summary>
    public partial class PhotoArea : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the PhotoArea class.
        /// </summary>
        public PhotoArea()
        {
            this.InitializeComponent();
            this.DataContext = new PhotoAreaViewModel(OcrData.Instance, OcrConversionStateManager.Instance);
        }
    }
}
