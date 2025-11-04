# Deployment Guide

## Production Deployment

### Prerequisites
- Docker and Docker Compose installed
- Domain name (optional, for production)
- SSL certificate (for HTTPS)
- Environment variables configured

### Environment Configuration

Create a `.env` file or set environment variables:

```bash
# Database
POSTGRES_USER=your_db_user
POSTGRES_PASSWORD=your_secure_password
POSTGRES_DB=EmployeeManagementDb

# API
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET_KEY=your_very_secure_jwt_secret_key_minimum_32_characters
JWT_ISSUER=EmployeeManagement
JWT_AUDIENCE=EmployeeManagement
JWT_EXPIRATION_MINUTES=1440

# Connection String
CONNECTION_STRING=Host=postgres;Port=5432;Database=EmployeeManagementDb;Username=your_db_user;Password=your_secure_password
```

### Docker Compose Production

Create `docker-compose.prod.yml`:

```yaml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: ${POSTGRES_DB}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    restart: unless-stopped
    networks:
      - employee-management-network

  api:
    build:
      context: .
      dockerfile: EmployeeManagement.API/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
      - Jwt__SecretKey=${JWT_SECRET_KEY}
      - Jwt__Issuer=${JWT_ISSUER}
      - Jwt__Audience=${JWT_AUDIENCE}
      - Jwt__ExpirationMinutes=${JWT_EXPIRATION_MINUTES}
    depends_on:
      - postgres
    restart: unless-stopped
    networks:
      - employee-management-network
    ports:
      - "8080:8080"

  frontend:
    build:
      context: ./employee-management-frontend
      dockerfile: Dockerfile
    environment:
      - REACT_APP_API_URL=http://your-api-domain.com/api
    restart: unless-stopped
    networks:
      - employee-management-network
    ports:
      - "80:80"

volumes:
  postgres_data:

networks:
  employee-management-network:
    driver: bridge
```

### Deployment Steps

1. **Build and start services:**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
   ```

2. **Run database migrations:**
   ```bash
   docker-compose exec api dotnet ef database update --project /src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj --startup-project /src/EmployeeManagement.API/EmployeeManagement.API.csproj
   ```

3. **Verify deployment:**
   ```bash
   curl http://localhost:8080/health
   ```

### Production Checklist

- [ ] Change JWT secret key to a secure random value
- [ ] Use strong database passwords
- [ ] Enable HTTPS (use reverse proxy like Nginx)
- [ ] Configure CORS for production domain
- [ ] Set up logging and monitoring
- [ ] Configure backup strategy for PostgreSQL
- [ ] Set up SSL certificates
- [ ] Configure firewall rules
- [ ] Set up rate limiting
- [ ] Disable Swagger UI in production
- [ ] Configure health checks
- [ ] Set up automated backups

### Reverse Proxy Setup (Nginx)

Example Nginx configuration:

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:3000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    location /api {
        proxy_pass http://localhost:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

### Monitoring

Consider setting up:
- Application Insights or similar
- Log aggregation (ELK stack, Splunk, etc.)
- Health check monitoring
- Database performance monitoring

### Backup Strategy

```bash
# Backup database
docker-compose exec postgres pg_dump -U postgres EmployeeManagementDb > backup_$(date +%Y%m%d_%H%M%S).sql

# Restore database
docker-compose exec -T postgres psql -U postgres EmployeeManagementDb < backup.sql
```

### Scaling

For horizontal scaling:
- Use load balancer for API
- Use read replicas for PostgreSQL
- Use CDN for frontend static files
- Consider container orchestration (Kubernetes, Docker Swarm)

