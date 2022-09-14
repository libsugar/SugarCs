# LibSugar 

[![.NET](https://github.com/libsugar/SugarCs/actions/workflows/dotnet.yml/badge.svg)](https://github.com/libsugar/SugarCs/actions/workflows/dotnet.yml)
[![NPM](https://img.shields.io/npm/v/com.libsugar.sugar)](https://www.npmjs.com/package/com.libsugar.sugar)
[![Nuget](https://img.shields.io/nuget/v/LibSugar)](https://www.nuget.org/packages/LibSugar/)
[![openupm](https://img.shields.io/npm/v/com.libsugar.sugar?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.cn/packages/com.libsugar.sugar/)
![MIT](https://img.shields.io/github/license/libsugar/SugarCs)
[![Api文档](https://img.shields.io/badge/Api%20文档-222222?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAXCAYAAAAV1F8QAAAACXBIWXMAAAsSAAALEgHS3X78AAABpElEQVRIib2WQU7CUBCGfwh7dOdO8sc9uHMnJzC6N7Hu3KknsJzA3kC8AZ5AuEE5ABNMXLhE414zZJ4OpS1gin8C75U3ne9N5zHT2mQyiQC0sKghgC7KNQMwIDldYTdXwxxe5KzdrXH/vYiMAMQkh2WGdZIaUW+dXRXoGMCziCS6LCI7uSD9IhkDeCpw9A7gkGQtfADsAjgDMHJ21yLSBxCJyGkuyJQWgFKSC2skZyQ1P/rYL92SpmAPQF9Eok1BpSLZz8CuFATgwSJcAs3+AsIvLDz6JoCPEGGIrF58+8aK3Q0nAMY2jysFWR5f7LLtUrEvIp0qI1IV/XkrB3m9uXnrv0CVHoZSbRN04C+2Cfp082nVoI6NesyPbK61clAZyCpA0y4HVtVVidbGSkDWGhL306uNY+sMCznqIF+5/SUDGbpotLed2yP7aRceVOSwXQLpGiTYPFpx1leDrm/zDberqMRhag5S25A60tFvQiFaxW90XfPifTSsG6pBCL0sqpDgrG5JJiKigKXuqtK3oC87jr4gzltyxtZDgr2ertVvQgC+AfGSgtzXl3gYAAAAAElFTkSuQmCC)](https://libsugar.github.io/SugarCs/api/LibSugar.html)

类似语法糖，但是是库

---

[API 文档](https://libsugar.github.io/SugarCs/api/LibSugar.html)

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
