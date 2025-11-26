# 画线引光小游戏

## 项目简介
这是一款基于Unity开发的画线引光益智小游戏，玩家通过绘制路径来引导光线到达目标点。

## 游戏玩法
1. **画线**: 按住鼠标左键在屏幕上绘制路径
2. **引光**: 按空格键让光源沿着绘制的路径移动
3. **目标**: 引导光线到达所有目标点完成关卡
4. **重置**: 按R键重置光源位置，ESC键暂停游戏

## 项目结构

### 核心脚本
- `GameManager.cs` - 游戏管理器，负责关卡和游戏状态
- `LineDrawer.cs` - 画线系统，处理玩家输入和路径绘制
- `LightFollower.cs` - 光线跟随系统，控制光源沿路径移动
- `Target.cs` - 目标点，检测光线到达
- `TargetManager.cs` - 目标管理器，管理关卡中的所有目标

### UI系统
- `GameUI.cs` - 游戏界面，显示关卡信息、进度等

### 环境系统
- `CameraController.cs` - 相机控制，包含跟随和边界限制
- `BackgroundController.cs` - 背景控制，动态背景和星空效果

### 工具类
- `InputManager.cs` - 输入管理，统一处理鼠标、触摸和键盘输入

## 安装和运行

### 系统要求
- Unity 2021.3 LTS 或更高版本
- 支持 Windows、macOS、Android、iOS 平台

### 快速开始
1. 使用Unity打开项目文件夹
2. 确保所有脚本文件已正确导入
3. 创建新场景并添加必要的游戏对象
4. 运行游戏测试功能

### 场景设置指南
1. 创建空的GameObject命名为`GameManager`，添加`GameManager`脚本
2. 创建空的GameObject命名为`LineDrawer`，添加`LineDrawer`脚本
3. 创建光源GameObject，添加`LightFollower`脚本
4. 创建目标点GameObject，添加`Target`脚本
5. 创建空的GameObject命名为`TargetManager`，添加`TargetManager`脚本
6. 创建UI Canvas，添加`GameUI`脚本
7. 设置相机，添加`CameraController`脚本
8. 创建背景，添加`BackgroundController`脚本

## 核心功能特性

### 画线系统
- 支持鼠标和触摸输入
- 自动优化路径点，减少不必要的节点
- 可配置线条宽度、颜色和材质
- 实时路径预览

### 光线跟随
- 平滑的路径跟随动画
- 动态光强度调节
- 粒子拖尾效果
- 到达检测和完成事件

### 目标系统
- 多目标支持
- 灵活的完成条件配置
- 视觉和音效反馈
- 自动进度追踪

### 用户界面
- 关卡信息显示
- 实时进度条
- 提示轮播系统
- 暂停菜单

## 自定义配置

### 画线参数
```csharp
// 在LineDrawer组件中配置
lineWidth = 0.1f;              // 线条宽度
minDistanceBetweenPoints = 0.1f; // 最小点间距
lineColor = Color.white;       // 线条颜色
```

### 光线参数
```csharp
// 在LightFollower组件中配置
followSpeed = 3f;              // 跟随速度
lightIntensity = 2f;            // 光强度
lightRange = 5f;                // 光照范围
```

### 目标参数
```csharp
// 在Target组件中配置
activationDistance = 0.5f;      // 激活距离
successColor = Color.green;     // 完成颜色
```

## 扩展功能建议

### 关卡系统
- 添加关卡数据结构
- 实现关卡加载和保存
- 设计关卡编辑器

### 特效系统
- 更多粒子效果
- 屏幕震动效果
- 时间慢动作

### 音效系统
- 背景音乐
- 交互音效
- 环境音效

### 数据存储
- 关卡进度保存
- 玩家设置
- 成就系统

## 故障排除

### 常见问题
1. **画线不显示**: 检查LineRenderer组件和材质设置
2. **光线不动**: 确认路径点数量和跟随系统设置
3. **目标不激活**: 检查碰撞体和激活距离设置
4. **UI不显示**: 确认Canvas和TextMeshPro组件

### 调试技巧
- 使用Unity Console查看错误信息
- 添加Debug.Log输出调试信息
- 使用Scene视图检查组件配置

## 贡献指南
欢迎提交问题和改进建议！

## 许可证
MIT License