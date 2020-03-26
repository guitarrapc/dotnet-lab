using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    /// <summary>
    /// make everything normal.
    /// </summary>
    public interface INormal
    {
    }

    // this will not not inheric comment from interface.
    /// <inheritdoc/>
    public class Normal : INormal { }
}
