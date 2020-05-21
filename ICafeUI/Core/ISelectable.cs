using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafeUI.Core
{
    public interface ISelectable
    {
        event EventHandler OnDestroy;

        void Select();
        void Deselect();
        void Destroy();
        void Refresh();
        bool IsSelected();
    }
}
