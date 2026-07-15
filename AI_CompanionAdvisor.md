# Tính năng AI — Linh vật đồng hành gợi ý chiến thuật (Companion Advisor)

## 1. Mục tiêu tính năng
Một "linh vật" nhỏ bám theo nhân vật chính trong lúc chơi, chủ động đưa ra gợi ý về
hướng đi / lối đánh dựa trên trạng thái game hiện tại. Có 2 lớp gợi ý:

| Lớp | Cơ chế | Khi nào dùng |
|---|---|---|
| **Rule-based (offline)** | Hệ luật đọc trực tiếp state game (HP/Đói/Mệt, ngày-đêm, quái gần, túi đồ, boss...) và chọn câu gợi ý phù hợp | Chạy liên tục, không cần mạng, phản hồi tức thời |
| **AI thật (Claude API)** | Gửi bản tóm tắt trạng thái game lên **Anthropic Claude API**, nhận lại lời khuyên bằng ngôn ngữ tự nhiên | Khi người chơi **nhấn Enter** để hỏi lời khuyên sâu hơn (cooldown 20s/lần) |

Đây là kiến trúc **hybrid**: phần lõi gameplay không phụ thuộc mạng (rule-based luôn hoạt động),
phần AI sinh ngôn ngữ tự nhiên là lớp bổ sung, có xử lý lỗi khi mất mạng/thiếu API key.

## 2. Kiến trúc & luồng dữ liệu

```
                     ┌────────────────────────┐
                     │   CompanionContext.cs   │  ← đọc PlayerStats, TimeSystem,
                     │  (gom trạng thái game)  │     Inventory, EnemyBase, BossEnemy,
                     └───────────┬─────────────┘     MainHouse... thành 1 snapshot
                                 │
             ┌───────────────────┴───────────────────┐
             ▼                                        ▼
 ┌─────────────────────────┐            ┌──────────────────────────────┐
 │   CompanionAdvisor.cs    │            │   ClaudeAdvisorClient.cs      │
 │  Rule engine 8 mức ưu    │            │  Gửi state → Claude API       │
 │  tiên (máu thấp, quái    │            │  (UnityWebRequest, coroutine) │
 │  gần, đói/mệt, raid,     │            │  Nhận lại text tự nhiên       │
 │  boss, đêm, túi đầy...)  │            └──────────────────────────────┘
 └───────────┬──────────────┘
             ▼
 ┌─────────────────────────┐
 │  CompanionBubbleUI.cs    │  ← hiển thị bong bóng thoại phía trên linh vật
 └─────────────────────────┘
```

**Trigger gọi AI thật:** `PlayerController.TryInteractAtMouse()` — khi người chơi click
chuột trái trúng linh vật → gọi `CompanionAdvisor.RequestDeepAdvice()`.

## 3. Các file code chính (để show / giải thích cho GV)

| File | Vai trò |
|---|---|
| `Assets/Scripts/Companion/CompanionContext.cs` | Gom toàn bộ trạng thái game liên quan (HP/Đói/Mệt, ngày/đêm, quái gần nhất, có vũ khí không, tài nguyên trong túi, boss còn sống không, khoảng cách tới nhà chính) thành 1 `Snapshot`. Dùng chung cho cả rule engine lẫn prompt gửi AI. |
| `Assets/Scripts/Companion/CompanionAdvisor.cs` | **"Bộ não" rule-based** — mỗi giây đánh giá `Snapshot` theo danh sách luật ưu tiên cố định, chọn ra 1 tip phù hợp nhất, có cơ chế chống lặp/spam. Đồng thời là nơi gọi và quản lý trạng thái (Idle/Loading/Result/Error) cho lời khuyên AI thật. |
| `Assets/Scripts/Companion/ClaudeAdvisorClient.cs` | **Tầng gọi AI thật** — build request JSON (model, system prompt định hình "persona" linh vật, nội dung state game), POST tới `https://api.anthropic.com/v1/messages`, parse response, xử lý lỗi mạng/HTTP. |
| `Assets/Scripts/Companion/CompanionConfig.cs` | Đọc API key từ `Assets/StreamingAssets/advisor_config.json` (file không commit lên Git vì chứa secret). |
| `Assets/Scripts/Companion/CompanionFollower.cs` | Di chuyển linh vật bám theo nhân vật chính (nội suy mượt, không dạy AI gì — thuần vật lý/animation). |
| `Assets/Scripts/Companion/CompanionBubbleUI.cs` | UI hiển thị bong bóng thoại (tip thường / "đang suy nghĩ..." / kết quả AI / lỗi). |

## 4. Ví dụ luật rule-based (trích `CompanionAdvisor.EvaluateRules`)

```csharp
if (s.hpRatio <= 0.25f)
    tip = "Máu bạn đang thấp lắm! Tìm chỗ nghỉ hoặc ăn ngay đi.";
else if (s.nearestEnemyChasing && !s.hasWeaponEquipped)
    tip = "Có quái gần đấy mà bạn chưa cầm vũ khí — coi chừng!";
else if (s.hungerRatio <= 0.2f)
    tip = "Bạn đang đói rồi, nên ăn hoặc nấu thịt sớm nhé.";
// ... (mệt thấp, boss xuất hiện, đêm ở xa base, túi đầy, mặc định)
```

## 5. Ví dụ prompt gửi Claude (trích `CompanionContext.ToPromptText`)

```
Ngày sống sót: 6 (ban đêm).
HP: 40%, Đói: 65%, Mệt: 30%.
Vũ khí đang cầm: có.
Túi đồ: còn chỗ, Gỗ=12, Đá=4, Thức ăn=2.
Quái gần nhất cách 3.2m (đang đuổi theo).
Chưa có boss xuất hiện.
```

**System prompt** (persona cố định, không đổi theo từng lần gọi):
> "Bạn là một linh vật nhỏ đồng hành cùng người chơi trong game sinh tồn Wild Base Survival.
> Trả lời bằng tiếng Việt, giọng thân thiện, ngắn gọn tối đa 3 câu. Đưa ra lời khuyên chiến
> thuật CỤ THỂ dựa trên dữ liệu trạng thái game được cung cấp bên dưới. Không bịa số liệu
> ngoài dữ liệu được cung cấp."

→ Claude trả lời dựa hoàn toàn trên state thật của ván chơi, không hallucinate số liệu ngoài
những gì được cung cấp.

## 6. Kỹ thuật
- Đây là chỗ **đầu tiên trong project** dùng network call (`UnityWebRequest`) — toàn bộ phần
  còn lại của game chạy hoàn toàn offline.
- Thiết kế **fail-safe**: nếu mất mạng / sai key / server lỗi → linh vật báo lỗi thân thiện,
  gameplay chính (rule-based tip, di chuyển, combat...) không bị ảnh hưởng.
- Tách biệt rõ **dữ liệu → luật → hiển thị** (Context / Advisor / BubbleUI) nên có thể thay
  đổi luật hoặc đổi UI mà không đụng vào logic đọc state hay logic gọi API.
- API key không hard-code trong source — đọc từ file JSON riêng, đã thêm vào `.gitignore`.

## 7. Demo 
1. Vào Play trong Unity.
2. Chỉnh HP/Đói/Mệt (context menu debug trên `PlayerStats`) → linh vật đổi tip theo đúng luật ưu tiên.
3. Click chuột trái vào linh vật → thấy trạng thái "đang suy nghĩ..." → nhận lời khuyên
   thật từ Claude, nội dung bám sát state hiện tại của ván chơi (không phải câu cố định).
4. Tắt mạng, thử click lại → linh vật báo lỗi thân thiện, game vẫn chơi bình thường.
