## 更新记录

### V1.0

- HGUI的第一个正式版本
- 基本UI包含UIElement,HGraphics,HText,HImage,HLine,HCanvas 6种基本UI元素
- 事件包含UserEvent,TextInput,GestureEvent 三种事件
- 复合型UI包含Slider,ScrollX,ScrollY,GridScroll,Paint,Rocker,UIContainer,TreeView,UIDate,UIPalette,
- ScrollYExtand,DropDown,StackPanel,TabControl,DockPanel,DesignedDockPanel, DragContent,DataGrid,InputBox
- 数据处理与压缩包含Lzma,lz4,DataBuffer,Ini,QueueBuffer,SwapBuffer等等
- 网络处理包含TCP,UDP,KCP
- 数学函数包含一套简单的2D无力碰撞算法
- UI页面管理包含UIBase,UIMenu,UINotify,UIPage,PopWindow.带有自动回收再利,尺寸自适应，语言自动切换，数据通信等功能
- Tween简单的UI动画，当切换页面时，当前动画将被全部释放
- HCanvas网格合批处理，合批后生成多个网格和材质球,每个材质球支持4张纹理,每帧总计会造成11次GC.Alloc.
- DataBuffer和BlockBuffer都为安全类型的指针数据处理类，但是BlockBuffer申请的内存BlockInfo<T>需要手动释放
- 其它就不多做介绍了，例如音频管理器UIAudioManager