using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ICafe.Core
{
    class Player
    {
        List<Node> nodes;
        Timer timer;

        public Player(List<Node> nodes)
        {
            this.nodes = nodes;

            timer = new Timer(1f / 60f);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }
    }
}
