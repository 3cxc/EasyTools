namespace EasyTools.DataBase.Serialization
{
    public struct HintData
    {
        public float x, y;
        public int font;

        public HintData(float x, float y, int font)
        {
            this.x = x;
            this.y = y;
            this.font = font;
        }
    }
}
