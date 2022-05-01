﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibSugar
{
    public static partial class Sugar
    {
        /// <summary>
        /// Task that will never continue
        /// </summary>
        /// <returns></returns>
        public static Task Abort() => Task.Delay(Timeout.Infinite);
    }
}
