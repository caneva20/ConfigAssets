# Config Assets
[![openupm](https://img.shields.io/npm/v/me.caneva20.config-assets?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/me.caneva20.config-assets/)

Simple & Lightweight solution for managing configuration assets in Unity projects

## Install
The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add me.caneva20.config-assets
```

## Usage
First create a class and extend from `Config<T>`, where `T` is the class itself.

```C#
public class MyConfig : Config<MyConfig> {}
```

Then add as many fields as you need, note that it must be Serializable by Unity for it to save. Anything that is valid for a [`ScriptableObject`](https://docs.unity3d.com/Manual/class-ScriptableObject.html) is valid here as well.

```C#
public class MyConfig : Config<MyConfig> {
    [SerializeField] private string _myString;
    [SerializeField] private bool _myBool = true;

    //This also works
    public int myInt;
}
```

Your class is now accessible through a [Singleton](https://en.wikipedia.org/wiki/Singleton_pattern).
To use it just call `YOUR_CLASS_NAME.Instance.YOUR_FIELD`

``` C#
int valueFromConfig = MyConfig.Instance.myInt;
```

#

Whenever you get back to unity a new `.asset` file will be created for your configuration and it will be added to `Preloaded assets` under the player settings.
To access and configure/change your configuration file there's two options:
1. Find your `.asset` file, usually under `Configurations>Resources`
2. Through Unity's `Project Settings` under `Edit>Project Settings...` in the toolbar, and then selecting the desired configuration under the `Config assets` section

## Customization
You may add a `[Config]` attribute to your config class ir order to customize some of its properties:
```C#
[Config(Scope = SettingsScope.User)]
public class MyConfig : Config<MyConfig> { }
```
