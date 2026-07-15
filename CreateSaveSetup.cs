using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Save System (Day 39)
/// - Tạo SaveManager trong scene
/// - Thêm MainMenuUI vào GameManager (hoặc tạo riêng)
/// - Tắt autoStartOnAwake trên GameManager (bật Main Menu)
/// </summary>
public static class CreateSaveSetup
{
    [MenuItem("Tools/WBS/Setup Save System (Day 39)")]
    static void Run()
    {
        // ── 1. SaveManager ────────────────────────────────────────────────────
        var existingSM = Object.FindFirstObjectByType<SaveManager>();
        if (existingSM == null)
        {
            var go = new GameObject("SaveManager");
            go.AddComponent<SaveManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create SaveManager");
            Debug.Log("[WBS] SaveManager tạo trong scene ✓");
        }
        else
        {
            Debug.Log("[WBS] SaveManager đã có, bỏ qua.");
        }

        // ── 2. MainMenuUI ─────────────────────────────────────────────────────
        var existingMenu = Object.FindFirstObjectByType<MainMenuUI>();
        if (existingMenu == null)
        {
            var gm = Object.FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.gameObject.AddComponent<MainMenuUI>();
                Debug.Log("[WBS] MainMenuUI gắn vào GameManager ✓");
            }
            else
            {
                var go = new GameObject("MainMenuUI");
                go.AddComponent<MainMenuUI>();
                Undo.RegisterCreatedObjectUndo(go, "Create MainMenuUI");
                Debug.Log("[WBS] MainMenuUI tạo riêng (không tìm thấy GameManager) ✓");
            }
        }
        else
        {
            Debug.Log("[WBS] MainMenuUI đã có, bỏ qua.");
        }

        // ── 3. Tắt autoStartOnAwake trên GameManager ──────────────────────────
        var gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            var so = new SerializedObject(gameManager);
            var prop = so.FindProperty("autoStartOnAwake");
            if (prop != null && prop.boolValue)
            {
                prop.boolValue = false;
                so.ApplyModifiedProperties();
                Debug.Log("[WBS] GameManager.autoStartOnAwake = false ✓ (Main Menu sẽ hiện khi chạy game)");
            }
            else
            {
                Debug.Log("[WBS] autoStartOnAwake đã là false hoặc không tìm thấy.");
            }
        }

        // ── 4. Hướng dẫn ─────────────────────────────────────────────────────
        EditorUtility.DisplayDialog(
            "WBS — Day 39 Save System",
            "Hệ thống Save/Load đã được cài đặt!\n\n" +

            "─── Việc cần làm ───\n\n" +

            "1. Gán catalogs vào SaveManager (Inspector):\n" +
            "   Item Catalog   → kéo TẤT CẢ ItemData assets vào\n" +
            "   Building Catalog → kéo TẤT CẢ BuildingData assets vào\n\n" +

            "2. Thêm scene vào Build Settings:\n" +
            "   File → Build Settings → Add Open Scenes\n" +
            "   (cần thiết để SceneManager.LoadScene hoạt động trong build)\n\n" +

            "─── Tính năng ───\n\n" +
            "• Main Menu: New Game / Tiếp tục / Thoát\n" +
            "• Auto-save khi qua ngày mới\n" +
            "• Lưu: vị trí, HP/Đói/Mệt, túi đồ, công trình + rương\n" +
            "• Nhấn Enter ở Main Menu để vào game nhanh\n\n" +

            "Debug: GameManager.autoStartOnAwake = true → bỏ qua menu (chỉ khi dev)",
            "OK");
    }
}
