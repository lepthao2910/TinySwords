using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Create Missing Items (Day 24-25)
/// Tạo Item_Arrow, Item_Bow, Item_WoodenShield và cập nhật các Shield cũ sang ItemType.Shield.
/// </summary>
public static class CreateMissingItems
{
    const string FOLDER = "Assets/Data/Items";

    [MenuItem("Tools/WBS/Create Missing Items (Day 24-25)")]
    static void CreateItems()
    {
        // Item_GoblinShard — vật liệu rơi từ Goblin (Day 26)
        CreateIfMissing("Item_GoblinShard", item =>
        {
            item.itemId      = "goblin_shard";
            item.displayName = "Mảnh Goblin";
            item.description = "Mảnh vỡ thu được từ Goblin. Dùng để chế tạo.";
            item.itemType    = ItemType.Resource;
            item.maxStack    = 20;
        });
        // 1. Arrow trước để Bow có thể tham chiếu
        CreateIfMissing("Item_Arrow", item =>
        {
            item.itemId      = "arrow";
            item.displayName = "Mũi Tên";
            item.description = "Làm từ gỗ. Dùng cho cung.";
            item.itemType    = ItemType.Resource;
            item.maxStack    = 99;
        });

        AssetDatabase.SaveAssets(); // flush để load được ngay bên dưới

        // 2. Bow — tham chiếu ammoItem = Item_Arrow
        var arrowAsset = AssetDatabase.LoadAssetAtPath<ItemData>($"{FOLDER}/Item_Arrow.asset");
        CreateIfMissing("Item_Bow", item =>
        {
            item.itemId              = "bow";
            item.displayName         = "Cung";
            item.description         = "Vũ khí tầm xa. Cần mũi tên để bắn.";
            item.itemType            = ItemType.Weapon;
            item.maxStack            = 1;
            item.attackDamage        = 10f;
            item.attackCooldown      = 0.8f;
            item.knockbackForce      = 2f;
            item.fatigueCostPerSwing = 4f;
            item.isRanged            = true;
            item.projectileSpeed     = 14f;
            item.ammoItem            = arrowAsset;
        });

        // 3. Khiên Gỗ
        CreateIfMissing("Item_WoodenShield", item =>
        {
            item.itemId               = "wooden_shield";
            item.displayName          = "Khiên Gỗ";
            item.description          = "Giữ Shift để chặn. Giảm 40% sát thương nhận.";
            item.itemType             = ItemType.Shield;
            item.maxStack             = 1;
            item.blockDamageReduction = 0.4f;
            item.blockSpeedMultiplier = 0.55f;
        });

        // 4. Cập nhật shield cũ sang ItemType.Shield + gán block stats
        FixShield("Item_StoneShield", 0.55f, 0.60f,
                  "Khiên Đá", "Giữ Shift để chặn. Giảm 55% sát thương nhận.");
        FixShield("Item_GoldShield",  0.70f, 0.65f,
                  "Khiên Vàng", "Giữ Shift để chặn. Giảm 70% sát thương nhận.");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[WBS] ✓ Tạo/cập nhật Item_Arrow, Item_Bow, Item_WoodenShield, Item_StoneShield, Item_GoldShield");
        EditorUtility.DisplayDialog("WBS", "Tạo item xong!\n\nItem_Arrow, Item_Bow, Item_WoodenShield đã được tạo.\nItem_StoneShield và Item_GoldShield đã cập nhật sang ItemType.Shield.", "OK");
    }

    static void CreateIfMissing(string assetName, System.Action<ItemData> init)
    {
        string path = $"{FOLDER}/{assetName}.asset";
        if (AssetDatabase.LoadAssetAtPath<ItemData>(path) != null)
        {
            Debug.Log($"[WBS] {assetName}.asset đã tồn tại, bỏ qua.");
            return;
        }
        var instance = ScriptableObject.CreateInstance<ItemData>();
        init(instance);
        AssetDatabase.CreateAsset(instance, path);
        Debug.Log($"[WBS] Tạo {assetName}.asset ✓");
    }

    static void FixShield(string assetName, float reduction, float speedMult, string displayName, string desc)
    {
        string path   = $"{FOLDER}/{assetName}.asset";
        var    shield = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (shield == null) { Debug.LogWarning($"[WBS] Không tìm thấy {assetName}.asset"); return; }

        shield.itemType             = ItemType.Shield;
        shield.blockDamageReduction = reduction;
        shield.blockSpeedMultiplier = speedMult;
        if (!string.IsNullOrEmpty(displayName)) shield.displayName = displayName;
        if (!string.IsNullOrEmpty(desc))        shield.description = desc;
        EditorUtility.SetDirty(shield);
        Debug.Log($"[WBS] Cập nhật {assetName}.asset → ItemType.Shield ✓");
    }
}