# LOG NGÀY 4 (15/6/2026) — HUD Cơ Bản

## Kiểm tra trạng thái Editor
- Player.prefab: StatDefs (HP/Hunger/Fatigue) đã được linked ✅
- Player.prefab: PlayerInput gắn đúng action map "Player" ✅
- Player.prefab: **BUG** gravityScale=1, Constraints=0 → đã fix sang gravityScale=0, Constraints=FreezeRotation(4), Interpolation=1
- SampleScene: vẫn còn objects cũ (Ally, Enemy, AddAllyButt, AddEnemyButt, Camera cũ, StartPoint, Text(TMP), UI container) → đã xóa 35 YAML blocks

## Objects còn lại trong SampleScene (sau cleanup)
- Main Camera, Canvas, EventSystem, GameSystems, Global Light 2D, Image

## Scripts tạo mới

### `Assets/Scripts/UI/HUDManager.cs`
- Lắng nghe `OnDayChanged`, `OnDayCycleChanged`
- Hiển thị "Ngày N" và "HH:MM" real-time qua TimeSystem
- Slot cho day/night icon GameObjects (optional)
- SerializeField: `dayLabel`, `timeLabel`, `dayIcon`, `nightIcon`

### `Assets/Scripts/Stats/StatBar.cs` (updated)
- Thêm pulse effect: sine wave trên alpha khi inWarning state
- Thêm optional `TextMeshProUGUI valueLabel` hiển thị số
- `pulseSpeed = 3f`, `pulseMinAlpha = 0.45f`
- Smooth transition khi vào/ra warning state (reset pulseTimer)

### `Assets/Editor/WBSSceneSetup.cs`
- Menu: **Tools > Wild Base Survival > Setup Scene (Full)**
- Tạo GameSystems (GameManager + TimeSystem + DebugPanel)
- Instantiate Player prefab (hoặc tạo mới nếu không có prefab)
- Tạo HUDCanvas với:
  - 3 StatBars (HP đỏ, Đói cam, Mệt xanh dương) — top-left
  - Day/Time labels (TMP) — top-right
  - Background semi-transparent cho mỗi bar
- Tự động gán references qua SerializedObject (không cần drag-drop thủ công)
- Menu phụ: Setup HUD Only / Setup GameSystems Only

## Cách dùng trong Unity Editor
1. Mở SampleScene
2. Menu **Tools > Wild Base Survival > Setup Scene (Full)**
3. Nhấn OK → toàn bộ HUD + GameSystems được tạo tự động
4. Gán `StatDef_HP/Hunger/Fatigue.asset` vào field `Stat Def` trên từng StatBar (optional, có fallback)

## TODO ngày 5 (map test + tilemap)
- Tạo Tilemap 3 lớp (Ground, Obstacles, Decoration)
- Va chạm Tilemap (Obstacles layer)
- Giới hạn camera/player theo map bounds
- Đặt cây và đá placeholder (chưa tương tác)
