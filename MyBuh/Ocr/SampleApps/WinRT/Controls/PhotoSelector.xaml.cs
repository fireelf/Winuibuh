// -
// <copyright file="PhotoSelector.xaml.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.Controls
{
    using System;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Data;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// PhotoSelector class contains the code behind for the PhotoSelector user control.
    /// The PhotoSelector user control allows the user to select the method of retriving 
    /// a photo: capture from the camera or load from the camera roll.
    /// </summary>
    public partial class PhotoSelector : UserControl
    {
        /// <summary>
        /// Indicates that the size of the photo has to be reduced (scaled-down)
        /// to a certain limit before sending it over the wire.
        /// </summary>
        private const bool DoLimitPhotoSize = true;

        /// <summary>
        /// Max photo width
        /// </summary>
        private const uint PhotoMaxWidth = 640;

        /// <summary>
        /// Max photo height
        /// </summary>
        private const uint PhotoMaxHeight = 480;

        /// <summary>
        /// A reference to the OcrData instance that stores the 
        /// photo stream and the text obtained after the OCR conversion.
        /// </summary>
        private OcrData ocrData;

        /// <summary>
        /// Whether the show is in progress
        /// </summary>
        private bool showInProgress;

        /// <summary>
        /// Initializes a new instance of the PhotoSelector class.
        /// </summary>
        public PhotoSelector()
        {
            this.InitializeComponent();

            this.ocrData = OcrData.Instance;
        }

        /// <summary>
        /// Open the image selection standard dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private async void OpenPicture_Click(object sender, RoutedEventArgs e)
        {
            if (!this.showInProgress)
            {
                this.showInProgress = true;

                try
                {
                    var picker = new FileOpenPicker();

                    picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.FileTypeFilter.Add(".jpg");
                    picker.FileTypeFilter.Add(".jpeg");

                    var file = await picker.PickSingleFileAsync();
                    if (file != null)
                    {
                        // open captured file and set the image source on the control
                        this.ocrData.PhotoStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    }
                }
                finally
                {
                    this.showInProgress = false;
                }
            }
        }
    }
}