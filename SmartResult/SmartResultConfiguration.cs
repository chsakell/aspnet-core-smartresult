using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using SmartResult;

namespace AspNet.Core.SmartResult
{
    #region Detection

    public delegate bool IsMobile(HttpContext context);
    public delegate bool IsNative(HttpContext context);

    #endregion

    /// <summary>
    /// Configuration for SmartResult
    /// </summary>
    public class SmartResultConfiguration
    {
        readonly IsMobile isMobile;
        readonly IsNative isNative;

        public List<SmartResultProfile> Profiles { get; }

        public SmartResultConfiguration(List<SmartResultProfile> profiles, IsMobile isMobile = null, IsNative isNative = null)
        {
            this.Profiles = profiles;
            this.isMobile = isMobile ?? IsMobileBrowser;
            this.isNative = isNative;
        }

        private bool IsMobileBrowser(HttpContext context)
        {
            return context.Request.IsMobileBrowser();
        }

        public IsMobile IsMobileBrowser()
        {
            return this.isMobile;
        }

        public IsNative IsNativeDevice()
        {
            return this.isNative;
        }
    }
}
