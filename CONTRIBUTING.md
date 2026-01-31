# 貢獻指南

感謝您考慮為 **Azure Web App** 專案做出貢獻！我們歡迎所有形式的貢獻。

## 🤝 如何貢獻

### 報告問題 (Bug Reports)

如果您發現問題，請：

1. 檢查 [Issues](https://github.com/yaochangyu/azure-web-app/issues) 確認問題尚未被報告
2. 創建新的 Issue，包含：
   - 清晰的標題和描述
   - 重現步驟
   - 預期行為 vs 實際行為
   - 環境資訊（OS、.NET 版本等）
   - 螢幕截圖（如適用）

### 功能建議 (Feature Requests)

我們歡迎新功能建議！請：

1. 創建 Issue 並標記為 `enhancement`
2. 詳細描述功能和使用場景
3. 說明該功能如何使專案受益

### 提交程式碼 (Pull Requests)

#### 開發流程

1. **Fork 專案**
   ```bash
   # 在 GitHub 上點擊 Fork 按鈕
   git clone https://github.com/YOUR_USERNAME/azure-web-app.git
   cd azure-web-app
   ```

2. **創建分支**
   ```bash
   git checkout -b feature/your-feature-name
   # 或
   git checkout -b fix/your-bug-fix
   ```

3. **進行修改**
   - 遵循現有的程式碼風格
   - 添加必要的測試
   - 更新相關文檔

4. **測試您的修改**
   ```bash
   # 恢復依賴
   dotnet restore AspNetCoreApp/AspNetCoreApp.csproj
   
   # 建置專案
   dotnet build AspNetCoreApp/AspNetCoreApp.csproj
   
   # 運行測試（如有）
   dotnet test
   
   # 本地運行驗證
   dotnet run --project AspNetCoreApp/AspNetCoreApp.csproj
   ```

5. **提交變更**
   ```bash
   git add .
   git commit -m "feat: add new feature description"
   # 或
   git commit -m "fix: fix bug description"
   ```

6. **推送到您的 Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **創建 Pull Request**
   - 前往原始倉庫
   - 點擊 "New Pull Request"
   - 選擇您的分支
   - 填寫 PR 描述

#### Commit 訊息規範

我們建議使用 [Conventional Commits](https://www.conventionalcommits.org/) 格式：

```
<type>(<scope>): <subject>

<body>

<footer>
```

**類型 (Type)**：
- `feat`: 新功能
- `fix`: 錯誤修復
- `docs`: 文檔更新
- `style`: 程式碼格式（不影響功能）
- `refactor`: 重構
- `test`: 測試相關
- `chore`: 構建過程或輔助工具變更

**範例**：
```bash
feat(api): add weather forecast caching

Add Redis caching for weather forecast data to improve performance.
Reduces API response time from 200ms to 50ms.

Closes #123
```

#### Pull Request 檢查清單

在提交 PR 前，請確認：

- [ ] 程式碼遵循專案風格
- [ ] 所有測試通過
- [ ] 添加了必要的測試（如適用）
- [ ] 更新了相關文檔
- [ ] Commit 訊息清晰且有意義
- [ ] 沒有包含敏感資訊（密鑰、憑證等）

## 📋 程式碼風格

### C# 程式碼規範

- 使用 4 個空格縮排（不使用 Tab）
- 使用 PascalCase 命名類別和方法
- 使用 camelCase 命名私有欄位
- 添加 XML 文檔註釋於公開 API
- 遵循 [C# 編碼慣例](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)

**範例**：

```csharp
/// <summary>
/// 取得天氣預報資料
/// </summary>
/// <param name="days">預測天數</param>
/// <returns>天氣預報清單</returns>
public IEnumerable<WeatherForecast> GetForecast(int days)
{
    // 實作內容
}
```

### 文檔規範

- Markdown 檔案使用清晰的標題層次
- 包含程式碼範例和螢幕截圖
- 保持簡潔但完整
- 使用適當的表情符號增強可讀性 😊

## 🧪 測試

雖然目前專案測試覆蓋率有限，但我們鼓勵：

- 為新功能添加單元測試
- 為 Bug 修復添加回歸測試
- 使用有意義的測試名稱

```csharp
[Fact]
public void GetForecast_ReturnsCorrectNumberOfDays()
{
    // Arrange
    var controller = new WeatherForecastController();
    
    // Act
    var result = controller.Get();
    
    // Assert
    Assert.Equal(5, result.Count());
}
```

## 🔒 安全性

### 報告安全漏洞

**請勿公開報告安全漏洞！**

如果您發現安全問題，請：
1. 透過 GitHub Security Advisories 私下報告
2. 或發送電子郵件至維護者
3. 提供詳細的漏洞說明和重現步驟

### 安全編碼實踐

- 永不硬編碼密鑰或密碼
- 使用參數化查詢防止 SQL 注入
- 驗證和清理所有使用者輸入
- 遵循 [OWASP 最佳實踐](https://owasp.org/)

## 📞 需要幫助？

如果您有任何問題：

- 查看 [README.md](README.md) 和 [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)
- 搜尋現有的 [Issues](https://github.com/yaochangyu/azure-web-app/issues)
- 創建新的 Issue 提問
- 參與 [Discussions](https://github.com/yaochangyu/azure-web-app/discussions)（如啟用）

## 📜 授權

提交貢獻即表示您同意您的程式碼將以 [MIT License](LICENSE) 授權。

## 🙏 致謝

感謝所有貢獻者！您的參與讓這個專案變得更好。

---

再次感謝您的貢獻！🎉
