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
| `XxxService` | **Triển khai** — command handler, query handler, notification handler, mô hình miền; các *bộ điều hợp*: Controller (HTTP đến), Listener (sự kiện qua hàng đợi), REST client (HTTP đi). | ASP.NET Core Web API |
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

→ **8 microservice × 3 project = 24 project** trong `DotNetMicroservicesPoc.sln`.

```
DotNetMicroservicesPoc/
├─ DotNetMicroservicesPoc.sln       24 project
├─ docker-compose.yml               toàn bộ hạ tầng
├─ src/                             8 microservice × 3 project
├─ postgres/                        createdatabases.sql + backups/
├─ eureka-server/                   Task #2.6
├─ scripts/                         Task #2.8
├─ Web/                             Task #2.9  (SPA VueJS)
├─ BlazorWasmClient/                Task #2.10
└─ AgentPortalApiGateway/           Task #2.11 (Ocelot)
```

## 2. Cài đặt môi trường (Task #2)

| # | Yêu cầu | Trạng thái |
|---|---|---|
| 1 | MediatR + đăng ký DI | ✅ MediatR 12.4.1 (8 project triển khai), MediatR.Contracts 2.0.1 (8 project .Api) |
| 2 | PostgreSQL ≥ 10.0 | ✅ PostgreSQL 16 |
| 3 | RabbitMQ | ✅ RabbitMQ 3.13.7 (Erlang 26.2.5.16) |
| 4 | Elasticsearch ≥ 6.0 | ✅ Elasticsearch 7.17.26 |
| 5 | Docker + Docker Compose | ✅ Docker 29.7.2, Compose v5.4.0 |
| 6 | Thư mục `eureka-server` | ✅ |
| 7 | Tạo các CSDL | ✅ 4 CSDL theo `postgres/createdatabases.sql` |
| 8 | Thư mục `scripts` | ✅ |
| 9 | Thư mục `Web` | ✅ |
| 10 | Thư mục `BlazorWasmClient` | ✅ |
| 11 | Thư mục `AgentPortalApiGateway` | ✅ |
| — | **Build hệ thống** | ✅ **Build succeeded — 0 Warning(s), 0 Error(s)** |

### Chạy toàn bộ hạ tầng

```bash
docker compose up -d
```

| Dịch vụ | Cổng | Thông tin đăng nhập |
|---|---|---|
| PostgreSQL | **5433** | `postgres` / `postgres` (và `lab_user` / `lab_pass`) |
| RabbitMQ (AMQP) | **5673** | |
| RabbitMQ (quản trị web) | **15673** | `guest` / `guest` |
| Elasticsearch | **9200** | |

### Cơ sở dữ liệu

`postgres/createdatabases.sql` tạo user `lab_user` và 4 CSDL:
`lab_netmicro_payments`, `lab_netmicro_jobs` (Hangfire), `lab_netmicro_policy`, `lab_netmicro_pricing`.
File backup để nộp nằm ở `postgres/backups/` (định dạng custom, restore bằng `pg_restore` hoặc pgAdmin).

### Build

```bash
dotnet build DotNetMicroservicesPoc.sln
```

---

## 3. Những chỗ lệch so với đề bài (đã chuẩn hoá cho chạy được)

**1. Nguồn của 6 thư mục bổ sung (Task #2.6 → #2.11) — không lệch, chỉ là ghi chú xác thực.**
Sáu thư mục `eureka-server`, `postgres`, `scripts`, `Web`, `BlazorWasmClient`,
`AgentPortalApiGateway` được giải nén **trực tiếp từ 6 file `.rar` thầy đính kèm trên Elearning**
— tổng cộng **159 file, không sửa một ký tự nào**.
Có đối chiếu thêm bằng SHA-256 từng file với dự án mã nguồn mở
[`asc-lab/dotnetcore-microservices-poc`](https://github.com/asc-lab/dotnetcore-microservices-poc)
của Altkom Software & Consulting — chính là công ty được nhắc tới trong phần mở đầu của đề bài:
**trùng khớp 100% trên cả 159 file**. Vậy case study của đề được xây trên đúng dự án đó, nên khi
cần tra cứu cách các service thật giao tiếp với nhau thì đọc được trực tiếp mã nguồn dự án gốc.
`postgres/createdatabases.sql` dùng đúng bản của thầy (giữ nguyên xuống dòng LF).

**2. `AgentPortalApiGateway` và `BlazorWasmClient` nằm trong repo nhưng KHÔNG nằm trong solution build.**
Hai project này là mã nguồn hoàn chỉnh, tham chiếu tới các kiểu dữ liệu thật trong những project
`.Api` (ví dụ `CreateOfferResult`, `PersonDto`, `GetPolicyDetailsQueryResult`). Trong khi đó Task #1
quy định rõ *"chỉ tạo tên, chưa viết code"*, nên các project `.Api` ở đây còn rỗng — đưa hai project
kia vào solution sẽ sinh 57 lỗi biên dịch. Để giữ **build sạch 0 lỗi** đúng yêu cầu chấm điểm,
thư mục vẫn được add vào repo nhưng `.csproj` không được add vào `.sln`. Khi nào các project `.Api`
được viết code thật (bài thực hành sau) thì thêm vào là build được.

**3. Không còn lớp `Startup.cs`.**
Đề gợi ý `services.AddMediatR()` trong `Startup.ConfigureServices`. Từ .NET 6, template Web API
dùng *minimal hosting* nên không sinh `Startup.cs`. Chỗ đăng ký tương đương là `builder.Services`
trong `Program.cs`.

**4. `Install-Package MediatR` → `dotnet add package MediatR`.**
`Install-Package` là lệnh của Package Manager Console trong Visual Studio; khi thao tác bằng dòng
lệnh thì lệnh tương đương là `dotnet add package`. Kết quả ghi vào `.csproj` giống hệt.

**5. MediatR ghim bản 12.4.1.**
Từ MediatR 13 (2025) thư viện chuyển sang giấy phép thương mại. Bản 12.4.1 là bản **MIT** cuối cùng.

**6. Cổng hạ tầng khác mặc định.**
Máy đã có sẵn PostgreSQL 16 ở 5432 và một RabbitMQ của đồ án khác ở 5672. Để không đụng độ, hạ tầng
bài này dùng 5433 và 5673/15673 — khai báo đầy đủ trong `docker-compose.yml`.

**7. `dotnet new sln` trên .NET 10 mặc định sinh `.slnx`.**
Đã dùng `--format sln` để tạo file `.sln` cổ điển cho Visual Studio / Rider bản cũ đọc được.
