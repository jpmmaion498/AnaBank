# Configurações de Deploy - AnaBank

## Variáveis de Ambiente para Produção

### APIs
```bash
# Accounts API
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Data Source=/data/anabank_accounts.db
ConnectionStrings__Redis=redis:6379
Jwt__SecretKey=SUA_CHAVE_SECRETA_SUPER_SEGURA_AQUI
Jwt__Issuer=AnaBank
Jwt__Audience=AnaBank.APIs
Jwt__ExpirationHours=24

# Transfers API
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Data Source=/data/anabank_transfers.db
ConnectionStrings__Redis=redis:6379
Services__AccountsApi=http://accounts-api:8080
Jwt__SecretKey=SUA_CHAVE_SECRETA_SUPER_SEGURA_AQUI
Jwt__Issuer=AnaBank
Jwt__Audience=AnaBank.APIs
```

### Worker
```bash
# Fees Worker
ConnectionStrings__DefaultConnection=Data Source=/data/anabank_fees.db
ConnectionStrings__Kafka=kafka:9092
FeeSettings__TransferFeeAmount=2.00
```

### Secrets (Para Kubernetes/Azure)
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: anabank-secrets
type: Opaque
data:
  jwt-secret: <base64_encoded_secret>
  redis-password: <base64_encoded_password>
```

## HTTPS/SSL para Produção

### Nginx SSL
```bash
# Gerar certificados auto-assinados (desenvolvimento)
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout deploy/nginx/ssl/anabank.key \
  -out deploy/nginx/ssl/anabank.crt \
  -subj "/C=BR/ST=SP/L=SaoPaulo/O=AnaBank/CN=anabank.local"
```

### Let's Encrypt (produção)
```bash
# Usar certbot para certificados reais
certbot --nginx -d anabank.com -d api.anabank.com
```