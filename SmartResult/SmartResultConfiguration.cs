using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartResult
{
    /// <summary>
    /// Configuration
    /// </summary>
    public class SmartResultConfiguration
    {
        private readonly List<Profile> profiles;
        public delegate bool IsMobile(HttpContext context);
        public delegate bool IsNative(HttpContext context);
        IsMobile isMobile;
        IsNative isNative;

        public List<Profile> Profiles
        {
            get
            {
                return this.profiles;
            }
            private set { }
        }

        public SmartResultConfiguration(List<Profile> profiles, IsMobile isMobile = null, IsNative isNative = null)
        {
            this.profiles = profiles;
            this.isMobile = isMobile != null ? isMobile : IsMobileBrowser;
            this.isNative = isNative != null ? isNative : IsMobileBrowser;
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
