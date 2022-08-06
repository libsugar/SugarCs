# LibSugar 

[![NPM](https://img.shields.io/npm/v/com.libsugar.sugar)](https://www.npmjs.com/package/com.libsugar.sugar)
[![Nuget](https://img.shields.io/nuget/v/LibSugar)](https://www.nuget.org/packages/LibSugar/)
[![openupm](https://img.shields.io/npm/v/com.libsugar.sugar?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.cn/packages/com.libsugar.sugar/)
![MIT](https://img.shields.io/github/license/libsugar/SugarCs)

类似语法糖，但是是库

## 安装

- [Nuget](https://www.nuget.org/packages/LibSugar/)
  ```
  dotnet add package LibSugar --version <version>
  ```
  或
  ```
  <PackageReference Include="LibSugar" Version="<version>" />
  ```

- Unity Package 由 [npmjs](https://www.npmjs.com/package/com.libsugar.sugar)

  如下编辑你的 `Packages/manifest.json` 文件

  ```json
  {
    "scopedRegistries": [
      {
        "name": "npm",
        "url": "https://registry.npmjs.org",
        "scopes": [
          "com.libsugar"
        ]
      }
    ],
    "dependencies": {
      "com.libsugar.sugar": "<version>"
    }
  }
  ```
  或者在 unity 编辑器中操作  
  配置 `Project Settings -> Package Manager -> Scoped Registeries`  
  然后在包管理器中添加包  
