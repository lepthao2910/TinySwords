using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Raid System (Day 31)
/// Tạo BaseRaidSystem + RaidWarningUI vào scene.
/// </summary>
public static class CreateRaidSetup
{
    [MenuItem("Tools/WBS/Setup Raid System (Day 31)")]
    static void Run()
    {
        bool hadRaid    = false;
        bool hadWarning = false;

        foreach (var obj in Object.FindObjectsByType<BaseRaidSystem>(FindObjectsSortMode.None))
            hadRaid = true;
        foreach (var obj in Object.FindObjectsByType<RaidWarningUI>(FindObjectsSortMode.None))
            hadWarning = true;

        // ── BaseRaidSystem ────────────────────────────────────────────────────
        if (!hadRaid)
        {
            var go = new GameObject("BaseRaidSystem");
            go.AddComponent<BaseRaidSystem>();
            Undo.RegisterCreatedObjectUndo(go, "Create BaseRaidSystem");

            // Đặt gần cạnh map (người dùng tự chỉnh vị trí sau)
            go.transform.position = Vector3.zero;
            Debug.Log("[WBS] Tạo BaseRaidSystem trong scene ✓");
        }
        else
        {
            Debug.Log("[WBS] BaseRaidSystem đã có, bỏ qua.");
        }

        // ── RaidWarningUI ─────────────────────────────────────────────────────
        if (!hadWarning)
        {
            var go = new GameObject("RaidWarningUI");
            go.AddComponent<RaidWarningUI>();
            Undo.RegisterCreatedObjectUndo(go, "Create RaidWarningUI");
            Debug.Log("[WBS] Tạo RaidWarningUI trong scene ✓");
        }
        else
        {
            Debug.Log("[WBS] RaidWarningUI đã có, bỏ qua.");
        }

        EditorUtility.DisplayDialog("WBS — Day 31 Raid System",
            "BaseRaidSystem và RaidWarningUI đã được thêm vào scene.\n\n" +
            "Cài đặt BaseRaidSystem trong Inspector:\n" +
            "① Raid Every N Nights = 3  (đợt mỗi 3 đêm)\n" +
            "② First Raid Night = 3      (an toàn 2 đêm đầu)\n" +
            "③ Warning Duration = 30s\n" +
            "④ Base Raid Count = 5       (số quái đợt đầu)\n" +
            "⑤ Raid Count Per Day = 1    (thêm 1 quái/ngày sống)\n" +
            "⑥ Spawn Radius = 4          (kéo transform về phía cửa base)\n" +
            "⑦ Raid Prefabs: kéo SpearGoblin và/hoặc GnollEnemy vào\n\n" +
            "Test nhanh:\n" +
            "• DebugPanel (F1) → +12h × 3 lần → đến đêm thứ 3\n" +
            "• Xem console log: [Raid] ⚠ và [Raid] ▶",
            "OK");
    }
}
