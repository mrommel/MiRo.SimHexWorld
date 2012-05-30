using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance
{
    interface IStatusCallback
    {
        void OnSucceeded();
        void OnFailed();
    }
}
