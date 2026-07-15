# LOG NGÀY 9 (22/6/2026) — Inventory UI: Lưới ô đồ + Hotbar

## Files đã tạo / cập nhật

| File | Trạng thái | Mô tả |
|---|---|---|
| `Scripts/UI/InventorySlotUI.cs` | ✅ MỚI | View 1 ô đồ: icon, số lượng, highlight, click, hover→tooltip |
| `Scripts/UI/InventoryUI.cs` | ✅ MỚI | Lưới 20 ô, mở/đóng Tab+Esc, click-to-move (2-click swap) |
| `Scripts/UI/HotbarUI.cs` | ✅ MỚI | Hotbar 6 ô (slot 0–5), phím 1–6 chọn ô |
| `Scripts/UI/TooltipUI.cs` | ✅ MỚI | Tooltip theo chuột, hiện tên + mô tả item khi hover |
| `Scripts/Inventory/InventorySlot.cs` | ✅ SỬA | Thêm `SwapContentsWith(other)` |
| `Scripts/Inventory/Inventory.cs` | ✅ SỬA | Thêm `SwapSlots(a, b)` với logic gộp stack nếu cùng loại |

---

## Kiến trúc

### Dòng dữ liệu
```
Inventory (data) ──OnInventoryChanged──► InventoryUI.RefreshAll()
                                     ──► HotbarUI.RefreshAll()
```

### Click-to-move (InventoryUI)
1. Click ô có item → `selectedIndex = i`, highlight bật
2. Click ô khác → `Inventory.SwapSlots(selected, target)` → clear highlight
3. Click cùng ô đang chọn → bỏ chọn
4. Nếu 2 ô cùng loại item + stackable → gộp stack (không swap)

### SwapSlots logic
- Ưu tiên gộp stack nếu `slotA.Item == slotB.Item && maxStack > 1`
- Ngược lại: `slotA.SwapContentsWith(slotB)` — tận dụng quyền truy cập private setter trong C#

---

## Cần setup trong Unity Editor

### Prefab: InventorySlotUI
```
GameObject "SlotUI" (Image — background, RaycastTarget = true)
  ├── [InventorySlotUI component]
  ├── Image "Icon" (Raycast Target = false)
  ├── TextMeshProUGUI "Qty" (góc dưới phải)
  └── Image "Highlight" (border/overlay, disabled mặc định)
```
→ Lưu vào `Assets/Prefabs/UI/InventorySlotUI.prefab`

### Scene Hierarchy (Canvas)
```
Canvas
  ├── HUD (HUDManager)
  │     ├── DayLabel, TimeLabel, DayIcon, NightIcon
  │     └── StatBars
  ├── Hotbar (HotbarUI)          ← luôn hiển thị
  │     ├── Slot0 (InventorySlotUI)
  │     ├── Slot1 … Slot5
  │     └── (GridLayoutGroup: horizontal, spacing 4)
  ├── InventoryPanel (InventoryUI)   ← mặc định ẩn
  │     ├── Background (Image)
  │     └── SlotGrid (GridLayoutGroup: 5 cột × 4 hàng)
  └── Tooltip (TooltipUI)        ← mặc định ẩn
        ├── Panel (Image bg)
        ├── NameText (TMP)
        └── DescText (TMP)
```

### Gán Inspector
**InventoryUI:**
- `panel` → GameObject "InventoryPanel"
- `slotContainer` → Transform "SlotGrid"
- `slotPrefab` → Prefab InventorySlotUI

**HotbarUI:**
- `slots[0..5]` → 6 InventorySlotUI trong Hotbar

**TooltipUI:**
- `panel` → GameObject "Panel" (con của Tooltip)
- `nameText`, `descText` → TMP components
- `rectTransform` → RectTransform của chính tooltip GameObject

**Inventory (MonoBehaviour):**
- Gắn vào Player hoặc GameSystems
- `slotCount` = 20

---

## Test nhanh trong Play Mode
```csharp
// Tạo script TestInventory.cs tạm thời:
void Start()
{
    Inventory.Instance.TryAdd(woodItem,  15);
    Inventory.Instance.TryAdd(stoneItem, 30);
}
```
Sau đó nhấn Tab → lưới hiện, hover ô → tooltip xuất hiện, click 2 ô → hoán đổi.

---

## TODO ngày 10
- Tương tác thu thập: chặt cây rơi Gỗ (cần rìu trong hotbar)
- Cây có máu (HP), đổ, respawn sau X phút
