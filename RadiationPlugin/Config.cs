using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Playables;

namespace RadiationPlugin
{
    public class Config
    {
        [Description("Should the plugin be enabled")]
        public bool PluginEnabled { get; set; } = true;

        [Description("Controls the effects intensity as the time on the warhead decreases, this array controls based on index. These are the time frames compared to indexes: \n # 90-80 Seconds: index 0 \n # 80-50 Seconds: index 1 \n # 50-30 Seconds: index 2 \n # 30 and bellow: index 3")]
        public byte[] IntensityPerTime { get; set; } = { 0, 1, 2, 3 };

        [Description("How often should the plugin check for who is in nuke? Note: this is also the value that is added to exposure while the player is detected in nuke. Exposure also reduces by this value every time the server checks and the player is not in nuke.")]
        public float CheckInterval { get; set; } = 5.0f;

        [Description("How long should a player be allowed to stay in nuke before radiation effects them?")]
        public float MaxExposure { get; set; } = 50f;

        [Description("Extra length of effect when it is applied.")]
        public float EffectDurationBuffer { get; set; } = 1f;
    }
}
