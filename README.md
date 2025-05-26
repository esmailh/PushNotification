# 📢 Notification Service

سیستمی برای ارسال **پوش نوتیفیکیشن** و **پیامک** به صورت انبوه، زمان‌بندی‌شده و با قابلیت مدیریت کمپین.

---

## ✨ ویژگی‌ها

- ✅ **ارسال انبوه نوتیفیکیشن** با قابلیت تقسیم‌بندی هوشمند (Batching)
- 🕒 **ارسال زمان‌بندی‌شده** با استفاده از [Hangfire](https://www.hangfire.io/)
- 🔁 **Retry خودکار** برای نوتیفیکیشن‌های ناموفق
- 🧩 پشتیبانی از **Multi-Tenant**
- 📊 مدیریت کمپین‌ها و وضعیت ارسال هر نوتیفیکیشن
- 🚫 قابلیت **لغو کمپین** حتی در میانه ارسال
- 🔄 API برای مدیریت کمپین، ارسال و لغو ارسال
- 🌍 قابلیت فیلتر کاربران بر اساس شهر، استان، دستگاه و ...

---

## 🏗️ تکنولوژی‌های استفاده شده

- ASP.NET Core 7 Web API
- Entity Framework Core (InMemory برای تست)
- Hangfire (با MemoryStorage)
- Clean Architecture (Domain, Application, Infrastructure, API)

---

## 🧪 اجرای پروژه

### 1. اجرای پروژه

```bash
dotnet run
```

سپس Swagger در مسیر زیر قابل دسترسی است:
```
http://localhost:<port>/swagger
```

### 2. اجرای Hangfire Dashboard

```
http://localhost:<port>/hangfire
```

---

## 🧵 ساختار پروژه

```bash
NotificationService/
├── Domain/
│   └── Models, Enums, Interfaces
├── Application/
│   └── Dispatcher, BatchSender, RetryService
├── Infrastructure/
│   ├── Channels/
│   └── Persistence/
├── API/
│   └── Controllers/
├── Program.cs
├── appsettings.json
```

---

## 🔌 APIهای کلیدی

### 📤 ارسال کمپین جدید

```
POST /api/campaigns/start
```

Body:
```json
{
  "title": "My Campaign",
  "type": "Push",
  "messageTemplate": "سلام از طرف تیم ما!"
}
```

Query Parameter:
```
?userIds=123&userIds=456&userIds=789
```

---

### 🛑 لغو کمپین

```
POST /api/campaigns/cancel/{campaignId}
```

- این عملیات تمام جاب‌های Hangfire مربوط به کمپین را حذف می‌کند.
- فیلد `IsCancelled` کمپین را `true` می‌کند.

---

### ♻️ ارسال مجدد پیام‌های ناموفق

```
POST /api/campaigns/retry-failed
```

---

## ⚙️ تنظیمات Batch

در فایل `appsettings.json`:

```json
"BatchOptions": {
  "Size": 5000
}
```

---

## 📁 TODOهای پیشنهادی

- اضافه کردن احراز هویت (JWT, API Key)
- اضافه کردن Multi-Tenant بر اساس Header یا Claim
- استفاده از SQL Server یا PostgreSQL به جای InMemory

---

## 📚 لایسنس

MIT License © [یوسرا](https://github.com/your-github)
