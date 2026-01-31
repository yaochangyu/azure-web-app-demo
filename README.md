# Azure Web App - ASP.NET Core API

一個簡單的 ASP.NET Core Web API 專案，展示如何自動部署到 Azure App Service。

A simple ASP.NET Core Web API project demonstrating automated deployment to Azure App Service.

## 📋 專案簡介 | Project Overview

這是一個使用 ASP.NET Core 10.0 建立的 RESTful API 專案，包含：
- 天氣預報 API
- 版本資訊 API
- 自動化 GitHub Actions CI/CD 部署到 Azure

This is a RESTful API project built with ASP.NET Core 10.0, featuring:
- Weather Forecast API
- Version Information API
- Automated GitHub Actions CI/CD deployment to Azure

## 🚀 快速開始 | Quick Start

### 前置需求 | Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 或 VS Code (可選)
- Git

### 本地開發 | Local Development

1. **克隆專案 | Clone the repository**

```bash
git clone https://github.com/yaochangyu/azure-web-app.git
cd azure-web-app
```

2. **還原相依套件 | Restore dependencies**

```bash
dotnet restore AspNetCoreApp/AspNetCoreApp.csproj
```

3. **執行應用程式 | Run the application**

```bash
cd AspNetCoreApp
dotnet run
```

應用程式將在 `http://localhost:5074` 和 `https://localhost:7132` 啟動。

The application will start at `http://localhost:5074` and `https://localhost:7132`.

4. **測試 API | Test the API**

開啟瀏覽器或使用 curl：

Open your browser or use curl:

```bash
# 天氣預報 API | Weather Forecast API
curl http://localhost:5074/api/weatherforecast

# 版本資訊 API | Version Information API
curl http://localhost:5074/api/version
```

## 📚 API 端點 | API Endpoints

### 1. Weather Forecast API

取得未來 5 天的天氣預報資料（模擬數據）

Get weather forecast data for the next 5 days (simulated data)

```
GET /api/weatherforecast
```

**回應範例 | Response Example:**

```json
[
  {
    "date": "2024-02-01",
    "temperatureC": 15,
    "temperatureF": 58,
    "summary": "Mild"
  },
  {
    "date": "2024-02-02",
    "temperatureC": 20,
    "temperatureF": 67,
    "summary": "Warm"
  }
]
```

### 2. Version Information API

取得應用程式的版本資訊

Get application version information

```
GET /api/version
```

**回應範例 | Response Example:**

```json
{
  "version": "1.0.0",
  "buildDate": "2024-01-31 10:30:45",
  "environment": "Development"
}
```

## 🏗️ 專案結構 | Project Structure

```
azure-web-app/
├── .github/
│   └── workflows/
│       └── azure-deploy.yml      # GitHub Actions 自動部署設定
├── AspNetCoreApp/
│   ├── Controllers/
│   │   ├── WeatherForecastController.cs
│   │   └── VersionController.cs
│   ├── Models/
│   ├── Program.cs                # 應用程式入口點
│   └── AspNetCoreApp.csproj      # 專案設定檔
├── DEPLOYMENT_GUIDE.md           # 詳細部署指南
└── README.md                     # 本文件
```

## 🔧 開發指令 | Development Commands

```bash
# 建置專案 | Build the project
dotnet build AspNetCoreApp/AspNetCoreApp.csproj

# 執行專案 | Run the project
dotnet run --project AspNetCoreApp/AspNetCoreApp.csproj

# 發佈專案 (Release 模式) | Publish the project (Release mode)
dotnet publish AspNetCoreApp/AspNetCoreApp.csproj --configuration Release --output ./publish

# 測試專案 | Test the project
dotnet test
```

## 📦 部署 | Deployment

### 自動部署 | Automated Deployment

當程式碼推送到 `main` 分支時，GitHub Actions 會自動：
1. 建置應用程式
2. 執行測試
3. 部署到 Azure App Service

When code is pushed to the `main` branch, GitHub Actions will automatically:
1. Build the application
2. Run tests
3. Deploy to Azure App Service

### 手動部署 | Manual Deployment

詳細的部署說明請參考 [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)

For detailed deployment instructions, see [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)

## 🌐 線上演示 | Live Demo

應用程式已部署到 Azure：

The application is deployed on Azure:

**URL:** https://azure-web-app-api.azurewebsites.net

**範例 API 呼叫 | Example API Calls:**

```bash
# 天氣預報 | Weather Forecast
curl https://azure-web-app-api.azurewebsites.net/api/weatherforecast

# 版本資訊 | Version Information
curl https://azure-web-app-api.azurewebsites.net/api/version
```

## 🛠️ 技術堆疊 | Technology Stack

- **Framework:** ASP.NET Core 10.0
- **Language:** C# 13
- **API Style:** RESTful
- **Deployment:** Azure App Service
- **CI/CD:** GitHub Actions
- **Cloud Platform:** Microsoft Azure

## 📖 相關文件 | Related Documentation

- [部署指南 (Deployment Guide)](./DEPLOYMENT_GUIDE.md) - 完整的自動和手動部署說明
- [ASP.NET Core 官方文檔](https://docs.microsoft.com/aspnet/core/)
- [Azure App Service 文檔](https://docs.microsoft.com/azure/app-service/)

## 📄 授權 | License

此專案僅供學習和演示目的使用。

This project is for learning and demonstration purposes only.

## 👤 作者 | Author

yaochangyu

---

## 常見問題 | FAQ

### Q: 如何在本地啟用 HTTPS？
**A:** 執行以下命令信任開發憑證：
```bash
dotnet dev-certs https --trust
```

### Q: How to enable HTTPS locally?
**A:** Run the following command to trust the development certificate:
```bash
dotnet dev-certs https --trust
```

### Q: 如何查看 OpenAPI/Swagger 文檔？
**A:** 在開發模式下運行應用，然後訪問：
```
http://localhost:5074/openapi/v1.json
```

### Q: How to view OpenAPI/Swagger documentation?
**A:** Run the application in development mode and visit:
```
http://localhost:5074/openapi/v1.json
```

### Q: 如何變更應用程式的連接埠？
**A:** 編輯 `AspNetCoreApp/Properties/launchSettings.json` 或使用環境變數：
```bash
dotnet run --urls "http://localhost:8080;https://localhost:8081"
```

### Q: How to change the application port?
**A:** Edit `AspNetCoreApp/Properties/launchSettings.json` or use environment variables:
```bash
dotnet run --urls "http://localhost:8080;https://localhost:8081"
```

---

**需要幫助？ | Need Help?**  
如有問題，請在 GitHub Issues 中提出。

If you have questions, please create an issue on GitHub.
