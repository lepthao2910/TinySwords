using UnityEditor;
using UnityEngine;

public static class CreateDay41Setup
{
    [MenuItem("Tools/WBS/Setup Tutorial (Day 41)")]
    static void Run()
    {
        var gm = Object.FindFirstObjectByType<GameManager>();

        var existing = Object.FindFirstObjectByType<TutorialManager>();
        if (existing == null)
        {
            GameObject target = gm != null ? gm.gameObject : new GameObject("TutorialManager");
            target.AddComponent<TutorialManager>();
            if (gm == null) Undo.RegisterCreatedObjectUndo(target, "Create TutorialManager");
            Debug.Log($"[WBS] TutorialManager gắn vào {target.name} ✓");
        }
        else
        {
            Debug.Log("[WBS] TutorialManager đã có, bỏ qua.");
        }

        EditorUtility.DisplayDialog(
            "WBS — Day 41 Tutorial",
            "Tutorial ngày đầu đã cài đặt!\n\n" +

            "─── 4 bước hướng dẫn ───\n" +
            "[1/4]  Thu thập Gỗ   — giữ E gần cây\n" +
            "[2/4]  Chế tạo Rìu   — nhấn C\n" +
            "[3/4]  Ăn để no      — kéo thịt vào Hotbar, nhấn 1–6\n" +
            "[4/4]  Xây Nhà chính — nhấn B\n\n" +

            "─── Optional trong Inspector ───\n" +
            "TutorialManager → Wood Item: kéo ItemData Gỗ vào\n" +
            "TutorialManager → Axe Item:  kéo ItemData Rìu vào\n" +
            "TutorialManager → Main House Data: kéo BuildingData Nhà chính vào\n" +
            "(Nếu để trống: dùng ItemType/MainHouse.Instance làm fallback)\n\n" +

            "─── Phím tắt ───\n" +
            "T: Bỏ qua toàn bộ tutorial\n\n" +

            "Tutorial chỉ hiện ngày 1 (DaysAlive == 0).\n" +
            "Tắt 'Only First Day' trong Inspector để test.",
            "OK");
    }
}
