# Neo3-GUI Tauri 迁移可行性评估报告

## 1. 当前技术栈分析

### 1.1 项目架构概览

Neo3-GUI 采用**混合架构**，不是纯 Electron 应用：

```
┌─────────────────────────────────────────────────────┐
│                    Electron Shell                    │
│  ┌─────────────────────────────────────────────────┐│
│  │              React 前端 (ClientApp)              ││
│  │  - React 18 + Ant Design 4.x                    ││
│  │  - MobX 状态管理                                 ││
│  │  - i18next 国际化                               ││
│  └─────────────────────────────────────────────────┘│
│                         │                            │
│                    HTTP/WebSocket                    │
│                         │                            │
│  ┌─────────────────────────────────────────────────┐│
│  │            .NET Core 后端 (neo3-gui)            ││
│  │  - ASP.NET Core Web API                         ││
│  │  - Neo 3.9.1 SDK                                ││
│  │  - LevelDB 存储                                 ││
│  └─────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────┘
```

### 1.2 前端技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| React | 18.2.0 | UI 框架 |
| Ant Design | 4.23.6 | UI 组件库 |
| MobX | 6.6.2 | 状态管理 |
| React Router | 6.4.2 | 路由 |
| i18next | 21.10.0 | 国际化 |
| Axios | 1.1.3 | HTTP 请求 |
| Electron | 21.1.1 | 桌面容器 |

**前端文件统计**：75 个 JS/JSX 文件

### 1.3 后端技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 10.0 | 运行时 |
| ASP.NET Core | - | Web API |
| Neo SDK | 3.9.1 | 区块链核心 |
| LevelDB | - | 数据存储 |
| SQLite | 7.0.1 | 本地数据库 |

**后端文件统计**：213 个 C# 文件

### 1.4 Electron API 使用分析

**使用 Electron API 的文件**（6个）：

| 文件 | 使用的 API | 用途 |
|------|-----------|------|
| `config.js` | `app.getAppPath()`, `fs` | 读写配置文件 |
| `neonode.js` | `app`, `spawn` | 启动 .NET 后端进程 |
| `menuaction.js` | `shell.openExternal()`, `app.getVersion()` | 打开外链、获取版本 |
| `walletaction.js` | `dialog.showOpenDialog()`, `dialog.showSaveDialog()` | 文件选择对话框 |
| `deploy.js` | `dialog.showOpenDialog()`, `fs` | 选择合约文件 |
| `upgrade.js` | `dialog.showOpenDialog()`, `fs` | 选择合约文件 |

**Electron API 使用汇总**：

```javascript
// @electron/remote
- app.getAppPath()      // 获取应用路径
- app.getVersion()      // 获取版本号
- dialog.showOpenDialog() // 打开文件对话框
- dialog.showSaveDialog() // 保存文件对话框

// electron
- shell.openExternal()  // 打开外部链接

// Node.js (通过 Electron)
- fs.readFileSync()     // 读取文件
- fs.writeFile()        // 写入文件
- fs.existsSync()       // 检查文件存在
- child_process.spawn() // 启动子进程
```

---

## 2. Tauri 兼容性评估

### 2.1 前端代码复用性

| 组件 | 兼容性 | 说明 |
|------|--------|------|
| React 组件 | ✅ 完全兼容 | 无需修改 |
| Ant Design | ✅ 完全兼容 | 无需修改 |
| MobX 状态管理 | ✅ 完全兼容 | 无需修改 |
| HTTP 请求 (Axios) | ✅ 完全兼容 | 无需修改 |
| 路由 | ✅ 完全兼容 | 无需修改 |
| 国际化 | ✅ 完全兼容 | 无需修改 |

**结论**：约 95% 的前端代码可直接复用

### 2.2 需要替换的 Electron API

| Electron API | Tauri 替代方案 | 复杂度 |
|--------------|---------------|--------|
| `dialog.showOpenDialog()` | `@tauri-apps/api/dialog` → `open()` | 低 |
| `dialog.showSaveDialog()` | `@tauri-apps/api/dialog` → `save()` | 低 |
| `shell.openExternal()` | `@tauri-apps/api/shell` → `open()` | 低 |
| `app.getAppPath()` | `@tauri-apps/api/path` | 低 |
| `app.getVersion()` | `@tauri-apps/api/app` → `getVersion()` | 低 |
| `fs.readFileSync()` | `@tauri-apps/api/fs` → `readTextFile()` | 低 |
| `fs.writeFile()` | `@tauri-apps/api/fs` → `writeTextFile()` | 低 |
| `child_process.spawn()` | Tauri Command (Rust) | **高** |

### 2.3 关键挑战：.NET 后端进程管理

**当前实现** (`neonode.js`)：
```javascript
// Electron 中通过 Node.js 启动 .NET 后端
const ps = spawn("./neo3-gui.exe", [], {
  cwd: path.join(startPath, "build-neo-node"),
  env: childEnv,
});
```

**Tauri 解决方案**：

需要在 Rust 侧实现进程管理：

```rust
// src-tauri/src/main.rs
use std::process::{Command, Child};
use tauri::State;
use std::sync::Mutex;

struct NeoNode(Mutex<Option<Child>>);

#[tauri::command]
fn start_neo_node(state: State<NeoNode>, network: String) -> Result<(), String> {
    let mut node = state.0.lock().unwrap();
    let child = Command::new("./neo3-gui")
        .current_dir("./build-neo-node")
        .env("NEO_NETWORK", network)
        .spawn()
        .map_err(|e| e.to_string())?;
    *node = Some(child);
    Ok(())
}

#[tauri::command]
fn stop_neo_node(state: State<NeoNode>) -> Result<(), String> {
    let mut node = state.0.lock().unwrap();
    if let Some(mut child) = node.take() {
        child.kill().map_err(|e| e.to_string())?;
    }
    Ok(())
}
```

---

## 3. 迁移工作量估算

### 3.1 需要修改的文件

| 类别 | 文件数 | 修改程度 |
|------|--------|----------|
| Electron API 相关 | 6 | 中等重写 |
| 配置/启动文件 | 3 | 完全重写 |
| Tauri 新增文件 | 5+ | 新建 |
| 其他前端文件 | 0 | 无需修改 |

### 3.2 具体工作项

| 任务 | 预计工时 | 说明 |
|------|----------|------|
| Tauri 项目初始化 | 2h | 创建 Tauri 项目结构 |
| Rust 进程管理模块 | 8h | 启动/停止 .NET 后端 |
| 文件对话框迁移 | 4h | 替换 6 个文件中的 dialog API |
| 文件系统操作迁移 | 4h | 替换 fs 操作 |
| Shell 操作迁移 | 1h | 替换 openExternal |
| 配置文件路径处理 | 4h | 适配 Tauri 路径系统 |
| 打包配置 | 8h | 配置多平台打包 |
| 测试与调试 | 16h | 功能测试、兼容性测试 |
| 文档更新 | 4h | 更新构建和使用文档 |

**总计预估**：约 **51 工时**（约 6-7 个工作日）

### 3.3 风险缓冲

考虑到未知问题和调试时间，建议预留 **30% 缓冲**：

**最终估算**：**8-10 个工作日**

---

## 4. 收益分析

### 4.1 包大小对比

| 指标 | Electron | Tauri | 改善 |
|------|----------|-------|------|
| 基础运行时 | ~100MB | ~3-5MB | **95%↓** |
| 应用总大小（含 .NET） | ~150MB | ~50-60MB | **60%↓** |

> 注：由于需要捆绑 .NET 运行时，Tauri 版本仍需包含 .NET 相关文件

### 4.2 性能对比

| 指标 | Electron | Tauri | 改善 |
|------|----------|-------|------|
| 启动时间 | 3-5s | 1-2s | **50-60%↓** |
| 内存占用（空闲） | 150-200MB | 50-80MB | **60%↓** |
| CPU 占用（空闲） | 1-3% | <1% | **70%↓** |

### 4.3 其他收益

| 收益 | 说明 |
|------|------|
| 安全性 | Tauri 默认禁用 Node.js，减少攻击面 |
| 更新体验 | Tauri 支持增量更新 |
| 原生体验 | 更好的系统集成 |
| 维护成本 | Electron 版本更新频繁，Tauri 更稳定 |

---

## 5. 风险评估

### 5.1 平台兼容性

| 平台 | Electron | Tauri | 风险 |
|------|----------|-------|------|
| Windows | ✅ | ✅ | 低 |
| macOS | ✅ | ✅ | 低 |
| Linux | ✅ | ✅ | 中（需要 WebKit2GTK） |

### 5.2 功能缺失风险

| 功能 | 风险等级 | 说明 |
|------|----------|------|
| 文件对话框 | 低 | Tauri 完全支持 |
| 进程管理 | 中 | 需要 Rust 实现 |
| 系统托盘 | 低 | Tauri 支持 |
| 自动更新 | 低 | Tauri 内置支持 |

### 5.3 主要风险点

1. **Rust 学习曲线**
   - 团队需要掌握基础 Rust 知识
   - 进程管理模块需要 Rust 实现

2. **调试复杂度**
   - Tauri 调试工具不如 Electron 成熟
   - Rust 错误排查需要经验

3. **生态系统**
   - Tauri 插件生态不如 Electron 丰富
   - 某些边缘场景可能缺少现成方案

---

## 6. 建议方案

### 6.1 推荐：渐进式迁移

```
阶段 1（2天）：环境搭建
├── 创建 Tauri 项目
├── 配置构建系统
└── 验证基础功能

阶段 2（3天）：核心迁移
├── 实现 Rust 进程管理
├── 迁移文件对话框
└── 迁移文件系统操作

阶段 3（2天）：集成测试
├── 功能测试
├── 多平台测试
└── 性能测试

阶段 4（2天）：打包发布
├── 配置多平台打包
├── 测试安装包
└── 文档更新
```

### 6.2 迁移可行性结论

| 评估维度 | 结论 |
|----------|------|
| 技术可行性 | ✅ **可行** |
| 工作量 | 中等（8-10 工作日） |
| 收益 | 显著（包大小↓60%，内存↓60%） |
| 风险 | 可控 |

### 6.3 最终建议

**推荐迁移**，理由：

1. ✅ 前端代码 95% 可复用
2. ✅ Electron API 使用量少，替换成本低
3. ✅ 显著的性能和包大小改善
4. ✅ 更好的安全性和维护性
5. ⚠️ 唯一挑战是 Rust 进程管理，但实现难度可控

### 6.4 替代方案

如果不想迁移到 Tauri，可考虑：

1. **Electron 优化**
   - 使用 electron-builder 优化打包
   - 启用 ASAR 压缩
   - 预计可减少 20-30% 包大小

2. **Neutralino.js**
   - 更轻量的 Electron 替代
   - 但生态不如 Tauri 成熟

---

## 附录：迁移代码示例

### A1. 文件对话框迁移

**Before (Electron)**:
```javascript
import { dialog } from '@electron/remote';

const opendialog = () => {
  dialog.showOpenDialog({
    title: "选择文件",
    filters: [{ name: 'JSON', extensions: ['json'] }]
  }).then(res => {
    console.log(res.filePaths[0]);
  });
};
```

**After (Tauri)**:
```javascript
import { open } from '@tauri-apps/api/dialog';

const opendialog = async () => {
  const selected = await open({
    title: "选择文件",
    filters: [{ name: 'JSON', extensions: ['json'] }]
  });
  console.log(selected);
};
```

### A2. 配置文件读写迁移

**Before (Electron)**:
```javascript
import fs from 'fs';
import { app } from '@electron/remote';

const configPath = path.join(app.getAppPath(), 'config.json');
const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));
```

**After (Tauri)**:
```javascript
import { readTextFile, writeTextFile } from '@tauri-apps/api/fs';
import { resolveResource } from '@tauri-apps/api/path';

const configPath = await resolveResource('config.json');
const config = JSON.parse(await readTextFile(configPath));
```

---

*报告生成时间：2025-01-30*
*评估版本：Neo3-GUI v1.6.0*
