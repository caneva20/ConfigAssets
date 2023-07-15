# Config Assets

Simple & Lightweight solution for managing configuration assets in Unity projects

## Install

1. Install `Editor Coroutines` from Unity's package manager
    ```shell
    com.unity.editorcoroutines
    ```
    
    > You can access the package manager by going to `Window > Package Manager` at the top bar in Unity 

2. Download & import the latest `config-assets.unitypackage` from [here](https://github.com/caneva20/ConfigAssets/releases/latest)
or the [releases](https://github.com/caneva20/ConfigAssets/releases) page.

# Usage

First create a `partial class` and add the `Config` attribute to it

```C#
[Config]
public partial class MyConfig {
    // Your fields & attributes
}
```

Then add as many fields as you need, note that it must be Serializable by Unity for it to save. Anything that is valid
for a [`ScriptableObject`](https://docs.unity3d.com/Manual/class-ScriptableObject.html) is valid here as well.

```C#
[Config]
public partial class MyConfig {
    [SerializeField] private string _myString;
    [SerializeField] private bool _myBool = true;

    //This also works
    public int myInt;
}
```

Your class is now accessible through a direct static access.
<br>To use it just call `YOUR_CLASS_NAME.YOUR_FIELD`

``` C#
int valueFromConfig = MyConfig.myInt;
```

<br>

* Whenever you get back to Unity, a new `.asset` file will be created for your configuration and it will be added
  to `Preloaded assets` under the player settings.
* You can access your configuration through Unity's `Project Settings` under `Edit>Project Settings...` in the toolbar,
  and then selecting the desired configuration under the `Config assets` section.

# Customization

The `[Config]` attribute has some properties to allow you to customize you configuration a bit:

| Attribute           | Description                                                                                                                                                               | Default                         |
|---------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------|
| `DisplayName`       | The name used under `Project Settings`                                                                                                                                    | `null` (Will use the type name) |
| `EnableProvider`    | Whether or not to generate a [`SettingsProvider`](https://docs.unity3d.com/ScriptReference/SettingsProvider.html)                                                         | `true`                          |
| `Scope`             | The [scope](https://docs.unity3d.com/ScriptReference/SettingsScope.html) used by the [`SettingsProvider`](https://docs.unity3d.com/ScriptReference/SettingsProvider.html) | `SettingsScope.Project`         |
| `Keywords`          | The keywords used by the [`SettingsProvider`](https://docs.unity3d.com/ScriptReference/SettingsProvider.html)                                                             | none/empty                      |
| `GenerateSingleton` | Whether or not to generate the `.Instance` property                                                                                                                       | `true`                          |