using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Lizard Elite (Day 29)
/// Tạo animation clips, AnimatorController và prefab cho Lizard Elite (elite với kỹ năng lao tới).
/// Elite canh giữ mỏ vàng, rơi Item_Gold + Item_LizardScale.
/// </summary>
public static class CreateLizardElite
{
    const string LIZARD_PNG = "Assets/Asset/Enemy Pack/Enemies/Caveborn/Lizard";
    const string ANIM_OUT   = "Assets/Animation/Monsters";
    const string PREFAB_OUT = "Assets/Prefabs/Monsters";
    const string ITEM_FOLDER = "Assets/Data/Items";

    [MenuItem("Tools/WBS/Setup Lizard Elite (Day 29)")]
    static void Run()
    {
        // ── 1. Tạo Item_LizardScale nếu chưa có ───────────────────────────────
        CreateLizardScaleItem();

        // ── 2. Load sprites ────────────────────────────────────────────────────
        var idleSprites   = LoadSprites($"{LIZARD_PNG}/Lizard_Idle.png");
        var runSprites    = LoadSprites($"{LIZARD_PNG}/Lizard_Run.png");
        var attackSprites = LoadSprites($"{LIZARD_PNG}/Lizard_Attack.png");

        if (idleSprites.Length == 0 || runSprites.Length == 0 || attackSprites.Length == 0)
        {
            EditorUtility.DisplayDialog("WBS Error",
                "Không tải được Lizard sprites. Kiểm tra đường dẫn:\n" + LIZARD_PNG, "OK");
            return;
        }

        // ── 3. Tạo animation clips ─────────────────────────────────────────────
        string idlePath   = $"{ANIM_OUT}/LizardElite_Idle.anim";
        string runPath    = $"{ANIM_OUT}/LizardElite_Run.anim";
        string attackPath = $"{ANIM_OUT}/LizardElite_Attack.anim";

        SaveClip(idlePath,   MakeClip(idleSprites,   fps: 8f,  loop: true));
        SaveClip(runPath,    MakeClip(runSprites,    fps: 10f, loop: true));
        SaveClip(attackPath, MakeClip(attackSprites, fps: 10f, loop: false));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // ── 4. Tạo AnimatorController ──────────────────────────────────────────
        string ctrlPath = $"{ANIM_OUT}/LizardElite.controller";
        var controller  = AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrlPath);
        if (controller == null)
            controller = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);

        BuildController(controller,
            AssetDatabase.LoadAssetAtPath<AnimationClip>(idlePath),
            AssetDatabase.LoadAssetAtPath<AnimationClip>(runPath),
            AssetDatabase.LoadAssetAtPath<AnimationClip>(attackPath));

        AssetDatabase.SaveAssets();

        // ── 5. Tạo prefab ──────────────────────────────────────────────────────
        var goldItem  = AssetDatabase.LoadAssetAtPath<ItemData>($"{ITEM_FOLDER}/Item_Gold.asset");
        var scaleItem = AssetDatabase.LoadAssetAtPath<ItemData>($"{ITEM_FOLDER}/Item_LizardScale.asset");

        var go = CreateLizardGO(controller, idleSprites[0], goldItem, scaleItem);
        string prefabPath = $"{PREFAB_OUT}/LizardElite.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[WBS] ✓ Lizard Elite setup xong!");
        EditorUtility.DisplayDialog("WBS — Day 29",
            "LizardElite.prefab đã được tạo.\n\n" +
            "Các bước tiếp theo:\n" +
            "1. Kéo LizardElite.prefab vào scene gần mỏ vàng\n" +
            "2. Elite sẽ tự canh giữ khu vực (Enable Patrol = false)\n" +
            "3. Cân chỉnh Charge Range (cam) và Chase Range (vàng) trong Scene Gizmos\n" +
            "4. Thêm LizardElite vào EnemySpawner nếu muốn spawn theo wave",
            "OK");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static Sprite[] LoadSprites(string path)
        => AssetDatabase.LoadAllAssetsAtPath(path)
            .OfType<Sprite>()
            .OrderBy(s => s.name)
            .ToArray();

    static AnimationClip MakeClip(Sprite[] sprites, float fps, bool loop)
    {
        var clip = new AnimationClip { frameRate = fps };
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var binding = new EditorCurveBinding
        {
            type         = typeof(SpriteRenderer),
            path         = "",
            propertyName = "m_Sprite"
        };
        var keys = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = sprites[i] };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        return clip;
    }

    static void SaveClip(string path, AnimationClip clip)
    {
        var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (existing != null)
        {
            EditorUtility.CopySerialized(clip, existing);
            EditorUtility.SetDirty(existing);
        }
        else
        {
            AssetDatabase.CreateAsset(clip, path);
        }
    }

    static void BuildController(AnimatorController ctrl,
        AnimationClip idle, AnimationClip run, AnimationClip attack)
    {
        var sm = ctrl.layers[0].stateMachine;
        foreach (var s in sm.states.ToArray())
            sm.RemoveState(s.state);

        if (ctrl.parameters.All(p => p.name != "Speed"))
            ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);
        if (ctrl.parameters.All(p => p.name != "Atk"))
            ctrl.AddParameter("Atk", AnimatorControllerParameterType.Trigger);

        var idleState   = sm.AddState("Elite_Idle");
        var runState    = sm.AddState("Elite_Run");
        var attackState = sm.AddState("Elite_Attack");

        idleState.motion   = idle;
        runState.motion    = run;
        attackState.motion = attack;
        sm.defaultState    = idleState;

        // Idle ↔ Run
        var t = idleState.AddTransition(runState);
        t.hasExitTime = false; t.duration = 0.05f;
        t.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");

        t = runState.AddTransition(idleState);
        t.hasExitTime = false; t.duration = 0.05f;
        t.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");

        // Any → Attack (Atk trigger)
        var anyT = sm.AddAnyStateTransition(attackState);
        anyT.hasExitTime        = false;
        anyT.duration           = 0f;
        anyT.canTransitionToSelf = false;
        anyT.AddCondition(AnimatorConditionMode.If, 0f, "Atk");

        // Attack → Idle (exit time)
        t = attackState.AddTransition(idleState);
        t.hasExitTime = true; t.exitTime = 1f; t.duration = 0f;

        EditorUtility.SetDirty(ctrl);
    }

    static GameObject CreateLizardGO(AnimatorController ctrl, Sprite defaultSprite,
        ItemData goldItem, ItemData scaleItem)
    {
        var go = new GameObject("LizardElite");

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CapsuleCollider2D>();
        col.size   = new Vector2(0.7f, 1.1f);
        col.offset = new Vector2(0f, 0.05f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = defaultSprite;
        sr.sortingLayerName = "Monsters";

        var anim = go.AddComponent<Animator>();
        anim.runtimeAnimatorController = ctrl;

        var enemy = go.AddComponent<EnemyBase>();

        var so = new SerializedObject(enemy);

        // Stats — elite tier: HP cao, damage cao
        so.FindProperty("maxHP").floatValue              = 80f;
        so.FindProperty("moveSpeed").floatValue          = 2.5f;
        so.FindProperty("attackDamage").floatValue       = 18f;
        so.FindProperty("attackRange").floatValue        = 1.1f;
        so.FindProperty("chaseRange").floatValue         = 7f;
        so.FindProperty("attackCooldown").floatValue     = 2.5f;
        so.FindProperty("knockbackOnHit").floatValue     = 5f;
        so.FindProperty("iFrameDuration").floatValue     = 0.6f;
        so.FindProperty("knockbackDur").floatValue       = 0.12f;

        // Patrol — elite canh giữ vị trí cố định (không wander xa)
        so.FindProperty("enablePatrol").boolValue        = false;

        // Charge — kỹ năng đặc biệt
        so.FindProperty("enableCharge").boolValue        = true;
        so.FindProperty("chargeRange").floatValue        = 5.5f;
        so.FindProperty("chargeMinRange").floatValue     = 1.5f;
        so.FindProperty("chargeSpeed").floatValue        = 12f;
        so.FindProperty("chargeDamage").floatValue       = 20f;
        so.FindProperty("chargeKnockback").floatValue    = 7f;
        so.FindProperty("chargeDuration").floatValue     = 0.45f;
        so.FindProperty("chargeCooldown").floatValue     = 6f;

        so.FindProperty("attackTriggerName").stringValue = "Atk";

        // Drop items: vàng (2-3) + vảy (1)
        var dropItemsProp  = so.FindProperty("dropItems");
        var dropCountsProp = so.FindProperty("dropCounts");

        int dropCount = (goldItem != null ? 1 : 0) + (scaleItem != null ? 1 : 0);
        dropItemsProp.arraySize  = dropCount;
        dropCountsProp.arraySize = dropCount;

        int idx = 0;
        if (goldItem != null)
        {
            dropItemsProp.GetArrayElementAtIndex(idx).objectReferenceValue = goldItem;
            dropCountsProp.GetArrayElementAtIndex(idx).intValue            = 2;
            idx++;
        }
        if (scaleItem != null)
        {
            dropItemsProp.GetArrayElementAtIndex(idx).objectReferenceValue = scaleItem;
            dropCountsProp.GetArrayElementAtIndex(idx).intValue            = 1;
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        return go;
    }

    static void CreateLizardScaleItem()
    {
        string path = $"{ITEM_FOLDER}/Item_LizardScale.asset";
        if (AssetDatabase.LoadAssetAtPath<ItemData>(path) != null) return;

        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId      = "lizard_scale";
        item.displayName = "Vảy Thằn Lằn";
        item.description = "Vảy cứng từ Thằn Lằn Elite. Dùng để nâng cấp vũ khí và giáp cao cấp.";
        item.itemType    = ItemType.Resource;
        item.maxStack    = 10;
        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();
        Debug.Log("[WBS] Tạo Item_LizardScale.asset ✓");
    }
}