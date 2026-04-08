# Book Management System (ASP.NET Core MVC + Web API)
本專案使用 ASP.NET Core MVC + Web API + EF Core 開發的書籍管理系統，提供書籍 CRUD、搜尋 / 排序 / 分頁、封面圖片上傳與顯示，以及基本資料驗證、logging記錄功能。

此專案同時包含：
MVC 頁面功能：使用者透過網頁介面進行書籍管理
Web API 功能：提供前後端分離、第三方系統串接使用

## Features
### MVC
- 書籍資料 CRUD
- 搜尋 / 排序 / 分頁 
- 書籍封面圖片上傳與顯示（上傳限制：最大5MB，僅允許 `.jpg` / `.jpeg` / `.png`）
- 書籍封面圖片儲存於 `wwwroot/upload`資料夾
- 基本資料驗證
- Service / Controller 分層
- EF Core Migration 建置/更新資料庫  

### Web API
可透過 Swagger 測試 API
- 取得書籍清單 API
- 單筆書籍查詢 API
- 新增書籍資料 API
- 編輯書籍資料 API
- 刪除書籍資料 API



## Tech Stack
- ASP.NET Core MVC (.NET 10)
- C#
- Entity Framework Core
- SQL Server 
- Bootstrap


## Project Structure 
- `Controllers/`
- `Services/`
- `Data/`（DbContext）
- `Migrations/`
- `Models/`（Entities、ViewModels、DTO、Inputs）
- `Views/`
- `wwwroot/upload/` （書籍封面圖片儲存位置）



## Prerequisites
- .NET SDK: 10.0 (10.0.104 tested)
- SQL Server（或 LocalDB）
- EF Core packages:
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools
- Swashbuckle.AspNetCore


## Getting Started
1. 下載 Demo 圖片（`docs/demo-images`）
2. 複製 `appsettings.json.template` → 改名成 `appsettings.json`，並設定連線字串
3. 執行程式
4. 新建書籍或編輯既有書籍，上傳封面圖片（圖片會存到 `wwwroot/upload/`）



## Notes
- 執行前請建立 `wwwroot/upload/` 資料夾，並確認應用程式有寫入權限。
- 上傳檔案限制在 `BCoverStorage.cs` 中設定（目前最大5MB，允許 `.jpg`、`.jpeg`、`.png`）。


## Future Improvements
- CSV 匯入 / 匯出
- 身份驗證與角色權限
- 單元測試
- 圖片刪除與更新流程優化
