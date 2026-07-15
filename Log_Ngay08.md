# LOG NGÀY 8 (22/6/2026) — Inventory System: Data + Logic

## Mục tiêu ngày 8
Xây dựng lớp data + logic cho túi đồ:
- Item ScriptableObject
- Stack logic (gộp stack đang dở, mở slot trống)
- Add / Remove / Has / GetCount
- Xử lý đầy túi (leftover + GameEvent)

---

## Files đã tạo / cập nhật

| File | Trạng thái | Mô tả |
|---|---|---|
| `Scripts/Inventory/ItemType.cs` | ✅ MỚI | Enum 5 loại: Resource, Food, Tool, Weapon, Consumable |
| `Scripts/Inventory/ItemData.cs` | ✅ MỚI | ScriptableObject — id, displayName, icon, itemType, maxStack |
| `Scripts/Inventory/InventorySlot.cs` | ✅ MỚI | Class [Serializable] — Item + Quantity, Add/Remove/Clear |
| `Scripts/Inventory/Inventory.cs` | ✅ MỚI | MonoBehaviour singleton — TryAdd/TryRemove/HasItem/GetCount/Clear |
| `Scripts/Core/GameEvents.cs` | ✅ SỬA | Thêm OnInventoryChanged, OnInventoryFull |
| `Scripts/Utilities/DebugPanel.cs` | ✅ SỬA | Thêm dòng "Túi: X/20 ô" + tag ĐẦY |

---

## Thiết kế

### ItemData (ScriptableObject)
```
Create > WBS > Item Data  →  tạo file Data/Items/Item_Wood.asset v.v.
```
Các trường quan trọng:
- `itemId` — dùng cho Save/Load (ngày 39)
- `maxStack` — Resource = 99; Tool/Weapon = 1
- `icon` — gán trong Inspector

### InventorySlot
Plain C# class, `[Serializable]` để Inspector có thể hiển thị (nếu cần).

**Add(item, amount)** — trả về leftover chưa vào được:
1. Nếu slot trống: gán Item
2. Nếu sai loại: trả về nguyên amount
3. Tính canAdd = maxStack - Quantity, thêm min(canAdd, amount), trả leftover

**Remove(amount)** — trả về lượng thực sự đã bỏ; tự reset slot nếu về 0.

### Inventory (MonoBehaviour)

**TryAdd(item, amount)**:
- Pass 1: tìm slot đã có cùng item + chưa đầy → bù vào
- Pass 2: tìm slot trống → mở mới
- Nếu có gì thêm được → fire `OnInventoryChanged`
- Nếu leftover > 0 → fire `OnInventoryFull`
- Trả về leftover (0 = thêm hết)

**TryRemove(item, amount)**:
- Kiểm tra `HasItem(amount)` trước, từ chối ngay nếu không đủ
- Duyệt slots lấy đủ amount, fire `OnInventoryChanged`

---

## Cách test trong Unity Editor

1. Gắn script `Inventory` vào Player GameObject (hoặc GameSystems)
2. Tạo vài `ItemData` asset: `Assets/Data/Items/Item_Wood.asset`, `Item_Stone.asset`
3. Trong Play Mode, mở Debug Panel (F1) → dòng "Túi: 0/20 ô" xuất hiện
4. Gọi `Inventory.Instance.TryAdd(woodItem, 5)` qua script test hoặc ContextMenu

---

## Ghi chú

- `IsFull` check: trả về true chỉ khi TẤT CẢ slot đều không trống
- `slotCount` mặc định 20 — chỉnh trong Inspector nếu cần
- Ngày 9 sẽ thêm UI lưới ô đồ + hotbar 6 ô; `IReadOnlyList<InventorySlot>` expose sẵn cho UI duyệt

---

## TODO ngày 9
- UI Inventory: lưới ô đồ, tooltip, click chuyển ô
- Hotbar 6 ô
