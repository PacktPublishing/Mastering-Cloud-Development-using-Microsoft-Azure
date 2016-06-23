using System;

namespace IAMVeryBad
{
    internal class UserContext : IDisposable
    {
        public Users Users { get; internal set; }
    }
}