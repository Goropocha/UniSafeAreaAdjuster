# UniSafeAreaAdjuster

## What's This?
Unity plugin that adjust canvas to [Safe Area](https://developer.apple.com/design/human-interface-guidelines/ios/visual-design/adaptivity-and-layout/) without play at Editor or build in iOS.<br>
**Available Orientation: Landscape and Portrait**<br>
**Available iPhone: iPhone X, iPhone XS, iPhone XR, iPhone XS Max, iPad Pro 11, iPad Pro 12.9 (3rd generation)**

Sample Project was created Unity version 2018.3.13f1.<br>

![Landscape Safe Area GIF](https://camo.qiitausercontent.com/61b190ef7b546e78c8bdfa094120837a2d909d68/68747470733a2f2f71696974612d696d6167652d73746f72652e73332e616d617a6f6e6177732e636f6d2f302f3133333339392f33346162323039642d356362392d343732612d646635352d3132353830663431336164392e676966 "Landscape Safe Area")

![Portrait Safe Area GIF](https://camo.qiitausercontent.com/096e9046ac194bec428fd66f61646fa5daafed26/68747470733a2f2f71696974612d696d6167652d73746f72652e73332e616d617a6f6e6177732e636f6d2f302f3133333339392f39653639643361342d653863662d626663362d383932352d3031663465376332313136352e676966 "Portrait Safe Area")

## How to Use
1. Adjust GameScene Screen (See the following screen. If you use 2018.3 or higher, then you needn't to create custom aspects.)<br>
![Adjust GameScene Screen](https://camo.qiitausercontent.com/5556a669f8753f81fb6d9f30633fd54d6d83a282/68747470733a2f2f71696974612d696d6167652d73746f72652e73332e616d617a6f6e6177732e636f6d2f302f3133333339392f31353938623336342d363735342d353531322d346661322d3066313936346264646430302e706e67)
2. Create GameObject into Canvas and atach `SafeAreaAdjuster.cs` to GameObject you created. Then, set UI Game Objects (Such as Image, Text, ScrollRect ect..) to child of it. (See the following screenshot. In the next screenshot, the Text GameObjects corresponds to the SafeArea GameObject.)<br>
![](https://camo.qiitausercontent.com/76f22a053fe793ddf37db286cb6cebce9f4ace54/68747470733a2f2f71696974612d696d6167652d73746f72652e73332e616d617a6f6e6177732e636f6d2f302f3133333339392f30363432663062372d613630392d303130332d643461382d3361336438386237346331622e706e67)
3. SafeArea Game object RectTransform should be spread out as shown below:<br>
![Game object RectTransform](https://camo.qiitausercontent.com/f01503d5a8f65913214f2672a2b0aa46c264b427/68747470733a2f2f71696974612d696d6167652d73746f72652e73332e616d617a6f6e6177732e636f6d2f302f3133333339392f38626334393034322d383733632d326538612d643130372d3664383066383234646531652e706e67)
4. Done.

## How to Customize Safe Area
1. Open `SafeAreaAdjusterSimulateData.cs`
2. Add New SafeArea type to `SimulateType`.
3. Add New Resolution (px) to `public static Vector2Int[] Resolutions`. You must match SimulateType and the index of array.
4. Add New SafeArea's Resolution (px) to `public static Rect[,] SafeAreaResolutions`. You must match SimulateType and the index of array.

## Document (Japanese)
[iPhoneX以降のセーフエリアの見栄えをエディタ上で、かつ実行せずに確認・調整できる便利クラスを作成しました](https://goropocha.hatenablog.com/entry/2020/03/26/003011)

## Version History

### 1.1.0 (April 21, 2019)
* Support iPad Pro 11, iPad Pro 12.9 (3rd generation)
* Fixed Problem about Screen.Width and Height if you use Unity versin 2018.3.3 or above.
### 1.0.0 (October 18, 2018)
* First release

## Author
[Goropocha](https://github.com/Goropocha)

## License
[The MIT License](https://github.com/Goropocha/UniSafeAreaAdjuster/blob/master/LICENSE)

