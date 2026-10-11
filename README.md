# DotNetMicroservicesPoc — Bài thực hành số 4

**Case study: Cài đặt kiến trúc Microservices trên .NET Core (bài toán đại lý bảo hiểm)**
Phần 2 — Cài đặt môi trường

| | |
|---|---|
| Sinh viên | Lê Hữu Anh Tài |
| MSSV | 2424802010066 |
| Lớp | D24CNTT05 |
| Nhóm | Nhóm 11 |
| Môn | Thực hành Kiến trúc và Thiết kế phần mềm |
| GVHD | ThS. Cao Thanh Xuân |
| Ngày | 11/10/2026 |

---

## 1. Quy tắc tổ chức source code (Task #1)

Mỗi microservice có **đúng 3 project** theo quy tắc đề bài:

| Project | Vai trò | Loại |
|---|---|---|
| `XxxService.Api` | **Định nghĩa API** — các lệnh (Command), truy vấn (Query), sự kiện (Event) và lớp truyền dữ liệu (DTO) mà dịch vụ phơi ra ngoài. Đây là phần *cổng* (port) trong kiến trúc Ports & Adapters. | Class Library |
| `XxxService` | **Triển khai** — command handler, query handler, notification handler, mô hình miền; các *bộ điều hợp* (adapter): Controller cho HTTP đến, Listener cho sự kiện qua hàng đợi, REST client cho HTTP đi. | ASP.NET Core Web API |
| `XxxService.Test` | **Kiểm thử** — unit test và integration test. | xUnit |

Chiều phụ thuộc: `XxxService.Test` → `XxxService` → `XxxService.Api`
(project triển khai phụ thuộc vào định nghĩa API, **không bao giờ ngược lại**).

### Tám microservice theo phụ lục đề bài

| # | Microservice | Chức năng |
|---|---|---|
| 3 | `AuthService` | Xác thực người dùng, cấp JWT |
| 4 | `ChatService` | Trò chuyện giữa nhân viên hỗ trợ (SignalR) |
| 5 | `PricingService` | Tính giá sản phẩm bảo hiểm |
| 6 | `PolicyService` | Dịch vụ chính — tạo báo giá và hợp đồng |
| 7 | `PolicySearchService` | Tìm kiếm hợp đồng, lập chỉ mục trên Elasticsearch |
| 8 | `PaymentService` | Tài khoản chính sách, khoản thanh toán |
| 9 | `ProductService` | Danh mục sản phẩm bảo hiểm |
| 10 | `DocumentService` | Sinh chứng chỉ PDF (JS Report) |

→ **8 microservice × 3 project = 24 project** trong solution `DotNetMicroservicesPoc.sln`.

```
DotNetMicroservicesPoc/
├─ DotNetMicroservicesPoc.sln
├─ docker-compose.yml
├─ postgres/
│  └─ createdatabases.sql
└─ src/
   ├─ AuthService/
   │  ├─ AuthService.Api/        Commands/ Queries/ Events/ Dtos/
   │  ├─ AuthService/            CommandHandlers/ QueryHandlers/ EventHandlers/
   │  │                          Domain/ Controllers/ Listeners/ RestClients/
   │  └─ AuthService.Test/       UnitTests/ IntegrationTests/
   └─ … 7 microservice còn lại cùng cấu trúc
```

## 2. Cài đặt môi trường (Task #2)

| # | Yêu cầu | Trạng thái |
|---|---|---|
| 1 | MediatR + đăng ký DI | ✅ MediatR 12.4.1 ở 8 project triển khai, MediatR.Contracts 2.0.1 ở 8 project định nghĩa API |
| 2 | PostgreSQL ≥ 10.0 | ✅ PostgreSQL 16 |
| 3 | RabbitMQ | ✅ rabbitmq:3-management |
| 4 | Elasticsearch ≥ 6.0 | ✅ Elasticsearch 7.17.26 |
| 5 | Docker + Docker Compose | ✅ Docker 29.7.2, Compose v5.4.0 |
| 7 | Tạo các CSDL | ✅ 8 CSDL (xem `postgres/createdatabases.sql`) |

### Chạy toàn bộ hạ tầng

```bash
docker compose up -d
```

| Dịch vụ | Cổng | Ghi chú |
|---|---|---|
| PostgreSQL | **5433** | user/pass: `postgres` / `postgres` |
| RabbitMQ (AMQP) | **5673** | |
| RabbitMQ (giao diện quản trị) | **15673** | `guest` / `guest` |
| Elasticsearch | **9200** | |

### Build

```bash
dotnet build DotNetMicroservicesPoc.sln
```

Kết quả: **Build succeeded — 0 Warning(s), 0 Error(s)**.

---

## 3. Những chỗ lệch so với đề bài (đã chuẩn hoá cho chạy được)

**1. Không còn lớp `Startup.cs`.**
Đề gợi ý `services.AddMediatR()` trong `Startup.ConfigureServices`. Từ .NET 6 trở đi,
template Web API dùng *minimal hosting* nên không sinh ra `Startup.cs` nữa. Chỗ đăng ký
tương đương là `builder.Services` ngay trong `Program.cs` — đã làm đúng như vậy.

**2. `Install-Package MediatR` → `dotnet add package`.**
`Install-Package` là lệnh của Package Manager Console trong Visual Studio. Khi thao tác bằng
dòng lệnh thì lệnh tương đương là `dotnet add package MediatR`. Kết quả trong `.csproj` giống hệt.

**3. MediatR ghim bản 12.4.1.**
Từ MediatR 13 (2025), thư viện chuyển sang giấy phép thương mại. Bản 12.4.1 là bản **MIT**
cuối cùng — dùng bản này để dự án không vướng ràng buộc bản quyền.

**4. Cổng hạ tầng khác mặc định.**
Máy đã có sẵn PostgreSQL 16 ở cổng 5432 và một RabbitMQ của đồ án khác ở 5672. Để không
đụng độ, hạ tầng của bài này dùng 5433 và 5673/15673.

**5. `createdatabases.sql` được viết lại.**
Đề yêu cầu lấy file này trong `postgres.rar`. File nén đó không được đính kèm cùng đề trên
Elearning, nên file SQL được viết lại theo đúng danh sách microservice ở phụ lục đề bài,
mỗi dịch vụ một CSDL riêng (*database-per-service*).

**6. Các thư mục từ file .rar chưa được add.**
Các mục 2.6 và 2.8 → 2.11 yêu cầu add `eureka-server`, `scripts`, `Web`, `BlazorWasmClient`,
`AgentPortalApiGateway` lấy từ các file `.rar`. Những file nén này không có trong phần đính
kèm của đề trên Elearning nên chưa thực hiện được. Cấu trúc solution đã chừa sẵn chỗ để add
vào khi có file.
