using System;
using System.Collections.Generic;
using System.Text;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class CompoundService : BasicWebService
    {
        public override bool Start(HostControl hostControl)
        {
            return true;
        }
    }
}
