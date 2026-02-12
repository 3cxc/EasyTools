# EasyTools
--------
[![Latest Release](https://img.shields.io/github/v/release/3cxc/EasyTools)](https://github.com/3cxc/EasyTools/release)
[![License](https://img.shields.io/github/license/3cxc/EasyTools.svg)](https://github.com/3cxc/EasyTools/blob/master/LICENSE)

## 介绍
EasyTools 是一个基于 LabAPI 的插件，它提供了一些简单的自定义玩法功能，以满足自定义服务器的需求。

该插件最初是为了我个人的服务器而创建的，现将其开源。

## 可用命令
- .ac 可向局内管理反馈，管理也可用此命令互相聊天
- .bc 公屏聊天
- .c(.cc) 队伍内聊天
- .killme(.suicide) 自杀命令

## 自定义功能
- D级人员开局给卡
- 定时公告
- 文本聊天系统
- SCP血量调整系统
- 保安下班
- 粉糖系统
- 玩家进出日志记录
- SCP-207(可乐)无害化
- SCP-1853(洗手液)无害化
- SCP站立回血系统
- 硬币抽卡系统
- SCP-3114随机生成系统
- 称号系统

## 安装方式
> [!NOTE]
> 本插件依赖 [HintSeviceMeow](https://github.com/MeowServer/HintServiceMeow) 的 LabAPI 版本

1. 前往HintSeviceMeow的[Releases](https://github.com/MeowServer/HintServiceMeow)下载`HintServiceMeow-LabAPI.dll`
2. 将`HintServiceMeow-LabAPI.dll`重命名为`0HintServiceMeow.dll`并放入**插件**目录下:
   - Linux: `.config/SCP Secret Laboratory/LabAPI/plugins/global/`
   - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/global/`
3. 在[Releases](https://github.com/3cxc/EasyTools/releases)下载`EasyTools.dll`和`dependencies.zip`
4. 将`EasyTools.dll`放入**插件**目录下:
   - Linux: `.config/SCP Secret Laboratory/LabAPI/plugins/global/`
   - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/global/`
5. 将`dependencies.zip`解压出来的文件放入**依赖**目录下:
   - Linux: `.config/SCP Secret Laboratory/LabAPI/dependencies/global/`
   - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/dependencies/global/`
6. 重启服务器

## 基于项目
- [HelpSense](https://github.com/XLittleLeft/HelpSense) 本项目的基础代码来源
- [NewDIR](https://github.com/YF-OFFICE/NewDIR) 称号系统
- HUDInfo 914合成提示
- UsefulAdditions 快速刷新(快速重生)
