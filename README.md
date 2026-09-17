
Antes de empezar, para ejecutar leer las indicaciones


```bash
docker-compose up --build

```

Servicios levantados:
- Frontend Angular: `http://localhost:4200`
- Backend API: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- SQL Server 2022: `localhost:1433` (Password: `YourStrong@Passw0rd2026!`)
Credenciales iniciales de prueba (Pre-sembradas):
- Usuario: `admin@rimac.com`
- Contraseña: `Admin1234!`

---

### Backend (xUnit)
```bash
cd backend
dotnet test TaskManager.sln --verbosity normal
```

### Frontend
```bash
cd frontend/task-manager-ui
npm install
npm run test:ci
```

---

## DevSecOps- formateo

- **Linters de**:
  ```bash
  dotnet format TaskManager.sln --verify-no-changes
  ```
- **Linters y Formato**:
  ```bash
  npm run lint
  npm run format
  ```
- **Vulnerabilidades**:
  ```bash
  dotnet list package --vulnerable
  npm audit
  ```
