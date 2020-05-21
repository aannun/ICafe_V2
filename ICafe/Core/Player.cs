using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace ICafe.Core
{
    public class Player
    {
        public Action StateChanged;

        public bool IsPlaying { get { return playing; } private set { if (value != playing) { playing = value; StateChanged?.Invoke(); } } }
        bool playing;

        List<Node> nodes, start_nodes;
        DispatcherTimer timer;

        Stopwatch stop_watch;

        public Player(List<Node> nodes, long frequency = 1)
        {
            InitNodes(nodes);

            stop_watch = new Stopwatch();

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(frequency);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        public void InitNodes(List<Node> nodes)
        {
            if (nodes == null) return;
            if (timer != null && IsPlaying) return;

            this.nodes = nodes;
            this.start_nodes = new List<Node>();

            for (int i = 0; i < nodes.Count; i++)
            {
                var list = nodes[i].GetFields();
                bool empty = true;

                for (int j = 0; j < list.Length; j++)
                {
                    if (list[j].Inputs.Count > 0)
                    {
                        empty = false;
                        break;
                    }
                }
                if (empty)
                    this.start_nodes.Add(nodes[i]);
            }
        }

        public void Start()
        {
            if (IsPlaying == true) return;

            IsPlaying = true;
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].ResetExecution();
                nodes[i].Start();
            }
            timer.Start();
        }

        public void Stop()
        {
            if (IsPlaying == false) return;

            timer.Stop();
            IsPlaying = false;

            Wrapper.ClearContextState();

            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Stop();

            System.GC.Collect();
        }

        private void Update()
        {
            //stop_watch.Restart();

            for (int i = 0; i < start_nodes.Count; i++)
            {
                var out_val = start_nodes[i].CallFullExecution(true);
                if (out_val != 0)
                {
                    Stop();
                    Core.Console.WriteLine("Output error: " + out_val.ToString());
                    return;
                }
            }

            for (int i = 0; i < nodes.Count; i++)
                nodes[i].ResetExecution();

            //stop_watch.Stop();
            //Console.WriteLine(stop_watch.ElapsedMilliseconds.ToString());
        }
    }
}
