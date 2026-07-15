# LOG NGÀY 1–2 (12/6/2026 – 13/6/2026)

## NGÀY 1 — Nền tảng & Tài liệu

### Đã hoàn thành
- [x] Xác nhận scope MVP theo kế hoạch 52 ngày
- [x] Viết TDD_WildBaseSurvival.md (kiến trúc, data, hệ thống, scene, sorting layers, physics layers)
- [x] Xác nhận engine: Unity 2D URP, asset: Tiny Swords, repo: đã có trên GitLab
- [x] Backlog = kế hoạch 52 ngày KeHoach_WildBaseSurvival_52Ngay.md

### Tình trạng project ban đầu
- Project cũ: auto-battler/RTS với Ally (Archer, Lancer, Monk, Warrior) vs Enemy (Bear, Harpoon, Paddle, Shaman, Snake, Spider, Thief, Zombie)
- Đã xóa toàn bộ code cũ không phù hợp với game survival

---

## NGÀY 2 — Cấu trúc & Player

### Đã hoàn thành
- [x] Xóa code cũ: Units.cs, toàn bộ Ally/, Enemy/, Weapon/, AddButt.cs, Interface.cs, Reward.cs
- [x] Xóa prefabs cũ: tất cả unit prefabs cũ
- [x] Tạo cấu trúc thư mục Scripts/: Core, Player, Stats, Time, Inventory, Crafting, Resources, Building, Combat, Monsters, WaveConfig, Animals, UI, Save, Audio, Utilities
- [x] Tạo cấu trúc Prefabs/: Player, Monsters, Animals, Buildings, UI, Effects, Projectiles
- [x] Tạo Assets/Data/: Items, Recipes, Stats, Monsters, Buildings, WaveConfig
- [x] Tạo Assets/Animation/: Player, Monsters
- [x] Core/GameState.cs — enum game states
- [x] Core/GameEvents.cs — event bus static C# Actions
- [x] Core/GameManager.cs — singleton DontDestroyOnLoad, quản lý game state
- [x] Stats/StatDefinition.cs — ScriptableObject định nghĩa stat
- [x] Player/PlayerStats.cs — HP/Đói/Mệt, decay logic, starvation, fatigue running
- [x] Player/PlayerController.cs — New Input System, 8-direction WASD, Rigidbody2D, animator parameters (Speed, FaceX, IsInteracting), sprite flip

### Chưa làm (cần Unity Editor)
- [ ] Setup Pixel Perfect Camera trên Main Camera
- [ ] Tạo Sorting Layers trong Project Settings
- [ ] Tạo Physics Layers và Collision Matrix
- [ ] Tạo Player Animator Controller với clips từ Pawn sprites
- [ ] Dựng Player prefab (Pawn sprites + PlayerController + PlayerStats + Rigidbody2D + PlayerInput)
- [ ] Test di chuyển trong SampleScene

### Ghi chú kỹ thuật
- Player sprite dùng: `Assets/Asset/Units/Blue Units/Pawn/Pawn_Idle.png`, `Pawn_Run.png`, `Pawn_Interact *.png`
- Input Actions: `Assets/InputSystem_Actions.inputactions` (có sẵn Move, Attack, Interact)
- PlayerInput component dùng "Send Messages" mode → OnMove(InputValue), OnAttack(InputValue), OnInteract(InputValue)

---

## TODO NGÀY 3 (14/6)
- Hoàn thiện setup Unity Editor (Camera, Layers, Animator)
- Hệ thống chỉ số: StatDefinition SO assets, HUD test bars
- Đói giảm theo game time (cần TimeSystem để scale đúng)
