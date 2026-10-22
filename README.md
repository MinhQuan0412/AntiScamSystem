# AntiScam System - Hệ thống Cảnh báo Lừa đảo & Hỗ trợ An toàn Du lịch

## Giới thiệu
Ứng dụng web giúp người dùng:
- Kiểm tra tin nhắn/tình huống nghi ngờ lừa đảo
- Tra cứu số điện thoại/số tài khoản bị báo cáo
- Xem thư viện kịch bản lừa đảo và phương án cảnh giác
- Lập lộ trình du lịch và nhận cảnh báo khi lệch tuyến

## Kiến trúc hệ thống

```
AntiScamSystem/
├── src/
│   ├── AntiScam.API/              # ASP.NET Core 8 Web API
│   ├── AntiScam.Application/      # Application layer (CQRS, Services)
│   ├── AntiScam.Domain/           # Domain entities, interfaces
│   ├── AntiScam.Infrastructure/   # EF Core, MySQL, External services
│   └── AntiScam.ML.Service/       # Python FastAPI (ML classification)
├── tests/
│   ├── AntiScam.UnitTests/
│   └── AntiScam.IntegrationTests/
├── docker-compose.yml
├── docker-compose.override.yml
└── nginx/
```

## Công nghệ sử dụng

| Hạng mục | Công nghệ |
|----------|-----------|
| Backend | C# / ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 + Pomelo (MySQL) |
| Auth | ASP.NET Core Identity + JWT + Refresh Token |
| Background Jobs | BackgroundService / Hangfire |
| Frontend | React 18 + TypeScript + Vite + Tailwind CSS |
| Maps | Leaflet + OpenStreetMap |
| Database | MySQL 8 |
| ML Service | Python FastAPI + scikit-learn |
| Notifications | SMTP Email + Telegram Bot API |
| Reports | ClosedXML (Excel) + QuestPDF (PDF) |
| Logging | Serilog |
| Tests | xUnit + Moq |
| Deployment | Docker + Docker Compose + Nginx |

## Chức năng

| Mã | Chức năng | Mức ưu tiên |
|----|-----------|-------------|
| F01 | Đăng nhập, đăng ký, phân quyền | Bắt buộc |
| F02 | Quản lý thư viện kịch bản lừa đảo | Bắt buộc |
| F03 | Thu thập, gán nhãn, tiền xử lý dữ liệu | Bắt buộc |
| F04 | Huấn luyện, đánh giá mô hình phân lớp | Bắt buộc |
| F05 | Kiểm tra tin nhắn nghi ngờ | Bắt buộc |
| F06 | Báo cáo & tra cứu số điện thoại/tài khoản | Bắt buộc |
| F07 | Cập nhật kịch bản mới, huấn luyện lại | Bắt buộc |
| F08 | Kiểm duyệt báo cáo | Nên có |
| F09 | Danh bạ số khẩn theo địa phương | Nên có |
| F10 | Lộ trình du lịch, cảnh báo lệch tuyến | Nên có |
| F11 | Gửi cảnh báo người thân + SOS | Mở rộng |
| F12 | Thống kê, xuất Excel/PDF | Nên có |

## Hướng dẫn chạy

### Yêu cầu
- Docker & Docker Compose
- .NET 8 SDK (nếu phát triển local)
- Python 3.11+ (nếu phát triển ML service local)

### Chạy với Docker
```bash
docker-compose up -d
```

### Chạy local
```bash
# Backend API
cd src/AntiScam.API
dotnet run

# ML Service
cd src/AntiScam.ML.Service
pip install -r requirements.txt
uvicorn main:app --reload --port 8001
```

## Biến môi trường
Xem file `.env.example` để cấu hình.

## API Documentation
Swagger UI: `http://localhost:5000/swagger`
