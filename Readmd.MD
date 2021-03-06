Travis CI编译情况：[![Build Status](https://travis-ci.com/huduntech-dev/pdfconverter.svg?token=MfZC3wQKdkN1pbzqr6BF&branch=master)](https://travis-ci.com/huduntech-dev/pdfconverter)
# 迅捷PDF转换器 简介及版本更新情况
迅捷PDF转换器，提供PDF与Office文件、图像互相转换的功能。

# 相关版本更新情况 #
## 6.4版本 ##
**修改时间**：2016-04-25至2016-05-31  
**耗时**：5周时间。  
**测试Bug**：1周  
**编码人员**：KongMengyuan,HanHaitao  
**测试人员**：侯福涛  
**设计人员**：夏勇  
   
**内存问题**  
- 解决了内存不停增长导致崩溃问题  
- 解决了转换完成后内存始终占用的问题  
**界面更新**  
- 解决了界面在转换过程中的卡顿问题  
- 解决了拖入文件有卡顿（先读入文件再读页数）  
- 点击转换按钮之后立刻变化每条数据的小按钮    
- 修改界面显示方式,更加时尚  
- 界面启动更加流畅,快速  
**功能改进**  
- PDF转为PPT，默认选择PPTX，允许用户自己切换到PPT格式  
**新增模块**  
- 新增电子书转换  
- 新增CAD转换 

**安装包改进**  
- 增加多语言界面安装（全部都变为多语言对应内容）

## V6.3版本 ##
2016-01-04至2016-03-15,8.5周时间,春节放假12天,修改测试Bug用时1.5周 ,编码者：KongMengyuan  
**修改或增加内容**  
-     加载速度	更改窗口加载方式，解决启动速度慢的问题。  
-     多语言    制作英文版、繁体中文，并在对应的操作系统中测试。安装包也应当适应多语言。    
-               其中英文版可以完全免费。  
-     右键菜单	在PDF文件右键增加“转换到Word文件”菜单，并在其他文件右键菜单也增加类似的功能。  
-     页面	    支持输入形如“1-5;7,9;15-20,25”这样的样式，表示将文件分割为“1-5”“7,9”“15-20,25”三个文件（用分号分割）。并且在设置界面中，支持更复杂的设置。
       
**安装包	KongMY增加**  
      1、	普通用户安装没有桌面快捷方式，所有程序里面也没有快捷方式  
      2、	获取CPU的序列号不用了,使用网卡序列号,要看无线网卡是否好用,无网卡时会不会出错（出错也可以不理会,因为毕竟是网络环境才可以转换）,解决返回找不到序列号返回0 的问题. KongMY修改为先查CPU序号,没查到再查网卡MAC地址,要同时修改客服的注册号生成程序  
      3、	MainInfo使用线程通知（把慢的东西放在线程里，结束时发一个通知给其它使用的地方）  
      4、	更改目录Function OnChangeDirRequest(如果为空或者不存在,则修改为默认目录)  
        “您选择的安装路径无效，请重新选择”  
      5、	需要在安装win10时，不写入Program Files (x86)目录，而是写入ProgramData目录(win7,win8,win10都有这个目录,Win10下面它是隐藏的,有的Win10用户前者不允许更改Config.ini)。程序数据文件夹{ProgramData }是属于电脑C盘的一个系统文件夹，它是公用的被创建文件夹或文件存放的地方，这些文件夹或文件仅由创建者完整控制。在XP时代叫Documents and Settings,（记录和设置）.Vista,win7以后的系统中叫用户和ProgramData  
      完善权限相关参考[郑总写的权限文章及其链接](https://gist.github.com/huduntech/9bde433073462b0fa328)   
      6、	检测安装时的多语言，如果是英文版，就直接免费了（程序判断操作系统是否也是英文版，如果是英文版，同时config.ini里面也是language=en，符合这两个才是免费的）  

--------------------
## V6.2版本 ##
2015-11-13至2015-11-27,2周时间,修改测试Bug用时1.5周,KongMengyuan
    界面有很大变化  
    增加了自动更新功能  
    进度条更改显示方式  
    增加了总页数  
    加快了程序的显示速度  
    文件转Word增加分文档功能  
    主程序MainInfo_Load的写法完全不同(代码)  
    所有的网页访问方式重新整理,速度加快(代码)  
    对ListViewPlus部分代码重新封装(加快显示,同时修改对应的Bug)  
