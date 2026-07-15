using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Tools → WBS → Setup Gnoll (Day 27)
/// Tạo BoneProjectile prefab, animation clips, AnimatorController và GnollEnemy prefab.
/// </summary>
public static class CreateGnoll
{
    const string GNOLL_PNG  = "Assets/Asset/Enemy Pack/Enemies/Gnoll";
    const string ANIM_OUT   = "Assets/Animation/Monsters";
    const string PREFAB_OUT = "Assets/Prefabs/Monsters";
    const string ITEM_FOLDER = "Assets/Data/Items";

    [MenuItem("Tools/WBS/Setup Gnoll (Day 27)")]
    static void Run()
    {
        // ── 1. Tạo Item_GnollBone nếu chưa có ─────────────────────────────────
        CreateGnollBoneItem();

        // ── 2. Load sprites ────────────────────────────────────────────────────
        var idleSprites  = LoadSprites($"{GNOLL_PNG}/Gnoll_Idle.png");
        var walkSprites  = LoadSprites($"{GNOLL_PNG}/Gnoll_Walk.png");
        var throwSprites = LoadSprites($"{GNOLL_PNG}/Gnoll_Throw.png");

        if (idleSprites.Length == 0 || walkSprites.Length == 0 || throwSprites.Length == 0)
        {
            EditorUtility.DisplayDialog("WBS Error",
                "Không tải được Gnoll sprites. Kiểm tra đường dẫn:\n" + GNOLL_PNG, "OK");
            return;
        }

        // ── 3. Tạo animation clips ─────────────────────────────────────────────
        string idlePath  = $"{ANIM_OUT}/Gnoll_Idle.anim";
        string walkPath  = $"{ANIM_OUT}/Gnoll_Walk.anim";
        string throwPath = $"{ANIM_OUT}/Gnoll_Throw.anim";

        SaveClip(idlePath,  MakeClip(idleSprites,  fps: 8f,  loop: true));
        SaveClip(walkPath,  MakeClip(walkSprites,  fps: 10f, loop: true));
        SaveClip(throwPath, MakeClip(throwSprites, fps: 10f, loop: false));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // ── 4. Tạo AnimatorController ──────────────────────────────────────────
        string ctrlPath = $"{ANIM_OUT}/Gnoll.controller";
        var controller  = AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrlPath);
        if (controller == null)
            controller = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);

        BuildController(controller,
            AssetDatabase.LoadAssetAtPath<AnimationClip>(idlePath),
            AssetDatabase.LoadAssetAtPath<AnimationClip>(walkPath),
            AssetDatabase.LoadAssetAtPath<AnimationClip>(throwPath));

        AssetDatabase.SaveAssets();

        // ── 5. Tạo BoneProjectile prefab ──────────────────────────────────────
        string bonePrefabPath = $"{PREFAB_OUT}/BoneProjectile.prefab";
        var bonePrefabExisted = AssetDatabase.LoadAssetAtPath<GameObject>(bonePrefabPath) != null;
        var boneSprites       = LoadSprites($"{GNOLL_PNG}/Gnoll_Bone.png");
        var boneGO            = CreateBoneGO(boneSprites.Length > 0 ? boneSprites[0] : null);
        PrefabUtility.SaveAsPrefabAsset(boneGO, bonePrefabPath);
        Object.DestroyImmediate(boneGO);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var bonePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(bonePrefabPath);

        // ── 6. Tạo GnollEnemy prefab ──────────────────────────────────────────
        var idleSprite0  = idleSprites[0];
        var gnollGO      = CreateGnollGO(controller, idleSprite0, bonePrefab);

        string gnollPrefabPath = $"{PREFAB_OUT}/GnollEnemy.prefab";
        PrefabUtility.SaveAsPrefabAsset(gnollGO, gnollPrefabPath);
        Object.DestroyImmediate(gnollGO);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[WBS] ✓ Gnoll setup xong!");
        EditorUtility.DisplayDialog("WBS — Day 27",
            "GnollEnemy.prefab và BoneProjectile.prefab đã được tạo.\n\n" +
            "Các bước tiếp theo:\n" +
            "1. Thêm EnemySpawner vào scene (GameObject trống)\n" +
            "2. Gán SpearGoblin hoặc GnollEnemy vào Enemy Prefabs\n" +
            "3. Cân chỉnh Spawn Radius, Base Max Alive, Max Alive Per Day\n" +
            "4. Gán Drop Items (Item_GnollBone) cho GnollEnemy nếu cần",
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
        AnimationClip idle, AnimationClip walk, AnimationClip throwClip)
    {
        var sm = ctrl.layers[0].stateMachine;
        foreach (var s in sm.states.ToArray())
            sm.RemoveState(s.state);

        if (ctrl.parameters.All(p => p.name != "Speed"))
            ctrl.AddParameter("Speed", AnimatorControllerParameterType.Float);
        if (ctrl.parameters.All(p => p.name != "Atk"))
            ctrl.AddParameter("Atk", AnimatorControllerParameterType.Trigger);

        var idleState  = sm.AddState("Gnoll_Idle");
        var walkState  = sm.AddState("Gnoll_Walk");
        var throwState = sm.AddState("Gnoll_Throw");

        idleState.motion  = idle;
        walkState.motion  = walk;
        throwState.motion = throwClip;
        sm.defaultState   = idleState;

        // Idle ↔ Walk
        var t = idleState.AddTransition(walkState);
        t.hasExitTime = false; t.duration = 0.05f;
        t.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");

        t = walkState.AddTransition(idleState);
        t.hasExitTime = false; t.duration = 0.05f;
        t.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");

        // Any → Throw (Atk trigger)
        var anyT = sm.AddAnyStateTransition(throwState);
        anyT.hasExitTime        = false;
        anyT.duration           = 0f;
        anyT.canTransitionToSelf = false;
        anyT.AddCondition(AnimatorConditionMode.If, 0f, "Atk");

        // Throw → Idle (exit time)
        t = throwState.AddTransition(idleState);
        t.hasExitTime = true; t.exitTime = 1f; t.duration = 0f;

        EditorUtility.SetDirty(ctrl);
    }

    static GameObject CreateBoneGO(Sprite boneSprite)
    {
        var go = new GameObject("BoneProjectile");

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CircleCollider2D>();
        col.radius    = 0.18f;
        col.isTrigger = true;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = boneSprite;
        sr.sortingLayerName = "Monsters";

        go.AddComponent<BoneProjectile>();
        return go;
    }

    static GameObject CreateGnollGO(AnimatorController ctrl, Sprite defaultSprite, GameObject bonePrefab)
    {
        var go = new GameObject("GnollEnemy");

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;

        var col = go.AddComponent<CapsuleCollider2D>();
        col.size   = new Vector2(0.55f, 0.85f);
        col.offset = new Vector2(0f, 0.05f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = defaultSprite;
        sr.sortingLayerName = "Monsters";

        var anim = go.AddComponent<Animator>();
        anim.runtimeAnimatorController = ctrl;

        var enemy = go.AddComponent<GnollEnemy>();

        var so = new SerializedObject(enemy);
        // Balance pass #1 (Day 28)
        so.FindProperty("maxHP").floatValue              = 28f;   // 25 → 28
        so.FindProperty("moveSpeed").floatValue          = 2.0f;
        so.FindProperty("attackDamage").floatValue       = 7f;    // 5 → 7
        so.FindProperty("attackRange").floatValue        = 3.5f;  // tầm ném
        so.FindProperty("chaseRange").floatValue         = 6.5f;  // 6 → 6.5 (tầm phát hiện cao hơn)
        so.FindProperty("attackCooldown").floatValue     = 2.0f;  // 2.2 → 2.0
        so.FindProperty("knockbackOnHit").floatValue     = 3f;
        so.FindProperty("iFrameDuration").floatValue     = 0.5f;
        so.FindProperty("knockbackDur").floatValue       = 0.12f;
        so.FindProperty("enablePatrol").boolValue        = true;
        so.FindProperty("patrolRadius").floatValue       = 3.5f;
        so.FindProperty("patrolWaitTime").floatValue     = 2.5f;
        so.FindProperty("loseChaseMultiplier").floatValue = 1.5f;
        so.FindProperty("attackTriggerName").stringValue  = "Atk";
        so.FindProperty("bonePrefab").objectReferenceValue = bonePrefab;
        so.FindProperty("boneSpeed").floatValue            = 8f;
        so.FindProperty("boneRange").floatValue            = 7f;

        // Drop items: Item_GnollBone (1 cái)
        var gnollBone = AssetDatabase.LoadAssetAtPath<ItemData>($"{ITEM_FOLDER}/Item_GnollBone.asset");
        if (gnollBone != null)
        {
            so.FindProperty("dropItems").arraySize  = 1;
            so.FindProperty("dropCounts").arraySize = 1;
            so.FindProperty("dropItems").GetArrayElementAtIndex(0).objectReferenceValue = gnollBone;
            so.FindProperty("dropCounts").GetArrayElementAtIndex(0).intValue            = 1;
        }

        so.ApplyModifiedPropertiesWithoutUndo();

        return go;
    }

    static void CreateGnollBoneItem()
    {
        string path = $"{ITEM_FOLDER}/Item_GnollBone.asset";
        if (AssetDatabase.LoadAssetAtPath<ItemData>(path) != null) return;

        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId      = "gnoll_bone";
        item.displayName = "Xương Gnoll";
        item.description = "Xương ném của Gnoll. Có thể dùng để chế tạo.";
        item.itemType    = ItemType.Resource;
        item.maxStack    = 20;
        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();
        Debug.Log("[WBS] Tạo Item_GnollBone.asset ✓");
    }
}