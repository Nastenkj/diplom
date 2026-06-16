# preg-app

## Локальная отладка (docker-compose)

### Запуск
```bash
docker compose up --build
```

### Адреса
- **Frontend / приложение (Angular + nginx)**: `http://localhost/`  
  (в compose проброшен `80:80`)
- **Kafka UI**: `http://localhost:8081`  
  (в compose проброшен `8081:8080`)
- **Kafka (broker)**: `localhost:9092`  
  (`PLAINTEXT_HOST://localhost:9092`)

## Тестовые аккаунты (seed в миграции)
Seed-данные создаются в: `PregnancyAppBackend/Migrations/20250302185320_Init.cs`.

Пароль для всех тестовых пользователей: **`123`**.

### Администратор
- `admin@example.com`

### Доктора
- `doctor1@example.com`
- `doctor2@example.com`

### Пациенты
- `patient1@example.com`
- `patient2@example.com`
- `patient3@example.com`
- `patient4@example.com`
- `patient5@example.com`
- `patient6@example.com`
