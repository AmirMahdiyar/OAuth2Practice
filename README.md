# 🛡️ OAuth2 & User Identity Management System

An advanced **ASP.NET Core Web API** practice project designed to explore modern authentication flows. 

---

## ⚠️ Disclaimer & Purpose

> [!IMPORTANT]
> **Educational Purpose Only**: This repository is a **practice project**. The primary goal is to demonstrate the implementation of complex authentication logic and OAuth2 flows. 
> 
> **Architecture Note**: While functional, the focus was on "how it works" rather than "perfect clean architecture" or "production-grade optimization." 
> 
> **Recommendation**: Feel free to use this as a reference or a blueprint for your own logic, but ensure you implement proper clean architecture, security hardening (like password hashing), and error handling before using it in a real-world application.

---

## ✨ Key Features

* **🌐 Multi-Provider OAuth2**: Seamless integration with Google and GitHub using the **Strategy Design Pattern**.
* **🔐 Practice-Focused Implementation**: Real-world logic for handling access tokens and payloads.
* **📧 Smart Email System**: Automated password reset/set flows via **MailKit** and Gmail SMTP.
* **🎟️ Secure JWS Tokens**: Custom JSON Web Signature (JWS) generation for user sessions.
* **🛠️ Flexible Auth Flows**: Supports linking multiple social accounts to a single local user account.

---

## 🏗️ Architecture & Design Patterns

### 🧩 Strategy Pattern for OAuth
The system uses an `IOAuthStrategyRepository` to dynamically resolve the correct authentication logic at runtime.
- **Google**: Validates ID Tokens using official Google auth libraries.
- **GitHub**: (In-Progress) Designed to handle profiles via OAuth Access Tokens.



### 💾 Persistence Logic
- **Repository Pattern**: Simplified data access layer for Users and Authentication records.
- **Unit of Work**: Ensures atomicity and consistency in database commits.

---

## 📡 API Endpoints

### 🔐 Authentication
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/User/Login/OAuth` | Login/Register via Google or GitHub |
| `POST` | `/User/SignIn` | Standard local user registration |
| `POST` | `/User/Login` | Standard username/password login |

### 📧 Password Management
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/User/{id}/UserAuthentications/Password` | Request a password reset link via Email |
| `POST` | `/User/ChangePassword/{jwt}` | Set or change password using the email token |

---

## 🚀 Getting Started

### ⚙️ Configuration
Update your `appsettings.json` or use **User Secrets** (Recommended for development):

```json
{
  "GoogleInformation": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  },
  "EmailInformation": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@gmail.com",
    "Password": "your-app-specific-password"
  }
}
