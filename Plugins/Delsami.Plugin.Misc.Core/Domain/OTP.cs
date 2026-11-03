using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.Core.Domain
{
    public class OTP : BaseEntity
    {
        public string Value { get; set; }

        public string Username { get; set; }

        public bool Used { get; set; }

        public int TryCount { get; set; }

        public DateTime? UsedDateUtc { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public DateTime ExpireOnUtc { get; set; }

    }
}