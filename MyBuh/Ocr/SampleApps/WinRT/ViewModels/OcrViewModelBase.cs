// -
// <copyright file="OcrViewModelBase.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.ViewModels
{
    using System;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Data;
    using Microsoft.Hawaii.Ocr.SampleAppWinRT.Utils;

    /// <summary>
    /// Base class for at least some of the view model classes used in the OcrTestClient project.
    /// Since view models commonly need access to OcrData and OcrConversionStateManager instances
    /// we collected that commonality here.
    /// </summary>
    public class OcrViewModelBase : ModelBase
    {
        /// <summary>
        /// OCR data
        /// </summary>
        private OcrData internalOcrData;

        /// <summary>
        /// OCR state manager
        /// </summary>
        private OcrConversionStateManager internalOcrConversionStateManager;

        /// <summary>
        /// Initializes a new instance of the OcrViewModelBase class.
        /// </summary>
        /// <param name="ocrData">
        /// A reference to the OcrData instance that stores the 
        /// photo stream and the text obtained after the OCR conversion.
        /// </param>
        /// <param name="ocrConversionStateManager">
        /// A reference to the OcrConversionStateManager instance that stores the 
        /// status of the last OCR conversion.
        /// </param>
        public OcrViewModelBase(OcrData ocrData, OcrConversionStateManager ocrConversionStateManager)
        {
            if (ocrData == null)
            {
                throw new ArgumentNullException("ocrData");
            }

            if (ocrConversionStateManager == null)
            {
                throw new ArgumentNullException("ocrConversionStateManager");
            }

            this.OcrData = ocrData;
            this.OcrConversionStateManager = ocrConversionStateManager;
        }

        /// <summary>
        /// Gets the reference to the OcrData instance that stores the 
        /// photo stream and the text obtained after the OCR conversion.
        /// Note: the internal set will raise the PropertyChanged event.
        /// </summary>
        public OcrConversionStateManager OcrConversionStateManager
        {
            get
            {
                return this.internalOcrConversionStateManager;
            }

            private set
            {
                if (!object.ReferenceEquals(this.internalOcrConversionStateManager, value))
                {
                    this.internalOcrConversionStateManager = value;
                    this.OnPropertyChanged("OcrConversionStateManager");
                }
            }
        }

        /// <summary>
        /// Gets the reference to the OcrConversionStateManager instance that stores the 
        /// status of the last OCR conversion.
        /// Note: the internal set will raise the PropertyChanged event.
        /// </summary>
        public OcrData OcrData
        {
            get
            {
                return this.internalOcrData;
            }

            private set
            {
                if (!object.ReferenceEquals(this.internalOcrData, value))
                {
                    this.internalOcrData = value;
                    this.OnPropertyChanged("OcrData");
                }
            }
        }
    }
}
