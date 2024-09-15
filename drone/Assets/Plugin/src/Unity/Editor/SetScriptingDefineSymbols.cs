using UnityEditor;

[InitializeOnLoad]
public class SetScriptingDefineSymbols
{
    static SetScriptingDefineSymbols()
    {
        // AndroidにNO_USE_GRPCを追加
        SetSymbolForGroup(BuildTargetGroup.Android, "NO_USE_GRPC");
        SetSymbolForGroup(BuildTargetGroup.Standalone, "NO_USE_GRPC");
        SetSymbolForGroup(BuildTargetGroup.WebGL, "NO_USE_GRPC");
        SetSymbolForGroup(BuildTargetGroup.iOS, "NO_USE_GRPC");

        // 他のグループの設定もここに記述
    }

    static void SetSymbolForGroup(BuildTargetGroup group, string symbol)
    {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        if (!defines.Contains(symbol))
        {
            defines += ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        }
    }
}
