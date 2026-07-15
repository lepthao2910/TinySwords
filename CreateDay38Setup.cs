using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Win/Lose Screens (Day 38)
/// - Thêm VictoryScreen vào GameManager (hoặc tạo riêng)
/// - Kiểm tra GameOverScreen đã có chưa
/// </summary>
public static class CreateDay38Setup
{
    [MenuItem("Tools/WBS/Setup Win-Lose Screens (Day 38)")]
    static void Run()
    {
        // ── VictoryScreen ──────────────────────────────────────────────────────
        var existingVS = Object.FindFirstObjectByType<VictoryScreen>();
        if (existingVS == null)
        {
            var gm = Object.FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.gameObject.AddComponent<VictoryScreen>();
                Debug.Log("[WBS] VictoryScreen gắn vào GameManager ✓");
            }
            else
            {
                var go = new GameObject("VictoryScreen");
                go.AddComponent<VictoryScreen>();
                Undo.RegisterCreatedObjectUndo(go, "Create VictoryScreen");
                Debug.Log("[WBS] VictoryScreen tạo riêng (không tìm thấy GameManager) ✓");
            }
        }
        else
        {
            Debug.Log("[WBS] VictoryScreen đã có, bỏ qua.");
        }

        // ── GameOverScreen ─────────────────────────────────────────────────────
        var existingGO = Object.FindFirstObjectByType<GameOverScreen>();
        if (existingGO == null)
        {
            var gm = Object.FindFirstObjectByType<GameManager>();
            GameObject target = gm != null ? gm.gameObject : new GameObject("GameOverScreen");
            target.AddComponent<GameOverScreen>();
            if (gm == null) Undo.RegisterCreatedObjectUndo(target, "Create GameOverScreen");
            Debug.Log("[WBS] GameOverScreen tạo ✓");
        }
        else
        {
            Debug.Log("[WBS] GameOverScreen đã có, bỏ qua.");
        }

        EditorUtility.DisplayDialog(
            "WBS — Day 38 Win/Lose Screens",
            "Màn hình thắng/thua đã được cài đặt!\n\n" +

            "─── Điều kiện thắng ───\n" +
            "Boss bị hạ (OnBossDefeated) → GameState.Victory → VictoryScreen hiện\n\n" +

            "─── Điều kiện thua ───\n" +
            "• Player chết + không có MainHouse → GameState.GameOver\n" +
            "• MainHouse bị phá → GameState.GameOver\n\n" +

            "─── Thống kê hiển thị ───\n" +
            "• Ngày sống sót  (TimeSystem.DaysAlive)\n" +
            "• Kẻ địch tiêu diệt  (RunStats.EnemiesKilled)\n" +
            "• Vật phẩm trong túi  (RunStats.TotalItemsHeld)\n\n" +

            "─── Bug đã sửa ───\n" +
            "GameManager.HandlePlayerDied giờ bỏ qua khi MainHouse còn — player hồi sinh đúng cách.\n\n" +

            "─── Phím tắt ───\n" +
            "R: Thử lại / Chơi lại từ VictoryScreen và GameOverScreen",
            "OK");
    }
}
