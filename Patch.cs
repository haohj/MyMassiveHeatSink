using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace MyMassiveHeatSink
{
    public class Patch : UserMod2
    {
        public static string Namespace { get; private set; }
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);
            new POptions().RegisterOptions(this, typeof(Config));
            Debug.Log("菜单配置加载！");
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
