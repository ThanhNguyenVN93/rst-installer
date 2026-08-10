# Intel RST Auto Installer

Tự động nhận diện thế hệ CPU/chipset Intel, cấu hình SATA/VMD hiện tại, và tải + cài đúng bản driver Intel Rapid Storage Technology (RST) tương ứng — không cần tự tra cứu thủ công trên trang Intel.

## Tính năng

- Nhận diện CPU/chipset qua WMI, có fallback sang SetupAPI nếu WMI không đủ thông tin.
- Hỗ trợ từ Gen 8 tới Gen 15 và dòng Core Ultra (Series 1/2/3), bao gồm cả các model có tiền tố tier như `Ultra X9`.
- Phát hiện trạng thái **VMD** (Volume Management Device) và **SATA Controller Mode** (AHCI/RAID) thực tế trên máy.
- Tự so sánh version RST đã cài với bản khuyến nghị theo đúng chipset (so sánh đầy đủ 5 phần version).
- Tải driver có **verify SHA256 checksum** trước khi cài — không cài file tải về nếu checksum không khớp.
- Có **mirror dự phòng**: nếu link tải chính từ Intel không truy cập được, tự động chuyển sang bản mirror trên GitHub Releases của repo này.
- Bảng mapping CPU → driver (`driver-mapping.json`) có thể cập nhật từ xa qua GitHub mà không cần phát hành bản mới của tool.
- Ghi log ra file tự động mỗi lần chạy, kèm nút xuất/copy log và gửi báo lỗi (email hoặc GitHub Issues) trực tiếp từ giao diện.

## Yêu cầu hệ thống

- Windows 10/11 (64-bit khuyến nghị).
- Quyền Administrator (bắt buộc để cài driver hệ thống).
- .NET Framework 4.7.2 trở lên (có sẵn mặc định trên Windows 10/11).

## Cách dùng

1. Tải `Intel RST Auto Installer.exe` ở [Release v1.0.0](../../releases/tag/v1.0.0) — **chỉ tải đúng bản này**, các release khác (VD `driver-packages-v1`) là file mirror nội bộ, không phải app.
2. Chạy `Intel RST Auto Installer.exe` (sẽ tự yêu cầu quyền Admin qua UAC).
3. Xem thông tin CPU/VMD/SATA đã nhận diện và version RST khuyến nghị.
4. Bấm **Download & Install** nếu có bản mới hơn bản đang cài.

## Build từ source

Yêu cầu Visual Studio 2019+ hoặc MSBuild có sẵn (.NET Framework 4.7.2 targeting pack).

```bash
msbuild "Intel RST Auto Installer.csproj" /t:Build /p:Configuration=Release
```

File thực thi sẽ nằm ở `bin\Release\Intel RST Auto Installer.exe`.

## Báo lỗi

Dùng nút **Report an issue** trong app, hoặc tạo [Issue](../../issues) trực tiếp trên repo này.

## Ủng hộ

Nếu tool hữu ích với bạn: [ko-fi.com/thanhnguyen150993](https://ko-fi.com/thanhnguyen150993)

## License

[MIT](LICENSE)
