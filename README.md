## 📦 How To Clone

Make sure you have the following installed:

- **.NET SDK 8 or newer** (SDK 10 also works)
- **Git**

Check your installed SDK version:

```bash
dotnet --version
```


## ⬇️ 1-Download .NET SDK

If you don't have the **.NET SDK** installed. 
You can download the latest SDK from the official Microsoft website:

[Download .NET SDK](https://dotnet.microsoft.com/en-us/download)

After installing, verify the installation:

```bash
dotnet --version
```
##📥 2-Clone the Repository:
```bash
git clone https://github.com/Mohamed-ALQarram/AtConnect.git
cd AtConnect
```

## ⚙️ 3-Configure App Settings

Before running the project, you need to configure some settings.  
The main configuration is in `appsettings.json` (or `appsettings.Development.json`).

Typical settings you may need to update:

```json
{
  "ConnectionStrings": {
    "AtConnectPostgresConnection": "Your-Connection"
  },
  "AtConnect": {
    "Jwt": {
      "Issuer": "https://Your-Issuer",
      "Audience": "https://Your-Audience",
      "LifeTime": "00:05:00",
      "SigningKey": "Your-Signing-key"

    },
    "Smtp": {
      "Host": "smtp.gmail.com",
      "Port": "587",
      "Email": "Your-Gmail",
      "Password": "Your-Password"
    }
  }
  "AllowedHosts": "*"
}
```

## 📦 4-Restore Dependencies

Restore all NuGet packages automatically:
```bash
dotnet restore
```

## ▶️ Run the API
build the project
```bash
dotnet build
```
Run the project:
```bash
dotnet run
```

## ▶️ Run the API

After running the project, the API will be available at:

**HTTPS:** [https://localhost:7217](https://localhost:7217)

---

## 📑 API Documentation (Swagger)

You can explore and test the API endpoints using Swagger UI:

[https://localhost:7217/swagger](https://localhost:7217/swagger)


