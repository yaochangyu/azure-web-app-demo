# Azure Web App - ASP.NET Core 自動部署範例

[![Deploy to Azure](https://github.com/yaochangyu/azure-web-app/actions/workflows/deploy.yml/badge.svg)](https://github.com/yaochangyu/azure-web-app/actions/workflows/deploy.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)

這是一個展示如何使用 **GitHub Actions** 自動部署 **ASP.NET Core** 應用到 **Azure App Service** 的完整範例專案。

## ✨ 功能特點

- 🚀 **自動化部署**：推送程式碼到 `main` 分支自動觸發部署
- ☁️ **Azure 整合**：使用 Service Principal 安全認證
- 🔄 **CI/CD Pipeline**：完整的建置、測試、部署流程
- 📦 **.NET 10.0**：使用最新的 .NET 框架
- 🌐 **RESTful API**：包含天氣預報和版本資訊 API
- 📝 **詳細文檔**：完整的部署指南和最佳實踐

## 🏗️ 專案結構

```
azure-web-app/
├── AspNetCoreApp/              # ASP.NET Core 專案
│   ├── Controllers/            # API 控制器
│   │   ├── WeatherForecastController.cs
│   │   └── VersionController.cs
│   ├── Models/                 # 資料模型
│   ├── Program.cs              # 應用程式進入點
│   └── appsettings.json        # 配置檔案
├── .github/
│   └── workflows/
│       └── deploy.yml          # GitHub Actions 工作流程
├── DEPLOYMENT_GUIDE.md         # 📘 詳細部署指南
├── LICENSE                     # MIT 授權
└── README.md                   # 本文件
```

## 🚀 快速開始

### 前置需求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Azure 訂閱帳戶](https://azure.microsoft.com/free/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- [Git](https://git-scm.com/)

### 本地運行

```bash
# 克隆專案
git clone https://github.com/yaochangyu/azure-web-app.git
cd azure-web-app

# 恢復依賴
dotnet restore AspNetCoreApp/AspNetCoreApp.csproj

# 運行應用
dotnet run --project AspNetCoreApp/AspNetCoreApp.csproj

# 應用將在 http://localhost:5000 運行
```

### 測試 API

```bash
# 天氣預報 API
curl http://localhost:5000/api/weatherforecast

# 版本資訊 API
curl http://localhost:5000/api/version
```

## ☁️ 部署到 Azure

### 自動部署（推薦）

1. **Fork 此專案**到您的 GitHub 帳戶

2. **創建 Azure App Service**
   ```bash
   # 登入 Azure
   az login
   
   # 創建資源群組（如果還沒有）
   az group create --name Lab --location eastasia
   
   # 創建 App Service Plan
   az appservice plan create \
     --name web-app \
     --resource-group Lab \
     --sku B1 \
     --is-linux
   
   # 創建 App Service
   az webapp create \
     --resource-group Lab \
     --plan web-app \
     --name azure-web-app-api \
     --runtime "DOTNETCORE|10.0"
   ```

3. **設定 GitHub Secret**
   
   創建 Azure Service Principal：
   ```bash
   az ad sp create-for-rbac \
     --name "github-actions-azure-web-app" \
     --role contributor \
     --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/Lab \
     --sdk-auth
   ```
   
   將輸出的 JSON 複製到 GitHub Repository Settings → Secrets → New secret：
   - Name: `AZURE_CREDENTIALS`
   - Value: [貼上 JSON 內容]

4. **推送程式碼觸發部署**
   ```bash
   git push origin main
   ```

5. **查看部署狀態**
   - 前往 GitHub Repository → Actions 標籤
   - 查看工作流程執行狀態

完整部署指南請參考 [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)

## 📡 API 端點

### 線上版本（Azure）
- **Base URL**: https://azure-web-app-api.azurewebsites.net
- **天氣預報**: `GET /api/weatherforecast`
- **版本資訊**: `GET /api/version`

### 本地版本
- **Base URL**: http://localhost:5000
- **天氣預報**: `GET /api/weatherforecast`
- **版本資訊**: `GET /api/version`

## 🛠️ 技術棧

- **框架**: ASP.NET Core 10.0
- **語言**: C# 13
- **雲平台**: Microsoft Azure App Service
- **CI/CD**: GitHub Actions
- **部署**: Azure CLI

## 📖 文檔

- [部署指南](DEPLOYMENT_GUIDE.md) - 完整的部署步驟和故障排除
- [貢獻指南](CONTRIBUTING.md) - 如何為專案做出貢獻

## 🤝 貢獻

歡迎貢獻！請查看 [CONTRIBUTING.md](CONTRIBUTING.md) 了解如何開始。

### 貢獻者

感謝所有為此專案做出貢獻的開發者！

## 📄 授權

本專案採用 MIT 授權 - 詳見 [LICENSE](LICENSE) 文件

## 🔒 安全性提醒

⚠️ **重要**：這是一個公開的範例專案，請注意：

- ❌ **絕不提交**敏感資訊到 Git（密鑰、密碼、連接字串）
- ✅ **使用 GitHub Secrets** 存儲所有敏感配置
- ✅ **定期輪換** Service Principal 憑證
- ✅ **啟用 Azure 安全功能**（如防火牆、SSL）
- ✅ **審查權限**，遵循最小權限原則

## 📞 支援

如有問題或建議，請：
- 提交 [Issue](https://github.com/yaochangyu/azure-web-app/issues)
- 發起 [Pull Request](https://github.com/yaochangyu/azure-web-app/pulls)
- 查看 [部署指南](DEPLOYMENT_GUIDE.md)

## 🌟 致謝

- [Microsoft Azure](https://azure.microsoft.com/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [GitHub Actions](https://github.com/features/actions)

---

**⭐ 如果這個專案對您有幫助，請給個星星！**
