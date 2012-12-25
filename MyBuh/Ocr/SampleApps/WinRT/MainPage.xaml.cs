// ----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ----------------------------------------------------------------------

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT
{
    using Microsoft.Hawaii.Ocr.Client;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Controls;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Data;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Utils;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.ViewModels;
    using System;
    using System.Diagnostics;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// MainPage class contains the code behind for the MainPage user control.
    /// The MainPage user control is the main user control used by this application.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>
        /// OCR result data
        /// </summary>
        private OcrData ocrData;

        /// <summary>
        /// OCR State manager
        /// </summary>
        private OcrConversionStateManager ocrConversionStateManager;

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            if (!this.CredentialsAreSet())
            {
                return;
            }

            this.ocrData = OcrData.Instance;
            this.ocrData.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.OcrData_PropertyChanged);

            this.ocrConversionStateManager = OcrConversionStateManager.Instance;
            this.ocrConversionStateManager.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.ConversionStateManager_PropertyChanged);
        }

        /// <summary>
        /// Verify that credentials are set
        /// </summary>
        private bool CredentialsAreSet()
        {
            bool isValid = !string.IsNullOrEmpty(HawaiiClient.HawaiiApplicationId);

            if (!isValid)
            {
                ExitApp(
                    "Service credentials are not set. Please consult the \"Hawaii Installation Guide\" on how to obtain credentials",
                    "Credentials not set");
            }
            return isValid;
        }

        /// <summary>
        /// Exits an app by throwing an unhandled exception and displaying an error message
        /// </summary>
        /// <param name="message">message for the exception</param>
        /// <param name="caption">xaption for the messdage</param>
        private async void ExitApp(string message, string caption)
        {
            await Dispatcher.RunAsync(
               Windows.UI.Core.CoreDispatcherPriority.Normal,
               async () =>
               {
                   MessageDialog msg = new MessageDialog(message, caption);
                   await msg.ShowAsync();
                   throw new Exception(message);
               });
        }

        /// <summary>
        /// Event that fires when state of conversion has changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event parameter</param>
        private async void ConversionStateManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "OcrConversionState")
            {
                if (this.ocrConversionStateManager.OcrConversionState != OcrConversionState.Converting)
                {
                    await Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal, 
                        () =>
                        {
                            this.mainFlipView.SelectedIndex = 2;
                        });
                }
            }
        }

        /// <summary>
        /// Property has changed for the OCR data
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event parameter</param>
        private void OcrData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhotoStream")
            {
                if (this.ocrData.PhotoStream != null)
                {
                    this.UpdateAfterPhotoStreamChanged();
                }
            }
        }

        /// <summary>
        /// The photo has been updated
        /// </summary>
        private void UpdateAfterPhotoStreamChanged()
        {
            this.StartOcrConversion();
            var photoItem = (FlipViewItem)this.mainFlipView.Items[1];
            var textoItem = (FlipViewItem)this.mainFlipView.Items[2];
            textoItem.IsEnabled = photoItem.IsEnabled = true;
            textoItem.Visibility = photoItem.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Ocr conversion button was clicked
        /// </summary>
        private async void StartOcrConversion()
        {
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                await OcrClientUtils.GetPhotoBits(this.ocrData.PhotoStream),
                async (output) =>
                {
                    // This section defines the body of what is known as an anonymous method. 
                    // This anonymous method is the callback method 
                    // called on the completion of the OCR process.
                    // Using Dispatcher.BeginInvoke ensures that 
                    // OnOcrCompleted is invoked on the Main UI thread.
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => OnOcrCompleted(output));
                });

            this.ocrConversionStateManager.OcrConversionState = OcrConversionState.Converting;
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, 
                () =>
                { 
                    this.mainFlipView.SelectedIndex = 1; 
                });
        }

        /// <summary>
        /// Fires when OCR has been completed
        /// </summary>
        /// <param name="result">OCR has been completed</param>
        private void OnOcrCompleted(OcrServiceResult result)
        {
            Debug.Assert(result != null, "result is null");

            if (result.Status == Status.Success)
            {
                this.ocrData.SetOcrResult(result.OcrResult.OcrTexts);

                if (this.ocrData.GetWordCount() > 0)
                {
                    this.ocrConversionStateManager.OcrConversionState = OcrConversionState.ConversionOK;
                }
                else
                {
                    this.ocrConversionStateManager.OcrConversionState = OcrConversionState.ConversionEmpty;
                }
            }
            else
            {
                this.ocrConversionStateManager.OcrConversionState = OcrConversionState.ConversionError;
                this.ocrConversionStateManager.OcrConversionErrorMessage = result.Exception.Message;
            }

            this.SetTextAreaMode(TextViewMode.Detailed);
        }

        /// <summary>
        /// Sets the mode of the text area
        /// </summary>
        /// <param name="textViewMode">mode of the text area</param>
        private void SetTextAreaMode(TextViewMode textViewMode)
        {
            if (this.mainFlipView.Items.Count >= 3)
            {
                FlipViewItem textAreaPivot = (FlipViewItem)this.mainFlipView.Items[2];
                StackPanel stackPanel = (StackPanel)textAreaPivot.Content;
                TextArea textArea = (TextArea)stackPanel.Children[1];
                TextAreaViewModel textAreaViewModel = (TextAreaViewModel)textArea.DataContext;
                textAreaViewModel.TextViewMode = textViewMode;
            }
        }

        /// <summary>
        /// Helper method to display messages.
        /// </summary>
        /// <param name="message">A valid message.</param>
        /// <param name="caption">A valid caption.</param>
        private async void DisplayMessage(string message, string caption)
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog msg = new MessageDialog(message, caption);
                    await msg.ShowAsync();
                });
        }

        private void photoSelector_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}