using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Boss Loot (Day 37)
/// - Tạo Item_BossCore.asset (Ngọc Quái Vương) — vật phẩm hiếm chỉ có từ boss
/// - Thêm BossActivationZone vào scene (trigger zone, player nhấn F để đánh thức)
/// - Thêm BossLootUI vào scene
/// </summary>
public static class CreateBossLootItem
{
    const string BOSS_CORE_PATH = "Assets/Data/Items/Item_BossCore.asset";
    const string GOLD_PATH      = "Assets/Data/Items/Item_Gold.asset";

    [MenuItem("Tools/WBS/Setup Boss Loot (Day 37)")]
    static void Run()
    {
        // ── 1. Tạo Item_BossCore (Ngọc Quái Vương) ───────────────────────────
        var existing = AssetDatabase.LoadAssetAtPath<ItemData>(BOSS_CORE_PATH);
        bool isNew   = existing == null;

        if (isNew)
        {
            var item            = ScriptableObject.CreateInstance<ItemData>();
            item.itemId         = "boss_core";
            item.displayName    = "Ngọc Quái Vương";
            item.description    = "Tinh hoa của Quái Vương, chứa đựng sức mạnh huyền bí.\nVật phẩm hiếm — chỉ rơi từ Boss.";
            item.itemType       = ItemType.Resource;
            item.maxStack       = 5;

            EnsureFolder("Assets/Data");
            EnsureFolder("Assets/Data/Items");
            AssetDatabase.CreateAsset(item, BOSS_CORE_PATH);
            AssetDatabase.SaveAssets();
            Debug.Log($"[WBS] Ngọc Quái Vương tạo tại {BOSS_CORE_PATH} ✓");
        }
        else
        {
            Debug.Log("[WBS] Item_BossCore đã tồn tại, bỏ qua.");
        }

        // ── 2. Thêm BossActivationZone vào scene ──────────────────────────────
        var existingZone = Object.FindFirstObjectByType<BossActivationZone>();
        if (existingZone == null)
        {
            var go  = new GameObject("BossActivationZone");
            go.AddComponent<BossActivationZone>();
            var col    = go.AddComponent<CircleCollider2D>();
            col.radius = 5f;
            col.isTrigger = true;
            Undo.RegisterCreatedObjectUndo(go, "Create BossActivationZone");
            Debug.Log("[WBS] BossActivationZone tạo trong scene ✓ — di chuyển đến khu boss.");
        }
        else
        {
            Debug.Log("[WBS] BossActivationZone đã có trong scene, bỏ qua.");
        }

        // ── 3. Thêm BossLootUI vào scene ─────────────────────────────────────
        var existingLootUI = Object.FindFirstObjectByType<BossLootUI>();
        if (existingLootUI == null)
        {
            // Gắn vào cùng GameObject với BossHPBar nếu có
            var hpBar = Object.FindFirstObjectByType<BossHPBar>();
            if (hpBar != null)
            {
                hpBar.gameObject.AddComponent<BossLootUI>();
                Debug.Log("[WBS] BossLootUI gắn vào BossHPBar ✓");
            }
            else
            {
                var go = new GameObject("BossLootUI");
                go.AddComponent<BossLootUI>();
                Undo.RegisterCreatedObjectUndo(go, "Create BossLootUI");
                Debug.Log("[WBS] BossLootUI tạo trong scene ✓");
            }
        }
        else
        {
            Debug.Log("[WBS] BossLootUI đã có trong scene, bỏ qua.");
        }

        AssetDatabase.Refresh();

        // ── 4. Hướng dẫn cấu hình ─────────────────────────────────────────────
        var goldItem = AssetDatabase.LoadAssetAtPath<ItemData>(GOLD_PATH);
        string goldStatus = goldItem != null
            ? $"✓ Tìm thấy: {GOLD_PATH}"
            : $"✗ Không tìm thấy — kiểm tra đường dẫn {GOLD_PATH}";

        EditorUtility.DisplayDialog(
            "WBS — Day 37 Boss Loot & Activation Zone",
            "Loot boss & khu vực kích hoạt đã được cài đặt!\n\n" +

            "─── Việc cần làm ───\n\n" +

            "1. Gán Loot vào Boss Prefab:\n" +
            "   Mở Boss Prefab → BossEnemy → Boss Drops:\n" +
            "   [0] item = Item_Gold    count = 50\n" +
            "   [1] item = Item_BossCore  count = 1\n\n" +

            "2. Cấu hình BossActivationZone:\n" +
            "   - Di chuyển đến khu vực boss trong scene\n" +
            "   - Gán Boss Prefab vào Boss Prefab field\n" +
            "   - Tùy chọn: gán Boss Spawn Point (child empty GameObject)\n" +
            "   - requireInteract = true (player nhấn F) / false (tự động)\n" +
            "   - Điều chỉnh CircleCollider2D radius theo kích thước zone\n\n" +

            $"Vàng: {goldStatus}\n" +
            $"Ngọc Quái Vương: {BOSS_CORE_PATH}\n\n" +

            "Boss sẽ xuất hiện qua 2 cách:\n" +
            "  • Tự động: khi DaysAlive >= 9 (BossSpawnTrigger)\n" +
            "  • Thủ công: player đến gần zone → nhấn F (BossActivationZone)",
            "OK");
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        var parts  = path.Split('/');
        var parent = string.Join("/", parts, 0, parts.Length - 1);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, parts[parts.Length - 1]);
    }
}
