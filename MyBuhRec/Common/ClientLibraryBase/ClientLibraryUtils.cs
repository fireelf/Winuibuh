// -
// <copyright file="ClientLibraryUtils.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security;

    /// <summary>
    /// A class to store utility functions used by the client libraries.
    /// </summary>
    public static class ClientLibraryUtils
    {
        /// <summary>
        /// Looks for service scope.
        /// </summary>
        /// <param name="configFilePath">The config file path to look for.</param>
        /// <param name="defaultServiceScope">The default value to return if the config file does not exist.</param>
        /// <returns>Returns the service service scope.</returns>
        [SecuritySafeCritical]
        public static string LookupServiceScopeFromConfig(string configFilePath, string defaultServiceScope)
        {
            return LookupConfigFromFile(configFilePath, defaultServiceScope);
        }

        /// <summary>
        /// Looks for service host name.
        /// </summary>
        /// <param name="configFilePath">The config file path to look for.</param>
        /// <param name="defaultHostName">The default value to return if the config file does not exist.</param>
        /// <returns>Returns the service host name</returns>
        [SecuritySafeCritical]
        public static string LookupHostNameFromConfig(string configFilePath, string defaultHostName)
        {
            return LookupConfigFromFile(configFilePath, defaultHostName);
        }

        /// <summary>
        /// The function is used on a desktop client to allow override of defautl service path.
        /// It looks for settings within specified file.  If it does not exist, it returns the default.
        /// The [SecurityCritical] sttribute is needed due to CA2140 analysis rules.
        /// </summary>
        /// <param name="configFilePath">The config file path to look for.</param>
        /// <param name="defaultValue">The default value to return if the config file does not exist.</param>
        /// <returns>The config value specified in the config file, if present.  The default otherwise.</returns>
        [SecuritySafeCritical]
        public static string LookupConfigFromFile(string configFilePath, string defaultValue)
        {
            // Use reflection to invoke Win32-specific code on Win32 Platform.
            // If we run on Win32, then look for files on the disk to get the config value
            Type fileType = Type.GetType("System.IO.File", false);

            if (fileType == null)
            {
                return defaultValue;
            }

            MethodInfo existsMethodInfo = null;
            MethodInfo readLinesMethodInfo = null;

#if !NETFX_CORE  // not Win8
            existsMethodInfo = fileType.GetMethod("Exists");

            // get the method for File.Readlines(string)
            readLinesMethodInfo = fileType.GetMethod("ReadLines", new Type[] { typeof(string) });
#endif
            if (existsMethodInfo == null || readLinesMethodInfo == null)
            {
                return defaultValue;
            }

            // The following try/finally block is needed to avoid CA2202 and CA2000 warnings.
            try
            {
                bool fileExists = (bool)existsMethodInfo.Invoke(null, new object[] { configFilePath });

                if (fileExists)
                {
                    IEnumerable<string> lines = (IEnumerable<string>)readLinesMethodInfo.Invoke(null, new object[] { configFilePath });
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            return line;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (IOException)
            {
                // If we hit an IO exception, we fall back to the default.
            }
            catch (InvalidOperationException)
            {
                // on Win8 XAML app
            }
            catch (UnauthorizedAccessException)
            {
                // If we hit an access exception when reading the config, use the default.
                // These catches are not expected to be hit, but mainly here to keep from
                // crashing if somebody outside the test team manages to create these exact
                // files but not have read permissions set.
            }

            // Otherwise use the default.
            return defaultValue;
        }
    }
}