using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Repair System (Day 33)
/// - Thêm BuildingRepairUI vào scene
/// - Gán repairCost cho WoodWall, StoneWall, MainHouse và các công trình khác
/// </summary>
public static class CreateRepairSetup
{
    const string DATA_FOLDER  = "Assets/Data/Buildings";
    const string ITEM_FOLDER  = "Assets/Data/Items";

    [MenuItem("Tools/WBS/Setup Repair System (Day 33)")]
    static void Run()
    {
        // ── 1. Thêm BuildingRepairUI vào scene ──────────────────────────────
        bool hasUI = Object.FindFirstObjectByType<BuildingRepairUI>() != null;
        if (!hasUI)
        {
            var go = new GameObject("BuildingRepairUI");
            go.AddComponent<BuildingRepairUI>();
            Undo.RegisterCreatedObjectUndo(go, "Create BuildingRepairUI");
            Debug.Log("[WBS] Tạo BuildingRepairUI trong scene ✓");
        }
        else
        {
            Debug.Log("[WBS] BuildingRepairUI đã có, bỏ qua.");
        }

        // ── 2. Load item refs ────────────────────────────────────────────────
        var itemWood  = AssetDatabase.LoadAssetAtPath<ItemData>($"{ITEM_FOLDER}/Item_Wood.asset");
        var itemStone = AssetDatabase.LoadAssetAtPath<ItemData>($"{ITEM_FOLDER}/Item_Stone.asset");

        // ── 3. Gán repairCost cho từng BuildingData ──────────────────────────
        SetRepairCost("Building_WoodWall",      itemWood,  2, null,      0);
        SetRepairCost("Building_StoneWall",     null,      0, itemStone, 3);
        SetRepairCost("Building_MainHouse",     itemWood,  5, itemStone, 3);
        SetRepairCost("Building_StorageChest",  itemWood,  3, null,      0);
        SetRepairCost("Building_Forge",         itemWood,  3, itemStone, 2);
        SetRepairCost("Building_Campfire",      itemWood,  2, null,      0);
        SetRepairCost("Building_RestArea",      itemWood,  3, null,      0);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("WBS — Day 33 Repair System",
            "BuildingRepairUI và repairCost đã được cài đặt!\n\n" +
            "Chi phí sửa chữa (full HP):\n" +
            "• Hàng rào gỗ:   2 Gỗ\n" +
            "• Tường đá:       3 Đá\n" +
            "• Nhà chính:      5 Gỗ + 3 Đá\n" +
            "• Nhà kho:        3 Gỗ\n" +
            "• Lò rèn:         3 Gỗ + 2 Đá\n" +
            "• Lửa trại:       2 Gỗ\n" +
            "• Khu nghỉ:       3 Gỗ\n\n" +
            "Cách dùng:\n" +
            "Đứng gần công trình hư hỏng (≤2.5u) → bảng xuất hiện dưới màn hình\n" +
            "Nhấn E hoặc click 'Sửa chữa' để sửa.\n\n" +
            "Pathfinding:\n" +
            "Enemy AI giờ tự lách qua tường/công trình khi bị kẹt.",
            "OK");
    }

    static void SetRepairCost(string assetName, ItemData item1, int count1,
                                                ItemData item2, int count2)
    {
        string path = $"{DATA_FOLDER}/{assetName}.asset";
        var data = AssetDatabase.LoadAssetAtPath<BuildingData>(path);
        if (data == null) return;

        var list = new System.Collections.Generic.List<BuildingData.Ingredient>();
        if (item1 != null && count1 > 0)
            list.Add(new BuildingData.Ingredient { item = item1, count = count1 });
        if (item2 != null && count2 > 0)
            list.Add(new BuildingData.Ingredient { item = item2, count = count2 });

        data.repairCost = list.ToArray();
        EditorUtility.SetDirty(data);
        Debug.Log($"[WBS] {assetName}: repairCost set ✓");
    }
}