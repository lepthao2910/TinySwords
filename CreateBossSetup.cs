using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Boss (Day 36)
/// - Thêm BossSpawnTrigger vào scene (vị trí origin)
/// - Thêm BossHPBar vào scene
/// - In hướng dẫn tạo Boss Prefab trong dialog
/// </summary>
public static class CreateBossSetup
{
    [MenuItem("Tools/WBS/Setup Boss (Day 36)")]
    static void Run()
    {
        // ── 1. BossSpawnTrigger ───────────────────────────────────────────────
        var existingTrigger = Object.FindFirstObjectByType<BossSpawnTrigger>();
        if (existingTrigger == null)
        {
            var go = new GameObject("BossSpawnTrigger");
            go.AddComponent<BossSpawnTrigger>();
            Undo.RegisterCreatedObjectUndo(go, "Create BossSpawnTrigger");
            Debug.Log("[WBS] BossSpawnTrigger tạo trong scene ✓  — di chuyển gần nhà chính.");
        }
        else
        {
            Debug.Log("[WBS] BossSpawnTrigger đã có trong scene, bỏ qua.");
        }

        // ── 2. BossHPBar ─────────────────────────────────────────────────────
        var existingBar = Object.FindFirstObjectByType<BossHPBar>();
        if (existingBar == null)
        {
            var go = new GameObject("BossHPBar");
            go.AddComponent<BossHPBar>();
            Undo.RegisterCreatedObjectUndo(go, "Create BossHPBar");
            Debug.Log("[WBS] BossHPBar tạo trong scene ✓");
        }
        else
        {
            Debug.Log("[WBS] BossHPBar đã có trong scene, bỏ qua.");
        }

        // ── 3. Hướng dẫn ─────────────────────────────────────────────────────
        EditorUtility.DisplayDialog(
            "WBS — Day 36 Boss System",
            "Hệ thống Boss đã được cài đặt!\n\n" +

            "─── Việc cần làm ───\n\n" +

            "1. Tạo Boss Prefab:\n" +
            "   a. Tạo GameObject mới, đặt tên 'Boss'\n" +
            "   b. Thêm component BossEnemy\n" +
            "   c. Set các giá trị trong Inspector:\n" +
            "      · maxHP = 200  (hoặc cao hơn)\n" +
            "      · moveSpeed = 1.8\n" +
            "      · attackDamage = 20\n" +
            "      · attackRange = 1.5\n" +
            "      · chaseRange = 10\n" +
            "      · attackCooldown = 2\n" +
            "      · knockbackDur = 0.05  (gần bất tử knockback)\n" +
            "      · aoeDamage = 30, aoeRadius = 3, aoeCooldown = 6\n" +
            "      · summonCooldown = 12, minionCount = 3\n" +
            "   d. Kéo prefab quái thường vào minionPrefabs[]\n" +
            "   e. Lưu thành Prefab trong Assets/Prefabs/Enemies/\n\n" +

            "2. Gán Boss Prefab:\n" +
            "   Chọn BossSpawnTrigger trong Hierarchy\n" +
            "   → kéo Boss Prefab vào Boss Prefab field\n\n" +

            "3. Đặt BossSpawnTrigger:\n" +
            "   Di chuyển đến vị trí muốn boss xuất hiện (gần nhà chính)\n" +
            "   bossSpawnDay = 9  (0-indexed) = hiển thị 'Ngày 10'\n\n" +

            "Boss sẽ tự động xuất hiện khi DaysAlive >= bossSpawnDay.",
            "OK");
    }
}
