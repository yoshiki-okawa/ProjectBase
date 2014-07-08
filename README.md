Project Base
===========

Base project template for medium to large software. .NET, WebAPI, OWIN self host, SignalR, SPA using AngularJS, MS Unity, OAuth, Multilayered architecture

## Introduction

Nowadays, we have many great open source frameworks, but there is no single project which shows real usages of them.
Project Base aims to help you start a new project which makes use of many open source frameworks, such as SignalR, AngularJS, MS Unity.

## Step By Step Setup

### 0. Pre requisites

Visual Studio 2013 with Update 2 http://www.visualstudio.com/downloads/download-visual-studio-vs

### 1. Download source codes

Clone https://github.com/yoshikiokawa/ProjectBase.git

I use TortoiseGit. https://code.google.com/p/tortoisegit/wiki/Download

### 2. Install SSL certificate for localhost:8000 if required (Recommended)

Run 'C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\Shortcuts\Developer Command Prompt for VS2013'

Run the following command:
```
makecert -r -pe -n "CN=localhost" -b 01/01/2000 -e 01/01/2050 -eku 1.3.6.1.5.5.7.3.1 -ss my -sr localMachine -sky exchange -sp "Microsoft RSA SChannel Cryptographic Provider" -sy 12
```
Find thumbprint of the certificate created. If you do not know how to find it, please follow instruction in http://msdn.microsoft.com/en-us/library/ms734695%28v=vs.110%29.aspx

Open command prompt and run the following command:
```
netsh http add sslcert ipport=0.0.0.0:8000 certhash=0000000000003ed9cd0c315bbb6dc1c08da5e6 appid={00112233-4455-6677-8899-AABBCCDDEEFF} 
```
Replace certhash with the thumbprint found above and appid with any GUID.

### 3. Use 64 bit IIS Express (Optional)

If you prefer running IIS Express in 64 bit, run the following command using command prompt:
```
reg add HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\12.0\WebProjects /v Use64BitIISExpress /t REG_DWORD /d 1
```

### 4. Build

Open ProjectBase.sln

Build -> Build Solution - This might take a while first time because it will download references from nuget.

Build -> Transform All T4 Templates

## TODOs

