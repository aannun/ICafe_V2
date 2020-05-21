using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ICafeUI
{
    public interface IControlInterface
    {
        void AddControl(Control control);
        void RemoveControl(Control control);
    }
}
