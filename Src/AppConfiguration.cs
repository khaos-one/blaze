using System;
using System.Collections.Generic;
using System.Text;

namespace Blaze
{
    [Serializable]
    internal sealed class AppConfiguration
    {
        public string CompilerVersion { get; set; }
        public string[] References { get; set; }
    }
}
