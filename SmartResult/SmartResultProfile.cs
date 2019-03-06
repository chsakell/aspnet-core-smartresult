using System;
using AutoMapper;

namespace SmartResult
{
    public class SmartResultProfile<D,M,N> : ISmartResultProfile<D, M, N>, ISmartResultProfile
    {
        public Profile Profile { get; }

        public Type Desktop => typeof(D);

        public Type Mobile => typeof(M);

        public Type Native => typeof(N);

        public SmartResultProfile(Profile profile)
        {
            Profile = profile;
        }
    }

    public class SmartResultProfile<D, T> : ISmartResultProfile<D, T>, ISmartResultProfile
    {
        public Profile Profile { get; }

        public Type Desktop => typeof(D);

        public Type Mobile => Client == Client.Mobile ? typeof(T) : null;

        public Type Native => Client == Client.Native ? typeof(T) : null;

        public Type MobileOrNative => typeof(T);

        public Client Client { get; }

        public SmartResultProfile(Profile profile, Client client)
        {
            Profile = profile;
            Client = client;
        }
    }
}
