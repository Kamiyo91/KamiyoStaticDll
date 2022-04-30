namespace KamiyoStaticBLL.Models
{
    public class UnitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pos { get; set; }
        public SephirahType Sephirah { get; set; }
        public bool LockedEmotion { get; set; }
        public int MaxEmotionLevel { get; set; } = 0;
        public int EmotionLevel { get; set; }
        public bool AddEmotionPassive { get; set; } = true;
        public bool OnWaveStart { get; set; }
        public XmlVector2 CustomPos { get; set; }
    }
}