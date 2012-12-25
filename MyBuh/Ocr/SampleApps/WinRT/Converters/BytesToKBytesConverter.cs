// -
// <copyright file="BytesToKBytesConverter.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.Converters
{
    using System;
    using System.Globalization;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter that transforms Bytes in Kilo Bytes.
    /// </summary>
    public class BytesToKBytesConverter : IValueConverter
    {
        /// <summary>
        /// Converts the source data (value) that is a long representing some data length 
        /// expressed in bytes before passing it to the target of a data binding for display in the UI.
        /// </summary>
        /// <param name="value">
        /// The source data being passed to the target.
        /// A long representing some data length expressed in bytes.
        /// </param>
        /// <param name="targetType">
        /// The type of data expected by the target dependency property.
        /// </param>
        /// <param name="parameter">
        /// It will be ignored by this converter.
        /// </param>
        /// <param name="language">
        /// The language of the conversion. Ignored by this converter.
        /// </param>
        /// <returns>
        /// Returns a RotateTransform a string that is the representation of the given length expressed in kilo bytes.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ulong sizeInBytes = (ulong)value;
            ulong sizeInKiloBytes = (ulong)(sizeInBytes / 1024.0);

            return sizeInKiloBytes.ToString();
        }

        /// <summary>
        /// The ConvertBack is not implemented for this converter. 
        /// ConvertBack will throw NotImplementedException if invoked.
        /// </summary>
        /// <param name="value">
        /// The source data being passed to the target.
        /// A long representing some data length expressed in bytes.
        /// </param>
        /// <param name="targetType">
        /// The type of data expected by the target dependency property.
        /// </param>
        /// <param name="parameter">
        /// It will be ignored by this converter.
        /// </param>
        /// <param name="language">
        /// The language of the conversion. Ignored by this converter.
        /// </param>
        /// <returns>
        /// Not implemented
        /// </returns>
        /// <exception cref="NotImplementedException">not implemented method</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
