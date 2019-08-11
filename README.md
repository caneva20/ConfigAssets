# Config Assets
Simple &amp; Lightweight solution creating and loading config assets for Unity projects

## Install

Find `Packages/manifest.json` in your project and edit it to look like this:
```js
{
  "dependencies": {
    "caneva20.config-assets": "https://github.com/caneva20/ConfigAssets.git#master",
    ...
  },
}
```
### Or
You may also use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension)


## Usage
First create a class and extend from `Config<T>`

```C#
public class MyConfig : Config<MyConfig> {}
```

Then add as many fields as you need, note that it must be Serializable by Unity for it to save

```C#
public class MyConfig : Config<MyConfig> {
    [SerializeField] private string _myString;
    [SerializeField] private bool _myBool = true;
    
    //This also works
    public int myInt;
}
```

And lastly just call `YOUR_CLASS_NAME.Instance.YOUR_FIELD`
int valueFromConfig = MyConfig.Instance.myInt;
``` C#
int valueFromConfig = MyConfig.Instance.myInt;
```