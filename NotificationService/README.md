# NotificationService

سرویسی برای ارسال نوتیفیکیشن با قابلیت‌های زیر:

- پشتیبانی از چند tenant (Multi-Tenant)
- محدودیت نرخ ارسال (Rate Limiting)
- استفاده از Hangfire برای صف‌بندی کارها
- ذخیره در SQL با EF Core
- پشتیبانی از ارسال SMS و Push Notification
- Docker برای اجرای ساده سرویس
- دات‌نت 9 و استفاده از OpenAPI (Swagger)

## راه‌اندازی

1. پروژه را کلون کنید.
2. با استفاده از Docker سرویس را اجرا کنید:

```
docker build -t notificationservice .
docker run -p 8080:80 notificationservice
```

3. درخواست‌ها را با هدر `X-Tenant-ID` ارسال کنید.

## API

- ارسال نوتیفیکیشن:

```
POST /api/notifications
Headers:
  X-Tenant-ID: tenant1
Body:
{
  "userId": "user123",
  "type": "Push",
  "message": "سلام!"
}
```

- مشاهده تاریخچه:

```
GET /api/notifications/history
Headers:
  X-Tenant-ID: tenant1
```

- تلاش مجدد:

```
POST /api/notifications/retry-failed
Headers:
  X-Tenant-ID: tenant1
```
