using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;

namespace MyMassiveHeatSink
{
    public class Patch : UserMod2
    {
        public static string Namespace { get; private set; }
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);
            Patch.Namespace = base.GetType().Namespace;
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        private class Fan_Yi
        {
            public static void Postfix()
            {
                global::Debug.Log("加载翻译");
                Utils.Localize(typeof(STRINGS));
            }
        }
    }
}
