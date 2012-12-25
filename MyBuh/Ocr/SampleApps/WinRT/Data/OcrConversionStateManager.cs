// -
// <copyright file="OcrConversionStateManager.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.Data
{
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Utils;

    /// <summary>
    /// OcrConversionStateManager implements a singleton that provides access 
    /// to the state of the last OCR conversion. It inherits from ModelBase which means that 
    /// interested parties can subscribe to its PropertyChanged. It also means that references to 
    /// the singleton instance of type OcrConversionStateManager can be used in data binding 
    /// syntax in XAML files.
    /// </summary>
    public class OcrConversionStateManager : ModelBase
    {
        /// <summary>
        /// instance of OCR conversion manager
        /// </summary>
        private static OcrConversionStateManager instance = new OcrConversionStateManager();

        /// <summary>
        /// OCR conversion state
        /// </summary>
        private OcrConversionState ocrConversionState;

        /// <summary>
        /// Error Message of OCR conversion
        /// </summary>
        private string ocrConversionErrorMessage;

        /// <summary>
        /// Prevents a default instance of the <see cref="OcrConversionStateManager"/> class from being created
        /// </summary>
        private OcrConversionStateManager()
        {
            this.OcrConversionState = OcrConversionState.ConversionNotStarted;
        }

        /// <summary>
        /// Gets the singleton instance of type OcrConversionStateManager.
        /// </summary>
        public static OcrConversionStateManager Instance
        {
            get
            {
                return OcrConversionStateManager.instance;
            }
        }

        /// <summary>
        /// Gets or sets the state of the last OCR conversion.
        /// </summary>
        public OcrConversionState OcrConversionState
        {
            get
            {
                return this.ocrConversionState;
            }

            set
            {
                if (this.ocrConversionState != value)
                {
                    this.ocrConversionState = value;
                    this.OnPropertyChanged("OcrConversionState");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the error message associated with the last OCR conversion.
        /// </summary>
        public string OcrConversionErrorMessage
        {
            get
            {
                return this.ocrConversionErrorMessage;
            }

            set
            {
                if (this.ocrConversionErrorMessage != value)
                {
                    this.ocrConversionErrorMessage = value;
                    this.OnPropertyChanged("OcrConversionErrorMessage");
                }
            }
        }
    }
}
