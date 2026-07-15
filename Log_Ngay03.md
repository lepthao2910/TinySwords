# LOG NGÀY 3 (14/6/2026) — Hệ thống chỉ số

## Kiểm tra trạng thái dự án
- Unity 6 (6000.0.38f1) ✅
- `rb.linearVelocity` đúng API Unity 6 ✅
- .meta files đã được Unity tạo cho tất cả scripts mới ✅
- Không có compile error tiềm ẩn ✅

## Bug đã fix
- `PlayerStats`: khi `hungerDef == null`, Đói không bao giờ giảm → thêm fallback `0.208f/s`
- `RestoreHunger`/`RestoreFatigue`: dùng `Mathf.Min` → không clamp về 0 khi nhận delta âm → đổi sang `Mathf.Clamp`

## Scripts mới tạo

### `Assets/Scripts/Time/TimeSystem.cs`
- Singleton, DontDestroyOnLoad (gắn cùng GameManager)
- `dayDurationSeconds = 480f` (8 phút thực = 1 game day)
- Phát `GameEvents.OnDayChanged(int)` khi sang ngày mới
- Phát `GameEvents.OnDayCycleChanged(bool isDay)` khi chuyển ngày/đêm
- Ngày bắt đầu lúc 0.25 (6:00 sáng)
- Ngày: 6:00–18:00, Đêm: 18:00–6:00

### `Assets/Scripts/Stats/StatBar.cs`
- Gắn vào `Image (Filled)` trong Canvas
- Tự động tìm `PlayerStats` trong scene
- Đổi màu sang `warningColor` khi xuống dưới `warningThreshold`
- StatType enum: HP / Hunger / Fatigue

### `Assets/Scripts/Utilities/DebugPanel.cs`
- Bật/tắt bằng F1
- Slider điều chỉnh HP, Đói, Mệt trực tiếp trong Play Mode
- Hiển thị ngày, giờ, phase ngày/đêm
- Button Reset All + Kill (test death)
- Chỉ compile trong UNITY_EDITOR || DEVELOPMENT_BUILD

## ScriptableObject assets tạo trực tiếp (YAML)
- `Assets/Data/Stats/StatDef_HP.asset` — max=100, decay=0, warning=30%, color=đỏ
- `Assets/Data/Stats/StatDef_Hunger.asset` — max=100, decay=0.208/s (hết trong 8 phút), warning=25%, color=cam
- `Assets/Data/Stats/StatDef_Fatigue.asset` — max=100, decay=0 (managed by PlayerStats), warning=20%, color=vàng

## Hệ thống chỉ số hoàn chỉnh

```
Hunger.decayPerSecond = 0.208/s (100 → 0 trong 480s = 8 phút = 1 game day)
Hunger = 0 → TakeDamage(2f * dt) [starvationDamagePerSecond]
HP = 0 → Die() → GameEvents.OnPlayerDied

Fatigue drainWhileRunning = 8f/s (100 → 0 trong 12.5s chạy liên tục)
Fatigue regenWhenIdle = 4f/s (0 → 100 trong 25s đứng)
```

## Việc cần làm trong Unity Editor (ngày 4)
- Kéo StatDef_HP/Hunger/Fatigue assets vào slots trong PlayerStats component
- Tạo Canvas + 3 Image (Filled) với StatBar component
- Gắn TimeSystem vào GameManager GameObject
- Gắn DebugPanel vào một empty GameObject

## TODO ngày 4
- HUDManager.cs
- 3 thanh chỉ số với hiệu ứng cảnh báo (pulse/blink)
- Hiển thị số ngày sống sót + giờ game
