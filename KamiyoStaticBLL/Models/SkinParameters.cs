using System.Collections.Generic;

namespace KamiyoStaticBLL.Models
{
    public class SkinNames
    {
        public string PackageId { get; set; }
        public string Name { get; set; }
        public List<SkinParameters> SkinParameters { get; set; }
    }

    public class SkinParameters
    {
        public float PivotPosX { get; set; } = 0;
        public float PivotPosY { get; set; } = 0;
        public float PivotHeadX { get; set; } = 0;
        public float PivotHeadY { get; set; } = 0;
        public float HeadRotation { get; set; } = 0;
        public ActionDetail Motion { get; set; }
        public string FileName { get; set; }
    }
}