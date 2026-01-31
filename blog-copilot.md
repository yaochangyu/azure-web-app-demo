# Azure App Service 搭配 GitHub Actions 完整部署指南

Azure App Service 是微軟提供的全託管式 Web 應用程式平台，讓開發者能專注於程式開發，不需要擔心基礎架構的管理。搭配 GitHub Actions，可以實現完全自動化的持續部署流程。

這篇文章會介紹如何使用 Azure App Service 部署和管理 Web 應用程式，以及透過 GitHub Actions 實現 CI/CD 自動部署。

---

## 開發環境

■ Windows 11  
■ .NET 10.0  
■ Azure CLI  
■ Azure App Service  

---

## 什麼是 Azure App Service？

Azure App Service 是一個完全託管的平台即服務 (PaaS)，支援多種程式語言和框架：

- **.NET / .NET Core**：原生支援，效能最佳化
- **Java**：支援 Tomcat、JBoss 等容器
- **Node.js**：完整的 npm 套件支援
- **Python**：Django、Flask 等框架
- **PHP**：Laravel、WordPress 等應用

主要特點：

1. **自動縮放**：根據流量自動調整資源
2. **高可用性**：內建負載平衡和容錯移轉
3. **持續部署**：整合 GitHub Actions、Azure DevOps
4. **內建監控**：Application Insights 即時監控
5. **安全性**：SSL/TLS、認證授權、防火牆

---

## 建立 App Service 的三個步驟

### 步驟 1️⃣ 建立 App Service Plan

App Service Plan 定義了運算資源的規格，類似虛擬主機的方案。

```bash
# 建立資源群組
az group create \
  --name Lab \
  --location eastasia

# 建立 App Service Plan
az appservice plan create \
  --name web-app \
  --resource-group Lab \
  --sku B1 \
  --is-linux
```

**SKU 等級說明**：

| SKU | 用途 | 特色 |
|-----|------|------|
| F1 (Free) | 開發測試 | 免費，共享資源 |
| B1 (Basic) | 小型應用 | 基本功能，固定費用 |
| S1 (Standard) | 生產環境 | 自動縮放、備份 |
| P1V3 (Premium) | 高效能應用 | 更多記憶體、效能 |

### 步驟 2️⃣ 建立 Web App

```bash
# 建立 ASP.NET Core 應用
az webapp create \
  --resource-group Lab \
  --plan web-app \
  --name azure-web-app-api \
  --runtime "DOTNETCORE:10.0"
```

**常用 Runtime**：
- `"DOTNETCORE:10.0"` - .NET 10
- `"NODE:20-lts"` - Node.js 20 LTS  
- `"PYTHON:3.12"` - Python 3.12
- `"JAVA:17-java17"` - Java 17

### 步驟 3️⃣ 部署應用程式

有三種部署方式：

#### 方式 A：本機手動部署

```bash
# 發佈應用程式到指定目錄
dotnet publish AspNetCoreApp/AspNetCoreApp.csproj \
  --configuration Release \
  --output ./publish-local \
  --force

# 進入發佈目錄並建立 ZIP（重要：ZIP 結構要正確）
cd ./publish-local
zip -r ../publish-local.zip .
cd ..

# 部署到 Azure
az webapp deploy \
  --resource-group Lab \
  --name azure-web-app-api \
  --src-path ./publish-local.zip \
  --type zip
```

**為什麼 ZIP 結構重要？**

ZIP 內應該直接包含 `*.dll`、`appsettings.json` 等檔案，而不是嵌套在 `publish-local/` 目錄內。

#### 方式 B：GitHub Actions 自動部署（推薦）

這是最推薦的方式，能實現持續部署 (CI/CD)。詳見下一章節 [GitHub Actions 持續部署](#github-actions-持續部署)。

#### 方式 C：Azure CLI 快速部署

```bash
# 從本機資料夾直接部署
az webapp up \
  --name azure-web-app-api \
  --resource-group Lab \
  --runtime "DOTNETCORE:10.0"
```

---

## GitHub Actions 持續部署

GitHub Actions 讓你能自動化從代碼到部署的整個過程。每次 push 到 `main` 分支，都會自動編譯、測試、打包並部署到 Azure App Service。

### 架構流程

```
┌─────────────────┐
│  git push main  │
└────────┬────────┘
         │
         v
    ┌─────────────────────────────┐
    │  GitHub Actions Triggered   │
    └────────┬────────────────────┘
             │
    ┌────────┴─────────────────────────────┐
    │                                      │
    v                                      v
┌──────────────────────┐          ┌───────────────┐
│   Build Stage        │          │  Publish      │
│ - Checkout code      │          │  - ZIP files  │
│ - Setup .NET 10.0    │──────────┤  - Ready      │
│ - dotnet restore     │          │    for        │
│ - dotnet build       │          │    deploy     │
│ - dotnet publish     │          └───────┬───────┘
└──────────────────────┘                  │
                                          v
                          ┌──────────────────────────┐
                          │  Deploy to Azure         │
                          │  - Azure CLI login       │
                          │  - Upload ZIP            │
                          │  - App Service restart   │
                          └──────────────────────────┘
```

### 設定步驟

#### 步驟 1️⃣ 建立 Service Principal

Service Principal 是一個特殊的 Azure 帳戶，用於自動化任務的認證。

```bash
# 建立 Service Principal 並取得憑證
az ad sp create-for-rbac \
  --name "github-actions-azure-web-app" \
  --role contributor \
  --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/Lab \
  --sdk-auth
```

**輸出範例**：
```json
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

**⚠️ 安全提醒**：這個 JSON 檔案包含敏感憑證，絕對不要提交到版本控制系統！

#### 步驟 2️⃣ 設定 GitHub Secret

GitHub Secrets 存放機敏資訊（如憑證），GitHub Actions 可以在執行時安全地存取。

**方式 A：使用 GitHub Web UI**

1. 進入 GitHub 倉庫
2. 點擊 **Settings** → **Secrets and variables** → **Actions**
3. 點擊 **New repository secret**
4. 名稱：`AZURE_CREDENTIALS`
5. 值：貼上上一步輸出的完整 JSON
6. 點擊 **Add secret**

**方式 B：使用 GitHub CLI**

```bash
# 將憑證保存到臨時檔案
az ad sp create-for-rbac \
  --name "github-actions-azure-web-app" \
  --role contributor \
  --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/Lab \
  --sdk-auth > /tmp/azure-creds.json

# 設定為 GitHub Secret
gh secret set AZURE_CREDENTIALS \
  --repo yaochangyu/azure-web-app \
  < /tmp/azure-creds.json

# 安全地刪除本地憑證檔案
rm /tmp/azure-creds.json
```

#### 步驟 3️⃣ 建立 Workflow 檔案

在專案根目錄建立 `.github/workflows/deploy.yml`：

```yaml
name: Deploy to Azure App Service

on:
  push:
    branches:
      - main
  workflow_dispatch:

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
      # 步驟 1：檢出程式碼
      - uses: actions/checkout@v4

      # 步驟 2：設置 .NET 環境
      - name: Setup .NET 10.0
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      # 步驟 3：恢復依賴
      - name: Restore dependencies
        run: dotnet restore AspNetCoreApp/AspNetCoreApp.csproj

      # 步驟 4：編譯專案
      - name: Build
        run: dotnet build AspNetCoreApp/AspNetCoreApp.csproj --configuration Release --no-restore

      # 步驟 5：發佈專案
      - name: Publish
        run: dotnet publish AspNetCoreApp/AspNetCoreApp.csproj --configuration Release --output ${{ github.workspace }}/publish --no-build

      # 步驟 6：建立 ZIP 部署包
      - name: Create deployment package
        run: |
          cd ${{ github.workspace }}/publish
          zip -r ../app-deployment.zip .
          cd ..

      # 步驟 7：使用 Azure CLI 登入
      - name: Azure CLI login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      # 步驟 8：部署到 Azure App Service
      - name: Deploy to Azure App Service
        run: |
          az webapp deploy \
            --resource-group Lab \
            --name azure-web-app-api \
            --src-path app-deployment.zip \
            --type zip

      # 步驟 9：記錄部署狀態
      - name: Check deployment status
        run: |
          az webapp show \
            --resource-group Lab \
            --name azure-web-app-api \
            --query "state"
```

#### 步驟 4️⃣ 觸發自動部署

推送程式碼到 `main` 分支時，GitHub Actions 會自動執行：

```bash
# 進行代碼修改
echo "# Updated" >> README.md

# 提交並推送
git add .
git commit -m "chore: update README"
git push origin main
```

### 監控部署

#### 實時查看部署狀態

1. 進入 GitHub 倉庫
2. 點擊 **Actions** 標籤
3. 查看最新的工作流運行
4. 點擊具體工作流查看詳細步驟

**運行中** 🟡 → **成功** 🟢 → 應用已部署到 Azure

#### 查看部署日誌

```bash
# 使用 Azure CLI 查看最近的部署
az webapp deployment list \
  --resource-group Lab \
  --name azure-web-app-api \
  --query "[0].[id, deploymentStatus, endTime]"

# 查看應用程式的即時日誌
az webapp log tail \
  --resource-group Lab \
  --name azure-web-app-api
```

### 常見問題排除

#### 部署失敗：`AZURE_CREDENTIALS not found`

**原因**：GitHub Secret 未設定正確

**解決方案**：
```bash
# 驗證 Secret 已設定
gh secret list --repo yaochangyu/azure-web-app

# 重新設定 Secret
gh secret set AZURE_CREDENTIALS < /tmp/azure-creds.json
```

#### 部署失敗：`Zip structure is incorrect`

**原因**：ZIP 檔案內層級不正確

**解決方案**：確保 ZIP 內直接包含 `*.dll`、`appsettings.json` 等，而不是嵌套在資料夾內：

```bash
# ❌ 錯誤的結構
publish/
  publish/
    AspNetCoreApp.dll
    appsettings.json

# ✅ 正確的結構
publish/
  AspNetCoreApp.dll
  appsettings.json
```

#### 部署失敗：`Service Principal has insufficient permissions`

**原因**：Service Principal 沒有足夠的權限

**解決方案**：確認 Service Principal 已被授予 `contributor` 角色：

```bash
# 檢查角色指派
az role assignment list \
  --assignee <clientId> \
  --resource-group Lab
```

### GitHub Actions 最佳實踐

#### 1. 使用環境變數管理配置

在 Workflow 檔案中定義環境變數：

```yaml
env:
  AZURE_RESOURCE_GROUP: Lab
  AZURE_APP_NAME: azure-web-app-api
  PUBLISH_DIR: publish

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy
        run: |
          az webapp deploy \
            --resource-group ${{ env.AZURE_RESOURCE_GROUP }} \
            --name ${{ env.AZURE_APP_NAME }} \
            --src-path ${{ env.PUBLISH_DIR }}-app.zip
```

#### 2. 新增健康檢查步驟

部署後驗證應用是否正常運行：

```yaml
- name: Health check
  run: |
    for i in {1..5}; do
      if curl -f https://azure-web-app-api.azurewebsites.net/health; then
        echo "✅ Health check passed"
        exit 0
      fi
      echo "⏳ Attempt $i/5, waiting..."
      sleep 10
    done
    exit 1
```

#### 3. 通知部署結果

部署完成後發送通知：

```yaml
- name: Notify deployment status
  if: always()
  run: |
    if [ "${{ job.status }}" == "success" ]; then
      echo "✅ Deployment succeeded!"
    else
      echo "❌ Deployment failed!"
    fi
```

#### 4. 使用 Deployment Slots 進行測試部署

在生產環境前先部署到 Staging Slot：

```yaml
- name: Deploy to staging slot
  run: |
    az webapp deployment slot swap \
      --resource-group Lab \
      --name azure-web-app-api \
      --slot staging

- name: Validation tests
  run: |
    # 執行測試...
    dotnet test AspNetCoreApp.Tests/

- name: Swap to production
  if: success()
  run: |
    az webapp deployment slot swap \
      --resource-group Lab \
      --name azure-web-app-api \
      --slot staging \
      --target-slot production
```

### GitHub Actions vs 手動部署

| 特性 | GitHub Actions | 手動部署 |
|------|---|---|
| 部署觸發 | 自動 (push / schedule) | 手動執行 |
| 時間成本 | 低 (無需手動操作) | 高 (需手動執行) |
| 人為錯誤 | 低 (流程一致) | 高 (易出錯) |
| 可追蹤性 | 高 (完整日誌) | 中 (需記錄) |
| 適用場景 | 日常開發/生產環境 | 緊急修復/特殊情況 |

---



應用程式設定 (Application Settings) 會覆寫 `appsettings.json` 的值。

```bash
# 新增應用程式設定
az webapp config appsettings set \
  --resource-group Lab \
  --name azure-web-app-api \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Server=..." \
    ApiKey="your-secret-key"
```

**注意事項**：
- 使用 `__` (雙底線) 表示階層結構
- 機敏資料建議使用 Azure Key Vault
- 開發環境不要使用生產環境的設定

---

## 檢視應用程式記錄

### 啟用應用程式記錄

```bash
# 啟用檔案系統記錄
az webapp log config \
  --resource-group Lab \
  --name azure-web-app-api \
  --application-logging filesystem \
  --level information

# 即時串流記錄
az webapp log tail \
  --resource-group Lab \
  --name azure-web-app-api
```

### 整合 Application Insights

Application Insights 可以監控應用程式效能、追蹤例外狀況。

```bash
# 啟用 Application Insights
az monitor app-insights component create \
  --app azure-web-app-insights \
  --location eastasia \
  --resource-group Lab

# 取得 Instrumentation Key
az monitor app-insights component show \
  --app azure-web-app-insights \
  --resource-group Lab \
  --query instrumentationKey
```

在 `appsettings.json` 加入：

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key"
  }
}
```

---

## 調整效能與擴展

### 垂直擴展 (Scale Up)

升級 App Service Plan 的等級：

```bash
# 升級到 Standard S1
az appservice plan update \
  --name web-app \
  --resource-group Lab \
  --sku S1
```

### 水平擴展 (Scale Out)

增加執行個體數量：

```bash
# 手動擴展到 3 個執行個體
az appservice plan update \
  --name web-app \
  --resource-group Lab \
  --number-of-workers 3
```

### 自動擴展規則

```bash
# 建立自動擴展設定（需要 Standard 以上）
az monitor autoscale create \
  --resource-group Lab \
  --resource web-app \
  --resource-type Microsoft.Web/serverfarms \
  --name auto-scale-plan \
  --min-count 1 \
  --max-count 5 \
  --count 2

# 新增 CPU 使用率規則
az monitor autoscale rule create \
  --resource-group Lab \
  --autoscale-name auto-scale-plan \
  --condition "Percentage CPU > 70 avg 5m" \
  --scale out 1
```

---

## 設定自訂網域與 SSL

### 新增自訂網域

```bash
# 新增自訂網域
az webapp config hostname add \
  --resource-group Lab \
  --webapp-name azure-web-app-api \
  --hostname www.example.com
```

在 DNS 設定中新增 CNAME 記錄：
```
www.example.com → azure-web-app-api.azurewebsites.net
```

### 啟用 SSL 憑證

```bash
# 建立免費的 App Service Managed Certificate
az webapp config ssl bind \
  --resource-group Lab \
  --name azure-web-app-api \
  --certificate-thumbprint auto \
  --ssl-type SNI

# 強制使用 HTTPS
az webapp update \
  --resource-group Lab \
  --name azure-web-app-api \
  --set httpsOnly=true
```

---

## 設定部署位置 (Deployment Slots)

部署位置讓你可以在不影響生產環境的情況下測試新版本。

```bash
# 建立 staging 位置
az webapp deployment slot create \
  --resource-group Lab \
  --name azure-web-app-api \
  --slot staging

# 部署到 staging
az webapp deploy \
  --resource-group Lab \
  --name azure-web-app-api \
  --slot staging \
  --src-path app.zip \
  --type zip

# 驗證無誤後，交換到生產環境
az webapp deployment slot swap \
  --resource-group Lab \
  --name azure-web-app-api \
  --slot staging \
  --target-slot production
```

**優點**：
- 零停機時間部署
- 快速回滾機制
- A/B 測試流量分流

---

## 監控與診斷

### 健康檢查

在 `Program.cs` 新增健康檢查端點：

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
```

在 Azure 設定健康檢查：

```bash
az webapp config set \
  --resource-group Lab \
  --name azure-web-app-api \
  --health-check-path "/health"
```

### 效能監控

使用 Application Insights 儀表板：
1. 開啟 Azure Portal
2. 前往 Application Insights
3. 檢視效能、失敗請求、相依性

或使用 Azure Monitor Metrics：

```bash
# 檢視 CPU 使用率
az monitor metrics list \
  --resource /subscriptions/{subscription-id}/resourceGroups/Lab/providers/Microsoft.Web/sites/azure-web-app-api \
  --metric "CpuPercentage" \
  --start-time 2026-01-31T00:00:00Z \
  --end-time 2026-01-31T23:59:59Z
```

---

## 結合 Polly 增強可靠性

在微服務架構中，Azure App Service 應用程式經常需要呼叫外部 API。這時可以使用 Polly 實作重試、斷路器等策略。

### 安裝套件

```bash
dotnet add package Microsoft.Extensions.Http.Polly
```

### 設定 HttpClient 與 Polly 策略

```csharp
builder.Services.AddHttpClient("ExternalApi", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}
```

這樣當外部 API 暫時無法回應時，Polly 會自動重試；若持續失敗則觸發斷路器，避免系統資源耗盡。

---

## 成本優化建議

### 選擇適當的 SKU

- **開發/測試**：使用 F1 (Free) 或 B1 (Basic)
- **小型生產應用**：B1、B2
- **中型應用**：S1、S2
- **大型/高流量**：P1V3、P2V3

### 使用自動擴展

只在需要時擴展執行個體，避免固定使用高規格。

### 定期清理資源

```bash
# 列出所有 App Service
az webapp list --resource-group Lab --output table

# 刪除不需要的資源
az webapp delete \
  --resource-group Lab \
  --name unused-app
```

### 使用 Reserved Instances

長期使用可購買保留執行個體，最多可省 60% 成本。

---

## 常見問題排除

### 應用程式無法啟動

檢查記錄：
```bash
az webapp log tail \
  --resource-group Lab \
  --name azure-web-app-api
```

常見原因：
- Runtime 版本不符
- 缺少相依套件
- 環境變數設定錯誤

### 效能問題

1. 檢查 Application Insights 慢速請求
2. 確認資料庫連線是否最佳化
3. 考慮啟用 CDN 或 Redis Cache
4. 升級 App Service Plan

### 部署失敗

```bash
# 檢視部署記錄
az webapp deployment list-publishing-profiles \
  --resource-group Lab \
  --name azure-web-app-api
```

確認：
- Service Principal 權限正確
- GitHub Secret 設定無誤
- .csproj 路徑正確

---

## 安全性最佳實踐

### 1. 使用 Managed Identity

避免在程式碼中硬編碼憑證：

```bash
# 啟用系統指派的受控識別
az webapp identity assign \
  --resource-group Lab \
  --name azure-web-app-api
```

### 2. 設定 IP 限制

```bash
# 只允許特定 IP 存取
az webapp config access-restriction add \
  --resource-group Lab \
  --name azure-web-app-api \
  --rule-name AllowOfficeIP \
  --action Allow \
  --ip-address 203.0.113.0/24 \
  --priority 100
```

### 3. 啟用診斷記錄

```bash
az webapp log config \
  --resource-group Lab \
  --name azure-web-app-api \
  --web-server-logging filesystem \
  --detailed-error-messages true \
  --failed-request-tracing true
```

### 4. 定期更新 Runtime

保持 .NET、Node.js 等 runtime 在最新的安全版本。

---

## 心得

Azure App Service 搭配 GitHub Actions，提供了完整的 PaaS 解決方案和自動化部署流程：

**Azure App Service 優點**：

■ **快速部署**：從程式碼到上線只需幾分鐘  
■ **自動管理**：不需維護作業系統、修補程式  
■ **彈性擴展**：根據流量自動調整資源  
■ **整合豐富**：與 Azure 其他服務無縫整合  

**GitHub Actions 優點**：

■ **零配置成本**：GitHub 免費提供 Actions（公開倉庫無限使用）  
■ **自動化部署**：每次 push 自動編譯、測試、部署  
■ **完全可追蹤**：每次部署的日誌和代碼版本都可查詢  
■ **靈活定製**：YAML 語法簡單易懂，方便擴展  

**結合使用的優勢**：

✅ 開發者只需專注於代碼品質，無需操心部署細節  
✅ 部署流程完全自動化，減少人為錯誤  
✅ Service Principal 與 GitHub Secret 提供安全的認證方式  
✅ 支援多種部署策略（直接部署、Deployment Slots 測試等）  

對於現代應用開發團隊，Azure App Service + GitHub Actions 是最佳組合。

---

## 範例位置

本文完整範例程式碼與配置：  
[https://github.com/yaochangyu/azure-web-app](https://github.com/yaochangyu/azure-web-app)

相關指南：  
- [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) - 部署完整說明
- [.github/workflows/deploy.yml](.github/workflows/deploy.yml) - GitHub Actions Workflow

---

## 參考資料

- [Azure App Service 官方文件](https://learn.microsoft.com/azure/app-service/)
- [Azure CLI 參考](https://learn.microsoft.com/cli/azure/)
- [GitHub Actions 官方文件](https://docs.github.com/actions)
- [Application Insights 概觀](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Polly GitHub](https://github.com/App-vNext/Polly)

若有謬誤，煩請告知，新手發帖請多包涵

Microsoft MVP Award 2010~2017 C# 第四季  
Microsoft MVP Award 2018~2022 .NET
