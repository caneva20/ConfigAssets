dotnet build -c Release .\external\external.sln
git commit --only .\unity\Assets\config-assets\Plugins\config-assets.sourcegen.dll -m "update sourcegen dll"
git subtree split --prefix=unity/Assets/config-assets --branch upm

$version = (Get-Content -Raw -Path .\unity\Assets\config-assets\package.json | ConvertFrom-Json).version
git tag -a $version -m "Version $version" upm