# LOG NGÀY 5 (16/6/2026) — Map Test & Tilemap

## Kiểm tra trạng thái dự án (ngày 1–4)

### Xác nhận OK ✅
- Scripts Day 1–4: 11 files trong `Assets/Scripts/` đều còn nguyên
- `Assets/Editor/WBSSceneSetup.cs`: Editor tool hoàn chỉnh
- `Assets/Data/Stats/`: 3 StatDef assets (HP, Hunger, Fatigue)
- `Assets/Prefabs/Player/Player.prefab`: gravityScale=0, FreezeRotation, Interpolate ✅
- `Assets/Animation/Pawn/Player.controller`: Speed/IsInteracting/Attack/FaceX params ✅

### Phát hiện và xử lý
- `Assets/testcode.cs`: file test cũ từ project cũ → đã **xóa**
- `filterMode: 1` (Bilinear) trên toàn bộ sprites → thêm menu **Fix Sprite Filter Mode** trong Editor tool

---

## Scripts tạo mới — Ngày 5

### `Assets/Scripts/Map/MapBounds.cs`
- Singleton, lưu `boundsMin/boundsMax` (mặc định: -12,-8 → 12,8 world units)
- `Clamp(Vector2)` trả về vị trí bị giới hạn trong bounds
- `Contains(Vector2)` kiểm tra xem điểm có trong bounds không
- Gizmos vẽ viền map màu xanh trong Scene View

### `Assets/Scripts/Map/CameraFollow.cs`
- `[RequireComponent(typeof(Camera))]`
- Auto-find Player nếu không set target
- SmoothDamp follow (`smoothTime = 0.12f`)
- Clamp camera position theo MapBounds (trừ half-width/height của viewport)
- Không xung đột với PixelPerfectCamera (chỉ di chuyển X/Y, không thay đổi orthographicSize)

### `Assets/Scripts/Resources/ResourceObject.cs`
- `ResourceType` enum: Tree, Rock, GoldStone
- `maxHP`, `currentHP` — chưa có interaction logic (sẽ làm Day 10/11)
- Public: `Type`, `HPRatio`

---

## Scripts cập nhật

### `Assets/Scripts/Player/PlayerController.cs`
- Thêm 1 dòng trong `FixedUpdate`: `rb.position = MapBounds.Instance.Clamp(rb.position)`
- Player không thể di chuyển ra ngoài map bounds

### `Assets/Editor/WBSSceneSetup.cs`
- Thêm `using System.Linq` và `using UnityEngine.Tilemaps`
- `SetupScene()` nay gọi thêm `SetupMap()` sau `SetupPlayer()`
- **SetupGameSystems()**: thêm `EnsureComponent<MapBounds>(go)`
- **Mới: `SetupMap()`**:
  - Tạo `MapGrid` (Grid component)
  - Tạo 3 Tilemap layers: Ground, Obstacles, Decoration
  - Obstacles: Rigidbody2D(Static) + TilemapCollider2D (compositeOperation=Merge) + CompositeCollider2D
  - Gán CameraFollow lên Main Camera
  - Gọi `PlaceResources()`
- **Mới: `PlaceResources()`**: Đặt 7 cây + 5 đá tại vị trí cố định
- **Mới: `PlaceResourceGroup()`**: Tạo GO với SpriteRenderer, CircleCollider2D(r=0.25), ResourceObject
- **Mới: `[MenuItem] Fix Sprite Filter Mode`**: Đổi Bilinear → Point cho tất cả sprites trong Assets/Asset

---

## Menu Items mới
```
Tools > Wild Base Survival > Setup Scene (Full)        ← đã có, thêm SetupMap
Tools > Wild Base Survival > Setup Map Only            ← MỚI
Tools > Wild Base Survival > Fix Sprite Filter Mode    ← MỚI
```

---

## Cần làm thủ công trong Unity Editor (Tile Painting)

Sau khi chạy **Setup Map Only**:
1. **Sorting Layers**: Project Settings → Tags and Layers → thêm: `Ground`, `Resources`, `Player`, `Effects`
2. **Filter Mode**: Chạy **Fix Sprite Filter Mode** → 1 lần fix tất cả sprites về Point
3. **Tile Palette**: Window → 2D → Tile Palette → Create New Palette (chọn folder `Assets/Data/`)
4. **Import Tileset**: Kéo `Tilemap_color1.png` vào palette → Unity auto-tạo Tile assets
5. **Vẽ Ground**: Chọn layer `Ground`, dùng brush vẽ nền cỏ phủ kín vùng `-12,-8 → 12,8`
6. **Vẽ Obstacles**: Chọn layer `Obstacles`, vẽ nước viền map (4–5 tile rộng) và một số island
7. **Vẽ Decoration**: Chọn layer `Decoration`, vẽ chi tiết trang trí nhỏ

---

## Thông số kỹ thuật

| Thành phần | Giá trị |
|---|---|
| Map bounds mặc định | -12 → 12 (X), -8 → 8 (Y) = 24×16 world units |
| Tilemap cell size | 1×1 (default Grid) |
| Obstacles collision | CompositeCollider2D + Rigidbody2D Static |
| Camera smooth time | 0.12s |
| Tree positions (WU) | (-5,3) (-8,-2) (3,5) (7,-4) (-3,-6) (9,2) (-9,4) |
| Rock positions (WU) | (-4,-3) (5,4) (-7,1) (8,-5) (2,-7) |
| Rock scale | (2,2,1) — x2 vì sprite gốc 34×28px |

---

## TODO ngày 6 (chu kỳ ngày/đêm)
- `DayNightController.cs`: điều chỉnh `Global Light 2D` intensity theo `TimeSystem.TimeOfDay`
- Màu ánh sáng: ban ngày (1.0, trắng) → hoàng hôn (0.6, cam) → đêm (0.2, xanh đậm) → bình minh
- HUD đã có `dayLabel` + `dayIcon/nightIcon` — chỉ cần nối TimeSystem
