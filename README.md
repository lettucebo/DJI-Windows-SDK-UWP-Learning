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


