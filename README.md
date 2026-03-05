# MonitoringSystem

MonitoringSystem — локальная система мониторинга, состоящая из:
- **MonitoringBackend** (ASP.NET Core) — API + SignalR + раздача статического фронтенда (SPA)
- **PostgreSQL Portable** — база данных запускается локально из папки `postgres/` (без установки)
- **run-all.bat** — запуск всего проекта одной командой

Проект рассчитан на запуск на Windows как “из коробки”: скачал архив из **GitHub Releases**, распаковал, запустил `run-all.bat`.

---

## Возможности

- REST API для получения данных/метрик
- SignalR (`/metrics`) для обновлений в реальном времени
- Встроенная раздача фронтенда из `wwwroot` (без отдельного Node.js-сервера)
- Локальна�� PostgreSQL в комплекте (portable)

---

## Структура проекта

```text
MonitoringSystem/
  run-all.bat
  postgres/
    App/
    Data/
  backend/
    MonitoringSystem.Presentation.exe
    appsettings.json
    wwwroot/
      index.html
      static/...
```

> В репозитории также может быть папка `monitoring-frontend` — это **исходники фронтенда** (для разработки). Для запуска релиза она не нужна, т.к. фронтенд уже собран и лежит в `backend/wwwroot`.

---

## Требования

- Windows 10/11
- Права администратора **нужны только для backend** (при запуске появится UAC-запрос)
- PostgreSQL запускается **как обычный пользователь** (это требование PostgreSQL)

---

## Быстрый старт (из Release)

1. Скачай архив из **Releases**
2. Распакуй в любую папку (без кириллицы и без длинного пути желательно)
3. Запусти `run-all.bat`
4. Открой браузер: `http://127.0.0.1:5000/`

---

## Настройка подключения к базе

Настройки обычно лежат в:

- `backend/appsettings.json` (или `backend/appsettings.Production.json`)

Пример:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=5433;Database=monitoring;Username=postgres;Password=postgres"
  }
}
```

Обрати внимание:
- порт PostgreSQL по умолчанию в запуске: **5433**
- если меняешь порт/пользователя/пароль — меняй и строку подключения

---

## Частые проблемы

### PostgreSQL не стартует
PostgreSQL **нельзя** запускать от администратора. `run-all.bat` должен поднимать админ-права только для backend.

---

## Разработка

В репозитории может присутствовать:
- `src/MonitoringBackend` — backend (ASP.NET Core)
- `src/monitoring-frontend` — исходники фронтенда (React)

---

## Лицензия
Нету(
