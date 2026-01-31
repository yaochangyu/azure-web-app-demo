# 部署指南：GitHub Actions 自動部署到 Azure App Service

本指南說明如何設定自動部署和手動部署 ASP.NET Core 應用到 Azure App Service。

## 前置需求

1. ✅ Azure 帳戶
2. ✅ GitHub 倉庫
3. ✅ Azure App Service Plan（例：`web-app`）
4. ✅ Azure App Service（例：`azure-web-app-api`）
5. ✅ GitHub Secrets：`AZURE_WEBAPP_PUBLISH_PROFILE`（發佈設定檔）

---

## 部署流程

### 📘 自動部署（推薦）

自動部署通過 GitHub Actions 在每次 push 到 `main` 分支時觸發。無需手動操作，程式碼會自動編譯、測試、發佈和部署。

#### 步驟 1️⃣ 確認 App Service 已創建

```bash
# 檢查是否已有 App Service
az webapp list --resource-group Lab --output table

# 如果沒有，創建 App Service Plan
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

#### 步驟 2️⃣ 取得發佈設定檔

```bash
# 下載發佈設定檔（XML 格式）
az webapp deployment list-publishing-profiles \
  --resource-group Lab \
  --name azure-web-app-api \
  --xml
```

#### 步驟 3️⃣ 設定 GitHub Secret

**方式 A：使用 GitHub 網頁界面**

1. 進入你的 GitHub 倉庫
2. 點擊 **Settings** → **Secrets and variables** → **Actions**
3. 點擊 **New repository secret**
4. 名稱：`AZURE_WEBAPP_PUBLISH_PROFILE`
5. 值：貼上上面的完整 XML 內容
6. 點擊 **Add secret**

**方式 B：使用 GitHub CLI（更快）**

```bash
# 將 XML 保存到檔案
az webapp deployment list-publishing-profiles \
  --resource-group Lab \
  --name azure-web-app-api \
  --xml > /tmp/publish-profile.xml

# 設定為 GitHub Secret
gh secret set AZURE_WEBAPP_PUBLISH_PROFILE \
  --repo yaochangyu/azure-web-app \
  < /tmp/publish-profile.xml

# 驗證 Secret 已設定
gh secret list --repo yaochangyu/azure-web-app
```

#### 步驟 4️⃣ 推送程式碼以觸發部署

```bash
# 進行代碼修改後
git add .
git commit -m "Your commit message"
git push origin main
```

**自動部署將立即開始！** 🚀

#### 監控自動部署

1. 進入 GitHub 倉庫
2. 點擊 **Actions** 標籤
3. 查看最新工作流執行狀態（綠色 ✅ 表示成功）
4. 點擊具體工作流查看詳細日誌

#### 自動部署的優點

✨ 無需手動操作  
⚡ 代碼 push 後自動部署  
🔒 安全可靠  
📊 可追蹤部署歷史  

---

### 📗 手動部署

當需要立即部署或不想通過 GitHub Actions 時，可以手動部署。

#### 步驟 1️⃣ 編譯應用

```bash
# 進入項目目錄
cd /mnt/d/lab/azure-web-app

# 恢復依賴
dotnet restore AspNetCoreApp/AspNetCoreApp.csproj

# 編譯應用（Release 配置）
dotnet publish AspNetCoreApp/AspNetCoreApp.csproj \
  --configuration Release \
  --output ./publish-local \
  --force
```

#### 步驟 2️⃣ 打包發佈檔案

```bash
# ⚠️ 重要：從目錄內開始 zip，不要包含 publish-local 目錄本身
cd ./publish-local
zip -r ../publish-local.zip .
cd ..
```

**為什麼這樣打包？**

正確的 zip 結構應該是：
```
publish-local.zip
├── AspNetCoreApp
├── AspNetCoreApp.dll
├── appsettings.json
├── web.config
└── ...（其他檔案）
```

#### 步驟 3️⃣ 部署到 Azure

```bash
# 使用 Azure CLI 部署（推薦）
az webapp deploy \
  --resource-group Lab \
  --name azure-web-app-api \
  --src-path ./publish-local.zip \
  --type zip
```

部署過程會顯示進度，等待 `Deployment has completed successfully` 訊息。

#### 步驟 4️⃣ 驗證部署

```bash
# 檢查應用狀態（應為 Running）
az webapp show \
  --resource-group Lab \
  --name azure-web-app-api \
  --query "state"

# 測試天氣預報 API
curl -s https://azure-web-app-api.azurewebsites.net/api/weatherforecast | jq .

# 測試版本 API
curl -s https://azure-web-app-api.azurewebsites.net/api/version | jq .
```

#### 手動部署的用途

⚡ 需要立即部署時使用  
🔧 測試部署配置  
📋 在本地驗證應用後部署  

---

## 常見問題

### Q1：自動部署失敗怎麼辦？

**A：檢查以下幾點：**

1. **檢查 Secret 設定**
   ```bash
   gh secret list --repo yaochangyu/azure-web-app
   ```

2. **查看 GitHub Actions 日誌**
   - 進入倉庫 → **Actions** 標籤
   - 點擊失敗的工作流查看錯誤訊息

3. **檢查 App Service 狀態**
   ```bash
   az webapp show --resource-group Lab --name azure-web-app-api --query "state"
   ```

### Q2：手動部署失敗？

**A：檢查 Zip 打包結構和 .NET 版本匹配。**

### Q3：應該使用自動還是手動部署？

**A：日常開發使用自動部署，緊急情況使用手動部署。**

---

## API 端點

### 天氣預報 API
```
GET /api/weatherforecast
```

### 版本 API
```
GET /api/version
```

---

## 應用 URL

https://azure-web-app-api.azurewebsites.net

---

🎉 部署完成！推薦使用自動部署，只需 `git push origin main` 即可。

