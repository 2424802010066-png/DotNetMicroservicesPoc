-- =============================================================================
--  BAI THUC HANH SO 4 - Task #2.7: Tao cac co so du lieu cho he thong
--  Moi microservice so huu CSDL rieng (database-per-service) theo nguyen tac
--  bounded context cua DDD - dung nhu kien truc da thiet ke o Bai thuc hanh so 3.
--
--  Sinh vien: Le Huu Anh Tai - 2424802010066 - D24CNTT05
--
--  GHI CHU: de bai yeu cau lay file createdatabases.sql trong postgres.rar.
--  File nen do khong duoc dinh kem tren Elearning nen file nay duoc viet lai
--  theo danh sach microservice o phu luc cua de bai.
-- =============================================================================

-- 3) Dich vu xac thuc - nguoi dung, vai tro, quyen tren san pham
CREATE DATABASE authservice;
COMMENT ON DATABASE authservice IS 'Auth Service - nguoi dung, vai tro, cap phat JWT';

-- 4) Dich vu Chat - lich su tro chuyen giua cac dai ly (SignalR)
CREATE DATABASE chatservice;
COMMENT ON DATABASE chatservice IS 'Chat Service - cuoc tro chuyen, tin nhan';

-- 5) Dich vu Dinh gia - bieu phi, he so rui ro
CREATE DATABASE pricingservice;
COMMENT ON DATABASE pricingservice IS 'Pricing Service - bieu phi, tham so dinh phi';

-- 6) Dich vu Chinh sach - bao gia va hop dong bao hiem (dich vu chinh)
CREATE DATABASE policyservice;
COMMENT ON DATABASE policyservice IS 'Policy Service - bao gia (offer) va hop dong (policy)';

-- 7) Dich vu Tim kiem Chinh sach - du lieu doc, chi muc chinh nam o ElasticSearch
CREATE DATABASE policysearchservice;
COMMENT ON DATABASE policysearchservice IS 'Policy Search Service - mo hinh doc, con tro chi muc ES';

-- 8) Dich vu Thanh toan - tai khoan chinh sach, khoan thanh toan du kien
CREATE DATABASE paymentservice;
COMMENT ON DATABASE paymentservice IS 'Payment Service - tai khoan chinh sach, thanh toan';

-- 9) Dich vu San pham - danh muc san pham bao hiem
CREATE DATABASE productservice;
COMMENT ON DATABASE productservice IS 'Product Service - danh muc san pham bao hiem';

-- 10) Dich vu Tai lieu - mau chung tu, tep PDF da sinh (JS Report)
CREATE DATABASE documentservice;
COMMENT ON DATABASE documentservice IS 'Document Service - mau chung tu, chung chi PDF';
