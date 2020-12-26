# HGUI

#### 介绍
HGUI升级版，UI元素移除MonoBehaviour

#### 软件架构
软件架构说明

#### 安装教程

1.  打开工程的下的文件Packages/manifest.json
2.  添加对应的版本包，例如
    "dependencies": {
    "com.huqiang.hguiframework": "https://gitee.com/huqiang0204/hgui2.git#1.1.7"
	}
3.  打包时注意事项，请确保ProjectSetting面板 Graphics中添加了Shader HGUI/UIDef，否则移动端会报错

#### 使用说明

1.  首先在你的Editor界面编辑你的UI
2.  在canvas上面添加TestPageHelper，或其子类
3.  将你的AssetBundle打包至StreamingAssets文件夹中，打包的UI需要关联AssetBundle中的资源
4.  点击TestPageHelper的Inspector面板中的create按钮，UI数据包将默认生成在AssetsBundle文件夹中
5.  生成成功会得到一个bytes文件，具体使用看我demo，在app中
6.  将bytes数据加载到管理器HGUIManager.LoadModels(baseUI.bytes, "baseUI");
7.  异步加载你的相关Assetbundle，完毕后加载页面
    ElementAsset.LoadAssetsAsync("base.unity3d").PlayOver = (o, e) =>
    {
        UIPage.LoadPage<StartPage>();
    };
8.  或者你没有AssetBundle，可以直接加载页面UIPage.LoadPage<StartPage>();
9.  UI管理方面有自己的回收池，切换界面使用UIPage.LoadPage即可
10. 视频教程地址https://space.bilibili.com/19050274/video 不定期更新
11. 函数文档https://huqiang0204.gitee.io/home
12. 注释使用请将PackageCache\com.huqiang.hguiframework@...\HGUI.xml复制到Library\ScriptAssemblies\文件夹中

#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request


#### 码云特技

1.  使用 Readme\_XXX.md 来支持不同的语言，例如 Readme\_en.md, Readme\_zh.md
2.  码云官方博客 [blog.gitee.com](https://blog.gitee.com)
3.  你可以 [https://gitee.com/explore](https://gitee.com/explore) 这个地址来了解码云上的优秀开源项目
4.  [GVP](https://gitee.com/gvp) 全称是码云最有价值开源项目，是码云综合评定出的优秀开源项目
5.  码云官方提供的使用手册 [https://gitee.com/help](https://gitee.com/help)
6.  码云封面人物是一档用来展示码云会员风采的栏目 [https://gitee.com/gitee-stars/](https://gitee.com/gitee-stars/)
