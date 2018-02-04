using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap.Graph;

namespace TestIdentity.IoC
{
    public class GlobalRegistry : StructureMap.Registry
    {
        public GlobalRegistry()
        {
            this.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });
        }
    }
}