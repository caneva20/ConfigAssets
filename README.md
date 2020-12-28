# Config Assets
[![openupm](https://img.shields.io/npm/v/caneva20.config-assets?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/caneva20.config-assets/)

Simple & Lightweight solution for managing configuration assets in Unity projects

## Install
The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add caneva20.config-assets
```

**Or**

Use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension)

**Or**	

Find `Packages/manifest.json` in your project and edit it to look like this:
```json
{
  "dependencies": {
    "caneva20.config-assets": "https://github.com/caneva20/ConfigAssets.git#0.2.0-preview.3",
    ...
  },
}
```

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

And lastly, just call `YOUR_CLASS_NAME.Instance.YOUR_FIELD`

``` C#
int valueFromConfig = MyConfig.Instance.myInt;
```
