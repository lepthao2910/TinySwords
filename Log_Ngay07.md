# LOG NGÀY 7 (22/6/2026) — Build #1 Review & Bug Fix

## Trạng thái tuần 1 (ngày 1–6)

| Hệ thống | Script | Trạng thái |
|---|---|---|
| GameManager / State | `Core/GameManager.cs` | ✅ |
| Player movement (WASD + click) | `Player/PlayerController.cs` | ✅ |
| Player stats (HP/Đói/Mệt) | `Player/PlayerStats.cs` | ✅ |
| Stat bars + warning pulse | `Stats/StatBar.cs` | ✅ |
| HUD (ngày, giờ, icon phase) | `UI/HUDManager.cs` | ✅ |
| Debug panel (F1) | `Utilities/DebugPanel.cs` | ✅ |
| Đồng hồ game (8 phút/ngày) | `Time/TimeSystem.cs` | ✅ |
| Chu kỳ ngày/đêm lighting | `Time/DayNightController.cs` | ✅ |
| Camera theo player | `Map/CameraFollow.cs` | ✅ |
| Map bounds + clamp | `Map/MapBounds.cs` | ✅ |
| Cursor tuỳ chỉnh | `UI/GameCursor.cs` | ✅ |
| Click indicator | `UI/ClickIndicator.cs` | ✅ |
| Resource object (cây/đá) | `Resources/ResourceObject.cs` | ✅ |

## Bugs đã fix

### Bug 1 — CRITICAL: Ngày/đêm không chạy (TimeSystem không tick)
**Nguyên nhân:** `GameManager` mặc định ở `MainMenu`; `TimeSystem.Update()` bỏ qua nếu state ≠ `Playing`.

**Fix:** Thêm `autoStartOnAwake = true` vào `GameManager`. Trong `Awake()`, nếu flag bật → gọi `StartGame()` → chuyển sang `Playing`.

```csharp
[SerializeField] bool autoStartOnAwake = true;

void Awake()
{
    // ...singleton...
    if (autoStartOnAwake && CurrentState == GameState.MainMenu)
        StartGame();
}
```

> Tắt flag này khi thêm Main Menu UI (Day 39).

---

### Bug 2 — MEDIUM: DebugPanel F1 không hoạt động với New Input System
**Nguyên nhân:** Dùng `Input.GetKeyDown` (legacy API) — không tương thích khi "Active Input Handling" = New Input System only.

**Fix:** Đổi sang `Keyboard.current.f1Key.wasPressedThisFrame`.

```csharp
using UnityEngine.InputSystem;
// ...
if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
    isVisible = !isVisible;
```

---

### Bug 3 — MINOR: NullReferenceException nếu Camera.main == null
**Nguyên nhân:** `PlayerController.ScreenToWorld()` gọi `Camera.main.ScreenToWorldPoint()` không null-check.

**Fix:** Guard `if (Camera.main == null) return Vector2.zero;`

---

## Code cleanup
- Xóa 2 GUIStyle field không dùng (`labelStyle`, `sliderStyle`) trong DebugPanel
- Xóa `[SerializeField] KeyCode toggleKey` (thay bằng hardcode F1 via New IS)
- Xóa `/// <summary>` task-reference trong ResourceObject ("CHƯA có tương tác Day 10/11")
- Xóa `// Placeholder` + `// TODO` trong `TryInteractAtMouse()` — bối cảnh thuộc PR, không thuộc code

## Không phải bug (note cân bằng)

- `SecondsUntilPhaseEnd()` tính sai khi `TimeOfDay` vượt nửa đêm trong ban đêm → chưa có code gọi hàm này, bỏ qua.
- `MapBounds` default `(-12,-8)→(12,8)` quá nhỏ so với map 100×100 tile (64×64 WU) → chỉnh trong Inspector sau khi load Level 1 prefab vào scene.

## Cần làm thủ công trong Unity Editor trước khi test

1. Chạy **Tools → WBS → Setup GameSystems Only** để tạo `Global Light 2D` + gán `DayNightController`
2. Play → kiểm tra:
   - Ngày đếm lên sau 8 phút thực (`DaysAlive` trong DebugPanel F1)
   - Ánh sáng đổi màu theo keyframe (sáng → trưa → hoàng hôn → đêm)
   - Cam theo player, chuột phải → di chuyển, ClickIndicator hiện/tắt
   - 3 thanh stat giảm; đói → trừ HP; chết → GameOver state

## TODO ngày 8
- Bắt đầu Inventory System (Day 8: data + logic item ScriptableObject, stack, thêm/bớt)
