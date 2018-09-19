# DJI-Windows-SDK-UWP-Learning
大疆windows-sdk(alpha version)学习和自己的理(miu)解(lun)

---
## Prerequisite
To use DJI windows SDK:
* A PC or laptop with a Wi-Fi adapter(5GHz recommended)
* win7 or above
* vs 2017
* Mavic Air with latest firmware

## * Setup Environment
* Install DJI Assistant 2
* Prepare Mavic Air
* Change to 2.4GHz
* Connect Mavic Air to DJi Assistant 2
* Setup Summary
:![](pics/setup-summary.JPG):

---
## Build And Run SDK Sample
There are two projects included in the solution:
* DJIWindowsSDKDemo：demonstrates the usage of SDK's interfaces
* DJIVideoParser：used to decode the video from the aircraft(FFmepg is used in the parser)

---
## SDK Explanation
Since official version of this SDK hasn't been released now, what used here is an alpha version. It is still in vert early stage and not mant features are supported yet.
It consists of four modules：SDKManager,VideoFeeder,VirtualRemoteController and ComponentManager

1. SDKManager
Only entry of DJI Windows SDK. It contains the other modules.
2. VideoFeeder
Used to receive the real-time video feed.不同的飞行器会返回不同的视频流，当前的SDk只支持基础的视频流
3. VirtualRemoteController
SDK和Mavica Air通过Wi-Fi连接，充当一个虚拟遥控器。该模块包含一个SetJoyStickValue(float, float, float, float)接口以模拟操纵杆
4. ComponentManager
SDK将飞行器抽象成多个部分，该模块包含各个模块的管理

---
## 注意事项
* 当SDKManager 激活的时候SDKManager返回的ComponentManager和其返回的句柄必须为non-null[没有飞行器连接的时候句柄可用但会报错]
* 执行结果并不是立即返回的，SDK和飞行器之间的信息交互是异步的
---
## error occured when building,debuging...
1. *错误 C7510 “Callback”: 模板 从属名称的使用必须以“模板”为前缀*
* C/C++ -> 语言 -> 符合模式：关闭/permissive-[取消禁用多语言扩展，本是为了适应跨平台跨编译器编译Windows C++代码做的检查，但是拦住了WRL里的Callback末班函数的调用]
2. *错误 CS0246	未能找到类型或命名空间名“InkShapes”(是否缺少 using 指令或程序集引用?)
---
---
## Deep-Diging into DJIVideoParser with ffmepg
深入理解视频解析部分
### 1. djivideoparser.cpp：入口
* 初始化和销毁
* SetWindow(int product_id, int product_type, int index, void* window)
* set_callback(int product_id, int index, std::function<void(uint8_t *data, int width, int height)> func)
* set_parser_data(int product_id, int index, const unsigned char *data, size_t data_length)
### 2. modulemediator.cpp:VideoParserMgr类型的共享内存区的管理
* 初始化和销毁
### 3. videoparsermgr.cpp：管理各个设备传回的视频流，放在一个videoparser.VideoParser类型的共享内存区
* 初始化和销毁
* AddDevice(int product_id)
* ParserData(int product_id, int component_index, const unsigned char* data, unsigned int len)
* RemoveDevice(int product_id)
### 4. videoparser.cpp：VideoParser类型的共享内存区处理数据的入口
* 初始化和销毁 
对Previewer、DjiCodec和VideoWrapper进行初始化
* ParserData(const unsigned char* buff, unsigned int size)
根据不同的码长区分不同飞行器返回的数据流，然后调用VideoWrapper.PutToQueue()
### 5. VideoWrapper.cpp：封装了视频解析中的视频帧回退清空暂停等操作
* s_rate=30 -> fps?
* 初始化和销毁
* SetVideoFrameCallBack(std::function<void(uint8_t *data, int width, int height)> func)
* PauseParserThread(bool isPause)
* ClearFrame()
* PutToQueue(const uint8_t* buffer, int size, uint64_t pts)
根据pts区分数据流是实时视频还是视频帧，然后调用H264_Decoder.videoFrameParse()
* PutVideoToQueue(const uint8_t* buffer, int size, uint64_t pts)
* FramePacket(uint8_t* buff, int size, FrameType type, int width, int height)
### 6. h264_Decoder.cpp：解码部分with ffmepg
* videoFrameParse(const uint8_t* buff, int video_size, FrameType type, uint64_t pts)
将数据流加入队列，填充ffmepg

### 7. Queue.cpp：基本数据结构
### 8. Utils.cpp：工具


---
---
---

# From Official github samples:DJI SDK Sample for Windows 10 and Universal Windows Platform 
![Screenshot](http://www2.djicdn.com/assets/images/products/mavic-air/banner/drone-2aa05a6dc1fd6c94dfe80f6bd4a907ba.png?from=cdnMap)

This sample is a [Universal Windows Platform](https://docs.microsoft.com/en-us/windows/uwp/get-started/universal-application-platform-guide)* app that demonstrates DJI Windows SDK capabilities: 
* Aircraft's camera video feed 
* Aircraft control (Joysticks and Takeoff/Home functionality)
* Camera gimbal control 
* Flight telemetry data events

The app leverages XAML-based UI and Windows ML, running inference (evaluation) on top of aircraft's video feed using a [Windows ML](https://docs.microsoft.com/en-us/windows/uwp/machine-learning/) model. 

![Screenshot](https://user-images.githubusercontent.com/4735184/39741103-4605c4fc-524d-11e8-92d4-0ff200b501ca.jpg)

## Windows ML ##  
This sample uses a WinML model called InkShapes. It was made by [Nikola Metulev](https://github.com/nmetulev) and trained in [Custom Vision](https://www.customvision.ai/) on simple black-and-white hand-drawn pictures representing one of 21 categories, such as house, flower, stick figure, bike, and others.

With Custom Vision, you can train an AI model using your own images, then export it to different model formats including [ONNX](https://onnx.ai). 

The model is not smart enough yet. House, flower, and stick figure are recognized quite well. 
You may help Nikola training the model - just by drawing shapes in [Draw the shape!](https://www.microsoft.com/store/productId/9NBLLH7XBRSR) app. 

## Requirements: ##
1. [Windows 10 April 2018 Update](https://www.microsoft.com/en-us/software-download/windows10).  
2. [Windows 10 SDK](https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk) 1803 (for April 2018 Update).
3. [Visual Studio 2017](https://www.visualstudio.com/vs/) with Universal Windows Platform tools (including C++ tools for UWP), Desktop C++ Development and Desktop .NET development.
4. **DJI SDK for Windows**. All BUILD 2018 attendees should have a link to DJI Windows SDK alpha. 

## How to build ## 

**You need DJI Windows SDK.**

### Prepare the SDK: ### 
DJI Windows SDK is not included in this repo. You need to obtain it from DJI, and copy 2 folders from the SDK to this project's folder. 
1. Copy **DJIWindowsSDK** folder from the SDK's package to the root of this project. 
2. Copy **ThirdParties** folder from the SDK's sample (DJIWindowsSDKDemo/ThirdParties) to the root of this project. 
![How to copy SDK folders](https://user-images.githubusercontent.com/4735184/39739976-6c716f10-5248-11e8-9466-3e88e04bea03.jpg) 

Note, these 2 folders are in .gitignore, so if you want to have them in your repo, please remove these 2 lines from .gitignore: 
```
ThirdParties/
DJIWindowsSDK/ 
```

### Build with Visual Studio 2017: ### 
1. Once you copied all required dependencies, open **WinDrone.sln** in Visual Studio 2017 
2. Right-click on **DJIUWPDemo**, click 'Set as StartUp Project' 
3. Build, debug, deploy, enjoy! 

## *Platform notes ##
Current alpha version of DJI SDK only supports x86 architecture on Windows desktop. It has a few "classic" Win32 dependencies, so the app package requires Full Trust capability (using [Desktop Bridge](https://docs.microsoft.com/en-us/windows/uwp/porting/desktop-to-uwp-extensions)), and only runs on Windows 10 Desktop. 
Video decoding component doesn't leverage hardware acceleration yet. 

The sample C# app uses the SDK via an additional DJIClient DLL and PInvoke calls. 

Full Universal Windows Platform support and other improvements are coming later towards the release of DJI Windows SDK. 


