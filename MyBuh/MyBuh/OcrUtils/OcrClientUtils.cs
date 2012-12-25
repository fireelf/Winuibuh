// -
// <copyright file="OcrClientUtils.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.SampleAppWinRT.Utils
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Windows.Storage.Streams;

    /// <summary>
    /// Utils is a general utility class that contains helper methods used throughout this application.
    /// </summary>
    public class OcrClientUtils
    {
        /// <summary>
        /// Returns a byte array with the content of the stream.
        /// </summary>
        /// <param name="stream">stream to get bytes for</param>
        /// <returns>awaitable byte array</returns>
        public static async Task<byte[]> GetPhotoBits(IRandomAccessStream stream)
        {
            Debug.Assert(stream != null, "GetPhotoBits should not be called with a null stream.");
            var reader = new DataReader(stream.GetInputStreamAt(0));
            var bytes = new byte[stream.Size];
            await reader.LoadAsync((uint)stream.Size);
            reader.ReadBytes(bytes);
            return bytes;
        }
    }
}