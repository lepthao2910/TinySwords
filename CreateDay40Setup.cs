using UnityEditor;
using UnityEngine;

public static class CreateDay40Setup
{
    [MenuItem("Tools/WBS/Setup Milestones (Day 40)")]
    static void Run()
    {
        var gm = Object.FindFirstObjectByType<GameManager>();

        // ── MilestoneTracker ──────────────────────────────────────────────────
        var tracker = Object.FindFirstObjectByType<MilestoneTracker>();
        if (tracker == null)
        {
            GameObject target = gm != null ? gm.gameObject : new GameObject("MilestoneTracker");
            target.AddComponent<MilestoneTracker>();
            if (gm == null) Undo.RegisterCreatedObjectUndo(target, "Create MilestoneTracker");
            Debug.Log($"[WBS] MilestoneTracker gắn vào {target.name} ✓");
        }
        else
        {
            Debug.Log("[WBS] MilestoneTracker đã có, bỏ qua.");
        }

        // ── MilestoneHUD ──────────────────────────────────────────────────────
        var hud = Object.FindFirstObjectByType<MilestoneHUD>();
        if (hud == null)
        {
            GameObject target = gm != null ? gm.gameObject : new GameObject("MilestoneHUD");
            target.AddComponent<MilestoneHUD>();
            if (gm == null) Undo.RegisterCreatedObjectUndo(target, "Create MilestoneHUD");
            Debug.Log($"[WBS] MilestoneHUD gắn vào {target.name} ✓");
        }
        else
        {
            Debug.Log("[WBS] MilestoneHUD đã có, bỏ qua.");
        }

        EditorUtility.DisplayDialog(
            "WBS — Day 40 Milestones",
            "Hệ thống milestone đã cài đặt!\n\n" +

            "─── 4 Mục tiêu trên HUD ───\n" +
            "◻  Sống sót 10 ngày\n" +
            "◻  Xây dựng công trình chính\n" +
            "◻  Rèn vũ khí cấp cao\n" +
            "◻  Hạ gục Chúa Quái\n\n" +

            "─── Cần làm trong Inspector ───\n" +
            "MilestoneTracker → Required Buildings:\n" +
            "  kéo BuildingData (Lò rèn, Khu nghỉ...) vào đây\n\n" +
            "MilestoneTracker → High Tier Damage Threshold:\n" +
            "  đặt = attackDamage vũ khí cấp 2 (mặc định 20)\n\n" +

            "─── Phím tắt ───\n" +
            "M: Ẩn/hiện panel milestone góc trên-trái",
            "OK");
    }
}
