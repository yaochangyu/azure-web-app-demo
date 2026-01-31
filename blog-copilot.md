# Azure App Service 快速上手指南

Azure App Service 是微軟提供的全託管式 Web 應用程式平台，讓開發者能專注於程式開發，不需要擔心基礎架構的管理。這篇文章會介紹如何使用 Azure App Service 部署和管理你的 Web 應用程式。

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

#### 方式 A：本機部署

```bash
# 發佈應用程式
dotnet publish -c Release -o ./publish

# 建立部署 ZIP
cd publish
zip -r ../app.zip .

# 部署到 Azure
az webapp deploy \
  --resource-group Lab \
  --name azure-web-app-api \
  --src-path ../app.zip \
  --type zip
```

#### 方式 B：GitHub Actions 自動部署

參考專案中的 [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)，設定 GitHub Actions workflow。

主要步驟：
1. 建立 Service Principal
2. 設定 GitHub Secret：`AZURE_CREDENTIALS`
3. 推送程式碼到 `main` 分支，自動觸發部署

#### 方式 C：Azure CLI 直接部署

```bash
# 從本機資料夾直接部署
az webapp up \
  --name azure-web-app-api \
  --resource-group Lab \
  --runtime "DOTNETCORE:10.0"
```

---

## 設定環境變數

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

Azure App Service 提供了完整的 PaaS 解決方案，讓開發者專注於商業邏輯而非基礎架構管理。主要優點：

■ **快速部署**：從程式碼到上線只需幾分鐘  
■ **自動管理**：不需維護作業系統、修補程式  
■ **彈性擴展**：根據流量自動調整資源  
■ **整合豐富**：與 Azure 其他服務無縫整合  

搭配 GitHub Actions 實現 CI/CD、結合 Application Insights 監控、使用 Polly 增強韌性，可以建構出穩定可靠的生產環境。

對於中小型應用程式，Azure App Service 絕對是首選方案。如果是超大規模或需要極致客製化的場景，可考慮 Azure Kubernetes Service (AKS)。

---

## 範例位置

本文範例程式碼：  
[https://github.com/yaochangyu/azure-web-app](https://github.com/yaochangyu/azure-web-app)

相關部署指南：  
[DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)

---

## 參考資料

- [Azure App Service 官方文件](https://learn.microsoft.com/azure/app-service/)
- [Azure CLI 參考](https://learn.microsoft.com/cli/azure/)
- [Application Insights 概觀](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Polly GitHub](https://github.com/App-vNext/Polly)

若有謬誤，煩請告知，新手發帖請多包涵

Microsoft MVP Award 2010~2017 C# 第四季  
Microsoft MVP Award 2018~2022 .NET
