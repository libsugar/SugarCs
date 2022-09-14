# LibSugar 

[![.NET](https://github.com/libsugar/SugarCs/actions/workflows/dotnet.yml/badge.svg)](https://github.com/libsugar/SugarCs/actions/workflows/dotnet.yml)
[![NPM](https://img.shields.io/npm/v/com.libsugar.sugar)](https://www.npmjs.com/package/com.libsugar.sugar)
[![Nuget](https://img.shields.io/nuget/v/LibSugar)](https://www.nuget.org/packages/LibSugar/)
[![openupm](https://img.shields.io/npm/v/com.libsugar.sugar?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.libsugar.sugar/)
![MIT](https://img.shields.io/github/license/libsugar/SugarCs)
[![ApiDoc](https://img.shields.io/badge/ApiDoc-222222?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAXCAYAAAAV1F8QAAAACXBIWXMAAAsSAAALEgHS3X78AAABpElEQVRIib2WQU7CUBCGfwh7dOdO8sc9uHMnJzC6N7Hu3KknsJzA3kC8AZ5AuEE5ABNMXLhE414zZJ4OpS1gin8C75U3ne9N5zHT2mQyiQC0sKghgC7KNQMwIDldYTdXwxxe5KzdrXH/vYiMAMQkh2WGdZIaUW+dXRXoGMCziCS6LCI7uSD9IhkDeCpw9A7gkGQtfADsAjgDMHJ21yLSBxCJyGkuyJQWgFKSC2skZyQ1P/rYL92SpmAPQF9Eok1BpSLZz8CuFATgwSJcAs3+AsIvLDz6JoCPEGGIrF58+8aK3Q0nAMY2jysFWR5f7LLtUrEvIp0qI1IV/XkrB3m9uXnrv0CVHoZSbRN04C+2Cfp082nVoI6NesyPbK61clAZyCpA0y4HVtVVidbGSkDWGhL306uNY+sMCznqIF+5/SUDGbpotLed2yP7aRceVOSwXQLpGiTYPFpx1leDrm/zDberqMRhag5S25A60tFvQiFaxW90XfPifTSsG6pBCL0sqpDgrG5JJiKigKXuqtK3oC87jr4gzltyxtZDgr2ertVvQgC+AfGSgtzXl3gYAAAAAElFTkSuQmCC)](https://libsugar.github.io/SugarCs/api/LibSugar.html)

Like syntactic sugar, but is library

---

[API Documentation](https://libsugar.github.io/SugarCs/api/LibSugar.html)

## Installation

- [Nuget](https://www.nuget.org/packages/LibSugar/)
  ```
  dotnet add package LibSugar --version <version>
  ```
  or
  ```
  <PackageReference Include="LibSugar" Version="<version>" />
  ```

- Unity Package by [npmjs](https://www.npmjs.com/package/com.libsugar.sugar)

  Edit your `Packages/manifest.json` file like this

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
  or use gui in unity editor  
  config `Project Settings -> Package Manager -> Scoped Registeries`  
  then add package in package manager  
