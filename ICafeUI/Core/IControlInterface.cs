using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ICafeUI
{
    interface IControlInterface
    {
        void AddControl(Control control);
        void RemoveControl(Control control);
        Node GetNode(Guid id);
    }
}
