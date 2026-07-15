# KẾ HOẠCH PHÁT TRIỂN WILD BASE SURVIVAL — 52 NGÀY (12/6 → 2/8/2026)

**Mục tiêu:** Hoàn thành bản game chơi được (MVP đầy đủ theo GDD) trước ngày 3/8/2026.
**Giả định:** 1 người làm chính, ~5–8 giờ/ngày. Engine: Unity 2D (C#), asset: Tiny Swords (top-down pixel art, miễn phí, khớp phong cách dark fantasy/cartoon trong GDD). Nếu dùng Godot, kế hoạch giữ nguyên, chỉ đổi công cụ.
**Nguyên tắc:** Mỗi tuần kết thúc bằng 1 bản build chạy được. Tính năng "mở rộng sau" (mục 18 GDD: nhân vật, mùa, biome) KHÔNG nằm trong scope.

---

## GIAI ĐOẠN 1 — NỀN TẢNG (Tuần 1: 12/6 – 18/6)

**Ngày 1 — Thứ Sáu 12/6:** Chốt scope MVP, viết Technical Design Document (kiến trúc, data, hệ thống), tạo backlog toàn dự án, chọn engine + asset pack, cài Unity/Godot + Git, tạo repo.
**Ngày 2 — 13/6:** Tạo project, cấu trúc thư mục, import Tiny Swords assets, setup pixel-perfect camera, sorting layers, input system. Nhân vật di chuyển 4/8 hướng + animation idle/walk.
**Ngày 3 — 14/6:** Hệ thống chỉ số: Máu, Đói, Mệt (ScriptableObject/StatSystem). Đói giảm theo thời gian; Đói=0 → trừ máu; Máu=0 → chết. Mệt giảm khi chạy.
**Ngày 4 — 15/6:** HUD cơ bản: 3 thanh chỉ số, hiệu ứng cảnh báo khi thấp. Debug panel chỉnh thông số nhanh (phục vụ balance sau này).
**Ngày 5 — 16/6:** Dựng map test bằng Tilemap (cỏ, nước, đường), va chạm, giới hạn map. Đặt rải cây và đá (chưa tương tác).
**Ngày 6 — 17/6:** Chu kỳ ngày/đêm: đồng hồ game (1 ngày ≈ 8 phút), lighting 2D đổi theo giờ, đếm số ngày sống sót hiển thị trên HUD.
**Ngày 7 — 18/6:** Build #1. Test toàn bộ tuần 1, fix bug, dọn code, ghi chú balance. Buffer nếu tuần trễ tiến độ.

## GIAI ĐOẠN 2 — TÀI NGUYÊN & TÚI ĐỒ (Tuần 2: 19/6 – 25/6)

**Ngày 8 — 19/6:** Hệ thống Inventory (data + logic): item ScriptableObject, stack, thêm/bớt/đầy túi.
**Ngày 9 — 20/6:** UI Inventory: lưới ô đồ, tooltip, kéo thả hoặc click chuyển ô. Hotbar 6 ô.
**Ngày 10 — 21/6:** Tương tác thu thập: chặt cây rơi Gỗ (cần rìu), cây có máu + đổ + respawn sau X phút. Hiệu ứng rung + particle.
**Ngày 11 — 22/6:** Khai thác Đá và mỏ Vàng (cần cuốc). Vàng đặt ở khu xa/nguy hiểm theo GDD. Hành động khai thác tiêu hao Mệt.
**Ngày 12 — 23/6:** Thú nhỏ (gà/cừu Tiny Swords): AI lang thang, bỏ chạy khi bị đánh, chết rơi Thịt. Ăn thịt sống hồi ít đói.
**Ngày 13 — 24/6:** Hệ thống dùng đồ: ăn từ hotbar, item hồi máu/đói/mệt theo data. Nấu thịt tại lửa trại (công trình mini đầu tiên) → thịt nướng hồi nhiều hơn.
**Ngày 14 — 25/6:** Build #2: vòng lặp "đi farm → mệt/đói → ăn → farm tiếp" chơi được. Fix bug, buffer.

## GIAI ĐOẠN 3 — CHẾ TẠO & XÂY BASE (Tuần 3: 26/6 – 2/7)

**Ngày 15 — 26/6:** Hệ thống Crafting (data-driven theo công thức mục 15 GDD): rìu, cuốc, kiếm, giáo, cung từ gỗ+đá. UI bảng chế tạo.
**Ngày 16 — 27/6:** Hệ thống đặt công trình: ghost preview, kiểm tra vị trí hợp lệ, tốn tài nguyên, lưới snap.
**Ngày 17 — 28/6:** Nhà chính: HP công trình, là điều kiện thua nếu bị phá, điểm hồi sinh, mở khóa công trình khác. Nâng cấp cấp 1→2 mở thêm công thức.
**Ngày 18 — 29/6:** Nhà kho: rương chứa đồ riêng, UI chuyển đồ giữa túi và kho, nâng cấp tăng số ô. Đồ trong kho không mất khi chết.
**Ngày 19 — 30/6:** Lò rèn: chế tạo/nâng cấp vũ khí cấp cao (kiếm sắt, giáo cứng, khiên đá→khiên vàng) dùng đá+vàng.
**Ngày 20 — 1/7:** Khu nghỉ: nghỉ hồi Mệt nhanh, nâng cấp hồi thêm máu. Cơ chế chết: rơi một phần đồ trong túi, hồi sinh tại nhà chính (theo phương án giữa của mục 13 GDD).
**Ngày 21 — 2/7:** Build #3: full vòng lặp sinh tồn + xây base. Fix bug, buffer. **Mốc giữa kỳ: 40% dự án.**

## GIAI ĐOẠN 4 — CHIẾN ĐẤU (Tuần 4: 3/7 – 9/7)

**Ngày 22 — 3/7:** Khung combat: hitbox/hurtbox, sát thương, knockback, i-frame, số damage nổi, animation tấn công.
**Ngày 23 — 4/7:** Kiếm (cân bằng) + Giáo (tầm xa, chậm hơn). Đánh tiêu hao Mệt; Mệt thấp → damage giảm, di chuyển chậm (theo mục 5.3).
**Ngày 24 — 5/7:** Cung + mũi tên: bắn projectile, giữ khoảng cách, mũi tên chế từ gỗ.
**Ngày 25 — 6/7:** Khiên: giữ để block giảm sát thương, di chuyển chậm khi giơ khiên. Rìu/cuốc đánh được nhưng damage thấp.
**Ngày 26 — 7/7:** Normal Monster #1 (goblin): tuần tra quanh khu tài nguyên, phát hiện → đuổi → tấn công, rời phạm vi thì quay về. Chết rơi thịt/nguyên liệu.
**Ngày 27 — 8/7:** Normal Monster #2 (loại đánh khác để đa dạng). Spawner theo khu vực, mật độ tăng theo ngày sống sót.
**Ngày 28 — 9/7:** Build #4: farm có rủi ro thật sự. Cân chỉnh damage/HP đợt 1. Buffer.

## GIAI ĐOẠN 5 — ELITE, ĐÊM NGUY HIỂM & TẤN CÔNG BASE (Tuần 5: 10/7 – 16/7)

**Ngày 29 — 10/7:** Elite Monster: HP/damage cao, 1 kỹ năng đặc biệt (lao tới hoặc đánh vùng), canh giữ mỏ vàng, rơi vàng + nguyên liệu nâng cấp.
**Ngày 30 — 11/7:** Ban đêm: quái mạnh hơn/đông hơn, tầm nhìn giảm, quái đêm đặc biệt. Ngày để farm, đêm để trốn (mục 11 GDD).
**Ngày 31 — 12/7:** Hệ thống quái tấn công base theo đợt (mỗi 3–5 đêm): quái tìm đường tới nhà chính, đập công trình. Cảnh báo trước khi đợt đến.
**Ngày 32 — 13/7:** Hàng rào/tường gỗ + tường đá (công trình phòng thủ bổ sung cần thiết cho cơ chế thủ base). Quái ưu tiên phá tường chắn đường.
**Ngày 33 — 14/7:** Sửa chữa công trình bằng tài nguyên. AI tìm đường (pathfinding) ổn định quanh base.
**Ngày 34 — 15/7:** Độ khó leo thang theo ngày: bảng cấu hình ngày → loại quái, số lượng, chỉ số. Khớp nhịp mục 16 GDD (ngày 1–2 yên, 3–5 đông dần, 6–10 elite, sau 10 boss).
**Ngày 35 — 16/7:** Build #5: sinh tồn dài hạn có áp lực thật. Playtest 30 phút liên tục, fix bug. Buffer.

## GIAI ĐOẠN 6 — BOSS & TIẾN TRÌNH HOÀN CHỈNH (Tuần 6: 17/7 – 23/7)

**Ngày 36 — 17/7:** Boss: thiết kế 3 pattern tấn công (đánh thường, đánh vùng, gọi quái con), thanh máu boss riêng.
**Ngày 37 — 18/7:** Boss xuất hiện sau ngày 10 (hoặc người chơi kích hoạt tại khu boss). Rơi vật phẩm hiếm + nhiều vàng.
**Ngày 38 — 19/7:** Điều kiện thắng/thua hoàn chỉnh: thắng = hạ boss + nhà chính còn nguyên; thua = chết hẳn hoặc nhà chính bị phá. Màn hình thắng/thua, thống kê (số ngày, quái giết, tài nguyên).
**Ngày 39 — 20/7:** Save/Load game (JSON): vị trí, chỉ số, túi đồ, kho, công trình, ngày hiện tại. Menu chính: New Game / Continue / Quit.
**Ngày 40 — 21/7:** Hệ thống mục tiêu/milestone trên HUD (sống 10 ngày, xây đủ công trình, rèn vũ khí cấp cao, hạ boss) — tạo cảm giác tiến bộ theo mục 14 GDD.
**Ngày 41 — 22/7:** Tutorial nhẹ ngày đầu: hướng dẫn nhặt gỗ → chế rìu → ăn → xây nhà chính bằng popup/chỉ dẫn.
**Ngày 42 — 23/7:** Build #6: **FEATURE COMPLETE** — toàn bộ tính năng GDD đã có trong game. Từ đây không thêm tính năng mới.

## GIAI ĐOẠN 7 — POLISH & CÂN BẰNG (Tuần 7: 24/7 – 30/7)

**Ngày 43 — 24/7:** Âm thanh: nhạc nền ngày/đêm/combat, SFX (chặt, đào, đánh, ăn, xây, quái) từ nguồn miễn phí (freesound, OpenGameArt).
**Ngày 44 — 25/7:** Hiệu ứng: particle, screen shake, flash khi trúng đòn, animation chuyển cảnh ngày/đêm mượt.
**Ngày 45 — 26/7:** Polish UI: font thống nhất, icon item, pause menu, settings (âm lượng, độ phân giải).
**Ngày 46 — 27/7:** Cân bằng đợt cuối #1: tốc độ đói/mệt, công thức chế tạo, HP/damage quái theo bảng số liệu. Chơi thử full run 10+ ngày in-game.
**Ngày 47 — 28/7:** Cân bằng #2 + đưa 2–3 người ngoài chơi thử, ghi nhận phản hồi.
**Ngày 48 — 29/7:** Fix theo phản hồi playtest. Tối ưu hiệu năng (object pooling cho quái/projectile, kiểm tra FPS).
**Ngày 49 — 30/7:** Build #7 — Release Candidate 1. Quét bug toàn diện theo checklist.

## GIAI ĐOẠN 8 — HOÀN THIỆN & BÀN GIAO (31/7 – 2/8)

**Ngày 50 — 31/7:** Fix bug nghiêm trọng còn lại. Đóng băng code (code freeze) cuối ngày.
**Ngày 51 — 1/8:** Build bản final (Windows/WebGL), test bản build trên máy sạch. Làm trang itch.io hoặc gói zip, viết hướng dẫn chơi.
**Ngày 52 — 2/8:** Tổng duyệt cuối, quay video gameplay 1–2 phút, hoàn thiện tài liệu bàn giao. **NỘP TRƯỚC 3/8.** ✅

---

## QUY TẮC QUẢN LÝ TIẾN ĐỘ
1. Cuối mỗi ngày: ghi log việc đã xong / chưa xong. Việc chưa xong đẩy vào ngày buffer cuối tuần (ngày 7, 14, 21, 28, 35, 42, 49).
2. Nếu trễ quá 2 ngày buffer: cắt scope theo thứ tự — (1) Tutorial, (2) Quái normal #2, (3) Nâng cấp công trình cấp 2, (4) Cung+mũi tên. KHÔNG cắt: 3 chỉ số, farm, craft, nhà chính, combat kiếm, quái, boss, thắng/thua.
3. Commit Git mỗi ngày, tag mỗi bản build tuần.
