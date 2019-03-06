using System;
using AutoMapper;

namespace SmartResult
{
    public class SmartResultProfile
    {
        public Profile Profile { get; }

        public Type Desktop { get; }

        public Type Mobile { get; }

        public Type Native { get; }

        public SmartResultProfile(Profile profile, 
            Type desktop, Type mobile = null, Type native = null)
        {
            Profile = profile;
            Desktop = desktop;
            Mobile = mobile;
            Native = native;
        }
    }
}
