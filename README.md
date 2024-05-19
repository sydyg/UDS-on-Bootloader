# UDS on CAN上位机
上位机分为驱动层、协议层、应用层，实现UDS的BootLoder刷写，另外可以根据该上位机的软件框架，按照自己的需求，开发出诊断、下线标定等功能。
## 目录介绍
### bootloader目录
是bootloader相关的代码，主要实现bootloader流程，CRC校验，文件解析功能。
## driver目录
主要是ZLG、CANoe、Kvaser、Vspy等设备的驱动
## CANAPI目录
实现了设备启动、关闭、CAN报文接收、CAN报文发送等功能。
## 总结
本项目提供了一个演示demo，供开发者学习使用。
## 作者微信：sydygys。
可以自行学习开发教程：https://www.ind4.net/#/HomePage/CourseDetail?navType=Curriculum&courseId=3204。

有问题可以加作者微信。
## 支持定制化开发，已为某ADAS企业开发该上位机，稳定运行，需要的可联系。
![image](https://user-images.githubusercontent.com/24352068/201505142-1090c2e7-e928-43e8-aec0-361bafd601b0.png)
还有很多别的工具，比如Excel2DBC工具、实车路由测试工具、标定和故障码读取工具等，也可以开发，有需要的朋友可以联系。
