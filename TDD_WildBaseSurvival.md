# TECHNICAL DESIGN DOCUMENT — WILD BASE SURVIVAL
**Ngày tạo:** 12/6/2026  
**Engine:** Unity 2D (URP) | **Asset:** Tiny Swords (top-down pixel art)  
**Phiên bản:** 1.0

---

## 1. TỔNG QUAN KIẾN TRÚC

### 1.1 Mô hình hệ thống
```
GameManager (Singleton, DontDestroyOnLoad)
├── TimeSystem       — đồng hồ game, ngày/đêm, đếm ngày sống sót
├── StatSystem       — HP, Đói, Mệt của player
├── InventorySystem  — túi đồ, hotbar
├── CraftingSystem   — công thức chế tạo (data-driven)
├── BuildingSystem   — đặt công trình, ghost preview
├── CombatSystem     — hitbox/hurtbox, sát thương, knockback
├── SpawnerSystem    — sinh quái theo ngày và khu vực
├── SaveSystem       — JSON serialize/deserialize
└── AudioManager     — nhạc nền + SFX pool
```

### 1.2 Giao tiếp giữa hệ thống — Event Bus
Static C# events, không dùng UnityEvent để tránh coupling:
```csharp
public static class GameEvents {
    public static Action OnPlayerDied;
    public static Action<int> OnDayChanged;
    public static Action<bool> OnDayCycleChanged; // true = ban ngày
    public static Action<Item, int> OnItemPickedUp;
    public static Action OnMainHouseDestroyed;
    public static Action OnBossDefeated;
}
```

---

## 2. CẤU TRÚC THƯ MỤC SCRIPTS

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── GameEvents.cs
│   └── GameState.cs
├── Player/
│   ├── PlayerController.cs     — input, di chuyển 8 hướng, animation
│   ├── PlayerStats.cs          — HP, Đói, Mệt; logic giảm theo thời gian
│   └── PlayerCombat.cs         — tấn công bằng vũ khí hiện tại, i-frame
├── Stats/
│   ├── StatDefinition.cs       — ScriptableObject
│   └── StatBar.cs
├── Time/
│   ├── TimeSystem.cs           — 1 ngày real = 8 phút
│   └── DayNightController.cs
├── Inventory/
│   ├── ItemData.cs             — ScriptableObject
│   ├── Inventory.cs
│   ├── Hotbar.cs
│   └── ItemSlot.cs
├── Crafting/
│   ├── RecipeData.cs           — ScriptableObject
│   └── CraftingSystem.cs
├── Resources/
│   ├── HarvestableResource.cs
│   ├── Tree.cs
│   ├── Rock.cs
│   └── GoldVein.cs
├── Building/
│   ├── BuildingData.cs         — ScriptableObject
│   ├── PlacementSystem.cs
│   ├── Building.cs
│   ├── MainHouse.cs
│   ├── StorageChest.cs
│   ├── Forge.cs
│   ├── RestArea.cs
│   ├── Campfire.cs
│   └── Wall.cs
├── Combat/
│   ├── WeaponData.cs           — ScriptableObject
│   ├── Hitbox.cs
│   ├── Hurtbox.cs
│   ├── DamageNumber.cs
│   └── Projectile.cs
├── Monsters/
│   ├── MonsterData.cs          — ScriptableObject
│   ├── MonsterBase.cs
│   ├── MonsterAI.cs            — FSM: Patrol→Chase→Attack→Return
│   ├── GoblinMonster.cs
│   ├── ZombieMonster.cs
│   ├── EliteMonster.cs
│   ├── BossMonster.cs
│   └── MonsterSpawner.cs
├── WaveConfig/
│   └── WaveConfigData.cs       — ScriptableObject
├── Animals/
│   ├── AnimalBase.cs
│   └── ChickenAnimal.cs
├── UI/
│   ├── HUDManager.cs
│   ├── InventoryUI.cs
│   ├── HotbarUI.cs
│   ├── CraftingUI.cs
│   ├── BuildingMenuUI.cs
│   ├── PauseMenuUI.cs
│   ├── MainMenuUI.cs
│   ├── GameOverUI.cs
│   ├── VictoryUI.cs
│   ├── MilestoneUI.cs
│   └── TutorialUI.cs
├── Save/
│   ├── SaveData.cs
│   └── SaveSystem.cs
├── Audio/
│   ├── AudioManager.cs
│   └── SFXLibrary.cs
└── Utilities/
    ├── ObjectPool.cs
    └── DebugPanel.cs
```

---

## 3. DATA STRUCTURES (SCRIPTABLEOBJECTS)

### ItemData
```csharp
public enum ItemType { Material, Tool, Weapon, Consumable, Equipment }
// itemId, displayName, icon, type, maxStack
// Consumable: hpRestore, hungerRestore, fatigueRestore
```

### RecipeData
```csharp
public enum WorkbenchType { Hand, Campfire, Forge, WorkbenchTable }
// requiredBench, Ingredient[] inputs, ItemData output, int outputCount
```

### StatDefinition
```csharp
// statName, maxValue, decayPerSecond, warningThreshold (0-1), warningColor
```

### WaveConfigData
```csharp
// WaveEntry[]: dayStart, dayEnd, MonsterData[], baseCount, scalePerDay
// waveIntervalNights: tấn công base mỗi X đêm
```

---

## 4. SCENE STRUCTURE

```
MainMenu (Scene)
GameScene (Scene)
├── [Persistent] — GameManager, AudioManager, SaveSystem (DontDestroyOnLoad)
├── Map
│   ├── Tilemap_Ground     (cỏ, đất, đường)
│   ├── Tilemap_Obstacles  (nước, đá nền) — TilemapCollider2D
│   └── Tilemap_Decoration
├── Resources              — Trees, Rocks, GoldVeins
├── Animals
├── Player                 — PlayerController, PlayerStats, PlayerCombat
│   └── WeaponHolder       — child, swap sprite theo vũ khí
├── BaseBuildings
├── Spawners               — MonsterSpawner (1 per zone)
├── Cameras
│   ├── MainCamera         — PixelPerfectCamera, URP 2D Light, follows Player
│   └── UICamera
└── Canvas                 — HUD, InventoryUI, CraftingUI...
```

---

## 5. SORTING LAYERS

| Layer | Order | Dùng cho |
|-------|-------|----------|
| Ground | 0 | Tilemap nền |
| Resources | 1 | Cây, đá |
| Animals | 2 | Thú nhỏ |
| Monsters | 3 | Quái |
| Player | 4 | Player |
| Buildings | 5 | Công trình |
| Projectiles | 6 | Mũi tên |
| Effects | 7 | Particle, số damage |
| HUD | 8 | UI overlay |

---

## 6. PHYSICS LAYERS (COLLISION MATRIX)

| | Player | Monster | PlayerWeapon | MonsterWeapon | Resource | Building |
|---|---|---|---|---|---|---|
| Player | ✗ | ✗ | — | ✓ | ✗ | ✓ |
| Monster | ✗ | ✗ | ✓ | — | ✗ | ✓ |
| PlayerWeapon | — | ✓ | ✗ | ✗ | ✓ | ✗ |
| MonsterWeapon | ✓ | ✗ | ✗ | ✗ | ✗ | ✓ |

---

## 7. GAME FLOW STATE MACHINE

```
MainMenu → Playing ↔ Paused
Playing → GameOver  (nhà chính bị phá / chết hẳn)
Playing → Victory   (hạ boss + nhà chính còn)
GameOver → Playing  (New Game)
```

---

## 8. SAVE DATA (JSON)

```json
{
  "daysAlive": 5,
  "currentTime": 0.35,
  "player": { "posX": 12.5, "posY": -8.0, "hp": 80, "hunger": 60, "fatigue": 45 },
  "inventory": [{ "itemId": "wood", "count": 24 }],
  "chest": [],
  "buildings": [{ "type": "MainHouse", "posX": 0, "posY": 0, "hp": 200, "tier": 1 }]
}
```
Lưu tại: `Application.persistentDataPath/save.json`

---

## 9. QUYẾT ĐỊNH THIẾT KẾ

| Quyết định | Lý do |
|---|---|
| ScriptableObject cho tất cả data | Balance không cần rebuild |
| Event Bus (static C# Action) | Loose coupling, tránh dependency hell |
| Object Pool cho quái + projectile | Tránh GC spike |
| Single GameScene + DontDestroyOnLoad | Tránh load time |
| JSON save | Dễ debug, dễ mở rộng |
| Pixel Perfect Camera URP | Pixel art không bị blurring |

---

## 10. CODE CŨ — KẾ THỪA / XÓA

| File cũ | Hành động |
|---|---|
| `Scripts/Unit/Units.cs` | Xóa — thay bằng `MonsterBase.cs` + `PlayerStats.cs` |
| `Scripts/Unit/Ally/` toàn bộ | Xóa — không còn Ally AI |
| `Scripts/Unit/Enemy/Enemy.cs` | Tham khảo → `MonsterAI.cs` |
| `Scripts/Unit/Enemy/` cụ thể | Xóa — thay bằng Monster scripts mới |
| `Scripts/Weapon/Weapon.cs` | Tham khảo → `WeaponData.cs` |
| `Scripts/Weapon/Arrow.cs` | Tham khảo → `Projectile.cs` |
| `Scripts/AddButt.cs`, `Reward.cs`, `Interface.cs` | Xóa |

---

## 11. MILESTONE IN-GAME

| Mục tiêu | Điều kiện |
|---|---|
| Sống 3 ngày | Hoàn thành nhà chính |
| Sống 5 ngày | Rèn được kiếm sắt |
| Sống 7 ngày | Có đủ tường phòng thủ |
| Sống 10 ngày | Chạm mặt boss |
| Hạ boss | THẮNG |
