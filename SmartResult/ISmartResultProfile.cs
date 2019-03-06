using System;
using AutoMapper;

namespace SmartResult
{
    public interface ISmartResultProfile<D, M, N> : ISmartResultProfile
    {
        Type Desktop { get; }
        Type Mobile { get; }
        Type Native { get; }
    }

    public interface ISmartResultProfile<D, T> : ISmartResultProfile
    {
        Type Desktop { get; }
        Type MobileOrNative { get; }
    }

    public interface ISmartResultProfile
    {
        Profile Profile { get; }
    }

    public enum Client
    {
        Mobile,
        Native
    }
}
