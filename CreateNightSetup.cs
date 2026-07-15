using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Tools → WBS → Setup Night System (Day 30)
/// 1. Tạo NightHazardSystem trong scene (nếu chưa có).
/// 2. Tạo PlayerLantern.prefab (Point Light 2D + PlayerLantern script).
/// 3. Hướng dẫn gắn vào player.
/// </summary>
public static class CreateNightSetup
{
    const string PREFAB_OUT = "Assets/Prefabs/Effects";

    [MenuItem("Tools/WBS/Setup Night System (Day 30)")]
    static void Run()
    {
        // ── 1. NightHazardSystem trong scene ─────────────────────────────────
        bool hadSystem = false;
        foreach (var obj in Object.FindObjectsByType<NightHazardSystem>(FindObjectsSortMode.None))
        {
            hadSystem = true;
            break;
        }

        if (!hadSystem)
        {
            var sysGO = new GameObject("NightHazardSystem");
            sysGO.AddComponent<NightHazardSystem>();
            Undo.RegisterCreatedObjectUndo(sysGO, "Create NightHazardSystem");
            Debug.Log("[WBS] Tạo NightHazardSystem trong scene ✓");
        }
        else
        {
            Debug.Log("[WBS] NightHazardSystem đã có trong scene, bỏ qua.");
        }

        // ── 2. Tạo thư mục Effects nếu chưa có ───────────────────────────────
        if (!AssetDatabase.IsValidFolder(PREFAB_OUT))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Effects");
            AssetDatabase.SaveAssets();
        }

        // ── 3. Tạo PlayerLantern prefab ───────────────────────────────────────
        string prefabPath = $"{PREFAB_OUT}/PlayerLantern.prefab";
        bool   existed    = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null;

        var go = new GameObject("PlayerLantern");

        var lt = go.AddComponent<Light2D>();
        lt.lightType            = Light2D.LightType.Point;
        lt.intensity            = 0f;          // tắt ban ngày — PlayerLantern.cs tự bật đêm
        lt.pointLightOuterRadius = 0f;
        lt.pointLightInnerRadius = 0f;
        lt.color                = new Color(1f, 0.85f, 0.55f);  // ánh vàng ấm
        lt.shadowsEnabled       = false;

        go.AddComponent<PlayerLantern>();

        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string status = existed ? "PlayerLantern.prefab đã cập nhật." : "PlayerLantern.prefab đã được tạo.";
        Debug.Log($"[WBS] ✓ {status}");

        // ── 4. Hướng dẫn ──────────────────────────────────────────────────────
        EditorUtility.DisplayDialog("WBS — Day 30 Night System",
            status + "\n\n" +
            "Các bước tiếp theo:\n\n" +
            "① Kéo PlayerLantern.prefab vào player GameObject làm child\n" +
            "   → Transform: Position (0, 0, 0)\n\n" +
            "② Kiểm tra NightHazardSystem đã có trong scene (Hierarchy)\n\n" +
            "③ Trong EnemySpawner:\n" +
            "   • Night Spawn Mult = 1.5 (mặc định)\n" +
            "   • Night Interval Mult = 0.6 (spawn nhanh hơn ban đêm)\n" +
            "   • Night Only = true cho spawner chỉ hoạt động ban đêm\n\n" +
            "④ Trong EnemyBase (Inspector từng prefab):\n" +
            "   • Night Speed Mult = 1.25 (mặc định)\n" +
            "   • Night Damage Mult = 1.20 (mặc định)\n\n" +
            "⑤ Test: Dùng DebugPanel hoặc TimeSystem.Instance.AdvanceTime(12f) để chuyển sang đêm",
            "OK");
    }
}