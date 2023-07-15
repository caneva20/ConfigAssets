echo "Building sourcegen dll"
dotnet build -c Release .\external\external.sln
git commit --only .\unity\Assets\Plugins\config-assets.sourcegen.dll -m "update sourcegen dll"

echo "Updating tags"
$version = (Get-Content -Raw -Path .\unity\Assets\Plugins\config-assets\package.json | ConvertFrom-Json).version
git tag -a $version -m "v$version"

echo "Exporting Unity package"
# Unity must be added to PATH, usually at C:\Program Files\Unity\Hub\Editor\{VERSION}\Editor\
Start-Process -NoNewWindow -FilePath "Unity.exe" -ArgumentList "-quit", "-batchmode", "-nographics", "-projectPath", "unity", "-exportPackage", "Assets/Plugins", "config-assets.unitypackage" -PassThru | Wait-Process

# GitHub cli must be installed and logged in, install from: https://cli.github.com
echo "Creating Github release"
gh release create v$version --title "v$version"
gh release upload v$version unity/config-assets.unitypackage

echo "done exporting"