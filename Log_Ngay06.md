# LOG NGÀY 6 (17/6/2026) — Chu kỳ ngày/đêm

## Scripts tạo mới

### `Assets/Scripts/Time/DayNightController.cs`
- Tự động tìm `Global Light 2D` trong scene (`lightType == Global`)
- Đọc `TimeSystem.Instance.TimeOfDay` mỗi frame → nội suy 7 keyframe
- Thay đổi `globalLight.intensity` + `globalLight.color`

#### Keyframe mặc định:
| time | giờ | intensity | màu |
|---|---|---|---|
| 0.00 | 00:00 | 0.15 | Xanh đậm (nửa đêm) |
| 0.20 | 04:48 | 0.30 | Cam tối (rạng sáng) |
| 0.27 | 06:28 | 0.90 | Vàng ấm (sáng sớm) |
| 0.50 | 12:00 | 1.00 | Trắng (trưa) |
| 0.70 | 16:48 | 0.90 | Vàng (chiều) |
| 0.80 | 19:12 | 0.30 | Cam đỏ (hoàng hôn) |
| 0.90 | 21:36 | 0.15 | Xanh đậm (đêm) |

- `OnValidate()`: preview realtime khi kéo slider trong Inspector lúc Play mode

## Scripts cập nhật
- `WBSSceneSetup.cs`: thêm `EnsureComponent<DayNightController>(go)` vào `SetupGameSystems()`

## Trạng thái trước đó (đã có sẵn, không cần thêm)
- `TimeSystem.cs`: đồng hồ game 8 phút/ngày, `DaysAlive`, `TimeOfDay`, `GameHour`
- `HUDManager.cs`: hiển thị `dayLabel` (Ngày X), `timeLabel` (HH:MM), icon ngày/đêm

## Cần làm thủ công trong Unity Editor
1. Chạy **Tools → Wild Base Survival → Setup GameSystems Only**
   → Tự thêm `DayNightController` vào `GameSystems`
2. Đảm bảo scene có **Global Light 2D**: Hierarchy → Right-click → Light → Global Light 2D
   (hoặc GameObject → Light → Global Light 2D nếu dùng URP 2D)
3. Play mode: ánh sáng sẽ thay đổi tự động theo thời gian game

## Tinh chỉnh nếu cần
- Tăng `dayDurationSeconds` trong TimeSystem để ngày dài hơn
- Kéo thả `globalLight` vào slot trong Inspector nếu auto-find không hoạt động
- Chỉnh keyframe trực tiếp trong Inspector (mỗi dòng = 1 mốc thời gian + cường độ + màu)

## TODO ngày 7 (Build #1)
- Test toàn bộ tuần 1: player movement, stat bars, HUD, map, lighting
- Fix bug nếu có
- Commit và tag `build-1`
