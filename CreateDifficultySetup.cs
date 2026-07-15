using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Difficulty System (Day 34)
/// - Tạo DifficultyConfig.asset với 4 tier nhịp GDD mục 16
/// - Thêm DifficultyManager vào scene
/// </summary>
public static class CreateDifficultySetup
{
    const string CONFIG_PATH = "Assets/Data/DifficultyConfig.asset";

    [MenuItem("Tools/WBS/Setup Difficulty System (Day 34)")]
    static void Run()
    {
        // ── 1. Tạo hoặc load DifficultyConfig ───────────────────────────────
        var config = AssetDatabase.LoadAssetAtPath<DifficultyConfig>(CONFIG_PATH);
        bool isNew = config == null;
        if (isNew)
        {
            config = ScriptableObject.CreateInstance<DifficultyConfig>();
            EnsureFolder("Assets/Data");
            AssetDatabase.CreateAsset(config, CONFIG_PATH);
        }

        // ── 2. Điền entries nhịp GDD ─────────────────────────────────────────
        // Ngày 1–2: yên tĩnh (ít quái, chỉ số bình thường)
        // Ngày 3–5: đông dần (count tăng, chỉ số bình thường)
        // Ngày 6–10: elite (count cao hơn, quái mạnh hơn)
        // Ngày 11+:  boss era (rất đông, rất mạnh)
        config.entries = new DifficultyConfig.DayEntry[]
        {
            new DifficultyConfig.DayEntry   // Tier 0: Ngày 1–2 yên tĩnh
            {
                fromDay          = 0,
                hpMultiplier     = 1.0f,
                dmgMultiplier    = 1.0f,
                countMultiplier  = 0.5f,    // chỉ 50% max quái
                enemyPrefabOverride = null,
                raidPrefabOverride  = null,
            },
            new DifficultyConfig.DayEntry   // Tier 1: Ngày 3–5 đông dần
            {
                fromDay          = 3,
                hpMultiplier     = 1.0f,
                dmgMultiplier    = 1.0f,
                countMultiplier  = 1.0f,
                enemyPrefabOverride = null,
                raidPrefabOverride  = null,
            },
            new DifficultyConfig.DayEntry   // Tier 2: Ngày 6–10 elite
            {
                fromDay          = 6,
                hpMultiplier     = 1.35f,   // +35% máu
                dmgMultiplier    = 1.25f,   // +25% sát thương
                countMultiplier  = 1.25f,
                enemyPrefabOverride = null, // điền prefab elite thủ công trong Inspector nếu muốn
                raidPrefabOverride  = null,
            },
            new DifficultyConfig.DayEntry   // Tier 3: Ngày 11+ boss era
            {
                fromDay          = 11,
                hpMultiplier     = 1.75f,   // +75% máu
                dmgMultiplier    = 1.50f,   // +50% sát thương
                countMultiplier  = 1.5f,
                enemyPrefabOverride = null,
                raidPrefabOverride  = null,
            },
        };

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(isNew
            ? $"[WBS] DifficultyConfig tạo mới tại {CONFIG_PATH} ✓"
            : $"[WBS] DifficultyConfig đã cập nhật tại {CONFIG_PATH} ✓");

        // ── 3. Thêm DifficultyManager vào scene nếu chưa có ─────────────────
        var existing = Object.FindFirstObjectByType<DifficultyManager>();
        if (existing == null)
        {
            var go = new GameObject("DifficultyManager");
            var dm = go.AddComponent<DifficultyManager>();
            // Gán config qua serialized property
            var so   = new SerializedObject(dm);
            var prop = so.FindProperty("config");
            prop.objectReferenceValue = config;
            so.ApplyModifiedProperties();
            Undo.RegisterCreatedObjectUndo(go, "Create DifficultyManager");
            Debug.Log("[WBS] DifficultyManager tạo trong scene, config đã gán ✓");
        }
        else
        {
            Debug.Log("[WBS] DifficultyManager đã có trong scene, bỏ qua.");
        }

        EditorUtility.DisplayDialog(
            "WBS — Day 34 Difficulty System",
            "Bảng độ khó đã được cài đặt!\n\n" +
            "Nhịp GDD mục 16:\n" +
            "• Ngày 0–2:  count ×0.5  (yên tĩnh)\n" +
            "• Ngày 3–5:  count ×1.0  (đông dần)\n" +
            "• Ngày 6–10: HP ×1.35 | DMG ×1.25 | count ×1.25  (elite)\n" +
            "• Ngày 11+:  HP ×1.75 | DMG ×1.50 | count ×1.50  (boss era)\n\n" +
            "Tuỳ chỉnh:\n" +
            "Chọn Assets/Data/DifficultyConfig.asset trong Inspector\n" +
            "để gán prefab override (enemyPrefabOverride / raidPrefabOverride)\n" +
            "cho từng tier nếu muốn đổi loại quái theo ngày.",
            "OK");
    }

    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            var parts  = path.Split('/');
            var parent = string.Join("/", parts, 0, parts.Length - 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, parts[parts.Length - 1]);
        }
    }
}