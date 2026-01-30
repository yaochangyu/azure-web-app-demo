# GitHub Actions 部署到 Azure App Service

本指南說明如何使用 GitHub Actions 自動部署 ASP.NET Core 應用到 Azure App Service。

## 前置需求

1. ✅ Azure 帳戶
2. ✅ GitHub 倉庫
3. ✅ Azure App Service (已有 `ASP-Lab-b4d1` 計劃)
4. ✅ 已建立的 Web App

## 部署流程

### 1️⃣ 建立 Web App（如果還未建立）

```bash
# 建立 Web App
az webapp create \
  --resource-group Lab \
  --plan ASP-Lab-b4d1 \
  --name my-aspnet-app \
  --runtime "DOTNET|8.0"
```

替換 `my-aspnet-app` 為你想要的應用名稱。

### 2️⃣ 取得發佈設定檔

```bash
# 下載發佈設定檔
az webapp deployment list-publishing-profiles \
  --resource-group Lab \
  --name my-aspnet-app \
  --query "[0].xml" \
  --output tsv > PublishSettings.xml
```

### 3️⃣ 設定 GitHub Secrets

在你的 GitHub 倉庫中，進入 **Settings → Secrets and variables → Actions**，新增以下 Secrets：

#### 選項 A：使用發佈設定檔（推薦）

1. **AZURE_WEBAPP_PUBLISH_PROFILE**：
   - 複製 `PublishSettings.xml` 的內容
   - 在 GitHub 新增為 Secret

#### 選項 B：使用 Azure Credentials（更安全）

1. 執行命令建立服務主體：
```bash
az ad sp create-for-rbac \
  --name "github-actions-sp" \
  --role contributor \
  --scopes /subscriptions/{subscriptionId}/resourceGroups/Lab \
  --json-auth
```

2. 複製輸出內容
3. 在 GitHub 新增為 **AZURE_CREDENTIALS** Secret

### 4️⃣ 更新工作流檔案

編輯 `.github/workflows/deploy-to-azure.yml`：

```yaml
env:
  AZURE_WEBAPP_NAME: 'my-aspnet-app'  # 替換為你的 App Service 名稱
```

### 5️⃣ 推送程式碼

```bash
git add .
git commit -m "Add GitHub Actions deployment workflow"
git push origin main
```

工作流會自動觸發並部署你的應用！

## 可選：手動觸發部署

在 GitHub Actions 標籤下，點擊 **Run workflow** 手動執行部署。

## 監控部署

1. 進入倉庫的 **Actions** 標籤
2. 查看最新的工作流執行狀態
3. 查看詳細的構建和部署日誌

## 部署後驗證

```bash
# 檢查應用狀態
az webapp show \
  --resource-group Lab \
  --name my-aspnet-app \
  --query "state"

# 取得應用 URL
az webapp show \
  --resource-group Lab \
  --name my-aspnet-app \
  --query "defaultHostName" \
  --output tsv
```

## 常見問題

### 部署失敗？

1. 檢查 App Service 配置
2. 檢查依賴版本（.NET 版本等）
3. 查看 GitHub Actions 日誌以了解具體錯誤

### 如何更新 .NET 版本？

編輯 `deploy-to-azure.yml`：
```yaml
DOTNET_VERSION: '7.0.x'  # 或其他版本
```

### 自動部署不工作？

確保 `.github/workflows/deploy-to-azure.yml` 已提交到 GitHub。

---

🎉 部署完成！你的 ASP.NET Core 應用現在會在每次 push 到 main 分支時自動部署。
