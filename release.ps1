dotnet build -c Release .\external\external.sln
git commit --only .\unity\Assets\config-assets\Plugins\config-assets.sourcegen.dll -m "update sourcegen dll"
git subtree split --prefix=unity/Assets/config-assets --branch upm