using STRINGS;

namespace MyMassiveHeatSink
{
    internal class STRINGS
    {
        public static void DoReplacement()
        {
            LocString.CreateLocStringKeys(typeof(STRINGS), "");
        }

        public class BUILDINGS
        {
            public class PREFABS
            {
                public class MYMASSIVEHEATSINK
                {
                    public static LocString NAME = UI.FormatAsLink("MassiveHeatSink", "MYMASSIVEHEATSINK");

                    public static LocString EFFECT = "A self-sustaining machine powered by what appears to be refined \nAbsorbs and neutralizes energy when provided with piped Hydrogen Gas.";
                }
            }
        }
    }
}
