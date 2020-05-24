using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public struct EffectChain
    {
        public List<Effect> Chain;
        public bool init;

        public int Count { get { return Chain.Count; } }
        public bool Valid { get { return Chain != null; } }

        public EffectChain(EffectChain effectChain)
        {
            Chain = effectChain.Chain == null ? new List<Effect>() : new List<Effect>(effectChain.Chain);
            init = true;
        }

        public void Add(Effect effect)
        {
            Chain.Add(effect);
        }

        public Effect Get(int index)
        {
            return Chain[index];
        }
    }

    public class EffectNode : Node
    {
        [Out]
        public int Width = 800;
        [Out]
        public int Height = 800;
        [Out]
        public string Path_To_Effect;
        [Out]
        public EffectChain EffectChain;

        protected Effect effect;

        public override void Init()
        {
            //GetReactiveField("EffectChain").Active = true;
        }

        public override void Start()
        {
            LoadEffect(Path_To_Effect);
        }

        protected void LoadEffect(string path)
        {
            effect = Effect.CreateEffect((uint)Width, (uint)Height, path);
        }

        public override void Stop()
        {
            effect.Release();
        }

        public virtual void Execute(EffectChain effectChain)
        {
            SendOutputEffects(effectChain, effect);
        }

        protected void SendOutputEffects(EffectChain chain, params Effect[] effects)
        {
            EffectChain = new EffectChain(chain);
            for (int i = 0; i < effects.Length; i++)
                EffectChain.Add(effects[i]);
        }
    }
}
