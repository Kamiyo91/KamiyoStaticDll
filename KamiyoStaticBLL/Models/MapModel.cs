using System.Collections.Generic;
using UnityEngine;

namespace KamiyoStaticBLL.Models
{
    public class MapModel
    {
        public Component Component { get; set; }
        public string Stage { get; set; }
        public List<int> StageIds { get; set; }
        public bool IsPlayer { get; set; }
        public bool OneTurnEgo { get; set; }
        public float Bgx { get; set; } = 0.5f;
        public float Bgy { get; set; } = 0.5f;
        public float Fx { get; set; } = 0.5f;
        public float Fy { get; set; } = 407.5f / 1080f;
        public bool InitBgm { get; set; } = true;
    }
}