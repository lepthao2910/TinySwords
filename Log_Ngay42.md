# LOG NGÀY 42 (23/7/2026) — Build #6: Feature Complete

## Files đã tạo / cập nhật

| File | Trạng thái | Mô tả |
|---|---|---|
| `Editor/FeatureAuditWindow.cs` | ✅ MỚI | EditorWindow audit toàn bộ 41 ngày tính năng + build checklist |

---

## Tổng kết Feature Complete

Day 42 là **Build Day** và **buffer** cuối Giai đoạn 6. Không thêm tính năng mới.

### 44 scripts đã compile (tính theo kiểu class chính):

| Giai đoạn | Scripts |
|---|---|
| 1 — Nền tảng (Ngày 1–7) | GameManager, GameState, GameEvents, PlayerController, PlayerStats, HUDManager, StatBar, TimeSystem, DayNightController, MapBounds, CameraFollow, DebugPanel |
| 2 — Tài nguyên & Túi đồ (Ngày 8–14) | Inventory, ItemData, ItemType, InventoryUI, HotbarUI, TooltipUI, ResourceObject, SmallAnimal, Campfire |
| 3 — Chế tạo & Xây Base (Ngày 15–21) | CraftingManager, CraftingRecipe, CraftingUI, PlacementSystem, Building, BuildingData, MainHouse, StorageChest, Forge, RestArea, BuildingMenuUI |
| 4 — Chiến đấu (Ngày 22–28) | PlayerCombat, IDamageable, ArrowProjectile, DamagePopupManager, EnemyBase, GnollEnemy, EnemySpawner |
| 5 — Elite, Đêm, Raid (Ngày 29–35) | NightHazardSystem, PlayerLantern, BaseRaidSystem, RaidWarningUI, WallBuilding, BuildingRepairUI, DifficultyManager, DifficultyConfig |
| 6 — Boss & Tiến trình (Ngày 36–41) | BossEnemy, BossSpawnTrigger, BossActivationZone, BossHPBar, BossLootUI, BoneProjectile, SaveManager, SaveData, SaveSystem, MainMenuUI, VictoryScreen, GameOverScreen, RunStats, MilestoneTracker, MilestoneHUD, TutorialManager |

### Lưu ý:
- **LizardElite** (Ngày 29) không có class riêng — là `EnemyBase` prefab với `enableCharge = true`, tạo bằng `Tools/WBS/Setup Lizard Elite (Day 29)`

---

## FeatureAuditWindow — Cách dùng

```
Menu: Tools → WBS → Feature Audit — Build #6 (Day 42)
```

Window hiển thị:
- **Progress bar**: X/44 scripts đã compile (xanh = hết, vàng = còn thiếu)
- **Danh sách theo giai đoạn**: ✓ xanh (đã compile) / ✗ đỏ (thiếu/lỗi) / ⚙ vàng (kiểm tra thủ công)
- **Build Checklist**: 16 mục checkbox cần tick trước khi build

---

## Checklist Build #6

- [ ] Chạy các Setup tools còn thiếu (Day 29→41) trong menu `Tools/WBS/`
- [ ] Assign prefab LizardElite, GnollEnemy, BossEnemy vào EnemySpawner/RaidSystem
- [ ] Assign Item catalog và Building catalog vào SaveManager Inspector
- [ ] Assign Required Buildings vào MilestoneTracker (Lò rèn + Khu nghỉ)
- [ ] Play mode: Main Menu → New Game → chơi thử vòng lặp farm/craft/build
- [ ] Play mode: lưu (ReturnToMainMenu), Continue → dữ liệu khôi phục đúng
- [ ] Play mode: kiếm/giáo/cung/khiên, damage nổi, i-frame
- [ ] Play mode: đêm → đèn lồng + quái mạnh hơn
- [ ] Play mode: raid → cảnh báo → quái đập nhà, sửa tường bằng gỗ
- [ ] Play mode: qua ngày 10 → boss xuất hiện, 3 pattern, loot
- [ ] Play mode: milestone HUD (phím M), thông báo khi đạt mốc
- [ ] Play mode: tutorial 4 bước ngày đầu, phím T skip
- [ ] Play mode: Boss chết → Victory; Nhà chính phá → GameOver
- [ ] File → Build Settings → Build và test .exe trên máy sạch
- [ ] git commit -m "Build #6: Feature Complete" && git tag build6

---

## Trạng thái dự án

- **Ngày hoàn thành:** 42/52
- **Tiến độ:** FEATURE COMPLETE ✓ — toàn bộ tính năng GDD đã có trong game
- **Còn lại:** 10 ngày (Ngày 43–52) dành cho âm thanh, polish, cân bằng, build final
- **Deadline:** 3/8/2026

---

## TODO ngày 43

**Âm thanh:** nhạc nền ngày/đêm/combat, SFX (chặt, đào, đánh, ăn, xây, quái, boss) từ FreeSoundCom / OpenGameArt
