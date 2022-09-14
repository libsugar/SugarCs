# LibSugar 

[![.NET](https://github.com/libsugar/SugarCs/actions/workflows/dotnet.yml/badge.svg)](https://github.com/libsugar/SugarCs/actions/workflows/dotnet.yml)
[![NPM](https://img.shields.io/npm/v/com.libsugar.sugar)](https://www.npmjs.com/package/com.libsugar.sugar)
[![Nuget](https://img.shields.io/nuget/v/LibSugar)](https://www.nuget.org/packages/LibSugar/)
[![openupm](https://img.shields.io/npm/v/com.libsugar.sugar?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.libsugar.sugar/)
![MIT](https://img.shields.io/github/license/libsugar/SugarCs)

Like syntactic sugar, but is library

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
