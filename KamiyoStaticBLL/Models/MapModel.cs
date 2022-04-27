using System;
using System.Collections.Generic;

namespace KamiyoStaticBLL.Models
{
    public class MapModel
    {
        public Type Component { get; set; }
        public string Stage { get; set; }
        public List<LorId> StageIds { get; set; }
        public bool IsPlayer { get; set; }
        public bool OneTurnEgo { get; set; }
        public float Bgx { get; set; } = 0.5f;
        public float Bgy { get; set; } = 0.5f;
        public float Fx { get; set; } = 0.5f;
        public float Fy { get; set; } = 407.5f / 1080f;
        public bool InitBgm { get; set; } = true;
    }
}