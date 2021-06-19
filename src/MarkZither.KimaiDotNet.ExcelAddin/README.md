# KimaiDotNet Excel Add-in

## Building the project
This project uses postsharp to implement a global exception handler, at first run you will be
prompted to accept the postsharp license, please accept the Community License.

Installing the postsharp tooling is optional.

## Postsharp configuration
The logging must run in developer mode to [comply with the license](https://doc.postsharp.net/logging-license), that is controlled by the
postsharp.config `<Property Name="LoggingDeveloperMode" Value="True" />` this means it will only
log for 24 hours after compilation, more details can be found in the [Developer Mode logging documentation](https://doc.postsharp.net/logging-license#developer).

This produces a warning to alert you to the fact logging will only work until a specific date.

``` bash
Severity	Code	Description	Project	File	Line	Suppression State
Warning		MarkZither.KimaiDotNet.ExcelAddin.dll is being built with PostSharp Logging Developer Edition. 
Unless you are using Flashtrace, no log records will be emitted if MarkZither.KimaiDotNet.ExcelAddin.dll 
is executed after 06/20/2021 10:24:37. If you are using Flashtrace, ignore this message and 
suppress it by adding <PostSharpDisabledMessages>DIA005</PostSharpDisabledMessages> to your .csproj file.	MarkZither.KimaiDotNet.ExcelAddin			
```

I have supressed that message by adding `<PostSharpDisabledMessages>DIA005</PostSharpDisabledMessages>` to the csproj.

## Office Image Browser
https://www.spreadsheet1.com/office-excel-ribbon-imagemso-icons-gallery-page-02.html
https://bert-toolkit.com/imagemso-list.html