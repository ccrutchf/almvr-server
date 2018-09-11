using System;
using System.Collections.Generic;
using System.Text;

namespace AlmVR.Server.Core
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public class ControllerAssemblyAttribute : Attribute
    {
    }
}
