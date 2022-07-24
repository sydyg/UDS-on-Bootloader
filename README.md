# UDS on CAN上位机
上位机分为驱动层、协议层、应用层，实现UDS的BootLoder刷写，另外可以根据该上位机的软件框架，按照自己的需求，开发出诊断、下线标定等功能。
## 目录介绍
### bootloader目录
是bootloader相关的代码，主要实现bootloader流程，CRC校验，文件解析功能。
## driver目录
主要是ZLG、CANoe、Kvaser、Vspy等设备的驱动
## CANAPI目录
实现了设备启动、关闭、CAN报文接收、CAN报文发送等功能。
本项目提供了一个演示demo，供开发者学习使用。
要实现具体的功能，还需要一些配置的工作，可以联系作者支持。
微信：sydygys。
