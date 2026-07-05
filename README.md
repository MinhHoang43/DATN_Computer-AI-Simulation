# DATN_Computer Vision AI Simulation

## ĐỒ ÁN TỐT NGHIỆP

**Đề tài:** Nghiên cứu và mô phỏng hệ thống kiểm tra chất lượng PCB tự động trong dây chuyền sản xuất điện tử sử dụng Computer Vision và AI

**Giảng viên hướng dẫn:** GS.TS. Vũ Toàn Thắng  
**Sinh viên thực hiện:** Nguyễn Minh Hoàng  
**MSSV:** 20240929E  
**Lớp:** KSCS-K69  
**Khoa:** Cơ điện tử  
**Trường:** Trường Cơ khí - Đại học Bách khoa Hà Nội  

---

## 1. Mục đích và yêu cầu

### 1.1. Mục đích

Đề tài xây dựng và mô phỏng một hệ thống kiểm tra chất lượng PCB tự động trong dây chuyền sản xuất điện tử. Hệ thống sử dụng Computer Vision và mô hình YOLO để phát hiện lỗi thiếu linh kiện trên bo mạch ESP32 DevKit.

Phần mô phỏng được thực hiện trên Unity 3D. Quy trình mô phỏng gồm các bước chính: tạo PCB, đưa PCB đến vùng kiểm tra, chụp ảnh, nhận dạng lỗi bằng mô hình AI và phân loại PCB vào khay tương ứng.

Mục tiêu chính của đề tài là kiểm chứng khả năng kết hợp giữa mô hình nhận dạng ảnh và logic điều khiển mô phỏng trong một hệ thống kiểm tra sản phẩm tự động.

### 1.2. Yêu cầu

Các yêu cầu chính của đề tài gồm:

- Tìm hiểu tổng quan về PCB và bo mạch ESP32 DevKit.
- Tìm hiểu Computer Vision và ứng dụng trong kiểm tra sản phẩm công nghiệp.
- Nghiên cứu YOLO Classification và YOLO Detection.
- Nghiên cứu WGAN-GP để khảo sát khả năng sinh thêm ảnh hỗ trợ dữ liệu huấn luyện.
- Xây dựng bộ dữ liệu PCB gồm ảnh đạt yêu cầu và ảnh thiếu linh kiện.
- Chuẩn bị dữ liệu cho hai bài toán: Classification và Detection.
- Gán nhãn bounding box cho dữ liệu YOLO Detection.
- Huấn luyện và đánh giá YOLO Classification.
- Huấn luyện và đánh giá YOLO Detection.
- Thực nghiệm bổ sung dữ liệu bằng ảnh sinh từ WGAN-GP.
- So sánh hai hướng YOLO Classification và YOLO Detection.
- Lựa chọn mô hình phù hợp để tích hợp vào hệ thống mô phỏng.
- Xây dựng project Unity 3D mô phỏng hệ thống kiểm tra và phân loại PCB.
- Tích hợp kết quả nhận dạng của mô hình AI vào logic phân loại sản phẩm trong Unity.
- Lưu trữ code, kết quả train, báo cáo và dữ liệu liên quan để phục vụ kiểm tra, chạy lại và phát triển tiếp.

---

## 2. Nội dung và phạm vi đề tài

### 2.1. Nội dung đề tài

Đồ án tập trung vào các nội dung chính sau:

#### 2.1.1. Xây dựng dữ liệu PCB

Bộ dữ liệu được xây dựng dựa trên bo mạch ESP32 DevKit. Các trạng thái PCB được sử dụng gồm:

- `PCB_OK`
- `PCB_Missing_USB`
- `PCB_Missing_ESP32`
- `PCB_Missing_Resistor`
- `PCB_Missing_Capacitor`
- `PCB_Missing_CP2102`

Dữ liệu được chuẩn bị theo hai dạng:

- Dữ liệu phân loại ảnh cho YOLO Classification.
- Dữ liệu gán nhãn bounding box cho YOLO Detection.

#### 2.1.2. Huấn luyện YOLO Classification

YOLO Classification được dùng để phân loại trạng thái tổng thể của ảnh PCB. Hướng này phù hợp với trường hợp mỗi ảnh chỉ có một trạng thái lỗi rõ ràng.

#### 2.1.3. Huấn luyện YOLO Detection

YOLO Detection được dùng để phát hiện lỗi thiếu linh kiện bằng bounding box. Đầu ra của mô hình gồm tên lớp lỗi, độ tin cậy và vị trí vùng lỗi trên ảnh.

#### 2.1.4. Khảo sát WGAN-GP

WGAN-GP được dùng để sinh thêm ảnh PCB nhằm hỗ trợ mở rộng dữ liệu. Ảnh sinh không được sử dụng tự động toàn bộ, mà cần được lọc chất lượng trước khi đưa vào tập huấn luyện.

#### 2.1.5. Mô phỏng hệ thống trên Unity 3D

Unity 3D được dùng để mô phỏng logic hoạt động của hệ thống kiểm tra PCB. Các đối tượng chính trong mô phỏng gồm băng tải, PCB, camera, vùng kiểm tra, Cylinder và khay phân loại.

#### 2.1.6. Tích hợp mô hình AI vào mô phỏng

Kết quả nhận dạng từ YOLO Detection được sử dụng để hiển thị lỗi trên giao diện Unity và điều khiển quá trình phân loại PCB.

---

### 2.2. Phạm vi đề tài

Trong phạm vi đồ án, hệ thống tập trung vào bài toán phát hiện lỗi thiếu linh kiện trên bo mạch ESP32 DevKit.

Các lỗi được xét gồm:

- Thiếu cổng USB.
- Thiếu module ESP32.
- Thiếu điện trở.
- Thiếu tụ điện.
- Thiếu IC CP2102.

Hệ thống được triển khai ở mức mô phỏng logic trên Unity 3D. Các chuyển động của PCB, băng tải và cơ cấu phân loại được điều khiển bằng script C#. Đề tài chưa đi sâu vào mô phỏng vật lý chi tiết như lực, ma sát, va chạm, động học xi lanh hoặc sai số cơ khí thực tế.

Trong đồ án, YOLO Detection được chọn làm hướng chính để tích hợp vào Unity vì có thể phát hiện nhiều lỗi riêng biệt trên cùng một ảnh và trả về vị trí lỗi cụ thể. YOLO Classification và WGAN-GP được dùng để khảo sát, so sánh và hỗ trợ mở rộng dữ liệu.

---

## 3. Công nghệ, công cụ và ngôn ngữ lập trình

### 3.1. Công nghệ sử dụng

- Computer Vision.
- Deep Learning.
- Object Detection.
- Image Classification.
- Generative Adversarial Network.
- WGAN-GP.
- YOLO Detection.
- YOLO Classification.
- Unity Simulation.

### 3.2. Công cụ sử dụng

- Unity 3D.
- Unity Hub.
- Visual Studio Code.
- Google Colab.
- Roboflow.
- GitHub.
- Ultralytics YOLO.
- PyTorch.
- OpenCV.
- Albumentations.
- ONNX Runtime.

### 3.3. Ngôn ngữ lập trình

- **Python:** dùng cho xử lý dữ liệu, huấn luyện YOLO, huấn luyện WGAN-GP và kiểm thử mô hình.
- **C#:** dùng để lập trình logic mô phỏng trong Unity 3D.

---

## 4. Cấu trúc thư mục repository

Repository hiện được tổ chức theo dạng:

```text
DATN_Computer-AI-Simulation/
├── Assets/
├── Code Colab/
├── Kết quả train/
├── Packages/
├── ProjectSettings/
├── .gitignore
└── README.md
```

### 4.1. `Assets/`

Thư mục chứa các thành phần chính của project Unity, bao gồm:

- Scene mô phỏng.
- Script C#.
- Prefab.
- Material.
- Texture.
- UI.
- Resource dùng trong Unity.
- Các đối tượng mô phỏng như PCB, băng tải, camera, khay phân loại.

### 4.2. `Packages/`

Thư mục chứa thông tin package của Unity project. Khi mở project trên máy khác, Unity sẽ đọc các file trong thư mục này để tải lại các package cần thiết.

### 4.3. `ProjectSettings/`

Thư mục chứa cấu hình của project Unity như phiên bản project, input, tag, layer, render setting và các thiết lập liên quan.

### 4.4. `Code Colab/`

Thư mục chứa các file code dùng để huấn luyện và kiểm thử mô hình trên Google Colab, gồm:

- Code huấn luyện YOLO Classification.
- Code huấn luyện YOLO Detection.
- Code huấn luyện YOLO Classification với dữ liệu bổ sung từ WGAN-GP.
- Code huấn luyện YOLO Detection với dữ liệu bổ sung từ WGAN-GP.
- Code huấn luyện WGAN-GP.
- Code sinh ảnh và chọn ảnh sinh từ WGAN-GP.
- Code export hoặc chạy thử mô hình nếu có.

### 4.5. `Kết quả train/`

Thư mục chứa kết quả thực nghiệm của các mô hình, gồm:

```text
Kết quả train/
├── PCB_Detection_GAN_Result/
├── PCB_Detection_Result/
├── WGAN-GP/
├── YOLO_Classification_Gan_Result/
└── YOLO_Classification_Result/
```

Các file kết quả chính bao gồm:

- `results.csv`
- `results.png`
- `confusion_matrix.png`
- `labels.jpg`
- ảnh dự đoán của mô hình
- biểu đồ loss
- biểu đồ WGAN-GP
- `training_log.csv`


---

## 5. Dataset

Tạo label và box boulding cho dataset YOLO Detection : 

**Link dataset:** 
- [Dataset YOLO Detection trên Roboflow](https://app.roboflow.com/minh-hoang-nguyen-sd6ah/pcb_detection_yolo/browse?queryText=&pageSize=50&startingIndex=0&browseQuery=true) 

Dataset đầy đủ được lưu tại Google Drive:

**Link dataset:**  
- [Dataset YOLO Detection gốc](https://drive.google.com/file/d/1hs-7suoL_j-hlvvkl3_FN_zDDER3LPXD/view?usp=sharing)
- [Dataset YOLO Classification gốc](https://drive.google.com/file/d/1vxPNlVzcbJImr6Ye1AIMW70LuZxU637T/view?usp=sharing)
- [Dataset YOLO Detection + WGAN-GP](https://drive.google.com/file/d/18o4ocKOuWSi552JuA3A69XHT1U5mdXVH/view?usp=sharing)
- [Dataset YOLO Classification + WGAN-GP](https://drive.google.com/file/d/1QiZDep6uxXuiNA7sp7erJE7Ym2R8P-Gz/view?usp=sharing)
- [Dataset train WGAN-GP cho lớp Missing_Resistor](https://drive.google.com/file/d/1gI7ZqGyIXkTiJk2PKZ_drLfzBAhMVn3r/view?usp=sharing)
- [Dataset train WGAN-GP cho lớp Missing_Capacitor](https://drive.google.com/file/d/15Hd-TqaAbu7WLSaD_F6eb1M7iR9QFXLE/view?usp=sharing)
- [Dataset ảnh sinh bởi WGAN-GP sau khi lọc để đưa vào train](https://drive.google.com/drive/folders/1UJZyAO_MUhPnsN0Zu1mkJFoXOVowIgD0?usp=sharing)

Dataset gồm các lớp chính:

- `PCB_OK`
- `PCB_Missing_USB`
- `PCB_Missing_ESP32`
- `PCB_Missing_Resistor`
- `PCB_Missing_Capacitor`
- `PCB_Missing_CP2102`

Dữ liệu được chuẩn bị cho hai bài toán:

### 5.1. YOLO Classification

Dữ liệu được chia theo thư mục lớp. Mỗi ảnh có một nhãn tương ứng với trạng thái PCB.

### 5.2. YOLO Detection

Dữ liệu được gán nhãn bounding box cho các vùng linh kiện bị thiếu. Mỗi ảnh có file nhãn tương ứng theo định dạng YOLO.

Ngoài dữ liệu gốc, đề tài có khảo sát thêm dữ liệu sinh bởi WGAN-GP. Ảnh sinh chỉ được sử dụng sau khi lọc thủ công để tránh đưa các ảnh sai cấu trúc vào quá trình huấn luyện.

---


## 6. Cách mở project Unity

### 6.1. Yêu cầu

- Cài Unity Hub.
- Cài đúng phiên bản Unity 6.4 (6000.4.4f1) tương thích với project.
- Clone hoặc tải repository về máy.
- Đảm bảo thư mục project có các thư mục chính: `Assets`, `Packages`, `ProjectSettings`.

### 6.2. Cách mở

1. Mở Unity Hub.
2. Chọn **Add project**.
3. Trỏ đến thư mục chứa project đã tải về.
4. Chờ Unity tự tạo lại thư mục `Library` và `PackageCache`.
5. Mở Scene chính trong thư mục `Assets/Scenes`.
6. Nhấn **Play** để chạy mô phỏng.

### 6.3. Lưu ý

Các thư mục sau không được đưa lên GitHub vì Unity có thể tự sinh lại:

```text
Library/
Logs/
Temp/
Obj/
Build/
Builds/
```

Do đó, khi tải project về, Unity có thể mất một thời gian để import lại project trong lần mở đầu tiên.

---

## 7. Cách chạy code Colab

Các file code trong thư mục `Code Colab/` được thiết kế để chạy trên Google Colab.

Quy trình chung:

1. Mở file notebook tương ứng trong Google Colab.
2. Mount Google Drive .
3. Cập nhật lại đường dẫn dataset trong notebook.
4. Cài các thư viện cần thiết.
5. Chạy các cell theo thứ tự.
6. Sau khi train xong, tải về các file kết quả như `results.csv`, `results.png`, `confusion_matrix.png`, `best.pt` hoặc file export ONNX .

Các nhóm notebook chính:

```text
Code Colab/
├── YOLO Classification
├── YOLO Detection
├── YOLO Classification + WGAN-GP
├── YOLO Detection + WGAN-GP
├── pt-to-onnx
├── WGAN-GP-Resistor
└── WGAN-GP-Capacitor
```

---

## 8. Kết quả chính

### 8.1. YOLO Classification

YOLO Classification cho kết quả tốt khi mỗi ảnh chỉ có một trạng thái lỗi rõ ràng. Tuy nhiên, hướng này chỉ trả về một nhãn cho toàn ảnh nên chưa phù hợp nếu hệ thống cần biết vị trí linh kiện bị thiếu.

Nếu PCB có nhiều lỗi cùng lúc, Classification phải tạo thêm nhiều lớp tổ hợp lỗi, làm bộ dữ liệu phức tạp hơn.

### 8.2. YOLO Detection

YOLO Detection phù hợp hơn với bài toán kiểm tra PCB thiếu linh kiện vì mô hình trả về:

- tên lớp lỗi;
- độ tin cậy;
- tọa độ bounding box;
- số lượng lỗi trên cùng một ảnh.

Đây là đầu ra cần thiết để tích hợp vào Unity, hiển thị vị trí lỗi và điều khiển quá trình phân loại PCB.

### 8.3. WGAN-GP

WGAN-GP được dùng để khảo sát khả năng sinh thêm ảnh PCB. Kết quả cho thấy mô hình có thể tạo ra một số ảnh mang đặc điểm tổng thể của PCB, nhưng chất lượng ảnh chưa đồng đều. Vì vậy, ảnh sinh cần được lọc trước khi đưa vào tập huấn luyện.

### 8.4. Unity 3D Simulation

Mô phỏng Unity đã thể hiện được luồng kiểm tra và phân loại PCB ở mức logic:

1. PCB được tạo trong mô phỏng.
2. PCB di chuyển đến vùng kiểm tra.
3. Camera chụp ảnh bề mặt PCB.
4. Mô hình YOLO Detection xử lý ảnh.
5. Kết quả nhận dạng được hiển thị trên giao diện.
6. PCB được phân loại vào khay tương ứng.

---
