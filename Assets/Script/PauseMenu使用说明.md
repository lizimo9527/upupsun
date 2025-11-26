# PauseMenu 暂停菜单使用说明

## 功能概述

`PauseMenu` 脚本提供了完整的游戏暂停功能，包括：
- ✅ 暂停/恢复游戏
- ✅ 显示/隐藏暂停面板
- ✅ 回到首页
- ✅ 重新开始游戏
- ✅ ESC键支持
- ✅ 音频暂停/恢复
- ✅ 应用失去焦点时自动暂停

## 在Unity编辑器中的设置步骤

### 1. 创建UI结构

在游戏场景（play1）的Canvas下创建以下UI结构：

```
Canvas
├── PauseButton (暂停按钮 - 右上角)
│   ├── Image组件
│   └── Button组件
│
└── PausePanel (暂停弹出框 - 默认隐藏)
    ├── Image组件 (半透明背景)
    ├── HomeButton (回到首页按钮)
    │   ├── Image组件
    │   └── Button组件
    ├── RestartButton (重新开始按钮)
    │   ├── Image组件
    │   └── Button组件
    └── ResumeButton (继续游戏按钮 - 可选)
        ├── Image组件
        └── Button组件
```

### 2. 设置UI元素

#### PauseButton（暂停按钮）
- **位置**: 右上角
- **设置方法**: 
  1. 选中PauseButton
  2. 在RectTransform中，点击锚点预设（左上角），选择"右上角"（Top-Right）
  3. 调整Anchored Position，例如：x: -50, y: -50

#### PausePanel（暂停面板）
- **位置**: 屏幕中央
- **设置方法**:
  1. 选中PausePanel
  2. 在RectTransform中，设置锚点为"拉伸"（Stretch）
  3. 设置Left, Right, Top, Bottom都为0，使其覆盖整个屏幕
  4. 添加Image组件，设置颜色为半透明（例如：RGBA(0, 0, 0, 0.7)）
  5. **重要**: 在Inspector中取消勾选"Active"，使其默认隐藏

#### 按钮布局建议
- 将HomeButton、RestartButton、ResumeButton放在PausePanel中央
- 可以创建一个Vertical Layout Group来排列按钮
- 按钮间距建议：20-30像素

### 3. 添加PauseMenu脚本

1. 在场景中创建一个空GameObject，命名为 `PauseMenuManager`
2. 将 `PauseMenu.cs` 脚本添加到该GameObject上
3. 在Inspector中设置以下引用：
   - **Pause Panel**: 拖拽PausePanel GameObject
   - **Pause Button**: 拖拽PauseButton上的Button组件
   - **Home Button**: 拖拽HomeButton上的Button组件
   - **Restart Button**: 拖拽RestartButton上的Button组件
   - **Resume Button** (可选): 拖拽ResumeButton上的Button组件（如果有的话）
   - **Enable Esc Key**: 勾选（默认已勾选）
   - **Home Scene Name**: 设置为 "Start"（或你的首页场景名称）

### 4. 场景名称设置

如果首页场景名称不是 "Start"，需要：
1. 在PauseMenu组件的Inspector中
2. 找到 "Home Scene Name" 字段
3. 输入正确的场景名称（例如："Start"）

## 功能说明

### 暂停/恢复
- **方法1**: 点击右上角的暂停按钮
- **方法2**: 按ESC键（如果启用了Enable Esc Key）

### 回到首页
- 点击暂停面板上的"回到首页"按钮
- 会自动恢复游戏时间并加载首页场景

### 重新开始
- 点击暂停面板上的"重新开始"按钮
- 会重新加载当前游戏场景

### 继续游戏
- 如果暂停面板上有"继续"按钮，点击它
- 或者再次点击暂停按钮/按ESC键

## 技术细节

### 时间控制
- 暂停时：`Time.timeScale = 0f`
- 恢复时：`Time.timeScale = 1f`
- 场景切换前会自动恢复时间

### 音频控制
- 暂停时：`AudioListener.pause = true`
- 恢复时：`AudioListener.pause = false`

### 与GameManager的兼容性
- GameManager已经添加了暂停检测，暂停时不允许画线
- 协程使用`WaitForSeconds`，会在暂停时自动暂停

## 常见问题

### Q: 暂停后无法恢复？
A: 检查Time.timeScale是否被其他脚本修改，确保PauseMenu是唯一控制时间缩放的脚本。

### Q: ESC键不工作？
A: 检查PauseMenu组件的"Enable Esc Key"是否勾选。

### Q: 场景切换失败？
A: 确保场景名称正确，并且场景已添加到Build Settings中。

### Q: 暂停面板不显示？
A: 检查PausePanel的引用是否正确设置，以及PausePanel是否在Canvas下。

## 注意事项

1. **确保场景在Build Settings中**: File > Build Settings > 添加场景
2. **UI层级**: 确保PausePanel在Canvas的最上层（最后渲染）
3. **按钮交互**: 确保EventSystem存在（Unity通常会自动创建）
4. **时间恢复**: 场景切换前会自动恢复时间，无需手动处理

## 扩展功能

如果需要添加更多功能，可以：
- 在PauseMenu中添加设置按钮
- 添加音效控制
- 添加游戏统计信息显示
- 添加动画效果

