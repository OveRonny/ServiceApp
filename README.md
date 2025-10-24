#  🚗 Service & Fuel Tracker (PWA + API)

A **Progressive Web App (PWA)** built with **Blazor WebAssembly** and a **.NET API** using **Entity Framework Core** and **Vertical Slice Architecture**.  
The app helps you track **services, parts, and fuel usage** for vehicles or equipment.  
Deployed on **Azure** for global access.  

## ✨ Features
- 🛠 Track maintenance services (oil changes, inspections, repairs)  
- 🔩 Manage parts inventory and costs  
- ⛽ Record fuel purchases & consumption history  
- 📊 Dashboard with history and stats  
- 🚘 **Vehicle info integration**: Fetch vehicle details (Eu control, etc.) from the **Statens vegvesen API** (Norway)  
- 📱 **PWA**: installable on desktop & mobile  
  > *(Offline support is limited to static assets. Data requires connectivity.)*  
- 🌐 Backend built with **Entity Framework Core + Minimal APIs + Vertical Slice Architecture**  
- ☁️ Hosted on **Azure App Service / Azure Static Web Apps**

- ## 🏗️ Architecture

This project uses **Vertical Slice Architecture** for organized, feature-based development. Each feature (slice) contains:
- Command & Query handlers  
- EF Core DbContext access scoped to the slice    
- API endpoints colocated with business logic  

Additional backend features:
- **Minimal APIs** 
- **Azure Storage**  
- **Statens vegvesen API** 

**Stack Overview:**
- Frontend: Blazor WebAssembly (PWA support)  
- Backend: ASP.NET Core Minimal APIs + EF Core  
- Database: SQL Server / Azure SQL  
- File storage: Azure Blob Storage  
- Deployment: Azure App Service  
- External API: Statens vegvesen (Norway)
