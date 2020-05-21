using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICafeUI
{
    /// <summary>
    /// Logica di interazione per ConnectionSlot.xaml
    /// </summary>
    public partial class ConnectionSlot : UserControl
    {
        public bool IsInput { get; private set; }
        public Node node { get; private set; }

        List<ConnectionLine> lines;
        float connection_center_radius = 25;

        public ConnectionSlot()
        {
            InitializeComponent();

            lines = new List<ConnectionLine>();
        }

        public void Init(bool input, Node node)
        {
            this.IsInput = input;
            this.node = node;

            Border.IsHitTestVisible = false;
        }

        public void SetColor(Brush color)
        {
            Border.Background = color;
        }

        //public List<Node> GetConnections()
        //{
        //    var list = IsInput ? node.Inputs : node.Outputs;
        //    if (list == null) return null;

        //    List<Node> out_list = new List<Node>();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (!out_list.Contains(list[i]))
        //            out_list.Add(list[i]);
        //    }

        //    return out_list;
        //}

        void AddConnection()
        {
            ConnectionLine l = new ConnectionLine(connection_center_radius, connection_center_radius);
            l.SetLineStart(ActualWidth * 0.5f, ActualHeight * 0.5f);
            LineCanvas.Children.Add(l);
            lines.Add(l);
        }

        void RemoveConnection(ConnectionLine l)
        {
            LineCanvas.Children.Remove(l);
            lines.Remove(l);
        }

        public void Refresh()
        {
            //var connections = GetConnections();
            //if (connections == null) return;

            //if (connections.Count > lines.Count)
            //{
            //    int num = connections.Count - lines.Count;
            //    for (int i = 0; i < num; i++)
            //        AddConnection();
            //}
            //else if (connections.Count < lines.Count)
            //{
            //    int num = lines.Count - connections.Count;
            //    for (int i = 0; i < num; i++)
            //        RemoveConnection(lines[lines.Count - 1]);
            //}

            //AssignConnectionNodes();
            //UpdateConnections();
        }

        void AssignConnectionNodes()
        {
            //var connections = GetConnections();
            //for (int i = 0; i < connections.Count; i++)
            //    lines[i].SetNodes(IsInput ? node : connections[i], !IsInput ? node : connections[i]);
        }

        public void IgnoreConnection(Node node)
        {
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].NodeInput == node || lines[i].NodeOutput == node)
                    RemoveConnection(lines[i]);
            }
        }

        public void UpdateConnections()
        {
            //var connections = GetConnections();
            //if (connections == null) return;

            //for (int i = 0; i < connections.Count; i++)
            //{
            //    //ConnectionLine l = lines[i];
            //    //ConnectionSlot cs = IsInput ? connections[i].RightConnector : connections[i].LeftConnector;

            //    //Point end = TranslatePoint(new Point(cs.ActualWidth * -0.5f, cs.ActualHeight * -0.5f), cs);
            //    //l.SetLineEnd(-end.X, -end.Y);
            //}
        }
    }
}
